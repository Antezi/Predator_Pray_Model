using System;
using System.Collections.Generic;
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
    private int gen = 0;
    private List<double> prayCountList = new List<double>();
    private List<double> predatorCountList = new List<double>();
    private double startPrayCount = 500, startPredatorCount = 75;
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
        try
        {
            this.e = Convert.ToDouble(_prayBirth.Text);
            B = Convert.ToDouble(_predatorDeath.Text);
            a = Convert.ToDouble(_killingBox.Text);
            d = Convert.ToDouble(_deathPrayBox.Text);
        }
        catch
        {
            return;
        }

        // Количество жертв
        for (int i = 0; i < 150; i++)
        {
            if (gen == 0)
            {
                prayCount = (this.e - a * startPredatorCount) * startPrayCount + startPrayCount;
                gen++;
                prayCountList.Add(prayCount);
            }
            else
            {
                prayCount = (this.e - a * predatorCount) * prayCount + prayCount;
                gen++;
                prayCountList.Add(prayCount);
            }
            
            if (gen == 1)
            {
                predatorCount = (d * prayCountList[i] - B) * startPredatorCount + startPredatorCount;
                predatorCountList.Add(predatorCount);
            }

            else
            {
                predatorCount = (d * prayCountList[i] - B) * predatorCount + predatorCount;
                predatorCountList.Add(predatorCount);
            }
        }
        UpdatePlotData();
    }
    
    private void UpdatePlotData()
    {
        //mainPlotModel.Series.Clear();
        //phasePlotModel.Series.Clear();

        var predatorsSeries = new LineSeries { Title = "Predators" };
        var preysSeries = new LineSeries { Title = "Preys" };
        var phaseSeries = new LineSeries { Title = "Predators vs Preys" };

        // Добавляем данные из prayCountList и predatorCountList в серии для основной диаграммы
        for (int i = 0; i < prayCountList.Count; i++)
        {
            predatorsSeries.Points.Add(new DataPoint(i, predatorCountList[i]));
            preysSeries.Points.Add(new DataPoint(i, prayCountList[i]));
        }

        // Добавляем данные из prayCountList и predatorCountList в серию для фазового портрета
        for (int i = 0; i < prayCountList.Count; i++)
        {
            phaseSeries.Points.Add(new DataPoint(predatorCountList[i], prayCountList[i]));
        }

        mainPlotModel.Series.Add(predatorsSeries);
        mainPlotModel.Series.Add(preysSeries);
        phasePlotModel.Series.Add(phaseSeries);

        // Обновляем графики
        _plotModel.InvalidatePlot();
        _phaseModel.InvalidatePlot();
    }
    
    private PlotModel CreateMainPlotModel()
    {
        var plotModel = new PlotModel { Title = "Population Dynamics" };
        // Убираем добавление серий в модель графика
        return plotModel;
    }

    private PlotModel CreatePhasePlotModel()
    {
        var plotModel = new PlotModel { Title = "Phase Portrait" };
        // Убираем добавление серий в модель графика
        return plotModel;
    }
}