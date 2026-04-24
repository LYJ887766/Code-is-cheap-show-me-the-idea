using System;

namespace SnakeGame.Services
{
    /// <summary>
    /// 分数管理器 - 负责最高分的持久化存储
    /// </summary>
    public class ScoreManager
    {
        public int HighScore { get; private set; }

        public ScoreManager()
        {
            LoadHighScore();
        }

        /// <summary>
        /// 尝试更新最高分，如果当前分数更高则保存
        /// </summary>
        public bool TryUpdateHighScore(int currentScore)
        {
            if (currentScore > HighScore)
            {
                HighScore = currentScore;
                SaveHighScore();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 保存最高分到配置
        /// </summary>
        private void SaveHighScore()
        {
            try
            {
                Properties.Settings.Default.HighScore = HighScore;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"保存最高分失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 加载最高分从配置
        /// </summary>
        private void LoadHighScore()
        {
            try
            {
                HighScore = Properties.Settings.Default.HighScore;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载最高分失败: {ex.Message}");
                HighScore = 0;
            }
        }
    }
}
