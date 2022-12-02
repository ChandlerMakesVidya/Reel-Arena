using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth;
    public int health;
    public float drainRate;
    float timer;
    public GameObject deathCamera;
    public AudioSource damageSound;

    private void Start()
    {
        timer = drainRate;
        health = maxHealth;
    }

    private void Update()
    {
        if (Randomizer.PlayerHealthDrain != 0)
        {
            drainRate -= Time.deltaTime;
        }
        else
        {
            drainRate = timer;
        }

        if(drainRate <= 0.0f)
        {
            TakeDamage(Randomizer.PlayerHealthDrain);
            drainRate = timer;
        }
    }

    public void Heal(int amount)
    {
        health += amount;

        if (health > maxHealth) health = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        health -= amount * (int)Randomizer.EnemyDamageModifier;
        UIManager.UI.ShowPain();
        damageSound.Play();

        if(health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        FindObjectOfType<WeaponSwitch>().notDead = false;
        GameManager.GM.gameState = GameManager.GameState.GameOver;
        GameObject dc = Instantiate(deathCamera, Camera.main.transform.position, Camera.main.transform.rotation);
        Destroy(this.gameObject);
    }
}
