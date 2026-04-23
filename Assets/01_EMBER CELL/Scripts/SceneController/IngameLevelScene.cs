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

        private const string TARGET_QUEST_ID = "get_Military_map";

        private ResultUI resultUI; 
        private MissionFailUI missionFailUI; 

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

            if (QuestManager.Singleton != null)
            {
                QuestManager.Singleton.OnQuestStateChanged += OnQuestStateChanged;
            }

            resultUI = UIManager.Singleton.GetUI<ResultUI>(UIList.ResultUI); 
            if (resultUI != null) 
            {
                resultUI.Hide(); 
            }

            missionFailUI = UIManager.Singleton.GetUI<MissionFailUI>(UIList.MissionFailUI); 
            if (missionFailUI != null) 
            {
                missionFailUI.Hide(); 
            }

            UIManager.Show<MainHUD>(UIList.MainHUD);
            UIManager.Show<CrossHairUI>(UIList.CrossHairUI);

            var interactionUI = UIManager.Singleton.GetUI<InteractionUI>(UIList.InteractionUI);
            if (interactionUI != null)
            {
                interactionUI.ClearData();
                interactionUI.Hide();
            }

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

            if (QuestManager.Singleton != null)
            {
                QuestManager.Singleton.OnQuestStateChanged -= OnQuestStateChanged;
            }

            if (resultUI != null) 
            {
                resultUI.Hide(); 
            }

            if (missionFailUI != null) 
            {
                missionFailUI.Hide(); 
            }

            var interactionUI = UIManager.Singleton.GetUI<InteractionUI>(UIList.InteractionUI);
            if (interactionUI != null)
            {
                interactionUI.ClearData();
                interactionUI.Hide();
            }

            yield return null;

            UIManager.Hide<MainHUD>(UIList.MainHUD);
            UIManager.Hide<CrossHairUI>(UIList.CrossHairUI);
        }

        private void OnPlayerDeadStateChanged(bool isDead)
        {
            if (!isDead)
                return;

            if (isReturning)
                return;

            isReturning = true;

            Debug.Log("[IngameLevelScene] Player Dead → MissionFailUI"); 

            ShowMissionFailUI(); 
        }

        private void OnQuestStateChanged(string questID, QuestState state)
        {
            if (questID != TARGET_QUEST_ID)
                return;

            if (state != QuestState.Completed)
                return;

            if (isReturning)
                return;

            isReturning = true;

            Debug.Log("[IngameLevelScene] Quest Complete → ResultUI"); 

            ShowResultUI(); 
        }

        private void ShowResultUI() 
        {
            var interactionUI = UIManager.Singleton.GetUI<InteractionUI>(UIList.InteractionUI);
            if (interactionUI != null)
            {
                interactionUI.ClearData();
                interactionUI.Hide();
            }

            if (CharacterPlayerController.Instance != null)
            {
                CharacterPlayerController.Instance.SetSequenceControl(true);
            }

            resultUI = UIManager.Show<ResultUI>(UIList.ResultUI);
            if (resultUI != null)
            {
                resultUI.ShowResult();
            }

            Time.timeScale = 0f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }

        private void ShowMissionFailUI() 
        {
            var interactionUI = UIManager.Singleton.GetUI<InteractionUI>(UIList.InteractionUI); 
            if (interactionUI != null) 
            {
                interactionUI.ClearData(); 
                interactionUI.Hide(); 
            }

            if (CharacterPlayerController.Instance != null) 
            {
                CharacterPlayerController.Instance.SetSequenceControl(true); 
            }

            missionFailUI = UIManager.Show<MissionFailUI>(UIList.MissionFailUI);
            if (missionFailUI != null) 
            {
                missionFailUI.ShowMissionFail(); 
            }

            Time.timeScale = 0f; 
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }

        private IEnumerator ReturnAfterQuestComplete()
        {
            var interactionUI = UIManager.Singleton.GetUI<InteractionUI>(UIList.InteractionUI);
            if (interactionUI != null)
            {
                interactionUI.ClearData();
                interactionUI.Hide();
            }

            if (CharacterPlayerController.Instance != null &&
                CharacterPlayerController.Instance.InteractionSensor != null)
            {
                CharacterPlayerController.Instance.InteractionSensor.PulseManuallyNextFrame();
            }

            yield return new WaitForSecondsRealtime(1.5f);

            yield return StartCoroutine(ReturnToIngameRoutine());
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

            Main.Singleton.ChangeScene(SceneType.Camp); //Ingame ->Camp
        }
    }
}