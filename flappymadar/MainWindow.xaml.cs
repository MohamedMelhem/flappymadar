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
		Random rnd = new Random();

		double score;
		int gravity;
		bool gameOver;

		int pipeSpeed = 5;
		int pipeGap = 160;
		string difficulty = "EASY";

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
			UpdateDifficulty();
			txtScore.Content = $"Score: {score} | {difficulty}";

			// MADÁR
			birdHitBox = new Rect(
				Canvas.GetLeft(flappyBird),
				Canvas.GetTop(flappyBird),
				flappyBird.Width,
				flappyBird.Height);

			Canvas.SetTop(flappyBird, Canvas.GetTop(flappyBird) + gravity);

			if (Canvas.GetTop(flappyBird) < -30 || Canvas.GetTop(flappyBird) > 440)
				EndGame();

			// FELHŐK
			foreach (var cloud in MyCanvas.Children.OfType<Image>()
				.Where(x => (string)x.Tag == "cloud"))
			{
				Canvas.SetLeft(cloud, Canvas.GetLeft(cloud) - 1);
				if (Canvas.GetLeft(cloud) < -200)
					Canvas.SetLeft(cloud, 550);
			}

			// CSÖVEK (obj1, obj2, obj3)
			foreach (string tag in new[] { "obj1", "obj2", "obj3" })
			{
				var pipes = MyCanvas.Children.OfType<Image>()
					.Where(x => (string)x.Tag == tag)
					.ToList();

				if (pipes.Count != 2) continue;

				Image topPipe = pipes.First(p => Canvas.GetTop(p) < 0);
				Image bottomPipe = pipes.First(p => Canvas.GetTop(p) > 0);

				Canvas.SetLeft(topPipe, Canvas.GetLeft(topPipe) - pipeSpeed);
				Canvas.SetLeft(bottomPipe, Canvas.GetLeft(bottomPipe) - pipeSpeed);

				if (Canvas.GetLeft(topPipe) < -80)
				{
					ResetPipePair(topPipe, bottomPipe, 900);
					score++;
				}

				Rect topHit = new Rect(Canvas.GetLeft(topPipe), Canvas.GetTop(topPipe),
									   topPipe.Width, topPipe.Height);

				Rect bottomHit = new Rect(Canvas.GetLeft(bottomPipe), Canvas.GetTop(bottomPipe),
										  bottomPipe.Width, bottomPipe.Height);

				if (birdHitBox.IntersectsWith(topHit) ||
					birdHitBox.IntersectsWith(bottomHit))
				{
					EndGame();
				}
			}
		}

		private void ResetPipePair(Image top, Image bottom, double x)
		{
			int topY = rnd.Next(-280, -120);

			Canvas.SetLeft(top, x);
			Canvas.SetTop(top, topY);

			Canvas.SetLeft(bottom, x);
			Canvas.SetTop(bottom, topY + top.Height + pipeGap);
		}
		// itt lehet allitani a nehezseget
		// a pipeSpeed a csovek sebessege
		// a pipeGap a csovek kozotti tavolsag
		// a score pedig a pontszam
		// a difficulty pedig a nehezsegi szint neve
		private void UpdateDifficulty()
		{
			if (score >= 25)
			{
				difficulty = "BYE BYE";
				pipeSpeed = 20;
				pipeGap = 100;
			}
			if (score >= 20)
			{
				difficulty = "HALÁL";
				pipeSpeed = 9;
				pipeGap = 110;
			}
			else if (score >= 10)
			{
				difficulty = "MEDIUM";
				pipeSpeed = 7;
				pipeGap = 130;
			}
			else
			{
				difficulty = "EASY";
				pipeSpeed = 7;
				pipeGap = 170;
			}
		}
		// billentyuzet lenyomas es felengedes kezelese
		private void KeyIsDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Space && !gameOver)
			{
				gravity = -8;
				flappyBird.RenderTransform =
					new RotateTransform(-20, flappyBird.Width / 2, flappyBird.Height / 2);
			}

			if (e.Key == Key.R && gameOver)
				StartGame();
		}

		private void KeyIsUp(object sender, KeyEventArgs e)
		{
			gravity = 6;
			flappyBird.RenderTransform =
				new RotateTransform(10, flappyBird.Width / 2, flappyBird.Height / 2);
		}
		// jatek inditasa
	
		private void StartGame()
		{
			score = 0;
			gravity = 6;
			gameOver = false;

			pipeSpeed = 5;
			pipeGap = 160;
			difficulty = "EASY";

			Canvas.SetTop(flappyBird, 190);

			double startX = 400;

			foreach (string tag in new[] { "obj1", "obj2", "obj3" })
			{
				var pipes = MyCanvas.Children.OfType<Image>()
					.Where(x => (string)x.Tag == tag)
					.ToList();

				if (pipes.Count != 2) continue;

				Image topPipe = pipes.First(p => Canvas.GetTop(p) < 0);
				Image bottomPipe = pipes.First(p => Canvas.GetTop(p) > 0);

				ResetPipePair(topPipe, bottomPipe, startX);
				startX += 250;
			}

			gameTimer.Start();
			MyCanvas.Focus();
		}
		// jatek vege

		private void EndGame()
		{
			if (gameOver) return; 

			gameOver = true;
			gameTimer.Stop();
			txtScore.Content += $" || Game Over || Press R!Pontod :  {score} ";
		}
	}
}


//Vége