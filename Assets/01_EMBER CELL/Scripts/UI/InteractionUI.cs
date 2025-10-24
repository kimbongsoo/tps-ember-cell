using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    /// <summary>
    /// 주변 상호작용 가능한 오브젝트 UI 표시 및 선택
    /// </summary>
    public class InteractionDataContext
    {
        public IInteractionData Data { get; }
        public IInteractionProvider Provider { get; }
        public string ID => Data.ID;
        public bool ShouldRemoveAfterInteraction => true;

        public InteractionDataContext(IInteractionData data, IInteractionProvider provider)
        {
            Data = data;
            Provider = provider;
        }
    }

    public class InteractionUI : UIBase
    {
        [SerializeField] private Transform contentRoot;
        [SerializeField] private InteractionUI_ListItem listItemPrefab;

        private readonly List<InteractionDataContext> dataContexts = new();
        private readonly List<InteractionUI_ListItem> activeItems = new();
        private int currentSelectionIndex = -1;

        public void AddInteractionData(InteractionDataContext context)
        {
            dataContexts.Add(context);

            var itemData = new InteractionUI_ListItemData
            {
                id = context.ID,
                icon = context.Data.ActionIcon,
                message = context.Data.ActionMessage,
                isSelected = dataContexts.Count == 1
            };

            var newItem = Instantiate(listItemPrefab, contentRoot);
            newItem.SetData(itemData);
            activeItems.Add(newItem);

            if (currentSelectionIndex < 0)
                currentSelectionIndex = 0;
        }

        public void RemoveInteractionData(InteractionDataContext context)
        {
            int index = dataContexts.FindIndex(d => d.ID == context.ID);
            if (index >= 0 && index < activeItems.Count)
            {
                Destroy(activeItems[index].gameObject);
                activeItems.RemoveAt(index);
                dataContexts.RemoveAt(index);

                currentSelectionIndex = Mathf.Clamp(currentSelectionIndex - 1, 0, activeItems.Count - 1);
            }
        }

        public void ClearData()
        {
            foreach (var item in activeItems)
                Destroy(item.gameObject);
            activeItems.Clear();
            dataContexts.Clear();
            currentSelectionIndex = -1;
        }

        public void TryInteract()
        {
            if (currentSelectionIndex < 0 || currentSelectionIndex >= dataContexts.Count)
                return;

            var selected = dataContexts[currentSelectionIndex];
            selected.Provider.Interact(selected.Data);
            RemoveInteractionData(selected);
        }

        private void Update()
        {
            if (dataContexts.Count == 0) return;

            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                MoveSelection(-1);
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                MoveSelection(1);
        }

        private void MoveSelection(int direction)
        {
            if (dataContexts.Count == 0) return;

            int newIndex = Mathf.Clamp(currentSelectionIndex + direction, 0, dataContexts.Count - 1);
            if (newIndex == currentSelectionIndex) return;

            // 기존 선택 해제
            activeItems[currentSelectionIndex].SetData(new InteractionUI_ListItemData
            {
                id = dataContexts[currentSelectionIndex].ID,
                icon = dataContexts[currentSelectionIndex].Data.ActionIcon,
                message = dataContexts[currentSelectionIndex].Data.ActionMessage,
                isSelected = false
            });

            // 새 항목 선택
            activeItems[newIndex].SetData(new InteractionUI_ListItemData
            {
                id = dataContexts[newIndex].ID,
                icon = dataContexts[newIndex].Data.ActionIcon,
                message = dataContexts[newIndex].Data.ActionMessage,
                isSelected = true
            });

            currentSelectionIndex = newIndex;
        }
    }
}
