using UnityEngine;

public class AILaneJumperMover : IAILaneJumperMover
{
    private readonly IEnemyLaneSelector laneSelector;
    private readonly IEnemyDashSpeedProvider dashSpeedProvider;
    private readonly Transform[] lanes = new Transform[3];

    private Transform body;
    private AILaneJumperEnemySettings settings;
    private Vector3 dashStartPosition;
    private Vector3 dashTargetPosition;
    private int patrolLaneIndex = 1;
    private int patrolDirection = 1;

    public Transform SelectedLane { get; private set; }

    public AILaneJumperMover(IEnemyLaneSelector laneSelector, IEnemyDashSpeedProvider dashSpeedProvider)
    {
        this.laneSelector = laneSelector;
        this.dashSpeedProvider = dashSpeedProvider;
    }

    public void Configure(
        Transform body,
        Transform leftLane,
        Transform centerLane,
        Transform rightLane,
        AILaneJumperEnemySettings settings)
    {
        this.body = body;
        this.settings = settings;
        lanes[0] = leftLane;
        lanes[1] = centerLane;
        lanes[2] = rightLane;
    }

    public void TickPatrol(float deltaTime)
    {
        if (body == null || settings == null)
            return;

        Transform patrolLane = lanes[patrolLaneIndex];

        if (patrolLane == null)
            return;

        Vector3 targetPosition = new(patrolLane.position.x, body.position.y, body.position.z);

        body.position = Vector3.MoveTowards(
            body.position,
            targetPosition,
            settings.PatrolSpeed * deltaTime
        );

        if (Mathf.Abs(body.position.x - patrolLane.position.x) <= settings.LaneReachDistance)
            SelectNextPatrolLane();
    }

    public bool SelectClosestPlayerLane()
    {
        SelectedLane = laneSelector.SelectLane(lanes);
        return SelectedLane != null;
    }

    public void BeginDash()
    {
        if (body == null || SelectedLane == null)
            return;

        dashStartPosition = body.position;
        dashTargetPosition = new Vector3(
            SelectedLane.position.x,
            body.position.y,
            body.position.z
        );
    }

    public void TickDash(float deltaTime)
    {
        if (body == null || settings == null)
            return;

        float dashSpeed = dashSpeedProvider != null
            ? dashSpeedProvider.GetDashSpeed(settings)
            : settings.DashSpeed;

        body.position = Vector3.MoveTowards(
            body.position,
            dashTargetPosition,
            dashSpeed * deltaTime
        );

        float distance = Mathf.Max(0.001f, Vector3.Distance(dashStartPosition, dashTargetPosition));
        float traveled = Vector3.Distance(dashStartPosition, body.position);
        float normalizedTravel = Mathf.Clamp01(traveled / distance);
        float heightOffset = Mathf.Sin(normalizedTravel * Mathf.PI) * settings.JumpHeight;
        body.position = new Vector3(body.position.x, dashTargetPosition.y + heightOffset, body.position.z);
    }

    public bool HasReachedDashTarget()
    {
        if (body == null)
            return false;

        if (Vector3.SqrMagnitude(body.position - dashTargetPosition) > 0.0001f)
            return false;

        body.position = dashTargetPosition;
        return true;
    }

    private void SelectNextPatrolLane()
    {
        patrolLaneIndex += patrolDirection;

        if (patrolLaneIndex >= lanes.Length)
        {
            patrolLaneIndex = lanes.Length - 2;
            patrolDirection = -1;
            return;
        }

        if (patrolLaneIndex < 0)
        {
            patrolLaneIndex = 1;
            patrolDirection = 1;
        }
    }
}
