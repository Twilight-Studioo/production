using System;

namespace Feature.Interface
{
    public interface IGameController: IDisposable
    {
        void OnPossess(IPlayerView view);

        void Start();
    }
}