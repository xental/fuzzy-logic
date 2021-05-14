using Microsoft.VisualStudio.TestTools.UnitTesting;
using fuzzy_logic;
using System.Collections.Generic;
using System;

namespace TestFuzzy
{
    [TestClass]
    public class TestTrimf
    {
        private double eps = 0.0001;
        private Trimf func = new Trimf(new List<double> { 0.0, 1.0, 2.0 });

        [TestMethod]
        [DataRow(-0.01, 0.0)]
        [DataRow(0.0, 0.0)]
        [DataRow(0.5, 0.5)]
        [DataRow(1.0, 1.0)]
        [DataRow(1.5, 0.5)]
        [DataRow(2.0, 0.0)]
        [DataRow(2.01, 0.0)]
        public void Calculate(double x, double exp)
        {
            double result = func.Calculate(x);

            Assert.IsTrue((Math.Abs(exp) - Math.Abs(result)) < eps);
        }

        [TestMethod]
        [DataRow(-0.01, 0.0)]
        [DataRow(0.0, 1.0)]
        [DataRow(0.5, 1.0)]
        [DataRow(1.0, 1.0)]
        [DataRow(1.5, -1.0)]
        [DataRow(2.0, -1.0)]
        [DataRow(2.01, 0.0)]
        public void Derivative(double x, double exp)
        {
            double result = func.Derivative(x);

            Assert.IsTrue((Math.Abs(exp) - Math.Abs(result)) < eps);
        }
    }

    [TestClass]
    public class TestTrapmf
    {
        private double eps = 0.0001;
        private Trapmf func = new Trapmf(new List<double> { 0.0, 1.0, 2.0, 3.0 });

        [TestMethod]
        [DataRow(-0.01, 0.0)]
        [DataRow(0.0, 0.0)]
        [DataRow(0.5, 0.5)]
        [DataRow(1.0, 1.0)]
        [DataRow(1.5, 1.0)]
        [DataRow(2.0, 1.0)]
        [DataRow(2.5, 0.5)]
        [DataRow(3.0, 0.0)]
        [DataRow(3.01, 0.0)]
        public void Calculate(double x, double exp)
        {
            double result = func.Calculate(x);

            Assert.IsTrue((Math.Abs(exp) - Math.Abs(result)) < eps);
        }

        [TestMethod]
        [DataRow(-0.01, 0.0)]
        [DataRow(0.0, 1.0)]
        [DataRow(0.5, 1.0)]
        [DataRow(1.0, 1.0)]
        [DataRow(1.5, 0.0)]
        [DataRow(2.0, -1.0)]
        [DataRow(2.5, -1.0)]
        [DataRow(3.0, -1.0)]
        [DataRow(3.01, 0.0)]
        public void Derivative(double x, double exp)
        {
            double result = func.Derivative(x);

            Assert.IsTrue((Math.Abs(exp) - Math.Abs(result)) < eps);
        }
    }

    [TestClass]
    public class TestZmf
    {
        private double eps = 0.0001;
        private Zmf func = new Zmf(new List<double> { 1.0, 2.0 });

        [TestMethod]
        [DataRow(0.0, 1.0)]
        [DataRow(1.0, 1.0)]
        [DataRow(1.5, 0.5)]
        [DataRow(2.0, 0.0)]
        [DataRow(2.5, 0.0)]
        public void Calculate(double x, double exp)
        {
            double result = func.Calculate(x);

            Assert.IsTrue((Math.Abs(exp) - Math.Abs(result)) < eps);
        }

        [TestMethod]
        [DataRow(0.0, 0.0)]
        [DataRow(1.0, 0.0)]
        [DataRow(1.4, -1.6)]
        [DataRow(1.5, -2.0)]
        [DataRow(1.6, -1.6)]
        [DataRow(2.0, 0.0)]
        [DataRow(2.5, 0.0)]
        public void Derivative(double x, double exp)
        {
            double result = func.Derivative(x);

            Assert.IsTrue((Math.Abs(exp) - Math.Abs(result)) < eps);
        }
    }

    [TestClass]
    public class TestSmf
    {
        private double eps = 0.0001;
        private Smf func = new Smf(new List<double> { 1.0, 2.0 });

        [TestMethod]
        [DataRow(0.0, 0.0)]
        [DataRow(1.0, 0.0)]
        [DataRow(1.5, 0.5)]
        [DataRow(2.0, 1.0)]
        [DataRow(2.5, 1.0)]
        public void Calculate(double x, double exp)
        {
            double result = func.Calculate(x);

            Assert.IsTrue((Math.Abs(exp) - Math.Abs(result)) < eps);
        }

        [TestMethod]
        [DataRow(0.0, 0.0)]
        [DataRow(1.0, 0.0)]
        [DataRow(1.4, 1.6)]
        [DataRow(1.5, 2.0)]
        [DataRow(1.6, 1.6)]
        [DataRow(1.7, 1.2)]
        [DataRow(2.0, 0.0)]
        [DataRow(2.5, 0.0)]
        public void Derivative(double x, double exp)
        {
            double result = func.Derivative(x);

            Assert.IsTrue((Math.Abs(exp) - Math.Abs(result)) < eps);
        }
    }

    [TestClass]
    public class TestGaussmf
    {
        private double eps = 0.0001;
        private Gaussmf func = new Gaussmf(new List<double> { 2.0, 2.0 });

        [TestMethod]
        [DataRow(-2.0, 0.13533)]
        [DataRow(-0.5, 0.45783)]
        [DataRow(0.0, 0.60653)]
        [DataRow(2.0, 1.0)]
        [DataRow(4.0, 0.60653)]
        [DataRow(4.5, 0.45783)]
        [DataRow(6.0, 0.13533)]
        public void Calculate(double x, double exp)
        {
            double result = func.Calculate(x);
            Console.WriteLine(result);
            Assert.IsTrue((Math.Abs(exp) - Math.Abs(result)) < eps);
        }

        [TestMethod]
        [DataRow(-2.0, 0.13533)]
        [DataRow(-0.5, 0.28614)]
        [DataRow(0.0, 0.30326)]
        [DataRow(2.0, 0.0)]
        [DataRow(4.0, -0.3032)]
        [DataRow(4.5, -0.28615)]
        [DataRow(6.0, -0.13534)]
        public void Derivative(double x, double exp)
        {
            double result = func.Derivative(x);

            Assert.IsTrue((Math.Abs(exp) - Math.Abs(result)) < eps);
        }
    }

    [TestClass]
    public class TestBellmf
    {
        private double eps = 0.0001;
        private Bellmf func = new Bellmf(new List<double> { 0.5, 5.0, 0.0 });

        [TestMethod]
        [DataRow(-1.0, 0.00097)]
        [DataRow(-0.5, 0.5)]
        [DataRow(-0.2, 0.99989)]
        [DataRow(0.0, 1.0)]
        [DataRow(0.2, 0.99989)]
        [DataRow(0.5, 0.5)]
        [DataRow(1.0, 0.00097)]
        public void Calculate(double x, double exp)
        {
            double result = func.Calculate(x);

            Assert.IsTrue((Math.Abs(exp) - Math.Abs(result)) < eps);
        }

        [TestMethod]
        [DataRow(-1.0, 0.00974)]
        [DataRow(-0.5, 5.0)]
        [DataRow(-0.2, 0.00524)]
        [DataRow(0.2, -0.00524)]
        [DataRow(0.5, -5.0)]
        [DataRow(1.0, -0.00974)]
        public void Derivative(double x, double exp)
        {
            double result = func.Derivative(x);

            Assert.IsTrue((Math.Abs(exp) - Math.Abs(result)) < eps);
        }
    }
}
