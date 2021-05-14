using Microsoft.VisualStudio.TestTools.UnitTesting;
using fuzzy_logic;
using System.Collections.Generic;
using System;

namespace TestUtils
{
    [TestClass]
    public class TestUtils
    {
        Mfunc f1 = new Zmf(new List<double> { -1.2, -0.6 });
        //Mfunc f2 = new Smf(new List<double> { -1.1, -0.5 });
        Mfunc f2 = new Gaussmf(new List<double> { -1, 0.15 });



        [TestMethod]
        public void Nothing()
        {
            (double, double) x = Utils.Intersection(f1, f2);

            Console.WriteLine(string.Format("X={0} and Y={1}", x.Item1, x.Item2));
            Assert.AreEqual(1, 1);
        }

        [TestMethod]
        public void Nothing1()
        {
            Controller ctrl = new Controller();

            Variable temp = new Variable(0, 150);

            temp.RegisterTerm("Низкая", mfunc.tramf, new List<double>() { -1, 0, 50, 100 });
            temp.RegisterTerm("Средняя", mfunc.tramf, new List<double>() { 0, 50, 100, 150 });
            temp.RegisterTerm("Высокая", mfunc.tramf, new List<double>() { 50, 100, 150, 200 });

            //temp.Fuzzyfy(85);
            Console.WriteLine("###########################");
            Variable cons = new Variable(0, 8);
            cons.RegisterTerm("Средний", mfunc.trimf, new List<double>() { 2, 4, 6 });
            cons.RegisterTerm("Малый", mfunc.trimf, new List<double>() { 0, 2, 4 });
            cons.RegisterTerm("Большой", mfunc.trimf, new List<double>() { 4, 6, 8 });
            //cons.Fuzzyfy(3.5);


            Variable pres = new Variable(0, 100);
            pres.RegisterTerm("Среднее", mfunc.trimf, new List<double>() { 0, 50, 100 });
            pres.RegisterTerm("Низкое", mfunc.trimf, new List<double>() { -1, 0, 100 });
            pres.RegisterTerm("Высокое", mfunc.trimf, new List<double>() { 0, 100, 101 });

            ctrl.AddVariable("Давление", pres);
            ctrl.AddVariable("Расход", cons);
            ctrl.AddVariable("Температура", temp);

            (string, string) r1c1 = ("Температура", "Низкая");
            (string, string) r1c2 = ("Расход", "Малый");
            (string, string) r1d = ("Давление", "Низкое");

            (string, string) r2c1 = ("Температура", "Средняя");
            (string, string) r2d = ("Давление", "Среднее");

            (string, string) r3c1 = ("Температура", "Высокая");
            (string, string) r3c2 = ("Расход", "Большой");
            (string, string) r3d = ("Давление", "Высокое");


            ctrl.AddRule(new Rule(new List<(string, string)>() { r1c1, r1c2 }, r1d, roperator.and));
            ctrl.AddRule(new Rule(new List<(string, string)>() { r2c1}, r2d, roperator.and));
            ctrl.AddRule(new Rule(new List<(string, string)>() { r3c1, r3c2 }, r3d, roperator.or));

            Dictionary<string, double> input = new Dictionary<string, double>();
            input.Add("Температура", 85);
            input.Add("Расход", 3.5);

            ctrl.Train(input);
            ctrl.Inference("Давление");

        }

        [TestMethod]
        public void Nothing4()
        {
            Term t1 = new Term(mfunc.trimf, new List<double> { -1, 0, 100 });
            t1.Fuzzyfy(77);
            Term t2 = new Term(mfunc.trimf, new List<double> { 0, 50, 100 });
            t2.Fuzzyfy(50);
            Console.WriteLine($"X1: {t1.plato.Item1} X2: {t1.plato.Item2}");
            (double, double) res = Utils.Intersection(t1, t2);
            Console.WriteLine($"X: {res.Item1} Y: {res.Item2}");

        }

        [TestMethod]
        public void Nothing5()
        {
            Term t = new Term(mfunc.gaussmf, new List<double> { -1, 0.15 });
            t.Fuzzyfy(-1.192);
            Console.WriteLine($"X1: {t.plato.Item1} X2: {t.plato.Item2}");
            Term t2 = new Term(mfunc.trimf, new List<double> { -1, 1, 2.5 });
            t2.Fuzzyfy(1.75);
            Console.WriteLine($"X1: {t2.plato.Item1} X2: {t2.plato.Item2}");
            Term t3 = new Term(mfunc.tramf, new List<double> { 0, 1, 2.5, 6 });
            t3.Fuzzyfy(0.5);
            Console.WriteLine($"X1: {t3.plato.Item1} X2: {t3.plato.Item2}");
        }
    }
}

