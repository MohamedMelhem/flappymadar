using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace flappymadar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        double score;
        int gravity = 6;
        bool gameOver;
        Rect FlappybirdhitBox;
		public MainWindow()
        {
            InitializeComponent();
            gameTimer.Tick += Maineventtimer;
            gameTimer.Interval = TimeSpan.FromMicroseconds(20);
            StartGame();
		}

		private void Maineventtimer(object? sender, EventArgs e)
		{
	
		}

		private void KeyIsDown(object sender, KeyEventArgs e)
        {

		}
        private void KeyIsUp(object sender, KeyEventArgs e)
        {
		}
        private void StartGame()
        {

        }
        private void StopGame() { 
        }
	}
}