using fuzzy_logic;
using OxyPlot;
using OxyPlot.Series;
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
    /// Interaction logic for Term.xaml
    /// </summary>
    public partial class Term : Window
    {
        fuzzy_logic.Variable variable;
        List<double> args = new List<double>();
        Mfunc function;
        string var_name;
        mfunc mfunc;

        public Term(fuzzy_logic.Variable _var, string name)
        {
            variable = _var;
            var_name = name;
            Bindings.MyTermModel = null;
            InitializeComponent();
            Term_function.ItemsSource = Enum.GetValues(typeof(mfunc)).Cast<mfunc>();
            Term_function.SelectedIndex = 0;
        }

        public Term(fuzzy_logic.Variable _var, string name, string term_name, fuzzy_logic.Term _fterm)
        {
            variable = _var;
            var_name = name;
            Bindings.MyTermModel = null;
            InitializeComponent();
            Term_name.Text = term_name;
            Term_function.ItemsSource = Enum.GetValues(typeof(mfunc)).Cast<mfunc>();
            Term_function.SelectedItem = _fterm.func;

            string func_args = string.Empty;


            List<double> args = _fterm.function.GetArgs();
            /*
            foreach (var elem in _fterm.function.GetArgs())
            {
                if (func_args == string.Empty)
                {
                    func_args = elem.ToString();
                } else
                {
                    func_args = func_args + "," + elem.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            */

            func_args = string.Join(";", _fterm.function.GetArgs());
            Term_func_params.Text = func_args;

            Bindings.MyTermModel = new PlotModel { Title = Term_name.Text };

            FunctionSeries serie1 = new FunctionSeries();
            for (double i = variable.ledge; i <= variable.redge; i += 0.001)
            {
                DataPoint data = new DataPoint(i, _fterm.function.Calculate(i));
                serie1.Points.Add(data);
            }

            serie1.Title = Term_name.Text;
            serie1.StrokeThickness = 1.5;
            Bindings.MyTermModel.Series.Add(serie1);
            Term_plot.Model = Bindings.MyTermModel;
        }

        private void Term_function_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mfunc = (mfunc)Enum.Parse(typeof(mfunc), Term_function.SelectedItem.ToString(), true);

            string args = string.Empty;
            switch (mfunc)
            {
                case fuzzy_logic.mfunc.trimf:
                    args = Trimf.GetParams();
                    break;
                case fuzzy_logic.mfunc.trapmf:
                    args = Trapmf.GetParams();
                    break;
                case fuzzy_logic.mfunc.smf:
                    args = Smf.GetParams();
                    break;
                case fuzzy_logic.mfunc.zmf:
                    args = Zmf.GetParams();
                    break;
                case fuzzy_logic.mfunc.gaussmf:
                    args = Gaussmf.GetParams();
                    break;
                case fuzzy_logic.mfunc.bellmf:
                    args = Bellmf.GetParams();
                    break;
            }

            Func_label.Content = args;
        }

        private void Term_show_Click(object sender, RoutedEventArgs e)
        {
            args.Clear();

            foreach (var elem in Term_func_params.Text.Split(';'))
            {
                args.Add(double.Parse(elem));
            }

            switch (mfunc)
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
                    throw new ArgumentException(string.Format("Unknown mfunc: {0}", mfunc));
            }

            Bindings.MyTermModel = new PlotModel { Title = Term_name.Text };

            FunctionSeries serie1 = new FunctionSeries();
            for (double i = variable.ledge; i <= variable.redge; i += 0.001)
            {
                DataPoint data = new DataPoint(i, function.Calculate(i));
                serie1.Points.Add(data);
            }

            serie1.Title = Term_name.Text;
            serie1.StrokeThickness = 1.5;
            Bindings.MyTermModel.Series.Add(serie1);
            Term_plot.Model = Bindings.MyTermModel;
        }

        private void Term_save_Click(object sender, RoutedEventArgs e)
        {
            args.Clear();

            foreach (var elem in Term_func_params.Text.Split(';'))
            {
                args.Add(double.Parse(elem));
            }

            ((MainWindow)Application.Current.MainWindow).ctrl.variables[var_name].terms[Term_name.Text] = new fuzzy_logic.Term(mfunc, args);
            Bindings.Terms = ((MainWindow)Application.Current.MainWindow).ctrl.variables[var_name].terms.Keys.ToList();
            ((MainWindow)Application.Current.MainWindow).Terms_lv.ItemsSource = Bindings.Terms;
            this.Close();
        }
    }
}
