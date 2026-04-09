using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TEC
{
    public class NPCDialogueProvider : MonoBehaviour, IInteractionProvider
    {
        public static bool IsConversationSequenceOpen { get; private set; } = false;

        [Header("Interaction UI")]
        [SerializeField] private Sprite actionIcon;
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

        private QuestState currentQuestState = QuestState.NotStarted;

        private void Awake()
        {
            RefreshInteractionData();
        }
        private void Start()
        {
            UIManager.Singleton.GetUI<DialogueRoot>(UIList.DialogueRoot);

            if (DialogueRoot.Instance != null)
            {
                dialogueUI = DialogueRoot.Instance.DialogueUI;
                questAcceptUI = DialogueRoot.Instance.QuestAcceptUI;
            }

            QuestManager.Singleton.OnQuestStateChanged += OnQuestStateChanged;

            RefreshInteractionData();
        }

        private void OnDisable()
        {
            // 수정
            if (QuestManager.Singleton != null)
            {
                QuestManager.Singleton.OnQuestStateChanged -= OnQuestStateChanged;
            }
        }

        public void Interact(IInteractionData data)
        {
            if (data == null)
                return;

            if (dialogueData == null)
                return;

            // [CHANGED] 생성 보장
            UIManager.Singleton.GetUI<DialogueRoot>(UIList.DialogueRoot);

            if (DialogueRoot.Instance != null)
            {
                dialogueUI = DialogueRoot.Instance.DialogueUI;
            }

            if (dialogueUI == null)
                return;

            if (data.ID != "NPC_DIALOGUE")
                return;

            RefreshInteractionData();

            currentQuestState = GetCurrentQuestState();

            List<DialogueLineData> currentLines = GetCurrentDialogueLines(currentQuestState);
            if (currentLines == null || currentLines.Count == 0)
                return;

            IsConversationSequenceOpen = true;

            if (CameraSystem.Instance != null)
            {
                CameraSystem.Instance.EnterDialogueMode(dialogueCameraFollowPoint, dialogueCameraLookPoint);
            }

            // [CHANGED] Root 활성화
            DialogueRoot.Instance.Show();

            dialogueUI.ShowDialogue(CreateTempDialogue(currentLines), OnDialogueFinished);
        }

        private void OnDialogueFinished()
        {
            if (currentQuestState != QuestState.NotStarted)
            {
                EndConversationSequence();
                return;
            }

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

        private void OnAcceptDialogueFinished()
        {
            ExecuteAccept();
        }

        private void OnDeclineDialogueFinished()
        {
            ExecuteDecline();
        }

        private void ExecuteAccept()
        {
            Debug.Log($"[NPCDialogueProvider] Quest Accepted : {dialogueData.questID}");

            // 수정
            QuestManager.Singleton.StartQuest(dialogueData.questID);

            onQuestAccepted?.Invoke();

            RefreshInteractionData();

            if (CharacterPlayerController.Instance != null)
            {
                CharacterPlayerController.Instance.InteractionSensor.PulseManuallyNextFrame();
            }

            EndConversationSequence();
        }

        private void ExecuteDecline()
        {
            Debug.Log($"[NPCDialogueProvider] Quest Declined : {dialogueData.questID}");
            onQuestDeclined?.Invoke();

            RefreshInteractionData();

            EndConversationSequence();
        }

        private void EndConversationSequence()
        {
            IsConversationSequenceOpen = false;

            if (CameraSystem.Instance != null)
            {
                CameraSystem.Instance.ExitDialogueMode();
            }
        }

        private NPCDialogueDataSO CreateTempDialogue(List<DialogueLineData> lines)
        {
            var tempDialogueData = ScriptableObject.CreateInstance<NPCDialogueDataSO>();
            tempDialogueData.lines = lines;
            return tempDialogueData;
        }

        private QuestState GetCurrentQuestState()
        {
            if (dialogueData == null)
                return QuestState.NotStarted;

            if (string.IsNullOrEmpty(dialogueData.questID))
                return QuestState.NotStarted;

            // 수정
            return QuestManager.Singleton.GetQuestState(dialogueData.questID);
        }

        private List<DialogueLineData> GetCurrentDialogueLines(QuestState state)
        {
            if (dialogueData == null)
                return null;

            switch (state)
            {
                case QuestState.InProgress:
                    if (dialogueData.inProgressLines != null && dialogueData.inProgressLines.Count > 0)
                        return dialogueData.inProgressLines;
                    break;

                case QuestState.Completed:
                    if (dialogueData.completedLines != null && dialogueData.completedLines.Count > 0)
                        return dialogueData.completedLines;
                    break;
            }

            return dialogueData.lines;
        }

        private string GetCurrentActionMessage(QuestState state)
        {
            if (dialogueData == null)
                return actionMessage;

            switch (state)
            {
                case QuestState.InProgress:
                    if (string.IsNullOrEmpty(dialogueData.inProgressActionMessage) == false)
                        return dialogueData.inProgressActionMessage;
                    break;

                case QuestState.Completed:
                    if (string.IsNullOrEmpty(dialogueData.completedActionMessage) == false)
                        return dialogueData.completedActionMessage;
                    break;
            }

            if (string.IsNullOrEmpty(dialogueData.notStartedActionMessage) == false)
                return dialogueData.notStartedActionMessage;

            return actionMessage;
        }

        private void RefreshInteractionData()
        {
            interactions.Clear();

            QuestState state = GetCurrentQuestState();
            string currentActionMessage = GetCurrentActionMessage(state);

            interactions.Add(new NPCDialogueInteractionData("NPC_DIALOGUE", actionIcon, currentActionMessage));
        }

        private void OnQuestStateChanged(string questID, QuestState state)
        {
            if (dialogueData == null)
                return;

            if (dialogueData.questID != questID)
                return;

            RefreshInteractionData();

            if (CharacterPlayerController.Instance != null)
            {
                CharacterPlayerController.Instance.InteractionSensor.PulseManuallyNextFrame();
            }
        }
    }
}