using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;
using OxyPlot;
using OxyPlot.Avalonia;
using LineSeries = OxyPlot.Series.LineSeries;

namespace Predator_Pray_Model;

public partial class MainWindow : Window
{
    private TextBox _prayBirth, _predatorDeath, _killingBox, _deathPrayBox;
    private double e, B, a, d, prayCount, predatorCount;
    private int iter = 0;
    private int gen = 0;
    private List<double> prayCountList = new List<double>();
    private List<double> predatorCountList = new List<double>(); 
    private double[,] prayCounts, predatorCounts;
    private double startPrayCount = 500, startPredatorCount = 75;
    private double lastPray, lastPredator;
    private PlotModel mainPlotModel, phasePlotModel;
    private PlotView _plotModel, _phaseModel;

    public MainWindow()
    {

        MinHeight = 450;
        MaxHeight = 450;
        MinWidth = 1200;
        MaxWidth = 1200;

        InitializeComponent();

        _prayBirth = this.FindControl<TextBox>("PrayBirth");
        _predatorDeath = this.FindControl<TextBox>("PredatorDeath");
        _killingBox = this.FindControl<TextBox>("KillingBox");
        _deathPrayBox = this.FindControl<TextBox>("DeathPrayBox");
        _plotModel = this.FindControl<PlotView>("MainPlot");
        _phaseModel = this.FindControl<PlotView>("PhasePlot");
        
        mainPlotModel = CreateMainPlotModel();
        phasePlotModel = CreatePhasePlotModel();
        MainPlot.Model = mainPlotModel;
        PhasePlot.Model = phasePlotModel;
    }

    private void GenerateButton_OnClick(object? sender, RoutedEventArgs e)
    {
        mainPlotModel = CreateMainPlotModel();
        phasePlotModel = CreatePhasePlotModel();
        MainPlot.Model = mainPlotModel;
        PhasePlot.Model = phasePlotModel;
        gen = 0;
        
        try
        {
           this.e =  double.Parse(_prayBirth.Text, CultureInfo.InvariantCulture);
           B = double.Parse(_predatorDeath.Text, CultureInfo.InvariantCulture);
           a = double.Parse(_killingBox.Text, CultureInfo.InvariantCulture);
           d = double.Parse(_deathPrayBox.Text, CultureInfo.InvariantCulture);
           
            //this.e = Convert.ToDouble(_prayBirth.Text);
            //B = Convert.ToDouble(_predatorDeath.Text);
            //a = Convert.ToDouble(_killingBox.Text);
            //d = Convert.ToDouble(_deathPrayBox.Text);
        }
        catch
        {
            return;
        }
        var predatorsSeries = new LineSeries { Title = "Predators" };
        var preysSeries = new LineSeries { Title = "Preys" };
        var phaseSeries = new LineSeries { Title = "Predators vs Preys" };
        
        prayCounts = new double[3, 150];
        predatorCounts = new double[3, 150];
        // Количество жертв
        for (int j = 0; j < 3; j++)
        {
            for (int i = 0; i < 150; i++)
            {
                if (j == 1 && i == 0)
                {
                    prayCount = (this.e - a * 150) * startPrayCount + startPrayCount;
                    iter++;
                    prayCounts[j, i] = prayCount;
                }
                else if (j == 2 && i == 0)
                {
                    prayCount = (this.e - a * 100) * startPrayCount + startPrayCount;
                    iter++;
                    prayCounts[j, i] = prayCount;
                }
                else if (j == 0 && i == 0)
                {
                    prayCount = (this.e - a * startPredatorCount) * startPrayCount + startPrayCount;
                    iter++;
                    prayCounts[j, i] = prayCount;
                }
                else
                {
                    prayCount = (this.e - a * predatorCount) * prayCount + prayCount;
                    iter++;
                    prayCounts[j, i] = prayCount;
                }

                if (j == 1 && i == 0)
                {
                    predatorCount = (d * startPrayCount - B) * 150 + 150;
                    predatorCounts[j, i] = predatorCount;
                }
                else if (j == 2 && i == 0)
                {
                    predatorCount = (d * startPrayCount - B) * 100 + 100;
                    predatorCounts[j, i] = predatorCount;
                }
                else if (iter == 1)
                {
                    predatorCount = (d * prayCounts[j, i] - B) * startPredatorCount + startPredatorCount;
                    predatorCounts[j, i] = predatorCount;
                }

                else
                {
                    predatorCount = (d * prayCounts[j, i] - B) * predatorCount + predatorCount;
                    predatorCounts[j, i] = predatorCount;
                } 
            
            }
            UpdatePlotData();
        }

        UpdateMainPlotModel();
    }
    
    private void UpdatePlotData()
    {
        var phaseSeries = new LineSeries { Title = "Predators vs Preys" };
        
        for (int i = 0; i < 150; i++)
        {
            phaseSeries.Points.Add(new DataPoint(predatorCounts[gen, i], prayCounts[gen, i]));
        }
        gen++;
        
        phasePlotModel.Series.Add(phaseSeries);
        
        _phaseModel.InvalidatePlot();
    }

    private void UpdateMainPlotModel()
    {
        var predatorsSeries = new LineSeries { Title = "Охотники" };
        var preysSeries = new LineSeries { Title = "Жертвы" };

        int iter = 0;
        for (int j = 0; j < 3; j++)
        {
            for (int i = 0; i < 150; i++)
            {
                predatorsSeries.Points.Add(new DataPoint(iter, predatorCounts[j, i]));
                preysSeries.Points.Add(new DataPoint(iter, prayCounts[j, i]));
                iter++;
            }
        }
        
        mainPlotModel.Series.Add(predatorsSeries);
        mainPlotModel.Series.Add(preysSeries);
        
        _plotModel.InvalidatePlot();
    }
    
    private PlotModel CreateMainPlotModel()
    {
        var plotModel = new PlotModel { Title = "Динамика популяции" };
        // Убираем добавление серий в модель графика
        return plotModel;
    }

    private PlotModel CreatePhasePlotModel()
    {
        var plotModel = new PlotModel { Title = "Фазовый портрет" };
        // Убираем добавление серий в модель графика
        return plotModel;
    }
}