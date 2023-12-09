using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class AIAttackingBase : AIBaseBehavior
{
	[SerializeField] protected WeaponHandler weaponHandler;
	[SerializeField] protected float detectAIDistance = 5;
	[SerializeField] protected float maxPlayerDistance = 5.5f;

	public override void OnInit()
	{
		if (bInitialized)
			return;

		bInitialized = true;

		controller = GetComponent<AIController>();
		player = GameObject.FindWithTag("Player").transform;
	}
}