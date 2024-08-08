#region

using Feature.View;
using UniRx;

#endregion

namespace Feature.Presenter
{
    public class EnemyPresenter
    {
        private readonly EnemyView enemyView;
        private readonly CompositeDisposable swapTimer;

        public EnemyPresenter(EnemyView enemyView, EnemyModel enemyModel)
        {
            this.enemyView = enemyView;
            swapTimer = new();
        }

        public void OnPossess(EnemyView view)
        {
            enemyView.Execute();
        }
    }
}