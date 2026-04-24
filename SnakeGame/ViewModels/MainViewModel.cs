using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using SnakeGame.Core;
using SnakeGame.Models;
using SnakeGame.Rendering;
using SnakeGame.Services;

namespace SnakeGame.ViewModels
{
    /// <summary>
    /// 主窗口 ViewModel - 实现 INotifyPropertyChanged 接口
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly GameEngine gameEngine;
        private readonly GameRenderer renderer;
        private readonly ScoreManager scoreManager;

        private int score;
        private int highScore;
        private string level = "Standard";
        private string statusText = "Ready to Play";
        private SolidColorBrush statusForeground = new SolidColorBrush(Color.FromRgb(33, 150, 243));
        private string pauseButtonText = "Pause";
        private bool isDifficultyEnabled = true;
        private int selectedDifficultyIndex = 1;

        public int Score
        {
            get => score;
            set { score = value; OnPropertyChanged(); }
        }

        public int HighScore
        {
            get => highScore;
            set { highScore = value; OnPropertyChanged(); }
        }

        public string Level
        {
            get => level;
            set { level = value; OnPropertyChanged(); }
        }

        public string StatusText
        {
            get => statusText;
            set { statusText = value; OnPropertyChanged(); }
        }

        public SolidColorBrush StatusForeground
        {
            get => statusForeground;
            set { statusForeground = value; OnPropertyChanged(); }
        }

        public string PauseButtonText
        {
            get => pauseButtonText;
            set { pauseButtonText = value; OnPropertyChanged(); }
        }

        public bool IsDifficultyEnabled
        {
            get => isDifficultyEnabled;
            set { isDifficultyEnabled = value; OnPropertyChanged(); }
        }

        public int SelectedDifficultyIndex
        {
            get => selectedDifficultyIndex;
            set
            {
                if (selectedDifficultyIndex != value)
                {
                    selectedDifficultyIndex = value;
                    OnPropertyChanged();
                    OnDifficultyChanged();
                }
            }
        }

        public event EventHandler<GameRenderEventArgs> RenderRequested;
        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            gameEngine = new GameEngine(width: 25, height: 25);
            renderer = new GameRenderer();
            scoreManager = new ScoreManager();

            gameEngine.GameUpdated += GameEngine_GameUpdated;
            gameEngine.GameOver += GameEngine_GameOver;
            gameEngine.MoveIntervalMs = 100;

            HighScore = scoreManager.HighScore;
        }

        public GameEngine GameEngine => gameEngine;
        public GameRenderer Renderer => renderer;

        public void ChangeDirection(Direction direction)
        {
            gameEngine.ChangeDirection(direction);
        }

        public void TogglePause()
        {
            if (gameEngine.State == GameState.GameOver)
                return;

            gameEngine.TogglePause();
            UpdateUIState();
        }

        public void Restart()
        {
            gameEngine.Reset();
            gameEngine.Pause();
            Score = 0;
            PauseButtonText = "Resume";
            StatusText = "Ready to Play";
            StatusForeground = new SolidColorBrush(Color.FromRgb(33, 150, 243));
            IsDifficultyEnabled = true;
        }

        public void StartGame()
        {
            if (gameEngine.State == GameState.Paused)
            {
                gameEngine.Resume();
                PauseButtonText = "Pause";
                StatusText = "Playing";
                StatusForeground = new SolidColorBrush(Color.FromRgb(33, 150, 243));
                IsDifficultyEnabled = false;
            }
        }

        public void UpdateGame()
        {
            gameEngine.UpdateGame();
        }

        public void OnDifficultyChanged()
        {
            if (gameEngine == null) return;

            int interval;
            string[] difficultyNames = { "Easy", "Standard", "Hard" };

            switch (SelectedDifficultyIndex)
            {
                case 0:
                    interval = 200;
                    break;
                case 1:
                    interval = 100;
                    break;
                case 2:
                    interval = 50;
                    break;
                default:
                    interval = 100;
                    break;
            }

            Level = difficultyNames[SelectedDifficultyIndex];
            gameEngine.MoveIntervalMs = interval;
        }

        private void GameEngine_GameUpdated(object sender, EventArgs e)
        {
            RenderRequested?.Invoke(this, new GameRenderEventArgs(gameEngine));
            UpdateUIState();
        }

        private void GameEngine_GameOver(object sender, EventArgs e)
        {
            scoreManager.TryUpdateHighScore(gameEngine.Score);
            HighScore = scoreManager.HighScore;

            StatusText = $"Game Over! Score: {gameEngine.Score}, High Score: {scoreManager.HighScore}";
            StatusForeground = new SolidColorBrush(Color.FromRgb(244, 67, 54));
            PauseButtonText = "Resume";
        }

        private void UpdateUIState()
        {
            Score = gameEngine.Score;
            HighScore = scoreManager.HighScore;

            if (gameEngine.State == GameState.Paused)
            {
                PauseButtonText = "Resume";
                StatusText = "Paused";
                StatusForeground = new SolidColorBrush(Color.FromRgb(255, 152, 0));
                IsDifficultyEnabled = false;
            }
            else if (gameEngine.State == GameState.Playing)
            {
                PauseButtonText = "Pause";
                StatusText = "Playing";
                StatusForeground = new SolidColorBrush(Color.FromRgb(33, 150, 243));
                IsDifficultyEnabled = false;
            }
            else if (gameEngine.State == GameState.GameOver)
            {
                IsDifficultyEnabled = true;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class GameRenderEventArgs : EventArgs
    {
        public GameEngine Engine { get; }
        public GameRenderEventArgs(GameEngine engine) => Engine = engine;
    }
}
