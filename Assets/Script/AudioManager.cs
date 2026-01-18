using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("SFX Settings")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private List<AudioClip> sfxClips;

    private Dictionary<string, AudioClip> sfxLookup;

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Build lookup table
        sfxLookup = new Dictionary<string, AudioClip>();

        foreach (AudioClip clip in sfxClips)
        {
            if (clip == null) continue;

            if (!sfxLookup.ContainsKey(clip.name))
                sfxLookup.Add(clip.name, clip);
        }
    }

    // Custom volume
    public void PlaySFX(string clipName, float volume = 1)
    {
        if (!sfxLookup.ContainsKey(clipName))
        {
            Debug.LogWarning($"SFX not found: {clipName}");
            return;
        }

        volume = Mathf.Clamp01(volume);

        sfxSource.PlayOneShot(sfxLookup[clipName], volume);
    }
}
