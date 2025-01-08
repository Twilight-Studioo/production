using Core.Input;
using Core.Navigation;
using Core.Utilities;
using Feature.Component;
using Feature.Interface;
using Main.Controller.GameNavigation;
using UniRx;
using UnityEngine;
using VContainer;

namespace Main.Controller
{
    public class TitleController: IOutGameController
    {
        private readonly InputActionAccessor accessor;
        private readonly ScreenController<Navigation> controller;
        private readonly CinemachineSmoothPathManager cinemachineSmoothPathManager;
        [Inject]
        public TitleController(
            InputActionAccessor accessor,
            ScreenController<Navigation> controller,
            CinemachineSmoothPathManager cinemachineSmoothPathManager
        )
        {
            this.accessor = accessor.CheckNull();
            this.controller = controller.CheckNull();
            this.cinemachineSmoothPathManager = cinemachineSmoothPathManager.CheckNull();
        }
        public void Start()
        {
            Observable.EveryUpdate()
                .Select(_ => controller.CurrentDestination)
                .DistinctUntilChanged()
                .Subscribe(OnNavigationChanged)
                .AddTo(ObjectFactory.SuperObject);
            controller.Reset();
            cinemachineSmoothPathManager.SetPathPointFast(CinemachineSmoothPathManager.PathPoint.Title);
            controller.Navigate(Navigation.Title);
        }
        
        private void OnNavigationChanged(Destination<Navigation> navigation)
        {
            if (navigation == null)
            {
                return;
            }
            switch (navigation.Route)
            {
                case Navigation.Controls:
                case Navigation.Volumes:
                case Navigation.Option:
                    cinemachineSmoothPathManager.SetPathPoint(CinemachineSmoothPathManager.PathPoint.Option);
                    break;
                case Navigation.Title:
                    cinemachineSmoothPathManager.SetPathPoint(CinemachineSmoothPathManager.PathPoint.Title);
                    break;
                case Navigation.GameOver:
                case Navigation.Pause:
                    break;
                case Navigation.StageSelect:
                    cinemachineSmoothPathManager.SetPathPoint(CinemachineSmoothPathManager.PathPoint.StageSelect);
                    break;
            }
        }
    }
}