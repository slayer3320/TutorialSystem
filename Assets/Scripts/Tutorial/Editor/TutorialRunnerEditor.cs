using UnityEngine;
using UnityEditor;

namespace TutorialSystem.Editor
{
    [CustomEditor(typeof(TutorialRunner))]
    public class TutorialRunnerEditor : UnityEditor.Editor
    {
        private SerializedProperty configProp;
        private SerializedProperty autoStartOnEnableProp;

        private void OnEnable()
        {
            configProp = serializedObject.FindProperty("config");
            autoStartOnEnableProp = serializedObject.FindProperty("autoStartOnEnable");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var runner = target as TutorialRunner;

            // Tutorial Configuration
            EditorGUILayout.PropertyField(configProp, new GUIContent("Tutorial Config"), true);

            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(autoStartOnEnableProp, new GUIContent("Auto Start On Enable"));

            EditorGUILayout.Space(10);

            // Runtime Controls
            DrawRuntimeControls(runner);

            serializedObject.ApplyModifiedProperties();

            if (Application.isPlaying && runner.IsRunning)
                Repaint();
        }

        private void DrawRuntimeControls(TutorialRunner runner)
        {
            EditorGUILayout.LabelField("Runtime Controls", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");

            var phases = configProp.FindPropertyRelative("phases");
            bool hasConfig = phases != null && phases.arraySize > 0;

            if (!hasConfig)
            {
                EditorGUILayout.HelpBox("Please configure tutorial content", MessageType.Warning);
            }
            else if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Tutorial controls are available in Play Mode", MessageType.Info);
            }
            else
            {
                if (!runner.IsRunning)
                {
                    if (GUILayout.Button("Start Tutorial", GUILayout.Height(30)))
                        runner.StartTutorial();
                }
                else
                {
                    var manager = TutorialManager.Instance;
                    if (manager != null)
                    {
                        EditorGUILayout.LabelField("Current Phase", manager.CurrentPhase?.phaseName ?? "None");
                        EditorGUILayout.LabelField("Current Step", manager.CurrentStep?.stepName ?? "None");
                        EditorGUILayout.LabelField("Progress", $"{manager.Progress * 100:F0}%");
                    }

                    EditorGUILayout.Space(5);

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Stop"))
                        runner.StopTutorial();
                    if (GUILayout.Button("Skip"))
                        runner.SkipTutorial();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Previous Step"))
                        runner.PrevStep();
                    if (GUILayout.Button("Next Step"))
                        runner.NextStep();
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndVertical();
        }
    }
}
