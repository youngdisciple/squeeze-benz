using System.Collections.Generic;
using UnityEngine;

public class TrafficSpawner : MonoBehaviour
{
[SerializeField] Transform player;
[SerializeField] TrafficCar carPrefab;
[SerializeField] int maximumCars = 30;
[SerializeField] float forwardSpeed = 14;
[SerializeField] float lookAhead = 300;
readonly List<TrafficCar> active = new List<TrafficCar>();
readonly Queue<TrafficCar> pool = new Queue<TrafficCar>();
public IReadOnlyList<TrafficCar> ActiveCars => active;
public int TotalCount => active.Count + pool.Count;
System.Random random = new System.Random(42);
float nextRow = 100;
float nextOncoming = 200;
int corridor = 1;
int row;
    void FixedUpdate()
    {
    if (GameManager.Instance.IsRunning)
        nextRow += forwardSpeed * Time.fixedDeltaTime;
    }
    void LateUpdate()
    {
    if (!GameManager.Instance.IsRunning) return;
        float z = player.position.z;
        for (int i = active.Count - 1; i >= 0; i--)
            if (active[i].Body.position.z < z - 90 ||
                active[i].Body.position.z > z + 600)
            {
                active[i].gameObject.SetActive(false);
            pool.Enqueue(active[i]);
            active.RemoveAt(i);
            }
    int stage = GameManager.Instance.StageIndex;
    float density = stage == 0 ? 0.5f : stage == 1 ? 0.75f : 1;
    float spacing = stage == 0 ? 90 : stage == 1 ? 78 : 68;
    nextRow = Mathf.Max(nextRow, z + 85);
    while (nextRow < z + lookAhead)
    {
    int previous = corridor;
    bool transition = row % 3 == 0;
    if (transition) corridor = Mathf.Clamp(corridor + random.Next(-1, 2), 0, 2);
    for (int lane = 0; lane < 3; lane++)
    if (lane != corridor && (!transition || lane != previous) &&
    random.NextDouble() < density)
    Spawn(-2 + lane * 4, nextRow, forwardSpeed);
    row++;
    nextRow += spacing;
    }
    if (nextOncoming < z + lookAhead)
    {
    nextOncoming = Mathf.Max(nextOncoming, z + 180);
    Spawn(-6, nextOncoming, -20);
    nextOncoming += 150;
    }
    }
    void Spawn(float x, float z, float speed)
    {
    if (active.Count >= maximumCars) return;
    foreach (TrafficCar other in active)
    if (Mathf.Abs(other.Body.position.x - x) < 2 &&
    Mathf.Abs(other.Body.position.z - z) < 24) return;
    TrafficCar car = pool.Count > 0 ? pool.Dequeue() : Instantiate(carPrefab, transform);
    car.Place(new Vector3(x, 0.55f, z), speed);
    active.Add(car);
    }
}