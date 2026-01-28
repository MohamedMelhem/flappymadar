using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Media;







namespace FlappyMadar
{
	public partial class MainWindow : Window
	{
		DispatcherTimer gameTimer = new DispatcherTimer();
		Random rnd = new Random();
		// ÚJ LISTA + RANDOM (osztályszinten)
		List<Rectangle> rainDrops = new List<Rectangle>();
		Random rainRnd = new Random();
		//eso vege
		List<Rectangle> fogLayers = new List<Rectangle>();
		Random fogRnd = new Random();
		MediaPlayer bgMusic = new MediaPlayer();

		// Játék változók

		double score;
		int gravity;
		bool gameOver;
		string selectedBird = "yellow";

		int rainForce = 0;
		//eso

		int pipeSpeed = 5;
		int pipeGap = 160;
		//nehezseg
		string difficulty = "EASY";
		//menu
		bool inMenu = true;
		//játékos név kezdése defaultként
		string playerName = "Player";


		Rect birdHitBox;
		// Konstruktor
		public MainWindow()
		{
			
			InitializeComponent();

			Loaded += Window_Loaded;

			gameTimer.Interval = TimeSpan.FromMilliseconds(20);
			gameTimer.Tick += GameLoop;
			// Fake pontszámok betöltése
			LoadFakeScores();
			// Játék indítása
			StartGame();
		}
		private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (bgMusic != null)
				bgMusic.Volume = VolumeSlider.Value;
		}
		// ESŐ
		private void SpawnRain()
		{
			// Maximum 80 esőcsepp
			if (rainDrops.Count < 80)
			{
				Rectangle drop = new Rectangle
				{
					Width = 2,
					Height = 12,
					Fill = Brushes.AliceBlue,
					Opacity = 0.7
				};

				Canvas.SetLeft(drop, rainRnd.Next(0, 520));
				Canvas.SetTop(drop, rainRnd.Next(-200, 0));

				rainDrops.Add(drop);
				MyCanvas.Children.Add(drop);
			}
		}
		// Játék indítása gomb
		private void StartButton_Click(object sender, RoutedEventArgs e)
		{
			if (selectedBird == "red")
				flappyBird.Source = new BitmapImage(new Uri("/image/redbird.png", UriKind.Relative));
			else if (selectedBird == "blue")
				flappyBird.Source = new BitmapImage(new Uri("/image/bluebird.png", UriKind.Relative));
			else
				flappyBird.Source = new BitmapImage(new Uri("/image/flappyBird.png", UriKind.Relative));

			//zene
			bgMusic.Play();
			playerName = NameBox.Text;
			//eltunik a menu
			MenuPanel.Visibility = Visibility.Collapsed;
			inMenu = false;
				//jatek inditasa
			StartGame();
		}
		// Sárga madár kiválasztása
		private void SelectYellowBird(object sender, MouseButtonEventArgs e)
		{
			selectedBird = "yellow";

			YellowBorder.BorderBrush = Brushes.Gold;
			YellowBorder.BorderThickness = new Thickness(3);

			RedBorder.BorderBrush = Brushes.White;
			RedBorder.BorderThickness = new Thickness(2);

			flappyBird.Source = new BitmapImage(
				new Uri("/image/flappyBird.png", UriKind.Relative));
		}
		//Kék madár kiválasztása
		private void SelectBlueBird(object sender, MouseButtonEventArgs e)
		{
			selectedBird = "blue";
			BlueBorder.BorderBrush = Brushes.Blue; BlueBorder.BorderThickness = new Thickness(3);
			YellowBorder.BorderBrush = Brushes.White; YellowBorder.BorderThickness = new Thickness(2);
			RedBorder.BorderBrush = Brushes.White; RedBorder.BorderThickness = new Thickness(2);

			flappyBird.Source = new BitmapImage(new Uri("/image/bluebird.png", UriKind.Relative));
		}
		//Zene lejatszasa
		private void PlayMusic_Click(object sender, RoutedEventArgs e)
		{
			bgMusic.Play();
		}
		//Zene megallitasa
		private void StopMusic_Click(object sender, RoutedEventArgs e)
		{
			bgMusic.Pause();
		}

		private void SelectRedBird(object sender, MouseButtonEventArgs e)
		{
			selectedBird = "red";

			RedBorder.BorderBrush = Brushes.Red;
			RedBorder.BorderThickness = new Thickness(3);

			YellowBorder.BorderBrush = Brushes.White;
			YellowBorder.BorderThickness = new Thickness(2);

			flappyBird.Source = new BitmapImage(
				new Uri("/image/redbird.png", UriKind.Relative));
		}
		// KÖD
		private void SpawnFog()
		{
			if (fogLayers.Count < 5) // maximum 5 köd réteg
			{
				Rectangle fog = new Rectangle
				{
					Width = 600,
					Height = 490,
					Fill = new SolidColorBrush(Color.FromArgb(30, 255, 255, 255)), 
				};

				Canvas.SetLeft(fog, 0);
				Canvas.SetTop(fog, 0);

				fogLayers.Add(fog);
				MyCanvas.Children.Add(fog);
			}
		}
		// jatek ciklus
		private void GameLoop(object sender, EventArgs e)
		{
			if (inMenu) return;
			//itt hasonlo mint  a köd csak nehezseg alapu nem pontszam hogy dinamkan valtozzon
			if (difficulty == "MEDIUM" || difficulty == "HALÁL" || difficulty == "BYE BYE" || difficulty == "ŐRÜLET")
			{
				SpawnRain();

				foreach (var drop in rainDrops.ToList())
				{
					Canvas.SetTop(drop, Canvas.GetTop(drop) + 15);

					if (Canvas.GetTop(drop) > 500)
					{
						Canvas.SetTop(drop, rainRnd.Next(-200, 0));
						Canvas.SetLeft(drop, rainRnd.Next(0, 520));
					}
				}
			}


			// KÖD ugy van meg csinava hogy ha a nehezsegi szint HALÁL vagy BYE BYE akkor jelenik meg
			if (difficulty == "HALÁL" || difficulty == "BYE BYE" )
			{
				SpawnFog();

				foreach (var fog in fogLayers)
				{
					double currentTop = Canvas.GetTop(fog);
					Canvas.SetTop(fog, currentTop); // nagyon lassú lefelé mozgás

					if (Canvas.GetTop(fog) > 0) // ha elért az aljára, reset
					{
						Canvas.SetTop(fog, 0);
					}
				}
			}

			UpdateDifficulty();
	//Külön fajta ki iirasok stb.
			txtScore.Content = $"Score: {score} | {difficulty} |";
			if (score == 10)
			{
				txtScore.Content = "Rain has been enelabed";
			}
			if (score == 20) { 
			txtScore.Content = "Fog has been enabled";
			}

			// ESŐ

			rainForce++;
			if (rainForce > 6) rainForce = 6;
			// MADÁR
			birdHitBox = new Rect(
				Canvas.GetLeft(flappyBird),
				Canvas.GetTop(flappyBird),
				flappyBird.Width,
				flappyBird.Height);

			Canvas.SetTop(flappyBird, Canvas.GetTop(flappyBird) + gravity + rainForce);

			if (Canvas.GetTop(flappyBird) < -30 || Canvas.GetTop(flappyBird) > 440)
				EndGame();

			// FELHŐK
			foreach (var cloud in MyCanvas.Children.OfType<Image>()
				.Where(x => (string)x.Tag == "cloud"))
			{
				Canvas.SetLeft(cloud, Canvas.GetLeft(cloud) - 1);
				if (Canvas.GetLeft(cloud) < -200)
					Canvas.SetLeft(cloud, 600);
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
				foreach (var drop in rainDrops.ToList())
				{
					Canvas.SetTop(drop, Canvas.GetTop(drop) + 15);

					if (Canvas.GetTop(drop) > 500)
					{
						Canvas.SetTop(drop, rainRnd.Next(-200, 0));
						Canvas.SetLeft(drop, rainRnd.Next(0, 520));
					}
				}
				if (birdHitBox.IntersectsWith(topHit) ||
					birdHitBox.IntersectsWith(bottomHit))
				{
					EndGame();
				}

			}
		}

		//Zene betoltese
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				bgMusic.Open(new Uri(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "image/backgroundmusic.mp3")));
				bgMusic.Volume = VolumeSlider.Value;
				bgMusic.MediaEnded += (s, ev) => bgMusic.Position = TimeSpan.Zero;
				bgMusic.Play();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Zene betöltése sikertelen: " + ex.Message);
			}
		}

		// csőpár resetelése

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
			if (score >= 30)
			{
				difficulty = "BYE BYE";
				pipeSpeed = 20;
				pipeGap = 100;
				rainForce = 6;
				//Itt van a legnehezebb szint


			}
			if (score >= 20)
			{
				difficulty = "HALÁL";
				pipeSpeed = 9;
				pipeGap = 110;
				rainForce = 3;
				fogLayers.Capacity = 5;
			}
			else if (score >= 10)
			{
				difficulty = "MEDIUM";
				pipeSpeed = 7;
				pipeGap = 130;
				rainForce = 3;
			}
			else
			{
				difficulty = "EASY";
				pipeSpeed = 7;
				pipeGap = 170;
				rainForce = 0;
				fogLayers.Clear();
			}
		}
		// billentyuzet lenyomas es felengedes kezelese
		//Ezeket ne vltoztasd
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
		//itt ugyanaz csak lefele



		//igazabol csak a space a fontos a tobbi csak extra funkcio és ugye annyi mezovel megy fel a madar
		private void KeyIsUp(object sender, KeyEventArgs e)
		{
			gravity = 6;
			flappyBird.RenderTransform =
				new RotateTransform(10, flappyBird.Width / 2, flappyBird.Height / 2);
		}
		// jatek inditasa
		//Itt meg lehet adni a fake pontszamokat ide barmit irhatok ezeket is csak legenraltam
		private void LoadFakeScores()
		{
			ScoreList.Items.Add("🥇 IsMeGehh77 - 90 pont");
			ScoreList.Items.Add("🥈 FlappyPro - 72 pont");
			ScoreList.Items.Add("🥉 MadarKiller - 55 pont");
			ScoreList.Items.Add("NoobPlayer - 4 pont");
			ScoreList.Items.Add("PipeHater - 21 pont");
		}
		// jatek inditasa
		//ugye itt a rainForce az eso erossege a score a pontszam gravity a gravitacio es a tobbi valtozo is itt van inicializalva
		private void StartGame()
		{
			rainForce = 0;
			score = 0;
			gravity = 6;
			gameOver = false;

			pipeSpeed = 5;
			pipeGap = 160;
			difficulty = "EASY";
			//ugye easy be kezdjuk
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
			foreach (var fog in fogLayers)
				MyCanvas.Children.Remove(fog);
			fogLayers.Clear();

			foreach (var r in rainDrops)
				MyCanvas.Children.Remove(r);
			rainDrops.Clear();

			gameTimer.Start();
			MyCanvas.Focus();
		}


		// jatek vege
		// a Gamerover az a ciklusban van meghivva ha a madar neki megy a csoveknek vagy kimegy a jatek teruletrol
		/// <summary>
		/// Gametimer stop es a pontszam kiirasa a listaba
		/// menu panel az vissza állit
		/// 
		/// </summary>

		private void EndGame()
		{
			if (gameOver) return;

			gameOver = true;
			gameTimer.Stop();
			ScoreList.Items.Insert(0, $"🔥 {playerName} - {score} pont");
			MenuPanel.Visibility = Visibility.Visible;
			inMenu = true;

		}


	}
}


//Vége