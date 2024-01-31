using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[HideMonoScript]
public abstract class ScriptableManager<T> : ScriptableManager where T : ScriptableObject
{
    private static ScriptableObject _instance;

    protected virtual void GameStart() { }
    protected virtual void GameExit() { }

#if UNITY_EDITOR
    private void OnEnable()
    {
        if (!ValidSingleton())
            return;

        UnityEditor.EditorApplication.playModeStateChanged += OnPlayStateChange;
    }

    private void OnDisable()
    {
        UnityEditor.EditorApplication.playModeStateChanged -= OnPlayStateChange;
    }

    private void OnPlayStateChange(UnityEditor.PlayModeStateChange state)
    {
        if (state == UnityEditor.PlayModeStateChange.EnteredPlayMode)
            GameStart();
        else if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode)
            GameExit();
    }
#else
    private void OnEnable()
    {
        if (!ValidSingleton())
            return;

        GameStart();
    }

    private void OnDisable()
    {
        GameExit();
    }
#endif

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    private bool ValidSingleton()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogError($"Multiple instances of type: {GetType()} is not supported");
            DestroyImmediate(this);
            return false;
        }

        _instance = this;

        return true;
    }

}

public abstract class ScriptableManager : ScriptableObject
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Initialize()
    {
        Addressables.LoadAssetsAsync<ScriptableManager>("manager", x =>
        {

        });
    }
}
