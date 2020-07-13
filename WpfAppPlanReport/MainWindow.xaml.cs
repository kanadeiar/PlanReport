using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using WpfAppPlanReport.EF;

namespace WpfAppPlanReport
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<PlanReportLVModel> _listPlanReport = new List<PlanReportLVModel>();
        private bool _visibleNoCompleteOnly;
        private List<ReportsLVModel> _listReports = new List<ReportsLVModel>();
        public MainWindow()
        {
            InitializeComponent();
        }
        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!Database.Exists("name=PlanreportEntities"))
            {
                if (MessageBox.Show("Отсутствует база данных 'planreport'! Создать заново?", "Новая база данных",
                    MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    Close();
                    return;
                }
                else
                    Database.SetInitializer(new DropCreateDatabaseAlways<PlanReportEntities>());
            }
            
            await ViewDataInListView();
        }
        #region ListViewAll
        /// <summary> Отображаемые данные по всем </summary>
        class PlanReportLVModel
        {
            public string DateTime { get; set; }
            public string DepName { get; set; }
            public string PlanText { get; set; }
            public string ReportDateTime { get; set; }
            public string ReportText { get; set; }
            public string Complete { get; set; }
        }
        private async Task ViewDataInListView()
        {
            _listPlanReport.Clear();
            await Task.Run(() =>
            {
                using (var context = new PlanReportEntities())
                {
                    foreach (var plan in context.Plans)
                    {
                        if (plan.Reports.Count == 0)
                        {
                            _listPlanReport.Add(new PlanReportLVModel
                            {
                                DateTime = plan.Datetime?.ToString("dd.MM.yyyy"),
                                DepName = plan.Department.Name,
                                PlanText = plan.PlanText,
                                ReportDateTime = "не выполнено",
                                ReportText = "не выполнено",
                                Complete = "Нет",
                            });
                        }
                        else
                        {
                            if (_visibleNoCompleteOnly && plan.Reports.Select(p => (p.Complete == true)).Contains(true))
                                continue;
                            foreach (var rep in plan.Reports)
                            {
                                _listPlanReport.Add(new PlanReportLVModel
                                {
                                    DateTime = plan.Datetime?.ToString("dd.MM.yyyy"),
                                    DepName = plan.Department.Name,
                                    PlanText = plan.PlanText,
                                    ReportDateTime = rep.Datetime?.ToString("dd.MM.yyyy"),
                                    ReportText = rep.ReportText ?? "не выполнено",
                                    Complete = (rep.Complete != null && rep.Complete.Value) ? "Да" : "Нет",
                                });
                            }
                        }
                    }
                }
            });
            ListViewAll.ItemsSource = _listPlanReport.OrderByDescending(l => l.DateTime);
            CollectionView view =
                (CollectionView) CollectionViewSource.GetDefaultView(ListViewAll.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("DateTime");
            view.GroupDescriptions.Add(groupDescription);
        }

        private async void ButtonRefresh_OnClick(object sender, RoutedEventArgs e)
        {
            ButtonRefresh.IsEnabled = false;
            await ViewDataInListView();
            ButtonRefresh.IsEnabled = true;
        }
        private async void ButtonAll_OnClick(object sender, RoutedEventArgs e)
        {
            _visibleNoCompleteOnly = false;
            ButtonRefresh.IsEnabled = false;
            await ViewDataInListView();
            ButtonRefresh.IsEnabled = true;
        }
        private async void ButtonNoComplete_OnClick(object sender, RoutedEventArgs e)
        {
            _visibleNoCompleteOnly = true;
            ButtonRefresh.IsEnabled = false;
            await ViewDataInListView();
            ButtonRefresh.IsEnabled = true;
        }
        #endregion

        #region ListViewReports

        /// <summary> Отображаемые данные по отчетам </summary>
        class ReportsLVModel
        {
            public string DepName { get; set; }
            public string DateTime { get; set; }
            public string PlanText { get; set; }
            public string ReportDateTime { get; set; }
            public string ReportText { get; set; }
            public string Complete { get; set; }
        }
        private async Task ViewDataListViewReports()
        {
            _listReports.Clear();
            await Task.Run(() =>
            {
                using (var context = new PlanReportEntities())
                {
                    foreach (var plan in context.Plans.Where(p =>
                        (p.Datetime == DateTime.Now || p.Reports.Count == 0 ||
                         !(p.Reports.Select(r => r.Complete).Contains(true)))))
                    {
                        
                        _listReports.Add(new ReportsLVModel
                        {
                            DepName = plan.Department.Name,
                            DateTime = plan.Datetime?.ToString("dd.MM.yyyy"),
                            PlanText = plan.PlanText,
                            
                        });
                    }
                }
            });

            ListViewReports.ItemsSource = _listReports.OrderByDescending(l => l.DateTime);
            CollectionView view =
                (CollectionView) CollectionViewSource.GetDefaultView(ListViewReports.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("DepName");
            view.GroupDescriptions.Add(groupDescription);
        }
        #endregion

    }
}
