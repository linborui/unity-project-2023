using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class NpcDialog : MonoBehaviour
{
    [SerializeField] private GameObject toActivate;

    //[SerializeField] private Transform standingPoint;

    //private Transform avatar;
    
    private async void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            

            // disable main cam, enable dialog cam
            toActivate.SetActive(true);
            Debug.Log("Activate");
            // dısplay cursor
            //Cursor.visible = true;
            //Cursor.lockState = CursorLockMode.None;
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
