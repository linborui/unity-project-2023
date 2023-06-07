using Luminosity.IO;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    public bool clockwise;
    public float rotationGoal;
    public GameObject interactMessage;
    bool opened = false;
    bool opening = false;
    float openSpeed = 60f;
    float rotationCnt = 0f;
    Transform door;
    void Start()
    {
        door = transform.parent;
    }

    void Update()
    {
        if (!opening)
            return;

        if (rotationCnt < rotationGoal)
        {
            float y = door.eulerAngles.y;
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

            if (clockwise)
            {
                y += rotate;
            }
            else
            {
                y -= rotate;
            }
            door.rotation = Quaternion.Euler(door.rotation.eulerAngles.x, y, door.rotation.eulerAngles.z);
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
