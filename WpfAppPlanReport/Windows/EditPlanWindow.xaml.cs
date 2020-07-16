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
    /// Логика взаимодействия для EditPlanWindow.xaml
    /// </summary>
    public partial class EditPlanWindow : Window
    {
        public Plan Plan { get; set; }
        public List<Department> Departments { get; set; }
        public EditPlanWindow()
        {
            InitializeComponent();
        }
        private void EditPlanWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            ComboBoxDepPlan.ItemsSource = Departments;
            ComboBoxDepPlan.SelectedItem = Plan.Department;
            if (Plan.Datetime != null) 
                DatePickerDatePlan.SelectedDate = (DateTime) Plan.Datetime;
            else
                DatePickerDatePlan.SelectedDate = DateTime.Now.AddDays(1);
            TextBoxTextPlan.Text = Plan.PlanText;
        }
        private void ButtonOk_OnClick(object sender, RoutedEventArgs e)
        {
            if (ComboBoxDepPlan.SelectedItem == null)
            {
                MessageBox.Show("Необходимо выбрать отдел!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Plan.Department = (Department)ComboBoxDepPlan.SelectionBoxItem;
            Plan.Datetime = DatePickerDatePlan.SelectedDate;
            Plan.PlanText = TextBoxTextPlan.Text;
            DialogResult = true;
        }
        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
