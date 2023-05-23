using System.Linq;
using UnityEngine;

namespace Nightmares.Code.Model.Quests
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "Data/Quests", order = 0)]
    public class Quest : ScriptableObject
    {
        [SerializeField] private string displayName;
        [SerializeField] private int minLevel;
        [SerializeField] private QuestTask[] tasks;

        public bool Complete => tasks.All(t => t.Complete);
    }
}