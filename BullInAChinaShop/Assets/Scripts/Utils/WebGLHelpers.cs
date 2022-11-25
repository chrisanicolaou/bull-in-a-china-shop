using System.Runtime.InteropServices;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.Utils
{
    public static class WebGLHelpers {

        [DllImport("__Internal")]
        public static extern void Quit();
        
    }
}