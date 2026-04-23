using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TEC
{
    public class CampScene : SceneBase
    {
        public override IEnumerator OnStart()
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;

            AsyncOperation async = SceneManager.LoadSceneAsync(SceneType.Camp.ToString(), LoadSceneMode.Single); // [CHANGED]
            yield return new WaitUntil(()=> async.isDone);

            UIManager.Show<MainHUD>(UIList.MainHUD);
            UIManager.Show<CrossHairUI>(UIList.CrossHairUI);

            SoundManager.Singleton.PlayMusic("Music_1");

            yield return null;
        }

        public override IEnumerator OnEnd()
        {
            yield return null;

            UIManager.Hide<MainHUD>(UIList.MainHUD);
            UIManager.Hide<CrossHairUI>(UIList.CrossHairUI);
        }
    }
}