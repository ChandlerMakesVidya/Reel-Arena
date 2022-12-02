using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Randomizer : MonoBehaviour
{
    /*public static bool randomizerActive;
    public static List<int> activeRandomizers;
    static void OnLevelLoad(Scene scene, LoadSceneMode mode)
    {
        activeRandomizers = new List<int>(3);
        randomizerActive = true;
    }

    static IEnumerator PickRandomizer()
    {
        while (randomizerActive == true)
        {
            yield return new WaitForSeconds(5);
            int _rand = Random.Range(0, 2);
            activeRandomizers.Add(_rand);
            for(int i = 0; i < activeRandomizers.Count; i++)
            {
                print(activeRandomizers[i]);
            }
        }
    }*/

    public bool randomizerActive;
    public List<int> activeRandomizers;
    public float timer;
    public Text selectingRandomizerText;
    public Text activeRandomizersText;
    public AudioSource alarmSound;

    //random effects
    public static float PlayerSpeedModifier { get; private set; } = 1.0f;
    public static float EnemySpeedModifier { get; private set; } = 1.0f;
    public static bool InfiniteAmmo { get; private set; } = false;
    public static float FireSpeedModifier { get; private set; } = 1.0f;
    public static float FrictionModifier { get; private set; } = 1.0f;
    public static Vector3 Gravity { get; private set; } = new Vector3(0.0f, -9.81f, 0.0f);
    public static float PlayerDamageModifier { get; private set; } = 1.0f;
    public static float EnemyDamageModifier { get; private set; } = 1.0f;
    public static bool EnemiesExplodeUponDeath { get; private set; } = false;
    public static int PlayerHealthDrain { get; private set; } = 0;
    //public static bool ReducedVisibility { get; private set; } = false;
    public static bool RainingBombs { get; private set; } = false;

    private void Start()
    {
        ResetRandomizer();
    }

    /*public void ActivateRandomizer(bool value)
    {
        if(value == true)
        {
            randomizerActive = true;
            //StartCoroutine(SelectRandomEffect(timer));
        }
        else if (value == false)
        {
            randomizerActive = false;
            //StopCoroutine(SelectRandomEffect(timer));
        }
    }*/

    public void ResetRandomizer()
    {
        PlayerSpeedModifier = 1.0f;
        EnemySpeedModifier = 1.0f;
        InfiniteAmmo = false;
        FireSpeedModifier = 1.0f;
        FrictionModifier = 1.0f;
        Gravity = new Vector3(0.0f, -9.81f, 0.0f);
        Physics.gravity = Gravity;
        PlayerDamageModifier = 1.0f;
        EnemyDamageModifier = 1.0f;
        EnemiesExplodeUponDeath = false;
        PlayerHealthDrain = 0;
        //ReducedVisibility = false;
        RenderSettings.fog = false;
        RainingBombs = false;

        activeRandomizers = new List<int>(new int[3]);
        selectingRandomizerText.enabled = false;
        timer = 60.0f;
        CheckAndApplyEffects();
        UpdateTextDisplay();
    }

    private void Update()
    {
        if (randomizerActive)
        {
            timer -= Time.deltaTime;
            if(timer <= 55.0f && timer > 5.0f)
            {
                selectingRandomizerText.enabled = false;
            } else
            {
                selectingRandomizerText.enabled = true;
                if(timer <= 5.0f && timer > 0.0f)
                {
                    selectingRandomizerText.text = "Rolling Randomizer...";
                    if (alarmSound != null && !alarmSound.isPlaying) alarmSound.Play();
                } else if(timer <= 0.0f)
                {
                    int _rand;
                    do
                    {
                        _rand = Random.Range(1, 11);
                    } while (activeRandomizers.Contains(_rand));
                    activeRandomizers.Insert(0, _rand);
                    activeRandomizers.RemoveAt(3);
                    CheckAndApplyEffects();
                    UpdateTextDisplay();
                    timer = 60.0f;
                }
            }
        }
        else
        {
            timer = 60.0f;
        }
    }

    /*IEnumerator SelectRandomEffect(float t)
    {
        while (randomizerActive)
        {
            yield return new WaitForSeconds(5);
            selectingRandomizerText.enabled = false;
            yield return new WaitForSeconds(timer - 10);
            selectingRandomizerText.enabled = true;
            selectingRandomizerText.text = "Selecting Randomizer...";
            TempSoundManager.PlaySound(alarmSound);
            yield return new WaitForSeconds(2);
            TempSoundManager.PlaySound(alarmSound);
            yield return new WaitForSeconds(3);
            int _rand;
            do
            {
                _rand = Random.Range(1, 10);
            } while (activeRandomizers.Contains(_rand));
            activeRandomizers.Insert(0, _rand);
            activeRandomizers.RemoveAt(3);
            CheckAndApplyEffects();
            UpdateTextDisplay();
        }
    }*/

    void CheckAndApplyEffects()
    {
        if(activeRandomizers.Contains(1))
            PlayerSpeedModifier = 2.0f;
        else PlayerSpeedModifier = 1.0f;

        if(activeRandomizers.Contains(2))
            EnemySpeedModifier = 2.0f;
        else EnemySpeedModifier = 1.0f;

        if (activeRandomizers.Contains(3))
            InfiniteAmmo = true;
        else InfiniteAmmo = false;

        if (activeRandomizers.Contains(4))
            FireSpeedModifier = 2.0f;
        else FireSpeedModifier = 1.0f;

        if (activeRandomizers.Contains(5))
            FrictionModifier = 0.0f;
        else FrictionModifier = 1.0f;

        if (activeRandomizers.Contains(6))
            Physics.gravity = Gravity / 4.0f;
        else Physics.gravity = Gravity;

        if (activeRandomizers.Contains(7))
            PlayerDamageModifier = 4.0f;
        else PlayerDamageModifier = 1.0f;

        if (activeRandomizers.Contains(8))
            EnemyDamageModifier = 2.0f;
        else EnemyDamageModifier = 1.0f;

        if (activeRandomizers.Contains(9))
            PlayerHealthDrain = 1;
        else PlayerHealthDrain = 0;

        if (activeRandomizers.Contains(10))
            //ReducedVisibility = true;
            RenderSettings.fog = true;
        else //ReducedVisibility = false;
            RenderSettings.fog = false;

        if (activeRandomizers.Contains(11))
            EnemiesExplodeUponDeath = true;
        else EnemiesExplodeUponDeath = false;

        if (activeRandomizers.Contains(12))
            RainingBombs = true;
        else RainingBombs = false;
    }

    void UpdateTextDisplay()
    {
        activeRandomizersText.text = null;
        for(int i = 0; i < activeRandomizers.Count; i++)
        {
            switch (activeRandomizers[i])
            {
                case 1:
                    activeRandomizersText.text += "x2 Player Speed\n";
                    break;
                case 2:
                    activeRandomizersText.text += "x2 Enemy Speed\n";
                    break;
                case 3:
                    activeRandomizersText.text += "Infinite Ammo\n";
                    break;
                case 4:
                    activeRandomizersText.text += "x2 Firing Speed\n";
                    break;
                case 5:
                    activeRandomizersText.text += "No Friction\n";
                    break;
                case 6:
                    activeRandomizersText.text += "Low Gravity\n";
                    break;
                case 7:
                    activeRandomizersText.text += "Quad Damage\n";
                    break;
                case 8:
                    activeRandomizersText.text += "x2 Enemy Damage\n";
                    break;
                case 9:
                    activeRandomizersText.text += "Poison\n";
                    break;
                case 10:
                    activeRandomizersText.text += "Reduced Visibility\n";
                    break;
                case 11:
                    activeRandomizersText.text += "Enemies Explode on Death\n";
                    break;
                case 12:
                    activeRandomizersText.text += "Raining Bombs\n";
                    break;
                default:
                    activeRandomizersText.text += "\n";
                    break;
            }
        }
        selectingRandomizerText.text = activeRandomizersText.text.Split('\n')[0];
    }
}
