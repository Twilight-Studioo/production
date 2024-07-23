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

    public float Speed => _parameter.speed;

    public void SetPosition(Vector3 position)
    {
        Position.Value = position;
    }
}
