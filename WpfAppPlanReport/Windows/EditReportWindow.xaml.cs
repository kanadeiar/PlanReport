using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfAppPlanReport.EF;

namespace WpfAppPlanReport.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditReportWindow.xaml
    /// </summary>
    public partial class EditReportWindow : Window
    {
        public Report Report { get; set; }
        public string PlanText { get; set; }
        public EditReportWindow()
        {
            InitializeComponent();
        }
        private void EditReportWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            TextBlockTextPlan.Text = PlanText;
            if (Report.Datetime != null) 
                DatePickerDateReport.SelectedDate = (DateTime) Report.Datetime;
            else
                DatePickerDateReport.SelectedDate = DateTime.Now;
            TextBoxTextReport.Text = Report.ReportText;
            CheckBoxComplete.IsChecked = Report.Complete;
        }
        private void ButtonOk_OnClick(object sender, RoutedEventArgs e)
        {
            Report.Datetime = DatePickerDateReport.SelectedDate;
            Report.ReportText = TextBoxTextReport.Text;
            Report.Complete = CheckBoxComplete.IsChecked;
            DialogResult = true;
        }
        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
