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
            cancelAction.Performed += OnCancel_Internal;
            cancelAction.Pause();
            
            navigateAction = ActionAccessor.CreateAction(UI.Navigate);
            navigateAction.Performed += OnNavigation_Internal;
            navigateAction.Pause();

            clickAction = ActionAccessor.CreateAction(UI.Submit);
            clickAction.Performed += OnClick_Internal;
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
}