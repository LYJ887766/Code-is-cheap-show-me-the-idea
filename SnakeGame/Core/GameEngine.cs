using System;
using System.Diagnostics;
using SnakeGame.Models;

namespace SnakeGame.Core
{
    /// <summary>
    /// 游戏引擎类 - 管理游戏逻辑和状态
    /// </summary>
    public class GameEngine
    {
        public Snake Snake { get; private set; }
        public Food Food { get; private set; }
        public int Score { get; set; }
        public GameState State { get; set; }
        public int GameWidth { get; private set; }
        public int GameHeight { get; private set; }

        /// <summary>
        /// 蛇每次移动的时间间隔（毫秒），由外部根据难度设置
        /// </summary>
        public double MoveIntervalMs { get; set; } = 100;

        /// <summary>
        /// 用于精确计时的 Stopwatch，配合 CompositionTarget.Rendering 实现事件驱动更新
        /// </summary>
        public Stopwatch FrameStopwatch { get; private set; }

        // 事件定义
        public event EventHandler GameUpdated;
        public event EventHandler GameOver;

        public GameEngine(int width = 25, int height = 25)
        {
            GameWidth = width;
            GameHeight = height;
            Snake = new Snake();
            Food = new Food();
            Score = 0;
            State = GameState.Playing;
            FrameStopwatch = new Stopwatch();

            Food.Generate(GameWidth, GameHeight, Snake);
        }

        /// <summary>
        /// 更新游戏状态 (每帧调用一次)
        /// </summary>
        public void UpdateGame()
        {
            if (State != GameState.Playing)
                return;

            Snake.Move();

            if (Snake.CheckCollisionWithWall(GameWidth, GameHeight) ||
                Snake.CheckCollisionWithSelf())
            {
                State = GameState.GameOver;
                GameOver?.Invoke(this, EventArgs.Empty);
                return;
            }

            if (Snake.Head.Equals(Food.Position))
            {
                Snake.Grow();
                Score += 10;
                Food.Generate(GameWidth, GameHeight, Snake);
            }

            GameUpdated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 改变蛇的下一个方向
        /// </summary>
        public void ChangeDirection(Direction direction)
        {
            if (State == GameState.Playing)
            {
                Snake.NextDirection = direction;
            }
        }

        /// <summary>
        /// 重置游戏到初始状态
        /// </summary>
        public void Reset()
        {
            Snake.Reset();
            Score = 0;
            State = GameState.Playing;
            Food.Generate(GameWidth, GameHeight, Snake);
            GameUpdated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 暂停游戏
        /// </summary>
        public void Pause()
        {
            if (State == GameState.Playing)
            {
                State = GameState.Paused;
            }
        }

        /// <summary>
        /// 继续游戏
        /// </summary>
        public void Resume()
        {
            if (State == GameState.Paused)
            {
                State = GameState.Playing;
            }
        }

        /// <summary>
        /// 切换暂停状态
        /// </summary>
        public void TogglePause()
        {
            if (State == GameState.Playing)
                Pause();
            else if (State == GameState.Paused)
                Resume();
        }
    }
}
