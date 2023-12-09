using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIBaseBehavior : MonoBehaviour
{
	protected AIController controller;
	protected Transform player;
	[SerializeField] protected float detectEdgeDistance = 7;
	[SerializeField] protected float detectWallDistance = 5;
	[SerializeField] protected float minPlayerDistance = 3.5f;

	protected bool bInitialized = false;

	public abstract void OnInit();
	public abstract void OnEnter();
	public abstract void OnUpdate();
	public abstract void OnFixedUpdate();
	public abstract void OnTriggerEnter(Collider other);
	public abstract void OnTriggerExit(Collider other);
	public abstract void OnCollisionEnter(Collision other);
	public abstract void OnCollisionExit(Collision other);
	public abstract void OnExit();
}