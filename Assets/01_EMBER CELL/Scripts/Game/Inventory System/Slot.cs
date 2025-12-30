using UnityEngine;
using UnityEngine.UI;

namespace TEC
{
    public class Slot : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Button slotButton;

        private void Awake()
        {
            if (slotButton == null)
                slotButton = GetComponent<Button>();
        }

        public void SetInteractable(bool interactable)
        {
            if (slotButton == null)
                return;

            slotButton.interactable = interactable;
        }
    }
}
