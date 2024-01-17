using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
	[SerializeField] float roomSpawnOffset = 200f;
	[SerializeField] int minimumNumRooms = 4;
	[SerializeField] DungeonRoom[] rooms;

	public int CurrentLevel { get => currentLevel; private set { } }

	public static DungeonGenerator INSTANCE { get { return instance; } private set { } }

	private static DungeonGenerator instance;

	public delegate void OnDungeonDoneGeneratingEvent();
	public event OnDungeonDoneGeneratingEvent OnDungeonDoneGenerating;

	public float Progress { get; private set; } = 0;

	private List<DungeonRoom> dungeonLayout = new List<DungeonRoom>();

	private Vector3[] validSpawnDirections = new Vector3[] { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };

	public DungeonRoom StartRoom { get; private set; }

	private int currentLevel = 1;

	private CancellationTokenSource cancellationTokenSource;

	void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(this);
		}

		CTSUtility.Reset(ref cancellationTokenSource);
		GenerateDungeon().Forget();
	}

	public void Reset()
	{
		currentLevel = 1;
		CleanUpDungeon();
		CTSUtility.Reset(ref cancellationTokenSource);
		GenerateDungeon().Forget();
	}

	public void ProgressLevel()
	{
		currentLevel++;
		CleanUpDungeon();
		CTSUtility.Reset(ref cancellationTokenSource);
		GenerateDungeon().Forget();
	}

	void CleanUpDungeon()
	{
		foreach (DungeonRoom room in dungeonLayout)
		{
			foreach (DungeonDoor door in room.Doors)
			{
				Destroy(door.gameObject);
			}
			Destroy(room.gameObject);
		}
		dungeonLayout.Clear();
		StartRoom = null;
	}

	async UniTask GenerateDungeon()
	{
		int roomAmount = Mathf.FloorToInt(minimumNumRooms + Random.Range(0, 3) + currentLevel * 1.8f);

		// Pick and spawn center/starting room
		DungeonRoom room = rooms.GetRandom();

		StartRoom = Instantiate(room, transform.position, Quaternion.identity);
		StartRoom.tag = "StartRoom";
		dungeonLayout.Add(StartRoom);

		Queue<DungeonRoom> dungeonQueue = new Queue<DungeonRoom>();
		dungeonQueue.Enqueue(StartRoom);

		while (dungeonQueue.Count > 0 && !cancellationTokenSource.Token.IsCancellationRequested)
		{
			DungeonRoom currentCenterRoom = dungeonQueue.Dequeue();

			Progress = (dungeonLayout.Count / (float) roomAmount) * 100.0f;
#if UNITY_EDITOR
			Debug.Log($"Dungeon generation progress: {Progress}% complete");
#endif

			if (dungeonLayout.Count >= roomAmount)
				continue;

			bool spawnedRoom = false;

			// Go randomly around the current room and try to spawn more rooms checking in the 4 cardinal directions around each room
			List<Vector3> availableDirections = new List<Vector3>(validSpawnDirections);
			while (availableDirections.Count > 0)
			{
				Vector3 direction = availableDirections.GetRandom();

				availableDirections.Remove(direction);

				Vector3 neighbourPosition = currentCenterRoom.transform.position + Vector3.Scale(direction, (currentCenterRoom.Size.x_z() / 2.0f) + new Vector3(roomSpawnOffset, 0, roomSpawnOffset));
				Debug.DrawRay(neighbourPosition.x_z(), Vector3.up, Color.red, 25);

				if (!CanSpawnInDirection(neighbourPosition, currentCenterRoom.Size))
					continue;

				if (currentCenterRoom.Neighbours.Count > 1)
					continue;

				if (Random.value < 0.5f)
					continue;

				spawnedRoom = true;
				SpawnRoom(neighbourPosition, dungeonQueue, currentCenterRoom);
				await UniTask.NextFrame();
			}

			if (!spawnedRoom)
				dungeonQueue.Enqueue(StartRoom);
		}

		foreach (DungeonRoom dungeonRoom in dungeonLayout)
		{
			dungeonRoom.GenerateDoors();
		}

		foreach (DungeonRoom dungeonRoom in dungeonLayout)
		{
			dungeonRoom.ConnectDoors();
		}

		OnDungeonDoneGenerating?.Invoke();
	}

	void SpawnRoom(Vector3 position, Queue<DungeonRoom> queue, DungeonRoom neighbour)
	{
		DungeonRoom room = Instantiate(rooms.GetRandom(), position.x_z(), Quaternion.identity);

		room.AddNeighbour(neighbour);
		neighbour.AddNeighbour(room);

		queue.Enqueue(room);

		dungeonLayout.Add(room);
	}

	bool CanSpawnInDirection(Vector3 position, Vector3 roomSize) => !Physics.CheckBox(position, (roomSize - new Vector3(0.1f, 0, 0.1f)) / 2.0f);
}

public static class DungeonHelper
{
	public static bool CompareDirection(Vector3 direction, DungeonDoorFacing facing)
	{
		direction = direction.normalized;

		switch (facing)
		{
		case DungeonDoorFacing.Forward:
			return Vector3.Dot(direction, Vector3.forward) == 1;
		case DungeonDoorFacing.Backward:
			return Vector3.Dot(direction, Vector3.back) == 1;
		case DungeonDoorFacing.Left:
			return Vector3.Dot(direction, Vector3.left) == 1;
		case DungeonDoorFacing.Right:
			return Vector3.Dot(direction, Vector3.right) == 1;
		default:
#if UNITY_EDITOR
			Debug.LogWarning($"DungeonGenerator encountered unexpected switch-case in the CompareDirection-method comparing {direction} and {facing}.");
#endif
			return false;
		}
	}

	public static DungeonDoorFacing GetFacingFromDirection(Vector3 direction)
	{
		direction = direction.normalized;

		if (direction == Vector3.forward)
		{
			return DungeonDoorFacing.Forward;
		}
		else if (direction == Vector3.back)
		{
			return DungeonDoorFacing.Backward;
		}
		else if (direction == Vector3.left)
		{
			return DungeonDoorFacing.Left;
		}
		else if (direction == Vector3.right)
		{
			return DungeonDoorFacing.Right;
		}

#if UNITY_EDITOR
		Debug.LogWarning($"DungeonGenerator encountered unexpected if-case in the GetFacingFromDirection-method with direction input {direction}.");
#endif
		return DungeonDoorFacing.Forward;
	}

	public static DungeonDoorFacing GetOppositeFacing(DungeonDoorFacing facing)
	{
		switch (facing)
		{
		case DungeonDoorFacing.Forward:
			return DungeonDoorFacing.Backward;
		case DungeonDoorFacing.Backward:
			return DungeonDoorFacing.Forward;
		case DungeonDoorFacing.Left:
			return DungeonDoorFacing.Right;
		case DungeonDoorFacing.Right:
			return DungeonDoorFacing.Left;
		default:
#if UNITY_EDITOR
			Debug.LogWarning($"DungeonGenerator encountered unexpected switch-case in the GetOppositeFacing-method with facing input {facing}.");
#endif
			return DungeonDoorFacing.Forward;
		}
	}
}