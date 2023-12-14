using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class AIMovingBase : AIBaseBehavior
{
	public override void OnInit()
	{
		if (bInitialized)
			return;

		bInitialized = true;

		controller = GetComponent<AIController>();
		player = GameObject.FindWithTag("Player").transform;
	}
}