using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum AIState
{
	wandering,
	attacking,
	undefined
}

public class AIController : MonoBehaviour
{
	[Header("Generic AI settings")]
	[SerializeField] MoveBehaviour moveController;
	[SerializeField] LookTurretBehaviour lookController;
	[SerializeField] AnimationBehaviour animationController;
	[SerializeField] AIMovingBase wanderingBehavior;
	[SerializeField] AIAttackingBase attackingBehavior;

	#region OLD-SYSTEM
	// [SerializeField] float minPlayerDistance = 3.5f;
	// [SerializeField] float minEdgeDistance = 7;

	// [Header("Wander behavior settings")]
	// [SerializeField] float maxMoveTime = 2.5f;
	// [SerializeField] float minWallDistance = 5;

	// [Header("Attack behavior settings")]
	// [SerializeField] float maxPlayerDistance = 5.5f;
	// [SerializeField] float minCirclingDirectionSwitchTime = 2;
	// [SerializeField] float maxCirclingDirectionSwitchTime = 4.5f;
	// [SerializeField] float maxLookHalfAngle = 60f;
	// [SerializeField] float playerFaceAIMinHalfAngle = 5f;
	// [SerializeField] WeaponHandler weaponHandler;
	#endregion

	public AIState activeState { get; private set; } = AIState.undefined;

	private AIBaseBehavior activeBehavior;

	#region OLD-SYSTEM
	// private AIBaseBehavior wanderBehavior;
	// private AIBaseBehavior attackBehavior;
	#endregion

	// Start is called before the first frame update
	void Start()
	{
		#region OLD-SYSTEM
		// Transform player = GameObject.FindWithTag("Player").transform;
		// wanderBehavior = new AIWanderBehavior(this, maxMoveTime, minPlayerDistance, minEdgeDistance, minWallDistance, player);
		// attackBehavior = new AIAttackBehavior(this, minPlayerDistance, maxPlayerDistance, minEdgeDistance, minCirclingDirectionSwitchTime, maxCirclingDirectionSwitchTime, minWallDistance, playerFaceAIMinHalfAngle, player, weaponHandler);
		#endregion

		if (!wanderingBehavior)
			wanderingBehavior = GetComponent<AIMovingBase>();

		if (!attackingBehavior)
			attackingBehavior = GetComponent<AIAttackingBase>();

		SwitchBehavior(AIState.wandering);
	}

	// Update is called once per frame
	void Update()
	{
		activeBehavior?.OnUpdate();
	}

	public void SwitchBehavior(AIState newState)
	{
		if (newState == activeState)
			return;

		activeBehavior?.OnExit();
		switch (newState)
		{
		case AIState.wandering:
			activeBehavior = wanderingBehavior; //wanderBehavior;
			break;
		case AIState.attacking:
			activeBehavior = attackingBehavior; //attackBehavior;
			break;
		default:
			break;
		}
		activeBehavior?.OnInit();
		activeBehavior?.OnEnter();
		activeState = newState;
	}

	public void Move(Vector2 direction) => moveController.Move(direction);
	public Vector3 GetMoveDirection() => moveController.Direction;
	public bool GetIsMoving() => moveController.IsMoving;
	public Quaternion GetMoveLookRotation() => moveController.LookRotation;
	public float GetMoveForce() => moveController.MoveForce;

	public void LookAt(Vector2 target) => lookController.LookAt(target);
	public void Look(Vector2 direction) => lookController.Look(direction);
	public Vector2 GetLookDirection() => lookController.LookDirection;
	public Vector3 GetLookTargetPosition() => lookController.TargetPosition;
	public void SetLookUseDirection(bool useDirection) => lookController.UseDirection = useDirection;
	public bool GetLookUseDirection() => lookController.UseDirection;

	public void SetMoveAnimationSpeed(float speed) => animationController.SetSpeed(speed);
	public void StopMoveAnimation() => animationController.SetSpeed(0.0f);
}