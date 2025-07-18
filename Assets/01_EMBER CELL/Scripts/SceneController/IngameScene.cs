using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TEC
{
    public class IngameScene : SceneBase
    {
        public override IEnumerator OnStart()
        {
            AsyncOperation async = SceneManager.LoadSceneAsync(SceneType.Ingame.ToString(), LoadSceneMode.Single);
            yield return new WaitUntil(()=> async.isDone);
            // TODO : Ingame Scene Initialize
            // TODO : Show Ingame Scene UI
            UIManager.Show<MainHUD>(UIList.MainHUD);
            UIManager.Show<CrossHairUI>(UIList.CrossHairUI);
            // UIManager.Show<InteractionUI>(UIList.InteractionUI);

            yield return null;
        }

        public override IEnumerator OnEnd()
        {
            yield return null;

            // TODO : Ingame Scene Destory
            // TODO : Hide Ingame Scene UI
            UIManager.Hide<MainHUD>(UIList.MainHUD);
            UIManager.Hide<CrossHairUI>(UIList.CrossHairUI);
            // UIManager.Hide<InteractionUI>(UIList.InteractionUI);

        }

    }
}
