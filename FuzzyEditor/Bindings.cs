using OxyPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace FuzzyEditor
{
    public static class Bindings
    {
        

        private static List<string> variables = new List<string>() { "a", "b" };
        private static List<string> terms = new List<string>();
        private static List<string> rules = new List<string>();

        public static List<string> Vars
        {
            get
            {
                return variables;
            }
            set
            {
                variables = value;
            }
        }

        public static List<string> Terms
        {
            get
            {
                return terms;
            }
            set
            {
                terms = value;
            }
        }

        public static List<string> Rules
        {
            get
            {
                return rules;
            }
            set
            {
                rules = value;
            }
        }

        public static PlotModel MyModel { get; set; }
        public static PlotModel MyTermModel { get; set; }
        public static PlotModel MyExpModel { get; set; }
    }
}
