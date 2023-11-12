using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UDebug = UnityEngine.Debug;

public class AIAttackBehavior : AIBaseBehavior
{
	private float minPlayerDistance;

	private ActorBehaviour player;

	private Vector2 moveDirection;

	public AIAttackBehavior(AIController _controller, float _minPlayerDistance, ActorBehaviour _player)
	{
		controller = _controller;
		minPlayerDistance = _minPlayerDistance;
		player = _player;
	}

	public override void OnInit() { }

	public override void OnEnter() { }

	public override void OnExit() { }

	public override void OnUpdate()
	{
		Vector3 aiToPlayer = (player.transform.position - controller.transform.position).normalized;
		Vector2 aiToPlayer2D = new Vector2(aiToPlayer.x, aiToPlayer.z);
		Vector3 circleDirection = Vector3.Cross(aiToPlayer, Vector3.up);
		Vector2 circleDirection2D = new Vector2(circleDirection.x, circleDirection.z);

		if (Vector3.Distance(player.transform.position, controller.transform.position) > minPlayerDistance)
		{
			controller.Move(((circleDirection2D * 0.5f) + aiToPlayer2D));
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