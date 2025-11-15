using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    [CreateAssetMenu(fileName = "InteractionItemBoxData", menuName = "TEC/Interaction/Item Box Data")]
    public class InteractionItemBoxData : ScriptableObject, IInteractionData
    {
        public string ID => boxId;
        public Sprite ActionIcon => boxIcon;
        public string ActionMessage => boxName;
        public List<InteractionDropItemData> DropItems => dropItems;

        [Header("Box Setting")]
        [SerializeField] private string boxId;
        [SerializeField] private Sprite boxIcon;
        [SerializeField] private string boxName;

        [Header("Drop Items")]
        [SerializeField] private List<InteractionDropItemData> dropItems = new();

    }
}
