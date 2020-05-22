using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Класс для стрельбы шариками.
/// </summary>
public partial class BubbleShooter : MonoBehaviour
{
    /// <summary>
    /// Смещение позиции для пузырька в очереди.
    /// </summary>
    [SerializeField]
    private Vector3 _nextBubblePosOffset;

    /// <summary>
    /// Параметры выстрела шарика. Задаётся в инспекторе.
    /// </summary>
    [SerializeField]
    private ShootInfo shootInfo;

    /// <summary>
    /// Ссылка на следующий в очереди шарик.
    /// </summary>
    private Bubble _nextBubble;

    private LineCreater _lineCreater;

    private Transform _tr;

    private GameManager _gameManager;

    private BubbleFactory _spawner;

    /// <summary>
    /// События создания нового пузырька.
    /// </summary>
    public event UnityAction<Vector2> CreateNextBubble;

    void Start()
    {
        _tr = transform;
        _lineCreater = FindObjectOfType<LineCreater>();
        _spawner = FindObjectOfType<BubbleFactory>();
        _gameManager = FindObjectOfType<GameManager>();

        _lineCreater.shootInfo = shootInfo;
        CreateNextBubble += (arg) => _gameManager.SubtractMoves();
        CreateBubble();
        CreateShootingBubble();
    }


    /// <summary>
    /// Создание следующего пузырька
    /// </summary>
    void CreateBubble()
    {
        CreateNextBubble?.Invoke(_tr.position + _nextBubblePosOffset);

        if (_gameManager.Moves > 1)
        {
            _nextBubble = _spawner.GetRandom(_tr, _tr.position + _nextBubblePosOffset);
            _nextBubble.gameObject.layer = LayerMask.NameToLayer("UI");
            _nextBubble.Collider.enabled = false;
            _nextBubble.RigidBody.gravityScale = 0;
            _nextBubble.IsShooting = false;
        }
        else
        { 
            _nextBubble = null; 
        }
    }

    /// <summary>
    /// Создание пузырька, готового к выстрелу
    /// </summary>
    void CreateShootingBubble()
    {
        var bubble = _nextBubble;
        CreateBubble();

        if (!bubble)
            return;

        bubble.RigidBody.position = _tr.position;
        bubble.Collider.enabled = true;
        bubble.EndMove += CreateShootingBubble; //создадим ещё один шарик, когда прошлый закончит движение

        bubble.RigidBody.gravityScale = 0;
        bubble.IsShooting = true;

        //прикрепляем к пузырьку состояние выстреливания
        var shooting = bubble.gameObject.AddComponent<Shooting>();
        bubble.NeedDestroy += (arg) => Destroy(shooting);

        shooting.Bubble = bubble;
        shooting.Shoot += (velocity, maxVelocity) => 
        {
            bubble.StartMove(velocity, maxVelocity);//при выстреле пузырёк должен начать двигаться
            bubble.gameObject.layer = LayerMask.NameToLayer("Boll");
            _lineCreater.ClearLine(); 
        }; //линия должна очиститься при выстреле
        shooting.Aiming += _lineCreater.MakeLine; //при прицеливании линия должна рисоваться
        shooting.shootInfo = shootInfo;
    }

}
