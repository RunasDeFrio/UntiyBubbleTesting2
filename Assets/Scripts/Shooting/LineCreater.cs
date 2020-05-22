using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Класс рисования баллистической траектории движения.
/// </summary>
public class LineCreater : MonoBehaviour
{
    [SerializeField]
    private LineRenderer trajectoryLine;
    [SerializeField]
    private LineRenderer lineScatter0;
    [SerializeField]
    private LineRenderer lineScatter1;

    /// <summary>
    /// Размер игровой области по которой могут перемещаться объекты.
    /// </summary>
    [SerializeField]
    private Rect gameZone;

    /// <summary>
    /// Длина линии разброса.
    /// </summary>
    [SerializeField]
    private float scatterLength;

    /// <summary>
    /// Общее время для симуляции.
    /// </summary>
    [SerializeField]
    private float AllTime = 5;

    /// <summary>
    /// Промежуток времени за которую происходит такт симуляции.
    /// </summary>
    [SerializeField]
    private float deltaTime = 1 / 10.0f;

    /// <summary>
    /// Параметры выстрела.
    /// </summary>
    public ShootInfo shootInfo { get; set; }

    /// <summary>
    /// Проверка выхода объекта за границы игровой зоны.
    /// </summary>
    /// <param name="pos">Позиция объекта</param>
    /// <param name="radius">Радиус круга.</param>
    /// <param name="normal">Нормаль к бортику игровой зоны. С учётом длины выхода за границы.</param>
    /// <returns></returns>
    private bool CircleZoneIntersect(Vector2 pos, float radius, out Vector3 normal)
    {
        Vector2 min = gameZone.position;
        Vector2 max = gameZone.position + gameZone.size;
        normal = Vector2.zero;
        if (pos.x + radius > max.x)
        {
            normal = Vector3.left * (pos.x + radius - max.x);
            return true;
        }
        else if (pos.y + radius > max.y)
        {
            normal = Vector3.down * (pos.y + radius - max.y);
            return true;
        }
        else if (pos.x - radius < min.x)
        {
            normal = Vector3.right * (min.x - (pos.x - radius));
            return true;
        }
        else if (pos.y - radius < min.y)
        {
            normal = Vector3.up * (min.y - (pos.y - radius));
            return true;
        }
        else return false;
    }

    private void MakeScatterLine(Vector2 startPoint, Vector2 velocity)
    {
        if (velocity.sqrMagnitude >= 0.9 * shootInfo.maxStartVelocity * shootInfo.maxStartVelocity)
        {
            lineScatter0.positionCount = 2;
            lineScatter1.positionCount = 2;

            lineScatter0.SetPosition(0, startPoint);
            lineScatter1.SetPosition(0, startPoint);

            Vector2 velocityRot = Quaternion.Euler(0, 0, shootInfo.angleScatter) * velocity;
            lineScatter0.SetPosition(1, startPoint + scatterLength * (velocityRot).normalized );

            velocityRot = Quaternion.Euler(0, 0, -shootInfo.angleScatter) * velocity;
            lineScatter1.SetPosition(1, startPoint + scatterLength * (velocityRot).normalized);
        }
        else
        {
            lineScatter0.positionCount = 0;
            lineScatter1.positionCount = 0;
        }
    }

    /// <summary>
    /// Создание траектории движения центра круга с учётом гравитации и отталкивания от границ игровой области.
    /// </summary>
    /// <param name="startPoint">Точка начала движения.</param>
    /// <param name="velocity">Скорость объекта.</param>
    /// <param name="radius">Радиус круга.</param>
    public void MakeLine(Vector2 startPoint, Vector2 velocity, float radius)
    {
        MakeScatterLine(startPoint, velocity);

        int lineSize = Mathf.RoundToInt(AllTime / deltaTime);
        trajectoryLine.positionCount = lineSize;
        
        Vector3[] points = new Vector3[lineSize];
        //преобразуем в 3d
        Vector3 velocity3d = velocity;
        Vector3 gravity = Physics2D.gravity;

        //симуляция движения
        points[0] = startPoint;
        for (int i = 1; i < lineSize; i++)
        {
            //симуляция по формуле ускоренного движения
            velocity3d += gravity * deltaTime;
            points[i] = points[i - 1] + velocity3d * deltaTime;

            //При столкновении с бортиком игровой зоны - отталкиваемся
            Vector3 normal;
            if (CircleZoneIntersect(points[i], radius, out normal))
            {
                velocity3d = velocity3d - 2 * normal * Vector3.Dot(velocity3d, normal) / Vector3.Dot(normal, normal);
                //перемещаем центр круга в точку, в которой находился круг при столкновении
                points[i] += -(points[i] - points[i - 1]).normalized * normal.magnitude;
            }
        }
        trajectoryLine.SetPositions(points);
    }

    /// <summary>
    /// Очищаем линию.
    /// </summary>
    public void ClearLine()
    {
        trajectoryLine.positionCount = 0;
        lineScatter0.positionCount = 0;
        lineScatter1.positionCount = 0;
    }
}
