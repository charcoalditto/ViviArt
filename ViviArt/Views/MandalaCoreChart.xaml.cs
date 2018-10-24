using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Microcharts;
using SkiaSharp;
using System.ComponentModel;


namespace ViviArt
{
    public class MandalaCoreChartInput
    {
        public int CoreGoalID { get; set; }
        public DateTime Today { get; set; }
    }
    public class MandalaCoreChartViewModel : INotifyPropertyChanged
    {
        public string[] colorSet = new string[]{
            "#FF3D7F",
            "#FF9E9D",
            "#DAD8A7",
            "#7FC7AF",
            "#3FB8AF",
        };

        private string _coreGoalTitle;
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

        private MandalaCoreChartInput _inputSet;
        public MandalaCoreChartInput InputSet
        {
            get
            {
                return _inputSet;
            }
            set
            {
                _inputSet = value;
                SetTitle();

                SetEntries(DateType.Week);
                SetEntries(DateType.Month);
                SetEntries(DateType.Year);
            }
        }
        public void SetTitle()
        {
            var cg = DatabaseAccess.Current.GetItem<CoreGoal>(InputSet.CoreGoalID);
            CoreGoalTitle = cg?.Title ?? "*";
        }
        public SKColor GetColor(MandalaArtStatistics stat, DateTime statDt, string dateType)
        {
            var timeSet = statDt.StatDtSet(dateType);
            int percent = ((stat?.Count ?? 0) * 100 / (timeSet.EndDt - timeSet.StartDt).Days);
            int idx = (percent / 20);
            idx = (idx >= colorSet.Length) ? colorSet.Length - 1 : idx;
            return SKColor.Parse(colorSet[idx]);
        }

        public void SetEntries(string dateType)
        {
            List<Microcharts.Entry> entries = new List<Microcharts.Entry>();
            var statDt = InputSet.Today.StatDt(dateType);

            for (int middlePosition = 0; middlePosition < 9; middlePosition++)
            {
                if (middlePosition == 4) continue;
                var mg = MiddleGoal.GetItem(InputSet.CoreGoalID, middlePosition);
                var stat = MandalaArtStatistics.GetItem(mg?.ID ?? -1, dateType, statDt);
                entries.Add(new Microcharts.Entry(stat?.Count ?? 0)
                {
                    Label = mg?.Title ?? "-",
                    ValueLabel = string.Format("{0}", stat?.Count ?? 0),
                    Color = GetColor(stat, statDt, dateType),
                });
            }
            switch (dateType)
            {
                case DateType.Week:
                    EntriesWeek = entries;
                    break;
                case DateType.Month:
                    EntriesMonth = entries;
                    break;
                case DateType.Year:
                    EntriesYear = entries;
                    break;
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
                WeekChart = new RadarChart() { Entries = _entriesWeek };
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
                MonthChart = new RadarChart() { Entries = _entriesMonth };
            }
        }

        private List<Microcharts.Entry> _entriesYear = new List<Microcharts.Entry>();
        public List<Microcharts.Entry> EntriesYear
        {
            get
            {
                return _entriesYear;
            }
            set
            {
                _entriesYear = value;
                YearChart = new RadarChart() { Entries = _entriesYear };
            }
        }

        private RadarChart _weekChart;
        public RadarChart WeekChart
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

        private RadarChart _monthChart;
        public RadarChart MonthChart
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

        private RadarChart _yearChart;
        public RadarChart YearChart
        {
            get
            {
                return _yearChart;
            }
            set
            {
                _yearChart = value;
                OnPropertyChanged("YearChart");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public partial class MandalaCoreChart : ContentPage
    {
        public MandalaCoreChartViewModel viewModel;
        public MandalaCoreChart()
        {
            InitializeComponent();
            viewModel = new MandalaCoreChartViewModel();
            BindingContext = viewModel;
        }
    }
}
