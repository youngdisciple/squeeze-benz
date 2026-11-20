using UnityEngine;
public class NearMissDetector : MonoBehaviour
{
[SerializeField] CarController player;
[SerializeField] TrafficSpawner traffic;
void Update()
{
if (!GameManager.Instance.IsRunning) return;
foreach (TrafficCar car in traffic.ActiveCars)
{
if (car.Passed) continue;
float gap = car.Body.position.z - player.Body.position.z;
if (Mathf.Abs(gap) < 5.5f)
car.ClosestLateral = Mathf.Min(car.ClosestLateral,
Mathf.Abs(car.Body.position.x - player.Body.position.x));

if (gap < -5.5f)
{
car.Passed = true;
if (car.ClosestLateral > 1.9f && car.ClosestLateral < 2.9f &&
player.SpeedKph > 45)
ScoreManager.Instance.AddNearMiss();
}
}
}
}