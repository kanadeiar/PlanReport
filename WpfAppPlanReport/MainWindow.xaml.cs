using System.Data.Entity;
using System.Linq;
using System.Windows;
using WpfAppPlanReport.EF;

namespace WpfAppPlanReport
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
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
            using (var context = new PlanreportEntities())
            {
                foreach (var dep in context.Departments)
                {
                    foreach (var plan in dep.Plans)
                    {
                        string s = "не выполнено";
                        if (plan.Reports.Count != 0)
                            s = plan.Reports.First().ReportText;
                        ListBoxTest.Items.Add($"{dep.Name} {plan.Datetime} {plan.PlanText} {s}");
                    }
                }
            }


        }
    }
}
