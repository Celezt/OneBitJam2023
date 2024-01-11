using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonDoor : MonoBehaviour
{
	[SerializeField] float playerSpawnDistance = 2;
	[HideInInspector] public DungeonDoor destinationDoor;

	public DungeonDoorFacing facing { get; set; }
	public DungeonRoom room { get; set; }

	public delegate void OnPlayerTeleportEvent(DungeonRoom room);
	public event OnPlayerTeleportEvent OnPlayerTeleport;

	public void TeleportPlayer(Collider other)
	{
		if (!other.transform.CompareTag("Player"))
			return;

#if UNITY_EDITOR
		Debug.Log($"Teleporting {other.name}");
#endif

		OnPlayerTeleport?.Invoke(destinationDoor.room);
		other.GetComponent<Rigidbody>().position = destinationDoor.transform.position + destinationDoor.transform.forward * playerSpawnDistance;
	}

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawLine(transform.position, transform.position + transform.forward * playerSpawnDistance);
		Gizmos.DrawWireSphere(transform.position + transform.forward * playerSpawnDistance, 1f);

		if (!destinationDoor)
			return;

		Gizmos.color = Color.magenta;
		Gizmos.DrawLine(transform.position, destinationDoor.transform.position);
	}
#endif
}

public enum DungeonDoorFacing
{
	Forward,
	Backward,
	Left,
	Right
}