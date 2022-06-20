using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HordesController : MonoBehaviour
{
    public EnemySO bigEnemy;
    public EnemySO smallEnemy;
    public Transform spawnPoint;

    public float hordeInitialDelay;
    public float minDelay;
    public float delayDecreasePerRound;
    public int enemiesAmount;
    public int maxBigEnemies;
    public int maxSmallEnemies;

    int currentRound;

    void Start()
    {
        SpawnCurrentHorde();
        StartCoroutine(HordeSpawnCoroutine());
    }

    IEnumerator HordeSpawnCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Mathf.Max(minDelay,
            hordeInitialDelay - (currentRound * delayDecreasePerRound)));
            SpawnCurrentHorde();
            currentRound++;
        }
    }

    void SpawnCurrentHorde()
    {
        int minBigs = Mathf.Max(0,
            enemiesAmount - maxSmallEnemies);

        int amountOfBigs;
        if (minBigs >= maxBigEnemies)
            amountOfBigs = minBigs;
        else
            amountOfBigs = Random.Range(minBigs, maxBigEnemies + 1);

        int amountOfSmalls = enemiesAmount - amountOfBigs;

        for (int i = 0; i < amountOfBigs; i++)
            Instantiate(bigEnemy.prefab, spawnPoint.position, Quaternion.identity);
        for (int i = 0; i < amountOfSmalls; i++)
            Instantiate(smallEnemy.prefab, spawnPoint.position, Quaternion.identity);
    }
}
