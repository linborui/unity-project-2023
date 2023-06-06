using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
using UnityEngine.UI;

public class WeaponSwitching : MonoBehaviour
{

    public int selectedWeapon = 0;
    public GameObject player;
    private PlayerMovement pm;
    public GameObject image;
    private Image im;
    public bool addWeapon;

    // Start is called before the first frame update
    void Start()
    {
        SelectWeapon();
        pm = player.GetComponent<PlayerMovement>();
        im = image.GetComponent<Image>();

    }

    // Update is called once per frame
    void Update()
    {
        int previousSelectedWeapon = selectedWeapon;

        if (!pm.swinging)
        {
            if (InputManager.GetAxisRaw("ChangeWeapon") > 0f)
            {
                if (selectedWeapon >= transform.childCount - 1)
                    selectedWeapon = 0;
                else
                    selectedWeapon++;
            }
            if (InputManager.GetAxisRaw("ChangeWeapon") < 0f)
            {
                if (selectedWeapon <= 0)
                    selectedWeapon = transform.childCount - 1;
                else
                    selectedWeapon--;
            }
        }

        if (previousSelectedWeapon != selectedWeapon || addWeapon )
        {
            SelectWeapon();
            im.color = Color.white;

            addWeapon = false;
        }
    }

    void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == selectedWeapon)
                weapon.gameObject.SetActive(true);
            else
                weapon.gameObject.SetActive(false);
            i++;
        }
    }
}
