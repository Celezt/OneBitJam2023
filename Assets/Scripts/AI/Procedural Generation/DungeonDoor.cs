using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonDoor : MonoBehaviour
{
	[SerializeField] float playerSpawnDistance = 2;
	[HideInInspector] public DungeonDoor destinationDoor;

	public Vector2 Position => transform.position;

	public DungeonDoorFacing facing { get; set; }

	void OnTriggerEnter(Collider other)
	{
		if (!other.transform.CompareTag("Player"))
			return;

#if UNITY_EDITOR
		Debug.Log($"Teleporting {other.name}");
#endif

		other.GetComponent<Rigidbody>().position = destinationDoor.Position.x_z() + destinationDoor.transform.forward * playerSpawnDistance;
	}

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		Gizmos.DrawLine(transform.position, transform.position + transform.forward * playerSpawnDistance);
		Gizmos.DrawWireSphere(transform.position + transform.forward * playerSpawnDistance, 1f);
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