using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TEC
{
    public class DialogueUI : UIBase
    {
        // 다른 스크립트에서 대화 중 여부 확인용
        public static bool IsDialogueOpen { get; private set; } = false;

        [Header("Dialogue UI")]
        [SerializeField] private TextMeshProUGUI speakerNameText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Image portraitImage;
        [SerializeField] private Button nextButton;

        [Header("TypeWriter Effect")] 
        [SerializeField] private float typeWriterInterval = 0.03f; 

        private NpcDialogueDataSO currentDialogueData;
        private int currentLineIndex = -1;
        private Action onDialogueFinished;

        private Coroutine typeWriterCoroutine; 
        private bool isTyping = false; 

        private void Awake()
        {
            if (nextButton != null)
            {
                nextButton.onClick.RemoveListener(OnClickNext);
                nextButton.onClick.AddListener(OnClickNext);
            }

            gameObject.SetActive(false); 
        }

        public void ShowDialogue(NpcDialogueDataSO dialogueData, Action onFinished = null)
        {
            if (dialogueData == null)
                return;

            if (dialogueData.lines == null || dialogueData.lines.Count == 0)
                return;

            currentDialogueData = dialogueData;
            currentLineIndex = 0;
            onDialogueFinished = onFinished;

            Show();

            IsDialogueOpen = true;

            RefreshCurrentLine();
        }

        public void OnClickNext()
        {
            if (currentDialogueData == null)
                return;

            if (isTyping) 
            {
                CompleteCurrentTyping();
                return;
            }

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

        private void RefreshCurrentLine()
        {
            if (currentDialogueData == null)
                return;

            if (currentLineIndex < 0 || currentLineIndex >= currentDialogueData.lines.Count)
                return;

            DialogueLineData line = currentDialogueData.lines[currentLineIndex];

            if (speakerNameText != null)
                speakerNameText.text = line.speakerName;

            if (portraitImage != null)
            {
                portraitImage.sprite = line.portrait;
                portraitImage.enabled = line.portrait != null;
            }

            if (typeWriterCoroutine != null)
            {
                StopCoroutine(typeWriterCoroutine);
                typeWriterCoroutine = null;
            }

            if (messageText != null)
                messageText.text = string.Empty;

            typeWriterCoroutine = StartCoroutine(TypeWriterRoutine(line.message)); 
        }

        private IEnumerator TypeWriterRoutine(string fullMessage)
        {
            isTyping = true;

            if (messageText == null)
            {
                isTyping = false;
                yield break;
            }

            if (string.IsNullOrEmpty(fullMessage))
            {
                messageText.text = string.Empty;
                isTyping = false;
                typeWriterCoroutine = null;
                yield break;
            }

            for (int i = 0; i < fullMessage.Length; i++)
            {
                messageText.text += fullMessage[i];
                yield return new WaitForSeconds(typeWriterInterval);
            }

            isTyping = false;
            typeWriterCoroutine = null;
        }

        private void CompleteCurrentTyping() 
        {
            if (currentDialogueData == null)
                return;

            if (currentLineIndex < 0 || currentLineIndex >= currentDialogueData.lines.Count)
                return;

            if (typeWriterCoroutine != null)
            {
                StopCoroutine(typeWriterCoroutine);
                typeWriterCoroutine = null;
            }

            DialogueLineData line = currentDialogueData.lines[currentLineIndex];

            if (messageText != null)
                messageText.text = line.message;

            isTyping = false;
        }

        public override void Hide()
        {
            if (typeWriterCoroutine != null) 
            {
                StopCoroutine(typeWriterCoroutine); 
                typeWriterCoroutine = null; 
            }

            isTyping = false; 

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

            IsDialogueOpen = false;
        }
    }
}