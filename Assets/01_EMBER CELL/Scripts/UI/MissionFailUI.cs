using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace TEC
{
    public class MissionFailUI : UIBase
    {
        [Header("Mission Fail UI")]
        [SerializeField] private TextMeshProUGUI playtimeText; 
        [SerializeField] private TextMeshProUGUI hitRateText; 
        [SerializeField] private Button retryButton;

        [Header("Effect")]
        [SerializeField] private CanvasGroup canvasGroup; 
        [SerializeField] private float showDuration = 0.4f; 

        private void Awake()
        {
            if (retryButton != null)
            {
                retryButton.onClick.RemoveListener(OnClickRetry); 
                retryButton.onClick.AddListener(OnClickRetry); 
            }

            gameObject.SetActive(false);
        }

        public void ShowMissionFail(string playtime = "", string hitRate = "") 
        {
            if (playtimeText != null && string.IsNullOrEmpty(playtime) == false)
            {
                playtimeText.text = playtime; 
            }

            if (hitRateText != null && string.IsNullOrEmpty(hitRate) == false)
            {
                hitRateText.text = hitRate; 
            }

            UIManager.Hide<MainHUD>(UIList.MainHUD); 
            UIManager.Hide<CrossHairUI>(UIList.CrossHairUI); 

            Show();

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

        private void OnClickRetry() 
        {
            Hide();

            Time.timeScale = 1f; 
            Time.fixedDeltaTime = 0.02f; 

            if (InputManager.Singleton != null)
            {
                InputManager.Singleton.SetCursorForcedByUI(false, false); 
                InputManager.Singleton.SetCursorVisible(false); 
            }

            Main.Singleton.ChangeScene(SceneType.IngameLevel); 
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}