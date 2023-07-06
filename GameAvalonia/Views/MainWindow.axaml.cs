using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls.Shapes;
using Avalonia.Media.Imaging;
using System.Numerics;

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
        int playerSpeed = 10;
        int limit = 50;
        int score = 0;
        int damage = 0;
        int enemySpeed = 10;
        
        //
        //private TextBlock scoreText;
        //private TextBlock damageText;
        //private Rectangle _player;
        //private _canvas __canvas;
        //
        Rect playerHitBox;

        

        public MainWindow()
        {
            InitializeComponent();

            scoreText = this.FindControl<TextBlock>("scoreText");
            damageText = this.FindControl<TextBlock>("damageText");
            _canvas = this.FindControl<Canvas>("_canvas");
            _player = this.FindControl<Rectangle>("_player");

            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

            Focus();

            ImageBrush bg = new ImageBrush();

            bg.Source = new Bitmap("C:\\Users\\mikos\\source\\repos\\GameAvalonia\\GameAvalonia\\Assets\\KosmosImg.png");
            //bg.TileMode = TileMode.Tile;
            //bg.Viewport = new Rect(0, 0, 0.15, 0.15);
            //bg.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
            _canvas.Background = bg;

            ImageBrush playerImage = new ImageBrush();
            playerImage.Source = new Bitmap("C:\\Users\\mikos\\source\\repos\\GameAvalonia\\GameAvalonia\\Assets\\x-wing.png");
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

            damageText.Text = "Damage " + damage;

            if (enemyCounter < 0)
            {
                MakeEnemies();
                enemyCounter = limit;
            }

            if (moveLeft == true && Canvas.GetLeft(_player) > 0)
            {
                Canvas.SetLeft(_player, Canvas.GetLeft(_player) - playerSpeed * 1.5);
            }
            if (moveRight == true && Canvas.GetLeft(_player) + 90 < Width)
            {
                Canvas.SetLeft(_player, Canvas.GetLeft(_player) + playerSpeed * 1.5);
            }

                foreach (var x in _canvas.Children.OfType<Rectangle>())
                {
                    if (x is Rectangle && (string)x.Tag == "bullet")
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) - 20);

                        Rect bulletHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                        if (Canvas.GetTop(x) < 10)
                        {
                            itemRemover.Add(x);
                        }

                        foreach (var y in _canvas.Children.OfType<Rectangle>())
                        {
                            if (y is Rectangle && (string)y.Tag == "enemy")
                            {
                                Rect enemyHit = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                                if (bulletHitBox.Intersects(enemyHit))
                                {
                                    itemRemover.Add(x);
                                    itemRemover.Add(y);
                                    score++;
                                }
                            }
                        }
                    }

                    if (x is Rectangle && (string)x.Tag == "enemy")
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) + enemySpeed);

                        if (Canvas.GetTop(x) > 750)
                        {
                            itemRemover.Add(x);
                            damage += 10;
                        }

                        Rect enemyHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                        if (playerHitBox.Intersects(enemyHitBox))
                        {
                            itemRemover.Add(x);
                            damage += 5;
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

            if (damage > 99)
            {
                gameTimer.Stop();
                damageText.Text = "Damage: 100";
                damageText.Foreground = Brushes.Red;
                ////MessageBox.Show("Captain You have destroyed " + score + " Alien Ships" + Environment.NewLine + "Press Ok to Play Again", "MOO Says: ");
                // System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                // Application.Current.Shutdown();

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
                Rectangle newBullet = new Rectangle
                {
                    Tag = "bullet",
                    Height = 20,
                    Width = 5,
                    Fill = Brushes.Red,
                    Stroke = Brushes.Red
                };

                Canvas.SetLeft(newBullet, Canvas.GetLeft(_player) + _player.Width / 2);
                Canvas.SetTop(newBullet, Canvas.GetTop(_player) - newBullet.Height);
                _canvas.Children.Add(newBullet);

            }
        }
        private void MakeEnemies()
        {
            ImageBrush enemySprite = new ImageBrush();

            enemySpriteCounter = rand.Next(1, 5);

            switch (enemySpriteCounter)
            {
                case 1:
                    enemySprite.Source = new Bitmap("C:\\Users\\mikos\\source\\repos\\GameAvalonia\\GameAvalonia\\Assets\\tie-fighter.png");
                    break;
                case 2:
                    enemySprite.Source = new Bitmap("C:\\Users\\mikos\\source\\repos\\GameAvalonia\\GameAvalonia\\Assets\\2.png");
                    break;
                case 3:
                    enemySprite.Source = new Bitmap("C:\\Users\\mikos\\source\\repos\\GameAvalonia\\GameAvalonia\\Assets\\3.png");
                    break;
                case 4:
                    enemySprite.Source = new Bitmap("C:\\Users\\mikos\\source\\repos\\GameAvalonia\\GameAvalonia\\Assets\\4.png");
                    break;
                case 5:
                    enemySprite.Source = new Bitmap("C:\\Users\\mikos\\source\\repos\\GameAvalonia\\GameAvalonia\\Assets\\5.png");
                    break;
            }

            Rectangle newEnemy = new Rectangle
            {
                Tag = "enemy",
                Height = 50,
                Width = 56,
                Fill = enemySprite
            };

            Canvas.SetTop(newEnemy, -100);
            Canvas.SetLeft(newEnemy, rand.Next(30, 430)); 
            _canvas.Children.Add(newEnemy);
        }
    }
}