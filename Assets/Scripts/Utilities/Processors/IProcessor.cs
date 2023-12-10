using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProcessor<T> : IProcessor where T : new()
{
    public T Process(T value);
}

public interface IProcessor
{

}
