using System.Collections.Generic;
using System.Linq;
using Core.Utilities;
using Feature.Common.Constants;
using Feature.Common.Parameter;
using Feature.Interface;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using VContainer;

namespace Main.Controller.Save
{
    /// <summary>
    /// audio mixerの設定と再生を行うクラス
    /// </summary>
    public class AudioMixerController : MonoBehaviour, IAudioMixerController
    {
        [SerializeField] private AudioMixer audioMixer;
        [FormerlySerializedAs("audioSourceSE"),SerializeField] private AudioSource audioSourceBgm;
        [SerializeField] private GameObject seAudioPrefab;
        private readonly List<AudioSource> seSources = new ();
        
        // WARNING: 安易に変更しない
        private const float MinDecibels = -80.0f;
        private const float MaxDecibels = 10.0f;
        private const float DefaultVolume = 0.6f;
        // WARNING: 安易に変更しない

        [Inject]
        private GameSaveManager gameSaveManager;
        
        [Inject]
        private GameAudioAssets gameAudioAssets;

        private void Start()
        {
            gameSaveManager.OnValueChanged
                .Subscribe(OnValueChanged)
                .AddTo(this);
        }
        
        private void OnValueChanged(string key)
        {
            if (key == GameSaveKeys.MasterVolumeKey)
            {
                SetMasterVolume(gameSaveManager.GetMasterVolume());
            }
            else if (key == GameSaveKeys.BgmVolumeKey)
            {
                SetBgmVolume(gameSaveManager.GetBgmVolume());
            }
            else if (key == GameSaveKeys.SeVolumeKey)
            {
                SetSeVolume(gameSaveManager.GetSeVolume());
            }
        }

        public void SetMasterVolume(float volume)
        {
            SetVolume("Master",volume);
        }
        public void SetBgmVolume(float volume)
        {
            SetVolume("BGM", volume);
        }

        public void SetSeVolume(float volume)
        {
            SetVolume("SE", volume);
        }
        
        public void PlayLoopFile(AudioAssetType type)
        {
            var audioClip = gameAudioAssets.GetAudioClip(type);
            if (audioClip == null)
            {
                Debug.LogError("AudioClip is null");
                return;
            }
            if (audioSourceBgm.isPlaying)
            {
                audioSourceBgm.Stop();
            }
            audioSourceBgm.clip = audioClip;
            audioSourceBgm.loop = true;
            audioSourceBgm.Play();
        }
        public void PlayOneShotSE(AudioAssetType type)
        {
            
            var audioClip = gameAudioAssets.GetAudioClip(type);
            if (audioClip == null)
            {
                Debug.LogError("AudioClip is null");
                return;
            }
            var canUsingCache = seSources.FirstOrDefault(x => !x.isPlaying);
            if (canUsingCache == null)
            {
                var audioSource = Instantiate(seAudioPrefab, transform).GetComponent<AudioSource>().CheckNull();
                audioSource.clip = audioClip;
                audioSource.Play();
                seSources.Add(audioSource);
            }
            else
            {
                canUsingCache.clip = audioClip;
                canUsingCache.Play();
            }
        }
        
        private void SetVolume(string exposedParameter, float volume)
        {
            volume = Mathf.Clamp(volume, 0.0001f, 1.0f);

            float volumeInDecibels;

            // `defaultVolume`が0.0fや1.0fにならないように注意
            if (volume <= 0.0001f)
            {
                // 無音（最小デシベル値を設定）
                volumeInDecibels = MinDecibels;
            }
            else
            {
                // `S`を計算
                var s = -MaxDecibels / Mathf.Log10(DefaultVolume);

                // `volumeInDecibels`を計算
                volumeInDecibels = s * Mathf.Log10(volume) + MaxDecibels;

                // 最小デシベル値を下回らないようにクランプ
                if (volumeInDecibels < MinDecibels)
                {
                    volumeInDecibels = MinDecibels;
                }
            }

            audioMixer.SetFloat(exposedParameter, volumeInDecibels);
            // Debug.Log($"SetVolume {exposedParameter} Volume: {volume}, Decibels: {volumeInDecibels}dB");
        }
    }
}