using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public class KeyLockedDoor : MonoBehaviour, IInteractionProvider
    {
        public List<IInteractionData> Interactions => interactions;

        [Header("Key Setting")]
        [SerializeField] private string requiredKeyGuid;

        [Header("UI")]
        [SerializeField] private Sprite actionIcon;
        [SerializeField] private string actionMessage = "문 열기";

        [Header("Door Setting")]
        [SerializeField] private GameObject doorObject;

        private readonly List<IInteractionData> interactions = new List<IInteractionData>();
        private InteractionKeyLockData interactionData;

        private bool isOpened = false;

        private void Awake()
        {
            interactionData = new InteractionKeyLockData("KEY_LOCK_DOOR", actionIcon, actionMessage);
            interactions.Add(interactionData);
        }

        public void Interact(IInteractionData data)
        {
            if (isOpened)
                return;

            if (data != interactionData)
                return;

            if (PlayerInventory.Instance == null)
                return;

            bool hasKey = PlayerInventory.Instance.Contains(requiredKeyGuid);
            if (!hasKey)
            {
                Debug.Log("열쇠가 없습니다.");
                return;
            }

            OpenDoor();
        }

        private void OpenDoor()
        {
            isOpened = true;

            if (doorObject != null)
                doorObject.SetActive(false);

            Debug.Log("문이 열렸습니다.");
        }
    }
}
