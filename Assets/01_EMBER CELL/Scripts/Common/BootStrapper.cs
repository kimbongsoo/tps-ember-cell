#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace TEC
{
    public class BootStrapper
    {
        private const string BootStrapperMenuPath = "PROJECT KBS/BootStrapper/Activate BootStrapper";

        private static bool IsActiveBootStrapper
        {
            get => UnityEditor.EditorPrefs.GetBool(BootStrapperMenuPath, false);
            set
            {
                UnityEditor.EditorPrefs.SetBool(BootStrapperMenuPath, value);
                UnityEditor.Menu.SetChecked(BootStrapperMenuPath, value);
            }
        }


        [UnityEditor.MenuItem(BootStrapperMenuPath, false)]

        private static void ActivateBootStrapper()
        {
            IsActiveBootStrapper = !IsActiveBootStrapper;
            UnityEditor.Menu.SetChecked(BootStrapperMenuPath, IsActiveBootStrapper);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]

        public static void SystemBoot()
        {
            Scene activeScene = EditorSceneManager.GetActiveScene();
            if (IsActiveBootStrapper && false == activeScene.name.Equals("Main"))
            {
                InternalBoot();
            }

            //만약 인게임 씬에서 바로 실행한 상황이라면? 인게임 관련 UI가 나오도록 예외처리 추가
            if (activeScene.name.Equals("Ingame"))
            {
                // UIManager.Show<MainHUD>(UIList.MainHUD);
                // UIManager.Show<CrossHairUI>(UIList.CrossHairUI);
                // UIManager.Show<InteractionUI>(UIList.InteractionUI);
            }
        }

        private static void InternalBoot()
        {
            Main.Singleton.Initialize();
        }
    }
}
#endif