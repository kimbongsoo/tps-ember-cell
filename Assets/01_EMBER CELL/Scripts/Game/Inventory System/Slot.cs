// using UnityEngine;
// using UnityEngine.UI;

// namespace TEC
// {
//     public class Slot : MonoBehaviour
//     {
//         public InteractionDropItemData item;
//         public Image itemIcon;

//         public void UpdateSlotUI()
//         {
//             itemIcon.sprite = item.ActionIcon;
//             itemIcon.gameObject.SetActive(true);
//         }

//         public void RemoveSlot()
//         {
//             item = null;
//             itemIcon.gameObject.SetActive(false);
//         }
//     }
// }
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
            if (itemIcon == null || item == null)
                return;

            itemIcon.sprite = item.ActionIcon;
            itemIcon.gameObject.SetActive(true);
        }

        public void RemoveSlot()
        {
            item = null;

            if (itemIcon == null)
                return;

            itemIcon.sprite = null;
            itemIcon.gameObject.SetActive(false);
        }
    }
}
