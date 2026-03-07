using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TEC
{
    // [ADDED] 퀘스트 수락/거절 UI
    public class QuestAcceptUI : UIBase
    {
        [Header("Quest Accept UI")]
        [SerializeField] private TextMeshProUGUI questTitleText;
        [SerializeField] private TextMeshProUGUI questDescriptionText;
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button declineButton;

        private Action onAccept;
        private Action onDecline;

        private void Awake()
        {
            if (acceptButton != null)
            {
                acceptButton.onClick.RemoveListener(OnClickAccept);
                acceptButton.onClick.AddListener(OnClickAccept);
            }

            if (declineButton != null)
            {
                declineButton.onClick.RemoveListener(OnClickDecline);
                declineButton.onClick.AddListener(OnClickDecline);
            }

            Hide();
        }

        // [ADDED] 외부에서 퀘스트 수락 UI 호출
        public void ShowQuestAccept(string questTitle, string questDescription, Action onAccept, Action onDecline)
        {
            this.onAccept = onAccept;
            this.onDecline = onDecline;

            if (questTitleText != null)
                questTitleText.text = questTitle;

            if (questDescriptionText != null)
                questDescriptionText.text = questDescription;

            Show();
        }

        // [ADDED] 수락 버튼
        public void OnClickAccept()
        {
            Action accept = onAccept;
            Hide();
            accept?.Invoke();
        }

        // [ADDED] 거절 버튼
        public void OnClickDecline()
        {
            Action decline = onDecline;
            Hide();
            decline?.Invoke();
        }

        public override void Hide()
        {
            base.Hide();

            onAccept = null;
            onDecline = null;

            if (questTitleText != null)
                questTitleText.text = string.Empty;

            if (questDescriptionText != null)
                questDescriptionText.text = string.Empty;
        }
    }
}