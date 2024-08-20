#region

using Feature.View;

#endregion

namespace Main.Environment
{
    public interface IGameController
    {
        void OnPossess(PlayerView view);

        void Start();
    }
}