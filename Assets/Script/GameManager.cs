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

    [Header("Win Game")]
    [SerializeField] private GameObject loseCanvas;
    [SerializeField] private TMP_Text timerText2;
    private float timer2; 

    private float timer;

    [Header("Play particle")]
    [SerializeField] private ExplodeParticlePool particle;

    void Start()
    {
        endCanvas.SetActive(false);
        loseCanvas.SetActive(false);
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

    public void LoseGame()
    {
        StopAllCoroutines();
        StartCoroutine(LoseGameRestart());
    }

    private IEnumerator LoseGameRestart()
    {
        loseCanvas.SetActive(true);
        playerUI.SetActive(false);
        timer2 = restartTime;

        while (timer2 > 0f)
        {
            timer2 -= Time.deltaTime;

            if (timerText2 != null)
            {
                timerText2.text = $"Restarting in {Mathf.Ceil(timer2)}";
            }

            yield return null; // IMPORTANT
        }

        SceneManager.LoadScene("HelloCardboard");
    }

    public void PlayParticle(Vector3 position)
    {
        GameObject newParticle = particle.GetObject();
        newParticle.transform.position = position;

        ReturnParticleLaterlol(newParticle);
    }

    private IEnumerator ReturnParticleLaterlol(GameObject particle)
    {
        yield return new WaitForSeconds(2f);

        this.particle.ReturnObject(particle);
    }
}
