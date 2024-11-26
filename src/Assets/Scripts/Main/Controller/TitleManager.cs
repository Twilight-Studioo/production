using Core.Utilities;
using Feature.Interface;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Main.Controller
{
    public class TitleManager: IStartable
    {
        private readonly IOutGameController controller;
        [Inject]
        public TitleManager(
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