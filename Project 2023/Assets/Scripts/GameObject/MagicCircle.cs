using UnityEngine;
using UnityEngine.UI;
using Luminosity.IO;

public class MagicCircle : MonoBehaviour
{
    public GameObject destination;
    public Image radiusIndicator;
    public GameObject interactMessage;
    public CastleGateOpen gate;

    float pressTime = 3f;
    float indicatorTime;

    Vector3 playerPosition;

    void Start()
    {
        indicatorTime = 0f;
    }

    void Update()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            interactMessage.SetActive(true);
            indicatorTime = 0f;
            radiusIndicator.fillAmount = 0f;
            playerPosition = other.transform.position;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (InputManager.GetButton("Interact"))
            {
                indicatorTime += Time.deltaTime;
                radiusIndicator.fillAmount = indicatorTime / pressTime;
                if (indicatorTime >= pressTime)
                {
                    interactMessage.SetActive(false);
                    other.transform.position = destination.transform.position;
                    gate.detectMagicCircle();
                    gameObject.SetActive(false);
                }
            }
            else
            {
                indicatorTime = 0f;
                radiusIndicator.fillAmount = 0f;
            }

            if (other.transform.position != playerPosition)
            {
                indicatorTime = 0f;
                radiusIndicator.fillAmount = 0f;
                playerPosition = other.transform.position;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            interactMessage.SetActive(false);
            indicatorTime = 0f;
            radiusIndicator.fillAmount = 0f;
        }
    }
}
