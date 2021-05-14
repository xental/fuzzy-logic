using fuzzy_logic;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for Experiment.xaml
    /// </summary>
    public partial class Experiment : Window
    {
        Dictionary<string, double> input = new Dictionary<string, double>();
        public Experiment()
        {
            InitializeComponent();
            foreach (string item in Bindings.Vars)
            {
                Exp_variable.Items.Add(item);
            }

            Exp_variable.SelectedIndex = 0;

        }

        private void add_input_Click(object sender, RoutedEventArgs e)
        {
            string selected_var = Exp_variable.SelectedItem.ToString();
            double ledge = ((MainWindow)Application.Current.MainWindow).ctrl.variables[selected_var].ledge;
            double redge = ((MainWindow)Application.Current.MainWindow).ctrl.variables[selected_var].redge;
            InputLabel.Text = String.Format("Enter values in range {0} - {1}:", ledge, redge);
            InputBox.Visibility = System.Windows.Visibility.Visible;
        }

        private void setInput_value_Click(System.Object sender, System.Windows.RoutedEventArgs e)
        {
            string selected_var = Exp_variable.SelectedItem.ToString();
            double value = double.Parse(InputTextBox.Text);
            input.Add(selected_var, value);
            Exp_variable.Items.Remove(selected_var);
            InputBox.Visibility = System.Windows.Visibility.Collapsed;
            InputTextBox.Text = "";
            inputs_lst.Items.Add(String.Format("{0} - {1}:", selected_var, value.ToString()));
        }

        private void run_Click(object sender, RoutedEventArgs e)
        {
            fuzzy_logic.Controller temp_ctrl = ((MainWindow)Application.Current.MainWindow).ctrl;
            temp_ctrl.Train(input);

            string tested_var = Exp_variable.SelectedItem.ToString();
            (List<(double, double)>, List <fuzzy_logic.Term>) result = temp_ctrl.Inference(tested_var);

            Bindings.MyExpModel = new PlotModel { Title = tested_var };
            FunctionSeries serie = new FunctionSeries();
            int count = 0;
            foreach (fuzzy_logic.Term t in result.Item2)
            {
                FunctionSeries serie1 = new FunctionSeries();
                for (double i = temp_ctrl.variables[tested_var].ledge; i <= temp_ctrl.variables[tested_var].redge; i += 0.0001)
                {
                    DataPoint data = new DataPoint(i, t.Cacldulate(i));
                    serie1.Points.Add(data);
                }
                serie1.Title = string.Format("Правило: {0}", count);
                serie1.StrokeThickness = 1.0;
                serie1.LineStyle = LineStyle.Dash;
                count++;
                Bindings.MyExpModel.Series.Add(serie1);
            }

            FunctionSeries serie_result = new FunctionSeries();
            serie_result.Title = string.Format("Результат");
            serie_result.StrokeThickness = 2.0;
            serie_result.LineStyle = LineStyle.Solid;
            serie_result.BrokenLineColor = OxyColors.Blue;

            foreach ((double, double) point in result.Item1)
            {
                serie_result.Points.Add(new DataPoint(point.Item1, point.Item2));
            }

            Bindings.MyExpModel.Series.Add(serie_result);

            foreach (defuzzify method in Enum.GetValues(typeof(defuzzify)))
            {
                Bindings.MyExpModel.Annotations.Add(
                    new LineAnnotation
                    {
                        Type = LineAnnotationType.Vertical,
                        X = Controller.Defuzzyfy(result.Item1, method),
                        MaximumY = 1.5,
                        Text = method.ToString(),
                        TextPadding = 8,
                        TextOrientation = AnnotationTextOrientation.Horizontal
                    }
                );
            }

            Bindings.MyExpModel.Axes.Add(new LinearAxis
            {
                Key = "Vertical",
                Position = OxyPlot.Axes.AxisPosition.Left,
                AbsoluteMaximum = 100,
                AbsoluteMinimum = 0,
                MinorStep = 1,
            });

            Exp_plot.Model = Bindings.MyExpModel;
        }

        private void clear_input_Click(object sender, RoutedEventArgs e)
        {
            inputs_lst.Items.Clear();
            Exp_variable.Items.Clear();
            foreach (string item in Bindings.Vars)
            {
                Exp_variable.Items.Add(item);
            }

            Exp_variable.SelectedIndex = 0;
            input.Clear();
        }
    }
}
