using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using OpenAI;
public class NpcDialog : MonoBehaviour
{
    [SerializeField] private GameObject toActivate;
    [SerializeField] private Whisper whisper;
    //[SerializeField] private Transform standingPoint;

    //private Transform avatar;
    
    private async void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            whisper.CanRecord = true;
            toActivate.SetActive(true);
        }
    }
    private async void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            whisper.CanRecord = false;
            toActivate.SetActive(false);
        }
    }
    public void Recover()
    {
        //avatar.GetComponent<PlayerInput>().enabled = true;

        toActivate.SetActive(false);

        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }
}
