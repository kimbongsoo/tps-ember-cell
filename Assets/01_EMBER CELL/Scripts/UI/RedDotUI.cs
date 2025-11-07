using UnityEngine;
using UnityEngine.UI;

namespace TEC
{
    public class RedDotUI : UIBase
    {
        public static RedDotUI Instance => UIManager.Singleton.GetUI<RedDotUI>(UIList.RedDotUI);

        [SerializeField] private Image redDot;

        private void Awake()
        {
            if (redDot == null)
                redDot = GetComponentInChildren<Image>();

            gameObject.SetActive(false);
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}
