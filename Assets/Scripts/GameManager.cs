using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// Менеджер игры. Управляет событиями конца игры.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Кол-во игровых очков.
    /// </summary>
    [SerializeField]
    private int _gamePoinst = 0;

    /// <summary>
    /// Доступные ходы.
    /// </summary>
    [SerializeField]
    private int _moves = 5;
    /// <summary>
    /// Счётчик пузырьков которых нужно уничтожить игроку.
    /// </summary>
    private int _needBubbleDestroy { get; set; }


    private BubbleFactory _factory;


    public int Moves { get => _moves; private set => _moves = value; }
    public bool isVictory { get; set; }

    /// <summary>
    /// Кол-во пузырьков на сцене, которые нужно уничтожить
    /// </summary>
    public int CountNeedBubble { get; set; }

    public event UnityAction<int, bool> GameOver;
    public event UnityAction Victory;


    public event UnityAction<int> MovesChange;
    public event UnityAction<int> GamePointsChange;

    private void Start()
    {
        isVictory = false;
        _factory = FindObjectOfType<BubbleFactory>();
        GamePointsChange?.Invoke(_gamePoinst);
        MovesChange?.Invoke(Moves);
    }

    /// <summary>
    /// Объявить рекорд или объявить GameOver.
    /// </summary>
    private void SetGameOver()
    {
        GameOver?.Invoke(_gamePoinst, isVictory);
    }

    /// <summary>
    /// Добавить очки к счёту.
    /// </summary>
    /// <param name="gp"></param>
    public void AddGamePoints(int gp)
    {
        _gamePoinst += gp;
        GamePointsChange?.Invoke(_gamePoinst);
    }

    /// <summary>
    /// Вычесть кол-во нужных пузырей для выйгрыша.
    /// </summary>
    /// <param name="gp"></param>
    public void SubtractBubble(Bubble bubble)
    {
        if(bubble.Type == 0)
            _needBubbleDestroy++;
        if (_needBubbleDestroy > CountNeedBubble * 0.6 && !isVictory)
        {
            isVictory = true;
            Victory?.Invoke();
        }
        if (_factory.BubbleInScene <= Mathf.Clamp(Moves, 0, 2))
            SetGameOver();
    }

    /// <summary>
    /// Забрать ход.
    /// </summary>
    /// <param name="gp"></param>
    public void SubtractMoves()
    {
        Moves--;
        MovesChange?.Invoke(Moves);
        if (Moves < 1)
            SetGameOver();
    }

}
