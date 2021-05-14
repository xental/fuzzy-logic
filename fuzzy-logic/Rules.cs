using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace fuzzy_logic
{
    public class Rule
    {
        public List<(string, string)> condition;
        public (string, string) conclusion;
        public double importance;
        public roperator oper;

        public Rule(List<(string, string)> condition, (string, string) conclusion, roperator oper, double importance = 1.0)
        {
            if (condition.Count == 0)
                throw new ArgumentException("Cannot create rule with empty conditions");
            
            if (importance > 1 || importance < 0)
                throw new ArgumentException("Importance should be in range 0..1");

            this.condition = condition;
            this.conclusion = conclusion;
            this.importance = importance;
            this.oper = oper;
        }
    }
}
