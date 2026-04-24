using System;
using SnakeGame.Models;

namespace SnakeGame.Core
{
    /// <summary>
    /// 食物类 - 管理食物的位置和生成
    /// </summary>
    public class Food
    {
        public Position Position { get; set; }
        private Random random;

        public Food()
        {
            random = new Random();
            Position = new Position(0, 0);
        }

        /// <summary>
        /// 生成新食物 (随机位置，避开蛇身)
        /// </summary>
        public void Generate(int width, int height, Snake snake)
        {
            Position newPosition;
            do
            {
                newPosition = new Position(random.Next(width), random.Next(height));
            } while (snake.IsAtPosition(newPosition));

            Position = newPosition;
        }
    }
}
