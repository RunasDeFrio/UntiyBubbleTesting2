using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Класс для представления конца игры.
/// </summary>
class GameOverViewer : MonoBehaviour
{
    [SerializeField]
    Text _overText;

    [SerializeField]
    GameObject _target;

    GameManager _gameManager;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        var gm = _gameManager;
        gm.GameOver += SetGameOver;
    }

    private void SetGameOver(int gamePoinst, bool isVictory)
    {
        if (isVictory)
        {
            _overText.text = $"Победа!\nСчёт: {gamePoinst}";
        }
        else
        {
            _overText.text = $"Проигрыш!\nСчёт: {gamePoinst}";
        }
        _target.SetActive(true);
    }
}

