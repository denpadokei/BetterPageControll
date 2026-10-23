//using HarmonyLib;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace BetterPageControll.Patches
//{
//    [HarmonyPatch(typeof(GridView), nameof(GridView.ReloadData))]
//    internal class GridViewPatch
//    {
//        public static void OldReload(GridView gridView)
//        {
//            float cellWidth = gridView._dataSource.cellWidth;
//            float cellHeight = gridView._dataSource.cellHeight;
//            int numberOfCells = gridView._dataSource.numberOfCells;
//            //gridView._columnCount = Mathf.FloorToInt(gridView._contentTransform.rect.width / cellWidth);
//            gridView._columnCount = 5;
//            gridView._rowCount = Mathf.CeilToInt((float)numberOfCells / (float)gridView._columnCount);
//            foreach (MonoBehaviour key in gridView._spawnedCellsPerPrefabDictionary.Keys)
//            {
//                List<MonoBehaviour> list = gridView._spawnedCellsPerPrefabDictionary[key];
//                foreach (MonoBehaviour monoBehaviour in list)
//                {
//                    monoBehaviour.gameObject.SetActive(false);
//                    gridView._availableCellsPerPrefabDictionary[key].Enqueue(monoBehaviour);
//                }
//                list.Clear();
//            }
//            for (int i = 0; i < numberOfCells; i++)
//            {
//                int num = i % gridView._columnCount;
//                int num2 = i / gridView._columnCount;
//                RectTransform rectTransform = (RectTransform)gridView._dataSource.CellForIdx(gridView, i).transform;
//                rectTransform.anchorMin = new Vector2(0f, 1f);
//                rectTransform.anchorMax = new Vector2(0f, 1f);
//                rectTransform.pivot = new Vector2(0f, 1f);
//                rectTransform.anchoredPosition = new Vector2((float)num * cellWidth, (float)num2 * -cellHeight);
//            }
//        }

//        public static void NewReload(GridView gridView)
//        {
//            float cellWidth = gridView._dataSource.cellWidth;
//            float cellHeight = gridView._dataSource.cellHeight;
//            int numberOfCells = gridView._dataSource.numberOfCells;
//            gridView._rowCount = Mathf.CeilToInt((float)numberOfCells / (float)gridView._columnCount);
//            foreach (MonoBehaviour key in gridView._spawnedCellsPerPrefabDictionary.Keys)
//            {
//                List<MonoBehaviour> list = gridView._spawnedCellsPerPrefabDictionary[key];
//                foreach (MonoBehaviour monoBehaviour in list)
//                {
//                    monoBehaviour.gameObject.SetActive(false);
//                    gridView._availableCellsPerPrefabDictionary[key].Enqueue(monoBehaviour);
//                }
//                list.Clear();
//            }
//            for (int i = 0; i < numberOfCells; i++)
//            {
//                int num = i % gridView._columnCount;
//                int num2 = i / gridView._columnCount;
//                RectTransform rectTransform = (RectTransform)gridView._dataSource.CellForIdx(gridView, i).transform;
//                rectTransform.anchorMin = new Vector2(0f, 1f);
//                rectTransform.anchorMax = new Vector2(0f, 1f);
//                rectTransform.pivot = new Vector2(0f, 1f);
//                rectTransform.anchoredPosition = new Vector2((float)num * cellWidth, (float)num2 * -cellHeight);
//            }
//        }

//        [HarmonyPrefix]
//        public static bool Prefix(GridView __instance)
//        {
//            OldReload(__instance);
//            return false;
//        }
//    }
//}
