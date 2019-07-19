namespace Blobber
{
    using System;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    /// <summary>
    /// Uses ReorderableList for tiles and tileContents.
    /// Adds a button to automatically populate tiles and tileContents.
    /// </summary>
    [CustomEditor(typeof(MapAssets))]
    public class MapAssetsEditor : Editor
    {
        private ReorderableList tileList;
        private ReorderableList tileContentList;

        private void OnEnable()
        {
            tileList = new ReorderableList(serializedObject, serializedObject.FindProperty("tiles"), true, false, false, false);
            tileList.drawElementCallback += OnDrawTileListElement;
            tileContentList = new ReorderableList(serializedObject, serializedObject.FindProperty("tileContents"), true, false, false, false);
            tileContentList.drawElementCallback += OnDrawTileContentListElement;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            tileList.DoLayoutList();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultTile"), true);
            tileContentList.DoLayoutList();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("startTileContent"), true);
            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Update Assets"))
            {
                UpdateAssets();
            }
        }

        private void OnDrawTileListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var prop = serializedObject.FindProperty("tiles");
            if (0 <= index && index < prop.arraySize)
            {
                EditorGUI.PropertyField(rect, prop.GetArrayElementAtIndex(index), true);
            }
        }

        private void OnDrawTileContentListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var prop = serializedObject.FindProperty("tileContents");
            if (0 <= index && index < prop.arraySize)
            {
                EditorGUI.PropertyField(rect, prop.GetArrayElementAtIndex(index), true);
            }
        }

        private void UpdateAssets()
        {
            var mapAssets = target as MapAssets;
            Undo.RecordObject(mapAssets, "Update Assets");
            foreach (var guid in AssetDatabase.FindAssets("t:Tile"))
            {
                var asset = AssetDatabase.LoadAssetAtPath<Tile>(AssetDatabase.GUIDToAssetPath(guid));
                if (!mapAssets.tiles.Contains(asset))
                {
                    mapAssets.tiles.Add(asset);
                }
            }
            foreach (var guid in AssetDatabase.FindAssets("t:TileContent"))
            {
                var asset = AssetDatabase.LoadAssetAtPath<TileContent>(AssetDatabase.GUIDToAssetPath(guid));
                if (!mapAssets.tileContents.Contains(asset))
                {
                    mapAssets.tileContents.Add(asset);
                }
            }
        }
    }
}
