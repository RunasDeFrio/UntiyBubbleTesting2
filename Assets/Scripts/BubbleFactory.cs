using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Фабрика пузырьков.
/// </summary>
public class BubbleFactory : MonoBehaviour
{
    /// <summary>
    /// Массив префабов для спавна.
    /// </summary>
    [SerializeField]
    private List<GameObject> bubblePrefabs;

    /// <summary>
    /// Сохраннёные копии рассортированные по типу.
    /// </summary>
    private List<Stack<Bubble>> _store;

    private GameManager _gameManager;

    /// <summary>
    /// Кол-во пузырьков на сцене.
    /// </summary>
    public int BubbleInScene { get; private set; }

    public void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        var bubbles = FindObjectsOfType<Bubble>();
        foreach (var bub in bubbles)
        {
            bub.NeedDestroy += Pull;
            bub.NeedDestroy += (bubble) => _gameManager.AddGamePoints(bubble.GamePoints);
        }
        _store = new List<Stack<Bubble>>(bubblePrefabs.Count);

        for (int i = 0; i < bubblePrefabs.Count; i++)
            _store.Add(new Stack<Bubble>(10));
    }

    /// <summary>
    /// Возвращает пузырик, который или достаётся из хранилища или создаётся новый.
    /// </summary>
    /// <param name="type">Тип пузырика</param>
    /// <param name="parent">Родитель</param>
    /// <param name="pos">Позиция для размещения</param>
    /// <returns></returns>
    public Bubble GetForType(int type, Transform parent, Vector2 pos)
    {
        Bubble bubble;
        if (_store[type].Count > 0)
        {
            bubble = _store[type].Pop();
            bubble.transform.position = pos;
            bubble.transform.SetParent(parent);
            bubble.gameObject.SetActive(true);
        }
        else
        {
            BubbleInScene++;
            bubble = Instantiate(bubblePrefabs[type], pos, Quaternion.identity, transform).GetComponent<Bubble>();
            bubble.Type = type;
            _gameManager.Victory += bubble.Joints.BreakAllJoint;
            bubble.NeedDestroy += Pull;
            bubble.NeedDestroy += _gameManager.SubtractBubble;
            bubble.NeedDestroy += (bub) => _gameManager.AddGamePoints(bub.GamePoints);
        }
        return bubble;
    }

    /// <summary>
    /// Возвращает пузырик случайного типа, который или достаётся из хранилища или создаётся новый.
    /// </summary>
    /// <param name="type">Тип пузырика</param>
    /// <param name="parent">Родитель</param>
    /// <param name="pos">Позиция для размещения</param>
    /// <returns></returns>
    public Bubble GetRandom(Transform parent, Vector2 pos)
    {
        int type = Random.Range(0, bubblePrefabs.Count - 1);

        return GetForType(type, parent, pos);
    }

    /// <summary>
    /// Выключить пузырик и сложить его в хранилище. Пузырики при вызове события NeedDestroy автоматически помещаются в хранилище.
    /// </summary>
    /// <param name="bubble"></param>
    public void Pull(Bubble bubble)
    {
        if (bubble != null)
        {
            BubbleInScene--;
            bubble.gameObject.SetActive(false);
            _store[bubble.Type].Push(bubble);
        }
    }
}