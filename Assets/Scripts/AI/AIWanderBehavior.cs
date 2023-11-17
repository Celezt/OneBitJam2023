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

	private Vector3 moveDirection;

	public AIWanderBehavior(AIController _controller, float _maxMoveTime, float _minPlayerDistance, float _detectEdgeDistance, ActorBehaviour _player)
	{
		controller = _controller;
		maxMoveTime = _maxMoveTime;
		minPlayerDistance = _minPlayerDistance;
		player = _player;
		detectEdgeDistance = _detectEdgeDistance;
	}

	public override void OnInit() { }

	public override void OnEnter() { }

	public override void OnExit()
	{
		controller.Move(Vector2.zero);
	}

	public override void OnUpdate()
	{
		if (Vector3.Distance(player.transform.position, controller.transform.position) <= minPlayerDistance)
		{
			controller.SwitchBehavior(AIState.attacking);
			return;
		}

		if (moveTime <= 0.0f)
		{
			moveDirection = URand.insideUnitSphere;
			moveTime = maxMoveTime;
		}
		else
		{
			moveTime -= Time.deltaTime;
		}

		if (!Physics.Raycast(controller.transform.position + Vector3.up, moveDirection, detectEdgeDistance))
		{
			if (!Physics.Raycast(controller.transform.position + Vector3.up + moveDirection * detectEdgeDistance, Vector3.down, 2))
			{
				controller.Move(Vector2.zero);
				return;
			}
		}

		controller.Move(new Vector2(moveDirection.x, moveDirection.z));
	}

	public override void OnFixedUpdate() { }

	public override void OnCollisionEnter(Collision other) { }

	public override void OnCollisionExit(Collision other) { }

	public override void OnTriggerEnter(Collider other) { }

	public override void OnTriggerExit(Collider other) { }
}