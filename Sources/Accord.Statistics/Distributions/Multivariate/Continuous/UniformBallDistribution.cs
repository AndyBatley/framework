﻿// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.Statistics.Distributions.Multivariate
{
    using System;
    using Accord.Math;
    using Accord.Math.Decompositions;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Univariate;
    using System.Runtime.CompilerServices;
    using Accord.Math.Random;

    /// <summary>
    ///   Uniform distribution inside a n-dimensional ball.
    /// </summary>
    /// 
    /// <seealso cref="Accord.Statistics.Distributions.Multivariate.MultivariateContinuousDistribution" />
    /// 
    [Serializable]
    public class UniformBallDistribution : MultivariateContinuousDistribution,
        ISampleableDistribution<double[]>
    {

        // Distribution parameters
        private double[] center;
        private double radius;

        private double volume;

        
        /// <summary>
        /// Initializes a new instance of the <see cref="UniformBallDistribution"/> class.
        /// </summary>
        /// 
        /// <param name="dimension">The number of dimensions in the n-dimensional sphere.</param>
        /// 
        public UniformBallDistribution(int dimension)
            : this(new double[dimension])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniformBallDistribution"/> class.
        /// </summary>
        /// 
        /// <param name="mean">The sphere's mean.</param>
        /// <param name="radius">The sphere's radius.</param>
        /// 
        public UniformBallDistribution(double[] mean, double radius = 1)
            : base(mean.Length)
        {
            this.center = mean;
            this.radius = radius;

            double n = mean.Length;
            double num = Math.Pow(Math.PI, n / 2.0);
            double den = Gamma.Function(n / 2.0 + 1);
            volume = num / den * Math.Pow(radius, n);
        }

        /// <summary>
        /// Gets the sphere radius.
        /// </summary>
        /// 
        public double Radius { get { return radius; } }

        /// <summary>
        /// Gets the sphere volume.
        /// </summary>
        /// 
        public double Volume { get { return volume; } }

        /// <summary>
        /// Gets the sphere center (mean) vector.
        /// </summary>
        /// <value>A vector containing the mean values for the distribution.</value>
        public override double[] Mean
        {
            get { return center; }
        }

        /// <summary>
        /// Gets the variance for this distribution.
        /// </summary>
        /// <value>A vector containing the variance values for the distribution.</value>
        public override double[] Variance
        {
            get { return Vector.Create(Dimension, radius); }
        }

        /// <summary>
        /// Gets the variance-covariance matrix for this distribution.
        /// </summary>
        /// <value>A matrix containing the covariance values for the distribution.</value>
        public override double[,] Covariance
        {
            get { return Matrix.Diagonal(Variance); }
        }

        /// <summary>
        ///   Not implemented.
        /// </summary>
        /// 
        protected internal override double InnerDistributionFunction(params double[] x)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Gets the probability density function (pdf) for
        /// this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// <param name="x">A single point in the distribution range. For a
        /// univariate distribution, this should be a single
        /// double value. For a multivariate distribution,
        /// this should be a double array.</param>
        /// <returns>The probability of <c>x</c> occurring
        /// in the current distribution.</returns>
        /// <remarks>The Probability Density Function (PDF) describes the
        /// probability that a given value <c>x</c> will occur.</remarks>
        protected internal override double InnerProbabilityDensityFunction(params double[] x)
        {
            if (Distance.Euclidean(x, center) < radius)
                return 1 / volume;
            return 0;
        }

        /// <summary>
        /// Gets the log-probability density function (pdf)
        /// for this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// <param name="x">A single point in the distribution range. For a
        /// univariate distribution, this should be a single
        /// double value. For a multivariate distribution,
        /// this should be a double array.</param>
        /// <returns>The logarithm of the probability of <c>x</c>
        /// occurring in the current distribution.</returns>
        protected internal override double InnerLogProbabilityDensityFunction(params double[] x)
        {
            if (Distance.Euclidean(x, center) < radius)
                return -Math.Log(volume);
            return Double.NegativeInfinity;
        }


        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public override object Clone()
        {
            return new UniformBallDistribution(center.Copy(), radius);
        }

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        /// 
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        /// 
        public override double[][] Generate(int samples, double[][] result, Random source)
        {
            double[] normal = new double[Dimension + 2];

            for (int i = 0; i < result.Length; i++)
            {
                // Generate n + 2 independent normal distributed values
                NormalDistribution.Random(normal.Length, result: normal, source: source);
                normal.Normalize(inPlace: true); // make unit norm
                for (int j = 0; j < Dimension; j++) // take n dimensions
                    result[i][j] = normal[j] * radius + center[j];
            }

            return result;
        }

        /// <summary>
        ///   Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// 
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// 
        /// <returns>
        ///   A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// 
        public override string ToString(string format, IFormatProvider formatProvider)
        {
            return String.Format(formatProvider, "UniformBall(X; μ, r)");
        }

    }
}
