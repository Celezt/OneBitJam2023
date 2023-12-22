using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "GameManager", menuName = "Game Manager")]
public class GameManager : ScriptableManager, ISerializationCallbackReceiver
{
    public IEnumerable<ISettings> Settings => _settings;

    protected override bool IsSingleton => true;

    [SerializeReference, InlineList(ChildSpace = 8)]
    private List<ISettings> _settings = new();

    [SerializeField, PropertySpace(SpaceBefore = 8)]
    private UnityEvent _onGameStart;
    [SerializeField]
    private UnityEvent _onGameExit;

    protected override void GameStart()
    {
        foreach (var settings in _settings)
            settings.GameStart(_settings);

        _onGameStart.Invoke();
    }

    protected override void GameExit()
    {
        foreach (var settings in _settings)
            settings.GameExit(_settings);

        _onGameExit.Invoke();
    }

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
