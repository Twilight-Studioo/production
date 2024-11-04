#region

using Core.Input;
using Core.Input.Generated;
using Core.Navigation;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

#endregion

namespace Main.Controller.GameNavigation
{
    public abstract partial class ScreenProtocol : MonoBehaviour, IScreen
    {
        private InputActionEvent cancelAction;
        private InputActionEvent clickAction;
        private InputActionEvent navigateAction;

        [Inject] public InputActionAccessor ActionAccessor { get; set; }

        [Inject] public ScreenController<Navigation> Controller { get; set; }

        public void OnDestroy()
        {
            Destroy(gameObject);
        }

        private partial void OnCancel_Internal(InputAction.CallbackContext ctx);
        private partial void OnNavigation_Internal(InputAction.CallbackContext ctx);
        private partial void OnClick_Internal(InputAction.CallbackContext ctx);

        public void OnCreate()
        {
            cancelAction = ActionAccessor.CreateAction(UI.Cancel);
            navigateAction = ActionAccessor.CreateAction(UI.Navigate);
            clickAction = ActionAccessor.CreateAction(UI.Submit);
        }

        public void OnShow()
        {
            gameObject.SetActive(true);
            cancelAction.Performed += OnCancel_Internal;
           
            navigateAction.Performed += OnNavigation_Internal;
        
            clickAction.Performed += OnClick_Internal;
        }

        public void OnHide()
        {
            cancelAction.Performed -= OnCancel_Internal;
            navigateAction.Performed -= OnNavigation_Internal;
            clickAction.Performed -= OnClick_Internal;
            gameObject.SetActive(false);
        }
    }
}