using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ARAWorks.Base.Extensions;
using ARAWorks.Spawner;

#if VEGETATION_STUDIO_PRO
using AwesomeTechnologies;
#endif

namespace ARAEditor.Spawner
{
    [CustomEditor(typeof(SpawnerMask)), CanEditMultipleObjects]
    public class SpawnerMaskEditor : Editor
    {
        private SpawnerMask enemySpawnArea;

        private SerializedProperty showBoundsAlways;
        private SerializedProperty showArea;
        private SerializedProperty showHandles;
        private SerializedProperty showNodes;
        private SerializedProperty groundLayerMask;
        private SerializedProperty heightFromGround;
        private SerializedProperty showName;
        private SerializedProperty useObjectName;
        private SerializedProperty maskName;
        private SerializedProperty startRotation;
        private SerializedProperty useRandomStartRotation;
        private SerializedProperty showRotArrow;
        private SerializedProperty spawnType;
        private SerializedProperty spawnMethod;
        private SerializedProperty useCustomInitialPoolPos;
        private SerializedProperty customPoolPos;
        private SerializedProperty obstaclesToAvoidLayers;
        private SerializedProperty obstacleAvoidPrecision;
        private SerializedProperty navMeshSpawn;
        private SerializedProperty startOutsideMainNavArea;
        private SerializedProperty startingNavArea;
        private SerializedProperty endPosition;
        private SerializedProperty useRandomPointNearEnd;
        private SerializedProperty endRadius;

#if VEGETATION_STUDIO_PRO
        private SerializedProperty vspBiome;
        private SerializedProperty spawnInBiome;
        protected static readonly string integrationTab = "Asset Integration";
#endif

        protected static readonly float spacing = 21;

        protected static readonly string[] Tabs = { "Spawner Settings", "Editor Settings" };

        protected static SpawnerPrefMenuHandler.SpawnerSettings editorSettings => SpawnerPrefMenuHandler.GetEditorSettings();

        public virtual void OnEnable()
        {
            if (enemySpawnArea == null)
                enemySpawnArea = (SpawnerMask)target;

            if (enemySpawnArea.Nodes.Count == 0)
            {
                enemySpawnArea.CreateDefaultNodes();
            }

            showBoundsAlways = serializedObject.FindProperty("showBoundsAlways");
            showArea = serializedObject.FindProperty("showArea");
            showHandles = serializedObject.FindProperty("showHandles");
            showNodes = serializedObject.FindProperty("showNodes");
            groundLayerMask = serializedObject.FindProperty("groundLayerMask");
            heightFromGround = serializedObject.FindProperty("heightFromGround");
            showName = serializedObject.FindProperty("showName");
            useObjectName = serializedObject.FindProperty("useObjectName");
            maskName = serializedObject.FindProperty("maskName");
            startRotation = serializedObject.FindProperty("startRotation");
            useRandomStartRotation = serializedObject.FindProperty("useRandomStartRotation");
            showRotArrow = serializedObject.FindProperty("showRotArrow");
            spawnType = serializedObject.FindProperty("spawnType");
            spawnMethod = serializedObject.FindProperty("spawnMethod");
            useCustomInitialPoolPos = serializedObject.FindProperty("useCustomInitialPoolPos");
            customPoolPos = serializedObject.FindProperty("customPoolPos");

            // NavMeshMovementData
            SerializedProperty navMeshMovementData = serializedObject.FindProperty("navMeshMoveData");
            startOutsideMainNavArea = navMeshMovementData.FindPropertyRelative("startOutsideMainNavArea");
            startingNavArea = navMeshMovementData.FindPropertyRelative("startingNavArea");
            endPosition = navMeshMovementData.FindPropertyRelative("endPosition");
            useRandomPointNearEnd = navMeshMovementData.FindPropertyRelative("useRandomPointNearEnd");
            endRadius = navMeshMovementData.FindPropertyRelative("endRadius");
            // End NavMeshMovementData

            // ObstacleAvoidanceData
            SerializedProperty obstacleAvoidanceData = serializedObject.FindProperty("obstacleAvoidanceData");
            obstaclesToAvoidLayers = obstacleAvoidanceData.FindPropertyRelative("obstaclesToAvoidLayers");
            obstacleAvoidPrecision = obstacleAvoidanceData.FindPropertyRelative("obstacleAvoidPrecision");
            navMeshSpawn = obstacleAvoidanceData.FindPropertyRelative("navMeshSpawn");
            // End ObstacleAvoidanceData

#if VEGETATION_STUDIO_PRO
            vspBiome = serializedObject.FindProperty("vspBiome");
            spawnInBiome = serializedObject.FindProperty("spawnInBiome");
#endif
        }

        public override void OnInspectorGUI()
        {
            if (enemySpawnArea == null)
                enemySpawnArea = (SpawnerMask)target;
        }

        public void SpawnerInfoPanel(GUIStyle layoutStyle)
        {
            GUILayout.BeginVertical(layoutStyle);
            if (enemySpawnArea.enabled && enemySpawnArea.SPAWNTYPE != SpawnerMask.SpawnType.Point)
            {
                EditorGUILayout.LabelField("Insert Node: Ctrl-Click");
                EditorGUILayout.LabelField("Delete Node: Ctrl-Shift-Click");
            }
            else if (!enemySpawnArea.enabled)
            {
                EditorGUILayout.HelpBox("Cannot edit nodes when disabled.", MessageType.Info);
            }

            if (enemySpawnArea.Nodes.Count < 3 && enemySpawnArea.SPAWNTYPE != SpawnerMask.SpawnType.Point)
                EditorGUILayout.HelpBox("There has to be at least 3 nodes to define the area", MessageType.Warning);
            GUILayout.EndVertical();
        }

        public void ShowEditorSettings()
        {
            // Always Show Bounds
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(showBoundsAlways, new GUIContent("Always Show Bounds", "Show in sceneview even when object isn't selected"), GUILayout.Height(spacing));
            if (EditorGUI.EndChangeCheck())
                SceneView.RepaintAll();
            // Show Area
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(showArea, new GUIContent("Show Area", "Show polygon outline in sceneview"), GUILayout.Height(spacing));
            if (EditorGUI.EndChangeCheck())
                SceneView.RepaintAll();
            // Show Handles
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(showHandles, new GUIContent("Show Handles", "Show handles in sceneview"), GUILayout.Height(spacing));
            if (EditorGUI.EndChangeCheck())
                SceneView.RepaintAll();
            // Show Nodes
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(showNodes, new GUIContent("Show Nodes", "Show spheres at each node in sceneview"), GUILayout.Height(spacing));
            if (EditorGUI.EndChangeCheck())
                SceneView.RepaintAll();
            // Show Start Rotation Arrow
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(showRotArrow, new GUIContent("Show Start Rotation Arrow", "Show arrow in sceneview representing the start rotation"), GUILayout.Height(spacing));
            if (EditorGUI.EndChangeCheck())
                SceneView.RepaintAll();
            // Show Spawner Name
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(showName, new GUIContent("Show Spawner Label", "Show label in center of spawner"), GUILayout.Height(spacing));
            if (EditorGUI.EndChangeCheck())
                SceneView.RepaintAll();
            // Spawner Name Settings
            if (showName.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(useObjectName, new GUIContent("Use Object Name", "Use gameobject's name as spawner label"), GUILayout.Height(spacing));

                if (!useObjectName.boolValue)
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(maskName, new GUIContent("Spawner Name", "Spawner's label text"), GUILayout.Height(spacing));
                    if (EditorGUI.EndChangeCheck())
                        SceneView.RepaintAll();
                }
                else
                {
                    if (maskName.stringValue != enemySpawnArea.name)
                    {
                        maskName.stringValue = enemySpawnArea.name;
                        SceneView.RepaintAll();
                    }

                    if (editorSettings.showDisabledFields)
                    {
                        GUI.enabled = false;
                        EditorGUILayout.PropertyField(maskName, new GUIContent("Spawner Name", "To Enable Field: Select Show Spawner Label and Unselect Use Object Name \nSpawner's label text"), GUILayout.Height(spacing));
                        GUI.enabled = true;
                    }
                }
                EditorGUI.indentLevel--;
            }
            else if (editorSettings.showDisabledFields)
            {
                GUI.enabled = false;
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(useObjectName, new GUIContent("Use Object Name", "To Enable Field: Select Show Spawner Label \nUse gameobject's name as spawner label"), GUILayout.Height(spacing));
                EditorGUILayout.PropertyField(maskName, new GUIContent("Spawner Name", "To Enable Field: Select Show Spawner Label and Unselect Use Object Name \nSpawner's label text"), GUILayout.Height(spacing));

                if (useObjectName.boolValue && maskName.stringValue != enemySpawnArea.name)
                {
                    maskName.stringValue = enemySpawnArea.name;
                    SceneView.RepaintAll();
                }

                EditorGUI.indentLevel--;
                GUI.enabled = true;
            }
        }

        public void ShowSpawnerSettings(GUIStyle labelStyle)
        {
            // Spawn Type
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Spawn Settings", labelStyle);
            EditorGUILayout.PropertyField(spawnType, new GUIContent("Spawner Type", "How objects will spawn"), GUILayout.Height(spacing));
            if (EditorGUI.EndChangeCheck())
                SceneView.RepaintAll();

            // Spawn Method
            EditorGUILayout.PropertyField(spawnMethod, new GUIContent("Spawning Method", "Whether objects will be instantiated or be pooled (pooling > Instantiation)"), GUILayout.Height(spacing));
            if (enemySpawnArea.SPAWNMETHOD == SpawnerMask.SpawnMethod.Pooling)
            {
                EditorGUILayout.PropertyField(useCustomInitialPoolPos, new GUIContent("Use Custom Initial Pool Pos", "Set a custom position where enemies are pooled before spawned"), GUILayout.Height(spacing));
                if (useCustomInitialPoolPos.boolValue == true)
                    EditorGUILayout.PropertyField(customPoolPos, new GUIContent("Custom Pool Pos", "The transform position used for custom pooling position."), GUILayout.Height(spacing));
                else if (editorSettings.showDisabledFields)
                {
                    GUI.enabled = false;
                    EditorGUILayout.PropertyField(customPoolPos, new GUIContent("Custom Pool Pos", "To Enable Field: check Use Custom Initial Pool Pos.\nThe transform position used for custom pooling position."), GUILayout.Height(spacing));
                    GUI.enabled = true;
                }
            }
            else if (editorSettings.showDisabledFields)
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(useCustomInitialPoolPos, new GUIContent("Use Custom Initial Pool Pos", "To Enable Field: Change Spawning Method to Pooling.\nSet a custom position where enemies are pooled before spawned"), GUILayout.Height(spacing));
                GUI.enabled = true;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(useRandomStartRotation, new GUIContent("Random Start Rotation", "Every spawned enemy will start with a random rotation"), GUILayout.Height(spacing));
            if (EditorGUI.EndChangeCheck())
                SceneView.RepaintAll();

            if (!useRandomStartRotation.boolValue)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(startRotation, new GUIContent("Start Rotation", "The start rotation of every spawned enemy"), GUILayout.Height(spacing));
                if (EditorGUI.EndChangeCheck())
                    SceneView.RepaintAll();
            }
            else if (editorSettings.showDisabledFields)
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(startRotation, new GUIContent("Start Rotation", "To Enable Field: Unselect Use Random Rotation \nThe start rotation of every spawned enemy"), GUILayout.Height(spacing));
                GUI.enabled = true;
            }

            // Ground Layers
            EditorGUILayout.LabelField("Layers To Spawn On", labelStyle);
            EditorGUILayout.PropertyField(groundLayerMask, new GUIContent("Ground Layer Mask", "The ground layers that will be used for selection when adding points and spawning."), GUILayout.Height(spacing));
            //EditorGUILayout.PropertyField(heightFromGround, new GUIContent("Height From Ground", "Distance above the ground to create new points"), GUILayout.Height(spacing));
            // Obstacle Avoidance
            EditorGUILayout.LabelField("Obstacle Avoidance", labelStyle);
            EditorGUILayout.PropertyField(navMeshSpawn, new GUIContent("NavMesh Spawning", "The spawner will only spawn on the NavMesh (can combine with obstacle avoidance)"), GUILayout.Height(spacing));
            EditorGUILayout.PropertyField(obstacleAvoidPrecision, new GUIContent("Avoidance Precision", "How precise the obstacle avoidance will be (if it can't avoid it won't spawn)"), GUILayout.Height(spacing));
            if (enemySpawnArea.ObstacleAvoidPercision != SpawnerObstacleAvoidanceHandler.AvoidancePrecision.None)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(obstaclesToAvoidLayers, new GUIContent("Layers To Avoid", "The layers containing obstacles you want to avoid when spawning"), GUILayout.Height(spacing));
                EditorGUI.indentLevel--;
            }
            else if (editorSettings.showDisabledFields)
            {
                GUI.enabled = false;
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(obstaclesToAvoidLayers, new GUIContent("Layers To Avoid", "To Enable Field: Avoidance Precision cannot be None \nThe layers containing obstacles you want to avoid when spawning"), GUILayout.Height(spacing));
                EditorGUI.indentLevel--;
                GUI.enabled = true;
            }

            // NavMesh Custom Movement
            if (enemySpawnArea.NavMeshSpawning == false)
                EditorGUILayout.HelpBox("These settings can only be used if \"NavMesh Spawning\" is active.", MessageType.Warning);
            EditorGUILayout.LabelField("NavMesh Custom Movement", labelStyle);
            if (enemySpawnArea.NavMeshSpawning == true)
            {
                ShowNavMeshMovementSettings();
            }
            else if (editorSettings.showDisabledFields)
            {
                GUI.enabled = false;
                ShowNavMeshMovementSettings();
                GUI.enabled = true;
            }
        }

        public void ShowNavMeshMovementSettings()
        {
            EditorGUILayout.PropertyField(startOutsideMainNavArea, new GUIContent("Start Outside Main NavMesh Area", "The enemies will start outside of the main NavMesh area and move to End Position inside of it."), GUILayout.Height(spacing));
            if (startOutsideMainNavArea.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(startingNavArea, new GUIContent("Starting NavMesh Area", "The NavMesh area the creatures on the spawner will start on."), GUILayout.Height(spacing));
                EditorGUILayout.PropertyField(endPosition, new GUIContent("Ending Position", "The position enemies will move to, to return to the main navmesh area."), GUILayout.Height(spacing));
                EditorGUILayout.PropertyField(useRandomPointNearEnd, new GUIContent("Use Random Point Near End Position", "The enemies will start outside of the main NavMesh area and move to End Position inside of it."), GUILayout.Height(spacing));
                if (useRandomPointNearEnd.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(endRadius, new GUIContent("End Radius", "The radius around the End Position that enemies will stop in."), GUILayout.Height(spacing));
                    EditorGUI.indentLevel--;
                }
                else if (editorSettings.showDisabledFields)
                {
                    GUI.enabled = false;
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(endRadius, new GUIContent("End Radius", "The radius around the End Position that enemies will stop in."), GUILayout.Height(spacing));
                    EditorGUI.indentLevel--;
                    GUI.enabled = true;
                }
                EditorGUI.indentLevel--;
            }
            else
            {
                GUI.enabled = false;
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(startingNavArea, new GUIContent("Starting NavMesh Area", "The NavMesh area the creatures on the spawner will start on."), GUILayout.Height(spacing));
                EditorGUILayout.PropertyField(endPosition, new GUIContent("Ending Position", "The position enemies will move to, to return to the main navmesh area."), GUILayout.Height(spacing));
                EditorGUILayout.PropertyField(useRandomPointNearEnd, new GUIContent("Use Random Point Near End Position", "The enemies will start outside of the main NavMesh area and move to End Position inside of it."), GUILayout.Height(spacing));
                if (useRandomPointNearEnd.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(endRadius, new GUIContent("End Radius", "The radius around the End Position that enemies will stop in."), GUILayout.Height(spacing));
                    EditorGUI.indentLevel--;
                }
                else if (editorSettings.showDisabledFields)
                {
                    GUI.enabled = false;
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(endRadius, new GUIContent("End Radius", "The radius around the End Position that enemies will stop in."), GUILayout.Height(spacing));
                    EditorGUI.indentLevel--;
                    GUI.enabled = true;
                }
                EditorGUI.indentLevel--;
                GUI.enabled = true;
            }
        }

        public void ShowVegetationStudioProSettings(GUIStyle labelStyle)
        {
#if VEGETATION_STUDIO_PRO
            EditorGUILayout.LabelField("Vegetation Studio Pro", labelStyle);
            EditorGUILayout.HelpBox("Using VSP Biome Spawning overrides Spawner Type", MessageType.Warning);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(spawnInBiome, new GUIContent("Use VSP Biome Spawning", "If true the spawner will match the VSP biome mask"), GUILayout.Height(spacing));
            if (EditorGUI.EndChangeCheck())
                SceneView.RepaintAll();

            if (spawnInBiome.boolValue)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(vspBiome, new GUIContent("Biome For Spawning", "The VSP biome to match and spawn within"), GUILayout.Height(spacing));
                if (EditorGUI.EndChangeCheck())
                    SceneView.RepaintAll();
            }
            else if (editorSettings.showDisabledFields)
            {
                GUI.enabled = false;
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(vspBiome, new GUIContent("Biome For Spawning", "To Enable Field: Select Use VSP Biome Spawning \nThe VSP biome to match and spawn within"), GUILayout.Height(spacing));
                EditorGUI.indentLevel--;
                GUI.enabled = true;
            }
#endif
        }

        public virtual void OnSceneGUI()
        {
            if (enemySpawnArea == null)
                enemySpawnArea = (SpawnerMask)target;

#if VEGETATION_STUDIO_PRO
            if (enemySpawnArea.VSPBiome != null && enemySpawnArea.SpawnInBiome)
            {
                if (!enemySpawnArea.CheckNodesEqual(enemySpawnArea.VSPBiome.GetWorldSpaceNodePositions(), enemySpawnArea.GetWorldSpaceNodePositions()))
                {
                    List<Vector3> biomeWSNodes = enemySpawnArea.VSPBiome.GetWorldSpaceNodePositions();
                    List<Vector3> localSpaceNodes = enemySpawnArea.GetLocalSpaceNodePositions(biomeWSNodes);
                    enemySpawnArea.Nodes = localSpaceNodes;
                    enemySpawnArea.InitializeTriangles();
                }
            }

            if (showHandles.boolValue && enemySpawnArea.enabled && enemySpawnArea.SPAWNTYPE == SpawnerMask.SpawnType.Area && !enemySpawnArea.SpawnInBiome)
                ControlsForAreaSpawning();
#else
            if (showHandles.boolValue && enemySpawnArea.enabled && enemySpawnArea.SPAWNTYPE == SpawnerMask.SpawnType.Area)
                ControlsForAreaSpawning();
#endif
        }

        private void ControlsForAreaSpawning()
        {
            Event currentEvent = Event.current;

            if (currentEvent.shift || currentEvent.control)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            }

            if (currentEvent.shift && currentEvent.control)
            {
                DeleteNode();
            }

            //Add Node
            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && currentEvent.control &&
                !currentEvent.shift && !currentEvent.alt)
            {
                AddNode(currentEvent);
            }

            // Draws Handles if not pressing shift or alt
            if (!currentEvent.shift && !currentEvent.alt)
            {
                ShowHandlesAndTrackMove();
            }
        }

        /// <summary>
        /// Paints the button for deleting a node and handles deleting it
        /// </summary>
        private void DeleteNode()
        {
            for (int i = 0; i < enemySpawnArea.Nodes.Count; i++)
            {
                Vector3 cameraPosition = SceneView.currentDrawingSceneView.camera.transform.position;
                Vector3 worldSpaceNode = enemySpawnArea.transform.TransformPoint(enemySpawnArea.Nodes[i]);
                float distance = Vector3.Distance(cameraPosition, worldSpaceNode);

                Handles.color = Color.red;
                if (Handles.Button(worldSpaceNode,
                    Quaternion.LookRotation(worldSpaceNode - cameraPosition, Vector3.up), 0.030f * distance,
                    0.015f * distance, Handles.CircleHandleCap))
                {
                    List<Vector3> nodes = enemySpawnArea.DeleteNode(enemySpawnArea.Nodes[i]);
                    enemySpawnArea.Nodes = nodes;
                    enemySpawnArea.InitializeTriangles();
                    EditorUtility.SetDirty(enemySpawnArea);
                }
            }
        }

        private void AddNode(Event currentEvent)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
            var hits = Physics.RaycastAll(ray, 10000f);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider is TerrainCollider || enemySpawnArea.GroundLayerMask.Contains(hits[i].collider.gameObject.layer))
                {
                    Vector3 newNode = new Vector3(hits[i].point.x, (hits[i].point.y + heightFromGround.floatValue), hits[i].point.z);

                    List<Vector3> nodes = enemySpawnArea.AddNode(newNode);
                    enemySpawnArea.Nodes = nodes;
                    enemySpawnArea.InitializeTriangles();
                    EditorUtility.SetDirty(enemySpawnArea);

                    currentEvent.Use();
                    break;
                }
            }
        }

        /// <summary>
        /// Shows handles for nodes and tracks their movement when handle dragged
        /// </summary>
        private void ShowHandlesAndTrackMove()
        {
            Vector3 cameraPosition = SceneView.currentDrawingSceneView.camera.transform.position;

            for (int i = 0; i < enemySpawnArea.Nodes.Count; i++)
            {
                EditorGUI.BeginChangeCheck();
                Vector3 worldSpaceNode = enemySpawnArea.transform.TransformPoint(enemySpawnArea.Nodes[i]);
                float distance = Vector3.Distance(cameraPosition, worldSpaceNode);
                if (distance > 200 && enemySpawnArea.Nodes.Count > 50) continue;

                Vector3 newWorldSpaceNode = Handles.PositionHandle(worldSpaceNode, Quaternion.identity);
                Vector3 node = enemySpawnArea.transform.InverseTransformPoint(newWorldSpaceNode);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "changed spawner added node");
                    Undo.RecordObject(enemySpawnArea.transform, "changed spawner added node");
                    enemySpawnArea.Nodes[i] = node;
                    enemySpawnArea.InitializeTriangles();
                    EditorUtility.SetDirty(enemySpawnArea);
                }
            }
        }
    }
}