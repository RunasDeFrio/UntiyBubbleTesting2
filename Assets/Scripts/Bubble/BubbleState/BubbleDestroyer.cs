using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс для триггера уничтожения пузырьков внизу игровой зоны.
/// </summary>
public class BubbleDestroyer : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.GetComponent<Bubble>().Burst();
    }

}
