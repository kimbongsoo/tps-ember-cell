using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

namespace TEC
{
    [CreateAssetMenu(fileName = "InteractionSearchData", menuName = "KBS/Interaction/Search Data")]
    public class InteractionSearchData : ScriptableObject, IInteractionData
    {
        public string ID => dataId;
        public Sprite ActionIcon => dataIcon;
        public string ActionMessage => dataName;

        [Header("Search Data setting")]
        [SerializeField] private string dataId;
        [SerializeField] private Sprite dataIcon;
        [SerializeField] private string dataName;

    }
}
