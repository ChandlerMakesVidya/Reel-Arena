using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Slider healthBar;
    PlayerHealth playerHealth;
    // Start is called before the first frame update
    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        healthBar.maxValue = playerHealth.health;
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.value = playerHealth.health;
    }
}
