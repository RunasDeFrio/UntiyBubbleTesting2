using UnityEngine;
using System.Collections;
using UnityEngine.Events;

/// <summary>
/// Состояние анимации лопания пузырька.
/// </summary>
public class BurstingAnimation : MonoBehaviour
{
    private Transform _tr;
    private float _startTime;
    private Vector2 _startScale;

    public float DestroyTime { get; set; }
    public Vector2 DestinationScale { get; set; }

    public event UnityAction AnimationEnd;

    void Start()
    {
        _tr = transform;
        _startTime = Time.time;
        _startScale = _tr.localScale;
    }

    public void Update()
    {
        float t = (Time.time - _startTime) / DestroyTime;
        if (t <= 1.0f)
            _tr.localScale = Vector2.Lerp(_startScale, DestinationScale, t);
        else
        {
            _tr.localScale = _startScale;
            Destroy(this);
            AnimationEnd?.Invoke();
        }
    }
}
