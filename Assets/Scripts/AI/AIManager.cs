using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class AIManager : MonoBehaviour
{
	enum EAIAgentRoomDistance
	{
		SameRoom,
		NeighbourRoom
	}

	[Serializable]
	struct DistanceUpdateRate
	{
		public float updateRate;
		public EAIAgentRoomDistance distanceType;
	}

	[SerializeField] DistanceUpdateRate[] aiUpdateRates;

	public static AIManager INSTANCE { get { return instance; } private set { } }
	private static AIManager instance;

	delegate void OnUpdateNearAIEvent();
	event OnUpdateNearAIEvent OnUpdateNearAI;
	delegate void OnUpdateFarAIEvent();
	event OnUpdateFarAIEvent OnUpdateFarAI;

	private List<AIController> controllers = new List<AIController>();

	private List<DungeonDoor> doors;

	private DungeonRoom startRoom;

	void OnEnable()
	{
		if (instance != null && instance != this)
		{
			Debug.LogError($"Multiple AI Managers in scene not allowed, destroying duplicate!");
			Destroy(this);
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(this);

			if (DungeonGenerator.INSTANCE)
				DungeonGenerator.INSTANCE.OnDungeonDoneGenerating += SetStartRoom;
		}
	}

	void OnDisable()
	{
		if (instance == this && DungeonGenerator.INSTANCE)
			DungeonGenerator.INSTANCE.OnDungeonDoneGenerating -= SetStartRoom;
	}

	void Start()
	{

		foreach (DistanceUpdateRate distanceUpdateRate in aiUpdateRates)
		{
			switch (distanceUpdateRate.distanceType)
			{
			case EAIAgentRoomDistance.SameRoom:
				InvokeRepeating(nameof(UpdateSameRoomAI), distanceUpdateRate.updateRate, distanceUpdateRate.updateRate);
				continue;
			case EAIAgentRoomDistance.NeighbourRoom:
				InvokeRepeating(nameof(UpdateNeighbourRoomAI), distanceUpdateRate.updateRate, distanceUpdateRate.updateRate);
				continue;
			default:
				continue;
			}
		}

		doors = new List<DungeonDoor>(FindObjectsOfType<DungeonDoor>());
		if (doors.Count > 0)
		{
			foreach (DungeonDoor door in doors)
			{
				door.OnPlayerTeleport += UpdateAIList;
			}
		}
	}

	void SetStartRoom() => startRoom = DungeonGenerator.INSTANCE.StartRoom;

	void UpdateAIList(DungeonRoom room)
	{
		foreach (AIController controller in controllers)
		{
			if (controller.room == room)
			{
				OnUpdateNearAI += controller.UpdateAI;
				OnUpdateFarAI -= controller.UpdateAI;
			}
			else
			{
				OnUpdateNearAI -= controller.UpdateAI;
				OnUpdateFarAI += controller.UpdateAI;
			}
		}
	}

	void UpdateSameRoomAI() => OnUpdateNearAI?.Invoke();

	void UpdateNeighbourRoomAI() => OnUpdateFarAI?.Invoke();

	public void Subscribe(AIController controller)
	{
		if (!controllers.Contains(controller))
			controllers.Add(controller);
		if (!startRoom)
		{
			OnUpdateNearAI += controller.UpdateAI;
		}
		else
		{
			if (controller.room == startRoom)
				OnUpdateNearAI += controller.UpdateAI;
			else
				OnUpdateFarAI += controller.UpdateAI;
		}
	}

	public void UnSubscribe(AIController controller)
	{
		controllers.Remove(controller);
		OnUpdateFarAI -= controller.UpdateAI;
		OnUpdateNearAI -= controller.UpdateAI;
	}
}