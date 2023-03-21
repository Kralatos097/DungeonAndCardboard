using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TacticsMovement : MonoBehaviour
{
	protected bool Turn = false;
	public static bool PlayersTurn = false;

	protected List<ArenaTile> _selectableTiles = new List<ArenaTile>();
	private GameObject[] _tiles;

	protected Stack<ArenaTile> _path = new Stack<ArenaTile>();
	protected ArenaTile _currentTile;

	protected bool moving = false;
	protected bool attacking = false;
	protected int baseMove;
	protected int move;
	public float moveSpeed = 2;
	
	private float uiOverScaleFactor = 1.25f;

	[HideInInspector] public int atkRange = 0;

	protected float MoveY = .5f;
	protected bool passM = false;

	private Vector3 velocity = new Vector3();
	private Vector3 heading = new Vector3();

	private float halfHeight = 0;

	protected GameObject target;
	protected int _targetDistance = 0;
	protected ArenaTile ActualTargetTile;

	protected Active ActiveOne;
	protected int ActiveOneCd = 0;

	protected Active ActiveTwo;
	protected int ActiveTwoCd = 0;

	protected Consumable Consumable;

	private Passive _passive;

	protected Passive Passive
	{
		get => _passive;

		set
		{
			if (_passive != null && _passive.GetPassiveTrigger() == PassiveTrigger.OnObtained)
			{
				_passive.ReverseEffect(gameObject);
			}

			_passive = value;

			if (_passive != null && _passive.GetPassiveTrigger() == PassiveTrigger.OnObtained)
			{
				_passive.Effect(gameObject);
			}
		}
	}

	private Material _unitMat;
	private Color _baseColor;
	private Color _changeColor;

	protected CombatStat CombatStat;

	protected void Init()
	{
		_tiles = GameObject.FindGameObjectsWithTag("Tile");

		CombatStat = gameObject.GetComponent<CombatStat>();

		GetUnitInfo();
		if (!CombatStat.isAlive) return;
		SetCurrentTile();

		halfHeight = GetComponent<Collider>().bounds.extents.y;

		CombatStat.RollInit();


		if (!TurnManager.CombatStarted)
		{
			TurnManager.AddUnit(this);
		}
		else
		{
			TurnManager.AddUnitToQueue(this);
		}
	}

	protected virtual void GetUnitInfo()
	{
	}

	public ArenaTile GetCurrentTile()
	{
		return GetTargetTile(gameObject);
	}

	protected void SetCurrentTile()
	{
		_currentTile = GetTargetTile(gameObject);
		_currentTile.current = true;
	}

	protected ArenaTile GetTargetTile(GameObject target)
	{
		RaycastHit hit;
		ArenaTile tile = null;

		if (Physics.Raycast(target.transform.position, Vector3.down, out hit, 2))
		{
			tile = hit.collider.GetComponent<ArenaTile>();
		}

		return tile;
	}

	protected void ComputeAdjacencyList()
	{
		foreach (GameObject tile in _tiles)
		{
			ArenaTile t = tile.GetComponent<ArenaTile>();
			t.FindNeighbors(null);
		}
	}

	protected void ComputeAdjacencyListAtk()
	{
		foreach (GameObject tile in _tiles)
		{
			ArenaTile t = tile.GetComponent<ArenaTile>();
			t.FindNeighborsAtk();
		}
	}

	protected void ComputeAdjacencyList(ArenaTile target)
	{
		foreach (GameObject tile in _tiles)
		{
			ArenaTile t = tile.GetComponent<ArenaTile>();
			t.FindNeighbors(target);
		}
	}

	protected void FindSelectableTile()
	{
		ComputeAdjacencyList();
		SetCurrentTile();

		Queue<ArenaTile> process = new Queue<ArenaTile>();

		process.Enqueue(_currentTile);
		_currentTile.visited = true;

		while (process.Count > 0)
		{
			ArenaTile t = process.Dequeue();

			_selectableTiles.Add(t);
			t.selectable = true;

			if (t.distance < move)
			{
				foreach (ArenaTile tile in t.adjacencyList)
				{
					if (!tile.visited)
					{
						tile.parent = t;
						tile.visited = true;
						tile.distance = 1 + t.distance;
						process.Enqueue(tile);
					}
				}
			}
		}
	}

	protected void MoveToTile(ArenaTile tile)
	{
		_path.Clear();
		tile.target = true;
		moving = true;

		ArenaTile next = tile;
		while (next != null)
		{
			_path.Push(next);
			next = next.parent;
		}
	}

	protected void Move()
	{
		if (_path.Count > 0)
		{
			ArenaTile t = _path.Peek();
			Vector3 target = t.transform.position;

			/*target.y += halfHeight + t.GetComponent<Collider>().bounds.extents.y;*/
			target.y = transform.position.y;

			if (Vector3.Distance(transform.position, target) >= 0.05f)
			{
				CalculateHeading(target);
				SetHorizontalVelocity();

				transform.forward = heading;
				transform.position += velocity * Time.deltaTime;
			}
			else
			{
				//tile center reached
				transform.position = target;
				_path.Pop();
			}
		}
		else
		{
			RemoveSelectableTile();

			EndOfMovement();
		}
	}

	private void SetHorizontalVelocity()
	{
		velocity = heading * moveSpeed;
	}

	private void CalculateHeading(Vector3 target)
	{
		heading = target - transform.position;
		heading.Normalize();
	}

	protected void RemoveSelectableTile()
	{
		if (_currentTile != null)
		{
			_currentTile.current = false;
			_currentTile = null;
		}

		foreach (ArenaTile tile in _selectableTiles)
		{
			tile.Reset();
		}

		_selectableTiles.Clear();
	}

	public void BeginTurn()
	{
		StartTurnFx();
	}

	public void EndTurn()
	{
		Turn = false;
		EndTurnResetValues();
	}

	protected ArenaTile FindEndTile(ArenaTile t)
	{
		Stack<ArenaTile> tempPath = new Stack<ArenaTile>();

		ArenaTile next = t.parent;
		while (next != null)
		{
			tempPath.Push(next);
			next = next.parent;
		}

		if (tempPath.Count <= move)
		{
			return t.parent;
		}

		ArenaTile endTile = null;

		for (int i = 0; i <= move; i++)
		{
			endTile = tempPath.Pop();
		}

		return endTile;
	}

	protected bool FindPathFull(ArenaTile targetTile)
	{
		ComputeAdjacencyList(targetTile);
		SetCurrentTile();

		List<ArenaTile> openList = new List<ArenaTile>();
		List<ArenaTile> closedList = new List<ArenaTile>();

		openList.Add(_currentTile);
		_currentTile.h = Vector3.Distance(_currentTile.transform.position, targetTile.transform.position);
		_currentTile.f = _currentTile.h;

		while (openList.Count > 0)
		{
			ArenaTile t = FindLowestF(openList);

			closedList.Add(t);

			if (t == targetTile)
			{
				ActualTargetTile = FindEndTile(t);
				return true;
			}

			foreach (ArenaTile tile in t.adjacencyList)
			{
				if (closedList.Contains(tile))
				{
					//Do nothing, already processed
				}
				else if (openList.Contains(tile))
				{
					float tempG = t.g + Vector3.Distance(tile.transform.position, t.transform.position);

					if (tempG < tile.g)
					{
						tile.parent = t;
						tile.g = tempG;
						tile.f = tile.g + tile.h;
					}
				}
				else
				{
					tile.parent = t;

					tile.g = t.g + Vector3.Distance(tile.transform.position, t.transform.position);
					tile.h = Vector3.Distance(tile.transform.position, targetTile.transform.position);

					tile.f = tile.g + tile.h;

					openList.Add(tile);
				}
			}
		}

		return false;
	}

	protected bool FindPathWoTrap(ArenaTile targetTile)
	{
		ComputeAdjacencyListAtk();
		SetCurrentTile();

		List<ArenaTile> openList = new List<ArenaTile>();
		List<ArenaTile> closedList = new List<ArenaTile>();

		openList.Add(_currentTile);
		_currentTile.h = Vector3.Distance(_currentTile.transform.position, targetTile.transform.position);
		_currentTile.f = _currentTile.h;

		while (openList.Count > 0)
		{
			ArenaTile t = FindLowestF(openList);

			closedList.Add(t);

			if (t == targetTile)
			{
				ArenaTile obstTile = GetFirstObstacleOnPath(t);

				ActualTargetTile = FindEndTile(obstTile == null ? t : obstTile);
				return true;
			}

			foreach (ArenaTile tile in t.adjacencyList)
			{
				GameObject tileGo = tile.GetGameObjectOnTop();
				if (tileGo == null || tileGo.CompareTag("Trap") || tileGo.CompareTag(target.tag))
				{
					if (closedList.Contains(tile))
					{
						//Do nothing, already processed
					}
					else if (openList.Contains(tile))
					{
						float tempG = t.g + Vector3.Distance(tile.transform.position, t.transform.position);

						if (tempG < tile.g)
						{
							tile.parent = t;
							tile.g = tempG;
							tile.f = tile.g + tile.h;
						}
					}
					else
					{
						tile.parent = t;

						tile.g = t.g + Vector3.Distance(tile.transform.position, t.transform.position);
						tile.h = Vector3.Distance(tile.transform.position, targetTile.transform.position);

						tile.f = tile.g + tile.h;

						openList.Add(tile);
					}
				}
			}
		}

		//Debug.Log("Path not Found");
		return false;
	}

	protected bool FindPathWoCrate(ArenaTile targetTile)
	{
		ComputeAdjacencyListAtk();
		SetCurrentTile();

		List<ArenaTile> openList = new List<ArenaTile>();
		List<ArenaTile> closedList = new List<ArenaTile>();

		openList.Add(_currentTile);
		_currentTile.h = Vector3.Distance(_currentTile.transform.position, targetTile.transform.position);
		_currentTile.f = _currentTile.h;

		while (openList.Count > 0)
		{
			ArenaTile t = FindLowestF(openList);

			closedList.Add(t);

			if (t == targetTile)
			{
				ArenaTile crateTile = GetFirstCrateOnPath(t);
				target = crateTile.GetGameObjectOnTop();
				_targetDistance = crateTile.distance;
				ActualTargetTile = FindEndTile(crateTile);
				return true;
			}
			if(t.GetGameObjectOnTop() != null && t.GetGameObjectOnTop().CompareTag(target.tag))
			{
			    continue;
			}

			foreach (ArenaTile tile in t.adjacencyList)
			{
				GameObject tileGo = tile.GetGameObjectOnTop();
				if (tileGo == null || tileGo.CompareTag("Trap") || tileGo.CompareTag("Crate") ||
				    tileGo.CompareTag(target.tag))
				{
					if (closedList.Contains(tile))
					{
						//Do nothing, already processed
					}
					else if (openList.Contains(tile))
					{
						float tempG = t.g + Vector3.Distance(tile.transform.position, t.transform.position);

						if (tempG < tile.g)
						{
							tile.parent = t;
							tile.g = tempG;
							tile.f = tile.g + tile.h;
						}
					}
					else
					{
						tile.parent = t;

						tile.g = t.g + Vector3.Distance(tile.transform.position, t.transform.position);
						tile.h = Vector3.Distance(tile.transform.position, targetTile.transform.position);

						tile.f = tile.g + tile.h;

						openList.Add(tile);
					}
				}
			}
		}

		return false;
	}

	protected bool FindPathWoAll(ArenaTile targetTile) //A Faire
	{
		ComputeAdjacencyListAtk();
		SetCurrentTile();

		List<ArenaTile> openList = new List<ArenaTile>();
		List<ArenaTile> closedList = new List<ArenaTile>();

		openList.Add(_currentTile);
		_currentTile.h = Vector3.Distance(_currentTile.transform.position, targetTile.transform.position);
		_currentTile.f = _currentTile.h;

		while (openList.Count > 0)
		{
			ArenaTile t = FindLowestF(openList);

			closedList.Add(t);

			if (t == targetTile)
			{
				ArenaTile obstTile = GetFirstObstacleOnPath(t);
				ActualTargetTile = FindEndTile(obstTile);

				if(ActualTargetTile.GetGameObjectOnTop().CompareTag("Player"))
				{
				    target = ActualTargetTile.GetGameObjectOnTop();
				    _targetDistance = ActualTargetTile.distance;
				}

				return true;
			}

			foreach (ArenaTile tile in t.adjacencyList)
			{
				if (closedList.Contains(tile))
				{
					//Do nothing, already processed
				}
				else if (openList.Contains(tile))
				{
					float tempG = t.g + Vector3.Distance(tile.transform.position, t.transform.position);

					if (tempG < tile.g)
					{
						tile.parent = t;
						tile.g = tempG;
						tile.f = tile.g + tile.h;
					}
				}
				else
				{
					tile.parent = t;

					tile.g = t.g + Vector3.Distance(tile.transform.position, t.transform.position);
					tile.h = Vector3.Distance(tile.transform.position, targetTile.transform.position);

					tile.f = tile.g + tile.h;

					openList.Add(tile);
				}
			}
		}

		return false;
	}

	protected ArenaTile FindLowestF(List<ArenaTile> list)
	{
		ArenaTile lowest = list[0];

		foreach (ArenaTile t in list)
		{
			if (t.f < lowest.f)
			{
				lowest = t;
			}
		}

		list.Remove(lowest);

		return lowest;
	}

	protected ArenaTile GetFirstCrateOnPath(ArenaTile target)
	{
		int n = 0;
		ArenaTile firstCrate = null;
		while (target.parent != null)
		{
			if (target.GetGameObjectOnTop() != null && target.GetGameObjectOnTop().CompareTag("Crate"))
			{
				n = 0;
				firstCrate = target;
			}

			n += 1;

			target = target.parent;
		}

		//_targetDistance = n;
		firstCrate.distance = n;
		return firstCrate;
	}


	private ArenaTile GetFirstObstacleOnPath(ArenaTile target)
	{
		ArenaTile firstObst = null;
		while (target.parent != null)
		{
			if (target.GetGameObjectOnTop() != null)
			{
				firstObst = target;
			}

			target = target.parent;
		}

		return firstObst;
	}

	protected virtual void EndOfMovement()
	{
		//Fin du Soulevement du pion lors du mouvement
		transform.GetChild(0).Translate(0, -MoveY, 0);
		EndOfMovementFX();
		passM = false;
		moving = false;
	}

	protected void EndOfMovementFX()
	{
		FindObjectOfType<FXManager>().Play("DropFigurine", transform);
	}

	protected GameObject AlliesInAttackRange()
	{
		ComputeAdjacencyListAtk();
		SetCurrentTile();

		Queue<ArenaTile> process = new Queue<ArenaTile>();

		process.Enqueue(_currentTile);
		_currentTile.visited = true;

		while (process.Count > 0)
		{
			ArenaTile t = process.Dequeue();

			_selectableTiles.Add(t);

			if (t.distance < atkRange)
			{
				foreach (ArenaTile tile in t.adjacencyList)
				{
					if (!tile.visited)
					{
						tile.parent = t;
						tile.visited = true;
						tile.distance = 1 + t.distance;
						process.Enqueue(tile);

						GameObject TGO = tile.GetGameObjectOnTop();
						if (TGO != null)
						{
							if (TGO.CompareTag("Player"))
							{
								return TGO;
							}
						}
					}
				}
			}
		}

		return null;
	}

	protected void AffAttackRange()
	{
		ComputeAdjacencyListAtk();
		SetCurrentTile();
		ActiveTarget activeTarget = GetSelectedActive().GetActiveTarget();

		Queue<ArenaTile> process = new Queue<ArenaTile>();

		process.Enqueue(_currentTile);
		_currentTile.visited = true;

		while (process.Count > 0)
		{
			ArenaTile t = process.Dequeue();

			_selectableTiles.Add(t);
			t.selectable = true;

			if (activeTarget == ActiveTarget.SelfOnly) return;
			if (t.distance < atkRange)
			{
				foreach (ArenaTile tile in t.adjacencyList)
				{
					if (!tile.visited)
					{
						tile.parent = t;
						tile.visited = true;
						tile.distance = 1 + t.distance;
						process.Enqueue(tile);
					}
				}
			}
		}

		if (activeTarget == ActiveTarget.OthersOnly)
			_currentTile.selectable = false;
	}

	protected virtual Active GetSelectedActive()
	{
		return ActiveOne;
	}

	protected void Attack(GameObject target, int equip)
	{
		RemoveSelectableTile();
		int hit = GetHitChance();

		if (Passive != null)
		{
			if (Passive.GetPassiveTrigger() == PassiveTrigger.OnAttack)
			{
				hit = Passive.Effect(gameObject, hit);
			}
		}

		switch (equip)
		{
			case 1:
				ActiveOne.Effect(gameObject, target, hit);
				ActiveOneCd = ActiveOne.GetCd() + 1;
				break;
			case 2:
				ActiveTwo.Effect(gameObject, target, hit);
				ActiveTwoCd = ActiveTwo.GetCd() + 1;
				break;
			case 3:
				Consumable.Effect(gameObject, target, hit);
				Consumable = null;
				break;
			default:
				break;
		}

		//Debug.Log("ATTACKING " + target.gameObject.name + "!\n Now has : " + target.GetComponent<CombatStat>().CurrHp + " HP!");
		EndOfAttack();
	}

	//return 0 for a miss, 1 for a hit, 2 for a critical
	private int GetHitChance()
	{
		int nb = Random.Range(1, 7);
		return nb switch
		{
			1 => 0,
			6 => 2,
			_ => 1
		};
	}

	protected void TurnToTarget(GameObject target)
	{
		if (Mathf.Abs(Mathf.Abs(transform.position.x) - Mathf.Abs(target.transform.position.x)) >
		    Mathf.Abs(Mathf.Abs(transform.position.y) - Mathf.Abs(target.transform.position.y)))
		{
			if (transform.position.x > target.transform.position.x)
			{
				//turn left
				transform.rotation = Quaternion.Euler(0, -90, 0);
			}
			else
			{
				//turn right
				transform.rotation = Quaternion.Euler(0, 90, 0);
			}
		}
		else if (Mathf.Abs(Mathf.Abs(transform.position.x) - Mathf.Abs(target.transform.position.x)) <
		         Mathf.Abs(Mathf.Abs(transform.position.z) - Mathf.Abs(target.transform.position.z)))
		{
			if (transform.position.z > target.transform.position.z)
			{
				//turn down
				transform.rotation = Quaternion.Euler(0, 180, 0);
			}
			else
			{
				//turn up
				transform.rotation = Quaternion.Euler(0, 0, 0);
			}
		}
	}

	protected virtual void EndOfAttack()
	{
		RemoveSelectableTile();
	}

	public void EquipCDMinus(int value)
	{
		if (ActiveOne != null) ActiveOneCd -= value;
		if (ActiveTwo != null) ActiveTwoCd -= value;

		if (ActiveOneCd < 0) ActiveOneCd = 0;
		if (ActiveTwoCd < 0) ActiveTwoCd = 0;
	}

	private void StartTurnFx()
	{
		if (gameObject.CompareTag("Player"))
		{
			FindObjectOfType<AudioManager>().RandomPitch("AllieStartTurn");
		}
		else
		{
			FindObjectOfType<AudioManager>().RandomPitch("EnemyStartTurn");
		}

		_unitMat = transform.GetChild(0).GetComponent<Renderer>().material;

		_baseColor = _unitMat.color;
		_changeColor = new Color(1f, .5f, .5f);

		float timing = 0.3f;
		ColorClign(timing);
		Invoke("TrueBeginTurn", timing * 3);
	}

	public void DamageClign()
	{
		_unitMat = transform.GetChild(0).GetComponent<Renderer>().material;

		_baseColor = _unitMat.color;
		_changeColor = new Color(1, 1, 1, .5f);

		float timing = 0.2f;
		ColorClign(timing);
	}

	private void ColorClign(float t)
	{
		ChangeColorChange();
		Invoke("ChangeColorBase", t);
		Invoke("ChangeColorChange", t * 2);
		Invoke("ChangeColorBase", t * 3);
	}

	protected void TrueBeginTurn()
	{
		Turn = true;
	}

	protected void ChangeColorBase()
	{
		_unitMat.color = _baseColor;
	}

	protected void ChangeColorChange()
	{
		_unitMat.color = _changeColor;
	}

	public Passive GetPassive()
	{
		return Passive;
	}

	public void SetMove(int value)
	{
		move = baseMove + value;
	}

	public void ChangeMove(int value)
	{
		move += value;
		if (move < 0) move = 0;
	}

	private void EndTurnResetValues()
	{
		target = null;
		ActualTargetTile = null;
		_selectableTiles.Clear();
		_currentTile = null;
	}

	public void ShowUnit(bool value)
	{
		if (value) //Activating
		{
			transform.GetChild(0).localScale *= uiOverScaleFactor;
		}
		else
		{
			transform.GetChild(0).localScale /= uiOverScaleFactor;
		}
	}
}