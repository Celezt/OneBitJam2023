using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using URand = UnityEngine.Random;

public class AIRangedAttackBehavior : AIAttackingBase
{
	[SerializeField] private float maxMoveTime = 2.5f;
	[SerializeField] private float minMoveTime = 0.65f;

	private float moveTime;

	private Vector3 moveDirection;

	#region OLD-SYSTEM
	// public AIRangedAttackBehavior(AIController _controller, float _minPlayerDistance, float _maxMoveTime, float _detectEdgeDistance, float _detectWallDistance, Transform _player, WeaponHandler _weaponHandler)
	// {
	// 	controller = _controller;
	// 	minPlayerDistance = _minPlayerDistance;
	// 	player = _player;
	// 	detectEdgeDistance = _detectEdgeDistance;
	// 	weaponHandler = _weaponHandler;
	// 	detectWallDistance = _detectWallDistance;
	// 	maxMoveTime = _maxMoveTime;
	// }
	#endregion

	public override void OnInit() { base.OnInit(); }

	public override void OnEnter()
	{
		moveDirection = Vector3.ProjectOnPlane(URand.insideUnitSphere, Vector3.up);
		moveTime = minMoveTime;
		CTSUtility.Reset(ref performDevianceLookAsyncTokenSource);
		PerformDevianceLookAsync(performDevianceLookAsyncTokenSource.Token);
	}

	public override void OnExit()
	{
		CTSUtility.Clear(ref performDevianceLookAsyncTokenSource);
		controller.Move(Vector2.zero);
	}

	public override void OnUpdate()
	{
		weaponHandler.OnShoot();

		if (moveTime <= 0.0f)
		{
			moveDirection = Vector3.ProjectOnPlane(URand.insideUnitSphere, Vector3.up);
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
				moveTime = 0.0f;
				return;
			}
		}

		if (Physics.Raycast(controller.transform.position + Vector3.up, moveDirection, detectWallDistance))
		{
			controller.Move(Vector2.zero);
			moveTime = 0.0f;
			return;
		}

		Vector3 aiToPlayer = (player.transform.position - controller.transform.position).normalized;
		Vector2 aiToPlayer2D = new Vector2(aiToPlayer.x, aiToPlayer.z);

		float playerDistance = Vector3.Distance(player.transform.position, controller.transform.position);

		if (playerDistance < minPlayerDistance)
		{
			controller.Move(Vector2.zero);
			moveTime = 0.0f;
			return;
		}

		controller.Move(new Vector2(moveDirection.x, moveDirection.z));

		if (!useDirectionDeviance)
			controller.Look(aiToPlayer2D);
	}

	public override void OnFixedUpdate() { }

	public override void OnCollisionEnter(Collision other) { }

	public override void OnCollisionExit(Collision other) { }

	public override void OnTriggerEnter(Collider other) { }

	public override void OnTriggerExit(Collider other) { }

	protected override async UniTask PerformDevianceLookAsync(CancellationToken token)
	{
		await base.PerformDevianceLookAsync(token);

		while (!token.IsCancellationRequested)
		{
			Vector3 aiToPlayer = (player.transform.position - controller.transform.position).normalized;

			Vector2 lastAIToPlayerDirection = aiToPlayer.xz();

			lookingDeviation = GetDevianceDirectionVector(devianceDirectionMaxHalfAngle, aiToPlayer.xz());
			lookingDeviationDuration = URand.Range(devianceMinDuration, devianceMaxDuration);

			while (lookingDeviationDuration > 0.0f)
			{
				lookingDeviationDuration -= Time.deltaTime;
				if (aiToPlayer.xz() != lastAIToPlayerDirection)
					lookingDeviation = GetDevianceDirectionVector(devianceDirectionMaxHalfAngle, aiToPlayer.xz());
				controller.Look(lookingDeviation);
				await UniTask.Delay(1000, false, PlayerLoopTiming.Update, token);
			}

			lookingDeviationDelay = URand.Range(devianceMinStartDelay, devianceMaxStartDelay);

			while (lookingDeviationDelay > 0.0f)
			{
				lookingDeviationDelay -= Time.deltaTime;
				controller.Look(aiToPlayer.xz());
				await UniTask.Delay(1000, false, PlayerLoopTiming.Update, token);
			}
		}
	}
}