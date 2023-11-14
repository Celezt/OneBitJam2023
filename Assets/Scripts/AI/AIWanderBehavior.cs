using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using URand = UnityEngine.Random;
using UDebug = UnityEngine.Debug;

public class AIWanderBehavior : AIBaseBehavior
{
	private float moveTime;
	private float maxMoveTime;
	private float minPlayerDistance;

	private ActorBehaviour player;

	private Vector2 moveDirection;

	public AIWanderBehavior(AIController _controller, float _maxMoveTime, float _minPlayerDistance, ActorBehaviour _player)
	{
		controller = _controller;
		maxMoveTime = _maxMoveTime;
		minPlayerDistance = _minPlayerDistance;
		player = _player;
	}

	public override void OnInit() { }

	public override void OnEnter() { }

	public override void OnExit() { }

	public override void OnUpdate()
	{
		if (Vector3.Distance(player.transform.position, controller.transform.position) <= minPlayerDistance)
		{
			controller.Move(Vector2.zero);
			controller.SwitchBehavior(AIState.attacking);
			return;
		}

		if (moveTime <= 0.0f)
		{
			moveDirection = URand.insideUnitSphere;
			Vector3 projectedMoveDirection = Vector3.ProjectOnPlane(moveDirection, Vector3.up);
			moveDirection = new Vector2(projectedMoveDirection.x, projectedMoveDirection.z);
			moveTime = maxMoveTime;
		}
		else
		{
			moveTime -= Time.deltaTime;
		}
		controller.Move(moveDirection);
	}

	public override void OnFixedUpdate() { }

	public override void OnCollisionEnter(Collision other) { }

	public override void OnCollisionExit(Collision other) { }

	public override void OnTriggerEnter(Collider other) { }

	public override void OnTriggerExit(Collider other) { }
}