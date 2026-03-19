using System;
using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    [Serializable]
    public class DialogueLineData
    {
        public string speakerName;
        [TextArea(2, 5)]
        public string message;
        public Sprite portrait;
    }

    // NPC 대화 데이터 SO
    [CreateAssetMenu(fileName = "NPCDialogueDataSO", menuName = "PROJECT TEC/NPC/Dialogue Data")]
    public class NPCDialogueDataSO : ScriptableObject
    {
        [Header("Dialogue Info")]
        public string dialogueID;
        public string npcName;

        [Header("Not Started Dialogue Lines")]
        public List<DialogueLineData> lines = new();

        // 추가
        [Header("Accept Dialogue Lines")]
        public List<DialogueLineData> acceptLines = new();

        // 추가
        [Header("Decline Dialogue Lines")]
        public List<DialogueLineData> declineLines = new();

        // 추가
        [Header("In Progress Dialogue Lines")]
        public List<DialogueLineData> inProgressLines = new();

        // 추가
        [Header("Completed Dialogue Lines")]
        public List<DialogueLineData> completedLines = new();

        // 추가
        [Header("Interaction Message By State")]
        public string notStartedActionMessage = "NotStarted";
        public string inProgressActionMessage = "Inprogress";
        public string completedActionMessage = "Complete";

        [Header("Quest Offer")]
        public bool showQuestAcceptUIAfterDialogue = false;
        public string questID;
        public string questTitle;
        [TextArea(2, 5)]
        public string questDescription;
    }
}