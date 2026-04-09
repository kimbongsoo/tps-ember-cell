//===============================
// ReadMe
// - 각 UIemfdml enum 형 이름은 prefab이름과 같아야 한다.
// - "enum Type name" and "prefab name" are same!
// = Popup 과 Panel을 구분하는 방법 :
//  > Game에서 Esc 키를 눌러 UI가 닫히는 경우에는 Popup으로,
//   그렇지 않은 경우에는 Panel로 구분한다
//===============================

namespace TEC
{
    public enum UIList
    {
        //===================
        // Popup UI
        //===================
        SCENE_POPUP,
        InventoryContextMenu,
        MAX_SCENE_POPUP,
        //===================

        //===================
        // Panel UI
        //===================
        SCENE_PANEL,

        LoadingUI,
        CrossHairUI,
        TitleUI,
        MainHUD,
        InteractionUI,
        RedDotUI,
        DialogueRoot,
        // DialogueUI,
        // QuestAcceptUI,

        //인벤토리
        InventoryRenewalUI,

        MAX_SCENE_PANEL,
        //===================

    }
}
