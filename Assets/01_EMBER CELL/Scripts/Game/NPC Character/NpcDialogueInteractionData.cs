using UnityEngine;

namespace TEC
{
    // NPC용 상호작용 데이터
    public class NPCDialogueInteractionData : IInteractionData
    {
        public string ID => id;
        public Sprite ActionIcon => actionIcon;
        public string ActionMessage => actionMessage;

        private string id;
        private Sprite actionIcon;
        private string actionMessage;

        public NPCDialogueInteractionData(string id, Sprite actionIcon, string actionMessage)
        {
            this.id = id;
            this.actionIcon = actionIcon;
            this.actionMessage = actionMessage;
        }
    }
}