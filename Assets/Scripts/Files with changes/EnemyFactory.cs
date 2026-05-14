using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    [SerializeField] private GameObject[] _enemyPrefabs;
    [SerializeField] private Prompt _promptPrefab;

    private Enemy[] _enemies;

    private void Awake()
    {
        PromptGetter.LoadPromptData();

        LoadEnemies();
    }

    //NOW LOADS PREFABS AS ENEMY COMPONENTS INSTEAD OF JUST GAMEOBJECTS TO AVOID LOOKING FOR THE ENEMY COMPONENT LATER ON.
    private void LoadEnemies()
    {
        List<Enemy> enemyList = new List<Enemy>();

        foreach (GameObject prefab in _enemyPrefabs)
        {
            if (!prefab.TryGetComponent(out Enemy enemy)) continue;

            enemyList.Add(enemy);
        }

        _enemies = enemyList.ToArray();
        _enemyPrefabs = null;
    }

    public Enemy GetRandomEnemy()
    {
        int index = Random.Range(0, _enemies.Length);
        return _enemies[index];
    }

    //MOVED PROMPT GETTING TO ANOTHER SCRIPT AND ADDED COMPATIBILITY WITH NEW IMPLEMENTATION OF PROMPTS
    public void CreateEnemy(Enemy enemy, Vector3 position, PromptPool spawnerPool)
    {
        Enemy enemyInstance = Instantiate(enemy, position, Quaternion.identity);
        Prompt prompt = Instantiate(_promptPrefab);

        PromptData promptData;

        promptData = GetPromptData(spawnerPool, enemyInstance.EnemyPool, enemyInstance.UseOnlyEnemyPools);
        prompt.SetData(promptData);
        prompt.transform.parent = enemyInstance.transform;
        prompt.transform.localPosition = new Vector3(0.05f, 1.25f, 0);
        enemyInstance.Prompt = prompt;
    }

    private PromptData GetPromptData(PromptPool spawnerPool, PromptPool enemyPool, bool useOnlyEnemyPools)
    {
        if (useOnlyEnemyPools)
        {
            return enemyPool.GetRandomPromptData();
        }
        
        PromptPool combinedPool = spawnerPool | enemyPool;
        return combinedPool.GetRandomPromptData();
    }
}