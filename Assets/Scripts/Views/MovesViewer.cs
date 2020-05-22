using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Класс для представления кол-ва ходов на шарике.
/// </summary>
public class MovesViewer : MonoBehaviour
{
    private Text text;
    private Transform _tr;
    public void ViewMoves(int gp)
    {
        gp--;
        if (gp > 0)
            text.text = gp.ToString();
        else
            text.text = "";
    }

    // Start is called before the first frame update
    void Awake()
    {
        _tr = transform;
        text = GetComponent<Text>();

        FindObjectOfType<GameManager>().MovesChange += ViewMoves;
        FindObjectOfType<BubbleShooter>().CreateNextBubble += SetPosition;

    }

    private void SetPosition(Vector2 pos)
    {
        _tr.position = Camera.main.WorldToScreenPoint(pos);
    }
}
