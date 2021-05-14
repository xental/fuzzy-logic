using System;
using System.Collections.Generic;
using System.Text;

namespace fuzzy_logic
{
    public class Term: IComparable
    {
        public readonly Mfunc function;
        public (double, double)  plato;
        public double x;
        public mfunc func;
        public double fuzzy_value;

        public Term(mfunc ftype, List<double> args)
        {
            func = ftype;
            switch (ftype)
            {
                case mfunc.trimf:
                    function = new Trimf(args);
                    break;
                case mfunc.trapmf:
                    function = new Trapmf(args);
                    break;
                case mfunc.smf:
                    function = new Smf(args);
                    break;
                case mfunc.zmf:
                    function = new Zmf(args);
                    break;
                case mfunc.gaussmf:
                    function = new Gaussmf(args);
                    break;
                case mfunc.bellmf:
                    function = new Bellmf(args);
                    break;
                default:
                    throw new ArgumentException(string.Format("Unknown mfunc: {0}", ftype));
            }
        }

        public Term(Term t)
        {
            this.function = t.function;
            this.func = t.func;
        }

        public int CompareTo(object obj)
        {
            return function.Centre() > ((Term)obj).function.Centre() ? 1 : 0;
        }

        public void Fuzzyfy(double x)
        {
            this.x = x;
            fuzzy_value = function.Calculate(x);
            plato = function.Plato(x);
        }

        public double Cacldulate(double x)
        {
            if (double.IsNaN(fuzzy_value) || (plato.Item1 > x || plato.Item2 < x))
            {
                return function.Calculate(x);
            } 
            
            return fuzzy_value;            
        }
    }


    public class Variable
    {
        public double ledge;
        public double redge;
        public Dictionary<string, Term> terms = new Dictionary<string, Term>();

        public Variable(double ledge, double redge)
        {
            if (redge < ledge)
                throw new ArgumentException("ledge should be less then redge");

            this.ledge = ledge;
            this.redge = redge;
        }

        public void RegisterTerm(string tname, mfunc ftype, List<double> args)
        {
            terms.Add(tname, new Term(ftype, args));
        }

        public void DeleteTerm(string tname)
        {
            terms.Remove(tname);
        }

        public void ChangeTerm(string tname, mfunc ftype, List<double> args)
        {
            RegisterTerm(tname, ftype, args);
        }

        public void ClearTemrs()
        {
            terms.Clear();
        }

        public void Fuzzyfy(double value)
        {
            foreach(var term in terms)
            {
                term.Value.Fuzzyfy(value);
            }
        }
    }
}
