using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class AIBaseBehavior : MonoBehaviour
{
	protected AIController controller;
	protected Transform player;

	protected float lookingDeviationDuration;
	protected float lookingDeviationDelay;

	protected Vector2 lookingDeviation;

	protected CancellationTokenSource performDevianceLookAsyncTokenSource;

	[SerializeField] protected float raycastFrequency = 1 / 85;
	[SerializeField] protected float detectEdgeDistance = 7;
	[SerializeField] protected float detectWallDistance = 5;
	[SerializeField] protected float minPlayerDistance = 3.5f;
	[SerializeField] protected bool useDirectionDeviance = false;
	[SerializeField, Indent, ShowIf(nameof(useDirectionDeviance))] protected float devianceDirectionMaxHalfAngle = 90f;
	[SerializeField, Indent, ShowIf(nameof(useDirectionDeviance))] protected float devianceMinStartDelay = 0.15f;
	[SerializeField, Indent, ShowIf(nameof(useDirectionDeviance))] protected float devianceMaxStartDelay = 1.05f;
	[SerializeField, Indent, ShowIf(nameof(useDirectionDeviance))] protected float devianceMinDuration = 0.5f;
	[SerializeField, Indent, ShowIf(nameof(useDirectionDeviance))] protected float devianceMaxDuration = 1.5f;

	protected bool bInitialized = false;

	public abstract void OnInit();
	public abstract void OnEnter();
	public abstract void OnUpdate();
	public abstract void OnFixedUpdate();
	public abstract void OnTriggerEnter(Collider other);
	public abstract void OnTriggerExit(Collider other);
	public abstract void OnCollisionEnter(Collision other);
	public abstract void OnCollisionExit(Collision other);
#if UNITY_EDITOR
	public virtual void OnDrawGizmos()
	{
		if (Application.isEditor && !Application.isPlaying || !bInitialized)
			return;
	}
#endif
	protected virtual async UniTask PerformDevianceLookAsync(CancellationToken token)
	{
		if (!useDirectionDeviance)
			return;

		float initDelay = Random.Range(devianceMinStartDelay, devianceMaxStartDelay);
		await UniTask.WaitForSeconds(initDelay, false, PlayerLoopTiming.Update, token);
	}
	protected virtual Vector2 GetDevianceDirectionVector(float devianceMaxHalfAngle, Vector2 forward, int calcAttempts = 20)
	{
		if (!useDirectionDeviance)
			return Vector2.zero;

		Vector2 randDirection = Random.insideUnitCircle.normalized;
		Vector2 forwardDirection = forward.normalized;

		int directionAttempt = 0;
		while (Vector2.Angle(randDirection, forwardDirection) > devianceMaxHalfAngle && directionAttempt < calcAttempts)
		{
			randDirection = Random.insideUnitCircle.normalized;
			directionAttempt++;
		}

		if (directionAttempt >= calcAttempts)
			return Vector2.zero;

		return randDirection;
	}
	public abstract void OnExit();
}