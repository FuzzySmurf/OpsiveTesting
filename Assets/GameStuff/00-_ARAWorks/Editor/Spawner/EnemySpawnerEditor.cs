using ARAWorks.Base.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using ARAWorks.Spawner;

namespace ARAEditor.Spawner
{
    [CustomEditor(typeof(EnemySpawner)), CanEditMultipleObjects]
    public class EnemySpawnerEditor : SpawnerMaskEditor
    {
        private EnemySpawner enemySpawner;
        private SerializedProperty list;
        private SerializedProperty spawnTimer;
        private SerializedProperty playerCheckRadius;
        private SerializedProperty showSearchRadius;
        private SerializedProperty playerLayer;
        private SerializedProperty showRadiusOnlyWhenSelected;
        private SerializedProperty disableOnPlayerDistance;
        private SerializedProperty playerDistance;
        private SerializedProperty detectFromSeperateObject;
        private SerializedProperty objectToDetectFrom;

        private SerializedProperty currentWave;
        private SerializedProperty spawnNextWave;
        private SerializedProperty sameTimerEveryWave;
        private SerializedProperty numberOfWaves;
        private SerializedProperty sameMobsEveryWave;
        private SerializedProperty spawnContinueKillAmount;
        private SerializedProperty continuousAmountOutAtOnce;
        private SerializedProperty continuousDelayWaves;
        private SerializedProperty delayBetweenWaves;
        private SerializedProperty delayBetweenSpawns;
        private SerializedProperty spawnOnLoad;
        private SerializedProperty restartSpawnerOnPlayerDistance;
        private SerializedProperty dontResetMobOnDisable;
        private SerializedProperty waveStartEvent;
        private SerializedProperty waveEndEvent;
        private SerializedProperty allWavesCompleted;
        private SerializedProperty allEnemiesDefeated;
        private SerializedProperty alsoCheckForTag;

        protected static readonly string[] EnemyTabs = { "Enemies", "Player Detection", "Events" };
        private string[] allTabs;
        private string[] waveTabs;
        private List<int> removeFromDict = new List<int>();

        public override void OnEnable()
        {
            base.OnEnable();

            if (enemySpawner == null)
                enemySpawner = (target as EnemySpawner);

            list = serializedObject.FindProperty("mobs");
            spawnTimer = serializedObject.FindProperty("spawnTimer");
            currentWave = serializedObject.FindProperty("currentWave");
            spawnNextWave = serializedObject.FindProperty("spawnNextWave");
            sameTimerEveryWave = serializedObject.FindProperty("sameTimerEveryWave");
            numberOfWaves = serializedObject.FindProperty("numberOfWaves");
            sameMobsEveryWave = serializedObject.FindProperty("sameMobsEveryWave");
            spawnContinueKillAmount = serializedObject.FindProperty("spawnContinueKillAmount");
            continuousDelayWaves = serializedObject.FindProperty("continuousDelayWaves");
            continuousAmountOutAtOnce = serializedObject.FindProperty("continuousAmountOutAtOnce");
            delayBetweenWaves = serializedObject.FindProperty("delayBetweenWaves");
            delayBetweenSpawns = serializedObject.FindProperty("delayBetweenSpawns");

            // PlayerDetectionData
            SerializedProperty playerDetectionData = serializedObject.FindProperty("playerDetectionData");
            playerCheckRadius = playerDetectionData.FindPropertyRelative("playerCheckRadius");
            showSearchRadius = playerDetectionData.FindPropertyRelative("showSearchRadius");
            playerLayer = playerDetectionData.FindPropertyRelative("playerLayer");
            showRadiusOnlyWhenSelected = playerDetectionData.FindPropertyRelative("showRadiusOnlyWhenSelected");
            disableOnPlayerDistance = playerDetectionData.FindPropertyRelative("disableOnPlayerDistance");
            playerDistance = playerDetectionData.FindPropertyRelative("playerDistance");
            detectFromSeperateObject = playerDetectionData.FindPropertyRelative("detectFromSeperateObject");
            objectToDetectFrom = playerDetectionData.FindPropertyRelative("objectToDetectFrom");
            spawnOnLoad = playerDetectionData.FindPropertyRelative("spawnOnLoad");
            restartSpawnerOnPlayerDistance = playerDetectionData.FindPropertyRelative("restartSpawnerOnPlayerDistance");
            dontResetMobOnDisable = playerDetectionData.FindPropertyRelative("dontResetMobOnDisable");
            alsoCheckForTag = playerDetectionData.FindPropertyRelative("alsoCheckForTag");
            // End PlayerDetectionData

            // Events
            SerializedProperty spawnerEvents = serializedObject.FindProperty("spawnerEvents");
            waveStartEvent = spawnerEvents.FindPropertyRelative("waveStartEvent");
            waveEndEvent = spawnerEvents.FindPropertyRelative("waveEndEvent");
            allWavesCompleted = spawnerEvents.FindPropertyRelative("allWavesCompleted");
            allEnemiesDefeated = spawnerEvents.FindPropertyRelative("allEnemiesDefeated");
            // End Events

#if VEGETATION_STUDIO_PRO
            List<string> combinedTabs = EnemyTabs.Concatenate(Tabs).ToList();
            combinedTabs.Add(integrationTab);
#else
            List<string> combinedTabs = EnemyTabs.Concatenate(Tabs).ToList();
#endif

            allTabs = combinedTabs.ToArray();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (enemySpawner == null)
                enemySpawner = (target as EnemySpawner);

            serializedObject.Update();

            GUIStyle layoutStyle = new GUIStyle("box");
            layoutStyle.normal.textColor = editorSettings.fgColor;
            layoutStyle.hover.textColor = editorSettings.fgColor;
            layoutStyle.fontStyle = FontStyle.Bold;
            layoutStyle.fontSize = 18;

            GUIStyle labelStyle = new GUIStyle();
            labelStyle.normal.textColor = editorSettings.fgColor;
            labelStyle.hover.textColor = editorSettings.fgColor;
            labelStyle.fontStyle = FontStyle.Bold;

            // Title and background colors
            GUI.backgroundColor = editorSettings.bgColor;
            GUILayout.BeginVertical("ENEMY SPAWNER", layoutStyle);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUI.backgroundColor = editorSettings.fgColor;

            GUILayout.BeginVertical(layoutStyle);
#if VEGETATION_STUDIO_PRO
            if (!enemySpawner.SpawnInBiome)
                // Shows Spawner Info
                SpawnerInfoPanel(layoutStyle);
#else
            if (enemySpawner.SPAWNTYPE == SpawnerMask.SpawnType.Area)
                // Shows Spawner Info
                SpawnerInfoPanel(layoutStyle);
#endif
            // Tabs Grid
            GUIStyle tabStyle = new GUIStyle(EditorStyles.miniButton);
            tabStyle.fixedHeight = 24;
            tabStyle.fontSize = 12;
            enemySpawner.curEditorTab = GUILayout.SelectionGrid(enemySpawner.curEditorTab, allTabs, 3, tabStyle);

            // Show properties of selected tab
            switch (enemySpawner.curEditorTab)
            {
                case 0:
                    ShowEnemies(labelStyle);
                    break;
                case 1:
                    ShowPlayerSettings(labelStyle);
                    break;
                case 2:
                    ShowUnityEvents();
                    break;
                case 3:
                    base.ShowSpawnerSettings(labelStyle);
                    break;
                case 4:
                    base.ShowEditorSettings();
                    EnemySpawnerEditorSettings();
                    break;
#if VEGETATION_STUDIO_PRO
                case 5:
                    base.ShowVegetationStudioProSettings(labelStyle);
                    break;
#endif
            }
            GUILayout.EndVertical();
            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        private void ShowEnemies(GUIStyle labelStyle)
        {
            EditorGUILayout.LabelField("General Wave Settings", labelStyle);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(numberOfWaves, new GUIContent("Number of Waves", "The amount of waves to spawn"), GUILayout.Height(spacing));
            if (EditorGUI.EndChangeCheck())
                UpdateWaves();
            EditorGUILayout.PropertyField(sameMobsEveryWave, new GUIContent("Same Enemies Each Wave", "Each wave has the same amount and type of enemy spawning"), GUILayout.Height(spacing));
            EditorGUILayout.PropertyField(delayBetweenSpawns, new GUIContent("Delay Between Spawns", "How long (in seconds) to wait between each enemy spawn"), GUILayout.Height(spacing));

            EditorGUILayout.LabelField("Waves Spawn Settings", labelStyle);
            EditorGUILayout.PropertyField(spawnNextWave, new GUIContent("Spawn Next Wave Method", "How the subsequent waves will spawn. On a timer, after wave died, or spawn continuously on kills"), GUILayout.Height(spacing));

            switch (spawnNextWave.enumValueIndex)
            {
                case (int)EnemySpawner.SpawnNextWave.Timer:
                    // Show same timer each wave field
                    ShowFieldsIfApplicable(!sameMobsEveryWave.boolValue, (bool x) =>
                    {
                        string tooltip = (x ? "" : "To Enable Field: Unselect Same Enemies Each Wave \n") + "Each wave has the same time between them";
                        EditorGUILayout.PropertyField(sameTimerEveryWave, new GUIContent("Same Timer Each Wave", tooltip), GUILayout.Height(spacing));
                        if (!x && editorSettings.showDisabledFields)
                            sameTimerEveryWave.boolValue = true;
                    });

                    // Show spawn timer field
                    ShowFieldsIfApplicable(!sameTimerEveryWave.boolValue, (bool x) =>
                    {
                        string tooltip = (x ? "" : "To Enable Field: Select Same Timer Each Wave \n") + "Number of seconds to wait before spawning another batch";
                        EditorGUILayout.PropertyField(spawnTimer, new GUIContent("Spawn Timer", tooltip), GUILayout.Height(spacing));
                    });
                    break;
                case (int)EnemySpawner.SpawnNextWave.After_Wave_Killed:
                    EditorGUILayout.PropertyField(delayBetweenWaves, new GUIContent("Delay Between Waves", "How long (in seconds) to wait after a wave ended to start spawning the next wave"), GUILayout.Height(spacing));
                    break;

                case (int)EnemySpawner.SpawnNextWave.Continuous_Kills:
                    EditorGUILayout.PropertyField(continuousAmountOutAtOnce, new GUIContent("Max Enemies Out", "How many enemies can be out at one time (mainly for the initial amount spawned)"), GUILayout.Height(spacing));
                    EditorGUILayout.PropertyField(spawnContinueKillAmount, new GUIContent("Kills Before Spawn More", "How many enemies should be killed before spawning more (continues until all enemies from all waves are dead)"), GUILayout.Height(spacing));
                    EditorGUILayout.PropertyField(continuousDelayWaves, new GUIContent("Delay Between Waves", "How long (in seconds) to wait after a wave ended (the last enemy died) to start spawning the next wave"), GUILayout.Height(spacing));
                    break;
            }

            EditorGUILayout.HelpBox("Spawn a random number of enemies between their provided range for each spawn group", MessageType.Info);
            if (!sameMobsEveryWave.boolValue)
            {
                // Show wave tabs
                WaveTabs();
            }
            else
            {
                EditorGUILayout.PropertyField(list, new GUIContent("Enemies to Spawn"), true);
            }
        }

        private void WaveTabs()
        {
            waveTabs = new string[numberOfWaves.intValue];
            for (int i = 0; i < numberOfWaves.intValue; i++)
                waveTabs[i] = string.Concat("Wave ", i + 1);

            GUIStyle tabStyle = new GUIStyle(EditorStyles.miniButtonMid);
            tabStyle.fixedHeight = 24;
            tabStyle.fontSize = 13;

            if (enemySpawner.CurWaveTab > numberOfWaves.intValue - 1)
                enemySpawner.CurWaveTab = numberOfWaves.intValue - 1;
            else if (enemySpawner.CurWaveTab < 0)
                enemySpawner.CurWaveTab = 0;

            if (enemySpawner.Waves.dictionary.Count != numberOfWaves.intValue)
                UpdateWaves();

            enemySpawner.CurWaveTab = GUILayout.SelectionGrid(enemySpawner.CurWaveTab, waveTabs, 4, tabStyle);
            enemySpawner.currentWave = enemySpawner.Waves.dictionary[enemySpawner.CurWaveTab].mobs;
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(currentWave, new GUIContent("Enemies in Wave", "The enemies that will be in a given wave."), true);
            if (EditorGUI.EndChangeCheck())
                UpdateEditorWave(enemySpawner.CurWaveTab);

            EditorGUILayout.Space();
            // Wave Timer
            if (spawnNextWave.enumValueIndex == (int)EnemySpawner.SpawnNextWave.Timer)
            {
                // Show waves timer Field
                ShowFieldsIfApplicable(!sameTimerEveryWave.boolValue, (bool x) =>
                {
                    EditorGUI.BeginChangeCheck();
                    float tempTimer = EditorGUILayout.FloatField(new GUIContent("Timer", "Time before the next wave spawns"), enemySpawner.Waves.dictionary[enemySpawner.CurWaveTab].timer, GUILayout.Height(spacing));
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(enemySpawner, "Wave Timer Changed");
                        enemySpawner.Waves.dictionary[enemySpawner.CurWaveTab].timer = tempTimer;
                        UpdateEditorWave(enemySpawner.CurWaveTab);
                    }
                });
            }

            /*EditorGUILayout.Space();
            if (GUILayout.Button("Clear Enemies in Wave"))
            {
                enemySpawner.Waves.dictionary[enemySpawner.CurWaveTab].mobs.Clear();
                enemySpawner.editorWaves.dictionary[enemySpawner.CurWaveTab].mobs.Clear();
            }*/
        }

        private void UpdateWaves()
        {
            for (int i = 0; i < numberOfWaves.intValue; i++)
            {
                if (!enemySpawner.Waves.dictionary.ContainsKey(i))
                {
                    if (enemySpawner.editorWaves.dictionary.ContainsKey(i))
                        enemySpawner.Waves.dictionary.Add(i, new EnemySpawner.Wave() { mobs = new List<Mobs>(enemySpawner.editorWaves.dictionary[i].mobs), timer = enemySpawner.editorWaves.dictionary[i].timer });
                    else
                    {
                        enemySpawner.Waves.dictionary.Add(i, new EnemySpawner.Wave() { mobs = new List<Mobs>(), timer = 0 });
                        enemySpawner.editorWaves.dictionary.Add(i, new EnemySpawner.Wave() { mobs = new List<Mobs>(), timer = 0 });
                    }
                }
            }

            // If category removed, then remove it from dictionary
            removeFromDict.Clear();
            foreach (KeyValuePair<int, EnemySpawner.Wave> wave in enemySpawner.Waves.dictionary)
            {
                if (wave.Key >= numberOfWaves.intValue)
                    removeFromDict.Add(wave.Key);
            }

            foreach (int key in removeFromDict)
            {
                if (enemySpawner.Waves.dictionary.ContainsKey(key))
                    enemySpawner.Waves.dictionary.Remove(key);
            }
        }

        private void UpdateEditorWave(int index)
        {
            if (enemySpawner.editorWaves.dictionary.ContainsKey(index))
            {
                enemySpawner.editorWaves.dictionary[index].mobs = new List<Mobs>(enemySpawner.Waves.dictionary[index].mobs);
                enemySpawner.editorWaves.dictionary[index].timer = enemySpawner.Waves.dictionary[index].timer;
            }
            else
                enemySpawner.editorWaves.dictionary.Add(index, new EnemySpawner.Wave() { mobs = new List<Mobs>(enemySpawner.Waves.dictionary[index].mobs), timer = enemySpawner.Waves.dictionary[index].timer });
        }

        private void ShowPlayerSettings(GUIStyle labelStyle)
        {
            EditorGUILayout.LabelField("Tag and Layer Settings", labelStyle);
            // Show Player Layer Field
            ShowFieldsIfApplicable(!spawnOnLoad.boolValue, (bool x) =>
            {
                EditorGUILayout.PropertyField(playerLayer, new GUIContent("Player Layer", "The layer used to search for players"), GUILayout.Height(spacing));
            });

            // Show Player Tag Field
            ShowFieldsIfApplicable(disableOnPlayerDistance.boolValue || restartSpawnerOnPlayerDistance.boolValue || alsoCheckForTag.boolValue, (bool x) =>
            {
                EditorGUI.BeginChangeCheck();
                string tempTag = EditorGUILayout.TagField("Player Tag", enemySpawner.PlayerTag, GUILayout.Height(spacing));
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "changed spawner player tag");
                    enemySpawner.PlayerTag = tempTag;
                }
            });

            EditorGUILayout.LabelField("Start Spawning on Game Load", labelStyle);
            EditorGUILayout.PropertyField(spawnOnLoad, new GUIContent("Spawn on Load", "Enemies will start spawning on game load. Does not use player detection."), GUILayout.Height(spacing));

            // Show all player search to start spawning fields
            ShowFieldsIfApplicable(!spawnOnLoad.boolValue, (bool x) =>
            {
                EditorGUILayout.LabelField("Player Search To Start Spawning", labelStyle);
                EditorGUILayout.HelpBox("Spawning starts when an object of player layer is within search radius.", MessageType.Info);
                EditorGUILayout.PropertyField(playerCheckRadius, new GUIContent("Player Search Radius", "The radius around the spawner to search for players."), GUILayout.Height(spacing));
                EditorGUILayout.PropertyField(alsoCheckForTag, new GUIContent("Check For Tag And Layer", "Checks for player tag after checking for layer"), GUILayout.Height(spacing));
                EditorGUILayout.PropertyField(detectFromSeperateObject, new GUIContent("Search From Seperate Object", "Use the location of a seperate object to search for players"), GUILayout.Height(spacing));
                if (detectFromSeperateObject.boolValue)
                {
                    EditorGUILayout.PropertyField(objectToDetectFrom, new GUIContent("Object to Search From", "The location of this object will be used to search for players"), GUILayout.Height(spacing));
                }
                else if (editorSettings.showDisabledFields)
                {
                    GUI.enabled = false;
                    EditorGUILayout.PropertyField(objectToDetectFrom, new GUIContent("Object to Search From", "To Enable Field: Select Search From Seperate Object \nThe location of this object will be used to search for players"), GUILayout.Height(spacing));
                    GUI.enabled = true;
                }
            });

            EditorGUILayout.LabelField("On Player Distance", labelStyle);
            EditorGUILayout.HelpBox("If you want to keep enemy's current state (such as current health) don't use this.", MessageType.Warning);
            EditorGUILayout.PropertyField(disableOnPlayerDistance, new GUIContent("Disable Enemies on Player Distance ", "Disable Enemies when they're too far from any player (using player distance)"), GUILayout.Height(spacing));

            // Show don't reset mobs on disable field
            ShowFieldsIfApplicable(disableOnPlayerDistance.boolValue && enemySpawner.SPAWNMETHOD == SpawnerMask.SpawnMethod.Instantiate, (bool x) =>
            {
                EditorGUI.indentLevel++;
                string tooltip = (x ? "" : "~~CANNOT BE USED WITH POOLING~~\nTo Enable Field: Use Instantiation and Select Disable Enemies on Player Distance\n") + "When enemies are disabled don't use a new spawn";
                EditorGUILayout.PropertyField(dontResetMobOnDisable, new GUIContent("Don't Reset Enemies On Disable ", tooltip), GUILayout.Height(spacing));
                if (editorSettings.showDisabledFields)
                    dontResetMobOnDisable.boolValue = false;
                EditorGUI.indentLevel--;
            });

            EditorGUILayout.PropertyField(restartSpawnerOnPlayerDistance, new GUIContent("Restart Spawner on Player Distance ", "Restart the spawner when player is too far away (using player distance)"), GUILayout.Height(spacing));

            // Show don't reset mobs on disable field
            ShowFieldsIfApplicable(disableOnPlayerDistance.boolValue || restartSpawnerOnPlayerDistance.boolValue, (bool x) =>
            {
                EditorGUI.indentLevel++;
                string tooltip = (x ? "" : "To Enable Field: Select Disable Enemies on Player Distance or Restart Spawner on Player Distance \n") + "If enemies are further than this distance they disable.";
                EditorGUILayout.PropertyField(playerDistance, new GUIContent("Player Distance", tooltip), GUILayout.Height(spacing));
                EditorGUI.indentLevel--;
            });
        }

        private void ShowUnityEvents()
        {
            EditorGUILayout.HelpBox("In order to get the current wave number in \"On Wave Start\" and \"On Wave End\" you must:\n1. Pick your function in \"Dynamic int\" on the event field\n2. Your function must take an \"int\" parameter", MessageType.Warning);
            EditorGUILayout.PropertyField(waveStartEvent, new GUIContent("On Wave Start", "Triggers when a wave starts"));
            EditorGUILayout.PropertyField(waveEndEvent, new GUIContent("On Wave End", "Triggers when a wave ends"));
            EditorGUILayout.PropertyField(allWavesCompleted, new GUIContent("On All Waves Done", "Triggers when all waves are done"));
            EditorGUILayout.HelpBox("This was necessary because, when spawning next wave with \"Timer\" or \"Continuous Kills\" all waves may end before all enemies are killed.", MessageType.Info);
            EditorGUILayout.PropertyField(allEnemiesDefeated, new GUIContent("On All Enemies Defeated", "Triggers when all enemies are defeated"));
        }

        private void EnemySpawnerEditorSettings()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(showSearchRadius, new GUIContent("Show Search Radius", "Show wire sphere in sceneview which indicates the radius to search for players"), GUILayout.Height(spacing));
            if (EditorGUI.EndChangeCheck())
                SceneView.RepaintAll();

            // Show radius only when selected field
            ShowFieldsIfApplicable(showSearchRadius.boolValue, (bool x) =>
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                string tooltip = (x ? "" : "To Enable Field: Select Show Search Radius \n") + "Show the search radius only when gameobject is selected";
                EditorGUILayout.PropertyField(showRadiusOnlyWhenSelected, new GUIContent("Show Radius Only On Selected", tooltip), GUILayout.Height(spacing));
                if (EditorGUI.EndChangeCheck())
                    SceneView.RepaintAll();
                EditorGUI.indentLevel--;
            });
        }

        private void ShowFieldsIfApplicable(bool conditional, Action<bool> fields)
        {
            if (conditional)
            {
                fields(conditional);
            }
            else if (editorSettings.showDisabledFields)
            {
                GUI.enabled = false;
                fields(conditional);
                GUI.enabled = true;
            }
        }

        public override void OnSceneGUI()
        {
            base.OnSceneGUI();
        }
    }
}