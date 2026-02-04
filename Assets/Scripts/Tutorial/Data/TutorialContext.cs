using System.Collections.Generic;
using UnityEngine;

namespace TutorialSystem
{
    /// <summary>
    /// 教程运行时上下文
    /// </summary>
    public class TutorialContext
    {
        public TutorialConfig Config { get; private set; }
        public TutorialPhase CurrentPhase { get; set; }
        public TutorialStep CurrentStep { get; set; }
        public int PhaseIndex { get; set; }
        public int StepIndex { get; set; }
        public bool IsRunning { get; set; }
        public Canvas TargetCanvas { get; set; }

        public float Progress
        {
            get
            {
                var phases = Config?.phases;
                if (phases == null || phases.Count == 0) return 0f;
                
                int totalSteps = 0;
                int completedSteps = 0;
                
                for (int i = 0; i < phases.Count; i++)
                {
                    var phase = phases[i];
                    
                    for (int j = 0; j < phase.steps.Count; j++)
                    {
                        totalSteps++;
                        
                        if (i < PhaseIndex || (i == PhaseIndex && j < StepIndex))
                            completedSteps++;
                    }
                }
                
                return totalSteps > 0 ? (float)completedSteps / totalSteps : 0f;
            }
        }

        public TutorialContext(TutorialConfig config)
        {
            Config = config;
            PhaseIndex = -1;
            StepIndex = -1;
            IsRunning = false;
        }

        public void Reset()
        {
            CurrentPhase = null;
            CurrentStep = null;
            PhaseIndex = -1;
            StepIndex = -1;
            IsRunning = false;
        }
    }
}
