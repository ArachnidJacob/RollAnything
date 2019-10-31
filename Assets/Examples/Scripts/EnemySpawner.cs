using System.Collections;
using System.Collections.Generic;
using RollAnything;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public RollTable spawnPositions;

    public RollTable spawnableMinions;


    public void Spawn()
    {
      
        GameObject spawnPosition = (GameObject)spawnPositions.Roll().MyObject;
        GameObject spawnMonster = (GameObject)spawnableMinions.Roll().MyObject;
        Instantiate(spawnMonster, spawnPosition.transform.position, spawnPosition.transform.rotation);

    }
}