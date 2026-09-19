using System;
using UnityEngine;
using Unity.Mathematics;

namespace Solstice.Localization
{
    /// <summary>
    /// Maps between simulation coordinates and real-world latitude/longitude using a similarity
    /// transform (uniform scale + rotation + translation) linearized about a reference point.
    ///
    /// Create via Assets > Create > Solstice > World Frame, then enter two known correspondences.
    /// The fit is recomputed on load and whenever the asset is edited, so nothing goes stale.
    ///
    /// Latitude/longitude values are double2 ordered (latitude, longitude), matching
    /// WorldPoint.GetWorldPoint.
    /// </summary>
    [CreateAssetMenu(fileName = "WorldFrame", menuName = "Solstice/World Frame")]
    public class WorldFrame : ScriptableObject
    {
        [Serializable]
        private struct Correspondence
        {
            [Tooltip("Real-world latitude in degrees, from Google Earth")]
            public double latitude;

            [Tooltip("Real-world longitude in degrees, from Google Earth")]
            public double longitude;

            [Tooltip("Position of the same point in simulation space, on the ground plane")]
            public Vector2 simulationPoint;
        }

        [SerializeField] private Correspondence pointA;
        [SerializeField] private Correspondence pointB;

        // WGS84 ellipsoid parameters.
        private const double EquatorialRadius = 6378137.0;
        private const double EccentricitySquared = 0.00669437999014;

        // All derived. Nothing below here is serialized; it is rebuilt by Fit().
        private double2 _referenceLatLon;
        private double2 _coefficient;   // 'a' in w = a*z + b, as a complex number
        private double2 _offset;        // 'b', metres east/north
        private double _metresPerDegreeLatitude;
        private double _metresPerDegreeLongitude;
        private bool _isValid;

        /// <summary>False if the two correspondences are degenerate; conversions will return garbage.</summary>
        public bool IsValid => _isValid;

        /// <summary>Real-world metres per simulation unit.</summary>
        public double Scale => math.length(_coefficient);

        /// <summary>Compass bearing in degrees clockwise from north of the simulation +X axis.</summary>
        public double SimulationXAxisBearingDegrees
        {
            get
            {
                double degrees = math.degrees(math.atan2(_coefficient.x, _coefficient.y));
                return degrees < 0.0 ? degrees + 360.0 : degrees;
            }
        }

        private void OnEnable() => Fit();
        private void OnValidate() => Fit();

        /// <summary>Maps a simulation point to (latitude, longitude) in degrees.</summary>
        public double2 SimulationPointToWorldPoint(Vector2 simulationPoint)
        {
            double2 z = new double2(simulationPoint.x, simulationPoint.y);
            double2 w = ComplexMultiply(_coefficient, z) + _offset;
            return LocalMetresToLatLon(w);
        }

        /// <summary>Maps (latitude, longitude) in degrees back to a simulation point.</summary>
        public Vector2 WorldPointToSimulationPoint(double2 worldPosition)
        {
            double2 w = LatLonToLocalMetres(worldPosition) - _offset;
            double2 z = ComplexDivide(w, _coefficient);
            return new Vector2((float)z.x, (float)z.y);
        }

        /// <summary>
        /// Distance in metres between where this frame places a simulation point and where it
        /// actually is. Zero by construction for the two fitted points; use a third to validate.
        /// </summary>
        public double ResidualMetres(double latitude, double longitude, Vector2 simulationPoint)
        {
            double2 predicted = LatLonToLocalMetres(SimulationPointToWorldPoint(simulationPoint));
            double2 actual = LatLonToLocalMetres(new double2(latitude, longitude));
            return math.length(predicted - actual);
        }

        /// <summary>
        /// Projects a simulation-space position onto the ground plane. Unity is Y-up, so the
        /// ground plane is XZ; change this if your simulation works in XY.
        /// </summary>
        public static Vector2 Flatten(Vector3 position)
        {
            return new Vector2(position.x, position.z);
        }

        private void Fit()
        {
            _isValid = false;

            // Reference the tangent plane at the midpoint, so linearization error is smallest
            // where the points actually are.
            _referenceLatLon = new double2(
                0.5 * (pointA.latitude + pointB.latitude),
                0.5 * (pointA.longitude + pointB.longitude));

            // Radii of curvature at the reference latitude. The meridional radius sets the
            // north-south scale, the prime-vertical radius (times cos(lat)) the east-west scale.
            double latitudeRadians = math.radians(_referenceLatLon.x);
            double sinLatitude = math.sin(latitudeRadians);
            double denominator = 1.0 - EccentricitySquared * sinLatitude * sinLatitude;

            double meridionalRadius =
                EquatorialRadius * (1.0 - EccentricitySquared) / (denominator * math.sqrt(denominator));
            double primeVerticalRadius = EquatorialRadius / math.sqrt(denominator);

            double metresPerRadian = math.radians(1.0);
            _metresPerDegreeLatitude = meridionalRadius * metresPerRadian;
            _metresPerDegreeLongitude = primeVerticalRadius * math.cos(latitudeRadians) * metresPerRadian;

            double2 wA = LatLonToLocalMetres(new double2(pointA.latitude, pointA.longitude));
            double2 wB = LatLonToLocalMetres(new double2(pointB.latitude, pointB.longitude));

            double2 zA = new double2(pointA.simulationPoint.x, pointA.simulationPoint.y);
            double2 zB = new double2(pointB.simulationPoint.x, pointB.simulationPoint.y);

            double2 simulationBaseline = zB - zA;
            double2 worldBaseline = wB - wA;

            // Bail quietly rather than throwing: OnValidate fires while fields are half-typed.
            if (math.lengthsq(simulationBaseline) < 1e-12 || math.lengthsq(worldBaseline) < 1e-12)
            {
                _coefficient = new double2(1.0, 0.0);
                _offset = double2.zero;
                return;
            }

            _coefficient = ComplexDivide(worldBaseline, simulationBaseline);
            _offset = wA - ComplexMultiply(_coefficient, zA);
            _isValid = true;
        }

        private double2 LatLonToLocalMetres(double2 latLon)
        {
            return new double2(
                (latLon.y - _referenceLatLon.y) * _metresPerDegreeLongitude,  // east
                (latLon.x - _referenceLatLon.x) * _metresPerDegreeLatitude);  // north
        }

        private double2 LocalMetresToLatLon(double2 localMetres)
        {
            return new double2(
                _referenceLatLon.x + localMetres.y / _metresPerDegreeLatitude,
                _referenceLatLon.y + localMetres.x / _metresPerDegreeLongitude);
        }

        private static double2 ComplexMultiply(double2 p, double2 q)
        {
            return new double2(
                p.x * q.x - p.y * q.y,
                p.x * q.y + p.y * q.x);
        }

        private static double2 ComplexDivide(double2 p, double2 q)
        {
            double denominator = q.x * q.x + q.y * q.y;
            return new double2(
                (p.x * q.x + p.y * q.y) / denominator,
                (p.y * q.x - p.x * q.y) / denominator);
        }
    }
}