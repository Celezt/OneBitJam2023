using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[HideMonoScript]
public abstract class ScriptableManager : ScriptableObject
{
    private static ScriptableManager _instance;

    protected virtual bool IsSingleton => false;

    protected virtual void GameStart() { }
    protected virtual void GameExit() { }

#if UNITY_EDITOR
    private void OnEnable()
    {
        if (!ValidSingleton())
            return;

        EditorApplication.playModeStateChanged += OnPlayStateChange;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayStateChange;
    }

    private void OnPlayStateChange(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
            GameStart();
        else if (state == PlayModeStateChange.ExitingPlayMode)
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
        if (IsSingleton)
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogError($"Multiple instances of type: {GetType()} is not supported");
                DestroyImmediate(this);
                return false;
            }

            _instance = this;
        }

        return true;
    }
}
