using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TEC
{
    // [ADDED] NPC 상호작용 Provider
    // 주의: Collider가 붙은 같은 GameObject에 이 스크립트가 같이 있어야
    // InteractionSensor의 other.GetComponent<IInteractionProvider>() 에 감지됩니다.
    public class NPCDialogueProvider : MonoBehaviour, IInteractionProvider
    {
        [Header("Interaction UI")]
        [SerializeField] private Sprite actionIcon;
        [SerializeField] private string actionMessage = "대화하기";

        [Header("Dialogue Data")]
        [SerializeField] private NPCDialogueDataSO dialogueData;

        [Header("UI References")]
        [SerializeField] private DialogueUI dialogueUI;
        [SerializeField] private QuestAcceptUI questAcceptUI;

        [Header("Quest Events")]
        [SerializeField] private UnityEvent onQuestAccepted;
        [SerializeField] private UnityEvent onQuestDeclined;

        private List<IInteractionData> interactions = new();

        public List<IInteractionData> Interactions => interactions;

        private void Awake()
        {
            // [ADDED] NPC는 "대화하기" 1개의 상호작용만 제공
            interactions = new List<IInteractionData>()
            {
                new NPCDialogueInteractionData("NPC_DIALOGUE", actionIcon, actionMessage)
            };
        }

        public void Interact(IInteractionData data)
        {
            if (data == null)
                return;

            if (dialogueData == null)
                return;

            if (dialogueUI == null)
                return;

            if (data.ID != "NPC_DIALOGUE")
                return;

            // [ADDED] 대화 시작
            dialogueUI.ShowDialogue(dialogueData, OnDialogueFinished);
        }

        // [ADDED] 대화 종료 후 퀘스트 UI 분기
        private void OnDialogueFinished()
        {
            if (dialogueData == null)
                return;

            if (dialogueData.showQuestAcceptUIAfterDialogue == false)
                return;

            if (questAcceptUI == null)
                return;

            questAcceptUI.ShowQuestAccept(
                dialogueData.questTitle,
                dialogueData.questDescription,
                OnAcceptQuest,
                OnDeclineQuest
            );
        }

        // [ADDED] 수락 처리
        private void OnAcceptQuest()
        {
            Debug.Log($"[NPCDialogueProvider] Quest Accepted : {dialogueData.questID}");
            onQuestAccepted?.Invoke();
        }

        // [ADDED] 거절 처리
        private void OnDeclineQuest()
        {
            Debug.Log($"[NPCDialogueProvider] Quest Declined : {dialogueData.questID}");
            onQuestDeclined?.Invoke();
        }
    }
}