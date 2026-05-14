using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyFactory))]
public class EnemySpawner : MonoBehaviour
{
    private EnemyFactory _enemyFactory;

    [SerializeField] private float _startInterval = 3.5f;
    private float _enemyInterval;
    private float _intervalDecreaseRate = 0.01f;
    private float _minInterval = 3f;

    //ADDED PROMPT POOL FOR PROMPTS THAT CAN BE SPAWNED IN A GIVEN WAVE
    [SerializeField] private PromptPool _poolsToUse;

    //ADDED SETTINGS FOR WHEN THE AI CENTER IS COMPLETED TO CHANGE THE PROMPT POOLS USED AT GIVEN POINTS IN THE GAME
    [System.Serializable]
    private struct CenterCompleteSettings
    {
        public int CentersCompleted;
        public PromptPool PoolsToUse;
    }

    private int _centersCompleted = 0;
    [Space(15)]
    [SerializeField] private CenterCompleteSettings[] _centerCompleteSettings;
    private readonly Dictionary<int, PromptPool> _centerCompleteSettingsDict = new Dictionary<int, PromptPool>();

    private void Awake()
    {
        _enemyFactory = GetComponent<EnemyFactory>();

        foreach (var setting in _centerCompleteSettings)
        {
            _centerCompleteSettingsDict[setting.CentersCompleted] = setting.PoolsToUse;
        }

        _centerCompleteSettings = null;
    }

    void Start()
    {
        _enemyInterval = _startInterval;
        StartCoroutine(SpawnEnemy(_enemyInterval));
    }

    private void OnEnable()
    {
        AiCenter.OnAICenterCompleted += HandleAICenterCompleted;
    }

    private void OnDisable()
    {
        AiCenter.OnAICenterCompleted -= HandleAICenterCompleted;
    }

    //HANDLES THE AI CENTER COMPLETED EVENT TO DECREASE THE INTERVAL AND CHANGE THE PROMPT 
    //POOL BASED ON HOW MANY TIMES THE CENTER HAS BEEN COMPLETED
    private void HandleAICenterCompleted()
    {
        _centersCompleted++;
        _minInterval = Mathf.Max(1.5f, _minInterval - (_centersCompleted * 0.1f));

        if (_centerCompleteSettingsDict.TryGetValue(_centersCompleted, out var poolsToUse))
        {
            _poolsToUse = poolsToUse;
        }
    }

    private IEnumerator SpawnEnemy(float interval)
    {
        yield return new WaitForSeconds(interval);

        Camera cam = Camera.main;

        // Pick a side: 0 = left, 1 = right, 2 = top, 3 = bottom
        int side = Random.Range(0, 2);
        float offset = 0.05f; // how far outside the screen


        float x = 0f;
        float y = 0f;

        switch (side)
        {
            /*case 0: // Left
                x = 0f - offset;
                y = Random.Range(0f, 1f);
                break;

            case 1: // Right
                x = 1f+ offset;
                y = Random.Range(0f, 1f);
                break;*/

            case 0: // Top
                x = Random.Range(0f, 1f);
                y = 1f+ offset;
                break;

            case 1: // Bottom
                x = Random.Range(0f, 1f);
                y = 0f- offset;
                break;
        }

        // Convert viewport → world
        Vector3 spawnPos = cam.ViewportToWorldPoint(
            new Vector3(x, y, cam.nearClipPlane + 5f)
        );

        _enemyFactory.CreateEnemy(_enemyFactory.GetRandomEnemy(), spawnPos, _poolsToUse);

        _enemyInterval = Mathf.Max(_minInterval, _enemyInterval - _intervalDecreaseRate);
        StartCoroutine(SpawnEnemy(_enemyInterval));
    }
}