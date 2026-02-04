using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TutorialSystem.Editor
{
    [CustomEditor(typeof(TutorialRunner))]
    public class TutorialRunnerEditor : UnityEditor.Editor
    {
        private SerializedProperty configProp;
        private SerializedProperty autoStartOnEnableProp;
        private SerializedProperty phasesProp;

        // Foldout states
        private bool configFoldout = true;
        private bool phasesFoldout = true;
        private Dictionary<string, bool> phaseFoldouts = new Dictionary<string, bool>();
        private Dictionary<string, bool> stepFoldouts = new Dictionary<string, bool>();
        private Dictionary<string, bool> moduleFoldouts = new Dictionary<string, bool>();

        // Module type definitions
        private static readonly Dictionary<string, Type> ModuleTypes = new Dictionary<string, Type>
        {
            { "Popup Module", typeof(PopupModule) },
            { "Arrow Module", typeof(ArrowModule) },
            { "Highlight Module", typeof(HighlightModule) }
        };

        private void OnEnable()
        {
            configProp = serializedObject.FindProperty("config");
            autoStartOnEnableProp = serializedObject.FindProperty("autoStartOnEnable");
            phasesProp = configProp.FindPropertyRelative("phases");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var runner = target as TutorialRunner;

            DrawConfigSection();
            EditorGUILayout.Space(6);
            DrawPhasesSection();
            EditorGUILayout.Space(6);
            DrawRuntimeControls(runner);

            serializedObject.ApplyModifiedProperties();

            if (Application.isPlaying && runner.IsRunning)
                Repaint();
        }

        private void DrawConfigSection()
        {
            configFoldout = EditorGUILayout.Foldout(configFoldout, "Tutorial Configuration", true, EditorStyles.foldoutHeader);

            if (configFoldout)
            {
                EditorGUI.indentLevel++;

                var tutorialIdProp = configProp.FindPropertyRelative("tutorialId");
                var tutorialNameProp = configProp.FindPropertyRelative("tutorialName");
                var locTableProp = configProp.FindPropertyRelative("localizationTableName");
                var canSkipProp = configProp.FindPropertyRelative("canSkip");

                EditorGUILayout.PropertyField(tutorialNameProp, new GUIContent("Tutorial Name"));
                EditorGUILayout.PropertyField(locTableProp, new GUIContent("Localization Table"));

                EditorGUILayout.Space(2);
                EditorGUILayout.PropertyField(canSkipProp, new GUIContent("Can Skip"));
                EditorGUILayout.PropertyField(autoStartOnEnableProp, new GUIContent("Auto Start On Enable"));

                EditorGUI.indentLevel--;
            }
        }

        private void DrawPhasesSection()
        {
            phasesFoldout = EditorGUILayout.Foldout(phasesFoldout, $"Phases ({phasesProp.arraySize})", true, EditorStyles.foldoutHeader);

            if (phasesFoldout)
            {
                EditorGUI.indentLevel++;

                if (phasesProp.arraySize == 0)
                {
                    EditorGUILayout.HelpBox("No phases configured. Click 'Add Phase' to add one.", MessageType.Info);
                }

                for (int i = 0; i < phasesProp.arraySize; i++)
                {
                    DrawPhaseItem(i);
                }

                EditorGUILayout.Space(4);
                DrawAddRemoveButtons(phasesProp, "Phase");

                EditorGUI.indentLevel--;
            }
        }

        private void DrawPhaseItem(int phaseIndex)
        {
            var phaseProp = phasesProp.GetArrayElementAtIndex(phaseIndex);
            var phaseNameProp = phaseProp.FindPropertyRelative("phaseName");
            var stepsProp = phaseProp.FindPropertyRelative("steps");

            string phaseName = string.IsNullOrEmpty(phaseNameProp.stringValue) ? "Unnamed Phase" : phaseNameProp.stringValue;
            string phaseKey = $"phase_{phaseIndex}";
            if (!phaseFoldouts.ContainsKey(phaseKey))
                phaseFoldouts[phaseKey] = false;

            int stepCount = stepsProp.arraySize;

            // Phase header
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.BeginHorizontal();

                // Foldout with phase info
                string displayName = $"Phase {phaseIndex + 1}: {phaseName}  [{stepCount} Steps]";

                phaseFoldouts[phaseKey] = EditorGUILayout.Foldout(phaseFoldouts[phaseKey], displayName, true);

                // Delete button
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    if (EditorUtility.DisplayDialog("Delete Phase", 
                        $"Delete '{phaseName}'?", "Delete", "Cancel"))
                    {
                        phasesProp.DeleteArrayElementAtIndex(phaseIndex);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndVertical();
                        return;
                    }
                }

                EditorGUILayout.EndHorizontal();

                // Phase content
                if (phaseFoldouts[phaseKey])
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.PropertyField(phaseNameProp, new GUIContent("Phase Name"));

                    EditorGUILayout.Space(4);

                    // Steps
                    DrawStepsSection(stepsProp, phaseIndex);

                    EditorGUI.indentLevel--;
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawStepsSection(SerializedProperty stepsProp, int phaseIndex)
        {
            EditorGUILayout.LabelField($"Steps ({stepsProp.arraySize})", EditorStyles.boldLabel);

            if (stepsProp.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No steps in this phase.", MessageType.None);
            }

            for (int i = 0; i < stepsProp.arraySize; i++)
            {
                DrawStepItem(stepsProp, i, phaseIndex);
            }

            EditorGUILayout.Space(2);
            DrawAddRemoveButtons(stepsProp, "Step");
        }

        private void DrawStepItem(SerializedProperty stepsProp, int stepIndex, int phaseIndex)
        {
            var stepProp = stepsProp.GetArrayElementAtIndex(stepIndex);
            var stepNameProp = stepProp.FindPropertyRelative("stepName");
            var triggerProp = stepProp.FindPropertyRelative("completeTrigger");
            var modulesProp = stepProp.FindPropertyRelative("modules");

            string stepName = string.IsNullOrEmpty(stepNameProp.stringValue) ? "Unnamed Step" : stepNameProp.stringValue;
            string stepKey = $"step_{phaseIndex}_{stepIndex}";
            if (!stepFoldouts.ContainsKey(stepKey))
                stepFoldouts[stepKey] = false;

            string triggerType = GetTriggerTypeName(triggerProp);
            int moduleCount = modulesProp.arraySize;

            // Step item
            EditorGUILayout.BeginVertical("helpbox");
            {
                EditorGUILayout.BeginHorizontal();

                // Foldout with step info
                string displayName = $"Step {stepIndex + 1}: {stepName}  [{triggerType}] [{moduleCount} Modules]";

                stepFoldouts[stepKey] = EditorGUILayout.Foldout(stepFoldouts[stepKey], displayName, true);

                // Move buttons
                GUI.enabled = stepIndex > 0;
                if (GUILayout.Button("^", GUILayout.Width(20)))
                {
                    stepsProp.MoveArrayElement(stepIndex, stepIndex - 1);
                }
                GUI.enabled = stepIndex < stepsProp.arraySize - 1;
                if (GUILayout.Button("v", GUILayout.Width(20)))
                {
                    stepsProp.MoveArrayElement(stepIndex, stepIndex + 1);
                }
                GUI.enabled = true;

                // Delete button
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    stepsProp.DeleteArrayElementAtIndex(stepIndex);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    return;
                }

                EditorGUILayout.EndHorizontal();

                // Step content
                if (stepFoldouts[stepKey])
                {
                    EditorGUI.indentLevel++;
                    DrawStepContent(stepProp);
                    EditorGUI.indentLevel--;
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawStepContent(SerializedProperty stepProp)
        {
            bool useLocalization = GetGlobalLocalizationSetting();

            // Basic Info
            EditorGUILayout.LabelField("Basic Info", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(stepProp.FindPropertyRelative("stepName"), new GUIContent("Name"));

            EditorGUILayout.Space(4);

            // Content
            EditorGUILayout.LabelField("Content", EditorStyles.miniBoldLabel);
            if (useLocalization)
            {
                EditorGUILayout.PropertyField(stepProp.FindPropertyRelative("localizedTitle"), new GUIContent("Title"));
                EditorGUILayout.PropertyField(stepProp.FindPropertyRelative("localizedContent"), new GUIContent("Content"));
            }
            else
            {
                EditorGUILayout.PropertyField(stepProp.FindPropertyRelative("rawTitle"), new GUIContent("Title"));
                EditorGUILayout.PropertyField(stepProp.FindPropertyRelative("rawContent"), new GUIContent("Content"));
            }

            EditorGUILayout.Space(4);

            // Trigger
            EditorGUILayout.LabelField("Completion Trigger", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(stepProp.FindPropertyRelative("completeTrigger"), new GUIContent("Trigger"));

            EditorGUILayout.Space(4);

            // Modules
            var modulesProp = stepProp.FindPropertyRelative("modules");
            DrawModulesSection(modulesProp, stepProp.propertyPath);

            EditorGUILayout.Space(4);

            // Settings
            EditorGUILayout.LabelField("Settings", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(stepProp.FindPropertyRelative("pauseOnEnter"), new GUIContent("Pause On Enter"));
            EditorGUILayout.PropertyField(stepProp.FindPropertyRelative("resumeOnExit"), new GUIContent("Resume On Exit"));
        }

        private void DrawRuntimeControls(TutorialRunner runner)
        {
            EditorGUILayout.LabelField("Runtime Controls", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");

            bool hasConfig = phasesProp != null && phasesProp.arraySize > 0;

            if (!hasConfig)
            {
                EditorGUILayout.HelpBox("Please configure tutorial content.", MessageType.Warning);
            }
            else if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Tutorial controls are available in Play Mode.", MessageType.Info);
            }
            else
            {
                if (!runner.IsRunning)
                {
                    if (GUILayout.Button("Start Tutorial", GUILayout.Height(26)))
                        runner.StartTutorial();
                }
                else
                {
                    var manager = TutorialManager.Instance;
                    if (manager != null)
                    {
                        EditorGUILayout.LabelField("Current Phase", manager.CurrentPhase?.phaseName ?? "None");
                        EditorGUILayout.LabelField("Current Step", manager.CurrentStep?.stepName ?? "None");

                        var progressRect = EditorGUILayout.GetControlRect(false, 18);
                        EditorGUI.ProgressBar(progressRect, manager.Progress, $"Progress: {manager.Progress * 100:F0}%");
                    }

                    EditorGUILayout.Space(4);

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Stop"))
                        runner.StopTutorial();
                    if (GUILayout.Button("Skip"))
                        runner.SkipTutorial();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Previous"))
                        runner.PrevStep();
                    if (GUILayout.Button("Next"))
                        runner.NextStep();
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndVertical();
        }

        #region Helper Methods

        private void DrawModulesSection(SerializedProperty modulesProp, string stepPath)
        {
            EditorGUILayout.LabelField($"Modules ({modulesProp.arraySize})", EditorStyles.miniBoldLabel);

            if (modulesProp.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No modules configured.", MessageType.None);
            }

            for (int i = 0; i < modulesProp.arraySize; i++)
            {
                DrawModuleItem(modulesProp, i, stepPath);
            }

            EditorGUILayout.Space(2);
            DrawModuleAddRemoveButtons(modulesProp);
        }

        private void DrawModuleItem(SerializedProperty modulesProp, int moduleIndex, string stepPath)
        {
            var moduleProp = modulesProp.GetArrayElementAtIndex(moduleIndex);
            string moduleKey = $"{stepPath}_module_{moduleIndex}";

            if (!moduleFoldouts.ContainsKey(moduleKey))
                moduleFoldouts[moduleKey] = false;

            string moduleName = GetModuleTypeName(moduleProp);

            EditorGUILayout.BeginVertical("helpbox");
            {
                EditorGUILayout.BeginHorizontal();

                // Foldout with module type
                string displayName = $"Module {moduleIndex + 1}: {moduleName}";
                moduleFoldouts[moduleKey] = EditorGUILayout.Foldout(moduleFoldouts[moduleKey], displayName, true);

                // Move buttons
                GUI.enabled = moduleIndex > 0;
                if (GUILayout.Button("^", GUILayout.Width(20)))
                {
                    modulesProp.MoveArrayElement(moduleIndex, moduleIndex - 1);
                }
                GUI.enabled = moduleIndex < modulesProp.arraySize - 1;
                if (GUILayout.Button("v", GUILayout.Width(20)))
                {
                    modulesProp.MoveArrayElement(moduleIndex, moduleIndex + 1);
                }
                GUI.enabled = true;

                // Delete button
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    modulesProp.DeleteArrayElementAtIndex(moduleIndex);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    return;
                }

                EditorGUILayout.EndHorizontal();

                // Module content
                if (moduleFoldouts[moduleKey] && moduleProp.managedReferenceValue != null)
                {
                    EditorGUI.indentLevel++;
                    DrawModuleProperties(moduleProp);
                    EditorGUI.indentLevel--;
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawModuleProperties(SerializedProperty moduleProp)
        {
            EditorGUILayout.PropertyField(moduleProp, GUIContent.none, true);
        }

        private void DrawModuleAddRemoveButtons(SerializedProperty modulesProp)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // Add Module dropdown button
            if (GUILayout.Button("+ Add Module", GUILayout.Width(100)))
            {
                ShowAddModuleMenu(modulesProp);
            }

            if (modulesProp.arraySize > 0)
            {
                if (GUILayout.Button("- Remove Last", GUILayout.Width(100)))
                {
                    modulesProp.DeleteArrayElementAtIndex(modulesProp.arraySize - 1);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void ShowAddModuleMenu(SerializedProperty modulesProp)
        {
            var menu = new GenericMenu();

            foreach (var kvp in ModuleTypes)
            {
                var type = kvp.Value;
                var name = kvp.Key;
                menu.AddItem(new GUIContent(name), false, () =>
                {
                    modulesProp.InsertArrayElementAtIndex(modulesProp.arraySize);
                    var newElement = modulesProp.GetArrayElementAtIndex(modulesProp.arraySize - 1);
                    newElement.managedReferenceValue = Activator.CreateInstance(type);
                    modulesProp.serializedObject.ApplyModifiedProperties();
                });
            }

            menu.ShowAsContext();
        }

        private string GetModuleTypeName(SerializedProperty moduleProp)
        {
            if (moduleProp.managedReferenceValue == null)
                return "None";

            var module = moduleProp.managedReferenceValue as ITutorialModule;
            if (module != null)
                return module.ModuleName;

            return moduleProp.managedReferenceValue.GetType().Name.Replace("Module", "");
        }

        private void DrawAddRemoveButtons(SerializedProperty arrayProp, string itemName)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button($"+ Add {itemName}", GUILayout.Width(100)))
            {
                arrayProp.InsertArrayElementAtIndex(arrayProp.arraySize);
            }

            if (arrayProp.arraySize > 0)
            {
                if (GUILayout.Button("- Remove Last", GUILayout.Width(100)))
                {
                    arrayProp.DeleteArrayElementAtIndex(arrayProp.arraySize - 1);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private string GetTriggerTypeName(SerializedProperty triggerProp)
        {
            if (triggerProp.managedReferenceValue == null)
                return "None";

            var typeName = triggerProp.managedReferenceValue.GetType().Name;
            return typeName.Replace("Trigger", "");
        }

        private bool GetGlobalLocalizationSetting()
        {
            var manager = UnityEngine.Object.FindFirstObjectByType<TutorialManager>();
            if (manager != null)
            {
                var so = new SerializedObject(manager);
                var locProp = so.FindProperty("useLocalization");
                return locProp != null && locProp.boolValue;
            }
            return false;
        }

        #endregion
    }
}
