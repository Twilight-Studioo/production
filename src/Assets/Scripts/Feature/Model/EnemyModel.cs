#region

using Feature.Common.Parameter;
using UniRx;
using UnityEngine;

#endregion

public class EnemyModel
{
    private readonly EnemyParams _parameter;
    public readonly ReactiveProperty<bool> IsChasing = new(true);
    public readonly ReactiveProperty<Vector3> Position = new();

    public EnemyModel(EnemyParams parameter)
    {
        _parameter = parameter;
    }
}