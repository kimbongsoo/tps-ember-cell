using UnityEngine;
using UnityEngine.UI;

namespace TEC
{
    public class Slot : MonoBehaviour
    {
        public InteractionDropItemData item;
        public Image itemIcon;

        public void UpdateSlotUI()
        {
            Debug.Log(
                $"[Slot.UpdateSlotUI] slot={name} " +
                $"item={(item == null ? "null" : item.name)} " +
                $"iconSprite={(item != null && item.ActionIcon != null ? item.ActionIcon.name : "null")} " +
                $"imageRef={(itemIcon == null ? "null" : itemIcon.name)}",
                this
            );
            if (itemIcon == null || item == null)
            {
                return;
            }
            itemIcon.sprite = item.ActionIcon;
            itemIcon.gameObject.SetActive(true);

        }

        public void RemoveSlot()
        {
            item = null;

            if (itemIcon == null)
            {
                return;
            }

            itemIcon.sprite = null;
            itemIcon.gameObject.SetActive(false);
        }
    }
}
