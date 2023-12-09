using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStationaryIdleBehavior : AIMovingBase
{
	[SerializeField] private float maxLookHalfAngle = 60;

	private Vector3 initLookDirection;
	private int rotationDirection;

	#region OLD-SYSTEM
	// public AIStationaryIdleBehavior(AIController _controller, float _minPlayerDistance, float _maxLookHalfAngle, Transform _player)
	// {
	// 	controller = _controller;
	// 	minPlayerDistance = _minPlayerDistance;
	// 	player = _player;
	// 	maxLookHalfAngle = _maxLookHalfAngle;
	// }
	#endregion

	public override void OnInit()
	{
		base.OnInit();
		initLookDirection = controller.transform.forward;
		rotationDirection = Random.value > 0.5f ? 1 : -1;
	}

	public override void OnEnter() { }

	public override void OnExit()
	{
		controller.Move(Vector2.zero);
	}

	public override void OnUpdate()
	{

		Vector3 aiToPlayer = (player.transform.position - controller.transform.position).normalized;

		float playerDistance = Vector3.Distance(player.transform.position, controller.transform.position);

		if (playerDistance < minPlayerDistance && Vector3.Angle(initLookDirection, aiToPlayer) < maxLookHalfAngle)
		{
			controller.SwitchBehavior(AIState.attacking);
			return;
		}

		if (controller.transform.eulerAngles.y > initLookDirection.y + maxLookHalfAngle || controller.transform.eulerAngles.y < initLookDirection.y - maxLookHalfAngle)
		{
			rotationDirection *= -1;
		}

		Vector2 lookDirection = new Vector2(0, Mathf.PingPong(Time.time, maxLookHalfAngle * 2) - maxLookHalfAngle); //new Vector2(0, initLookDirection.y + (Time.deltaTime * rotationDirection)).normalized;

		controller.Look(lookDirection);
	}

	public override void OnFixedUpdate() { }

	public override void OnCollisionEnter(Collision other) { }

	public override void OnCollisionExit(Collision other) { }

	public override void OnTriggerEnter(Collider other) { }

	public override void OnTriggerExit(Collider other) { }
}