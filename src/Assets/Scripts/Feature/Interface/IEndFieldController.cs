using System;

namespace Feature.Interface
{
    public interface IEndFieldController
    {
        public void SubscribeToPlayerHealth(IObservable<int> playerHealthObservable);
    }
}