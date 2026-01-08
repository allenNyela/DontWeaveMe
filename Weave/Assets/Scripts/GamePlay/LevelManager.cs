using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

[System.Serializable]
public class LevelConfig
{
    public string sceneName;  // e.g. "Level 1 Night"
    public int flyQuota;      // how many flies must be eaten to clear this level
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Order & Quotas")]
    public LevelConfig[] levels;

    [Header("Events")]
    public UnityEvent OnFlyEaten;          // fires every time a fly is eaten
    public UnityEvent OnFlyQuotaReached;   // fires the first time fliesEaten >= flyQuota
    public UnityEvent OnAllLevelsCompleted;// fires when you finish the last level

    private int currentLevelIndex = 0;
    private int fliesEatenThisLevel = 0;
    private bool quotaReachedThisLevel = false;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i].sceneName == scene.name)
            {
                currentLevelIndex = i;
                ResetLevelFlyState();
                return;
            }
        }
    }

    private void ResetLevelFlyState()
    {
        fliesEatenThisLevel = 0;
        quotaReachedThisLevel = false;
    }


    public void NotifyFlyEaten()
    {
        fliesEatenThisLevel++;

        // event: any fly eaten
        //OnFlyEaten?.Invoke();

        //int quota = GetCurrentFlyQuota();
        //if (!quotaReachedThisLevel && quota > 0 && fliesEatenThisLevel >= quota)
        //{
        //    quotaReachedThisLevel = true;
        //    OnFlyQuotaReached?.Invoke();
        //}
    }

    public bool ReachQuota()
    {
        int quota = GetCurrentFlyQuota();
        return !quotaReachedThisLevel && quota > 0 && fliesEatenThisLevel >= quota;
    }


    public int GetCurrentFlyQuota()
    {
        if (levels == null || levels.Length == 0) return 0;
        if (currentLevelIndex < 0 || currentLevelIndex >= levels.Length) return 0;
        return levels[currentLevelIndex].flyQuota;
    }

    public int GetFliesEatenThisLevel()
    {
        return fliesEatenThisLevel;
    }

    public string GetCurrentSceneName()
    {
        if (levels == null || levels.Length == 0) return null;
        if (currentLevelIndex < 0 || currentLevelIndex >= levels.Length) return null;
        return levels[currentLevelIndex].sceneName;
    }

    public void LoadFirstLevel()
    {
        if (levels == null || levels.Length == 0)
        {
            Debug.LogWarning("LevelManager: No levels configured.");
            return;
        }

        currentLevelIndex = 0;
        SceneManager.LoadScene(levels[currentLevelIndex].sceneName);
    }

    public void LoadNextLevel()
    {
        int nextIndex = currentLevelIndex + 1;
        if (levels == null || nextIndex >= levels.Length)
        {
            Debug.Log("LevelManager: All levels completed.");
            OnAllLevelsCompleted?.Invoke();
            return;
        }

        currentLevelIndex = nextIndex;
        SceneManager.LoadScene(levels[currentLevelIndex].sceneName);
    }

    public void ReloadCurrentLevel()
    {
        if (levels == null || levels.Length == 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        if (currentLevelIndex < 0 || currentLevelIndex >= levels.Length)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        SceneManager.LoadScene(levels[currentLevelIndex].sceneName);
    }
}
