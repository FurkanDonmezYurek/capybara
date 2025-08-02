using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Serializable class to define named audio clips (for dynamic assignment in Inspector)
[System.Serializable]
public class NamedAudioClip
{
    public string name; // Unique identifier to play the sound (e.g., "Win", "Click")
    public AudioClip clip; // Actual AudioClip asset
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource; // Plays background music
    public AudioSource sfxSource; // Plays sound effects

    [Header("Music Clips")]
    public List<NamedAudioClip> musicClips; // List of background music options

    [Header("SFX Clips")]
    public List<NamedAudioClip> sfxClips; // List of sound effects

    private Dictionary<string, AudioClip> musicLibrary = new(); // Runtime music lookup
    private Dictionary<string, AudioClip> sfxLibrary = new(); // Runtime SFX lookup

    private const string MuteMusicKey = "MuteMusic"; // PlayerPrefs key for music mute
    private const string MuteSFXKey = "MuteSFX"; // PlayerPrefs key for SFX mute

    void Awake()
    {
        // Singleton pattern initialization
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist between scenes
            SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to scene loaded event

            BuildClipDictionaries(); // Convert lists to dictionaries
            LoadMuteSettings(); // Load saved mute settings
        }
        else
        {
            Destroy(gameObject); // Enforce singleton
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene Loaded: " + scene.name);

        switch (scene.name)
        {
            case "CoreLoop":
                AudioManager.Instance.PlayMusic("PuzzleTrack");
                break;
            case "Idle Scene":
                AudioManager.Instance.PlayMusic("IdleTrack");
                break;

        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe from scene loaded event
    }

    // Convert Inspector clip lists into fast-access dictionaries
    private void BuildClipDictionaries()
    {
        foreach (var entry in musicClips)
        {
            if (!musicLibrary.ContainsKey(entry.name) && entry.clip != null)
                musicLibrary.Add(entry.name, entry.clip);
        }

        foreach (var entry in sfxClips)
        {
            if (!sfxLibrary.ContainsKey(entry.name) && entry.clip != null)
                sfxLibrary.Add(entry.name, entry.clip);
        }
    }

    // Play music by name (loops automatically)
    public void PlayMusic(string name)
    {
        if (!musicLibrary.ContainsKey(name))
        {
            Debug.LogWarning("Music clip not found: " + name);
            return;
        }

        musicSource.clip = musicLibrary[name];
        musicSource.loop = true;
        musicSource.Play();
    }

    // Stop currently playing music
    public void StopMusic()
    {
        musicSource.Stop();
    }

    // Play one-shot SFX by name
    public void PlaySFX(string name)
    {
        if (!sfxLibrary.ContainsKey(name))
        {
            Debug.LogWarning("SFX clip not found: " + name);
            return;
        }

        sfxSource.PlayOneShot(sfxLibrary[name]);
    }

    // Toggle music mute and save preference
    public void MuteMusic(bool mute)
    {
        musicSource.mute = mute;
        PlayerPrefs.SetInt(MuteMusicKey, mute ? 1 : 0);
    }

    // Toggle SFX mute and save preference
    public void MuteSFX(bool mute)
    {
        sfxSource.mute = mute;
        PlayerPrefs.SetInt(MuteSFXKey, mute ? 1 : 0);
    }

    // Load previously saved mute states from PlayerPrefs
    private void LoadMuteSettings()
    {
        musicSource.mute = PlayerPrefs.GetInt(MuteMusicKey, 0) == 1;
        sfxSource.mute = PlayerPrefs.GetInt(MuteSFXKey, 0) == 1;
    }

    // Public getters for UI elements
    public bool IsMusicMuted() => musicSource.mute;

    public bool IsSFXMuted() => sfxSource.mute;
}
