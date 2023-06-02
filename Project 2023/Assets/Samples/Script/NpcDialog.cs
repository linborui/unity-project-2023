using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class NpcDialog : MonoBehaviour
{
    //[SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject toActivate;

    private Transform avatar;
    
    private async void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("PlayerCome IN");
           // mainCamera.SetActive(false);
           toActivate.SetActive(true);
            // dısplay cursor
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void Recover()
    {
        toActivate.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
