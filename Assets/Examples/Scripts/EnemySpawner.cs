using System.Collections;
using System.Collections.Generic;
using RollAnything;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float _spawnInterval;

    public RollTable spawnPositions;


    public RollTable spawnableMinions;


    public IEnumerator Spawn()
    {
        yield return new WaitForSeconds(_spawnInterval);
        GameObject spawnPosition = (GameObject)spawnPositions.Roll().MyObject;
        GameObject spawnMonster = (GameObject)spawnableMinions.Roll().MyObject;
        Instantiate(spawnMonster, spawnPosition.transform.position, spawnPosition.transform.rotation);

    }
}