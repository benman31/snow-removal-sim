// Author: Benjamin Enman
// Line rendering code based on the following tutorial: https://youtu.be/8mGZBYsSXcQ
using System;
using UnityEngine;


public class SnowblowerTrajectory : MonoBehaviour
{
    [SerializeField] private LineRenderer LineRenderer;
    [SerializeField] private Transform ReleasePosition;
    [SerializeField] private Transform TargetPosition;
    [SerializeField] private float Strength;
    [SerializeField] private float Weight;
    [SerializeField] private ParticleSystem SnowStreamPS;

    private LayerMask SnowCollisionMask;
    public static event Action<Vector3> OnGroundCollision;

    private bool SpoutHasMoved = false;

    [Header("Display Controls")]
    [SerializeField]
    [Range(10, 100)]
    private int LinePoints;
    [SerializeField]
    [Range(0.01f, 0.25f)]
    private float TimeBetweenPoints = 0.1f;


    private void Start()
    {
        SnowBlowerController.OnClearingSnow += HandleSnowBeingCleared;
        SnowBlowerInput.OnSpoutRotation += HandleSpoutMoved;
    }

    private void OnDestroy()
    {
        SnowBlowerController.OnClearingSnow -= HandleSnowBeingCleared;
        SnowBlowerInput.OnSpoutRotation -= HandleSpoutMoved;
    }

    void Awake()
    {
        SnowCollisionMask = LayerMask.GetMask("Ground");
    }


    void Update()
    {
        DrawProjection();
    }

    private void DrawProjection()
    {
        LineRenderer.enabled = SpoutHasMoved;
        TargetPosition.gameObject.SetActive(SpoutHasMoved);
        LineRenderer.positionCount = Mathf.CeilToInt(LinePoints / TimeBetweenPoints) + 2;
        Vector3 startPos = ReleasePosition.position;
        Vector3 startVelocity = SnowStreamPS.transform.forward * Strength / Weight;
        int i = 0;
        LineRenderer.SetPosition(i, startPos);
        for (float time = 0; time < LinePoints; time += TimeBetweenPoints)
        {
            i++;
            Vector3 point = startPos + time * startVelocity;
            // Kinematic equation
            point.y = startPos.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);
            LineRenderer.SetPosition(i, point);

            Vector3 lastPosition = LineRenderer.GetPosition(i - 1);

            if (Physics.Raycast(lastPosition, (point - lastPosition).normalized, out RaycastHit hit, (point - lastPosition).magnitude, SnowCollisionMask))
            {
                LineRenderer.SetPosition(i, hit.point);
                LineRenderer.positionCount = i + 1;

                TargetPosition.position = hit.point;
                HandleGroundCollision(hit);
                return;
            }
        }
    }

    // Tell the player tools script that our snow trajectory has made contact
    private void HandleGroundCollision(RaycastHit hit)
    {
        if (SnowStreamPS.isPlaying)
        {
            OnGroundCollision?.Invoke(hit.point);
        }
        
    }

    private void HandleSnowBeingCleared(bool isClearingSnow)
    {
        if (!isClearingSnow && SnowStreamPS.isPlaying)
        {
            SnowStreamPS.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        else if (isClearingSnow && SnowStreamPS.isStopped)
        {
            SnowStreamPS.Play();
        }
    }

    private void HandleSpoutMoved(bool hasMoved)
    {
        SpoutHasMoved = hasMoved;
    }
}
