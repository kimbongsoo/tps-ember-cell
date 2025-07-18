using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEditorInternal;
using UnityEngine;

namespace KBS
{
    public class SingletonBase<T> : MonoBehaviour where T : class
    {
        public static T Singleton
        {
            get
            {
                return _instance.Value;
            }
        }

        private static readonly System.Lazy<T> _instance = new System.Lazy<T>(() =>
        {
            T instance = UnityEngine.Object.FindObjectOfType(typeof(T)) as T;

            if (instance == null)
            {
                GameObject obj = new GameObject(typeof(T).Name);
                instance = obj.AddComponent(typeof(T)) as T;
#if UNITY_EDITOR
                if (EditorApplication.isPlaying)
                {
                    DontDestroyOnLoad(obj);
                }
#else
                DontDestroyOnLoad(obj);
#endif
            }

            return instance;
        });

        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
