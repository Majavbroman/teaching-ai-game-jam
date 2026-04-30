using TMPro;
using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using JetBrains.Annotations;

public class AiCenter : MonoBehaviour
{
    public static AiCenter Instance { get; private set; }

    private AiCenterData aiCenterData;

    [SerializeField] private GDPRUI _gdprUI;
    public bool GDPR { get
        {
            return aiCenterData.GDPR;
        }
    }

    [SerializeField] private Transform _sizeIndicator;
    private Vector3 _sizeIndicatorBase;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _sizeIndicatorBase = _sizeIndicator.localScale;
        SetData(CreateData());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        int childCount = other.transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Transform child = other.transform.GetChild(i);

            if (child.TryGetComponent(out Prompt prompt))
            {
                int severity = prompt.GetSeverity();
                severity += aiCenterData.GDPR ? 1 : 0;

                float severityScore = CalculateSeverityScore(severity);

                Debug.Log("Severity: " + severity + ", Score: " + severityScore.ToString("F2"));

                ScoreUI.Instance.UpdateScore(severityScore);
                UpdateSize(GetSizeChange(severity));
            }
        }

        Destroy(other.gameObject);
    }

    private void UpdateSize(Vector3 sizeChange)
    {
        transform.localScale += sizeChange;

        float size = transform.localScale.x;
        float endSize = aiCenterData.EndSize;
        float difference = size - endSize;
        Debug.Log("Current Size: " + transform.localScale.ToString("F2") + ", End Size: " + endSize.ToString("F2") + ", Difference: " + difference.ToString("F2"));
        if (difference > 0)
        {
            SetData(CreateData());
        }
    }

    private Vector3 GetSizeChange(int severity)
    {
        if (severity == 0)
        {
            return Vector3.one * 0.1f;

        }

        return Vector3.one * -(0.05f * severity);
    }

    private AiCenterData CreateData()
    {
        float sizeMult = UnityEngine.Random.Range(0.75f, 1.85f);
        float endDifference = UnityEngine.Random.Range(0.5f, 1f);
        float endSize = Math.Min(sizeMult + endDifference, 2.15f);
        float randomGDPR = UnityEngine.Random.Range(0, 100f);

        AiCenterData data = new AiCenterData
        {
            SizeMult = sizeMult,
            EndSize = endSize,
            GDPR = randomGDPR < 10f
        };
        return data;
    }

    private void SetData(AiCenterData aiCenterData)
    {
        this.aiCenterData = aiCenterData;
        transform.localScale = Vector2.one * aiCenterData.SizeMult;
        _sizeIndicator.localScale = _sizeIndicatorBase * aiCenterData.EndSize;
        _gdprUI.SetGDPRText(aiCenterData.GDPR);
        Debug.Log("New AiCenter Data - Size Mult: " + aiCenterData.SizeMult.ToString("F2") + ", End Size: " + aiCenterData.EndSize.ToString("F2") + ", GDPR: " + aiCenterData.GDPR);
    }
    
    private float CalculateSeverityScore(int severity)
    {
        return (-12.5f * severity) + 7.5f;
    }
}

public struct AiCenterData
{
    public float SizeMult;
    public float EndSize;
    public bool GDPR;
}

