using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KBS
{
    public class TitleScene : SceneBase
    {
        public override IEnumerator OnStart()
        {
            AsyncOperation async = SceneManager.LoadSceneAsync(SceneType.Title.ToString(), LoadSceneMode.Single);
            yield return new WaitUntil(()=> async.isDone);
            // TODO : Title Scene Initialize
            // TODO : Show Title Scene UI

            UIManager.Show<TitleUI>(UIList.TitleUI);

            yield return null;
        }

        public override IEnumerator OnEnd()
        {
            yield return null;

            // TODO : Title Scene Destory
            // TODO : Hide Title Scene UI
            UIManager.Hide<TitleUI>(UIList.TitleUI);
        }
    }
}
