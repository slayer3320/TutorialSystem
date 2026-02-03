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
        private int selectedPhaseIndex = -1;

        [MenuItem("Window/Tutorial System/Debug Window")]
        public static void ShowWindow()
        {
            var window = GetWindow<TutorialDebugWindow>("教程调试");
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
            GUILayout.Label("教程系统调试窗口", EditorStyles.boldLabel);
            
            var statusStyle = new GUIStyle(EditorStyles.label);
            if (Application.isPlaying)
            {
                statusStyle.normal.textColor = Color.green;
                GUILayout.Label("● 运行中", statusStyle);
            }
            else
            {
                statusStyle.normal.textColor = Color.gray;
                GUILayout.Label("● 编辑模式", statusStyle);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawRuntimeControls()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("运行时控制", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            var manager = TutorialManager.Instance;
            if (manager == null)
            {
                EditorGUILayout.HelpBox("TutorialManager 实例未找到", MessageType.Warning);
                EditorGUILayout.EndVertical();
                return;
            }

            DrawRuntimeStatus(manager);
            EditorGUILayout.Space(10);
            DrawControlButtons(manager);

            EditorGUILayout.EndVertical();
        }

        private void DrawRuntimeStatus(TutorialManager manager)
        {
            EditorGUILayout.LabelField("当前状态", EditorStyles.boldLabel);
            
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.LabelField("运行状态", manager.IsRunning ? "运行中" : "已停止");
                
                if (manager.IsRunning)
                {
                    EditorGUILayout.LabelField("当前教程", manager.CurrentConfig?.tutorialName ?? "无");
                    EditorGUILayout.LabelField("当前阶段", 
                        $"{manager.CurrentPhase?.phaseName ?? "无"} ({manager.CurrentPhaseIndex + 1}/{manager.CurrentConfig?.phases.Count ?? 0})");
                    EditorGUILayout.LabelField("当前步骤", 
                        $"{manager.CurrentStep?.stepName ?? "无"} ({manager.CurrentStepIndex + 1}/{manager.CurrentPhase?.steps.Count ?? 0})");
                    
                    EditorGUILayout.Space(5);
                    var progress = manager.Progress;
                    var rect = EditorGUILayout.GetControlRect(false, 20);
                    EditorGUI.ProgressBar(rect, progress, $"总进度: {progress:P0}");
                }
            }
        }

        private void DrawControlButtons(TutorialManager manager)
        {
            EditorGUILayout.LabelField("控制", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = manager.IsRunning;
            if (GUILayout.Button("上一步", GUILayout.Height(30)))
                manager.PrevStep();
            if (GUILayout.Button("下一步", GUILayout.Height(30)))
                manager.NextStep();
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = manager.IsRunning;
            if (GUILayout.Button("下一阶段"))
                manager.NextPhase();
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = manager.IsRunning;
            if (GUILayout.Button("停止教程", GUILayout.Height(25)))
                manager.StopTutorial();
            if (GUILayout.Button("跳过教程", GUILayout.Height(25)))
                manager.SkipTutorial();
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            // 跳转到阶段
            if (manager.IsRunning && manager.CurrentConfig != null)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("跳转到阶段:", EditorStyles.miniLabel);
                
                for (int i = 0; i < manager.CurrentConfig.phases.Count; i++)
                {
                    var phase = manager.CurrentConfig.phases[i];
                    EditorGUILayout.BeginHorizontal();
                    
                    var isCurrent = i == manager.CurrentPhaseIndex;
                    var style = isCurrent ? EditorStyles.boldLabel : EditorStyles.label;
                    EditorGUILayout.LabelField($"{i + 1}. {phase.phaseName}", style);
                    
                    if (GUILayout.Button("跳转", GUILayout.Width(50)))
                        manager.JumpToPhase(i);
                    
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private void DrawEditorInfo()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("编辑模式", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("在 Play 模式下可使用调试功能", MessageType.Info);
            EditorGUILayout.EndVertical();
        }

        private void OnInspectorUpdate()
        {
            if (Application.isPlaying)
                Repaint();
        }
    }
}
