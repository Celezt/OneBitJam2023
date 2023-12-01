using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIState
{
	wandering,
	attacking
}

public class AIController : MonoBehaviour
{
	[Header("Generic AI settings")]
	[SerializeField] MoveBehaviour moveController;
	[SerializeField] LookTurretBehaviour lookController;
	[SerializeField] float minPlayerDistance = 3.5f;
	[SerializeField] float minEdgeDistance = 7;

	[Header("Wander behavior settings")]
	[SerializeField] float maxMoveTime = 2.5f;
	[SerializeField] float minWallDistance = 5;

	[Header("Attack behavior settings")]
	[SerializeField] float maxPlayerDistance = 5.5f;
	[SerializeField] float minCirclingDirectionSwitchTime = 2;
	[SerializeField] float maxCirclingDirectionSwitchTime = 4.5f;
	[SerializeField] WeaponHandler weaponHandler;

	private AIState activeState;

	private AIBaseBehavior activeBehavior;

	private AIBaseBehavior wanderBehavior;
	private AIBaseBehavior attackBehavior;

	// Start is called before the first frame update
	void Start()
	{
		MoveBehaviour player = GameObject.Find("PlayerActor").GetComponent<MoveBehaviour>();
		wanderBehavior = new AIWanderBehavior(this, maxMoveTime, minPlayerDistance, minEdgeDistance, minWallDistance, player);
		attackBehavior = new AIAttackBehavior(this, minPlayerDistance, maxPlayerDistance, minEdgeDistance, minCirclingDirectionSwitchTime, maxCirclingDirectionSwitchTime, minWallDistance, player, weaponHandler);

		SwitchBehavior(AIState.wandering, true);
	}

	// Update is called once per frame
	void Update()
	{
		activeBehavior?.OnUpdate();
	}

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

	public void Move(Vector2 direction) => moveController.Move(direction);

	public void LookAt(Vector2 target) => lookController.LookAt(target);

	public void Look(Vector2 direction) => lookController.Look(direction);
}