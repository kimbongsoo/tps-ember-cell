using UnityEngine;

namespace TEC
{
    public class SingletonBase<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Singleton
        {
            get
            {
                if (_instance == null)
                {
                    _instance = UnityEngine.Object.FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject(typeof(T).Name);
                        _instance = obj.AddComponent<T>();
                    }
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }

        protected virtual void OnDestroy()
        {
            if (ReferenceEquals(_instance, this))
                _instance = null;
        }
    }
}
