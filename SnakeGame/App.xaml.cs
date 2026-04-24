using System;
using System.Windows;
using SnakeGame.Properties;

namespace SnakeGame
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeSettings();
        }

        /// <summary>
        /// 初始化应用程序设置
        /// </summary>
        private void InitializeSettings()
        {
            try
            {
                if (Settings.Default.HighScore == 0)
                {
                    Settings.Default.HighScore = 0;
                    Settings.Default.Save();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"初始化设置失败: {ex.Message}");
            }
        }
    }
}
