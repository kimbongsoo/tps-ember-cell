// using System.Collections;
// using System.Collections.Generic;
// using System.Globalization;
// using System.Xml.Serialization;
// using Gpm.Ui;
// using TMPro;
// using Unity.VisualScripting;
// using UnityEngine;

// namespace KBS
// {
//     public class InteractionDataContext
//     {
//         public IInteractionData Data { get; }
//         public IInteractionProvider Provider { get; }
//         public string ID => Data.ID;
//         public bool ShouldRemoveAfterInteraction => Data is InteractionDropItemData || Data is InteractionItemBoxData;

//         public InteractionDataContext(IInteractionData data, IInteractionProvider provider)
//         {
//             Data = data;
//             Provider = provider;
//         }
//     }
//     public class InteractionUI : UIBase
//     {
//         [SerializeField] private InfiniteScroll infiniteScroll;

//         private List<InteractionDataContext> dataContexts = new();  //실제 각ㅏ의 Infinite Scroll Data 컨테이너
//         private Dictionary<string, InteractionUI_ListItemData> stackedUIMap = new(); // ID를 Key로해서, 중복 된 데이터를 묶어놓은 데이터 컨테이너
//         private int currentSelectionIndex = -1;

//         private void Awake()
//         {
//             infiniteScroll.itemPrefab.gameObject.SetActive(false);
//         }

//         private void Update()
//         {
//             float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
//             if (mouseWheel != 0f)
//             {
//                 MoveSelection(mouseWheel);
//             }
//         }

//         public void AddInteractionData(InteractionDataContext context)
//         {
//             dataContexts.Add(context);

//             string id = context.ID;
//             if (stackedUIMap.TryGetValue(id, out var existing))
//             {
//                 //똑같은 ID를 가진 데이터가 이미 존재하는 경우 처리
//                 existing.stackCount++;
//                 infiniteScroll.UpdateData(existing);
//             }
//             else
//             {
//                 var listData = new InteractionUI_ListItemData
//                 {
//                     id = id,
//                     icon = context.Data.ActionIcon,
//                     message = context.Data.ActionMessage,
//                     isSelected = dataContexts.Count == 1, //첫 번째 데이터는 선택 상태로 표시
//                 };

//                 stackedUIMap[id] = listData;
//                 infiniteScroll.InsertData(listData);

//                 if (currentSelectionIndex < 0)
//                     currentSelectionIndex = 0;
//             }
//         }

//         public void RemoveInteractionData(InteractionDataContext context)
//         {
//             dataContexts.Remove(context);

//             string id = context.ID;
//             if (stackedUIMap.TryGetValue(id, out var listData))
//             {
//                 listData.stackCount--;

//                 if (listData.stackCount <= 0)
//                 {
//                     stackedUIMap.Remove(id);
//                     infiniteScroll.RemoveData(listData);

//                     var list = infiniteScroll.GetDataList();
//                     if (list.Count == 0)
//                         currentSelectionIndex = -1;
//                     else if (currentSelectionIndex >= list.Count)
//                         currentSelectionIndex = list.Count - 1;
//                 }
//                 else
//                 {
//                     infiniteScroll.UpdateData(listData);
//                 }
//             }

//         }

//         public void ClearData()
//         {
//             dataContexts.Clear();
//             stackedUIMap.Clear();
//             infiniteScroll.ClearData(true);
//             currentSelectionIndex = -1;

//         }

//         public void MoveSelection(float direction)
//         {
//             var list = infiniteScroll.GetDataList();
//             if (list.Count == 0)
//                 return;
//             if (currentSelectionIndex >= 0 && currentSelectionIndex < list.Count)
//             {
//                 var prev = list[currentSelectionIndex] as InteractionUI_ListItemData;
//                 prev.isSelected = false;
//                 infiniteScroll.UpdateData(prev);
//             }

//             currentSelectionIndex += (direction < 0f) ? 1 : -1;

//             if (currentSelectionIndex < 0)
//                 currentSelectionIndex = list.Count - 1;
//             else if (currentSelectionIndex >= list.Count)
//                 currentSelectionIndex = 0;

//             var next = list[currentSelectionIndex] as InteractionUI_ListItemData;
//             next.isSelected = true;
//             infiniteScroll.UpdateData(next);
//             infiniteScroll.MoveToFromDataIndex(currentSelectionIndex, InfiniteScroll.MoveToType.MOVE_TO_CENTER, 0.1f);
//         }

//         public void TryInteract()
//         {
//             if (currentSelectionIndex < 0)
//                 return;

//             var list = infiniteScroll.GetDataList();
//             if (currentSelectionIndex >= list.Count)
//                 return;

//             var selected = list[currentSelectionIndex] as InteractionUI_ListItemData;
//             if (selected == null)
//                 return;

//             TrySelectById(selected.id);

//             list = infiniteScroll.GetDataList();
//             if (list.Count == 0)
//             {
//                 currentSelectionIndex = -1;
//             }
//             else if (currentSelectionIndex >= list.Count)
//             {
//                 currentSelectionIndex = list.Count - 1;
//             }

//             if (currentSelectionIndex >= 0 && currentSelectionIndex < list.Count)
//             {
//                 if (list[currentSelectionIndex] is InteractionUI_ListItemData newSelected)
//                 {
//                     // newSelected.isSelected = true;
//                     // infiniteScroll.UpdateData(newSelected);
//                 }
//             }
//         }

//         private void TrySelectById(string id)
//         {
//             var copiedList = new List<InteractionDataContext>(dataContexts);
//             foreach (var context in copiedList)
//             {
//                 if (context.ID == id)
//                 {
//                     context.Provider.Interact(context.Data);

//                     if (context.ShouldRemoveAfterInteraction)
//                     {
//                         RemoveInteractionData(context);
//                     }
//                 }
//             }
//         }
//     }
// }
