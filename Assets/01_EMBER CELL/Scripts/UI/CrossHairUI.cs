using System.Collections;
using UnityEngine;

namespace TEC
{
    public class CrossHairUI : UIBase
    {
        private CharacterBase character;

        public static CrossHairUI Instance => UIManager.Singleton.GetUI<CrossHairUI>(UIList.CrossHairUI);

        [SerializeField] private RectTransform crosshairTop;
        [SerializeField] private RectTransform crosshairBottom;
        [SerializeField] private RectTransform crosshairLeft;
        [SerializeField] private RectTransform crosshairRight;

        [Header("Spread Settings")]
        public float maxSpread = 300f;              
        public float spreadSpeed = 0.1f;            
        public float recoverySpeed = 0.2f;         
        public float spreadMin = 0.1f;              
        public float spreadMax = 1f;                
        private float currentSpread = 0f;

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
                SetVisible(false);
                yield break;
            }

         
            character.OnArmedStateChanged += HandleArmedState;
            character.OnDeadStateChanged += HandleDeadState;
            character.onFireEvent += OnFireSpread;              
            character.onReloadCompleteEvent += OnReloadReset;   

            HandleArmedState(character.IsArmed);
            HandleDeadState(character.IsDead);
        }

        private void OnDisable()
        {
            if (character == null) return;

            character.OnArmedStateChanged -= HandleArmedState;
            character.OnDeadStateChanged -= HandleDeadState;
            character.onFireEvent -= OnFireSpread;
            character.onReloadCompleteEvent -= OnReloadReset;
        }

        private void HandleArmedState(bool armed)
        {
            SetVisible(armed);
            if (!armed)
            {
                currentSpread = spreadMin;
                SetCrosshairSpread(0f);
            }
        }

        private void HandleDeadState(bool dead)
        {
            if (dead)
            {
                SetVisible(false);
                currentSpread = spreadMin;
            }
        }

        private void OnFireSpread(int current, int max)
        {
            if (!gameObject.activeInHierarchy) return;

            currentSpread = Mathf.Clamp(currentSpread + spreadSpeed, spreadMin, spreadMax);
            SetCrosshairSpread(currentSpread / spreadMax);
        }

        private void OnReloadReset(int current, int max)
        {
            currentSpread = spreadMin;
            SetCrosshairSpread(currentSpread / spreadMax);
        }

        private void Update()
        {
            if (character == null || !character.IsArmed || character.IsDead)
                return;

            currentSpread = Mathf.Clamp(
                currentSpread - (recoverySpeed * Time.deltaTime),
                spreadMin,
                spreadMax
            );

            SetCrosshairSpread(currentSpread / spreadMax);
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

            if (crosshairTop) crosshairTop.anchoredPosition = new Vector2(0, spreadValue);
            if (crosshairBottom) crosshairBottom.anchoredPosition = new Vector2(0, -spreadValue);
            if (crosshairLeft) crosshairLeft.anchoredPosition = new Vector2(-spreadValue, 0);
            if (crosshairRight) crosshairRight.anchoredPosition = new Vector2(spreadValue, 0);
        }
    }
}
