using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TEC
{
    public class MainHUD : UIBase
    {
        public static MainHUD Instance => UIManager.Singleton.GetUI<MainHUD>(UIList.MainHUD);

        [Header("Stat UI")]
        [SerializeField] private Image hpForeground;
        [SerializeField] private Image spForeground;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private TextMeshProUGUI spText;

        [Header("Weapon UI")]
        [SerializeField] private Image weaponIcon;
        [SerializeField] private TextMeshProUGUI weaponNameText;
        [SerializeField] private TextMeshProUGUI ammoText;

        private CharacterBase character;

        private void OnEnable()
        {
            StartCoroutine(WaitForCharacter());
        }

        private IEnumerator WaitForCharacter()
        {
            yield return null;

            character = FindObjectOfType<CharacterBase>();
            if (character == null)
            {
                Debug.LogWarning("[MainHUD] CharacterBase not found. HUD will remain hidden.");
                yield break;
            }

            character.OnchangedHP += SetHP;
            character.OnChangedSP += SetSP;
            character.onFireEvent += UpdateAmmo;
            character.onReloadCompleteEvent += UpdateAmmo;
            character.OnArmedStateChanged += HandleArmedState;
            character.OnDeadStateChanged += HandleDeadState;

            SetHP(character.CurrentHP, character.MaxHP);
            SetSP(character.CurrentSP, character.MaxSP);
            UpdateAmmo(character.PrimaryWeapon?.RemainAmmo ?? 0, character.PrimaryWeapon?.MaxAmmo ?? 0);
            HandleArmedState(character.IsArmed);
            HandleDeadState(character.IsDead);
        }

        private void OnDisable()
        {
            if (character == null) return;

            character.OnchangedHP -= SetHP;
            character.OnChangedSP -= SetSP;
            character.onFireEvent -= UpdateAmmo;
            character.onReloadCompleteEvent -= UpdateAmmo;
            character.OnArmedStateChanged -= HandleArmedState;
            character.OnDeadStateChanged -= HandleDeadState;
        }

        private void HandleArmedState(bool armed)
        {
            SetAmmoVisible(armed);
            if (armed && character.PrimaryWeapon != null)
                UpdateAmmo(character.PrimaryWeapon.RemainAmmo, character.PrimaryWeapon.MaxAmmo);
        }

        private void HandleDeadState(bool dead)
        {
            if (dead)
                SetAmmoVisible(false);
        }

        public void SetHP(float current, float max)
        {
            if (hpForeground != null)
                hpForeground.fillAmount = current / max;

            if (hpText != null)
                hpText.text = $"{current:00}/{max:00}";
        }

        public void SetSP(float current, float max)
        {
            if (spForeground != null)
                spForeground.fillAmount = current / max;

            if (spText != null)
                spText.text = $"{current:00}/{max:00}";
        }

        private void UpdateAmmo(int current, int max)
        {
            if (ammoText == null)
                return;

            string currentColor = current == 0 ? "red" : "white";
            ammoText.text = $"<color={currentColor}>{current:00}</color> / {max:00}";
        }


        public void SetWeaponData(Sprite weaponImage, string weaponName)
        {
            if (weaponIcon != null)
                weaponIcon.sprite = weaponImage;

            if (weaponNameText != null)
                weaponNameText.text = weaponName;
        }

        public void SetAmmoVisible(bool visible)
        {
            if (ammoText != null)
                ammoText.gameObject.SetActive(visible);
        }


        public override void Show()
        {
            base.Show();
            StartCoroutine(DelayedApplyCamera());
        }

        private IEnumerator DelayedApplyCamera()
        {
            yield return new WaitUntil(() => Camera.main != null);

            Canvas canvas = GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;
            canvas.planeDistance = 1;
        }
    }
}
