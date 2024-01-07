using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonRoom : MonoBehaviour
{
	[System.Serializable]
	struct DungeonDoorPosition
	{
		public Transform possiblePosition;
		public DungeonDoorFacing facing;
	}

	[SerializeField] DungeonDoorPosition[] possibleDoorPositions;
	[SerializeField] DungeonDoor doorPrefab;
	[SerializeField] Vector2 roomSize = new Vector2(1, 1);

	public List<DungeonDoor> Doors { get; private set; } = new List<DungeonDoor>();
	public List<DungeonRoom> Neighbours { get; private set; } = new List<DungeonRoom>();

	public Vector2 Size => roomSize;

	public void AddNeighbour(DungeonRoom room)
	{
		Neighbours.Add(room);
	}

	public DungeonDoor GetOppositeNeighbourDoor(DungeonDoor door)
	{
		return Doors.Where(x => x.facing == DungeonHelper.GetOppositeFacing(door.facing)).FirstOrDefault();
	}

	public void GenerateDoors()
	{
		for (int i = 0; i < Neighbours.Count; i++)
		{
			Vector3 doorToCenterDirection = (transform.position - Neighbours[i].transform.position).normalized;
			if (doorToCenterDirection == Vector3.zero)
				Debug.LogWarning("Zero-vector encountered!");

			Transform doorPosition = possibleDoorPositions.Where(x => DungeonHelper.CompareDirection(doorToCenterDirection, x.facing)).FirstOrDefault().possiblePosition;

			if (!doorPosition)
				continue;

			DungeonDoor door = Instantiate(doorPrefab, doorPosition.position + Vector3.up * 0.5f, Quaternion.LookRotation(doorToCenterDirection), transform);

			door.facing = DungeonHelper.GetFacingFromDirection(doorToCenterDirection);

			Doors.Add(door);
		}
	}

	public void ConnectDoors()
	{
		for (int i = 0; i < Doors.Count; i++)
		{
			DungeonDoor door = Doors[i];
			if (door.destinationDoor)
				continue;

			List<DungeonDoor> possibleDoors = new List<DungeonDoor>();
			for (int j = 0; j < Neighbours.Count; j++)
			{
				DungeonRoom neighbour = Neighbours[j];
				DungeonDoor possibleDestinationDoor = neighbour.GetOppositeNeighbourDoor(door);

				if (possibleDestinationDoor)
				{
					possibleDoors.Add(possibleDestinationDoor);
				}
			}

			DungeonDoor destinationDoor = possibleDoors.OrderBy(x => Vector3.Distance(x.transform.position, door.transform.position)).FirstOrDefault();

			if (destinationDoor)
			{
				door.destinationDoor = destinationDoor;
				destinationDoor.destinationDoor = door;
			}
		}
	}

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(transform.position, new Vector3(roomSize.x, 1, roomSize.y));
		foreach (DungeonDoorPosition t in possibleDoorPositions)
		{
			Gizmos.DrawWireCube(t.possiblePosition.position + Vector3.up * 1.5f, new Vector3(1, 3, 1));
		}

		if (Doors.Count == 0)
			return;
		Gizmos.color = Color.green;
		foreach (DungeonDoor door in Doors)
		{
			Gizmos.DrawWireCube(door.transform.position + Vector3.up * 1.5f, new Vector3(1, 3, 1));
		}
	}
#endif
}