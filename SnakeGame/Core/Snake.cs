using System.Collections.Generic;
using System.Linq;
using SnakeGame.Models;

namespace SnakeGame.Core
{
    /// <summary>
    /// 蛇类 - 管理蛇的身体、移动和碰撞检测
    /// </summary>
    public class Snake
    {
        public List<Position> Body { get; private set; }
        public Direction CurrentDirection { get; set; }
        public Direction NextDirection { get; set; }

        private const int INITIAL_LENGTH = 3;
        private const int START_X = 12;
        private const int START_Y = 12;

        public Position Head => Body.Count > 0 ? Body[0] : new Position(0, 0);

        public Snake()
        {
            Body = new List<Position>();
            InitializeSnake();
            CurrentDirection = Direction.Right;
            NextDirection = Direction.Right;
        }

        /// <summary>
        /// 初始化蛇的身体 (中心位置, 长度为3)
        /// </summary>
        private void InitializeSnake()
        {
            Body.Clear();
            for (int i = 0; i < INITIAL_LENGTH; i++)
            {
                Body.Add(new Position(START_X - i, START_Y));
            }
        }

        /// <summary>
        /// 重置蛇到初始状态
        /// </summary>
        public void Reset()
        {
            InitializeSnake();
            CurrentDirection = Direction.Right;
            NextDirection = Direction.Right;
        }

        /// <summary>
        /// 蛇向当前方向移动一格
        /// </summary>
        public void Move()
        {
            if (!IsValidDirectionChange(CurrentDirection, NextDirection))
            {
                NextDirection = CurrentDirection;
            }

            CurrentDirection = NextDirection;

            Position newHead = GetNewHeadPosition();

            Body.Insert(0, newHead);
            Body.RemoveAt(Body.Count - 1);
        }

        /// <summary>
        /// 蛇身增长一节
        /// </summary>
        public void Grow()
        {
            if (Body.Count > 0)
            {
                Position newHead = GetNewHeadPosition();
                Body.Insert(0, newHead);
            }
        }

        /// <summary>
        /// 获取下一个头部位置
        /// </summary>
        private Position GetNewHeadPosition()
        {
            Position head = Head;
            switch (CurrentDirection)
            {
                case Direction.Up:
                    return new Position(head.X, head.Y - 1);
                case Direction.Down:
                    return new Position(head.X, head.Y + 1);
                case Direction.Left:
                    return new Position(head.X - 1, head.Y);
                case Direction.Right:
                    return new Position(head.X + 1, head.Y);
                default:
                    return head;
            }
        }

        /// <summary>
        /// 检查方向改变是否有效（不能直接掉头）
        /// </summary>
        private bool IsValidDirectionChange(Direction current, Direction next)
        {
            if (current == Direction.Left && next == Direction.Right) return false;
            if (current == Direction.Right && next == Direction.Left) return false;
            if (current == Direction.Up && next == Direction.Down) return false;
            if (current == Direction.Down && next == Direction.Up) return false;
            return true;
        }

        /// <summary>
        /// 检查蛇是否碰撞边界
        /// </summary>
        public bool CheckCollisionWithWall(int width, int height)
        {
            Position head = Head;
            return head.X < 0 || head.X >= width || head.Y < 0 || head.Y >= height;
        }

        /// <summary>
        /// 检查蛇是否碰撞自身
        /// </summary>
        public bool CheckCollisionWithSelf()
        {
            Position head = Head;
            for (int i = 1; i < Body.Count; i++)
            {
                if (Body[i].Equals(head))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查蛇是否与指定位置重合
        /// </summary>
        public bool IsAtPosition(Position pos)
        {
            return Body.Any(segment => segment.Equals(pos));
        }
    }
}
