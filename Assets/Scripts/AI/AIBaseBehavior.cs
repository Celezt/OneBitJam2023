using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIBaseBehavior
{
	protected AIController controller;
	protected MoveBehaviour player;
	protected float detectEdgeDistance;
	protected float detectWallDistance;

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