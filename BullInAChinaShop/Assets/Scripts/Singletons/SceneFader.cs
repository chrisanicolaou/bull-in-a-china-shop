using System;
using CharaGaming.BullInAChinaShop.Utils;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CharaGaming.BullInAChinaShop.Singletons
{
    public class SceneFader : Singleton<SceneFader>
    {
        [SerializeField]
        private float _defaultDuration = 2f;
        
        [SerializeField]
        private Image _fadeImage;

        [SerializeField]
        private AudioMixer _masterMixer;
        
        public float Duration { get; set; }

        private void Start()
        {
            _fadeImage.color = Color.black;
            Duration = _defaultDuration;
            _fadeImage.DOFade(0f, _defaultDuration);
            SceneManager.activeSceneChanged += OnSceneChange;
        }

        public void FadeToScene(string sceneName)
        {
            _masterMixer.DOSetFloat("masterVol", -50f, _defaultDuration);
            _fadeImage.DOFade(1f, _defaultDuration)
                .OnComplete(() =>
                {
                    SceneManager.LoadSceneAsync(sceneName);
                });
        }
        public void FadeToScene(string sceneName, Action callback)
        {
            _masterMixer.DOSetFloat("masterVol", -50f, _defaultDuration);
            _fadeImage.DOFade(1f, _defaultDuration)
                .OnComplete(() =>
                {
                    callback.Invoke();
                    SceneManager.LoadSceneAsync(sceneName);
                });
        }
        public void FadeToScene(string sceneName, float duration)
        {
            Duration = duration;
            _masterMixer.DOSetFloat("masterVol", -50f, Duration);
            _fadeImage.DOFade(1f, Duration)
                .OnComplete(() =>
                {
                    SceneManager.LoadSceneAsync(sceneName);
                });
        }
        public void FadeToScene(string sceneName, float duration, Action callback)
        {
            Duration = duration;
            _masterMixer.DOSetFloat("masterVol", -50f, Duration);
            _fadeImage.DOFade(1f, Duration)
                .OnComplete(() =>
                {
                    callback.Invoke();
                    SceneManager.LoadSceneAsync(sceneName);
                });
        }

        private void OnSceneChange(Scene current, Scene next)
        {
            _masterMixer.DOSetFloat("masterVol", CalculateMixerVol(), _defaultDuration);
            _fadeImage.DOFade(0f, Duration);
            Duration = _defaultDuration;
        }

        private float CalculateMixerVol()
        {
            return PlayerPrefs.Instance.MasterVol / 2 - 50;
        }
    }
}