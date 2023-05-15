using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Luminosity.IO;
public class DialogSystem : MonoBehaviour
{
    public TextMeshProUGUI textPlayerDisplay;
    public string[] sentences;
    [HideInInspector]
    public int index = 0;
    public float typingSpeed;
    private bool nextButton;
    private bool isNPC = true;
    public TextMeshProUGUI textNPCDisplay;
    // Start is called before the first frame update
    void OnEnable()
    {
        textPlayerDisplay.text = "";
        textNPCDisplay.text = "";
        index = 0;
        isNPC = true;
       StartCoroutine(Type()); 
    }

    IEnumerator Type()
    {
        foreach (char letter in sentences[index].ToCharArray())
        {
            if(!isNPC){
                textPlayerDisplay.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
            else if(isNPC){
                textNPCDisplay.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
        }
    }
    public void Update(){
        if(!isNPC && (textPlayerDisplay.text == sentences[index] ))
        {
            nextButton = true;
            if(InputManager.GetButtonDown("Next")){
                NextSentence();
            }
        }
        else if(isNPC &&  (textNPCDisplay.text == sentences[index]))
        {
            nextButton = true;
            if(InputManager.GetButtonDown("Next")){
                NextSentence();
            }
        }

    }
    public void NextSentence()
    {
        nextButton = false;
        isNPC = !isNPC;
        if(!isNPC){
            if(index < sentences.Length - 1)
            {
               index ++;
               textPlayerDisplay.text ="";
                StartCoroutine(Type());
            }
            else
            {
                textPlayerDisplay.text = "";
                nextButton = false;
            }
        }
        else if(isNPC){
            if(index < sentences.Length - 1)
            {
               index ++;
               textNPCDisplay.text ="";
                StartCoroutine(Type());
            }
            else
            {
                textNPCDisplay.text = "";
                nextButton = false;
            }
        }
    }
}
