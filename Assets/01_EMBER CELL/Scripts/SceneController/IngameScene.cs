// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.SceneManagement;

// namespace TEC
// {
//     public class IngameScene : SceneBase
//     {
//         public override IEnumerator OnStart()
//         {
//             Time.timeScale = 1f; 
//             Time.fixedDeltaTime = 0.02f; 

//             AsyncOperation async = SceneManager.LoadSceneAsync(SceneType.Ingame.ToString(), LoadSceneMode.Single);
//             yield return new WaitUntil(()=> async.isDone);

//             UIManager.Show<MainHUD>(UIList.MainHUD);
//             UIManager.Show<CrossHairUI>(UIList.CrossHairUI);

//             var interactionUI = UIManager.Singleton.GetUI<InteractionUI>(UIList.InteractionUI); 
//             if (interactionUI != null) 
//             {
//                 interactionUI.ClearData(); 
//                 interactionUI.Hide(); 
//             }

//             var resultUI = UIManager.Singleton.GetUI<ResultUI>(UIList.ResultUI); 
//             if (resultUI != null) 
//             {
//                 resultUI.Hide(); 
//             }

//             if (CharacterPlayerController.Instance != null &&
//                 CharacterPlayerController.Instance.InteractionSensor != null) 
//             {
//                 CharacterPlayerController.Instance.InteractionSensor.PulseManuallyNextFrame(); 
//             }

//             SoundManager.Singleton.PlayMusic("Music_1");

//             yield return null;
//         }

//         public override IEnumerator OnEnd()
//         {
//             var interactionUI = UIManager.Singleton.GetUI<InteractionUI>(UIList.InteractionUI); 
//             if (interactionUI != null) 
//             {
//                 interactionUI.ClearData(); 
//                 interactionUI.Hide(); 
//             }

//             var resultUI = UIManager.Singleton.GetUI<ResultUI>(UIList.ResultUI); 
//             if (resultUI != null) 
//             {
//                 resultUI.Hide(); 
//             }

//             yield return null;

//             UIManager.Hide<MainHUD>(UIList.MainHUD);
//             UIManager.Hide<CrossHairUI>(UIList.CrossHairUI);
//         }
//     }
// }