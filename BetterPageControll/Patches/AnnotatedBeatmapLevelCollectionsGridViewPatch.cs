//using HarmonyLib;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BetterPageControll.Patches
//{
//    [HarmonyPatch(typeof(AnnotatedBeatmapLevelCollectionsGridView), nameof(AnnotatedBeatmapLevelCollectionsGridView.SetData), new Type[] {typeof(IReadOnlyList<BeatmapLevelPack>) })]
//    internal class AnnotatedBeatmapLevelCollectionsGridViewPatch
//    {
//        [HarmonyPrefix]
//        public static void Prefix(AnnotatedBeatmapLevelCollectionsGridView __instance)
//        {
//            //__instance._gridView._columnCount = __instance._gridView.visibleColumnCount;
//            __instance._gridView._columnCount = 5;
//        }
//    }
//}
