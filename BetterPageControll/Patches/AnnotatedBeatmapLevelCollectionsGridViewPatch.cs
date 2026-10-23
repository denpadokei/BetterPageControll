using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace BetterPageControll.Patches
{
    /// <summary>
    /// Restores the 1.29.1 playlist column count.
    ///
    /// In 1.29.1 GridView.ReloadData calculated the column count from the
    /// content width: floor((viewportWidth - 3) / cellWidth) = 6 for the
    /// playlist grid. Starting with 1.40.8 the count is serialized instead
    /// (7 columns, 6 visible), so the seventh column is clipped and the grid
    /// scrolls horizontally by half a cell.
    ///
    /// SetData runs before GridView.SetDataSource / ReloadData, so setting
    /// both fields here makes ReloadData lay the cells out in six columns and
    /// makes the animator use six visible columns.
    /// </summary>
    [HarmonyPatch(typeof(AnnotatedBeatmapLevelCollectionsGridView), nameof(AnnotatedBeatmapLevelCollectionsGridView.SetData), new[] { typeof(IReadOnlyList<BeatmapLevelPack>) })]
    internal static class AnnotatedBeatmapLevelCollectionsGridViewSetDataPatch
    {
        private const int LegacyColumnCount = 6;
        private static bool _loggedLayout;

        [HarmonyPrefix]
        private static void Prefix(AnnotatedBeatmapLevelCollectionsGridView __instance)
        {
            try
            {
                var gridView = Traverse.Create(__instance).Field("_gridView").GetValue<GridView>();
                if (gridView == null)
                {
                    return;
                }

                float cellWidth = __instance.cellWidth;
                var gridRect = gridView.transform as RectTransform;
                int columnCount = GetLegacyColumnCount(gridRect, cellWidth);

                var gridTraverse = Traverse.Create(gridView);
                gridTraverse.Field("_columnCount").SetValue(columnCount);
                gridTraverse.Field("_visibleColumnCount").SetValue(columnCount);

                LogLayoutOnce(columnCount, gridRect, cellWidth);
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error("[BetterPageControll] Failed to apply legacy playlist columns: " + ex);
            }
        }

        private static int GetLegacyColumnCount(RectTransform gridRect, float cellWidth)
        {
            if (gridRect == null || cellWidth <= 0f)
            {
                return LegacyColumnCount;
            }

            // 1.29.1: floor((viewportWidth - 3) / cellWidth). The 1.40.8 grid
            // root is 93 units wide, so this yields 6 columns for 15-unit cells.
            int computed = Mathf.FloorToInt(((gridRect.rect.width - 3f) / cellWidth) + 0.001f);
            return computed >= 1 && computed <= LegacyColumnCount ? computed : LegacyColumnCount;
        }

        private static void LogLayoutOnce(int columnCount, RectTransform gridRect, float cellWidth)
        {
            if (_loggedLayout)
            {
                return;
            }

            _loggedLayout = true;
            float gridWidth = gridRect != null ? gridRect.rect.width : -1f;
            Plugin.Log?.Info($"[BetterPageControll] Legacy playlist grid: {columnCount} columns (grid width {gridWidth}, cell width {cellWidth}).");
        }
    }
}
