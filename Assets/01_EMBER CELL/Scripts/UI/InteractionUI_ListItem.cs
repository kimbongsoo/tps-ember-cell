using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TEC
{
    /// <summary>
    /// UI 리스트의 개별 아이템 (아이콘 + 텍스트)
    /// </summary>
    [System.Serializable]
    public class InteractionUI_ListItemData
    {
        public string id;
        public Sprite icon;
        public string message;
        public bool isSelected;
    }

    public class InteractionUI_ListItem : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private GameObject selection;

        public void SetData(InteractionUI_ListItemData data)
        {
            if (iconImage != null) iconImage.sprite = data.icon;
            if (messageText != null) messageText.text = data.message;
            if (selection != null) selection.SetActive(data.isSelected);
        }
    }
}
