using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KBS
{
    public class LoadingUI : UIBase
    {
        public float LoadingPercentage
        {
            set
            {
                currentLoadingPercentage = Mathf.Clamp01(value);
                loadingBar.fillAmount = currentLoadingPercentage;
                percentageText.text = $"{currentLoadingPercentage * 100f:0} %";
                
            }
        }

        [SerializeField] private UnityEngine.UI.Image loadingBar;
        [SerializeField] private TMPro.TextMeshProUGUI percentageText;

        private float currentLoadingPercentage = 0f;
    }
}
