using System.Windows;

namespace MemoryGame
{
    public partial class MainWindow : Window
    {
        GameWindow gw;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            gw = new GameWindow();
            if (gw.ShowDialog() == false)
                Show();
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
