using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ARAEditor.Spawner
{
    [InitializeOnLoad]
    public class SpawnerPrefMenuHandler : MonoBehaviour
    {
        private const string bgRKey = "Varadia.SpawnerSettings.bgR";
        private const string bgGKey = "Varadia.SpawnerSettings.bgG";
        private const string bgBKey = "Varadia.SpawnerSettings.bgB";
        private const string bgAKey = "Varadia.SpawnerSettings.bgA";
        private const string fgRKey = "Varadia.SpawnerSettings.fgR";
        private const string fgGKey = "Varadia.SpawnerSettings.fgG";
        private const string fgBKey = "Varadia.SpawnerSettings.fgB";
        private const string fgAKey = "Varadia.SpawnerSettings.fgA";
        private const string showDisabledFieldsKey = "Varadia.SpawnerSettings.showDisabledFields";

        public class SpawnerSettings
        {
            public Color bgColor = Color.black;
            public Color fgColor = Color.white;
            public bool showDisabledFields = true;
        }

        public static SpawnerSettings GetEditorSettings()
        {
            float bgR = EditorPrefs.GetFloat(bgRKey);
            float bgG = EditorPrefs.GetFloat(bgGKey);
            float bgB = EditorPrefs.GetFloat(bgBKey);
            float bgA = EditorPrefs.GetFloat(bgAKey);
            float fgR = EditorPrefs.GetFloat(fgRKey);
            float fgG = EditorPrefs.GetFloat(fgGKey);
            float fgB = EditorPrefs.GetFloat(fgBKey);
            float fgA = EditorPrefs.GetFloat(fgAKey);

            Color bgColor = new Color(bgR, bgG, bgB, bgA);
            Color fgColor = new Color(fgR, fgG, fgB, fgA);

            return new SpawnerSettings
            {
                bgColor = bgColor,
                fgColor = fgColor,
                showDisabledFields = EditorPrefs.GetBool(showDisabledFieldsKey)
            };
        }

        public static void SetEditorSettings(SpawnerSettings settings)
        {
            EditorPrefs.SetFloat(bgRKey, settings.bgColor.r);
            EditorPrefs.SetFloat(bgGKey, settings.bgColor.g);
            EditorPrefs.SetFloat(bgBKey, settings.bgColor.b);
            EditorPrefs.SetFloat(bgAKey, settings.bgColor.a);
            EditorPrefs.SetFloat(fgRKey, settings.fgColor.r);
            EditorPrefs.SetFloat(fgGKey, settings.fgColor.g);
            EditorPrefs.SetFloat(fgBKey, settings.fgColor.b);
            EditorPrefs.SetFloat(fgAKey, settings.fgColor.a);
            EditorPrefs.SetBool(showDisabledFieldsKey, settings.showDisabledFields);
        }
    }

    internal class SpawnerSettingsGUIContent
    {
        private static GUIContent bgColorContent = new GUIContent("Background Color", "The background color of the Spawner in the inspector");
        private static GUIContent fgColorContent = new GUIContent("Foreground Color", "The foreground color of the Spawner in the inspector");
        private static GUIContent showDisabledFieldsContent = new GUIContent("Show Disabled Fields", "Toggle whether to show the disabled fields in the Spawner inspector");

        public static void DrawSettingsButtons(SpawnerPrefMenuHandler.SpawnerSettings settings)
        {
            EditorGUI.indentLevel += 1;

            settings.bgColor = EditorGUILayout.ColorField(bgColorContent, settings.bgColor);
            settings.fgColor = EditorGUILayout.ColorField(fgColorContent, settings.fgColor);
            settings.showDisabledFields = EditorGUILayout.Toggle(showDisabledFieldsContent, settings.showDisabledFields);

            EditorGUI.indentLevel -= 1;
        }
    }

#if UNITY_2018_3_OR_NEWER
    static class SpawnerSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var provider = new SettingsProvider("Preferences/Varadia/Spawner Settings", SettingsScope.User)
            {
                label = "Spawner Settings",

                guiHandler = (searchContext) =>
                {
                    SpawnerPrefMenuHandler.SpawnerSettings settings = SpawnerPrefMenuHandler.GetEditorSettings();

                    EditorGUI.BeginChangeCheck();
                    SpawnerSettingsGUIContent.DrawSettingsButtons(settings);

                    if (EditorGUI.EndChangeCheck())
                    {
                        SpawnerPrefMenuHandler.SetEditorSettings(settings);
                    }
                },

                // Keywords for the search bar in the Unity Preferences menu
                keywords = new HashSet<string>(new[] { "Spawner", "Varadia"})
            };

            return provider;
        }
    }
#endif

}