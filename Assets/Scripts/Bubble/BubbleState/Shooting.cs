﻿using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Состояние пузырька выстреливания и прицеливания.
/// </summary>
public class Shooting : MonoBehaviour
{
    private Camera _camera;
    private Transform _tr;

    //начальная позиция и позиция натяжения
    private Vector2 _startPos;
    private Vector2 _tensionPos;

    private bool _isTouch = false;

    /// <summary>
    /// Родительский класс пузырька.
    /// </summary>
    public Bubble Bubble { get; set; }

    public ShootInfo shootInfo { get; set; }


    /// <summary>
    /// Событие выстреливания пузырька.
    /// Начальная скорость движения. Флаг максимальной скорости.
    /// </summary>
    public event UnityAction<Vector2, bool> Shoot;

    /// <summary>
    /// Событие прицеливания пузырька.
    /// Начальная позиция. Начальная скорость движения. Радиус пузырька.
    /// </summary>
    public event UnityAction<Vector2, Vector2, float> Aiming;


    void Start()
    {
        _camera = Camera.main;
        _tr = transform;

        _startPos = Bubble.RigidBody.position;
    }

    private void Update()
    {
        if(_isTouch)
            SetBubblePosition(_camera.ScreenToWorldPoint(Input.mousePosition));
    }


    private void OnMouseDrag()
    {
        _isTouch = true;
        SetBubblePosition(_camera.ScreenToWorldPoint(Input.mousePosition));
    }

    /// <summary>
    /// Устанавить позицию 
    /// </summary>
    private void SetBubblePosition(Vector2 tensionPos)
    {
        //Ограничение позиции натяжения
        _tensionPos = _startPos - Vector2.ClampMagnitude(_startPos - tensionPos, shootInfo.maxAimingRadius);

        
        Bubble.RigidBody.MovePosition(_tensionPos);
        //Отправляем параметры прицеливания
        Aiming?.Invoke(_tensionPos,
                       shootInfo.maxStartVelocity / shootInfo.maxAimingRadius * (_startPos - _tensionPos),
                       Bubble.Collider.radius * _tr.localScale.x);
    }

    private void OnMouseUp()
    {
        _isTouch = false;
        var velocity =  (_startPos - _tensionPos)/ shootInfo.maxAimingRadius;

        bool isMaxVelocity = velocity.sqrMagnitude >= 0.9;

        if (isMaxVelocity)
            velocity = Quaternion.Euler(0, 0, Random.Range(-shootInfo.angleScatter, shootInfo.angleScatter)) * velocity;
        
        velocity *= shootInfo.maxStartVelocity;
        Shoot?.Invoke(velocity, isMaxVelocity);

        Destroy(this);
    }

}
