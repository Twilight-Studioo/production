using Script.Feature.Model;
using Script.Feature.Presenter;
using Script.Feature.View;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Script.Main.Controller
{
    public class SwapController : MonoBehaviour
    {
        private SwapView view;
        private SwapModel model;
        private SwapPresenter presenter;
        public GameObject player;
        public InputPlayer inputPlayer;
        void Start()
        {
            SwapModel model = new SwapModel();
            presenter = new SwapPresenter(model, view, player);
            inputPlayer = new InputPlayer();
        }

        void Update()
        {
            if (Input.GetKeyDown("joystick button 5"))
            {
                presenter.EnterSwapMode();
            }

            if (Input.GetKeyUp("joystick button 5"))
            {
                presenter.ExecuteSwap();
            }

            presenter.Update();
        }
    }

}