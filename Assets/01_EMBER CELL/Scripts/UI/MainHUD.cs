using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace TEC
{
    public class MainHUD : UIBase
    {
        public static MainHUD Instance => UIManager.Singleton.GetUI<MainHUD>(UIList.MainHUD);

        [SerializeField] private Image hpForeground;
        [SerializeField] private Image spForeground;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private TextMeshProUGUI spText;

        [SerializeField] private Image weaponIcon;
        [SerializeField] private TextMeshProUGUI weaponNameText;
        [SerializeField] private TextMeshProUGUI ammoText;

        [Header("Minimap Compass")]
        [SerializeField] private RectTransform compassContainer;
        [SerializeField] private RectTransform northText;
        [SerializeField] private RectTransform southText;
        [SerializeField] private RectTransform eastText;
        [SerializeField] private RectTransform westText;

        [Header("Action UI")]
        [SerializeField] private GameObject actionUI;

        [Header("HP Test")]
        [SerializeField] private List<Image> hpSegmentFills = new(); // 10개

        //인디케이터 추가
        [Header("Indicator")]
        [SerializeField] private DamageDirectionIndicatorUI damageDirectionIndicator;

        public override void Show()
        {
            base.Show();
            StartCoroutine(DelayedApplyCamera());

            SetAmmoVisible(false);
        }

        private IEnumerator DelayedApplyCamera()
        {
            yield return new WaitUntil(() => Camera.main != null);

            Canvas canvas = GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;
            canvas.planeDistance = 1;
        }

        public void SetWeaponData(Sprite weaponImage, string weaponName)
        {
            weaponIcon.sprite = weaponImage;
            weaponNameText.text = weaponName;
        }


        public void SetAmmoText(int current, int max)
        {
            string currentColor = current == 0 ? "red" : "white";
            ammoText.text = $"<color={currentColor}>{current:00}</color> / {max:00}";
        }

        public void ToggleAmmoTextByArmedState(bool armed)
        {
            ammoText.gameObject.SetActive(armed);
        }

        public void ToggleAmmoTextByDeadState(bool dead)
        {
            if (dead)
            {
                ammoText.gameObject.SetActive(false);
            }
        }

        public void SetHP(float current, float max)
        {
            hpForeground.fillAmount = current / max;
            hpText.text = $"{current:00}/{max:00}";
        }
        
        // public void SetHP(float current, float max)
        // {
        //     current = Mathf.Clamp(current, 0f, max);

        //     const int segmentCount = 10;
        //     float hpPerSegment = max / segmentCount; // = 10

        //     for (int i = 0; i < hpSegmentFills.Count; i++)
        //     {
        //         float segmentMin = i * hpPerSegment;
        //         float segmentMax = (i + 1) * hpPerSegment;

        //         if (current >= segmentMax)
        //         {
        //             hpSegmentFills[i].fillAmount = 1f;
        //         }
        //         else if (current <= segmentMin)
        //         {
        //             hpSegmentFills[i].fillAmount = 0f;
        //         }
        //         else
        //         {
        //             float partial = (current - segmentMin) / hpPerSegment;
        //             hpSegmentFills[i].fillAmount = partial;
        //         }
        //     }
        // }


        public void SetSP(float current, float max)
        {
            spForeground.fillAmount = current / max;
            spText.text = $"{current:00}/{max:00}";
        }

        public void SetAmmoVisible(bool visible)
        {
            if (ammoText != null)
                ammoText.gameObject.SetActive(visible);
        }

        public void UpdateCompass(float playerYaw)
        {
            if (compassContainer == null)
                return;

            compassContainer.localRotation = Quaternion.Euler(0f, 0f, -playerYaw);

            Quaternion inverse = Quaternion.Euler(0f, 0f, playerYaw);

            // RectTransform 회전 보정
            ApplyTextRotation(northText, inverse);
            ApplyTextRotation(southText, inverse);
            ApplyTextRotation(eastText, inverse);
            ApplyTextRotation(westText, inverse);
        }

        private void ApplyTextRotation(RectTransform text, Quaternion rotation)
        {
            if (text == null) return;

            text.localRotation = rotation;
        }

        public void ToggleActionUI()
        {
            if(actionUI == null)
                return;

            actionUI.SetActive(!actionUI.activeSelf);
        }

        //인디케이터 추가
        public void ShowHitDirection(Transform player, Vector3 attackerPosition) // [CHANGED]
        {
            if (damageDirectionIndicator == null)
                return;

            damageDirectionIndicator.ShowFromAttackerPosition(player, attackerPosition);
        }
    }
}
