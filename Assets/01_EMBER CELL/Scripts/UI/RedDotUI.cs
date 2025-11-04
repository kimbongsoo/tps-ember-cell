using UnityEngine;

namespace TEC
{
    public class RedDotUI : UIBase
    {
        public static RedDotUI Instance => UIManager.Singleton.GetUI<RedDotUI>(UIList.RedDotUI);

        [SerializeField] private GameObject dotContainer;

        private bool isVisible = false;

        private void Awake()
        {
            SetVisible(false);
        }

        public override void Show()
        {
            base.Show();
            SetVisible(true);
        }

        public override void Hide()
        {
            base.Hide();
            SetVisible(false);
        }

        public void Toggle()
        {
            SetVisible(!isVisible);
        }

        private void SetVisible(bool visible)
        {
            isVisible = visible;

            if (dotContainer != null)
                dotContainer.SetActive(visible);
        }
    }
}
