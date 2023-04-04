using System.Collections;
using System.Collections.Generic;
using MyBox;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnEnemy : MonoBehaviour
{
    [MinMaxRange(0,10)]
    [SerializeField] private RangedInt spawnNb;

    [SerializeField] private SpawnEnemyEntity[] spawnEntityList;
    
    [SerializeField] private ArenaTile[] spawnPosition;
    

    private void SpawnEntity()
    {
        Debug.Log("ddd");
        int t = 0;
        foreach (SpawnEnemyEntity entity in spawnEntityList)
        {
            t += entity.spawnRate;
        }
        
        int res = Random.Range(1, t+1);
        
        GameObject retGo = SelectPrefab(res);
        if (retGo == null)
            Debug.LogWarning("No Prefab Selected");

        bool pass = true;
        List<int> nbList = new List<int>();

        while(pass)
        {
            int r = Random.Range(0, spawnPosition.Length);

            int n = 0;
            while(nbList.Contains(r))
            {
                r = Random.Range(0, spawnPosition.Length);
                if(n > spawnPosition.Length)
                {
                    return;
                }
                n++;
            }
            nbList.Add(r);

            ArenaTile tile = spawnPosition[r];

            if(tile.GetGameObjectOnTop() == null)
            {
                Debug.Log("hhhh");
                Vector3 pos = new Vector3(tile.transform.position.x, retGo.transform.position.y, tile.transform.position.z);
                Instantiate(retGo, pos, quaternion.identity);
                pass = false;
            }
        }
    }

    private GameObject SelectPrefab(int res)
    {
        Debug.Log("gjjjj");
        int t = 0;
        foreach (SpawnEnemyEntity entity in spawnEntityList)
        {
            t += entity.spawnRate;
            if (res <= t)
            {
                return entity.enemyPrefab;
            }
        }
        return null;
    }

    private void SpawnEntities(int nb)
    {
        for (int i = 0; i < nb; i++)
        {
            Debug.Log("nb : " + nb);
            SpawnEntity();
        }
    }

    public void LaunchSpawn()
    {
        int nb = Random.Range(spawnNb.Min, spawnNb.Max + 1);
        
        Debug.Log("gg nb : " + nb);
        SpawnEntities(nb);
    }
}
