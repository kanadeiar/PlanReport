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
using System.Windows.Navigation;
using System.Windows.Shapes;
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
