using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//泛型單例模式 類型是任何東西， T 為 type，MonoBehaviour 約束為Singleton的類型
public class Singleton<T> : MonoBehaviour where T :Singleton<T> 
{
    private static T instance; //T 代表mouse manager    
    public static T Instance //set get 在外部可訪問，但不可修改
    {
        get {return instance;}
    }
    protected virtual void Awake()
    {
        if(instance != null)
            Destroy(gameObject);
        else
            instance = (T)this;
    }
    public static bool isInitialized //確認單例模式是否被初始化過了
    {
        get{ return instance != null;}
    }
    protected virtual void OnDestroy() //有許多單利模式需要將他銷毀 判斷是否有被銷毀
    {
        if(instance == this){
            instance = null;
        }
    }
}
