using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethod //靜態類可隨時快速調用
{
    private const float dotThreshold = 0.5f; //因為是static 的class 所以裡面的變數要是常數
    //第一個參數為要哪個類別的礦展，擴展的類，第二個參數才是函數的變量
    //當player跑到敵人身後時，敵人的動畫，就不會對player產生傷害
    public static bool isFacingTarget(this Transform transform,Transform target)
    {
        var VectorToTarget = target.position - transform.position;
        VectorToTarget.Normalize();
        float dot = Vector3.Dot(transform.forward,VectorToTarget);
        return dot >= dotThreshold;
    }
}
