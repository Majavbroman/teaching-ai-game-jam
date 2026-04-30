using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private bool _attacked = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Attack") || _attacked) return;

        _attacked = true;

        int childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);

            if (child.TryGetComponent(out Prompt prompt))
            {
                int severity = prompt.GetSeverity();
                severity += AiCenter.Instance.GDPR ? 1 : 0;

                float severityScore = CalculateSeverityScore(severity);

                Debug.Log("Severity: " + severity + ", Score: " + severityScore.ToString("F2"));

                ScoreUI.Instance.UpdateScore(severityScore);
            }
        }

        Destroy(gameObject);
    }

    private float CalculateSeverityScore(int severity)
    {
        if (severity == 0) return -7.5f;

        return 2.5f * severity;
    }
}
