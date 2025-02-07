using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningTrackingApp.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace RunningTrackingApp.Helpers
{
    /// <summary>
    /// Implement the 3D Kalman filter using a constant acceleration model.
    /// This is an adaptation of the code found here: https://www.emgu.com/wiki/index.php/Kalman_Filter 
    /// extended to 3 dimensions and non-constant velocity.
    /// Note that the constant acceleration model may not be optimum, so further testing required.
    /// </summary>
    public class KalmanFilter3D : IGpsFilter
    {
        private Matrix<double> state; // [x, y, z, vx, vy, vz, ax, ay, az]
        private Matrix<double> covariance;
        private readonly Matrix<double> transition;
        private readonly Matrix<double> observation;
        private readonly Matrix<double> processNoise;
        private readonly Matrix<double> measurementNoise;


       
        public KalmanFilter3D(double initialX, double initialY, double initialZ, double dt = 1)
        {
            // Define the initial state vector: [x, y, z, vx, vy, vz, ax, ay, az] (Assume zero initial velocity and acceleration)
            state = DenseMatrix.OfArray(new double[,] { 
                { initialX }, { initialY }, { initialZ }, 
                { 0 }, { 0 }, { 0 }, 
                { 0 }, { 0 }, { 0 } });

            // Initial covariance matrix (uncertainty)
            covariance = DenseMatrix.CreateIdentity(9) * 1000;

            // State transition matrix (Assuming constant velocity model)
            // dt term allows for cases where the timestep between datapoints is not 1 second
            // New position depends on old position, velocity and acceleration
            // New velocity depends on old velocity and acceleration
            // New acceleration remains unchanged.
            transition = DenseMatrix.OfArray(new double[,]
            {
                {1, 0, 0, dt, 0, 0, 0.5*dt*dt, 0, 0 }, // x' = x + vx + 0.5*ax
                {0, 1, 0, 0, dt, 0, 0, 0.5*dt*dt, 0 }, // y' = y + vy + 0.5*ay
                {0, 0, 1, 0, 0, dt, 0, 0, 0.5*dt*dt }, // z' = z + vz + 0.5*az
                {0, 0, 0, 1, 0, 0, dt, 0, 0 }, // vx' = vx + ax
                {0, 0, 0, 0, 1, 0, 0, dt, 0 }, // vy' = vy + ay
                {0, 0, 0, 0, 0, 1, 0, 0, dt }, // vz' = vz + az
                {0, 0, 0, 0, 0, 0, 1, 0, 0 }, // ax' = ax
                {0, 0, 0, 0, 0, 0, 0, 1, 0 }, // ay' = ay
                {0, 0, 0, 0, 0, 0, 0, 0, 1 }  // az' = az
            });

            // Observation matrix (We only measure position)
            observation = DenseMatrix.OfArray(new double[,]
            {
                {1, 0, 0, 0, 0, 0, 0, 0, 0 },
                {0, 1, 0, 0, 0, 0, 0, 0, 0 },
                {0, 0, 1, 0, 0, 0, 0, 0, 0 }
            });

            // Process noise (model uncertainty)
            // Altitude has more noise than Latitude/Longitude and acceleration has more uncertainty than position and velocity
            processNoise = DenseMatrix.OfDiagonalArray(new double[] { 0.01, 0.01, 0.1, 0.1, 0.1, 0.1, 0.5, 0.5, 1 });

            // Measurement noise (lon/lat error ~5-10m, altitude is often worse)
            measurementNoise = DenseMatrix.OfDiagonalArray(new double[] {5, 5, 20});
        }


        /// <summary>
        /// Update the state of the filter by sending in the next point
        /// </summary>
        /// <param name="measuredX"></param>
        /// <param name="measuredY"></param>
        /// <param name="measuredZ"></param>
        public void Update(double measuredX, double measuredY, double measuredZ)
        {
            // Prediction step
            // Uses the transition model to predict the next state
            state = transition * state;

            // Increase uncertainty in covariance
            covariance = transition * covariance * transition.Transpose() + processNoise;

            // Measurement update
            var measurement = DenseMatrix.OfArray(new double[,] { { measuredX }, { measuredY }, { measuredZ } });

            // Compute how wrong the prediction was (measurement residual)
            var y = measurement - (observation * state);
            var s = observation * covariance * observation.Transpose() + measurementNoise;

            // Compute Kalman gain
            // - if GPS is reliable, trust it more
            // - if GPS is noisy, trust the prediction more
            var k = covariance * observation.Transpose() * s.Inverse(); // Kalman gain

            // Update state and covariance
            state += k * y;

            // Reduce uncertainty in the covariance matrix
            covariance = (DenseMatrix.CreateIdentity(9) - k * observation) * covariance;
        }
        

        public (double x, double y, double z) GetState()
        {
            return (state[0, 0], state[1, 0], state[2, 0]);
        }

    }
}
