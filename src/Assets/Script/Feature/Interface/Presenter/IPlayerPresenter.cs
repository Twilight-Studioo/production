using UnityEngine;
namespace Script.Feature.Interface.Presenter
{
    public interface IPlayerPresenter
    {
        public void Move(Vector2 vector2);

        public void Jump();
        public void SwapMode();
        public void Attack();
    }
}