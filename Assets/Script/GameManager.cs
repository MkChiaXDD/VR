using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("End Game")]
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject endCanvas;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private float restartTime = 5f;

    private float timer;

    void Start()
    {
        endCanvas.SetActive(false);
    }

    public void EndGame()
    {
        StopAllCoroutines();
        StartCoroutine(EndRestartGame());
    }

    private IEnumerator EndRestartGame()
    {
        endCanvas.SetActive(true);
        playerUI.SetActive(false);
        timer = restartTime;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;

            if (timerText != null)
            {
                timerText.text = $"Restarting in {Mathf.Ceil(timer)}";
            }

            yield return null; // IMPORTANT
        }

        SceneManager.LoadScene("HelloCardboard");
    }
}
