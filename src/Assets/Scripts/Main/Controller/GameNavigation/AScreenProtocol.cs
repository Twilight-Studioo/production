using Core.Input;
using Core.Input.Generated;
using Core.Navigation;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Main.Controller.GameNavigation
{
    public abstract partial class AScreenProtocol: AScreen
    {
        [Inject]
        public InputActionAccessor ActionAccessor { get; set; }
        
        [Inject]
        public ScreenController<Navigation> Controller { get; set; }
        
        private InputActionEvent cancelAction; 
        private InputActionEvent navigateAction;
        private InputActionEvent clickAction;
        
        private partial void OnCancel_Internal(InputAction.CallbackContext ctx);
        private partial void OnNavigation_Internal(InputAction.CallbackContext ctx);
        private partial void OnClick_Internal(InputAction.CallbackContext ctx);
        public override void OnCreate()
        {
            cancelAction = ActionAccessor.CreateAction(UI.Cancel);
            cancelAction.Started += OnCancel_Internal;
            cancelAction.Pause();
            
            navigateAction = ActionAccessor.CreateAction(UI.Navigate);
            navigateAction.Started += OnNavigation_Internal;
            navigateAction.Pause();

            clickAction = ActionAccessor.CreateAction(UI.Submit);
            clickAction.Started += OnClick_Internal;
            clickAction.Pause();
        }
        
        public override void OnShow()
        {
            cancelAction.Resume();
            navigateAction.Resume();
            clickAction.Resume();
        }

        public override void OnHide()
        {
            cancelAction.Pause();
            navigateAction.Pause();
            clickAction.Pause();
        }

        public override void OnDestroy()
        {
            cancelAction.Clear();
            navigateAction.Clear();
            clickAction.Clear();
        }
    }

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