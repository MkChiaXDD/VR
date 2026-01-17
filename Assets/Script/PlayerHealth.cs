using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Hearts (Left ? Right)")]
    [SerializeField] private List<GameObject> hearts;

    private int currentHealth;

    void Start()
    {
        currentHealth = hearts.Count;
        UpdateHearts();
    }

    public void TakeDamage(int amount = 1)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, hearts.Count);

        UpdateHearts();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount = 1)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, hearts.Count);

        UpdateHearts();
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            // Enable hearts from left to right
            hearts[i].SetActive(i < currentHealth);
        }
    }

    void Die()
    {
        Debug.Log("Player died");

        // TODO:
        // - Show game over
        // - Restart scene
        // - Disable controls
    }
}
