using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyFactory))]
public class EnemySpawner : MonoBehaviour
{
    private EnemyFactory _enemyFactory;

    [SerializeField] private float _startInterval = 3.5f;
    private float _enemyInterval;
    private float _intervalDecreaseRate = 0.01f;
    private float _minInterval = 2f;



    private void Awake()
    {
        _enemyFactory = GetComponent<EnemyFactory>();
    }

    void Start()
    {
        _enemyInterval = _startInterval;
        StartCoroutine(SpawnEnemy(_enemyInterval));
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

        _enemyFactory.CreateEnemy(_enemyFactory.GetRandomEnemy(), spawnPos);

        _enemyInterval = Mathf.Max(_minInterval, _enemyInterval - _intervalDecreaseRate);
        StartCoroutine(SpawnEnemy(_enemyInterval));
    }
}