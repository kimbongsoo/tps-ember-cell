using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TEC
{
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
            interactions = new List<IInteractionData>()
            {
                new NPCDialogueInteractionData("NPC_DIALOGUE", actionIcon, actionMessage)
            };
        }

        // [ADDED] 런타임 자동 연결
        private void Start()
        {
            // MainHUD가 아직 생성 안 됐을 수 있으므로 먼저 생성 보장
            UIManager.Singleton.GetUI<MainHUD>(UIList.MainHUD);

            if (dialogueUI == null)
                dialogueUI = FindObjectOfType<DialogueUI>(true);

            if (questAcceptUI == null)
                questAcceptUI = FindObjectOfType<QuestAcceptUI>(true);
        }

        public void Interact(IInteractionData data)
        {
            if (data == null)
                return;

            if (dialogueData == null)
                return;

            // [CHANGED] 직접 연결 실패 대비 런타임 재탐색
            if (dialogueUI == null)
                dialogueUI = FindObjectOfType<DialogueUI>(true);

            if (dialogueUI == null)
                return;

            if (data.ID != "NPC_DIALOGUE")
                return;

            dialogueUI.ShowDialogue(dialogueData, OnDialogueFinished);
        }

        private void OnDialogueFinished()
        {
            if (dialogueData == null)
                return;

            if (dialogueData.showQuestAcceptUIAfterDialogue == false)
                return;

            // [CHANGED] 직접 연결 실패 대비 런타임 재탐색
            if (questAcceptUI == null)
                questAcceptUI = FindObjectOfType<QuestAcceptUI>(true);

            if (questAcceptUI == null)
                return;

            questAcceptUI.ShowQuestAccept(
                dialogueData.questTitle,
                dialogueData.questDescription,
                OnAcceptQuest,
                OnDeclineQuest
            );
        }

        private void OnAcceptQuest()
        {
            Debug.Log($"[NPCDialogueProvider] Quest Accepted : {dialogueData.questID}");
            onQuestAccepted?.Invoke();
        }

        private void OnDeclineQuest()
        {
            Debug.Log($"[NPCDialogueProvider] Quest Declined : {dialogueData.questID}");
            onQuestDeclined?.Invoke();
        }
    }
}