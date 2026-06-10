using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : MonoBehaviour
{
    public enum NPCState { Patrol, Chase }

    [Header("이동 설정")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4.5f;
    public float detectionRange = 8f;
    public float catchRange = 1.2f;

    [Header("순찰 웨이포인트")]
    public Transform[] waypoints;

    private NavMeshAgent agent;
    private Transform player;
    private NPCState state = NPCState.Patrol;
    private int waypointIndex = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent.speed = patrolSpeed;

        if (waypoints.Length > 0)
            GoToNextWaypoint();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;
        if (player == null) return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        switch (state)
        {
            case NPCState.Patrol:
                HandlePatrol();
                if (distToPlayer <= detectionRange)
                    EnterChase();
                break;

            case NPCState.Chase:
                HandleChase();
                if (distToPlayer > detectionRange * 1.5f)
                    EnterPatrol();
                break;
        }

        // 잡기 판정
        if (distToPlayer <= catchRange)
            CatchPlayer();
    }

    // ─── 순찰 ───────────────────────────────────────────
    void HandlePatrol()
    {
        if (waypoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            GoToNextWaypoint();
    }

    void GoToNextWaypoint()
    {
        if (waypoints.Length == 0) return;
        agent.SetDestination(waypoints[waypointIndex].position);
        waypointIndex = (waypointIndex + 1) % waypoints.Length;
    }

    void EnterPatrol()
    {
        state = NPCState.Patrol;
        agent.speed = patrolSpeed;
        GoToNextWaypoint();
    }

    // ─── 추적 ───────────────────────────────────────────
    void HandleChase()
    {
        agent.SetDestination(player.position);
    }

    void EnterChase()
    {
        state = NPCState.Chase;
        agent.speed = chaseSpeed;
    }

    // ─── 게임 오버 ──────────────────────────────────────
    void CatchPlayer()
    {
        GameManager.Instance?.GameOver();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            CatchPlayer();
    }

    // 에디터에서 감지 범위 시각화
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, catchRange);
    }
}
