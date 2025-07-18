using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KBS
{
    public class UIManager : SingletonBase<UIManager>
    {
        public static T Show<T>(UIList uiName) where T : UIBase
        {
            var newUI = Singleton.GetUI<T>(uiName);
            if (newUI == null)
                return null;

            newUI.Show();

            return newUI;
        }

        public static T Hide<T>(UIList uiName) where T : UIBase
        {
            var targetUI = Singleton.GetUI<T>(uiName);
            if (targetUI == null)
                return null;

            targetUI.Hide();

            return targetUI;
        }

        [SerializeField] private Transform panelRoot;
        [SerializeField] private Transform popupRoot;

        private Dictionary<UIList, UIBase> panels = new Dictionary<UIList, UIBase>();
        private Dictionary<UIList, UIBase> popups = new Dictionary<UIList, UIBase>();

        private const string UI_PATH = "UI/Prefabs";
        private bool isInitialized = false;

        public void Initialize()
        {

            if (panelRoot == null)
            {
                GameObject goPanelRoot = new GameObject("Panel Root");
                panelRoot = goPanelRoot.transform;
                panelRoot.SetParent(transform);
                panelRoot.localPosition = Vector3.zero;
                panelRoot.localRotation = Quaternion.identity;
                panelRoot.localScale = Vector3.one;
            }

            if (popupRoot == null)
            {
                GameObject goPopupRoot = new GameObject("Popup Root");
                popupRoot = goPopupRoot.transform;
                popupRoot.SetParent(transform);
                popupRoot.localPosition = Vector3.zero;
                popupRoot.localRotation = Quaternion.identity;
                popupRoot.localScale = Vector3.one;
            }

            for (int index = (int)UIList.SCENE_POPUP + 1; index < (int)UIList.MAX_SCENE_POPUP; index++)
            {
                UIList uiName = (UIList)index;
                popups.Add(uiName, null);
            }

            for (int index = (int)UIList.SCENE_PANEL + 1; index < (int)UIList.MAX_SCENE_PANEL; index++)
            {
                UIList uiName = (UIList)index;
                panels.Add(uiName, null);
            }

            isInitialized = true;

        }

        public bool GetUI<T>(UIList uiName, out T ui, bool isReload = false) where T : UIBase
        {
            ui = GetUI<T>(uiName, isReload);
            return ui != null;
        }

        public T GetUI<T>(UIList uiName, bool isReload = false) where T : UIBase
        {
            Dictionary<UIList, UIBase> targetContainer = null;
            targetContainer = uiName > UIList.SCENE_POPUP && uiName < UIList.MAX_SCENE_POPUP ? popups : panels;
            Transform root = uiName > UIList.SCENE_POPUP && uiName < UIList.MAX_SCENE_POPUP ? popupRoot : panelRoot;

            if (!targetContainer.ContainsKey(uiName))
                return null;

            if (isReload && targetContainer[uiName])
            {
                Destroy(targetContainer[uiName].gameObject);
                targetContainer[uiName] = null;
            }

            if (!targetContainer[uiName])
            {
                string path = $"UI/Prefabs/UI.{uiName}";
                GameObject loadedUI = Resources.Load<GameObject>(path);

                if (!loadedUI)
                    return null;

                var newInstanceUI = Instantiate(loadedUI, root);
                if (newInstanceUI.TryGetComponent(out T component))
                {
                    newInstanceUI.gameObject.SetActive(false);
                    targetContainer[uiName] = component;
                }
            }

            return targetContainer[uiName] as T;
        }
    }
}
