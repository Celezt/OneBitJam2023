using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISpawner : MonoBehaviour
{
	[SerializeField] Transform[] enemies;
	[SerializeField] Transform[] spawnPoints;
	[SerializeField] float minSpawnRadius = 3;
	[SerializeField] float maxSpawnRadius = 10;
	[SerializeField] bool requireLineOfSight = false;
	[SerializeField] float minSpawnDelay = 2;
	[SerializeField] float maxSpawnDelay = 3.5f;
	[SerializeField] int minSpawnAmount = 1;
	[SerializeField] int maxSpawnAmount = 4;
	[SerializeField] int maxAliveAI = 8;

	private List<Transform> currentlyAliveAI;

	private float nextSpawnAttempt;

	void Awake()
	{
		if (enemies.Length == 0)
		{
#if UNITY_EDITOR
			Debug.LogError($"No enemies selected for AI spawner to spawn!");
#endif
			return;
		}

		currentlyAliveAI = new List<Transform>();
		nextSpawnAttempt = Time.time + Random.Range(minSpawnDelay, maxSpawnDelay);

		if (spawnPoints.Length == 0)
		{
#if UNITY_EDITOR
			Debug.LogError($"No spawn points selected for AI spawner {name} to spawn enemies at, defaulting to AI spawners position!");
#endif
			spawnPoints = new Transform[] { transform };
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (enemies.Length == 0)
			return;

		if (Time.time > nextSpawnAttempt)
		{
			SpawnAI();
			nextSpawnAttempt = Time.time + Random.Range(minSpawnDelay, maxSpawnDelay);
		}
	}

	void SpawnAI()
	{
		int numNewAI = Random.Range(minSpawnAmount, maxSpawnAmount);
		for (int i = 0; i < numNewAI; i++)
		{
			if (AliveAILimitReached())
				return;

			Vector3 spawnPointPosition = GetSpawnPoint().position;

			Vector3 samplePosition = spawnPointPosition + Vector3.up + (new Vector3(Random.insideUnitCircle.x, 0, Random.insideUnitCircle.y) * Random.Range(minSpawnRadius, maxSpawnRadius));

			for (int j = 0; j < 10; j++)
			{
				bool validSpawn;
				if (!requireLineOfSight)
					validSpawn = VerifySpawnPosition(samplePosition);
				else
					validSpawn = VerifySpawnPosition(samplePosition) && IsPositionVisible(samplePosition);

				if (validSpawn)
					break;

				samplePosition = spawnPointPosition + Vector3.up + (new Vector3(Random.insideUnitCircle.x, 0, Random.insideUnitCircle.y) * Random.Range(minSpawnRadius, maxSpawnRadius));
			}

			Transform ai = enemies[Random.Range(0, enemies.Length)];

			currentlyAliveAI.Add(Instantiate(ai, samplePosition, Quaternion.LookRotation(Random.insideUnitSphere, Vector3.up)));
		}
	}

	bool AliveAILimitReached() => currentlyAliveAI.Count < maxAliveAI;

	bool IsPositionVisible(Vector3 samplePosition)
	{
		return !Physics.Linecast(transform.position, samplePosition);
	}

	bool VerifySpawnPosition(Vector3 samplePosition)
	{
		bool hitObject = Physics.SphereCast(samplePosition + Vector3.up * 2, 1.2f, Vector3.down, out RaycastHit hit, 5);
		return hitObject && (hit.transform.tag != "Enemy" && hit.transform.tag != "Player");
	}

	Transform GetSpawnPoint() => spawnPoints[Random.Range(0, spawnPoints.Length)];
}