using Core.Utilities;
using Feature.Interface;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Main.Controller
{
    public class TitleManager: IStartable
    {
        private readonly ITitleController controller;
        [Inject]
        public TitleManager(
            ITitleController controller
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