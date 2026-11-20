using UnityEngine;
public class ScoreManager : MonoBehaviour
{
public static ScoreManager Instance { get; private set; }
[SerializeField] CarController player;
public int Score => Mathf.FloorToInt(points);
public float Distance { get; private set; }
public float TopSpeed { get; private set; }
public float Multiplier { get; private set; } = 1;
public int NearMisses { get; private set; }
public int Escapes { get; private set; }
float points;
float previousZ;
void Awake()
{
if (Instance != null && Instance != this) { Destroy(this); return; }
Instance = this;
}
void Start() { previousZ = player.Body.position.z; }
void Update()
{
if (!GameManager.Instance.IsRunning) return;
SampleDistance();
}
public void SampleDistance()
{
float z = player.Body.position.z;
float metres = Mathf.Max(0, z - previousZ);
previousZ = z;
Distance += metres;
TopSpeed = Mathf.Max(TopSpeed, player.SpeedKph);
Multiplier = player.SpeedKph < GameManager.Instance.MinimumSpeed ? 1 :
Mathf.Min(5, 1 + player.SpeedKph / 50);
points += metres * Multiplier;
}
public void AddNearMiss()
{
if (!GameManager.Instance.IsRunning) return;
NearMisses++;
points += 75;
GameManager.Instance.RewardNotice("Close call +75");
}
public void AddPoliceEscape()
{
if (!GameManager.Instance.IsRunning) return;
Escapes++;
points += 500;
GameManager.Instance.RewardNotice("Police escaped +500");
}
void OnDestroy() { if (Instance == this) Instance = null; }
}