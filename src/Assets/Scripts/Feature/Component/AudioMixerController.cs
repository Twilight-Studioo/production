using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Feature.Component
{
    public class AudioMixerController : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider seSlider;
        [SerializeField] private Slider masterSlider;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip audioClip;

        private void Awake()
        {
            InitializeSlider(bgmSlider, "BGM");
            InitializeSlider(seSlider, "SE");
            InitializeSlider(masterSlider,"Master");
        }

        private void InitializeSlider(Slider slider, string parameterName)
        {
            if (audioMixer.GetFloat(parameterName, out float volume))
            {
                slider.value = Mathf.Pow(10, volume / 20);
            }
            else
            {
                Debug.LogWarning($"AudioMixer parameter '{parameterName}' not found.");
            }
        }

        public void SetMaster(float volume)
        {
            SetVolume("Master",volume);
        }
        public void SetBGM(float volume)
        {
            SetVolume("BGM", volume);
        }

        public void SetSE(float volume)
        {
            SetVolume("SE", volume);
            if (!audioSource.isPlaying && audioClip != null)
            {
                audioSource.PlayOneShot(audioClip);
            }
        }
        private void SetVolume(string parameterName, float volume)
        {
            volume = Mathf.Clamp(volume, 0.0001f, 1f);
            float decibel = 20f * Mathf.Log10(volume);
            decibel = Mathf.Clamp(decibel, -80f, 0f);
            audioMixer.SetFloat(parameterName, decibel);
        }
    }
}