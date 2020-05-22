using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Класс-хранилище параметров для спавна пузырька.
/// </summary>
[Serializable]
public class SpawnInfo
{
    public string id;
    public Vector2 position;
    public int type;
    public List<string> connections;
}
