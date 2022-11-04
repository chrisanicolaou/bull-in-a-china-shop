using System;
using System.Collections;
using CharaGaming.BullInAChinaShop.Singletons;
using CharaGaming.BullInAChinaShop.UI;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.TestScripts
{
    public class TestDialogue : MonoBehaviour
    {

        [SerializeField]
        private AudioClip[] _audioClips;

        private System.Random _random;

        private void Start()
        {
            StartCoroutine(nameof(ShowDialogue));
            _random = new System.Random();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                StartCoroutine(nameof(ShowDialogue));
            }
        }

        private IEnumerator ShowDialogue()
        {
            yield return new WaitForSeconds(1f);
            
            DialogueBox.Instance.SetHeader("Mr. Bull")
                .SetBody("I am mad at you  ! Give me some money for me now   !!!")
                .StayOnScreen()
                .AddAudio(_audioClips[_random.Next(0, _audioClips.Length)])
                .OnComplete(ShowSecondDialogue)
                .Show();
        }

        private void ShowSecondDialogue()
        {
            DialogueBox.Instance.SetHeader("Mr. Bull")
                .SetBody("Now I am saying it again, and the dialogue box has stayed on the screen!!")
                .OnComplete(() => Debug.Log("Completed both!"))
                .Show();
        }
    }
}
