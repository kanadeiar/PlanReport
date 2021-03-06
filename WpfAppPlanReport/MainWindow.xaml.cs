﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using WpfAppPlanReport.EF;
using WpfAppPlanReport.Windows;

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
        private List<PlansLVModel> _listPlans = new List<PlansLVModel>();
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
            
            await ViewDataInListViewAsync();
            await ViewDataListViewReportsAsync();
            await ViewDataListViewPlansAsync();
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
        private async Task ViewDataInListViewAsync()
        {
            _listPlanReport.Clear();
            await Task.Run(() =>
            {
                using (var context = new PlanReportEntities())
                {
                    foreach (var plan in context.Plans)
                    {
                        if (_visibleNoCompleteOnly && plan.Reports.Select(r => (r.Complete == true)).Contains(true))
                            continue;
                        var repDateMax = plan.Reports.Max(r => r.Datetime);
                        var rep = plan.Reports.FirstOrDefault(r => r.Datetime == repDateMax);
                        _listPlanReport.Add(new PlanReportLVModel
                            {
                                DateTime = plan.Datetime?.ToString("dd.MM.yyyy"),
                                DepName = plan.Department.Name,
                                PlanText = plan.PlanText,
                                ReportDateTime = rep?.Datetime?.ToString("dd.MM.yyyy") ?? "отсутствует",
                                ReportText = rep?.ReportText ?? "не выполнено",
                                Complete = (rep?.Complete != null && rep.Complete.Value) ? "Да" : "Нет",
                            });
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
            await ViewDataInListViewAsync();
            ButtonRefresh.IsEnabled = true;
        }
        private async void ButtonAll_OnClick(object sender, RoutedEventArgs e)
        {
            _visibleNoCompleteOnly = false;
            ButtonRefresh.IsEnabled = false;
            await ViewDataInListViewAsync();
            ButtonRefresh.IsEnabled = true;
        }
        private async void ButtonNoComplete_OnClick(object sender, RoutedEventArgs e)
        {
            _visibleNoCompleteOnly = true;
            ButtonRefresh.IsEnabled = false;
            await ViewDataInListViewAsync();
            ButtonRefresh.IsEnabled = true;
        }
        #endregion

        #region ListViewReports

        /// <summary> Отображаемые данные по отчетам </summary>
        class ReportsLVModel
        {
            public int Id { get; set; }
            public string DepName { get; set; }
            public string DateTime { get; set; }
            public string PlanText { get; set; }
            public int ReportId { get; set; }
            public string ReportDateTime { get; set; }
            public string ReportText { get; set; }
            public string Complete { get; set; }
        }
        private async Task ViewDataListViewReportsAsync()
        {
            _listReports.Clear();
            await Task.Run(() =>
            {
                using (var context = new PlanReportEntities())
                {
                    var prevDate = DateTime.Now.AddDays(-1).Date;
                    var beginDate = DateTime.Now.Date;

                    foreach (var plan in context.Plans.Where(p =>
                        ((p.Datetime >= prevDate && p.Datetime <= beginDate) ||
                         (p.Datetime <= prevDate && p.Reports.Count == 0) ||
                         (p.Datetime <= prevDate && !(p.Reports.Select(r => r.Complete).Contains(true))) ||
                         (p.Datetime <= prevDate && p.Reports.Select(r => r.Datetime).All(d => d >= beginDate)) )))
                    {
                        var rep = plan.Reports.FirstOrDefault(r => r.Datetime != null && r.Datetime.Value.Date == DateTime.Now.Date);
                        _listReports.Add(new ReportsLVModel
                        {
                            Id = plan.Id,
                            DepName = plan.Department.Name,
                            DateTime = plan.Datetime?.ToString("dd.MM.yyyy"),
                            PlanText = plan.PlanText,
                            ReportId = rep?.Id ?? -1,
                            ReportDateTime = rep?.Datetime?.ToString("dd.MM.yyyy") ?? "отсутствует",
                            ReportText = rep?.ReportText ?? "не выполнено",
                            Complete = (rep?.Complete != null && rep.Complete.Value) ? "Да" : "Нет",
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
        private async void ButtonRefreshReport_OnClick(object sender, RoutedEventArgs e)
        {
            ButtonRefreshReport.IsEnabled = false;
            await ViewDataListViewReportsAsync();
            ButtonRefreshReport.IsEnabled = true;
        }
        private async void ButtonAddReport_OnClick(object sender, RoutedEventArgs e)
        {
            var select = (ReportsLVModel)ListViewReports.SelectedItem;
            if (select == null)
                return;
            if (select.ReportId != -1)
                return;
            using (var context = new PlanReportEntities())
            {
                var newReport = new Report
                {
                    PlanId = select.Id,
                };
                EditReportWindow window = new EditReportWindow
                {
                    Report = newReport,
                    PlanText = select.PlanText,
                    Title = "Добавление нового отчета",
                };
                window.ShowDialog();
                if (window.DialogResult.HasValue && window.DialogResult.Value)
                {
                    try
                    {
                        context.Reports.Add(newReport);
                        await context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка!\n" + ex.InnerException?.Message);
                    }
                }
            }
            await ViewDataListViewReportsAsync();
        }
        private async void ButtonEditReport_OnClick(object sender, RoutedEventArgs e)
        {
            var select = (ReportsLVModel)ListViewReports.SelectedItem;
            if (select == null)
                return;
            if (select.ReportId == -1)
                return;
            using (var context = new PlanReportEntities())
            {
                var editReport = await context.Reports.FindAsync(select.ReportId);
                EditReportWindow window = new EditReportWindow
                {
                    Report = editReport,
                    PlanText = select.PlanText,
                    Title = "Изменение выбранного отчета",
                };
                window.ShowDialog();
                if (window.DialogResult.HasValue && window.DialogResult.Value)
                {
                    try
                    {
                        await context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка!\n" + ex.InnerException?.Message);
                    }
                }
            }
            await ViewDataListViewReportsAsync();
        }
        private async void ButtonDeleteReport_OnClick(object sender, RoutedEventArgs e)
        {
            var select = (ReportsLVModel)ListViewReports.SelectedItem;
            if (select == null)
                return;
            if (select.ReportId == -1)
                return;
            using (var context = new PlanReportEntities())
            {
                var delReport = new Report {Id = select.ReportId};
                context.Entry(delReport).State = EntityState.Deleted;
                try
                {
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка!\n" + ex.InnerException?.Message);
                }
            }
            await ViewDataListViewReportsAsync();
        }
        #endregion

        #region ListViewPlans
        class PlansLVModel
        {
            public int Id { get; set; }
            public string DepName { get; set; }
            public string DateTime { get; set; }
            public string PlanText { get; set; }
        }
        private async Task ViewDataListViewPlansAsync()
        {
            _listPlans.Clear();
            await Task.Run(() =>
            {
                using (var context = new PlanReportEntities())
                {
                    var datePlusDay = DateTime.Now.AddDays(1);
                    var beginDate = DateTime.Now.Date;
                    foreach (var plan in context.Plans.Where(p =>
                        (p.Datetime == datePlusDay || 
                         p.Reports.Count == 0 ||
                         !(p.Reports.Select(r => r.Complete).Contains(true)) )))
                    {
                        _listPlans.Add(new PlansLVModel
                        {
                            Id = plan.Id,
                            DepName = plan.Department.Name,
                            DateTime = plan.Datetime?.ToString("dd.MM.yyyy"),
                            PlanText = plan.PlanText,
                        });
                    }
                }
            });
            ListViewPlans.ItemsSource = _listPlans.OrderByDescending(l => l.DateTime);
            CollectionView view =
                (CollectionView) CollectionViewSource.GetDefaultView(ListViewPlans.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("DepName");
            view.GroupDescriptions.Add(groupDescription);
        }
        private async void ButtonRefreshPlans_OnClick(object sender, RoutedEventArgs e)
        {
            ButtonRefreshPlans.IsEnabled = false;
            await ViewDataListViewPlansAsync();
            ButtonRefreshPlans.IsEnabled = true;
        }
        private async void ButtonAddPlan_OnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new PlanReportEntities())
            {
                var newPlan = new Plan();
                EditPlanWindow window = new EditPlanWindow
                {
                    Plan = newPlan,
                    Departments = context.Departments.ToList(),
                    Title = "Добавление нового плана",
                };
                window.ShowDialog();
                if (window.DialogResult.HasValue && window.DialogResult.Value)
                {
                    try
                    {
                        context.Plans.Add(newPlan);
                        await context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка!\n" + ex.Message + "\n" + ex.InnerException?.Message);
                    }
                }
            }
            await ViewDataListViewPlansAsync();
        }
        private async void ButtonEditPlan_OnClick(object sender, RoutedEventArgs e)
        {
            var select = (PlansLVModel)ListViewPlans.SelectedItem;
            if (select == null)
                return;
            using (var context = new PlanReportEntities())
            {
                var editPlan = await context.Plans.FindAsync(select.Id);
                EditPlanWindow window = new EditPlanWindow
                {
                    Plan = editPlan,
                    Departments = context.Departments.ToList(),
                    Title = "Изменение выбранного плана",
                };
                window.ShowDialog();
                if (window.DialogResult.HasValue && window.DialogResult.Value)
                {
                    try
                    {
                        await context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка!\n" + ex.Message + "\n" + ex.InnerException?.Message);
                    }
                }
            }
            await ViewDataListViewPlansAsync();
        }
        private async void ButtonDeletePlan_OnClick(object sender, RoutedEventArgs e)
        {
            var select = (PlansLVModel)ListViewPlans.SelectedItem;
            if (select == null)
                return;
            using (var context = new PlanReportEntities())
            {
                var delPlan = new Plan {Id = select.Id};
                context.Entry(delPlan).State = EntityState.Deleted;
                try
                {
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка!\n" + ex.InnerException?.Message);
                }
            }
            await ViewDataListViewPlansAsync();
        }
        #endregion

    }
}
