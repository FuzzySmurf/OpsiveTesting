using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ARAWorks.Pooling;

namespace ARAEditor.Pooling
{
    [CustomEditor(typeof(PoolingManager)), CanEditMultipleObjects]
    public class PoolingManagerEditor : Editor
    {
        private PoolingManager poolingManager;

        private SerializedProperty currentPoolGroup;
        private SerializedProperty categories;

        private static readonly string[] poolManagerTabs = { "Pooling", "Categories" };
        private readonly List<string> _removeFromDict = new List<string>();

        public virtual void OnEnable()
        {
            if (poolingManager == null)
                poolingManager = (PoolingManager)target;

            currentPoolGroup = serializedObject.FindProperty("currentPoolGroup");
            categories = serializedObject.FindProperty("categories");
        }

        public override void OnInspectorGUI()
        {
            if (poolingManager == null)
                poolingManager = (PoolingManager)target;

            serializedObject.Update();

            GUIStyle layoutStyle = new GUIStyle("box")
            {
                fontStyle = FontStyle.Bold,
                fontSize = 18
            };

            GUILayout.BeginVertical("Pooling Manager", layoutStyle);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.BeginVertical(layoutStyle);
            // Tabs Grid
            GUIStyle tabStyle = new GUIStyle(EditorStyles.miniButton);
            tabStyle.fixedHeight = 24;
            tabStyle.fontSize = 12;
            poolingManager.curEditorTab = GUILayout.SelectionGrid(poolingManager.curEditorTab, poolManagerTabs, 3, tabStyle);

            // Update grouping list based on categories
            if (categories.arraySize > poolingManager.pooledGroups.dictionary.Count)
                UpdateGroupingList();

            switch (poolingManager.curEditorTab)
            {
                case 0:
                    ShowPoolingGroups();
                    break;
                case 1:
                    ShowGroupingCategories();
                    break;
            }

            GUILayout.EndVertical();
            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        private void ShowPoolingGroups()
        {
            if (poolingManager.curCategoryTab > categories.arraySize - 1)
                poolingManager.curCategoryTab = categories.arraySize - 1;

            EditorGUILayout.Space();

            GUIStyle tabStyle = new GUIStyle(EditorStyles.miniButtonMid)
            {
                fixedHeight = 24,
                fontSize = 13
            };

            if (categories.arraySize > 0)
            {
                poolingManager.curCategoryTab = GUILayout.SelectionGrid(poolingManager.curCategoryTab, poolingManager.Categories, 4, tabStyle);
                if (poolingManager.curCategoryTab < 0)
                    poolingManager.curCategoryTab = 0;
                try
                {
                    poolingManager.currentPoolGroup = poolingManager.pooledGroups.dictionary[categories.GetArrayElementAtIndex(poolingManager.curCategoryTab).stringValue].pooledObjs;
                }
                catch (KeyNotFoundException) { }

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(currentPoolGroup, new GUIContent("Pooled Objects", "Objects to be pooled"), true);
                if (EditorGUI.EndChangeCheck())
                {
                    try
                    {
                        if (poolingManager.editorPooledGroups.dictionary.ContainsKey(categories.GetArrayElementAtIndex(poolingManager.curCategoryTab).stringValue))
                            poolingManager.editorPooledGroups.dictionary[categories.GetArrayElementAtIndex(poolingManager.curCategoryTab).stringValue] = new PoolingManager.PoolGroup() { pooledObjs = new List<PoolingManager.ObjectToPool>(poolingManager.pooledGroups.dictionary[categories.GetArrayElementAtIndex(poolingManager.curCategoryTab).stringValue].pooledObjs) };
                        else
                            poolingManager.editorPooledGroups.dictionary.Add(categories.GetArrayElementAtIndex(poolingManager.curCategoryTab).stringValue, new PoolingManager.PoolGroup() { pooledObjs = new List<PoolingManager.ObjectToPool>(poolingManager.pooledGroups.dictionary[categories.GetArrayElementAtIndex(poolingManager.curCategoryTab).stringValue].pooledObjs) });
                    }
                    catch (KeyNotFoundException) { UpdateGroupingList(); }
                }
            }
        }

        private void ShowGroupingCategories()
        {
            EditorGUILayout.HelpBox("Categories must be unique; duplicates display the same list.\nWhen you delete a category you may retrieve the object list deleted by making a new category of the same name.", MessageType.Warning);
            EditorGUILayout.HelpBox("Categories are for organizational purposes only and affect nothing outside of the editor.", MessageType.Info);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(categories, new GUIContent("Pooling Categories", "The tabs in Pooling which seperating pooling into groups"));
            if (EditorGUI.EndChangeCheck())
                UpdateGroupingList();
        }

        private void UpdateGroupingList()
        {
            for (int i = 0; i < categories.arraySize; i++)
            {
                string category = categories.GetArrayElementAtIndex(i).stringValue;
                if (!poolingManager.pooledGroups.dictionary.ContainsKey(category))
                {
                    if (poolingManager.editorPooledGroups.dictionary.ContainsKey(category))
                        poolingManager.pooledGroups.dictionary.Add(category, new PoolingManager.PoolGroup() { pooledObjs = new List<PoolingManager.ObjectToPool>(poolingManager.editorPooledGroups.dictionary[category].pooledObjs) });
                    else
                    {
                        poolingManager.pooledGroups.dictionary.Add(category, new PoolingManager.PoolGroup() { pooledObjs = new List<PoolingManager.ObjectToPool>() });
                        poolingManager.editorPooledGroups.dictionary.Add(category, new PoolingManager.PoolGroup() { pooledObjs = new List<PoolingManager.ObjectToPool>() });
                    }
                }
            }

            // If category removed, then remove it from dictionary
            _removeFromDict.Clear();
            foreach (KeyValuePair<string, PoolingManager.PoolGroup> group in poolingManager.pooledGroups.dictionary)
            {
                if (!Array.Exists(poolingManager.Categories, x => x == group.Key))
                    _removeFromDict.Add(group.Key);
            }

            foreach (string key in _removeFromDict)
                poolingManager.pooledGroups.dictionary.Remove(key);
        }
    }
}