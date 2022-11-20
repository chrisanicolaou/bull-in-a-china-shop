using System;
using UnityEditor;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.MainMenu
{
    [CustomEditor(typeof(WordSlammer))]
    public class WordSlammerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            WordSlammer slammer = (WordSlammer)target;
            
            DrawDefaultInspector();

            if (GUILayout.Button("Slam"))
            {
                slammer.SlamWords();
            }
        }
    }
}