using System;
using System.Collections;
using System.Collections.Generic;
using CharaGaming.BullInAChinaShop.Singletons;
using CharaGaming.BullInAChinaShop.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace CharaGaming.BullInAChinaShop
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField]
        private Button _playBtn;
        [SerializeField]
        private Button _exitBtn;
        [SerializeField]
        private Button _settingsBtn;

        [SerializeField]
        private AudioSource _musicSource;
        [SerializeField]
        private AudioSource _sfxSource;

        private void Start()
        {
            _playBtn.onClick.AddListener(OnPlayButtonPress);
            _exitBtn.onClick.AddListener(OnQuitButtonPress);
            _settingsBtn.onClick.AddListener(OnSettingsButtonPress);
        }

        private void OnPlayButtonPress()
        {
            SceneFader.Instance.FadeToScene("Day");
        }

        private void OnSettingsButtonPress()
        {
            throw new NotImplementedException();
        }

        private void OnQuitButtonPress()
        {
            WebGLHelpers.Quit();
        }
    }
}
