using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class CarController : MonoBehaviour
{
    [SerializeField] float topSpeedKph = 180;
    [SerializeField] float acceleration = 8;
    [SerializeField] float brakeStrength = 10;
    [SerializeField] float lateralSpeed = 6;
    [SerializeField] float grip = 5;

    public Rigidbody Body { get; private set; }
    public float SpeedKph => Mathf.Max(0, Body.linearVelocity.z) * 3.6f;
    public float Steering { get; private set; }
    public float Throttle { get; private set; }
    public bool Braking { get; private set; }

    void Awake()
    {
        Body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!GameManager.Instance.IsRunning) return;
        Throttle = (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) ? 1 : 0;
        Braking = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        Steering = Mathf.MoveTowards(Steering, Input.GetAxisRaw("Horizontal"), 5 * Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (!GameManager.Instance.IsRunning) return;
        float speed = Body.linearVelocity.z;
        float a = Braking ? -brakeStrength : Throttle > 0 ? acceleration : -1.2f;
        speed = Mathf.Clamp(speed + a * Time.fixedDeltaTime, 0, topSpeedKph / 3.6f);
        float desiredX = Steering * lateralSpeed * Mathf.Clamp01(speed / 12);
        float response = 1 - Mathf.Exp(-grip * Time.fixedDeltaTime);
        float vx = Mathf.Lerp(Body.linearVelocity.x, desiredX, response);
        Body.linearVelocity = new Vector3(vx, 0, speed);
        Body.MoveRotation(Quaternion.Euler(0, Mathf.Atan2(vx, Mathf.Max(12, speed)) * Mathf.Rad2Deg, 0));
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Hazard")) GameManager.Instance.Crash();
    }

    public void Configure(float topKph, float accel, float handling, float braking, float traction)
    {
        topSpeedKph = topKph;
        acceleration = accel;
        lateralSpeed = 5 + handling;
        brakeStrength = braking;
        grip = traction;
    }
}
