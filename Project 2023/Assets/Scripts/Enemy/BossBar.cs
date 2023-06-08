using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBar : MonoBehaviour
{
    public string[] bgm;
    public GameObject UI;
    public AI status;
    public Image Health;
    public AudioController Audio;
    public BoxCollider Gate;
    public GameObject tele;

    Vector3 pos;
    Quaternion rot;
    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        rot = transform.rotation;
    }

    // Update is called once per frameS
    void Update()
    {
        if(UI && Health && status){
            if(status.awareness && status.HP > 0){
                Health.fillAmount = status.HP / status.MaxHP;
                Audio.changeAudio(bgm[1]);
                UI.SetActive(true);
                Gate.enabled = true;
            }else{
                Audio.changeAudio(bgm[0]);
                UI.SetActive(false);
                Gate.enabled = false;
                if(status.HP > 0){
                    transform.position = pos;
                    transform.rotation = rot;
                    status.fsm.SetInteger("x", 0);
                    status.fsm.SetInteger("y", 0);
                }else{
                    tele.SetActive(true);
                }
            }
        }
    }
}
