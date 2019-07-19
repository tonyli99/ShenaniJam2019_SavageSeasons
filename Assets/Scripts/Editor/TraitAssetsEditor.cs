namespace Blobber
{
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    /// <summary>
    /// Uses ReorderableList for traits.
    /// Adds a button to automatically populate traits.
    /// </summary>
    [CustomEditor(typeof(TraitAssets))]
    public class TraitAssetsEditor : Editor
    {
        private ReorderableList list;

        private void OnEnable()
        {
            list = new ReorderableList(serializedObject, serializedObject.FindProperty("traits"), true, false, false, false);
            list.drawElementCallback += OnDrawListElement;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
            if (GUILayout.Button("Update Assets"))
            {
                UpdateAssets();
            }
        }

        private void OnDrawListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var prop = serializedObject.FindProperty("traits");
            if (0 <= index && index < prop.arraySize)
            {
                EditorGUI.PropertyField(rect, prop.GetArrayElementAtIndex(index), true);
            }
        }

        private void UpdateAssets()
        {
            var traitAssets = target as TraitAssets;
            Undo.RecordObject(traitAssets, "Update Assets");
            foreach (var guid in AssetDatabase.FindAssets("t:Trait"))
            {
                var asset = AssetDatabase.LoadAssetAtPath<Trait>(AssetDatabase.GUIDToAssetPath(guid));
                if (!traitAssets.traits.Contains(asset))
                {
                    traitAssets.traits.Add(asset);
                }
            }
        }
    }
}
