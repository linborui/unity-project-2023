using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public CharacterStates playerStates; //方便集中管理，其他想訪問palyerStats的代碼可以直接訪問這個GameManager

    //每個敵人都有IEndGameObserver 的interface，所以可以用List來管理
    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();

    //用觀察者的模式反向註冊
    public void RegisterPlayer(CharacterStates player){
        playerStates = player;
    }
    //觀察者將自己註冊到GameManager的List中
    public void AddObserver(IEndGameObserver observer){
        endGameObservers.Add(observer);
    }
    public void RemoveObserver(IEndGameObserver observer){
        endGameObservers.Remove(observer);
    }
    public void NotifyObservers(){ //廣播
        foreach(var observer in endGameObservers){
            observer.EndNotify(); //讓每個觀察者都執行EndNotifgy
        }
    }
}
