using Core.Utilities;
using Feature.Interface;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Main.Controller.SmasherClear
{
    public class SmasherClearManager: IStartable
    {
        private readonly IOutGameController controller;
        [Inject]
        public SmasherClearManager(
            IOutGameController controller
        )
        {
            this.controller = controller.CheckNull();
        }
        public void Start()
        {
            controller.Start();
        }
    }
}