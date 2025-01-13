#region

using System;
using Core.Input;
using Core.Input.Generated;
using Core.Navigation;
using Core.Utilities;
using Main.Scene;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

#endregion

namespace Main.Controller.GameNavigation
{
    public abstract partial class ScreenProtocol<T> : MonoBehaviour, IScreen where T : Enum
    {
        private InputActionEvent cancelAction;
        private InputActionEvent clickAction;
        private InputActionEvent navigateAction;

        [Inject] public InputActionAccessor ActionAccessor { get; set; }

        [Inject] public ScreenController<T> Controller { get; set; }

        protected RootInstance RootInstance => RootInstance.Shared;

        public void OnRequireDestroy()
        {
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }

        public void OnCreate()
        {
            cancelAction = ActionAccessor.CreateAction(UI.Cancel).CheckNull();
            navigateAction = ActionAccessor.CreateAction(UI.Navigate).CheckNull();
            clickAction = ActionAccessor.CreateAction(UI.Submit).CheckNull();
        }

        public virtual void OnShow()
        {
            cancelAction.Performed += OnCancel_Internal;
            navigateAction.Performed += OnNavigation_Internal;
            clickAction.Performed += OnClick_Internal;

            gameObject.SetActive(true);
        }

        public virtual void OnHide()
        {
            cancelAction.CheckNull().Performed -= OnCancel_Internal;
            navigateAction.CheckNull().Performed -= OnNavigation_Internal;
            clickAction.CheckNull().Performed -= OnClick_Internal;
            gameObject.SetActive(false);
        }

        private partial void OnCancel_Internal(InputAction.CallbackContext ctx);
        private partial void OnNavigation_Internal(InputAction.CallbackContext ctx);
        private partial void OnClick_Internal(InputAction.CallbackContext ctx);
    }
}