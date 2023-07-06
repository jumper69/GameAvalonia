using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GameAvalonia.Views
{
    public partial class GameOverWindow : Window
    {
        public int Score { get; set; }
        public MainWindow MainGameWindow { get; set; }

        public GameOverWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void OnRestartClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            MainGameWindow.ResetGame();
            this.Close();
        }

        public void SetScore(int score)
        {
            this.FindControl<TextBlock>("scoreText").Text = "Score: " + score;
        }
    }
}