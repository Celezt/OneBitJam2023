using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UDebug = UnityEngine.Debug;

public class AIAttackBehavior : AIBaseBehavior
{
	private float minPlayerDistance;
	private float maxPlayerDistance;
	private float minCircleSwitchTime;
	private float maxCircleSwitchTime;
	private float nextCircleSwitch;

	private Vector3 circleDirection;
	private Vector3 lastCross;

	public AIAttackBehavior(AIController _controller, float _minPlayerDistance, float _maxPlayerDistance, float _detectEdgeDistance, float _minCircleSwitchTime, float _maxCircleSwitchTime, MoveBehaviour _player)
	{
		controller = _controller;
		minPlayerDistance = _minPlayerDistance;
		maxPlayerDistance = _maxPlayerDistance;
		player = _player;
		detectEdgeDistance = _detectEdgeDistance;
		lastCross = Vector3.up;
		minCircleSwitchTime = _minCircleSwitchTime;
		maxCircleSwitchTime = _maxCircleSwitchTime;
	}

	public override void OnInit() { }

	public override void OnEnter()
	{
		nextCircleSwitch = Time.time + Random.Range(minCircleSwitchTime, maxCircleSwitchTime);
	}

	public override void OnExit()
	{
		controller.Move(Vector2.zero);
	}

	void UpdateCircleDirection(Vector3 aiToPlayer) => circleDirection = Vector3.Cross(aiToPlayer, lastCross);

	void SwitchCircleDirection()
	{
		if (Time.time > nextCircleSwitch)
		{
			lastCross *= -1;
			nextCircleSwitch = Time.time + Random.Range(minCircleSwitchTime, maxCircleSwitchTime);
		}
	}

	public override void OnUpdate()
	{
		Vector3 aiToPlayer = (player.transform.position - controller.transform.position).normalized;
		Vector2 aiToPlayer2D = new Vector2(aiToPlayer.x, aiToPlayer.z);
		Vector2 circleDirection2D = new Vector2(circleDirection.x, circleDirection.z);

		float playerDistance = Vector3.Distance(player.transform.position, controller.transform.position);

		UpdateCircleDirection(aiToPlayer);
		SwitchCircleDirection();

		if (!Physics.Raycast(controller.transform.position + Vector3.up, (circleDirection + aiToPlayer).normalized, detectEdgeDistance))
		{
			if (!Physics.Raycast(controller.transform.position + Vector3.up + (circleDirection + aiToPlayer).normalized * detectEdgeDistance, Vector3.down, 2))
			{
				controller.Move(Vector2.zero);
				return;
			}
		}

		if (playerDistance > maxPlayerDistance)
		{
			controller.Move(((circleDirection2D * 0.5f) + aiToPlayer2D));
		}
		else if (playerDistance < minPlayerDistance)
		{
			controller.Move(((circleDirection2D * 0.5f) - aiToPlayer2D));
		}
		else
		{
			controller.Move(circleDirection2D);
		}
	}

	public override void OnFixedUpdate() { }

	public override void OnCollisionEnter(Collision other) { }

	public override void OnCollisionExit(Collision other) { }

	public override void OnTriggerEnter(Collider other) { }

	public override void OnTriggerExit(Collider other) { }
}