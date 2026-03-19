using UnityEngine;

namespace TEC
{
    public class DoorInteractionData : IInteractionData
    {
        public string ID => id;
        public Sprite ActionIcon => actionIcon;
        public string ActionMessage => actionMessage;

        private string id;
        private Sprite actionIcon;
        private string actionMessage;

        public DoorInteractionData(string id, Sprite actionIcon, string actionMessage)
        {
            this.id = id;
            this.actionIcon = actionIcon;
            this.actionMessage = actionMessage;
        }
    }
}