using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSwitch : MonoBehaviour
{
    public int selectedWeapon = 0;
    public int[] weaponAmmo;
    public bool canSwitchWeapon;
    public bool notDead;
    Text ammoCounter;

    private void Awake()
    {
        SelectWeapon();
        weaponAmmo = new int[transform.childCount];
    }

    // Start is called before the first frame update
    void Start()
    {
        ammoCounter = UIManager.UI.ammoCounter;
        notDead = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (notDead)
        {
            if (canSwitchWeapon)
            {
                int _selectedWeapon = selectedWeapon;
                if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                {
                    if (selectedWeapon >= transform.childCount - 1)
                        selectedWeapon = 0;
                    else
                        selectedWeapon++;
                }

                if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                {
                    if (selectedWeapon <= 0)
                        selectedWeapon = transform.childCount - 1;
                    else
                        selectedWeapon--;
                }

                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    selectedWeapon = 0;
                }

                if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2)
                {
                    selectedWeapon = 1;
                }

                if (Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 3)
                {
                    selectedWeapon = 2;
                }

                if (Input.GetKeyDown(KeyCode.Alpha4) && transform.childCount >= 4)
                {
                    selectedWeapon = 3;
                }

                if (Input.GetKeyDown(KeyCode.Alpha5) && transform.childCount >= 5)
                {
                    selectedWeapon = 4;
                }

                if (_selectedWeapon != selectedWeapon)
                {
                    SelectWeapon();
                }
            }

            if (selectedWeapon == 0 || Randomizer.InfiniteAmmo)
            {
                ammoCounter.text = "∞";
            }
            else
            {
                ammoCounter.text = weaponAmmo[selectedWeapon].ToString();
            }
        }
        else
        {
            foreach(Transform weapon in transform)
            {
                weapon.gameObject.SetActive(false);
            }
        }
    }

    void SelectWeapon()
    {
        int i = 0;
        foreach(Transform weapon in transform)
        {
            if (i == selectedWeapon)
            {
                weapon.gameObject.SetActive(true);
            } else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }
}
