using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBallSpawner : MonoBehaviour
{
    [Header("Pool")]
    [SerializeField] private BallPool ballPool; // or your specific BallPool

    [Header("Spawn Points")]
    [SerializeField] private List<Transform> spawnPoints;

    [Header("Timing")]
    [SerializeField] private float spawnInterval = 5f;

    private GameObject currentBall;
    private Coroutine spawnRoutine;

    void Start()
    {
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnBall();

            yield return new WaitForSeconds(spawnInterval);

            // If still not picked up, swap it
            if (currentBall != null && currentBall.activeSelf)
            {
                ballPool.ReturnObject(currentBall);
                currentBall = null;
            }
        }
    }

    void SpawnBall()
    {
        if (spawnPoints.Count == 0) return;

        int index = Random.Range(0, spawnPoints.Count);
        Transform spawnPoint = spawnPoints[index];

        currentBall = ballPool.GetObject();
        currentBall.transform.position = spawnPoint.position;
        currentBall.transform.rotation = spawnPoint.rotation;
    }

    // ================= CALLED BY BALL =================

    public void OnBallPickedUp(GameObject ball)
    {
        if (ball != currentBall) return;

        currentBall = null;
    }

    public void OnBallFinished(GameObject ball)
    {
        if (ball != currentBall) return;

        currentBall = null;
    }
}
