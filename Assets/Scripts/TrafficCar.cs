using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class TrafficCar : MonoBehaviour
{
public Rigidbody Body { get; private set; }
public bool Passed { get; set; }
public float ClosestLateral { get; set; }
float speed;
void Awake() { Body = GetComponent<Rigidbody>(); }
public void Place(Vector3 position, float metresPerSecond)
{
gameObject.SetActive(true);
Body.position = position;
Body.rotation = metresPerSecond < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
speed = metresPerSecond;
Passed = false;
ClosestLateral = float.PositiveInfinity;
}
void FixedUpdate()
{
if (GameManager.Instance.IsRunning)
Body.MovePosition(Body.position + Vector3.forward * speed * Time.fixedDeltaTime);
}
}