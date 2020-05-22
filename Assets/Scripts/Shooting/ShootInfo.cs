using System;

/// <summary>
/// Параметры стрельбы пузырьком.
/// </summary>
[Serializable]
public struct ShootInfo
{
    /// <summary>
    /// Максимальная скорость выстреливания шарика.
    /// </summary>
    public float maxStartVelocity;

    /// <summary>
    /// Максимальный радиус для прицеливания.
    /// </summary>
    public float maxAimingRadius;

    /// <summary>
    /// Угол разброса в градусах.
    /// </summary>
    public float angleScatter;
}