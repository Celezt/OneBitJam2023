using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public interface IProcessor<T> : IProcessor where T : new()
{
    public T Process(T value);

    public static T Process(IList<IProcessor<T>> processors, T value)
    {
        for (int i = 0; i < processors.Count; i++)
            value = processors[i].Process(value);

        return value;
    }
}

public interface IProcessor
{
    public static T Process<T>(IList<IProcessor> processors, T value) where T : new()
    {
        for (int i = 0; i < processors.Count; i++)
        {
            if (processors[i] is IProcessor<T> p)
                value = p.Process(value);
        }

        return value;
    }
}
