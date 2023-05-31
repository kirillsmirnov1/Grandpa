using System;
using UnityEngine;
using UnityUtils.Variables;

namespace Nightmares.Code.Model.Quests
{
    [Serializable]
    public struct QuestTask
    {
        [Header("Flags")]
        [SerializeField] private bool resetAtSessionStart;
        [SerializeField] private bool showProgress;

        [Header("Values")]
        [SerializeField] private AVariable value;
        [SerializeField] private QuestComparison comparison;
        [SerializeField] private int intTarget;

        public bool ShowProgress => showProgress;
        
        public bool Complete
        {
            get
            {
                var intVal = (value as IntVariable).Value;
                
                return comparison switch
                {
                    QuestComparison.Equal => intVal == intTarget,
                    QuestComparison.ValGreater => intVal > intTarget,
                    QuestComparison.ValLesser => intVal < intTarget,
                    QuestComparison.ValGreaterOrEqual => intVal >= intTarget,
                    QuestComparison.ValLesserOrEqual => intVal <= intTarget,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        public string CurrentProgress => $"[{(value as IntVariable).Value} / {intTarget}]";

        public void PrepareForSession()
        {
            if (!resetAtSessionStart) return;
            (value as IntVariable).Value = 0;
        }
    }
}