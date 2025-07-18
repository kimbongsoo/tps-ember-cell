using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KBS
{
    public enum SceneType
    {
        None,
        Empty,
        Title,
        Ingame
    }
    public class Main : SingletonBase<Main>
    {
        public SceneBase currentScene;
        private SceneType currentSceneType;

        private bool isInitialized = false;


        private void Start()
        {
            StartUp();
        }

        public void Initialize()
        {
            if (isInitialized)
                return;
            // Manager & System Initialize
            UIManager.Singleton.Initialize();

            isInitialized = true;
        }

        public void StartUp()
        {
            Initialize();

#if UNITY_EDITOR
            Scene activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            if (activeScene.name.Equals("Main"))
            {
                ChangeScene(SceneType.Title);
            }
#else
            ChangeScene(SceneType.Title);
#endif
        }

        public void ChangeScene(SceneType sceneType)
        {
            switch (sceneType)
            {
                case SceneType.Title:
                    {
                        ChangeScene<TitleScene>(sceneType);
                    }
                    break;
                case SceneType.Ingame:
                    {
                        ChangeScene<IngameScene>(sceneType);
                    }
                    break;
            }
        }

        private void ChangeScene<T>(SceneType sceneType) where T : SceneBase
        {
            StartCoroutine(InternalChangeScene<T>(sceneType));
        }

        private IEnumerator InternalChangeScene<T>(SceneType sceneType) where T : SceneBase
        {
            var loadingUI = UIManager.Show<LoadingUI>(UIList.LoadingUI);
            loadingUI.LoadingPercentage = 0f;

            if (currentScene != null)
            {
                yield return StartCoroutine(currentScene.OnEnd());
                Destroy(currentScene.gameObject);
            }
            loadingUI.LoadingPercentage = 0.45f;
            yield return new WaitForEndOfFrame();


            GameObject newSceneInstance = new GameObject(typeof(T).Name);
            newSceneInstance.transform.SetParent(this.transform);
            currentScene = newSceneInstance.AddComponent<T>();
            yield return StartCoroutine(currentScene.OnStart());
            loadingUI.LoadingPercentage = 0.8f;
            yield return new WaitForEndOfFrame();

            yield return new WaitForSeconds(0.3f);
            loadingUI.LoadingPercentage = 1f;
            yield return new WaitForSeconds(0.3f);
            UIManager.Hide<LoadingUI>(UIList.LoadingUI);  

            currentSceneType = sceneType;               
        }

    
    }
}
