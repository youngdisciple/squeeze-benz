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


    //garage - upgrades
    
    readonly bool[] owned = { true, false };
    readonly bool[] upgraded = { false, false };
    public int SelectedCar { get; private set; }
    public int CarCount => cars.Length;
    public CarProfile GetCar(int i) => cars[i];
    public bool Owns(int i) => owned[i];
    public bool HasUpgrade(int i) => upgraded[i];
    public bool CanBuy(int i) => !owned[i] && Wallet >= cars[i].price;
    public bool CanUpgrade(int i) => owned[i] && !upgraded[i] && Wallet >= 100;


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

    [SerializeField] CarProfile[] cars =
    {
        new CarProfile { name = "Rustbucket", price = 0,
        speed = 4, acceleration = 4, handling = 1, braking = 2, traction = 2 },
        new CarProfile { name = "Bodega", price = 180,
        speed = 2, acceleration = 2, handling = 5, braking = 2, traction = 2 }
    };

    public void BuyCar(int i)
    {
        if (!CanBuy(i)) { Notify("Already owned or insufficient funds."); return; }
            Wallet -= cars[i].price;
            owned[i] = true;
            Notify("Purchased " + cars[i].name);
    }
    public void SelectCar(int i)
    {
        if (!owned[i]) { Notify("Buy the car first."); return; }
            SelectedCar = i;
            Notify("Selected " + cars[i].name);
    }
    public void UpgradeCar(int i)
    {
        if (!CanUpgrade(i)) { Notify("Upgrade unavailable or insufficient funds."); return; }
            Wallet -= 100;
            upgraded[i] = true;
            Notify("Acceleration upgraded by 20%.");
    }
    public void ApplySelectedCar(CarController player)
    {
        CarProfile car = cars[SelectedCar];
        float acceleration = (3 + car.acceleration * 1.5f) *
        (upgraded[SelectedCar] ? 1.2f : 1);
        player.Configure(110 + car.speed * 24, acceleration,
        car.handling, 6 + car.braking * 2, 2 + car.traction * 1.1f);
    }
}
