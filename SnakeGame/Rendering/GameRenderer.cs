using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using SnakeGame.Core;
using SnakeGame.Models;

namespace SnakeGame.Rendering
{
    /// <summary>
    /// 游戏渲染器 - 负责在 Canvas 上绘制游戏画面
    /// </summary>
    public class GameRenderer
    {
        private const int BLOCK_SIZE = 20;

        private readonly SolidColorBrush snakeHeadColor = new SolidColorBrush(Color.FromRgb(46, 125, 50));
        private readonly SolidColorBrush snakeBodyColor = new SolidColorBrush(Color.FromRgb(76, 175, 80));
        private readonly SolidColorBrush snakeBorderColor = new SolidColorBrush(Color.FromRgb(40, 100, 40));
        private readonly SolidColorBrush foodColor = new SolidColorBrush(Color.FromRgb(244, 67, 54));
        private readonly SolidColorBrush foodBorderColor = new SolidColorBrush(Color.FromRgb(200, 50, 40));
        private readonly SolidColorBrush gridColor = new SolidColorBrush(Color.FromRgb(224, 224, 224));

        /// <summary>
        /// 绘制完整游戏画面
        /// </summary>
        public void DrawGame(Canvas canvas, GameEngine engine)
        {
            canvas.Children.Clear();
            DrawGrid(canvas, engine.GameWidth, engine.GameHeight);
            DrawFood(canvas, engine.Food);
            DrawSnake(canvas, engine.Snake);
        }

        /// <summary>
        /// 绘制网格
        /// </summary>
        private void DrawGrid(Canvas canvas, int gameWidth, int gameHeight)
        {
            for (int i = 0; i <= gameWidth; i++)
            {
                Line line = new Line
                {
                    X1 = i * BLOCK_SIZE,
                    Y1 = 0,
                    X2 = i * BLOCK_SIZE,
                    Y2 = gameHeight * BLOCK_SIZE,
                    Stroke = gridColor,
                    StrokeThickness = 0.5
                };
                canvas.Children.Add(line);
            }

            for (int i = 0; i <= gameHeight; i++)
            {
                Line line = new Line
                {
                    X1 = 0,
                    Y1 = i * BLOCK_SIZE,
                    X2 = gameWidth * BLOCK_SIZE,
                    Y2 = i * BLOCK_SIZE,
                    Stroke = gridColor,
                    StrokeThickness = 0.5
                };
                canvas.Children.Add(line);
            }
        }

        /// <summary>
        /// 绘制蛇
        /// </summary>
        private void DrawSnake(Canvas canvas, Snake snake)
        {
            for (int i = 0; i < snake.Body.Count; i++)
            {
                Position pos = snake.Body[i];
                int pixelX = pos.X * BLOCK_SIZE;
                int pixelY = pos.Y * BLOCK_SIZE;

                Rectangle rect = new Rectangle
                {
                    Width = BLOCK_SIZE,
                    Height = BLOCK_SIZE,
                    Fill = i == 0 ? snakeHeadColor : snakeBodyColor,
                    Stroke = snakeBorderColor,
                    StrokeThickness = 0.5
                };

                Canvas.SetLeft(rect, pixelX);
                Canvas.SetTop(rect, pixelY);
                canvas.Children.Add(rect);
            }
        }

        /// <summary>
        /// 绘制食物
        /// </summary>
        private void DrawFood(Canvas canvas, Food food)
        {
            Position foodPos = food.Position;
            int pixelX = foodPos.X * BLOCK_SIZE;
            int pixelY = foodPos.Y * BLOCK_SIZE;

            Ellipse circle = new Ellipse
            {
                Width = BLOCK_SIZE - 2,
                Height = BLOCK_SIZE - 2,
                Fill = foodColor,
                Stroke = foodBorderColor,
                StrokeThickness = 1
            };

            Canvas.SetLeft(circle, pixelX + 1);
            Canvas.SetTop(circle, pixelY + 1);
            canvas.Children.Add(circle);
        }
    }
}
