using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Состояние движения пузырька до пересечения с другим.
/// </summary>
class Moving : MonoBehaviour
{
    private bool _isHit = false;
    /// <summary>
    /// Скорость пузырька.
    /// </summary>
    public Vector2 Velocity {private get; set; }
    
    /// <summary>
    /// Родительский класс пузырька.
    /// </summary>
    public Bubble Bubble { get; set; }

    /// <summary>
    /// Событие врезания шарика в другой шарик.
    /// </summary>
    public event UnityAction<Bubble, Rigidbody2D> HitInBubble;

    void Start()
    {
        if(!Bubble.RigidBody)
            Bubble.RigidBody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        //движение по дискретной формуле ускоренного движения в среде силы тяжести
        Velocity += Physics2D.gravity * Time.fixedDeltaTime;
        Bubble.RigidBody.MovePosition(Bubble.RigidBody.position + Velocity * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        if (!_isHit)//необходимо только первое столкновение с пузырьком
        {
            //Отфильтровываем столкновения
            if (collision.gameObject.layer == LayerMask.NameToLayer("Boll"))
            {
                var bubble = collision.gameObject.GetComponent<Bubble>();
                if (bubble != null)
                {
                    _isHit = true;
                    HitInBubble?.Invoke(bubble, collision.rigidbody);
                    Destroy(this); //убираем состояние
                }
            }
            else
            {
                //оттакливаемся (не мог никак протестировать на нескольких точек контакта, поэтому не уверен в корректности)
                //отталкивание абсолютно упругое
                foreach (ContactPoint2D contact in collision.contacts)
                    Velocity = Velocity - 2 * contact.normal * Vector2.Dot(Velocity, contact.normal);
            }
        }
    }
}