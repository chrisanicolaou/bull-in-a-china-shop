using System;
using System.Collections;
using CharaGaming.BullInAChinaShop.UI;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.TestScripts
{
    public class TestDialogue : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(nameof(ShowDialogue));
        }

        private IEnumerator ShowDialogue()
        {
            yield return new WaitForSeconds(1f);
            
            DialogueBox.Instance.SetHeader("Mr. Bull")
                .SetBody("I am mad at you  ! Give me some money for me now   !!!")
                .OnComplete(() => Debug.Log("I have finished!"))
                .Show();
        }
    }
}
