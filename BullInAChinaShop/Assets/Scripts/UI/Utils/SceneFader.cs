using System;
using CharaGaming.BullInAChinaShop.Utils;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CharaGaming.BullInAChinaShop.UI.Utils
{
    public class SceneFader : Singleton<SceneFader>
    {
        [SerializeField]
        private float _defaultDuration = 0.5f;
        
        [SerializeField]
        private Image _fadeImage;
        
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
            _fadeImage.DOFade(1f, _defaultDuration)
                .OnComplete(() =>
                {
                    SceneManager.LoadSceneAsync(sceneName);
                });
        }
        public void FadeToScene(string sceneName, Action callback)
        {
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
            _fadeImage.DOFade(1f, Duration)
                .OnComplete(() =>
                {
                    SceneManager.LoadSceneAsync(sceneName);
                });
        }
        public void FadeToScene(string sceneName, float duration, Action callback)
        {
            Duration = duration;
            _fadeImage.DOFade(1f, Duration)
                .OnComplete(() =>
                {
                    callback.Invoke();
                    SceneManager.LoadSceneAsync(sceneName);
                });
        }

        private void OnSceneChange(Scene current, Scene next)
        {
            _fadeImage.DOFade(0f, Duration);
            Duration = _defaultDuration;
        }
    }
}