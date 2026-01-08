using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SFXEntry
{
    public SFXTypes type;
    public AudioClip clip;
}

[System.Serializable]
public class SceneMusicEntry
{
    public string sceneName;   // e.g. "level 2", "cutscene 5"
    public AudioClip musicClip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource voSource;

    [Header("Default Music")]
    public AudioClip menuMusic;
    public AudioClip beforeEatingMomMusic;
    public AudioClip eatingMomMusic;
    public AudioClip afterEatingMomMusic;

    [Header("Scene Music Mapping")]
    public List<SceneMusicEntry> sceneMusicEntries = new List<SceneMusicEntry>();
    private Dictionary<string, AudioClip> sceneMusicLookup;

    [Header("SFX Library")]
    public List<SFXEntry> sfxEntries = new List<SFXEntry>();
    private Dictionary<SFXTypes, AudioClip> sfxLookup;

    [Header("VO Library")]
    public List<AudioClip> voClips = new List<AudioClip>();
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SFXLookup();
        BuildSceneMusicLookup();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void SFXLookup()
    {
        sfxLookup = new Dictionary<SFXTypes, AudioClip>();
        foreach (var entry in sfxEntries)
        {
            if (!sfxLookup.ContainsKey(entry.type) && entry.clip != null)
            {
                sfxLookup.Add(entry.type, entry.clip);
            }
        }
    }

    void BuildSceneMusicLookup()
    {
        sceneMusicLookup = new Dictionary<string, AudioClip>();
        foreach (var entry in sceneMusicEntries)
        {
            if (!string.IsNullOrEmpty(entry.sceneName) && entry.musicClip != null)
            {
                sceneMusicLookup[entry.sceneName] = entry.musicClip;
            }
        }
    }

    private void Start()
    {
        if (menuMusic != null && musicSource != null && !musicSource.isPlaying)
        {
            PlayMusic(menuMusic, true);
        }
        HandleSceneMusic(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HandleSceneMusic(scene.name);
    }

    private void HandleSceneMusic(string sceneName)
    {
        if (musicSource == null || sceneMusicLookup == null) return;

        if (!sceneMusicLookup.TryGetValue(sceneName, out var newClip) || newClip == null)
        {
            return;
        }
        if (musicSource.clip == newClip)
        {
            if (!musicSource.isPlaying)
            {
                musicSource.loop = true;
                musicSource.Play();
            }
            return;
        }
        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null || musicSource == null) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void PlayMenuMusic()
    {
        if (menuMusic != null)
            PlayMusic(menuMusic, true);
    }

    public void PlayBeforeEatingMomMusic()
    {
        if (beforeEatingMomMusic != null)
            PlayMusic(beforeEatingMomMusic, true);
    }

    public void PlayEatingMomMusic()
    {
        if (eatingMomMusic != null)
            PlayMusic(eatingMomMusic, true);
    }

    public void PlayAfterEatingMomMusic()
    {
        if (afterEatingMomMusic != null)
            PlayMusic(afterEatingMomMusic, true);
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public AudioSource PlaySFX(SFXTypes type, float volume = 1f)
    {
        if (sfxSource == null || sfxLookup == null) return null;

        if (sfxLookup.TryGetValue(type, out var clip) && clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
            return sfxSource;
        }
        else
        {
            Debug.LogWarning($"AudioManager: No SFX clip found for {type}");
            return null;
        }
    }

    public void getRandomVOFromList()
    {
        
        if (voSource == null || voClips.Count == 0)
        {
            Debug.Log("No VO Clips available");
            return;
        } 
        Debug.Log("Playing VO Clip");
        int randomIndex = Random.Range(0, voClips.Count);
        AudioClip clip = voClips[randomIndex];
        voSource.PlayOneShot(clip);   
    }
}
