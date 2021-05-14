using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace fuzzy_logic
{
    public enum mfunc
    {
        trimf,
        trapmf,
        smf,
        zmf,
        gaussmf,
        bellmf
    }

    public enum roperator
    {
        and,
        or
    }

    public enum defuzzify
    {
        last,
        first,
        centroid
    }

    public class FuzzyResult
    {
        public List<((double, double), string, Term)> fuzzy_set;
        public List<(double, double)> elems;
    }

    public class FuzzyResult_n
    {
        public string name;
        public Dictionary<string, Term> terms;
    }

    public class Controller
    {
        public List<Rule> rules = new List<Rule>();
        public Dictionary<string, Variable> variables = new Dictionary<string, Variable>();

        public Controller() { }
        public void AddRule(Rule r)
        {
            rules.Add(r);
        }

        public void AddVariable(string name, Variable v)
        {
            variables.Add(name, v);
        }

        public void Train(Dictionary<string, double> values)
        {
            foreach (var elem in values)
            {
                variables[elem.Key].Fuzzyfy(elem.Value);
            }
        }

        public static double Defuzzyfy(List<(double, double)> function, defuzzify mode)
        {

            double calculate_centroid(List<(double, double)> function)
            {
                double numerator = 0;
                double denuminator = 0;

                double delta = 0.0001;

                for (int i = 0; i < function.Count - 1; i++)
                {
                    numerator += (function[i].Item2 * function[i].Item1 + function[i + 1].Item2 * function[i + 1].Item1) * delta;
                    denuminator += (function[i].Item2 + function[i + 1].Item2) * delta;
                }


                return numerator / denuminator;
            }

            double calculate_first(List<(double, double)> function)
            {

                double x = double.NaN;
                double fval = double.NaN;

                foreach((double, double) point in function)
                {
                    if (double.IsNaN(fval) || point.Item2 > fval)
                    {
                        fval = point.Item2;
                        x = point.Item1;
                    }
                }

                return x;
            }

            double calculate_last(List<(double, double)> function)
            {

                double x = double.NaN;
                double fval = double.NaN;

                foreach ((double, double) point in function)
                {
                    if (double.IsNaN(fval) || point.Item2 >= fval)
                    {
                        fval = point.Item2;
                        x = point.Item1;
                    }
                }

                return x;
            }

            switch (mode)
            {
                case defuzzify.last:
                    return calculate_last(function);
                case defuzzify.first:
                    return calculate_first(function); ;
                case defuzzify.centroid:
                    return calculate_centroid(function);
                default:
                    throw new InvalidOperationException("Wrong deffuzification method");
            }
        }

        public (List<(double, double)>, List<Term>) Inference(string variable, double delta=0.0001)
        {
            List<Rule> temp = new List<Rule>();
            List<Term> term_lst = new List<Term>();
            List<(double, double)> function = new List<(double, double)>();

            foreach (Rule r in rules)
            {
                if (r.conclusion.Item1 == variable)
                {
                    temp.Add(r);
                }
            }

            for(double i = variables[variable].ledge; i <= variables[variable].redge; i+= delta)
            {
                function.Add((i, 0.0));
            }

            foreach (Rule r in temp)
            {
                double term_result = variables[r.condition[0].Item1].terms[r.condition[0].Item2].fuzzy_value;

                for (int i = 1; i < r.condition.Count; i++)
                {
                    double term_fres = variables[r.condition[i].Item1].terms[r.condition[i].Item2].fuzzy_value;
                    if (r.oper == roperator.and)
                    {
                        term_result = term_result > term_fres ? term_fres : term_result;
                    }
                    else
                    {
                        term_result = term_result < term_fres ? term_fres : term_result;
                    }
                }

                Term t = new Term(variables[variable].terms[r.conclusion.Item2]);
                (double, double, double) f_plato = Utils.FuzzyPlato(term_result, t);
                t.x = f_plato.Item3;
                t.fuzzy_value = term_result;
                t.plato = (f_plato.Item1, f_plato.Item2);

                term_lst.Add(t);

                int count = 0;

                for (double i = variables[variable].ledge; i < variables[variable].redge; i += delta)
                {
                    double temp_val = t.Cacldulate(i);

                    if (Math.Abs(function[count].Item2 - temp_val) > 0.00001 && (function[count].Item2 < temp_val))
                    {
                        function[count] = (function[count].Item1, temp_val);
                    }

                    count++;
                }
            }

            return (function, term_lst);
        }

        public void Inference_n(string variable)
        {
            List<Rule> temp = new List<Rule>();

            foreach (Rule r in rules)
            {
                if (r.conclusion.Item1 == variable)
                {
                    temp.Add(r);
                }
            }

            foreach (Rule r in temp)
            {
                double term_result = variables[r.condition[0].Item1].terms[r.condition[0].Item2].fuzzy_value;

                for (int i = 1; i < r.condition.Count; i++)
                {
                    double term_fres = variables[r.condition[i].Item1].terms[r.condition[i].Item2].fuzzy_value;
                    if (r.oper == roperator.and)
                    {
                        term_result = term_result > term_fres ? term_fres : term_result;
                    }
                    else
                    {
                        term_result = term_result < term_fres ? term_fres : term_result;
                    }
                }

                (double, double, double) result = Utils.FuzzyPlato(term_result, variables[variable].terms[r.conclusion.Item2]);
                variables[variable].terms[r.conclusion.Item2].fuzzy_value = term_result;
                variables[variable].terms[r.conclusion.Item2].x = result.Item3;
                variables[variable].terms[r.conclusion.Item2].plato = (result.Item1, result.Item2);
            }
        }
    }
}
