using UnityEngine;

namespace Solstice.Dynamics
{
    // One raycast suspension wheel: casts straight down from `mount` to find the
    // ground, then resists compression with a spring-damper force applied at the
    // contact point. Not a ForceModel — it needs a world-space application point
    // (for the roll/pitch torque that keeps the chassis level) and per-wheel state
    // (previous compression, for the damper term) that a shared ScriptableObject
    // asset can't safely hold.
    //
    // Defaults below are sized for a ~350kg vehicle (~87.5kg/corner) targeting
    // ~30% static sag and a damping ratio around 0.7 — see Player.cs for the
    // derivation if you need to retune for a different mass or ride feel.
    [System.Serializable]
    public class Wheel
    {
        [SerializeField] private Transform mount;
        [SerializeField] private float wheelRadius = 0.15f;
        [SerializeField] private float restLength = 0.35f;
        [SerializeField] private float maxTravel = 0.08f;
        [SerializeField] private float springStiffness = 36000f; // k, N/m
        [SerializeField] private float damperStiffness = 2500f;  // c, N/(m/s)
        [SerializeField] private LayerMask groundMask = ~0;

        [Header("Gizmos")]
        [SerializeField] private float forceGizmoScale = 0.0005f; // meters drawn per Newton

        private float _previousCompression;
        private bool _wasGrounded;
        private Vector3 _lastPoint;
        private Vector3 _lastForce;

        // Computes this step's spring-damper force and where to apply it. Returns
        // false when the wheel is airborne (nothing to push against), and resets
        // damper state so landing doesn't produce a bogus velocity spike.
        //
        // Direction comes from `chassis`, not `mount`, deliberately: `mount` is
        // typically the wheel's own visual mesh transform, which can carry an
        // arbitrary local rotation (steering pivot, modeling convenience, etc.)
        // that has nothing to do with the suspension's travel axis. Only its
        // position is used as the ray origin.
        public bool TryComputeForce(Transform chassis, float deltaTime, out Vector3 force, out Vector3 point)
        {
            Vector3 origin = mount.position;
            Vector3 suspensionDir = chassis.up;
            float maxRayDistance = restLength + maxTravel + wheelRadius;

            if (!Physics.Raycast(origin, -suspensionDir, out RaycastHit hit, maxRayDistance, groundMask, QueryTriggerInteraction.Ignore))
            {
                _previousCompression = 0f;
                _wasGrounded = false;
                force = Vector3.zero;
                point = origin;
                _lastPoint = point;
                _lastForce = force;
                return false;
            }

            float groundDistance = hit.distance - wheelRadius;
            float compression = Mathf.Clamp(restLength - groundDistance, 0f, maxTravel);
            // Note the sign: compression *decreases* as the chassis rises, so its
            // raw derivative points the opposite way from the chassis's actual
            // vertical velocity. Flipping it here is what makes -c*compressionVel
            // an actual brake on chassis motion instead of a boost — get this
            // backwards and the damper adds energy every rebound instead of
            // removing it, and the bounce grows without bound.
            float compressionVel = _wasGrounded ? (_previousCompression - compression) / deltaTime : 0f;

            force = suspensionDir * (springStiffness * compression - damperStiffness * compressionVel);
            point = hit.point;

            _previousCompression = compression;
            _wasGrounded = true;
            _lastPoint = point;
            _lastForce = force;
            return true;
        }

        // Editor-only visualization, called from Player.OnDrawGizmosSelected. Safe
        // to call in edit mode (before Play has ever run TryComputeForce) — it
        // just falls back to showing the ray's full travel range.
        public void DrawGizmos(Transform chassis)
        {
            if (mount == null || chassis == null) return;

            Vector3 origin = mount.position;
            Vector3 suspensionDir = chassis.up;
            float maxRayDistance = restLength + maxTravel + wheelRadius;

            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(origin, 0.02f);

            // Full ray: green→red with compression while grounded, dim when airborne.
            Gizmos.color = _wasGrounded
                ? Color.Lerp(Color.green, Color.red, _previousCompression / Mathf.Max(maxTravel, 0.0001f))
                : new Color(1f, 1f, 1f, 0.3f);
            Gizmos.DrawLine(origin, origin - suspensionDir * maxRayDistance);

            // Rest length marker, so you can see how much travel is left either way.
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(origin - suspensionDir * restLength, 0.02f);

            if (!_wasGrounded) return;

            // Tire footprint at the actual contact point, and the force applied there.
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_lastPoint, wheelRadius);

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(_lastPoint, _lastPoint + _lastForce * forceGizmoScale);
        }
    }
}