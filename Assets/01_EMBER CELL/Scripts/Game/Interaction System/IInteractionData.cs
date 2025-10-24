using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public interface IInteractionData
    {
        public string ID { get; }
        public Sprite ActionIcon { get; }
        public string ActionMessage { get; }
    }
}
