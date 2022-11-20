using UnityEngine;
using UnityEngine.EventSystems;

namespace CharaGaming.BullInAChinaShop.UI.Utils
{
    public class HoverSfx : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField]
        private AudioClip _clip;

        [SerializeField]
        private AudioSource _source;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_source.isPlaying) _source.Stop();
            
            _source.PlayOneShot(_clip);
        }
    }
}