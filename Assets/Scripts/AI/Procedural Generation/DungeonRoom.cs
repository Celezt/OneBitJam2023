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
	[SerializeField] Vector2Int roomSize = new Vector2Int(1, 1);

	public List<DungeonDoor> Doors { get; private set; }
	public List<DungeonRoom> Neighbours { get; private set; } = new List<DungeonRoom>();

	public Vector2 Position => transform.position;
	public int NeighbourCount => Neighbours.Count;
	public Vector2Int Size => roomSize;

	public void AddNeighbour(DungeonRoom room)
	{
		Neighbours.Add(room);
	}

	public DungeonDoor GetOppositeNeighbourDoor(DungeonDoorFacing facing)
	{
		return Doors.Where(x => x.facing == DungeonHelper.GetOppositeFacing(facing)).FirstOrDefault();
	}

	public void GenerateDoors()
	{
		for (int i = 0; i < Neighbours.Count; i++)
		{
			Vector3 doorToCenterDirection = (transform.position - Neighbours[i].Position.x_z()).normalized;
			Transform doorPosition = possibleDoorPositions.Where(x => DungeonHelper.CompareDirection(doorToCenterDirection, x.facing)).FirstOrDefault().possiblePosition;
			DungeonDoor door = Instantiate(doorPrefab, doorPosition.position, Quaternion.LookRotation(doorToCenterDirection));

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

			for (int j = 0; j < Neighbours.Count; j++)
			{
				DungeonRoom neighbour = Neighbours[i];
				DungeonDoor destinationDoor = neighbour.GetOppositeNeighbourDoor(door.facing);

				if (destinationDoor)
				{
					door.destinationDoor = destinationDoor;
					destinationDoor.destinationDoor = door;
					break;
				}
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
	}
#endif
}