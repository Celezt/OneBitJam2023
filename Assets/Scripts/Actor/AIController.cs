using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIState
{
	wandering,
	attacking
}

public class AIController : ActorBehaviour
{
	[SerializeField] float maxMoveTime = 2.5f;
	[SerializeField] float minPlayerDistance = 3.5f;

	private AIState activeState;

	private AIBaseBehavior activeBehavior;

	private AIBaseBehavior wanderBehavior;
	private AIBaseBehavior attackBehavior;

	// Start is called before the first frame update
	void Start()
	{
		ActorBehaviour player = GameObject.Find("PlayerActor").GetComponent<ActorBehaviour>();
		wanderBehavior = new AIWanderBehavior(this, maxMoveTime, minPlayerDistance, player);
		attackBehavior = new AIAttackBehavior(this, minPlayerDistance, player);

		SwitchBehavior(AIState.wandering, true);
	}

	// Update is called once per frame
	void Update()
	{
		activeBehavior?.OnUpdate();
	}

	// void FixedUpdate()
	// {
	// 	activeBehavior?.OnFixedUpdate();
	// }

	public void SwitchBehavior(AIState newState, bool onSceneStart = false)
	{
		if (newState == activeState && !onSceneStart)
			return;

		activeBehavior?.OnExit();
		switch (newState)
		{
		case AIState.wandering:
			activeBehavior = wanderBehavior;
			break;
		case AIState.attacking:
			activeBehavior = attackBehavior;
			break;
		}
		activeBehavior?.OnEnter();
		activeState = newState;
	}
}