#region

using System;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#endregion

public class EndFieldController
{
    private readonly Image endFieldImage;
    private readonly float fadeDuration = 2f;

    private readonly CompositeDisposable disposable = new();

    public EndFieldController()
    {
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
                    Debug.Log("Player has died. Starting fade out process.");
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

                if (alpha >= 1)
                {
                    SceneManager.LoadScene("EndScene");
                }
            })
            .AddTo(disposable);
    }

    public void Dispose()
    {
        disposable.Dispose();
    }
}