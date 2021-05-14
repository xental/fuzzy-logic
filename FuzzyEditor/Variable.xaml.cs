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
    /// Interaction logic for Variable.xaml
    /// </summary>
    public partial class Variable : Window
    {
        public Variable()
        {
            InitializeComponent();
        }

        private void Variable_save_Click(object sender, RoutedEventArgs e)
        {
            (double, double) edges = (Convert.ToDouble(Veriable_borders.Text.Split(';')[0]), Convert.ToDouble(Veriable_borders.Text.Split(';')[1]));
            fuzzy_logic.Variable _var = new fuzzy_logic.Variable(edges.Item1, edges.Item2);
            ((MainWindow)Application.Current.MainWindow).ctrl.AddVariable(Veriable_name.Text, _var);
            Bindings.Vars = ((MainWindow)Application.Current.MainWindow).ctrl.variables.Keys.ToList();
            ((MainWindow)Application.Current.MainWindow).Variables_lv.ItemsSource = Bindings.Vars;

            this.Close();
        }
    }
}
