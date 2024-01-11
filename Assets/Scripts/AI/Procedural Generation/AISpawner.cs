using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISpawner : MonoBehaviour
{
	[SerializeField] AIController[] enemies;
	[SerializeField] Transform[] spawnPoints;
	[SerializeField] float minSpawnRadius = 3;
	[SerializeField] float maxSpawnRadius = 10;
	[SerializeField] bool requireLineOfSight = false;
	[SerializeField] int minSpawnAmount = 1;
	[SerializeField] int maxSpawnAmount = 4;
	[SerializeField] int maxAliveAI = 8;

	private List<AIController> currentlyAliveAI;
	private DungeonRoom room;

	public delegate void OnRoomClearedEvent(DungeonRoom room);
	public event OnRoomClearedEvent OnRoomCleared;

	void Awake()
	{
		if (enemies.Length == 0)
		{
#if UNITY_EDITOR
			Debug.LogError($"No enemies selected for AI spawner to spawn!");
#endif
			return;
		}

		currentlyAliveAI = new List<AIController>();

		if (spawnPoints.Length == 0)
		{
#if UNITY_EDITOR
			Debug.LogError($"No spawn points selected for AI spawner {name} to spawn enemies at, defaulting to AI spawners position!");
#endif
			spawnPoints = new Transform[] { transform };
		}

		if (transform.parent.TryGetComponent(out DungeonRoom dungeonRoom))
			room = dungeonRoom;

		SpawnAI();
	}

	public void SpawnAI()
	{
		int numNewAI = Random.Range(minSpawnAmount, maxSpawnAmount);
		for (int i = 0; i < numNewAI; i++)
		{
			if (AliveAILimitReached())
				return;

			Vector3 spawnPointPosition = GetSpawnPoint().position;

			Vector3 randomDirection = Random.insideUnitSphere;

			Vector3 samplePosition = spawnPointPosition + Vector3.up + (new Vector3(randomDirection.x, 0, randomDirection.y) * Random.Range(minSpawnRadius, maxSpawnRadius));

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

			AIController aiToSpawn = enemies[Random.Range(0, enemies.Length)];

			AIController spawnedController = Instantiate(aiToSpawn, samplePosition, Quaternion.LookRotation(Random.insideUnitSphere, Vector3.up));
			spawnedController.OnAIDestroyed += TryNotifyRoomCleared;

			if (room)
				spawnedController.room = room;

			currentlyAliveAI.Add(spawnedController);
		}
	}

	bool AliveAILimitReached() => currentlyAliveAI.Count >= maxAliveAI;

	bool NoAliveAI() => currentlyAliveAI.Count == 0;

	void TryNotifyRoomCleared(AIController controller)
	{
		currentlyAliveAI.Remove(controller);
		controller.OnAIDestroyed -= TryNotifyRoomCleared;
		if (NoAliveAI())
			OnRoomCleared?.Invoke(room);
	}

	bool IsPositionVisible(Vector3 samplePosition)
	{
		return !Physics.Linecast(transform.position, samplePosition);
	}

	bool VerifySpawnPosition(Vector3 samplePosition)
	{
		bool hitObject = Physics.SphereCast(samplePosition + Vector3.up * 2, 0.8f, Vector3.down, out RaycastHit hit, 5);
		return hitObject && hit.transform.CompareTag("Enemy") && !hit.transform.CompareTag("Player");
	}

	Transform GetSpawnPoint() => spawnPoints[Random.Range(0, spawnPoints.Length)];

	void OnDrawGizmos()
	{
		for (int i = 0; i < spawnPoints.Length; i++)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(spawnPoints[i].position, minSpawnRadius);
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(spawnPoints[i].position, maxSpawnRadius);
			Gizmos.color = Color.blue;
			Gizmos.DrawCube(spawnPoints[i].position, Vector3.one);
		}
	}
}