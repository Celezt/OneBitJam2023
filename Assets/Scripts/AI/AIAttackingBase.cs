using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public abstract class AIAttackingBase : AIBaseBehavior
{
	[SerializeField] protected WeaponHandler weaponHandler;
	[SerializeField] protected float detectAIDistance = 5;
	[SerializeField] protected float maxPlayerDistance = 5.5f;

	protected Vector3 playerForward;
	protected Vector3 playerRight;

	public override void OnInit()
	{
		if (bInitialized)
			return;

		bInitialized = true;

		controller = GetComponent<AIController>();
		player = GameObject.FindWithTag("Player").transform;
	}

	public void SetPlayerForward(Vector2 lookAtPosition)
	{
		if (!player)
			return;

		playerForward = (new Vector3(lookAtPosition.x, 0, lookAtPosition.y) - player.transform.position).normalized;
		playerRight = Vector3.Cross(Vector3.up, playerForward);
	}

	// public void SetPlayerForward(InputAction.CallbackContext ctx)
	// {
	// 	if (!player)
	// 		return;
	// 	playerForward = (new Vector3(ctx.ReadValue<Vector2>().x, 0, ctx.ReadValue<Vector2>().y) - player.transform.position).normalized;
	// 	playerRight = Vector3.Cross(Vector3.up, playerForward);
	// }
}