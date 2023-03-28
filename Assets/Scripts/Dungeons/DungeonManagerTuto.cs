using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManagerTuto : DungeonManager
{
	private int _stuffCnt = 0;
	private int StuffCnt
	{
		get => _stuffCnt;

		set
		{
			_stuffCnt = value;

			if(_stuffCnt >= stuffs.Count)
			{
				_stuffCnt = 0;
			}
		}
	}

	[SerializeField] private List<Stuff> stuffs;

	protected override void LaunchTreasure()
	{
		artworkShown = true;
		PositiveLootFX();

		Stuff stuff = stuffs[StuffCnt];
		string type = stuff.stuffType;
		Debug.Log(StuffCnt + " - " + stuff);
		
		if(type == "Consumable")
		{
			DungeonUiManager.TreasureConsumableUi();
			treasureEffect = TreasureEffect.Consumable;
		}
		else
		{
			DungeonUiManager.TreasureStuffUi();
			treasureEffect = TreasureEffect.Stuff;
		}
	}
    
	protected override void LaunchFightLoot()
	{
		artworkShown = true;
		PositiveLootFX();

		Stuff stuff = stuffs[StuffCnt];
		string type = stuff.stuffType;
		Debug.Log(StuffCnt + " - " + stuff);
		
		if(type == "Consumable")
		{
			DungeonUiManager.TreasureConsumableUi();
			treasureEffect = TreasureEffect.Consumable;
		}
		else
		{
			DungeonUiManager.TreasureStuffUi();
			treasureEffect = TreasureEffect.Stuff;
		}
	}

	protected override Stuff PickStuff()
	{
		Stuff stuff = stuffs[StuffCnt];
		StuffCnt++;
		return stuff;
	}

	protected override Consumable PickConsumable()
	{
		Consumable stuff = (Consumable) stuffs[StuffCnt];
		StuffCnt++;
		return stuff;
	}
}