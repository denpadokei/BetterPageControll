using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BetterPageControll.Patches
{
    /// <summary>
    /// Minimal patch: Only intercept Init and ScrollToRowIdxInstant to adapt 2D API to 1D behavior.
    /// AnimateClose/AnimateOpen are NOT patched — let the original implementation handle them.
    /// The original already has correct DespawnAllActiveTweens; we just need to set the right fields.
    /// </summary>
    [HarmonyPatch(typeof(AnnotatedBeatmapLevelCollectionsGridViewAnimator))]
    internal class AnnotatedBeatmapLevelCollectionsGridViewAnimatorPatch
    {
        [HarmonyTargetMethods]
        public static IEnumerable<MethodBase> TargetMethods()
        {
            var t = typeof(AnnotatedBeatmapLevelCollectionsGridViewAnimator);

            // Target the new 5-param Init only
            var initMethod = t.GetMethod(
                nameof(AnnotatedBeatmapLevelCollectionsGridViewAnimator.Init),
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { typeof(float), typeof(float), typeof(int), typeof(int), typeof(int) },
                null
            );

            if (initMethod != null)
                yield return initMethod;

            // Target the new 2-param ScrollToRowIdxInstant
            var scrollMethod = t.GetMethod(
                nameof(AnnotatedBeatmapLevelCollectionsGridViewAnimator.ScrollToRowIdxInstant),
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { typeof(int), typeof(int) },
                null
            );

            if (scrollMethod != null)
                yield return scrollMethod;
        }

        [HarmonyPrefix]
        public static bool Prefix(MethodBase __originalMethod, AnnotatedBeatmapLevelCollectionsGridViewAnimator __instance, object[] __args)
        {
            if (__originalMethod == null || __args == null)
                return true;

            string methodName = __originalMethod.Name;

            try
            {
                switch (methodName)
                {
                    case nameof(AnnotatedBeatmapLevelCollectionsGridViewAnimator.Init):
                        {
                            // Init(float columnWidth, float rowHeight, int columnCount, int rowCount, int visibleColumnCount)
                            float columnWidth = (float)__args[0];
                            float rowHeight = (float)__args[1];
                            int columnCount = (int)__args[2];
                            int rowCount = (int)__args[3];
                            int visibleColumnCount = (int)__args[4];

                            var traverse = Traverse.Create(__instance);

                            // Store the parameters (needed for AnimateClose offsets)
                            traverse.Field("_columnWidth").SetValue(columnWidth);
                            traverse.Field("_rowHeight").SetValue(rowHeight);
                            traverse.Field("_columnCount").SetValue(columnCount);
                            traverse.Field("_rowCount").SetValue(rowCount);
                            traverse.Field("_visibleColumnCount").SetValue(visibleColumnCount);
                            traverse.Field("_selectedColumn").SetValue(0);
                            traverse.Field("_selectedRow").SetValue(0);

                            var contentTransform = traverse.Field("_contentTransform").GetValue<RectTransform>();
                            var viewportTransform = traverse.Field("_viewportTransform").GetValue<RectTransform>();

                            // Old behavior: Y dimension only (X dimension preserved as-is)
                            Vector2 contentSize = contentTransform.sizeDelta;
                            contentSize.y = rowHeight * (float)rowCount;
                            contentTransform.sizeDelta = contentSize;

                            Vector2 viewportSize = viewportTransform.sizeDelta;
                            viewportSize.y = rowHeight;
                            viewportTransform.sizeDelta = viewportSize;

                            // Reset Y position
                            var anchoredPos = contentTransform.anchoredPosition;
                            anchoredPos.y = 0f;
                            contentTransform.anchoredPosition = anchoredPos;

                            return false; // Skip original
                        }

                    case nameof(AnnotatedBeatmapLevelCollectionsGridViewAnimator.ScrollToRowIdxInstant):
                        {
                            // ScrollToRowIdxInstant(int selectedColumn, int selectedRow)
                            int selectedColumn = (int)__args[0];
                            int selectedRow = (int)__args[1];

                            var traverse = Traverse.Create(__instance);
                            traverse.Field("_selectedColumn").SetValue(selectedColumn);
                            traverse.Field("_selectedRow").SetValue(selectedRow);

                            var contentTransform = traverse.Field("_contentTransform").GetValue<RectTransform>();
                            var anchoredPos = contentTransform.anchoredPosition;

                            // Scroll Y only (old behavior: 1D vertical scrolling)
                            float rowHeight = traverse.Field("_rowHeight").GetValue<float>();
                            anchoredPos.y = -selectedRow * rowHeight;
                            contentTransform.anchoredPosition = anchoredPos;

                            return false; // Skip original
                        }
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"[AnnotatedBeatmapLevelCollectionsGridViewAnimator Patch] Error in {methodName}: {ex}");
            }

            return true;
        }
    }
}
