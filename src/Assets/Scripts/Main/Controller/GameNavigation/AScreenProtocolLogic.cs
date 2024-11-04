using Core.Navigation;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Main.Controller.GameNavigation
{
    public abstract partial class AScreenProtocol : AScreen
    {
        private partial void OnCancel_Internal(InputAction.CallbackContext ctx)
        {
            OnCancel();
        }

        protected abstract void OnCancel();
        
        private partial void OnNavigation_Internal(InputAction.CallbackContext ctx)
        {
            var value = ctx.ReadValue<Vector2>();
            OnNavigation(value);
        }
        
        protected abstract void OnNavigation(Vector2 value);
        
        private partial void OnClick_Internal(InputAction.CallbackContext ctx)
        {
            OnClick();
        }
        protected abstract void OnClick();
        
    }
}