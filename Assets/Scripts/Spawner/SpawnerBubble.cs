using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Спавнер пузырьков из JSON файла и отправляет данные о белых шарах.
/// </summary>
public class SpawnerBubble : MonoBehaviour
{
    Dictionary<string, Bubble> _bubbles;

    private Transform _tr;
    private BubbleFactory _spawner;
    private GameManager _gm;

    void Start()
    {
        _tr = transform;
        _spawner = FindObjectOfType<BubbleFactory>();
        _gm = FindObjectOfType<GameManager>();
        ReadLevel("lvl1");
    }

    /// <summary>
    /// Чтение уровня из JSON файла и спавн пузырьков.
    /// </summary>
    /// <param name="lvlName"></param>
    void ReadLevel(string lvlName)
    {
        TextAsset json = Resources.Load<TextAsset>(lvlName);
        SpawnInfo[] spawnInfos = JsonHelper.FromJson<SpawnInfo>(json.text);
        
        _bubbles = new Dictionary<string, Bubble>(spawnInfos.Length);

        int countPriorityBubble = 0;
        foreach (var info in spawnInfos)
        {
            Bubble newBubble = _spawner.GetForType(info.type, _tr, info.position);
            newBubble.RigidBody.gravityScale = 1;
            _bubbles.Add(info.id, newBubble);
            if (newBubble.Type == 0)
                countPriorityBubble++;
        }
        foreach (var info in spawnInfos)
            SetNeighbors(_bubbles[info.id], info);
        _gm.CountNeedBubble = countPriorityBubble;
    }

    /// <summary>
    /// Установка упругих связей с соседями.
    /// </summary>
    /// <param name="bubble"></param>
    /// <param name="info"></param>
    void SetNeighbors(Bubble bubble, SpawnInfo info)
    {
        List<Bubble> toConnect = new List<Bubble>(info.connections.Count);
        foreach(string connect in info.connections)
        {
            if (connect == "Ground")
                toConnect.Add(null);
            else if (_bubbles.ContainsKey(connect))
                toConnect.Add(_bubbles[connect]);
        }

        bubble.Joints.CreateSpringJoints(toConnect);
    }
}
