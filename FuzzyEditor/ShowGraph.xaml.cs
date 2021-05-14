using OxyPlot;
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
    /// Interaction logic for ShowGraph.xaml
    /// </summary>
    public partial class ShowGraph : Window
    {
        public ShowGraph(fuzzy_logic.Variable fvar, string fvar_name)
        {

            Bindings.MyModel = new PlotModel { Title = fvar_name };

            FunctionSeries serie = new FunctionSeries();
            foreach (var point in fvar.terms)
            {
                FunctionSeries serie1 = new FunctionSeries();
                for (double i = fvar.ledge; i <= fvar.redge; i += 0.001)
                {
                    DataPoint data = new DataPoint(i, point.Value.function.Calculate(i));
                    serie1.Points.Add(data);
                }

                serie1.Title = point.Key;
                serie1.StrokeThickness = 1.5;
                Bindings.MyModel.Series.Add(serie1);
            }

            InitializeComponent();
        }
    }
}
