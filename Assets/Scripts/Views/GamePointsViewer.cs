using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Класс для представления кол-ва заработанных очков.
/// </summary>
public class GamePointsViewer : MonoBehaviour
{
    private Text text;

    public void ViewGamePoints(int gp)
    {
        text.text = $"Ваш счёт: {gp}";
    }

    // Start is called before the first frame update
    void Awake()
    {
        text = GetComponent<Text>();

        GameManager gm = FindObjectOfType<GameManager>();
        gm.GamePointsChange += ViewGamePoints;
    }
}
