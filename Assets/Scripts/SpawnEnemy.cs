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
        int t = 0;
        foreach (SpawnEnemyEntity entity in spawnEntityList)
        {
            t += entity.spawnRate;
        }

        int res = Random.Range(1, t+1);
        t = 0;
        GameObject retGo = new GameObject();
        
        foreach (SpawnEnemyEntity entity in spawnEntityList)
        {
            t += entity.spawnRate;
            if (res <= t)
            {
                retGo = entity.enemyPrefab;
                break;
            }
        }

        bool pass = true;
        List<int> nbList = new List<int>();

        while(pass)
        {
            int r = Random.Range(0, spawnPosition.Length - 1);

            while (nbList.Contains(r))
            {
                r = Random.Range(0, spawnPosition.Length - 1);
            }
            nbList.Add(r);

            if(nbList.Count >= spawnPosition.Length)
            {
                return;
            }
            
            ArenaTile tile = spawnPosition[r];

            if(tile.GetGameObjectOnTop() == null)
            {
                Vector3 pos = new Vector3(tile.transform.position.x, retGo.transform.position.y, tile.transform.position.z);
                Instantiate(retGo, pos, quaternion.identity);
                pass = false;
            }
        }
    }

    private void SpawnEntities(int nb)
    {
        for (int i = 0; i < nb; i++)
        {
            SpawnEntity();
        }
    }

    public void LaunchSpawn()
    {
        int nb = Random.Range(spawnNb.Min, spawnNb.Max + 1);
        
        SpawnEntities(nb);
    }
}
