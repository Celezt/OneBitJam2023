using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UltEvents;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "GameManager", menuName = "Game Manager")]
public class GameManager : ScriptableManager<GameManager>
#if UNITY_EDITOR
    , ISerializationCallbackReceiver
#endif
{
    public IEnumerable<ISettings> Settings => _settings;

    [SerializeReference, InlineList(ChildSpace = 8)]
    private List<ISettings> _settings = new();

    [SerializeField, Space(8)]
    private UltEvent _onGameStartEvent;
    [SerializeField]
    private UltEvent _onGameExitEvent;

    protected override void GameStart()
    {
        foreach (var settings in _settings)
            settings.GameStart(_settings);

        _onGameStartEvent.Invoke();
    }

    protected override void GameExit()
    {
        foreach (var settings in _settings)
            settings.GameExit(_settings);

        _onGameExitEvent.Invoke();
    }

    public static void LoadScene(int sceneBuildIndex)
        => SceneManager.LoadScene(sceneBuildIndex);
    public static void LoadScene(string sceneName)
        => SceneManager.LoadScene(sceneName);

    public static void ReloadScene()
        => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    public static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public static void ActivateAllInput()
        => PlayerInput.all.ForEach(x => x.ActivateInput());

    public static void ActivateInput(int playerIndex) 
        => PlayerInput.GetPlayerByIndex(playerIndex).ActivateInput();

    public static void DeactivateAllInput()
    => PlayerInput.all.ForEach(x => x.DeactivateInput());

    public static void DeactivateInput(int playerIndex)
        => PlayerInput.GetPlayerByIndex(playerIndex).DeactivateInput();

#if UNITY_EDITOR
    private bool _isInitialized = false;

    public void OnBeforeSerialize()
    {
        if (!_isInitialized)
        {
            _isInitialized = true;

            bool hasChange = false;
            using var pooledObject = HashSetPool<Type>.Get(out var existingSettings);

            _settings.RemoveAll(x => x == null);

            foreach (Type settingsType in ReflectionUtility.GetTypesWithAttribute<GlobalSettingsAttribute>(AppDomain.CurrentDomain))
            {
                existingSettings.Add(settingsType);

                if (_settings.Any(x => x.GetType() == settingsType))
                    continue;

                var settings = (ISettings)Activator.CreateInstance(settingsType);

                _settings.Add(settings);
                hasChange = true;
            }

            // Remove no longer used settings.
            var toRemove = _settings.Select(x => x.GetType()).Except(existingSettings).ToList();
            foreach (Type settingsType in toRemove)
            {
                int index = _settings.FindIndex(x => x.GetType() == settingsType);

                if (index > -1)
                    hasChange = true;

                _settings.RemoveAt(index);
            }

            _settings.Sort((x, y) =>
            {
                Type xType = x.GetType();
                Type yType = y.GetType();
                int xOrder = xType.GetCustomAttribute<GlobalSettingsAttribute>().Order;
                int yOrder = yType.GetCustomAttribute<GlobalSettingsAttribute>().Order;

                if (xOrder != 0 || yOrder != 0) // Compare using order if any of them uses order.
                    return yOrder.CompareTo(xOrder);

                return xType.Name.CompareTo(yType.Name);
            });

            if (hasChange)
                UnityEditor.EditorUtility.SetDirty(this);
        }
    }

    public void OnAfterDeserialize() 
    {
        _isInitialized = false;
    }
#endif
}
