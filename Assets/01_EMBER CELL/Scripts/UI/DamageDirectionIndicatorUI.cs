using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TEC
{
    public class DamageDirectionIndicatorUI : MonoBehaviour
    {
        [SerializeField] private Image indicatorImage;
        [SerializeField] private float hideDelay = 0.15f;
        [SerializeField] private float fadeDuration = 0.35f;

        private Coroutine fadeRoutine = null;

        private void Awake()
        {
            if (indicatorImage != null)
            {
                SetAlpha(0f);
            }
        }

        public void ShowFromAttackerPosition(Transform player, Vector3 attackerPosition)
        {
            if (indicatorImage == null || player == null)
                return;

            Vector3 toAttacker = attackerPosition - player.position;
            toAttacker.y = 0f;

            if (toAttacker.sqrMagnitude <= 0.0001f)
                return;

            Vector3 localDir = player.InverseTransformDirection(toAttacker.normalized);

            float angle = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;

            transform.localRotation = Quaternion.Euler(0f, 0f, -angle);

            if (fadeRoutine != null)
                StopCoroutine(fadeRoutine);

            fadeRoutine = StartCoroutine(FadeRoutine());
        }

        public void HideIndicator()
        {
            if (fadeRoutine != null)
            {
                StopCoroutine(fadeRoutine);
                fadeRoutine = null;
            }

            SetAlpha(0f);
        }

        private IEnumerator FadeRoutine()
        {
            SetAlpha(1f);

            if (hideDelay > 0f)
                yield return new WaitForSeconds(hideDelay);

            float t = 0f;
            float start = 1f;

            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                float a = Mathf.Lerp(start, 0f, t / fadeDuration);
                SetAlpha(a);
                yield return null;
            }

            SetAlpha(0f);
            fadeRoutine = null;
        }

        private void SetAlpha(float alpha)
        {
            if (indicatorImage == null)
                return;

            Color c = indicatorImage.color;
            c.a = alpha;
            indicatorImage.color = c;
        }
    }
}
