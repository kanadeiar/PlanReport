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
        private List<PlanReportListView> _listPlan = new List<PlanReportListView>();
        private bool _visibleNoCompleteOnly;
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
                    Database.SetInitializer(new DropCreateDatabaseAlways<PlanreportEntities>());
            }
            await ViewDataInListView();
        }

        #region ListViewAll
        private async Task ViewDataInListView()
        {
            _listPlan.Clear();
            await Task.Run(() =>
            {
                using (var context = new PlanreportEntities())
                {
                    foreach (var dep in context.Departments)
                    {
                        foreach (var plan in dep.Plans)
                        {
                            if (plan.Reports.Count == 0)
                            {
                                _listPlan.Add(new PlanReportListView
                                {
                                    DateTime = plan.Datetime?.ToString("dd.MM.yyyy"),
                                    DepName = dep.Name,
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
                                    _listPlan.Add(new PlanReportListView
                                    {
                                        DateTime = plan.Datetime?.ToString("dd.MM.yyyy"),
                                        DepName = dep.Name,
                                        PlanText = plan.PlanText,
                                        ReportDateTime = rep.Datetime?.ToString("dd.MM.yyyy"),
                                        ReportText = rep.ReportText ?? "не выполнено",
                                        Complete = (rep.Complete != null && rep.Complete.Value) ? "Да" : "Нет",
                                    });
                                }
                            }
                        }
                    }
                }
            });
            ListViewAll.ItemsSource = _listPlan.OrderByDescending(l => l.DateTime);
            CollectionView view =
                (CollectionView) CollectionViewSource.GetDefaultView(ListViewAll.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("DateTime");
            view.GroupDescriptions.Add(groupDescription);
        }
        class PlanReportListView
        {
            public string DateTime { get; set; }
            public string DepName { get; set; }
            public string PlanText { get; set; }
            public string ReportDateTime { get; set; }
            public string ReportText { get; set; }
            public string Complete { get; set; }
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


    }
}
