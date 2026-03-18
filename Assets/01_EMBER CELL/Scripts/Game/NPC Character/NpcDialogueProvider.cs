using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TEC
{
    public class NPCDialogueProvider : MonoBehaviour, IInteractionProvider
    {
        // 추가
        public static bool IsConversationSequenceOpen { get; private set; } = false;

        [Header("Interaction UI")]
        [SerializeField] private Sprite actionIcon;
        // [SerializeField] private string actionMessage = "대화하기"
        [SerializeField] private string actionMessage;

        [Header("Dialogue Data")]
        [SerializeField] private NPCDialogueDataSO dialogueData;

        [Header("Dialogue Camera")]
        [SerializeField] private Transform dialogueCameraFollowPoint;
        [SerializeField] private Transform dialogueCameraLookPoint;

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

        private void Start()
        {
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

            if (dialogueUI == null)
                dialogueUI = FindObjectOfType<DialogueUI>(true);

            if (dialogueUI == null)
                return;

            if (data.ID != "NPC_DIALOGUE")
                return;

            // 추가
            IsConversationSequenceOpen = true;

            if (CameraSystem.Instance != null)
            {
                CameraSystem.Instance.EnterDialogueMode(dialogueCameraFollowPoint, dialogueCameraLookPoint);
            }

            dialogueUI.ShowDialogue(dialogueData, OnDialogueFinished);
        }

        private void OnDialogueFinished()
        {
            if (dialogueData == null)
            {
                EndConversationSequence();
                return;
            }

            if (dialogueData.showQuestAcceptUIAfterDialogue == false)
            {
                EndConversationSequence();
                return;
            }

            if (questAcceptUI == null)
                questAcceptUI = FindObjectOfType<QuestAcceptUI>(true);

            if (questAcceptUI == null)
            {
                EndConversationSequence();
                return;
            }

            questAcceptUI.ShowQuestAccept(
                dialogueData.questTitle,
                dialogueData.questDescription,
                OnAcceptQuest,
                OnDeclineQuest
            );
        }

        private void OnAcceptQuest()
        {
            // 추가
            if (CameraSystem.Instance != null)
            {
                CameraSystem.Instance.EnterDialogueMode(dialogueCameraFollowPoint, dialogueCameraLookPoint);
            }

            if (dialogueData != null && dialogueData.acceptLines != null && dialogueData.acceptLines.Count > 0)
            {
                dialogueUI.ShowDialogue(CreateTempDialogue(dialogueData.acceptLines), OnAcceptDialogueFinished);
                return;
            }

            ExecuteAccept();
        }

        private void OnDeclineQuest()
        {
            // 추가
            if (CameraSystem.Instance != null)
            {
                CameraSystem.Instance.EnterDialogueMode(dialogueCameraFollowPoint, dialogueCameraLookPoint);
            }

            if (dialogueData != null && dialogueData.declineLines != null && dialogueData.declineLines.Count > 0)
            {
                dialogueUI.ShowDialogue(CreateTempDialogue(dialogueData.declineLines), OnDeclineDialogueFinished);
                return;
            }

            ExecuteDecline();
        }

        // 추가
        private void OnAcceptDialogueFinished()
        {
            ExecuteAccept();
        }

        // 추가
        private void OnDeclineDialogueFinished()
        {
            ExecuteDecline();
        }

        // 추가
        private void ExecuteAccept()
        {
            Debug.Log($"[NPCDialogueProvider] Quest Accepted : {dialogueData.questID}");
            onQuestAccepted?.Invoke();
            EndConversationSequence();
        }

        // 추가
        private void ExecuteDecline()
        {
            Debug.Log($"[NPCDialogueProvider] Quest Declined : {dialogueData.questID}");
            onQuestDeclined?.Invoke();
            EndConversationSequence();
        }

        // 추가
        private void EndConversationSequence()
        {
            IsConversationSequenceOpen = false;

            if (CameraSystem.Instance != null)
            {
                CameraSystem.Instance.ExitDialogueMode();
            }
        }

        // 추가
        private NPCDialogueDataSO CreateTempDialogue(List<DialogueLineData> lines)
        {
            var tempDialogueData = ScriptableObject.CreateInstance<NPCDialogueDataSO>();
            tempDialogueData.lines = lines;
            return tempDialogueData;
        }
    }
}