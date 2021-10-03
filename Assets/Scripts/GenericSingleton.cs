using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GameData에 있는 정보에 접근하기 쉽게 하기위해서 GameData를 싱글톤으로 만들어주기 위한 곳입니다.

public class GenericSingleton<T> : MonoBehaviour where T : Component
{
    [SerializeField] private static T instance = null;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();

                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    instance = obj.GetComponent<T>();
                    obj.name = typeof(T).Name;
                }
            }

            return instance;
        }
    }
        
    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
