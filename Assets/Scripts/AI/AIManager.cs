using UnityEngine;

[DisallowMultipleComponent]
public class AIManager : MonoBehaviour
{
	[SerializeField] float aiUpdateRatePerSecond = (5f / 60f);

	public delegate void AIUpdateEvent();
	public event AIUpdateEvent OnAIUpdate;

	public static AIManager INSTANCE { get { return instance; } private set { } }

	private static AIManager instance;

	void OnEnable()
	{
		if (instance != null && instance != this)
		{
			Debug.LogError($"Multiple AI Managers in scene not allowed, destroying duplicate!");
			Destroy(this);
		}
		else
		{
			instance = this;
			InvokeRepeating(nameof(UpdateAI), aiUpdateRatePerSecond, aiUpdateRatePerSecond);
		}
	}

	void UpdateAI()
	{
		OnAIUpdate?.Invoke();
	}
}