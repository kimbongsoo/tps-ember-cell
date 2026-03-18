using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TEC
{
    public class QuestAcceptUI : UIBase
    {
        // 다른 스크립트에서 퀘스트 UI 오픈 여부 확인용
        public static bool IsQuestAcceptOpen { get; private set; } = false;

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

            gameObject.SetActive(false);
        }

        public void ShowQuestAccept(string questTitle, string questDescription, Action onAccept, Action onDecline)
        {
            this.onAccept = onAccept;
            this.onDecline = onDecline;

            if (questTitleText != null)
                questTitleText.text = questTitle;

            if (questDescriptionText != null)
                questDescriptionText.text = questDescription;

            Show();

            IsQuestAcceptOpen = true;

        }

        public void OnClickAccept()
        {
            Action accept = onAccept;
            Hide();
            accept?.Invoke();
        }

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

            IsQuestAcceptOpen = false;
        }
    }
}
