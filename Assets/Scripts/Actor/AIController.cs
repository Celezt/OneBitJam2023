using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIState
{
	wandering,
	attacking
}

public class AIController : MoveBehaviour
{
	[Header("Generic AI settings")]
	[SerializeField] float minPlayerDistance = 3.5f;
	[SerializeField] float minEdgeDistance = 7;

	[Header("Wander behavior settings")]
	[SerializeField] float maxMoveTime = 2.5f;

	[Header("Attack behavior settings")]
	[SerializeField] float maxPlayerDistance = 5.5f;
	[SerializeField] float minCirclingDirectionSwitchTime = 2;
	[SerializeField] float maxCirclingDirectionSwitchTime = 4.5f;

	private AIState activeState;

	private AIBaseBehavior activeBehavior;

	private AIBaseBehavior wanderBehavior;
	private AIBaseBehavior attackBehavior;

	// Start is called before the first frame update
	void Start()
	{
		MoveBehaviour player = GameObject.Find("PlayerActor").GetComponent<MoveBehaviour>();
		wanderBehavior = new AIWanderBehavior(this, maxMoveTime, minPlayerDistance, minEdgeDistance, player);
		attackBehavior = new AIAttackBehavior(this, minPlayerDistance, maxPlayerDistance, minEdgeDistance, minCirclingDirectionSwitchTime, maxCirclingDirectionSwitchTime, player);

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