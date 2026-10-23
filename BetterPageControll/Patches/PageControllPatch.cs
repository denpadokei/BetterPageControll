//using HarmonyLib;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BetterPageControll.Patches
//{
//    [HarmonyPatch(typeof(PageControl), nameof(PageControl.SetPagesCount), new Type[] { typeof(int) })]
//    internal class PageControllPatch
//    {
//        private const float PageWidth = 30f;

//        [HarmonyPrefix]
//        public static void Prefix(PageControl __instance)
//        {
//            Plugin.Log.Info($"befor sizeDelta x : {__instance._content.sizeDelta.x}");
//            if (__instance._content.sizeDelta.x != PageWidth)
//            {
//                __instance._content.sizeDelta = new UnityEngine.Vector2(PageWidth, __instance._content.sizeDelta.y);
//            }
//            Plugin.Log.Info($"after sizeDelta x : {__instance._content.sizeDelta.x}");
//        }
//    }
//}
