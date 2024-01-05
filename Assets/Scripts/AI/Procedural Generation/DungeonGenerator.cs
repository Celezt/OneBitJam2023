using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
	[SerializeField] DungeonRoom[] rooms;

	private List<DungeonRoom> dungeonLayout = new List<DungeonRoom>();

	private Vector2[] validSpawnDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

	private DungeonRoom startRoom;

	void Awake()
	{
		GenerateDungeon();
	}

	void GenerateDungeon()
	{
		int roomAmount = 5;

		// Pick and spawn center/starting room
		DungeonRoom room = rooms.GetRandom();

		startRoom = Instantiate(room, transform.position, Quaternion.identity);

		Queue<DungeonRoom> dungeonQueue = new Queue<DungeonRoom>();
		dungeonQueue.Enqueue(startRoom);

		while (dungeonQueue.Count > 0)
		{
			DungeonRoom currentCenterRoom = dungeonQueue.Dequeue();

			if (dungeonLayout.Count >= roomAmount)
				continue;

			// Go randomly around the current room and try to spawn more rooms checking in the 4 cardinal directions around each room
			List<Vector2> availableDirections = new List<Vector2>(validSpawnDirections);
			while (availableDirections.Count > 0)
			{
				Vector2 direction = availableDirections.GetRandom();

				availableDirections.Remove(direction);

				Vector2 neighbourPosition = currentCenterRoom.Position + direction;

				if (!CanSpawnInDirection(currentCenterRoom.Position, direction))
					continue;

				if (currentCenterRoom.NeighbourCount > 1)
					continue;

				if (Random.value < 0.5f)
					continue;

				SpawnRoom(neighbourPosition, dungeonQueue, currentCenterRoom);
			}
		}

		foreach (DungeonRoom dungeonRoom in dungeonLayout)
		{
			dungeonRoom.GenerateDoors();
		}
	}

	void SpawnRoom(Vector2 position, Queue<DungeonRoom> queue, DungeonRoom neighbour)
	{
		DungeonRoom room = Instantiate(rooms.GetRandom(), position, Quaternion.identity);

		room.AddNeighbour(neighbour);
		neighbour.AddNeighbour(room);

		queue.Enqueue(room);

		dungeonLayout.Add(room);
	}

	Vector2 GetNextDirection() => validSpawnDirections.GetRandom();

	bool CanSpawnInDirection(Vector2 position, Vector2 direction) => Physics.CheckBox(position + direction, Vector3.one * 0.5f);
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
			return DungeonDoorFacing.Forward;
		}
	}
}