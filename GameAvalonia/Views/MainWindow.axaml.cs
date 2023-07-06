using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls.Shapes;
using Avalonia.Media.Imaging;
using NAudio.Wave;

namespace GameAvalonia.Views
{
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        bool moveLeft, moveRight;
        List<Rectangle> itemRemover = new List<Rectangle>();

        Random rand = new Random();

        int enemySpriteCounter = 0;
        int enemyCounter = 100;
        int playerSpeed = 15;
        int limit = 50;
        int score = 0;
        int health = 100;
        int enemySpeed = 10;

        private WaveOutEvent waveOut;

        Rect playerHitBox;

        

        public MainWindow()
        {
            InitializeComponent();

            scoreText = this.FindControl<TextBlock>("scoreText");
            healthText = this.FindControl<TextBlock>("healthText");
            _canvas = this.FindControl<Canvas>("_canvas");
            _player = this.FindControl<Rectangle>("_player");
            waveOut = new WaveOutEvent();

            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

            Focus();

            ImageBrush bg = new ImageBrush();

            bg.Source = new Bitmap("Assets/Images/background.png");
            bg.Stretch = Stretch.UniformToFill;
            _canvas.Background = bg;

            ImageBrush playerImage = new ImageBrush();
            playerImage.Source = new Bitmap("Assets/Images/x-wing.png");
            _player.Fill = playerImage;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void GameLoop(object sender, EventArgs e)
        {
            playerHitBox = new Rect(Canvas.GetLeft(_player), Canvas.GetTop(_player), _player.Width, _player.Height);
            enemyCounter -= 1;
            scoreText.Text = "Score: " + score;

            healthText.Text = "Health " + health;

            if (enemyCounter < 0)
            {
                MakeEnemies();
                enemyCounter = limit;
            }

            if (moveLeft == true && Canvas.GetLeft(_player) > 0)
            {
                Canvas.SetLeft(_player, Canvas.GetLeft(_player) - playerSpeed);
            }
            if (moveRight == true && Canvas.GetLeft(_player) + 90 < Width)
            {
                Canvas.SetLeft(_player, Canvas.GetLeft(_player) + playerSpeed);
            }

                foreach (var x in _canvas.Children.OfType<Rectangle>())
                {
                    if (x is Rectangle && (string)x.Tag == "laser")
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) - 60);

                        Rect laserHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                        if (Canvas.GetTop(x) < 10)
                        {
                            itemRemover.Add(x);
                        }

                        foreach (var y in _canvas.Children.OfType<Rectangle>())
                        {
                            if (y is Rectangle && (string)y.Tag == "enemy")
                            {
                                Rect enemyHit = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                                if (laserHitBox.Intersects(enemyHit))
                                {
                                    itemRemover.Add(x);
                                    itemRemover.Add(y);
                                    score++;

                                using (var audioFileHit = new AudioFileReader("Assets/Sounds/explosion.mp3"))
                                {
                                    waveOut.Stop();
                                    waveOut.Init(audioFileHit);
                                    waveOut.Play();
                                }
                            }
                            }
                        }
                    }

                    if (x is Rectangle && (string)x.Tag == "enemy")
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) + enemySpeed);

                        if (Canvas.GetTop(x) > 1000)
                        {
                            itemRemover.Add(x);
                            health -= 10;
                        }

                        Rect enemyHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                        if (playerHitBox.Intersects(enemyHitBox))
                        {
                            itemRemover.Add(x);
                            health -= 5;
                        }
                    }
                }
            
            foreach (Rectangle i in itemRemover)
            {
                _canvas.Children.Remove(i);
            }

            if (score > 5)
            {
                limit = 20;
                enemySpeed = 15;
            }

            if (health < 1)
            {
                gameTimer.Stop();
                healthText.Text = "Health: 0";
                healthText.Foreground = Brushes.Red;

                GameOverWindow gameOverWindow = new GameOverWindow
                {
                    MainGameWindow = this,
                    Score = score
                };
                gameOverWindow.SetScore(score);
                gameOverWindow.ShowDialog(this);
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                moveLeft = true;
            }
            if (e.Key == Key.Right)
            {
                moveRight = true;
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                moveLeft = false;
            }
            if (e.Key == Key.Right)
            {
                moveRight = false;
            }

            if (e.Key == Key.Space)
            {
                Rectangle newLaser = new Rectangle
                {
                    Tag = "laser",
                    Height = 25,
                    Width = 5,
                    Fill = Brushes.CadetBlue
                };

                Canvas.SetLeft(newLaser, Canvas.GetLeft(_player) + _player.Width / 2);
                Canvas.SetTop(newLaser, Canvas.GetTop(_player) - newLaser.Height);
                _canvas.Children.Add(newLaser);

                using (var audioFileLaser = new AudioFileReader("Assets/Sounds/laser.mp3"))
                {
                    waveOut.Stop();
                    waveOut.Init(audioFileLaser);
                    waveOut.Play();
                }
            }
        }
        private void MakeEnemies()
        {
            ImageBrush enemySprite = new ImageBrush();

            enemySpriteCounter = rand.Next(1, 5);

            switch (enemySpriteCounter)
            {
                case 1:
                    enemySprite.Source = new Bitmap("Assets/Images/tie-fighter.png");
                    break;
                case 2:
                    enemySprite.Source = new Bitmap("Assets/Images/tie-skx1.png");
                    break;
                case 3:
                    enemySprite.Source = new Bitmap("Assets/Images/delta-7.png");
                    break;
                case 4:
                    enemySprite.Source = new Bitmap("Assets/Images/v-19.png");
                    break;
            }

            Rectangle newEnemy = new Rectangle
            {
                Tag = "enemy",
                Height = 70,
                Width = 84,
                Fill = enemySprite
            };

            Canvas.SetTop(newEnemy, -100);
            Canvas.SetLeft(newEnemy, rand.Next(30, 430)); 
            _canvas.Children.Add(newEnemy);
        }
        public void ResetGame()
        {
            health = 100;
            score = 0;
            enemySpeed = 10;
            limit = 50;
            moveLeft = false;
            moveRight = false;
            enemyCounter = 100;
            healthText.Foreground = Brushes.White;
            foreach (var x in _canvas.Children.OfType<Rectangle>().ToList())
            {
                _canvas.Children.Remove(x);
            }

            Canvas.SetLeft(_player, 324);
            Canvas.SetTop(_player, 900);

            if (!_canvas.Children.Contains(_player))
            {
                _canvas.Children.Add(_player);
            }

            gameTimer.Start();
            Focus();
        }
    }
}