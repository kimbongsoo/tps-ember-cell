using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public class CrossHairUI : UIBase
    {
        public static CrossHairUI Instance => UIManager.Singleton.GetUI<CrossHairUI>(UIList.CrossHairUI);
        [SerializeField] private RectTransform crosshairTop;
        [SerializeField] private RectTransform crosshairBottom;
        [SerializeField] private RectTransform crosshairLeft;
        [SerializeField] private RectTransform crosshairRight;

        public float maxSpread = 300f;

        public void ToggleCrosshairByArmedState(bool armed)
        {
            SetVisible(armed);
        }

        public void ToggleCrosshairByDeadState(bool dead)
        {
            if (dead)
            {
                SetVisible(false);
            }
        }

        private void SetVisible(bool visible)
        {
            if (crosshairTop) crosshairTop.gameObject.SetActive(visible);
            if (crosshairBottom) crosshairBottom.gameObject.SetActive(visible);
            if (crosshairLeft) crosshairLeft.gameObject.SetActive(visible);
            if (crosshairRight) crosshairRight.gameObject.SetActive(visible);
        }

        public void SetCrosshairSpread(float spread)
        {
            float spreadValue = Mathf.Clamp01(spread) * maxSpread;

            crosshairTop.anchoredPosition = new Vector2(0, spreadValue);
            crosshairBottom.anchoredPosition = new Vector2(0, -spreadValue);
            crosshairLeft.anchoredPosition = new Vector2(-spreadValue, 0);
            crosshairRight.anchoredPosition = new Vector2(spreadValue, 0);
        }
    }
}