using System;
using HarmonyLib;

namespace BetterPageControll.Patches
{
    /// <summary>
    /// Restores the 1.29.1 playlist grid animation.
    ///
    /// 1.29.1 only expanded the grid vertically. 1.40.8 expands it in both
    /// directions: AnimateOpen widens the viewport to
    /// (columnCount + 2) * cellWidth, which makes the invisible
    /// ContentInteractionArea wider than the playlist row. With hundreds of
    /// playlists that area also becomes very tall and ends up covering the
    /// PageControl strip, so some page dots stop receiving pointer hits.
    ///
    /// The legacy layout is obtained by:
    ///  * forcing the playlist GridView to the 1.29.1 column count (6) in
    ///    AnnotatedBeatmapLevelCollectionsGridView.SetData;
    ///  * always returning 0 from GetContentXOffset, so the content never
    ///    scrolls horizontally;
    ///  * temporarily using a column count of visibleColumnCount - 2 while
    ///    AnimateOpen computes its viewport width, which keeps the width at
    ///    visibleColumnCount * cellWidth;
    ///  * temporarily clearing _padding, because 1.29.1's height formula
    ///    had no padding (1.40.8 adds _padding * 2).
    /// </summary>
    internal static class AnnotatedBeatmapLevelCollectionsGridViewAnimatorLegacyPatch
    {
        private sealed class AnimateOpenState
        {
            public readonly int ColumnCount;
            public readonly float Padding;

            public AnimateOpenState(int columnCount, float padding)
            {
                ColumnCount = columnCount;
                Padding = padding;
            }
        }

        [HarmonyPatch(typeof(AnnotatedBeatmapLevelCollectionsGridViewAnimator), "GetContentXOffset")]
        private static class GetContentXOffsetPatch
        {
            [HarmonyPrefix]
            private static bool Prefix(ref float __result)
            {
                // 1.29.1 did not scroll horizontally at all.
                __result = 0f;
                return false;
            }
        }

        [HarmonyPatch(typeof(AnnotatedBeatmapLevelCollectionsGridViewAnimator), nameof(AnnotatedBeatmapLevelCollectionsGridViewAnimator.AnimateOpen))]
        private static class AnimateOpenPatch
        {
            [HarmonyPrefix]
            private static void Prefix(AnnotatedBeatmapLevelCollectionsGridViewAnimator __instance, out AnimateOpenState __state)
            {
                __state = null;

                try
                {
                    var traverse = Traverse.Create(__instance);
                    int visibleColumnCount = traverse.Field("_visibleColumnCount").GetValue<int>();

                    // Save the serialized values so the rest of the game keeps
                    // seeing the normal 1.40.8 configuration.
                    __state = new AnimateOpenState(
                        traverse.Field("_columnCount").GetValue<int>(),
                        traverse.Field("_padding").GetValue<float>());

                    // AnimateOpen computes width as (columnCount + 2) * columnWidth.
                    traverse.Field("_columnCount").SetValue(visibleColumnCount - 2);
                    traverse.Field("_padding").SetValue(0f);
                }
                catch (Exception ex)
                {
                    Plugin.Log?.Error("[BetterPageControll] Failed to prepare legacy AnimateOpen: " + ex);
                    __state = null;
                }
            }

            [HarmonyPostfix]
            private static void Postfix(AnnotatedBeatmapLevelCollectionsGridViewAnimator __instance, AnimateOpenState __state)
            {
                if (__state == null)
                {
                    return;
                }

                try
                {
                    var traverse = Traverse.Create(__instance);
                    traverse.Field("_columnCount").SetValue(__state.ColumnCount);
                    traverse.Field("_padding").SetValue(__state.Padding);
                }
                catch (Exception ex)
                {
                    Plugin.Log?.Error("[BetterPageControll] Failed to restore fields after AnimateOpen: " + ex);
                }
            }
        }
    }
}
