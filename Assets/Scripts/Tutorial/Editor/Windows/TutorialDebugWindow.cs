using System.Linq;
using UnityEngine;
using UnityEditor;

namespace TutorialSystem.Editor
{
    /// <summary>
    /// 教程调试窗口
    /// </summary>
    public class TutorialDebugWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        private bool showPhasesFoldout = true;
        private bool showProgressFoldout = true;
        private int selectedPhaseIndex = -1;

        [MenuItem("Window/Tutorial System/Debug Window")]
        public static void ShowWindow()
        {
            var window = GetWindow<TutorialDebugWindow>("Tutorial Debug");
            window.minSize = new Vector2(400, 500);
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        }

        private void OnPlayModeChanged(PlayModeStateChange state)
        {
            Repaint();
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            DrawHeader();
            EditorGUILayout.Space(10);

            if (Application.isPlaying)
                DrawRuntimeControls();
            else
                DrawEditorInfo();

            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Tutorial System Debug Window", EditorStyles.boldLabel);
            
            var statusStyle = new GUIStyle(EditorStyles.label);
            if (Application.isPlaying)
            {
                statusStyle.normal.textColor = Color.green;
                GUILayout.Label("● Running", statusStyle);
            }
            else
            {
                statusStyle.normal.textColor = Color.gray;
                GUILayout.Label("● Editor Mode", statusStyle);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawRuntimeControls()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Runtime Controls", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            var manager = TutorialManager.Instance;
            if (manager == null)
            {
                EditorGUILayout.HelpBox("TutorialManager instance not found", MessageType.Warning);
                EditorGUILayout.EndVertical();
                return;
            }

            DrawRuntimeStatus(manager);
            EditorGUILayout.Space(10);
            DrawControlButtons(manager);
            EditorGUILayout.Space(10);
            DrawProgressSection();

            EditorGUILayout.EndVertical();
        }

        private void DrawRuntimeStatus(TutorialManager manager)
        {
            EditorGUILayout.LabelField("Current Status", EditorStyles.boldLabel);
            
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.LabelField("Run State", manager.IsRunning ? "Running" : "Stopped");
                
                if (manager.IsRunning)
                {
                    EditorGUILayout.LabelField("Current Tutorial", manager.CurrentConfig?.tutorialName ?? "None");
                    EditorGUILayout.LabelField("Current Phase", 
                        $"{manager.CurrentPhase?.phaseName ?? "None"} ({manager.CurrentPhaseIndex + 1}/{manager.CurrentConfig?.phases.Count ?? 0})");
                    EditorGUILayout.LabelField("Current Step", 
                        $"{manager.CurrentStep?.stepName ?? "None"} ({manager.CurrentStepIndex + 1}/{manager.CurrentPhase?.steps.Count ?? 0})");
                    
                    EditorGUILayout.Space(5);
                    var progress = manager.Progress;
                    var rect = EditorGUILayout.GetControlRect(false, 20);
                    EditorGUI.ProgressBar(rect, progress, $"Total Progress: {progress:P0}");
                }
            }
        }

        private void DrawControlButtons(TutorialManager manager)
        {
            EditorGUILayout.LabelField("Controls", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = manager.IsRunning;
            if (GUILayout.Button("Previous Step", GUILayout.Height(30)))
                manager.PrevStep();
            if (GUILayout.Button("Next Step", GUILayout.Height(30)))
                manager.NextStep();
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = manager.IsRunning;
            if (GUILayout.Button("Next Phase"))
                manager.NextPhase();
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = manager.IsRunning;
            if (GUILayout.Button("Stop Tutorial", GUILayout.Height(25)))
                manager.StopTutorial();
            if (GUILayout.Button("Skip Tutorial", GUILayout.Height(25)))
                manager.SkipTutorial();
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            // Jump to Phase
            if (manager.IsRunning && manager.CurrentConfig != null)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Jump to Phase:", EditorStyles.miniLabel);
                
                for (int i = 0; i < manager.CurrentConfig.phases.Count; i++)
                {
                    var phase = manager.CurrentConfig.phases[i];
                    EditorGUILayout.BeginHorizontal();
                    
                    var isCurrent = i == manager.CurrentPhaseIndex;
                    var style = isCurrent ? EditorStyles.boldLabel : EditorStyles.label;
                    EditorGUILayout.LabelField($"{i + 1}. {phase.phaseName}", style);
                    
                    if (GUILayout.Button("Jump", GUILayout.Width(50)))
                        manager.JumpToPhase(i);
                    
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private void DrawEditorInfo()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Editor Mode", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Debug features are available in Play Mode", MessageType.Info);
            EditorGUILayout.EndVertical();
        }

        private void DrawProgressSection()
        {
            showProgressFoldout = EditorGUILayout.Foldout(showProgressFoldout, "Tutorial Progress (Save Data)", true);
            if (!showProgressFoldout) return;

            var progressManager = TutorialProgressManager.Instance;
            if (progressManager == null)
            {
                EditorGUILayout.HelpBox("TutorialProgressManager instance not found.\nAdd TutorialProgressManager component to the scene.", MessageType.Warning);
                return;
            }

            EditorGUILayout.BeginVertical("box");

            var completedTutorials = progressManager.GetCompletedTutorials();
            EditorGUILayout.LabelField($"Completed Tutorials: {completedTutorials.Count}", EditorStyles.boldLabel);

            if (completedTutorials.Count > 0)
            {
                EditorGUILayout.Space(5);
                using (new EditorGUI.IndentLevelScope())
                {
                    foreach (var tutorialName in completedTutorials)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField($"✓ {tutorialName}", EditorStyles.label);
                        if (GUILayout.Button("Reset", GUILayout.Width(50)))
                        {
                            // 重置单个教程需要重新获取列表并移除
                            var list = progressManager.GetCompletedTutorials();
                            list.Remove(tutorialName);
                            progressManager.SetCompletedTutorials(list);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("No tutorials completed yet.", EditorStyles.miniLabel);
            }

            EditorGUILayout.Space(10);

            // 控制按钮
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear All Progress", GUILayout.Height(25)))
            {
                if (EditorUtility.DisplayDialog("Clear Tutorial Progress",
                    "Are you sure you want to clear all tutorial progress?", "Yes", "No"))
                {
                    progressManager.ClearAllProgress();
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void OnInspectorUpdate()
        {
            if (Application.isPlaying)
                Repaint();
        }
    }
}
