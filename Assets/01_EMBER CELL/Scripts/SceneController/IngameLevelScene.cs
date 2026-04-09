using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TEC
{
    public class IngameLevelScene : SceneBase
    {
        private CharacterBase playerCharacterBase; 
        private bool isReturning = false; 

        public override IEnumerator OnStart()
        {
            Time.timeScale = 1f; 
            Time.fixedDeltaTime = 0.02f; 

            AsyncOperation async = SceneManager.LoadSceneAsync(SceneType.IngameLevel.ToString(), LoadSceneMode.Single);

            if (async == null) 
            {
                Debug.LogError($"[IngameLevelScene] LoadSceneAsync Failed : {SceneType.IngameLevel}");
                yield break;
            }

            yield return new WaitUntil(() => async.isDone);

            if (CharacterPlayerController.Instance != null) 
            {
                playerCharacterBase = CharacterPlayerController.Instance.GetComponent<CharacterBase>(); 
            }

            if (playerCharacterBase != null) 
            {
                playerCharacterBase.OnDeadStateChanged += OnPlayerDeadStateChanged; 
            }

            UIManager.Show<MainHUD>(UIList.MainHUD);
            UIManager.Show<CrossHairUI>(UIList.CrossHairUI);
            // UIManager.Show<InteractionUI>(UIList.InteractionUI);

            SoundManager.Singleton.PlayMusic("Music_1");

            yield return null;
        }

        public override IEnumerator OnEnd()
        {
            if (playerCharacterBase != null) 
            {
                playerCharacterBase.OnDeadStateChanged -= OnPlayerDeadStateChanged; 
                playerCharacterBase = null; 
            }

            yield return null;

            UIManager.Hide<MainHUD>(UIList.MainHUD);
            UIManager.Hide<CrossHairUI>(UIList.CrossHairUI);
            // UIManager.Hide<InteractionUI>(UIList.InteractionUI);
        }

        private void OnPlayerDeadStateChanged(bool isDead) 
        {
            if (!isDead) 
                return;

            if (isReturning) 
                return;

            isReturning = true;
            StartCoroutine(ReturnToIngameRoutine()); 
        }

        private IEnumerator ReturnToIngameRoutine() 
        {
            float originalTimeScale = Time.timeScale; 
            float targetTimeScale = 0.2f; 
            float slowDuration = 2f; 
            float restoreSpeed = 2f; 

            Time.timeScale = targetTimeScale; 
            Time.fixedDeltaTime = 0.02f * Time.timeScale; 

            yield return new WaitForSecondsRealtime(slowDuration); 

            float t = 0f; 
            while (t < 1f) 
            {
                t += Time.unscaledDeltaTime * restoreSpeed; 
                Time.timeScale = Mathf.Lerp(targetTimeScale, originalTimeScale, t); 
                Time.fixedDeltaTime = 0.02f * Time.timeScale; 
                yield return null; 
            }

            Time.timeScale = 1f; 
            Time.fixedDeltaTime = 0.02f; 

            Main.Singleton.ChangeScene(SceneType.Ingame);
        }
    }
}