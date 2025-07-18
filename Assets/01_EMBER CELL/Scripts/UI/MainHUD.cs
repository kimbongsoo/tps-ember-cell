using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

        [SerializeField] private TextMeshProUGUI scoreText;

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

        public void SetHP(float current, float max)
        {
            hpForeground.fillAmount = current / max;
            hpText.text = $"{current:00}/{max:00}";
        }

        public void SetSP(float current, float max)
        {
            spForeground.fillAmount = current / max;
            spText.text = $"{current:00}/{max:00}";
        }

        public void SetScore(int score)
        {
            scoreText.text = $"Score:{score}";
        }


        
    }
}
