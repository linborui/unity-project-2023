using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalPlaceSet : MonoBehaviour
{
    [Header("Portal")]
    public Portal FixedPortalIn; // 0
    public Portal FixedPortalOut;// 1
    [Header("NPC")]
    public GameObject NPC;
    [Header("Portal Placement")]
    public Transform _A;
    public Transform _B;
    public Transform _C;
    public Transform _D;
    public Transform _E;
    [Header("NPC Placement")]
    public Transform Level1_Placement;
    public Transform Forest_Placement;
    public Transform Level2_Placement;
    [HideInInspector]
    public int state;
    public GameObject timer;

    private int inPortal_In_state0 =2;
    private int outPortal_In_state0 =3;
    private int inPortal_In_state2 = 1;
    private int outPortal_In_state2 = 2;
    public PortalContainer _container;
    public PortalPlacement portalplacement;
    public GameObject fantasyWorld;
    public GameObject enemyLevel1;
    public GameObject enemyLevel2;
    public GameObject enemyForest;
    // state 0 :Level 1 : PortalIn in A , PortalOut in B; when level 1 success, PortalIn will change to 
    // state 1 : when level 1 success, PortalIn in E , PortalOut in D
    // state 2 : when player in forest, PortalOut will be placed in C
    // state 3 : when player in forest, PortalOut will be placed in B
    // Start is called before the first frame update
    bool first = true;
    public void UpdateState(int statepar){
        state = statepar;
        Invoke("changePosition",2f);
    }
    public void changePosition(){
        if(state == 0){ // player in level 1
            timer.SetActive(false);
            fantasyWorld.SetActive(false);

            enemyForest.SetActive(false);
            enemyLevel1.SetActive(true);
            enemyLevel2.SetActive(false);
            
            FixedPortalOut.transform.position = _B.position;
            FixedPortalOut.transform.rotation = _B.rotation;
            NPC.transform.position = Level1_Placement.transform.position;
        }
        else if(state == 3){
            timer.SetActive(true);
            fantasyWorld.SetActive(true);
            enemyForest.SetActive(true);
            enemyLevel1.SetActive(false);
            enemyLevel2.SetActive(false);
            timer.GetComponent<TimerControl>().CountStart();
            NPC.transform.position = Forest_Placement.transform.position;
            if(first == true)
            {
                FixedPortalOut.transform.position = _C.position;
                FixedPortalOut.transform.rotation = _C.rotation;
            }
            else
            {
                FixedPortalOut.transform.position = _B.position;
                FixedPortalOut.transform.rotation = _B.rotation;
            }
        }
        else if(state == 1){
            timer.SetActive(false);
            fantasyWorld.SetActive(false);
            enemyForest.SetActive(false);
            enemyLevel1.SetActive(false);
            enemyLevel2.SetActive(true);
            NPC.transform.position = Level2_Placement.transform.position;
            portalplacement.portalInNum = inPortal_In_state2;
            portalplacement.portalOutNum = outPortal_In_state2;
            FixedPortalIn.transform.position = _E.position;
            FixedPortalIn.transform.rotation = _E.rotation;
            FixedPortalOut.transform.position = _D.position;
            FixedPortalOut.transform.rotation = _D.rotation;
        }
        else if(state == 2){
            timer.SetActive(true);
            timer.GetComponent<TimerControl>().CountStart();
            fantasyWorld.SetActive(true);

            enemyForest.SetActive(false);
            enemyLevel1.SetActive(false);
            enemyLevel2.SetActive(true);
            
            _container.AppendGameObject = null;
            NPC.transform.position = Level2_Placement.transform.position;
            if(first == true)
            {
                FixedPortalOut.transform.position = _A.position;
                FixedPortalOut.transform.rotation = _A.rotation;
            }
            else 
            {
                FixedPortalOut.transform.position = _B.position;
                FixedPortalOut.transform.rotation = _B.rotation;
            }
        }
    }
    void Awake()
    {
        portalplacement.portalInNum = inPortal_In_state0;
        portalplacement.portalOutNum = outPortal_In_state0;
    }
    void Start()
    {
        state = 0;
    }

    public void Change(){
        first = !first;
        UpdateState(state);
    }
}
