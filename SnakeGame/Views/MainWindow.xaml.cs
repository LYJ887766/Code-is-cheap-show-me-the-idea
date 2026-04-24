using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using SnakeGame.Models;
using SnakeGame.ViewModels;

namespace SnakeGame.Views
{
    public partial class MainWindow : Window
    {
        private MainViewModel viewModel;
        private bool isRenderRegistered = false;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainViewModel();
            DataContext = viewModel;
            viewModel.RenderRequested += ViewModel_RenderRequested;
            viewModel.GameEngine.FrameStopwatch.Restart();
        }

        private void ViewModel_RenderRequested(object sender, GameRenderEventArgs e)
        {
            viewModel.Renderer.DrawGame(GameCanvas, e.Engine);
        }

        private void StartGameLoop()
        {
            if (!isRenderRegistered)
            {
                viewModel.GameEngine.FrameStopwatch.Restart();
                CompositionTarget.Rendering += OnFrameRendering;
                isRenderRegistered = true;
            }
        }

        private void StopGameLoop()
        {
            if (isRenderRegistered)
            {
                CompositionTarget.Rendering -= OnFrameRendering;
                viewModel.GameEngine.FrameStopwatch.Stop();
                isRenderRegistered = false;
            }
        }

        private void OnFrameRendering(object sender, EventArgs e)
        {
            if (viewModel.GameEngine.FrameStopwatch.ElapsedMilliseconds >= viewModel.GameEngine.MoveIntervalMs)
            {
                viewModel.GameEngine.FrameStopwatch.Restart();
                viewModel.UpdateGame();
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                case Key.W:
                    viewModel.StartGame();
                    StartGameLoop();
                    viewModel.ChangeDirection(Direction.Up);
                    e.Handled = true;
                    break;
                case Key.Down:
                case Key.S:
                    viewModel.StartGame();
                    StartGameLoop();
                    viewModel.ChangeDirection(Direction.Down);
                    e.Handled = true;
                    break;
                case Key.Left:
                case Key.A:
                    viewModel.StartGame();
                    StartGameLoop();
                    viewModel.ChangeDirection(Direction.Left);
                    e.Handled = true;
                    break;
                case Key.Right:
                case Key.D:
                    viewModel.StartGame();
                    StartGameLoop();
                    viewModel.ChangeDirection(Direction.Right);
                    e.Handled = true;
                    break;
                case Key.Space:
                    viewModel.TogglePause();
                    if (viewModel.GameEngine.State == GameState.Paused)
                        StopGameLoop();
                    else if (viewModel.GameEngine.State == GameState.Playing)
                        StartGameLoop();
                    e.Handled = true;
                    break;
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.TogglePause();
            if (viewModel.GameEngine.State == GameState.Paused)
                StopGameLoop();
            else if (viewModel.GameEngine.State == GameState.Playing)
                StartGameLoop();
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            StopGameLoop();
            viewModel.Restart();
            viewModel.Renderer.DrawGame(GameCanvas, viewModel.GameEngine);
        }

        private void DifficultyCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (viewModel == null) return;
            viewModel.SelectedDifficultyIndex = DifficultyCombo.SelectedIndex;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.PreviewKeyDown += Window_PreviewKeyDown;
            DifficultyCombo.SelectedIndex = viewModel.SelectedDifficultyIndex;
            viewModel.Renderer.DrawGame(GameCanvas, viewModel.GameEngine);
        }
    }
}
