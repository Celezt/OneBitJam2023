using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISettings
{
    public void GameStart(IEnumerable<ISettings> settings);
    public void GameExit(IEnumerable<ISettings> settings);
}
