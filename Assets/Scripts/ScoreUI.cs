using UnityEngine;
using TMPro;
using Unity.Profiling.LowLevel;
using System;
using JetBrains.Annotations;

public class ScoreUI : MonoBehaviour
{
    public static ScoreUI Instance;
    public float companyValue = 0;
    public TextMeshProUGUI valueText;

    [SerializeField] private AudioClip positiveValueSound;
    [SerializeField] private AudioClip negativeValueSound;


    private void Awake()
    {
        if (ScoreUI.Instance == null)
        {
            Instance = this;
            
        }
        else if (ScoreUI.Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (valueText != null) 
        { 
        valueText.text = "Company Value = " + companyValue.ToString("F2");

        }
    }
    public void UpdateScore(float change)
    {

        companyValue += change;
        UpdateUI();
    }
}
