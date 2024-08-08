#region

using Feature.View;

#endregion

namespace Feature.Common.Environment
{
    public interface IGameController
    {
        void OnPossess(PlayerView view);

        void Start();
    }
}