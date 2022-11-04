using System;
using CharaGaming.BullInAChinaShop.Utils;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace CharaGaming.BullInAChinaShop.TestScripts
{
    public class TestUpgradeController : MonoBehaviour
    {
        [SerializeField]
        private GameObject _cashRegister;

        [SerializeField]
        [ColorUsage(true, true)]
        private Color _minGlowColor;
        
        [SerializeField]
        [ColorUsage(true, true)]
        private Color _maxGlowColor;

        [SerializeField]
        [Range(0.1f, 1f)]
        private float _pulseDuration;

        [SerializeField]
        private KeyCode _enableKey;

        [SerializeField]
        private GameObject _upgradeMenu;

        private Material _cashRegisterMat;

        private bool _shouldEnable = true;
        
        private TweenChain _pulsingTweens = new TweenChain();

        private void Start()
        {
            _cashRegisterMat = _cashRegister.GetComponent<Image>().material;
            _cashRegisterMat.color = Color.black;
        }

        private void Update()
        {
            if (Input.GetKeyDown(_enableKey))
            {
                ToggleRegister();
            }
        }

        private void ToggleRegister()
        {
            if (_shouldEnable)
            {
                EnableRegister();
            }
            else
            {
                DisableRegister();
            }

            _shouldEnable = !_shouldEnable;
        }

        private void DisableRegister()
        {
            if (_pulsingTweens != null && _pulsingTweens.IsRunning()) _pulsingTweens.KillAll();
            
            var btn = _cashRegister.GetComponent<Button>();
            if (btn != null) Destroy(btn);
            _cashRegisterMat.color = Color.black;
        }

        private void EnableRegister()
        {
            var btn = _cashRegister.AddComponent<Button>();
            btn.onClick.AddListener(() => _upgradeMenu.SetActive(true));
            btn.navigation = new Navigation();
            btn.transition = Selectable.Transition.None;
            _pulsingTweens.AddToQueue(_cashRegisterMat.DOColor(_minGlowColor, _pulseDuration)
                .OnComplete(() => _pulsingTweens = _cashRegisterMat.PulseColor(_minGlowColor, _maxGlowColor, _pulseDuration)));
        }

        private void OnDestroy()
        {
            _cashRegisterMat.color = Color.black;
        }
    }
}
