using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UDebug = UnityEngine.Debug;

public class AIAttackBehavior : AIAttackingBase
{
	[SerializeField] private float minCircleSwitchTime = 2;
	[SerializeField] private float maxCircleSwitchTime = 4.5f;
	[SerializeField] private float playerFaceAIMinAngle = 5;

	private float nextCircleSwitch;

	private Vector3 circleDirection;
	private Vector3 lastCross;

	#region OLD-SYSTEM
	// public AIAttackBehavior(AIController _controller, float _minPlayerDistance, float _maxPlayerDistance, float _detectEdgeDistance, float _minCircleSwitchTime, float _maxCircleSwitchTime, float _detectWallDistance, float _playerFaceAIMinAngle, Transform _player, WeaponHandler _weaponHandler)
	// {
	// 	controller = _controller;
	// 	minPlayerDistance = _minPlayerDistance;
	// 	maxPlayerDistance = _maxPlayerDistance;
	// 	player = _player;
	// 	detectEdgeDistance = _detectEdgeDistance;
	// 	lastCross = Vector3.up;
	// 	minCircleSwitchTime = _minCircleSwitchTime;
	// 	maxCircleSwitchTime = _maxCircleSwitchTime;
	// 	weaponHandler = _weaponHandler;
	// 	detectWallDistance = _detectWallDistance;
	// 	playerFaceAIMinAngle = _playerFaceAIMinAngle;
	// }
	#endregion

	public override void OnInit() { base.OnInit(); }

	public override void OnEnter()
	{
		lastCross = Vector3.up;
		nextCircleSwitch = Time.time + Random.Range(minCircleSwitchTime, maxCircleSwitchTime);
	}

	public override void OnExit()
	{
		controller.Move(Vector2.zero);
	}

	void UpdateCircleDirection(Vector3 aiToPlayer) => circleDirection = Vector3.Cross(aiToPlayer, lastCross);

	void SwitchCircleDirection()
	{
		if (Physics.Raycast(controller.transform.position + Vector3.up, circleDirection.normalized, out RaycastHit hit, detectAIDistance))
		{
			if (hit.transform.TryGetComponent(out AIController aIController))
			{
				lastCross *= -1;
				nextCircleSwitch = Time.time + Random.Range(minCircleSwitchTime, maxCircleSwitchTime);
				return;
			}
		}

		Vector3 playerToAI = (controller.transform.position - player.transform.position).normalized;
		if (Vector3.Dot(player.transform.forward, controller.transform.forward) < 0.0f && Vector3.Angle(player.transform.forward, playerToAI) < playerFaceAIMinAngle)
		{
			lastCross *= -1;
			nextCircleSwitch = Time.time + Random.Range(minCircleSwitchTime, maxCircleSwitchTime);
			return;
		}

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

		weaponHandler.OnShoot();

		controller.Look(aiToPlayer2D);

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