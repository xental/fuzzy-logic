using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;

namespace fuzzy_logic
{

    public static class Utils
    {
        public static (double, double, double) FuzzyPlato(double fuzzy_y, Term t)
        {
            double fuzzy_x;
            (double, double) plato;
            if (fuzzy_y == t.function.Calculate(t.function.Centre()))
            {
                plato = (t.function.Centre(), t.function.Centre());
                fuzzy_x = t.function.Centre();
            }
            else
            {
                Empty line = new Empty(t.function.Centre() /2, fuzzy_y);
                (double, double) left = Intersection(line, t.function);
                Empty line_rr = new Empty((t.function.Centre()+0.001), fuzzy_y);
                (double, double) right = Intersection(t.function, line_rr);
                plato = (left.Item1, right.Item1);

                if (fuzzy_y < t.function.Calculate(t.function.Centre()))
                    fuzzy_x =  plato.Item1;
                else fuzzy_x = plato.Item1;
            }

            return (plato.Item1, plato.Item2, fuzzy_x);
        }

        public static double Intersection((double, double) l1p1, (double, double) l1p2, (double, double) l2p1, (double, double) l2p2)
        {
            (double, double) s10 = (l1p2.Item1 - l1p1.Item1, l1p2.Item2 - l1p1.Item2);
            (double, double) s32 = (l2p2.Item1 - l2p1.Item1, l2p2.Item2 - l2p1.Item2);
            (double, double) s02 = (l1p1.Item1 - l2p1.Item1, l1p1.Item2 - l2p1.Item2);

            double t_numer = s32.Item1 * s02.Item2 - s32.Item2 * s02.Item1;
            double denom = s10.Item1 * s32.Item2 - s32.Item1 * s10.Item2;
            double t = t_numer / denom;

            return l1p1.Item1 + (t * s10.Item1); //, l1p1.Item2 + (t * s10.Item2));
        }

        public static (double, double) Intersection(Term left, Term right)
        {
            (double, double) f_intersect = Intersection(left.function, right.function);

            if (f_intersect.Item1 > right.plato.Item1)
            {
                Empty line = new Empty(right.plato.Item2, right.fuzzy_value);
                return Intersection(left.function, line);
            } else if (f_intersect.Item1 < left.plato.Item2)
            {
                Empty line = new Empty(left.plato.Item1, left.fuzzy_value);
                return Intersection(line, right.function);
            }

            return f_intersect;

        }

        public static (double, double) Intersection(Mfunc left, Mfunc right)
        {
            double eps = 0.00001;
            double cur_pos = (left.Centre() + right.Centre()) / 2;
            double result = cur_pos;

            while (Math.Abs(cur_pos) > eps)
            {
                double left_res;
                double left_deriv;
                double right_res;
                double right_deriv;
                if (left is Zmf)
                {
                    if (left.Calculate(((Zmf)left).GetBreakPoint()) < right.Calculate(((Zmf)left).GetBreakPoint()))
                    {
                        left_res = ((Zmf)left).GetUpper(result);
                        left_deriv = ((Zmf)left).GetUpperDeriv(result);
                    } else
                    {
                        left_res = ((Zmf)left).GetLower(result);
                        left_deriv = ((Zmf)left).GetLowerDeriv(result);
                    }
                } else
                {
                    left_res = left.Calculate(result);
                    left_deriv = left.Derivative(result);
                }

                if (right is Smf)
                {
                    if (left.Calculate(((Smf)right).GetBreakPoint()) < right.Calculate(((Smf)right).GetBreakPoint()))
                    {
                        right_res = ((Smf)right).GetUpper(result);
                        right_deriv = ((Smf)right).GetUpperDeriv(result);
                    }
                    else
                    {
                        right_res = ((Smf)right).GetLower(result);
                        right_deriv = ((Smf)right).GetLowerDeriv(result);
                    }
                }
                else
                {
                    right_res = right.Calculate(result);
                    right_deriv = right.Derivative(result);
                }

                cur_pos = (left_res - right_res) / (left_deriv - right_deriv);
                result -= cur_pos;
            }

            return (result, left.Calculate(result));
        }
    }

    public abstract class Mfunc
    {
        public abstract double Calculate(double x);
        public abstract double Derivative(double x);
        public abstract (double, double) Plato(double x);
        public abstract double Centre();

        public abstract List<double> GetArgs();
    }

    public class Empty: Mfunc
    {
        private readonly double center;
        private readonly double y;
        public Empty(double center, double y)
        {
            this.center = center;
            this.y = y;
        }

        public override double Calculate(double x)
        {
            return y;
        }

        public override double Derivative(double x)
        {
            return 0;
        }

        public override (double, double) Plato(double x)
        {
            throw new NotImplementedException();
        }

        public override double Centre()
        {
            return center;
        }

        public static string GetParams()
        {
            throw new NotImplementedException();
        }

        public override List<double> GetArgs()
        {
            throw new NotImplementedException();
        }
    }

    public class Trimf: Mfunc
    {
        private readonly double a;
        private readonly double b;
        private readonly double c;

        public Trimf(List<double> args)
        {
            if (args.Count != 3)
                throw new ArgumentException(string.Format("Trimf require 3 arguments, but {0} passed", args.Count));

            a = args[0];
            b = args[1];
            c = args[2];
        }
        public override double Calculate(double x)
        {
            return Math.Max(
                    Math.Min(
                        (x - a) / (b - a),
                        (c - x) / (c - b)),
                    0.0
                   );
        }

        public override double Derivative(double x)
        {
            if (a <= x && x <= b)
                return 1.0 / (b - a);
            else if (b <= x && x <= c)
                return -1.0 / (c - b);
            else
                return 0.0;
        }

        public override double Centre()
        {
            return b;
        }

        public override (double, double) Plato(double x)
        {
            if (x == Centre())
                return (x, x);
            
            if (x < Centre())
            {
                return (x, Utils.Intersection(
                        (Centre(), Calculate(x)),
                        (c, Calculate(x)),
                        (Centre(), 1.0),
                        (c, 0.0)
                    ));
            } else
            {
                return (Utils.Intersection(
                    (a, 0.0),
                    (Centre(), 1.0),
                    (a, Calculate(x)),
                    (Centre(), Calculate(x))
                    ), x);
            }
        }

        public static string GetParams()
        {
            return "a,b,c";
        }

        public override List<double> GetArgs()
        {
            return new List<double>() { a, b, c };
        }

    }

    public class Trapmf : Mfunc
    {
        private readonly double a;
        private readonly double b;
        private readonly double c;
        private readonly double d;


        public Trapmf(List<double> args)
        {
            if (args.Count != 4)
                throw new ArgumentException(string.Format("Tramf require 4 arguments, but {0} passed", args.Count));

            a = args[0];
            b = args[1];
            c = args[2];
            d = args[3];
        }

        public override double Calculate(double x)
        {
            return Math.Max(
                        Math.Min(
                            Math.Min(
                                (x - a) / (b - a),
                                (d - x) / (d - c)
                            ),
                            1.0
                        ),
                        0.0
                    );
        }

        public override double Derivative(double x)
        {
            if (a <= x && x <= b)
                return 1.0 / (b - a);
            else if (c <= x && x <= d)
                return -1.0 / (d - c);
            else
                return 0.0;
        }
        public override (double, double) Plato(double x)
        {
            if (b <= x && x <= c)
                return (b, c);

            if (x < Centre())
            {
                return (x, Utils.Intersection(
                        (c, Calculate(x)),
                        (d, Calculate(x)),
                        (c, 1.0),
                        (d, 0.0)
                    ));
            }
            else
            {
                return (Utils.Intersection(
                    (a, 0.0),
                    (b, 1.0),
                    (a, Calculate(x)),
                    (b, Calculate(x))
                    ), x);
            }
        }

        public override double Centre()
        {
            return (c+b)/2;
        }

        public static string GetParams()
        {
            return "a,b,c,d";
        }
        public override List<double> GetArgs()
        {
            return new List<double>() { a, b, c, d };
        }

    }

    public class Smf : Mfunc
    {
        private readonly double a;
        private readonly double b;

        public Smf(List<double> args)
        {
            if (args.Count != 2)
                throw new ArgumentException(string.Format("Smf require 2 arguments, but {0} passed", args.Count));

            a = args[0];
            b = args[1];
        }

        public double GetUpper(double x)
        {
            return x < b ? 1 - 2 * Math.Pow((x - b) / (b - a), 2): 1.0;
        }

        public double GetLower(double x)
        {
            return x > a ? 2 * Math.Pow((x - a) / (a - b), 2): 0.0;
        }

        public double GetUpperDeriv(double x)
        {
            return x < b ? -4 * (x - b) / Math.Pow(b - a, 2): 0.0;
        }

        public double GetLowerDeriv(double x)
        {
            return x > a ? 4 * (x - a) / Math.Pow(b - a, 2): 0.0;
        }

        public override double Calculate(double x)
        {
            if (x <= (a + b) / 2)
                return GetLower(x);
            else
                return GetUpper(x);
        }

        public override double Derivative(double x)
        {
            if (x <= (a + b) / 2)
                return GetLowerDeriv(x);
            else return GetUpperDeriv(x);
        }

        public override (double, double) Plato(double x)
        {
            return (x, double.MaxValue);
        }

        public override double Centre()
        {
            return b;
        }

        public double GetBreakPoint()
        {
            return (a + b) / 2;
        }

        public static string GetParams()
        {
            return "a,b";
        }

        public override List<double> GetArgs()
        {
            return new List<double>() { a, b };
        }
    }

    public class Zmf : Mfunc
    {
        private readonly double a;
        private readonly double b;

        public Zmf(List<double> args)
        {
            if (args.Count != 2)
                throw new ArgumentException(string.Format("Zmf require 2 arguments, but {0} passed", args.Count));

            a = args[0];
            b = args[1];
        }

        public double GetUpper(double x)
        {
            return x < b ? 1 - 2 * Math.Pow((x - a) / (b - a), 2): 1.0;
        }

        public double GetLower(double x)
        {
            return x > a ? 2 * Math.Pow((x - b) / (b - a), 2): 0.0;
        }

        public double GetUpperDeriv(double x)
        {
            return x < b ? -4 * (x - a) / Math.Pow(b - a, 2): 1.0;
        }

        public double GetLowerDeriv(double x)
        {
            return x > a ? 4 * (x - b) / Math.Pow(b - a, 2): 0.0;
        }

        public override double Calculate(double x)
        {
            if (x <= (a + b) / 2)
                return GetUpper(x);
            else
                return GetLower(x);
        }

        public override double Derivative(double x)
        {
            if (x <= (a + b) / 2)
                return GetUpperDeriv(x);
            else return GetLowerDeriv(x);
        }

        public override (double, double) Plato(double x)
        {
            return (double.MinValue, x);
        }

        public override double Centre()
        {
            return a;
        }

        public double GetBreakPoint()
        {
            return (a + b) / 2;
        }

        public static string GetParams()
        {
            return "a,b";
        }

        public override List<double> GetArgs()
        {
            return new List<double>() { a, b };
        }
    }

    public class Gaussmf : Mfunc
    {
        private readonly double sig;
        private readonly double c;

        public Gaussmf(List<double> args)
        {
            if (args.Count != 2)
                throw new ArgumentException(string.Format("Gaussmf require 2 arguments, but {0} passed", args.Count));

            sig = args[1];
            c = args[0];
        }

        public override double Calculate(double x)
        {
            return Math.Exp(
                -Math.Pow(x - c, 2) / (2 * Math.Pow(sig, 2))
            );
        }

        public override double Derivative(double x)
        {
            return -((x - c) * Calculate(x)) / Math.Pow(sig, 2);
        }

        public override (double, double) Plato(double x)
        {
            if (x <= Centre())
                return (x, 2 * Centre() - x);
            else return (Centre() - x, x);
        }

        public override double Centre()
        {
            return c;
        }

        public static string GetParams()
        {
            return "c,sig";
        }
        public override List<double> GetArgs()
        {
            return new List<double>() { c, sig };
        }
    }

    public class Bellmf : Mfunc
    {
        private readonly double a;
        private readonly double b;
        private readonly double c;

        public Bellmf(List<double> args)
        {
            if (args.Count != 3)
                throw new ArgumentException(string.Format("Bellmf require 3 arguments, but {0} passed", args.Count));

            a = args[0];
            b = args[1];
            c = args[2];
        }

        public override double Calculate(double x)
        {
            return 1 / (
                1 + Math.Pow(
                        Math.Abs(
                            (x - c) / a
                        ),
                        2 * b
                    )
            );
        }

        public override double Derivative(double x)
        {
            double x_m_c_p2b = Math.Pow(Math.Abs(x - c), 2 * b);
            double a_p2b = Math.Pow(Math.Abs(a), 2 * b);
            return -(2 * a_p2b * b * x_m_c_p2b) /
                    ((x - c) * Math.Pow(x_m_c_p2b + a_p2b, 2));
        }

        public override (double, double) Plato(double x)
        {
            if (x <= Centre())
                return (x, 2 * Centre() - x);
            else return (Centre() - x, x);
        }

        public override double Centre()
        {
            return b;
        }

        public static string GetParams()
        {
            return "a,b,c";
        }

        public override List<double> GetArgs()
        {
            return new List<double>() { a, b, c };
        }
    }
}
