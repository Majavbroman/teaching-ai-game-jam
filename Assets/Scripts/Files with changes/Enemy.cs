using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //ADDED CUSTOM ENEMY POOLS FOR EXTRA VARIETY IN THE GAME
    [SerializeField] private bool _useOnlyEnemyPools = false;
    [SerializeField] private PromptPool _enemyPool;
    public PromptPool EnemyPool => _enemyPool;
    public bool UseOnlyEnemyPools => _useOnlyEnemyPools;

    private bool _attacked = false;

    public Prompt Prompt { get; set; }

    //MOVED THE COLLISION DETECTION INTO ANOTHER SCRIPT TO FIX BUG
    public void TryAttack()
    {
        if (_attacked) return;

        _attacked = true;
        
        int severity = Prompt.GetSeverity();
        severity += AiCenter.Instance.GDPR ? 1 : 0;

        float severityScore = CalculateSeverityScore(severity);
        Debug.Log("Severity: " + severity + ", Score: " + severityScore.ToString("F2"));

        ScoreUI.Instance.UpdateScore(severityScore);

        Destroy(gameObject);
    }

    private float CalculateSeverityScore(int severity)
    {
        if (severity == 0) return -7.5f;

        return 2.5f * severity;
    }
}
