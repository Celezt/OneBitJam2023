using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URand = UnityEngine.Random;
using UDebug = UnityEngine.Debug;
using System.Threading;
using Cysharp.Threading.Tasks;

public class AIWanderBehavior : AIMovingBase
{
	[SerializeField] private float maxMoveTime = 2.5f;
	[SerializeField] private float minMoveTime = 0.65f;

	private float moveTime;

	private Vector3 moveDirection;

	#region OLD-SYSTEM
	// public AIWanderBehavior(AIController _controller, float _maxMoveTime, float _minPlayerDistance, float _detectEdgeDistance, float _detectWallDistance, Transform _player)
	// {
	// 	controller = _controller;
	// 	maxMoveTime = _maxMoveTime;
	// 	minPlayerDistance = _minPlayerDistance;
	// 	player = _player;
	// 	detectEdgeDistance = _detectEdgeDistance;
	// 	detectWallDistance = _detectWallDistance;
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
		if (Vector3.Distance(player.transform.position, controller.transform.position) <= minPlayerDistance)
		{
			controller.SwitchBehavior(AIState.attacking);
			return;
		}

		if (moveTime <= 0.0f)
		{
			moveDirection = Vector3.ProjectOnPlane(URand.insideUnitSphere, Vector3.up);
			moveTime = URand.Range(minMoveTime, maxMoveTime);
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

		if (Physics.Raycast(controller.transform.position + Vector3.up, moveDirection, detectWallDistance))
		{
			controller.Move(Vector2.zero);
			moveTime = 0.0f;
			return;
		}

		controller.Move(new Vector2(moveDirection.x, moveDirection.z));
		if (!useDirectionDeviance)
			controller.Look(new Vector2(moveDirection.x, moveDirection.z));
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
			Vector2 lastMoveDirection = moveDirection.xz();

			lookingDeviation = GetDevianceDirectionVector(devianceDirectionMaxHalfAngle, moveDirection.xz());
			lookingDeviationDuration = URand.Range(devianceMinDuration, devianceMaxDuration);

			while (lookingDeviationDuration > 0.0f)
			{
				lookingDeviationDuration -= Time.deltaTime;
				if (moveDirection.xz() != lastMoveDirection)
					lookingDeviation = GetDevianceDirectionVector(devianceDirectionMaxHalfAngle, moveDirection.xz());
				controller.Look(lookingDeviation);
				await UniTask.Delay(1000, false, PlayerLoopTiming.Update, token);
			}

			lookingDeviationDelay = URand.Range(devianceMinStartDelay, devianceMaxStartDelay);

			while (lookingDeviationDelay > 0.0f)
			{
				lookingDeviationDelay -= Time.deltaTime;
				controller.Look(moveDirection.xz());
				await UniTask.Delay(1000, false, PlayerLoopTiming.Update, token);
			}
		}
	}
}