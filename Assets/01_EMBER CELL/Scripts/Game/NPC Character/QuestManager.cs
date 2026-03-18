using System;
using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    // 수정
    public enum QuestState
    {
        NotStarted,
        InProgress,
        Completed
    }

    public class QuestManager : SingletonBase<QuestManager>
    {

        public event Action<string, QuestState> OnQuestStateChanged;
        private Dictionary<string, QuestState> questStateMap = new();

        public QuestState GetQuestState(string questID)
        {
            if (string.IsNullOrEmpty(questID))
                return QuestState.NotStarted;

            if (questStateMap.TryGetValue(questID, out var state))
                return state;

            return QuestState.NotStarted;
        }

        public bool HasQuest(string questID)
        {
            if (string.IsNullOrEmpty(questID))
                return false;

            return questStateMap.ContainsKey(questID);
        }

        public void StartQuest(string questID)
        {
            if (string.IsNullOrEmpty(questID))
                return;

            SetQuestState(questID, QuestState.InProgress);
        }

        public void CompleteQuest(string questID)
        {
            if (string.IsNullOrEmpty(questID))
                return;

            SetQuestState(questID, QuestState.Completed);
        }

        public void ResetQuest(string questID)
        {
            if (string.IsNullOrEmpty(questID))
                return;

            SetQuestState(questID, QuestState.NotStarted);
        }

        private void SetQuestState(string questID, QuestState state)
        {
            if (string.IsNullOrEmpty(questID))
                return;

            if (questStateMap.TryGetValue(questID, out var currentState))
            {
                if (currentState == state)
                    return;

                questStateMap[questID] = state;
            }
            else
            {
                questStateMap.Add(questID, state);
            }

            Debug.Log($"[QuestManager] Quest State Changed : {questID} -> {state}");
            OnQuestStateChanged?.Invoke(questID, state);
        }
    }
}