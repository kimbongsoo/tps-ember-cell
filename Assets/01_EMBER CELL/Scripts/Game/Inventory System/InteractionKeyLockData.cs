using UnityEngine;

namespace TEC
{
    public class InteractionKeyLockData : IInteractionData
    {
        public string ID => id;
        public Sprite ActionIcon => icon;
        public string ActionMessage => message;

        private readonly string id;
        private readonly Sprite icon;
        private readonly string message;

        public InteractionKeyLockData(string id, Sprite icon, string message)
        {
            this.id = id;
            this.icon = icon;
            this.message = message;
        }
    }
}
