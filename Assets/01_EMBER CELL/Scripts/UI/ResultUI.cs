// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;

// namespace TEC
// {
//     public class ResultUI : UIBase
//     {
//         [Header("Result UI")]
//         [SerializeField] private TextMeshProUGUI playtimeText; 
//         [SerializeField] private TextMeshProUGUI hitRateText; 
//         [SerializeField] private Button continueButton; 

//         private void Awake()
//         {
//             if (continueButton != null)
//             {
//                 continueButton.onClick.RemoveListener(OnClickContinue); 
//                 continueButton.onClick.AddListener(OnClickContinue); 
//             }

//             gameObject.SetActive(false); 
//         }

//         public void ShowResult(string playtime = "", string hitRate = "") 
//         {
//             if (playtimeText != null && string.IsNullOrEmpty(playtime) == false) 
//             {
//                 playtimeText.text = playtime; 
//             }

//             if (hitRateText != null && string.IsNullOrEmpty(hitRate) == false) 
//             {
//                 hitRateText.text = hitRate; 
//             }

//             UIManager.Hide<MainHUD>(UIList.MainHUD);
//             UIManager.Hide<CrossHairUI>(UIList.CrossHairUI);

//             Show();

//             if (InputManager.Singleton != null) 
//             {
//                 InputManager.Singleton.SetCursorForcedByUI(true, true); 
//                 InputManager.Singleton.SetCursorVisible(true); 
//             }
//         }

//         private void OnClickContinue() 
//         {
//             Hide();

//             Time.timeScale = 1f; 
//             Time.fixedDeltaTime = 0.02f; 

//             if (InputManager.Singleton != null) 
//             {
//                 InputManager.Singleton.SetCursorForcedByUI(false, false); 
//                 InputManager.Singleton.SetCursorVisible(false); 
//             }

//             Main.Singleton.ChangeScene(SceneType.Ingame); 
//         }

//         public override void Hide()
//         {
//             base.Hide();
//         }
//     }
// }

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace TEC
{
    public class ResultUI : UIBase
    {
        [Header("Result UI")]
        [SerializeField] private TextMeshProUGUI playtimeText; 
        [SerializeField] private TextMeshProUGUI hitRateText; 
        [SerializeField] private Button continueButton; 

        [Header("Effect")]
        [SerializeField] private CanvasGroup canvasGroup; 
        [SerializeField] private float showDuration = 0.4f; 

        private void Awake()
        {
            if (continueButton != null)
            {
                continueButton.onClick.RemoveListener(OnClickContinue); 
                continueButton.onClick.AddListener(OnClickContinue); 
            }

            gameObject.SetActive(false); 
        }

        public void ShowResult(string playtime = "", string hitRate = "") 
        {
            if (playtimeText != null && string.IsNullOrEmpty(playtime) == false) 
            {
                playtimeText.text = playtime; 
            }

            if (hitRateText != null && string.IsNullOrEmpty(hitRate) == false) 
            {
                hitRateText.text = hitRate; 
            }

            // 기존 UI 숨김
            UIManager.Hide<MainHUD>(UIList.MainHUD);
            UIManager.Hide<CrossHairUI>(UIList.CrossHairUI);

            var interactionUI = UIManager.Singleton.GetUI<InteractionUI>(UIList.InteractionUI);
            if (interactionUI != null)
            {
                interactionUI.ClearData();
                interactionUI.Hide();
            }

            var dialogueRoot = UIManager.Singleton.GetUI<DialogueRoot>(UIList.DialogueRoot);
            if (dialogueRoot != null)
            {
                dialogueRoot.Hide();
            }

            Show();

            // 초기 상태
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }

            transform.localScale = Vector3.one * 0.8f;

            StartCoroutine(ShowEffect());

            if (InputManager.Singleton != null) 
            {
                InputManager.Singleton.SetCursorForcedByUI(true, true); 
                InputManager.Singleton.SetCursorVisible(true); 
            }
        }

        private IEnumerator ShowEffect()
        {
            float time = 0f;

            while (time < showDuration)
            {
                time += Time.unscaledDeltaTime;
                float t = time / showDuration;

                if (canvasGroup != null)
                {
                    canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
                }

                transform.localScale = Vector3.Lerp(Vector3.one * 0.8f, Vector3.one, t);

                yield return null;
            }

            if (canvasGroup != null)
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }

        private void OnClickContinue() 
        {
            Hide();

            Time.timeScale = 1f; 
            Time.fixedDeltaTime = 0.02f; 

            if (InputManager.Singleton != null) 
            {
                InputManager.Singleton.SetCursorForcedByUI(false, false); 
                InputManager.Singleton.SetCursorVisible(false); 
            }

            Main.Singleton.ChangeScene(SceneType.Camp); //Ingame -> Camp
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}