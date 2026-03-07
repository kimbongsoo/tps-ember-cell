using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TEC
{
    // [ADDED] 대화 진행 UI
    public class DialogueUI : UIBase
    {
        [Header("Dialogue UI")]
        [SerializeField] private TextMeshProUGUI speakerNameText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Image portraitImage;
        [SerializeField] private Button nextButton;

        private NPCDialogueDataSO currentDialogueData;
        private int currentLineIndex = -1;
        private Action onDialogueFinished;

        private void Awake()
        {
            // [ADDED] 버튼이 있으면 다음 대사 진행 연결
            if (nextButton != null)
            {
                nextButton.onClick.RemoveListener(OnClickNext);
                nextButton.onClick.AddListener(OnClickNext);
            }

            Hide();
        }

        // [ADDED] 외부에서 대화 시작
        public void ShowDialogue(NPCDialogueDataSO dialogueData, Action onFinished = null)
        {
            if (dialogueData == null)
                return;

            if (dialogueData.lines == null || dialogueData.lines.Count == 0)
                return;

            currentDialogueData = dialogueData;
            currentLineIndex = 0;
            onDialogueFinished = onFinished;

            Show();
            RefreshCurrentLine();
        }

        // [ADDED] 다음 버튼 클릭
        public void OnClickNext()
        {
            if (currentDialogueData == null)
                return;

            currentLineIndex++;

            if (currentLineIndex >= currentDialogueData.lines.Count)
            {
                Action finished = onDialogueFinished;
                Hide();
                finished?.Invoke();
                return;
            }

            RefreshCurrentLine();
        }

        // [ADDED] 현재 대사 UI 반영
        private void RefreshCurrentLine()
        {
            if (currentDialogueData == null)
                return;

            if (currentLineIndex < 0 || currentLineIndex >= currentDialogueData.lines.Count)
                return;

            DialogueLineData line = currentDialogueData.lines[currentLineIndex];

            if (speakerNameText != null)
                speakerNameText.text = line.speakerName;

            if (messageText != null)
                messageText.text = line.message;

            if (portraitImage != null)
            {
                portraitImage.sprite = line.portrait;
                portraitImage.enabled = line.portrait != null;
            }
        }

        public override void Hide()
        {
            base.Hide();

            currentDialogueData = null;
            currentLineIndex = -1;
            onDialogueFinished = null;

            if (speakerNameText != null)
                speakerNameText.text = string.Empty;

            if (messageText != null)
                messageText.text = string.Empty;

            if (portraitImage != null)
            {
                portraitImage.sprite = null;
                portraitImage.enabled = false;
            }
        }
    }
}