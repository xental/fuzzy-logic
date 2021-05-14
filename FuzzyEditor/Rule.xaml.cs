using fuzzy_logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FuzzyEditor
{
    /// <summary>
    /// Interaction logic for Rule.xaml
    /// </summary>
    public partial class Rule : Window
    {
        public List<(string, string)> condition = new List<(string, string)>();
        public (string, string) conclusion;
        public double importance = 1.0;
        public roperator oper;
        private bool edit = false;
        private int edit_id;

        public static string ConvertRule(List<(string, string)> _condition, (string, string) _conclusion, roperator _oper, double _importance)
        {
            string result = "Если";

            foreach (var elem in _condition)
            {
                if (result == "Если")
                    result += string.Format(" {0} равно {1}", elem.Item1, elem.Item2);
                else
                    result += string.Format(" {0} {1} равно {2}", _oper.ToString(), elem.Item1, elem.Item2);
            }

            result += string.Format(" = {0} равно {1} [уверенность - {2}]", _conclusion.Item1, _conclusion.Item2, _importance.ToString());

            return result;
        }

        public Rule()
        {
            InitializeComponent();
            Rule_operator.ItemsSource = Enum.GetValues(typeof(roperator)).Cast<roperator>();
            Rule_operator.SelectedIndex = 0;
            Rule_variable.ItemsSource = ((MainWindow)Application.Current.MainWindow).ctrl.variables.Keys.ToList();
            Rule_variable.SelectedIndex = 0;
            oper = (roperator)Enum.Parse(typeof(roperator), Rule_operator.SelectedItem.ToString(), true);

            Rule_term.ItemsSource = ((MainWindow)Application.Current.MainWindow).ctrl.variables[Rule_variable.SelectedItem.ToString()].terms.Keys.ToList();
            Rule_term.SelectedIndex = 0;

            Rule_conc_var.ItemsSource = ((MainWindow)Application.Current.MainWindow).ctrl.variables.Keys.ToList();
            Rule_conc_var.SelectedIndex = 0;

            Rule_conc_term.ItemsSource = ((MainWindow)Application.Current.MainWindow).ctrl.variables[Rule_conc_var.SelectedItem.ToString()].terms.Keys.ToList();
            Rule_conc_term.SelectedIndex = 0;
        }

        public Rule(fuzzy_logic.Rule _rule, int _id)
        {
            edit = true;
            edit_id = _id;
            InitializeComponent();
            condition = _rule.condition;
            conclusion = _rule.conclusion;

            oper = _rule.oper;
            importance = _rule.importance;

            Rule_operator.ItemsSource = Enum.GetValues(typeof(roperator)).Cast<roperator>();
            Rule_operator.SelectedItem = oper.ToString();
            Rule_variable.ItemsSource = ((MainWindow)Application.Current.MainWindow).ctrl.variables.Keys.ToList();
            Rule_variable.SelectedIndex = 0;

            Rule_term.ItemsSource = ((MainWindow)Application.Current.MainWindow).ctrl.variables[Rule_variable.SelectedItem.ToString()].terms.Keys.ToList();
            Rule_term.SelectedIndex = 0;

            Rule_conc_var.ItemsSource = ((MainWindow)Application.Current.MainWindow).ctrl.variables.Keys.ToList();
            Rule_conc_var.SelectedIndex = 0;

            Rule_conc_term.ItemsSource = ((MainWindow)Application.Current.MainWindow).ctrl.variables[Rule_conc_var.SelectedItem.ToString()].terms.Keys.ToList();
            Rule_conc_term.SelectedIndex = 0;
            Rule_preview_txb.Text = ConvertRule(condition, conclusion, oper, importance);
        }

        private void Rule_add_cond_Click(object sender, RoutedEventArgs e)
        {
            condition.Add((Rule_variable.SelectedItem.ToString(), Rule_term.SelectedItem.ToString()));
        }

        private void Rule_save_conc_Click(object sender, RoutedEventArgs e)
        {
            conclusion = (Rule_conc_var.SelectedItem.ToString(), Rule_conc_term.SelectedItem.ToString());
            importance = double.Parse(Rule_importance.Text);
        }

        private void Rule_preview_Click(object sender, RoutedEventArgs e)
        {
            Rule_preview_txb.Text = ConvertRule(condition, conclusion, oper, importance);
        }

        private void Rule_operator_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            oper = (roperator)Enum.Parse(typeof(roperator), Rule_operator.SelectedItem.ToString(), true);
        }

        private void Rule_clear_cond_Click(object sender, RoutedEventArgs e)
        {
            condition.Clear();
        }

        private void Rule_Save_Click(object sender, RoutedEventArgs e)
        {
            if (!edit)
                ((MainWindow)Application.Current.MainWindow).ctrl.AddRule(new fuzzy_logic.Rule(condition, conclusion, oper, importance));
            else
                ((MainWindow)Application.Current.MainWindow).ctrl.rules[edit_id] = new fuzzy_logic.Rule(condition, conclusion, oper, importance);

            List<string> rules = new List<string>();
            List<fuzzy_logic.Rule> r_list = ((MainWindow)Application.Current.MainWindow).ctrl.rules;
            for (int i = 0; i < r_list.Count; i++)
            {
                rules.Add(ConvertRule(r_list[i].condition, r_list[i].conclusion, r_list[i].oper, r_list[i].importance));
            }

            Bindings.Rules = rules;
            ((MainWindow)Application.Current.MainWindow).Rules_lv.ItemsSource = Bindings.Rules;

            this.Close();
        }

        private void Rule_variable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Rule_term.ItemsSource = ((MainWindow)Application.Current.MainWindow).ctrl.variables[Rule_variable.SelectedItem.ToString()].terms.Keys.ToList();
            Rule_term.SelectedIndex = 0;
        }
    }
}
