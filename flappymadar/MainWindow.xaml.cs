using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace FlappyMadar
{
	public partial class MainWindow : Window
	{
		DispatcherTimer gameTimer = new DispatcherTimer();

		double score;
		int gravity;
		bool gameOver;
		Rect birdHitBox;

		public MainWindow()
		{
			InitializeComponent();

			Loaded += (s, e) => MyCanvas.Focus();

			gameTimer.Interval = TimeSpan.FromMilliseconds(20);
			gameTimer.Tick += GameLoop;

			StartGame();
		}

		private void GameLoop(object sender, EventArgs e)
		{
			txtScore.Content = "Score: " + score;

			birdHitBox = new Rect(Canvas.GetLeft(flappyBird),
								  Canvas.GetTop(flappyBird),
								  flappyBird.Width,
								  flappyBird.Height);

			Canvas.SetTop(flappyBird, Canvas.GetTop(flappyBird) + gravity);

			if (Canvas.GetTop(flappyBird) < -30 || Canvas.GetTop(flappyBird) > 440)
			{
				EndGame();
			}

			foreach (var x in MyCanvas.Children.OfType<Image>())
			{
				if (x.Tag == null) continue;

				if ((string)x.Tag == "obj1" || (string)x.Tag == "obj2" || (string)x.Tag == "obj3")
				{
					Canvas.SetLeft(x, Canvas.GetLeft(x) - 5);

					if (Canvas.GetLeft(x) < -80)
					{
						Canvas.SetLeft(x, 600);
						score += 0.5;
					}

					Rect pipeHitBox = new Rect(Canvas.GetLeft(x),
											   Canvas.GetTop(x),
											   x.Width,
											   x.Height);

					if (birdHitBox.IntersectsWith(pipeHitBox))
					{
						EndGame();
					}
				}

				if ((string)x.Tag == "cloud")
				{
					Canvas.SetLeft(x, Canvas.GetLeft(x) - 1);

					if (Canvas.GetLeft(x) < -200)
					{
						Canvas.SetLeft(x, 550);
					}
				}
			}
		}

		private void KeyIsDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Space && !gameOver)
			{
				gravity = -8;
				flappyBird.RenderTransform =
					new RotateTransform(-20, flappyBird.Width / 2, flappyBird.Height / 2);
			}

			if (e.Key == Key.R && gameOver)
			{
				StartGame();
			}
		}

		private void KeyIsUp(object sender, KeyEventArgs e)
		{
			gravity = 8;
			flappyBird.RenderTransform =
				new RotateTransform(10, flappyBird.Width / 2, flappyBird.Height / 2);
		}

		private void StartGame()
		{
			score = 0;
			gravity = 8;
			gameOver = false;

			Canvas.SetTop(flappyBird, 190);

			int pipeX = 400;
			foreach (var x in MyCanvas.Children.OfType<Image>())
			{
				if (x.Tag == null) continue;

				if ((string)x.Tag == "obj1" || (string)x.Tag == "obj2" || (string)x.Tag == "obj3")
				{
					Canvas.SetLeft(x, pipeX);
					pipeX += 200;
				}
			}

			gameTimer.Start();
			MyCanvas.Focus();
		}

		private void EndGame()
		{
			if (gameOver) return;

			gameOver = true;
			gameTimer.Stop();
			txtScore.Content += "  GAME OVER - Press R";
		}
	}
}
