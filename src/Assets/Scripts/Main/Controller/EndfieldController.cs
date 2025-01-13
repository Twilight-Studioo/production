#region

using System;
using Core.Utilities;
using Feature.Interface;
using Main.Scene;
using Main.Scene.Generated;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

#endregion

namespace Main.Controller
{
    public class EndFieldController : IEndFieldController
    {
        private readonly CompositeDisposable disposable = new();
        private readonly Image endFieldImage;
        private readonly float fadeDuration = 2f;
        private readonly RootInstance rootInstance;

        private bool isEnded;

        [Inject]
        public EndFieldController()
        {
            this.rootInstance = RootInstance.Shared;
            endFieldImage = GameObject.Find("EndField").GetComponent<Image>();
            endFieldImage.color = new(0, 0, 0, 0);
        }

        public void SubscribeToPlayerHealth(IObservable<int> playerHealthObservable)
        {
            playerHealthObservable
                .Subscribe(health =>
                {
                    if (health <= 0)
                    {
                        endFieldImage.enabled = true;
                        FadeToBlackAndChangeScene();
                    }
                })
                .AddTo(disposable);
        }

        private void FadeToBlackAndChangeScene()
        {
            var fadeTimer = 0f;

            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    fadeTimer += Time.deltaTime;
                    var alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
                    endFieldImage.color = new(0, 0, 0, alpha);

                    if (alpha >= 1 && !isEnded)
                    {
                        isEnded = true;
                        SceneLoaderFeatures.GameOverScene(null).Bind(rootInstance).Load();
                    }
                })
                .AddTo(disposable);
        }

        public void Dispose()
        {
            disposable.Dispose();
        }
    }
}