using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Put this on a root GameObject named GameManager.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool IsPaused { get; private set; }
    public bool IsRunning => runActive && !IsPaused;
    public int StageIndex { get; private set; }
    public float MinimumSpeed { get; private set; } = 50f;
    public string PoliceStatus { get; set; } = "No patrol";
    public bool IsChasing { get; set; }
    public string Notice => Time.time < noticeUntil ? notice : "";

    public event Action Crashed;
    public event Action BonusAwarded;

    bool runActive;
    string notice;
    float noticeUntil;

    // Result - session properties
    public int FinalScore { get; private set; }
    public float FinalDistance { get; private set; }
    public float FinalTopSpeed { get; private set; }
    public int FinalNearMisses { get; private set; }
    public int FinalEscapes { get; private set; }
    public int MoneyEarned { get; private set; }
    public int Wallet { get; private set; }
    public int BestScore { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1;
        runActive = scene.name == "GameScene";
        IsPaused = false;
        StageIndex = 0;
        MinimumSpeed = 50;
        PoliceStatus = "No patrol";
        IsChasing = false;
    }

    void Update()
    {
        if (runActive && Input.GetKeyDown(KeyCode.Escape)) TogglePause();
    }

    public void TogglePause()
    {
        if (!runActive) return;
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0 : 1;
    }

    public void SetStage(int stage, float minimum)
    {
        if (stage != StageIndex) Notify("Section " + (stage + 1));
        StageIndex = stage;
        MinimumSpeed = minimum;
    }

    public void Notify(string text)
    {
        notice = text;
        noticeUntil = Time.time + 3;
        Debug.Log(text);
    }

    public void RewardNotice(string text)
    {
        Notify(text);
        BonusAwarded?.Invoke();
    }

    public void Crash()
    {
        if (!IsRunning) return;
            ScoreManager score = ScoreManager.Instance;
        if (score != null) score.SampleDistance();
            runActive = false; // Prevent a second crash from paying again.
        if (score != null)
        {
            FinalScore = score.Score;
            FinalDistance = score.Distance;
            FinalTopSpeed = score.TopSpeed;
            FinalNearMisses = score.NearMisses;
            FinalEscapes = score.Escapes;
        }
        else
        {
            FinalScore = FinalNearMisses = FinalEscapes = 0;
            FinalDistance = FinalTopSpeed = 0;
        }
        
        MoneyEarned = Mathf.FloorToInt(FinalScore / 20f);
        Wallet += MoneyEarned;
        BestScore = Mathf.Max(BestScore, FinalScore);
        Crashed?.Invoke();
        Time.timeScale = 1;
        SceneManager.LoadScene("EndScene");
    }

    [ContextMenu("RestartRun")]
    public void RestartRun()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("GameScene");
    }

    void OnDestroy()
    {
        if (Instance != this) return;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Instance = null;
        Time.timeScale = 1;
    }
}
