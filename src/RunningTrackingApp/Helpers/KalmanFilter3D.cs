using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace RunningTrackingApp.Helpers
{
    /// <summary>
    /// Implement the 3D Kalman filter using a constant acceleration model.
    /// This is an adaptation of the code found here: https://www.emgu.com/wiki/index.php/Kalman_Filter 
    /// extended to 3 dimensions and non-constant velocity.
    /// </summary>
    public class KalmanFilter3D
    {
        private Matrix<double> state; // [x, y, z, vx, vy, vz]
        private Matrix<double> covariance;
        private readonly Matrix<double> transition;
        private readonly Matrix<double> observation;
        private readonly Matrix<double> processNoise;
        private readonly Matrix<double> measurementNoise;


       
        public KalmanFilter3D(double initialX, double initialY, double initialZ)
        {
            // Initial state: [x, y, z, vx, vy, vz, ax, ay, az] (Assume zero initial velocity and acceleration)
            state = DenseMatrix.OfArray(new double[,] { 
                { initialX }, { initialY }, { initialZ }, 
                { 0 }, { 0 }, { 0 }, 
                { 0 }, { 0 }, { 0 } });

            // Initial covariance matrix (uncertainty)
            covariance = DenseMatrix.CreateIdentity(9) * 1000;

            // State transition matrix (Assuming constant velocity model)
            transition = DenseMatrix.OfArray(new double[,]
            {
                {1, 0, 0, 1, 0, 0, 0.5, 0, 0 }, // x' = x + vx + 0.5*ax
                {0, 1, 0, 0, 1, 0, 0, 0.5, 0 }, // y' = y + vy + 0.5*ay
                {0, 0, 1, 0, 0, 1, 0, 0, 0.5 }, // z' = z + vz + 0.5*az
                {0, 0, 0, 1, 0, 0, 1, 0, 0 }, // vx' = vx + ax
                {0, 0, 0, 0, 1, 0, 0, 1, 0 }, // vy' = vy + ay
                {0, 0, 0, 0, 0, 1, 0, 0, 1 }, // vz' = vz + az
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
            processNoise = DenseMatrix.CreateIdentity(9) * 0.01;

            // Measurement noise (lon/lat error ~5-10m, altitude is often worse)
            measurementNoise = DenseMatrix.OfDiagonalArray(new double[] {5, 5, 20});
        }


        public void Update(double measuredX, double measuredY, double measuredZ)
        {
            // Prediction step
            state = transition * state;
            covariance = transition * covariance * transition.Transpose() + processNoise;

            // Measurement update
            var measurement = DenseMatrix.OfArray(new double[,] { { measuredX }, { measuredY }, { measuredZ } });

            var y = measurement - (observation * state); // Measurement residual
            var s = observation * covariance * observation.Transpose() + measurementNoise;
            var k = covariance * observation.Transpose() * s.Inverse(); // Kalman gain

            // Correct state and covariance
            state += k * y;
            covariance = (DenseMatrix.CreateIdentity(9) - k * observation) * covariance;
        }
        

        public (double x, double y, double z) GetState()
        {
            return (state[0, 0], state[1, 0], state[2, 0]);
        }

    }
}
