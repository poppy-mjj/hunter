using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    Image healthSlider;
    Image energySlider;
    public GameObject player;
    private CharacterStats characterStats;
    //public GameObject HealthHolder;

    private void Awake()
    {
        characterStats = player.GetComponent<CharacterStats>();
        healthSlider = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        if(player.CompareTag("Player"))
        energySlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();
    }
    private void Update()
    {
        if (characterStats.characterData != null)
        {
            UpdateHealth();
            if (!player)
                SceneManager.LoadScene("Start");
            else
                if (player.CompareTag("Player"))
                UpdateEnergy();
            
        }
    }

    void UpdateHealth()
    {
        float sliderPercent = (float)characterStats.CurrentHealth / characterStats.MaxHealth;
        healthSlider.fillAmount = sliderPercent;
    }
    void UpdateEnergy()
    {
        float sliderPercent = (float)characterStats.CurrentEnergy / characterStats.MaxEnergy;
        energySlider.fillAmount = sliderPercent;
    }
}
