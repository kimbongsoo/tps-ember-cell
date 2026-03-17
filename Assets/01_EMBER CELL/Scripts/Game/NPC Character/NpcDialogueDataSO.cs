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

        [Header("Dialogue Lines")]
        public List<DialogueLineData> lines = new();

        [Header("Quest Offer")]
        public bool showQuestAcceptUIAfterDialogue = false;
        public string questID;
        public string questTitle;
        [TextArea(2, 5)]
        public string questDescription;
    }
}