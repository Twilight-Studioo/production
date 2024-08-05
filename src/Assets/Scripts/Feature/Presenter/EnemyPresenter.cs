using UniRx;
using UnityEngine;

public class EnemyPresenter
{
    private readonly EnemyModel _model;
    private readonly EnemyView _view;

    public EnemyPresenter(EnemyModel model, EnemyView view)
    {
        _model = model;
        _view = view;

        _view.SetPresenter(this);

        _model.Position.Subscribe(pos => _view.UpdatePosition(pos));

        Observable.EveryUpdate()
            .Where(_ => _model.IsChasing.Value)
            .Subscribe(_ => MoveTowardsPlayer());
    }

    public void StartChasing()
    {
        _model.IsChasing.Value = true;
    }

    public void StopChasing()
    {
        _model.IsChasing.Value = false;
    }

    private void MoveTowardsPlayer()
    {
        UnityEngine.Vector3 currentPosition = _view.GetCurrentPosition();
        UnityEngine.Vector3 targetPosition = new UnityEngine.Vector3(_view.GetPlayerPosition().x, currentPosition.y, currentPosition.z);
        UnityEngine.Vector3 newPosition = UnityEngine.Vector3.MoveTowards(currentPosition, targetPosition, _model.Speed * Time.deltaTime);
        _model.SetPosition(newPosition);
    }
}
