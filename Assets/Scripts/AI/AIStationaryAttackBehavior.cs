using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStationaryAttackBehavior : AIAttackingBase
{
	[SerializeField] private float maxLookHalfAngle = 60;

	#region OLD-SYSTEM
	// public AIStationaryAttackBehavior(AIController _controller, float _maxPlayerDistance, float _maxLookHalfAngle, Transform _player, WeaponHandler _weaponHandler)
	// {
	// 	controller = _controller;
	// 	maxPlayerDistance = _maxPlayerDistance;
	// 	player = _player;
	// 	weaponHandler = _weaponHandler;
	// 	maxLookHalfAngle = _maxLookHalfAngle;
	// }
	#endregion

	public override void OnInit() { base.OnInit(); }

	public override void OnEnter() { }

	public override void OnExit()
	{
		controller.Move(Vector2.zero);
	}

	public override void OnUpdate()
	{
		Vector3 aiToPlayer = (player.transform.position - controller.transform.position).normalized;
		Vector2 aiToPlayer2D = new Vector2(aiToPlayer.x, aiToPlayer.z);

		float playerDistance = Vector3.Distance(player.transform.position, controller.transform.position);

		if (playerDistance > maxPlayerDistance || Vector3.Angle(controller.transform.forward, aiToPlayer) > maxLookHalfAngle)
		{
			controller.SwitchBehavior(AIState.wandering);
			return;
		}

		weaponHandler.OnShoot();

		controller.Look(aiToPlayer2D);
	}

	public override void OnFixedUpdate() { }

	public override void OnCollisionEnter(Collision other) { }

	public override void OnCollisionExit(Collision other) { }

	public override void OnTriggerEnter(Collider other) { }

	public override void OnTriggerExit(Collider other) { }
}