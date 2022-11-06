using System;
using UnityEngine;
using UnityEngine.UI;

namespace CharaGaming.BullInAChinaShop.UI
{
    public class UpgradeMenu : MonoBehaviour
    {
        [SerializeField]
        private Button _exitButton;
        
        [SerializeField]
        private Button _stockMenuButton;
        
        [SerializeField]
        private Button _upgradeMenuButton;

        [SerializeField]
        private GameObject _stockMenuObj;
        
        [SerializeField]
        private GameObject _upgradeMenuObj;

        private void Start()
        {
            _exitButton.onClick.AddListener(() => gameObject.SetActive(false));
            _stockMenuButton.onClick.AddListener(() =>
            {
                _upgradeMenuObj.SetActive(false);
                _stockMenuObj.SetActive(true);
            });
            _upgradeMenuButton.onClick.AddListener(() =>
            {
                _stockMenuObj.SetActive(false);
                _upgradeMenuObj.SetActive(true);
            });
        }
    }
}
