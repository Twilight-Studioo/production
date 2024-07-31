using Feature.Common.Parameter;
using UniRx;
using UnityEngine;

public class EnemyModel
{
    public readonly ReactiveProperty<bool> IsChasing = new ReactiveProperty<bool>(true);
    public readonly ReactiveProperty<Vector3> Position = new ReactiveProperty<Vector3>();

    private readonly EnemyParams _parameter;

    public EnemyModel(EnemyParams parameter)
    {
        _parameter = parameter;
    }
}
