using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairManage : MonoBehaviour
{
    //set a Chunk array
    static public DirectCast[] Casts;
    static public int maximum;

    // declear a queue to store the index of chunks
    static public Queue<int> indexList = new Queue<int>();
    void Update(){
        if(indexList.Count == 0)
            return;
        int index = indexList.Dequeue();;
        if(index + 1 < StairManage.maximum)
            StairManage.Casts[index+1].setWake();
        if(index + 2 < StairManage.maximum)
            StairManage.Casts[index+2].setWake();


    }
    void Awake(){
        //get all the chunks
        Casts = FindObjectsOfType<DirectCast>();
        maximum = Casts.Length;
        // sort chunks by its index
        System.Array.Sort(Casts, delegate(DirectCast x, DirectCast y) { return x.index.CompareTo(y.index); });
    }
    // Start is called before the first frame update
    void OnTirggerEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player"){
            //set the first chunk to be awake
            Casts[0].setWake();
            Casts[1].setWake();
        }
    }
}
