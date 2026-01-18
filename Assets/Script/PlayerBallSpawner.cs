using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBallSpawner : MonoBehaviour
{
    [Header("Pool")]
    [SerializeField] private BulletPool pool;

    [Header("Spawn Points")]
    [SerializeField] private List<Transform> spawnPoints;

    [Header("Timing")]
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private float ballLifetime = 5f;

    private HashSet<GameObject> activeBalls = new HashSet<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            TrySpawnBall();
        }
    }

    void TrySpawnBall()
    {
        // Only spawn if no active ball exists
        if (activeBalls.Count > 0)
            return;

        GameObject ball = pool.GetObject();

        Transform spawnPoint =
            spawnPoints[Random.Range(0, spawnPoints.Count)];

        ball.transform.position = spawnPoint.position;
        ball.transform.rotation = spawnPoint.rotation;

        activeBalls.Add(ball);

        StartCoroutine(BallLifetimeRoutine(ball));
    }


    IEnumerator BallLifetimeRoutine(GameObject ball)
    {
        float timer = 0f;

        while (timer < ballLifetime)
        {
            // If ball was picked up, stop lifetime countdown
            if (!activeBalls.Contains(ball))
                yield break;

            timer += Time.deltaTime;
            yield return null;
        }

        // Time expired, return to pool
        DespawnBall(ball);
    }

    // ================= CALLED BY PICKUP =================

    public void OnBallPickedUp(GameObject ball)
    {
        if (activeBalls.Contains(ball))
        {
            activeBalls.Remove(ball);
        }
    }

    public void DespawnBall(GameObject ball)
    {
        if (activeBalls.Contains(ball))
            activeBalls.Remove(ball);

        pool.ReturnObject(ball);
    }
}
