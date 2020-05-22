using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Основной класс пузырька, осуществляет переключение состояний при получении событий.
/// </summary>
/// Хотелось бы отметить, что реализация компонент-состояние возможно не самый оптимальный, 
/// однако даёт гибкость в управлении объектом.
public class Bubble : MonoBehaviour
{
    /// <summary>
    /// Кол-во очков которые будут присвоены при уничтожении пузыря.
    /// </summary>
    [SerializeField]
    private int _gamePoints;

    /// <summary>
    /// Кол-во однотипных соседених шариков, необходимое для уничтожения.
    /// </summary>
    [SerializeField]
    private int _maxCountNeighborsWithSameType = 2;

    /// <summary>
    /// Радиус для проверки соседних пузырей для связи.
    /// </summary>
    [SerializeField]
    private float _radiusCheckingNeighbors = 1.5f;

    private bool _maxVelocity = false;

    public int Type { get; set; }

    public int GamePoints { get => _gamePoints; set => _gamePoints = value; }

    /// <summary>
    /// Флаг для контроля выстреленного пузыря.
    /// </summary>
    public bool IsShooting { get; set; }
    
    public Rigidbody2D RigidBody { get; set; }

    public CircleCollider2D Collider { get; set; }

    /// <summary>
    /// Соединения между пузырьками.
    /// </summary>
    public Joints Joints { get; private set; }

    /// <summary>
    /// Событие начала процесса лопанья пузыря.
    /// </summary>
    public event UnityAction<Rigidbody2D> BurstStart;

    /// <summary>
    /// Событие-сигнал о необходимости уничтожения объекта.
    /// </summary>
    public event UnityAction<Bubble> NeedDestroy;

    /// <summary>
    /// Событие окончания процесса передвижения.
    /// </summary>
    public event UnityAction EndMove;

    private void Awake()
    {
        IsShooting = false;
        RigidBody = GetComponent<Rigidbody2D>();
        Collider = GetComponent<CircleCollider2D>();
        Joints = new Joints(this);
        Joints.RegistrationSpringJoints(GetComponents<SpringJoint2D>());
    }

    private void OnEnable()
    {
        IsShooting = false;
        _maxVelocity = false;

        RigidBody.velocity = Vector2.zero;
        RigidBody.isKinematic = false;
        Collider.enabled = true;

        RigidBody.gravityScale = 1;
    }

    private void OnDisable()
    {
        BurstStart = null;
        EndMove = null;
    }


    /// <summary>
    /// Найти соседей данного пузыря и если кол-во однотипных пузырьков достаточно, то уничтожаем их.
    /// </summary>
    /// <returns></returns>
    private List<Bubble> GetNeighbors()
    {
        Collider2D[] neighborsColliders = Physics2D.OverlapCircleAll(RigidBody.position, _radiusCheckingNeighbors, 1 << LayerMask.NameToLayer("Boll"));
        List<Bubble> bubbleBonds = new List<Bubble>(neighborsColliders.Length);
        int countBubblesWithSameType = 0;
        foreach (Collider2D colliderBubble in neighborsColliders)
        {
            var bub = colliderBubble.GetComponent<Bubble>();
            if (bub != this)
            {
                bubbleBonds.Add(bub);
                if (bub.Type == Type)
                    countBubblesWithSameType++;
            }
        }
        if (countBubblesWithSameType > _maxCountNeighborsWithSameType)
        {
            Burst();
            foreach (var bubble in bubbleBonds)
                if (bubble.Type == Type)
                    bubble.Burst();
            return null;
        }
        else
        {
            return bubbleBonds;
        }
    }


    /// <summary>
    /// Запуск движения пузыря с использованием расчётов.
    /// </summary>
    /// <param name="velocity">Начальная скорость движения пузыря.</param>
    /// <param name="maxVelocity">Была ли задана максимальная скорость?</param>
    public void StartMove(Vector2 velocity, bool maxVelocity)
    {
        _maxVelocity = maxVelocity;

        var _moving = gameObject.AddComponent<Moving>();
        _moving.Bubble = this;
        _moving.Velocity = velocity;
        _moving.HitInBubble += ProcessHitOnBubble;
        BurstStart += (arg0) => Destroy(_moving);
    }

    /// <summary>
    /// Обработка удара по пузырьку.
    /// </summary>
    /// <param name="bubble"></param>
    /// <param name="rb"></param>
    public void ProcessHitOnBubble(Bubble bubble, Rigidbody2D rb)
    {
        EndMove?.Invoke();
        IsShooting = false;

        if (_maxVelocity)
        {
            RigidBody.position = rb.position;
            bubble.Burst();
        }
        RigidBody.gravityScale = 1;

        List<Bubble> neighbors = GetNeighbors();
        if(neighbors != null)
            Joints.CreateSpringJoints(neighbors);
    }

    /// <summary>
    /// Лопнуть пузырь.
    /// </summary>
    public void Burst()
    {
        BurstStart?.Invoke(RigidBody);

        if (IsShooting)
            EndMove?.Invoke();

        RigidBody.velocity = Vector2.zero;
        RigidBody.isKinematic = true;
        Collider.enabled = false;
        var anim = gameObject.AddComponent<BurstingAnimation>();
        anim.DestroyTime = 0.5f;
        anim.DestinationScale = 1.25f * transform.localScale;
        anim.AnimationEnd += () => NeedDestroy?.Invoke(this);
    }
}