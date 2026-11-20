using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class PoliceController : MonoBehaviour
{
    public enum PoliceState { Patrol, Chase, Escaped }

    [SerializeField] private CarController player;
    [SerializeField] private float speedLimit = 90f;
    [SerializeField] private float detectionRange = 80f;
    [SerializeField] private float escapeGap = 100f;
    [SerializeField] private float escapeSeconds = 3f;
    [SerializeField] private UnityEvent onEscape = new UnityEvent();

    [Header("Movement (m/s)")]
    [SerializeField] private float patrolSpeed = 17f;
    [SerializeField] private float maxChaseSpeed = 42f;   // ~151 km/h, below the player's 180 km/h top speed
    [SerializeField] private float catchUpBoost = 8f;     // how much faster than the player it tries to go
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float laneChangeSpeed = 6f;

    public PoliceState State { get; private set; }

    private Rigidbody body;
    private BoxCollider hitbox;
    private int trafficMask;
    private bool stoppedByTraffic;
    private float escapeTimer;
    private float resetTimer;
    private float currentSpeed;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        hitbox = GetComponent<BoxCollider>();
        trafficMask = LayerMask.GetMask("Traffic");
    }

    private void FixedUpdate()
    {
        GameManager game = GameManager.Instance;
        if (game == null || !game.IsRunning || player == null || player.Body == null) return;

        float gap = player.Body.position.z - body.position.z;
        float distance = Vector3.Distance(player.Body.position, body.position);

        if (State == PoliceState.Patrol)
        {
            if (player.SpeedKph > speedLimit && distance <= detectionRange)
            {
                State = PoliceState.Chase;
                escapeTimer = 0f;
                game.Notify("Police pursuit");
            }
            else if (gap > 120f || gap < -500f)
            {
                body.position = new Vector3(6f, 0.55f, player.Body.position.z + 180f);
                stoppedByTraffic = false;
            }
        }
        else if (State == PoliceState.Chase)
        {
            escapeTimer = gap > escapeGap ? escapeTimer + Time.fixedDeltaTime : 0f;

            
        if (escapeTimer >= escapeSeconds)
    {
        State = PoliceState.Escaped;
        resetTimer = 16f;
        game.RewardNotice("Police escaped");
        onEscape.Invoke();
    }

        }
        else
        {
            resetTimer -= Time.fixedDeltaTime;

            if (resetTimer <= 0f)
            {
                body.position = new Vector3(6f, 0.55f, player.Body.position.z + 180f);
                stoppedByTraffic = false;
                State = PoliceState.Patrol;
            }
        }

        game.IsChasing = State == PoliceState.Chase;
        game.PoliceStatus = State == PoliceState.Chase
            ? $"CHASE: lead {gap:0} m; escape {escapeTimer:0.0}/{escapeSeconds:0.0} s"
            : State == PoliceState.Patrol
                ? $"Patrol / limit {speedLimit:0} km/h"
                : "Escaped";

        if (stoppedByTraffic)
        {
            game.PoliceStatus += " / stopped by traffic";
            return;
        }

        // Chase: try to go a bit faster than the player (capped), so it visibly closes in.
        // If it gets ahead of the player, ease off so the player can retake the lead.
        float playerSpeed = Mathf.Max(0f, player.Body.linearVelocity.z);
        float targetSpeed = patrolSpeed;
        if (State == PoliceState.Chase)
        {
            targetSpeed = gap > 0f
                ? Mathf.Min(maxChaseSpeed, playerSpeed + catchUpBoost)
                : playerSpeed * 0.8f;
        }
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);

        Vector3 next = body.position + Vector3.forward * currentSpeed * Time.fixedDeltaTime;
        float targetX = State == PoliceState.Chase ? Mathf.Clamp(player.Body.position.x, -2.5f, 6.5f) : 6f;

        next.x = Mathf.MoveTowards(next.x, targetX, laneChangeSpeed * Time.fixedDeltaTime);
        next = StopAtTraffic(next);
        body.MovePosition(next);
    }

    private Vector3 StopAtTraffic(Vector3 next)
    {
        // Kinematic cars do not block MovePosition, so sweep the whole hitbox before moving.
        Vector3 scale = transform.lossyScale;
        Vector3 halfExtents = Vector3.Scale(hitbox.size * 0.5f,
            new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z)));
        Vector3 center = body.position + body.rotation * Vector3.Scale(hitbox.center, scale);

        foreach (Collider other in Physics.OverlapBox(center, halfExtents, body.rotation,
            trafficMask, QueryTriggerInteraction.Ignore))
        {
            if (!IsTraffic(other)) continue;
            StopForTraffic();
            return body.position;
        }

        Vector3 movement = next - body.position;
        float distance = movement.magnitude;
        if (distance <= Mathf.Epsilon) return next;

        float allowedDistance = distance;
        foreach (RaycastHit hit in Physics.BoxCastAll(center, halfExtents, movement / distance,
            body.rotation, distance, trafficMask, QueryTriggerInteraction.Ignore))
        {
            if (!IsTraffic(hit.collider)) continue;
            allowedDistance = Mathf.Min(allowedDistance, Mathf.Max(0f, hit.distance - 0.02f));
            StopForTraffic();
        }

        return body.position + movement.normalized * allowedDistance;
    }

    private bool IsTraffic(Collider other)
    {
        // Police also use the Traffic layer and Hazard tag; exclude every police car.
        return other.attachedRigidbody != body && other.CompareTag("Hazard")
            && other.GetComponentInParent<PoliceController>() == null;
    }

    private void StopForTraffic()
    {
        stoppedByTraffic = true;
        currentSpeed = 0f;
    }
}
