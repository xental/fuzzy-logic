using fuzzy_logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;

namespace FuzzyEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    
    public partial class MainWindow : Window
    {
        public Controller ctrl = new Controller();
        public MainWindow()
        {
            InitializeComponent();
            if (File.Exists("temp.xml")) {
                ctrl = Utils.ReadXML("temp.xml");
            }
            Bindings.Vars = ctrl.variables.Keys.ToList();
            Variables_lv.ItemsSource = Bindings.Vars;

            List<string> rules = new List<string>();
            for (int i = 0; i < ctrl.rules.Count; i++)
            {
                rules.Add(Rule.ConvertRule(ctrl.rules[i].condition, ctrl.rules[i].conclusion, ctrl.rules[i].oper, ctrl.rules[i].importance));
            }
            Bindings.Rules = rules;
            Rules_lv.ItemsSource = Bindings.Rules;

            
        }

        private void Variables_lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Variables_lv.SelectedItem != null)
            {
                Bindings.Terms = ctrl.variables[Variables_lv.SelectedItem.ToString()].terms.Keys.ToList();
                Terms_lv.ItemsSource = Bindings.Terms;
            } else
            {
                Variables_lv.SelectedIndex = 0;               
            }
        }

        private void Variables_btn_add_Click(object sender, RoutedEventArgs e)
        {
            Variable var_window = new Variable();
            var_window.Show();
        }

        private void Variables_btn_show_Click(object sender, RoutedEventArgs e)
        {
            ShowGraph graph_page = new ShowGraph(ctrl.variables[Variables_lv.SelectedItem.ToString()], Variables_lv.SelectedItem.ToString());
            graph_page.Show();
        }

        private void Variables_btn_del_Click(object sender, RoutedEventArgs e)
        {
            ctrl.variables.Remove(Variables_lv.SelectedItem.ToString());
            Bindings.Vars = ctrl.variables.Keys.ToList();
            Variables_lv.ItemsSource = Bindings.Vars;
        }

        private void Terms_btn_add_Click(object sender, RoutedEventArgs e)
        {
            Term Term_window = new Term(ctrl.variables[Variables_lv.SelectedItem.ToString()], Variables_lv.SelectedItem.ToString());

            Term_window.Show();
        }

        private void Terms_btn_edit_Click(object sender, RoutedEventArgs e)
        {
            Term Term_window = new Term(ctrl.variables[Variables_lv.SelectedItem.ToString()], 
                                        Variables_lv.SelectedItem.ToString(),
                                        Terms_lv.SelectedItem.ToString(),
                                        ctrl.variables[Variables_lv.SelectedItem.ToString()].terms[Terms_lv.SelectedItem.ToString()]
                                        );

            Term_window.Show();
        }

        private void Terms_btn_del_Click(object sender, RoutedEventArgs e)
        {
            ctrl.variables[Variables_lv.SelectedItem.ToString()].terms.Remove(Terms_lv.SelectedItem.ToString());
            if (Terms_lv.SelectedItem != null)
            {
                Bindings.Terms = ctrl.variables[Variables_lv.SelectedItem.ToString()].terms.Keys.ToList();
                Terms_lv.ItemsSource = Bindings.Terms;
            }
            else
            {
                Terms_lv.SelectedIndex = 0;
            }
        }

        private void Rules_btn_add_Click(object sender, RoutedEventArgs e)
        {
            Rule Rule_window = new Rule();
            Rule_window.Show();
        }

        private void Rules_btn_del_Click(object sender, RoutedEventArgs e)
        {
            ctrl.rules.RemoveAt(Rules_lv.SelectedIndex); 
            List<string> rules = new List<string>();
            for (int i = 0; i < ctrl.rules.Count; i++)
            {
                rules.Add(Rule.ConvertRule(ctrl.rules[i].condition, ctrl.rules[i].conclusion, ctrl.rules[i].oper, ctrl.rules[i].importance));
            }

            Bindings.Rules = rules;
            Rules_lv.ItemsSource = Bindings.Rules;
        }

        private void Rules_btn_edit_Click(object sender, RoutedEventArgs e)
        {
            Rule rule_window = new Rule(ctrl.rules[Rules_lv.SelectedIndex], Rules_lv.SelectedIndex);

            rule_window.Show();
        }

        private void Menu_run_Click(object sender, RoutedEventArgs e)
        {
            Experiment exp_windows = new Experiment();

            exp_windows.Show();
        }

        private void Menu_save_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new SaveFileDialog();
            fileDialog.ShowDialog();
            string save_path = fileDialog.FileName;
            Utils.SaveXML(save_path.ToString(), ctrl);
        }

        private void Menu_load_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.ShowDialog();

            ctrl = Utils.ReadXML(fileDialog.FileName);

            Bindings.Vars = ctrl.variables.Keys.ToList();
            Variables_lv.ItemsSource = Bindings.Vars;
            List<string> rules = new List<string>();
            for (int i = 0; i < ctrl.rules.Count; i++)
            {
                rules.Add(Rule.ConvertRule(ctrl.rules[i].condition, ctrl.rules[i].conclusion, ctrl.rules[i].oper, ctrl.rules[i].importance));
            }

            Bindings.Rules = rules;
            Rules_lv.ItemsSource = Bindings.Rules;
        }
    }
}
