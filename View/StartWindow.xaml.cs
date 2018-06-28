using System.Windows;

namespace HomeBudget.View
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
        }

        private void InputBudgetButton_Click_1(object sender, RoutedEventArgs e)
        {
            BudgetEditorWindow window1 = new BudgetEditorWindow();
            window1.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CurrencyConverterWindow cw = new CurrencyConverterWindow();
            cw.Show();
            this.Close();
        }

        private void chartsButton_Click(object sender, RoutedEventArgs e)
        {
            ChartsWindow cw = new ChartsWindow();
            cw.Show();
            this.Close();
        }

        private void priorityButton_Click(object sender, RoutedEventArgs e)
        {
            PriorityWindow pw = new PriorityWindow();
            pw.Show();
            this.Close();
        }
    }
}
