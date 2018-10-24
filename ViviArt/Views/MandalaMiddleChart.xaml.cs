using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Microcharts;
using SkiaSharp;
using System.ComponentModel;

namespace ViviArt
{
    public class MandalaMiddleChartInput
    {
        public int MiddleGoalID { get; set; }
        public DateTime Today { get; set; }
    }
    public class MandalaMiddleChartViewModel : INotifyPropertyChanged
    {
        public string[] colorSet = new string[]{
            "#FF3D7F",
            "#FF9E9D",
            "#DAD8A7",
            "#7FC7AF",
            "#3FB8AF", 
        };

        private string _coreGoalTitle;
        private string _middleGoalTitle;
        public string CoreGoalTitle 
        { 
            get
            {
                return _coreGoalTitle;
            } 
            set
            {
                _coreGoalTitle = value;
                OnPropertyChanged("CoreGoalTitle");
            }
        }
        public string MiddleGoalTitle 
        { 
            get
            {
                return _middleGoalTitle;
            }
            set
            {
                _middleGoalTitle = value;
                OnPropertyChanged("MiddleGoalTitle");
            }
        }

        private MandalaMiddleChartInput _inputSet;
        public MandalaMiddleChartInput InputSet { 
            get
            {
                return _inputSet;
            } 
            set
            {
                _inputSet = value;
                SetTitle();
                SetEntriesDay();
                SetEntriesWeek();
                SetEntriesMonth();
            }
        }

        public int CountDay { get; } = 17;
        public int CountWeek { get; } = 7;
        public int CountMonth { get; } = 7;

        public SKColor GetColor(MandalaArtStatistics stat, DateTime statDt, string dateType)
        {
            var timeSet = statDt.StatDtSet(dateType);
            int percent = ((stat?.Count ?? 0) * 100 / (timeSet.EndDt - timeSet.StartDt).Days);
            int idx = (percent / 20);
            idx = (idx >= colorSet.Length) ? colorSet.Length - 1 : idx;
            return SKColor.Parse(colorSet[idx]);
        }
        public void SetTitle()
        {
            var mg = DatabaseAccess.Current.GetItem<MiddleGoal>(InputSet.MiddleGoalID);
            var cg = DatabaseAccess.Current.GetItem<CoreGoal>(mg.CoreGoalID);
            CoreGoalTitle = cg?.Title ?? "*";
            MiddleGoalTitle = mg?.Title ?? "-";
        }
        public void SetEntriesDay()
        {
            List<Microcharts.Entry> entries = new List<Microcharts.Entry>();
            var timeSet = InputSet.Today.StatDtSet(DateType.Day);
            var startDt = timeSet.EndDt.AddDays(-CountDay);
            var endDt = timeSet.EndDt;

            foreach (var it in MandalaArtStatistics.GetItems(InputSet.MiddleGoalID, DateType.Day, startDt, endDt))
            {
                var statDt = it.Item1;
                var stat = it.Item2;

                entries.Add(new Microcharts.Entry(stat?.Count ?? 0)
                {
                    Label = statDt.ToString("MM/dd"),
                    ValueLabel = string.Format("{0}", stat?.Count ?? 0),
                    Color = GetColor(stat, statDt, DateType.Day),
                });
            }
            EntriesDay = entries;
        }

        public void SetEntriesWeek()
        {
            var timeSet = InputSet.Today.StatDtSet(DateType.Week);
            var startDt = timeSet.EndDt.AddDays(-7 * CountWeek);
            var endDt = timeSet.EndDt;

            List<Microcharts.Entry> entries = new List<Microcharts.Entry>();
            foreach (var it in MandalaArtStatistics.GetItems(InputSet.MiddleGoalID, DateType.Week, startDt, endDt))
            {
                var statDt = it.Item1;
                var stat = it.Item2;

                entries.Add(new Microcharts.Entry(stat?.Count ?? 0)
                {
                    Label = statDt.ToString("MM/dd"),
                    ValueLabel = string.Format("{0}", stat?.Count ?? 0),
                    Color = GetColor(stat, statDt, DateType.Week)
                });
            }
            EntriesWeek = entries; 
        }


        public void SetEntriesMonth()
        {
            var timeSet = InputSet.Today.StatDtSet(DateType.Month);
            var startDt = timeSet.EndDt.AddMonths(-CountMonth);
            var endDt = timeSet.EndDt;

            List<Microcharts.Entry> entries = new List<Microcharts.Entry>();
            foreach (var it in MandalaArtStatistics.GetItems(InputSet.MiddleGoalID, DateType.Month, startDt, endDt))
            {
                var statDt = it.Item1;
                var stat = it.Item2;

                entries.Add(new Microcharts.Entry(stat?.Count ?? 0)
                {
                    Label = statDt.ToString("MM/dd"),
                    ValueLabel = string.Format("{0}", stat?.Count ?? 0),
                    Color = GetColor(stat, statDt, DateType.Month),
                });
            }
            EntriesMonth = entries;
        }

        private List<Microcharts.Entry> _entriesDay = new List<Microcharts.Entry>();
        public List<Microcharts.Entry> EntriesDay
        {
            get
            {
                return _entriesDay;
            }
            set
            {
                _entriesDay = value;
                DayChart = new LineChart() { Entries = _entriesDay };
            }
        }

        private List<Microcharts.Entry> _entriesWeek = new List<Microcharts.Entry>();
        public List<Microcharts.Entry> EntriesWeek
        {
            get
            {
                return _entriesWeek;
            }
            set
            {
                _entriesWeek = value;
                WeekChart = new LineChart() { Entries = _entriesWeek };
            }
        }

        private List<Microcharts.Entry> _entriesMonth = new List<Microcharts.Entry>();
        public List<Microcharts.Entry> EntriesMonth
        {
            get
            {
                return _entriesMonth;
            }
            set
            {
                _entriesMonth = value;
                MonthChart = new LineChart() { Entries = _entriesMonth};
            }
        }


        private LineChart _dayChart;
        public LineChart DayChart
        {
            get
            {
                return _dayChart;
            }
            set
            {
                _dayChart = value;
                OnPropertyChanged("DayChart");
            }
        }


        private LineChart _weekChart;
        public LineChart WeekChart
        {
            get
            {
                return _weekChart;
            }
            set
            {
                _weekChart = value;
                OnPropertyChanged("WeekChart");
            }
        }

        private LineChart _monthChart;
        public LineChart MonthChart
        {
            get
            {
                return _monthChart;
            }
            set
            {
                _monthChart = value;
                OnPropertyChanged("MonthChart");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public partial class MandalaMiddleChart : ContentPage
    {
        public MandalaMiddleChartViewModel viewModel;
        public MandalaMiddleChart()
        {
            InitializeComponent();
            viewModel = new MandalaMiddleChartViewModel();
            BindingContext = viewModel;
        }
    }
}
