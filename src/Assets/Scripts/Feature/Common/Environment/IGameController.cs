using Feature.View;

namespace Feature.Common.Environment
{
    public interface IGameController
    {
        void OnPossess(PlayerView view);

        void Start();
    }
}