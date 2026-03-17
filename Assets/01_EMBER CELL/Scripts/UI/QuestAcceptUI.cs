// using System;
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;

// namespace TEC
// {
//     // [ADDED] 퀘스트 수락/거절 UI
//     public class QuestAcceptUI : UIBase
//     {
//         [Header("Quest Accept UI")]
//         [SerializeField] private TextMeshProUGUI questTitleText;
//         [SerializeField] private TextMeshProUGUI questDescriptionText;
//         [SerializeField] private Button acceptButton;
//         [SerializeField] private Button declineButton;

//         private Action onAccept;
//         private Action onDecline;

//         private void Awake()
//         {
//             if (acceptButton != null)
//             {
//                 acceptButton.onClick.RemoveListener(OnClickAccept);
//                 acceptButton.onClick.AddListener(OnClickAccept);
//             }

//             if (declineButton != null)
//             {
//                 declineButton.onClick.RemoveListener(OnClickDecline);
//                 declineButton.onClick.AddListener(OnClickDecline);
//             }

//             Hide();
//         }

//         // [ADDED] 외부에서 퀘스트 수락 UI 호출
//         public void ShowQuestAccept(string questTitle, string questDescription, Action onAccept, Action onDecline)
//         {
//             this.onAccept = onAccept;
//             this.onDecline = onDecline;

//             if (questTitleText != null)
//                 questTitleText.text = questTitle;

//             if (questDescriptionText != null)
//                 questDescriptionText.text = questDescription;

//             Show();
//         }

//         // [ADDED] 수락 버튼
//         public void OnClickAccept()
//         {
//             Action accept = onAccept;
//             Hide();
//             accept?.Invoke();
//         }

//         // [ADDED] 거절 버튼
//         public void OnClickDecline()
//         {
//             Action decline = onDecline;
//             Hide();
//             decline?.Invoke();
//         }

//         public override void Hide()
//         {
//             base.Hide();

//             onAccept = null;
//             onDecline = null;

//             if (questTitleText != null)
//                 questTitleText.text = string.Empty;

//             if (questDescriptionText != null)
//                 questDescriptionText.text = string.Empty;
//         }
//     }
// }

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

            // HUD 숨김 + 마우스 활성화
            IsQuestAcceptOpen = true;

            // if (MainHUD.Instance != null)
            //     MainHUD.Instance.SetDialogueMode(true);

            // Cursor.visible = true;
            // Cursor.lockState = CursorLockMode.None;
        }

        public void OnClickAccept()
        {
            Action accept = onAccept;
            Hide();
            accept?.Invoke();

            if (CameraSystem.Instance != null)
            {
                CameraSystem.Instance.ExitDialogueMode();
            }
        }

        public void OnClickDecline()
        {
            Action decline = onDecline;
            Hide();
            decline?.Invoke();

            if (CameraSystem.Instance != null)
            {
                CameraSystem.Instance.ExitDialogueMode();
            }
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

            // HUD 복구 + 마우스 원복
            IsQuestAcceptOpen = false;

            // if (MainHUD.Instance != null && DialogueUI.IsDialogueOpen == false)
            //     MainHUD.Instance.SetDialogueMode(false);

            // if (DialogueUI.IsDialogueOpen == false)
            // {
            //     Cursor.visible = false;
            //     Cursor.lockState = CursorLockMode.Locked;
            // }
        }
    }
}
