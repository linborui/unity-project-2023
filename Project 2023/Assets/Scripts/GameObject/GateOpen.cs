using Luminosity.IO;
using UnityEngine;

public class GateOpen : MonoBehaviour
{
    public Transform gate1, gate2;
    public float rotationGoal;
    public GameObject interactMessage;
    bool opened = false;
    bool opening = false;
    float openSpeed = 45f;
    float rotationCnt = 0f;

    void Start()
    {
    }
    void Update()
    {
        if (!opening)
            return;

        if (rotationCnt < rotationGoal)
        {
            float y1 = gate1.eulerAngles.y;
            float y2 = gate2.eulerAngles.y;
            float rotate = openSpeed * Time.deltaTime;
            if (rotationCnt + rotate > rotationGoal)
            {
                rotate = rotationGoal - rotationCnt;
                rotationCnt = rotationGoal;
                opening = false;
            }
            else
            {
                rotationCnt += rotate;
            }

            y1 -= rotate;
            y2 += rotate;

            gate1.rotation = Quaternion.Euler(gate1.rotation.eulerAngles.x, y1, gate1.rotation.eulerAngles.z);
            gate2.rotation = Quaternion.Euler(gate2.rotation.eulerAngles.x, y2, gate2.rotation.eulerAngles.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (opened)
            return;

        interactMessage.SetActive(true);

        if (other.tag.Equals("Player"))
        {
            if (InputManager.GetButtonDown("Interact"))
            {
                opened = true;
                opening = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (opened)
            return;

        if (other.tag.Equals("Player"))
        {
            if (InputManager.GetButtonDown("Interact"))
            {
                opened = true;
                opening = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        interactMessage.SetActive(false);
    }
}
