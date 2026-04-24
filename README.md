# 贪吃蛇游戏 - 项目指南

## 项目概述

这是一个使用 WPF (.NET Framework 4.7.2) 实现的贪吃蛇游戏，采用 **MVVM 架构模式**。

---

## 项目结构

```
SnakeGame/
├── Models/                          # 数据模型层
│   ├── Position.cs                  # 二维坐标结构体
│   ├── Direction.cs                 # 移动方向枚举
│   └── GameState.cs                 # 游戏状态枚举
├── Core/                            # 核心逻辑层
│   ├── Snake.cs                     # 蛇类（身体、移动、碰撞）
│   ├── Food.cs                      # 食物类（位置、生成）
│   └── GameEngine.cs                # 游戏引擎（状态管理、事件）
├── ViewModels/                      # ViewModel 层
│   └── MainViewModel.cs             # 主窗口 ViewModel (INotifyPropertyChanged)
├── Views/                           # 视图层
│   ├── MainWindow.xaml              # XAML 布局（数据绑定）
│   └── MainWindow.xaml.cs           # 代码隐藏（帧循环、输入处理）
├── Rendering/                       # 渲染层
│   └── GameRenderer.cs              # Canvas 绘制
├── Services/                        # 服务层
│   └── ScoreManager.cs              # 最高分持久化
├── App.xaml / App.xaml.cs           # 应用入口
├── App.config                       # 配置文件
└── SnakeGame.csproj                 # 项目文件
```

---

## 游戏功能

### 核心功能
- [x] 蛇的移动（4个方向，防止180度掉头）
- [x] 食物生成和吃掉（随机位置，避开蛇身）
- [x] 碰撞检测（边界和自身）
- [x] 得分计算（每个食物 +10 分）
- [x] 游戏结束检测

### 控制
- [x] 方向键控制（↑↓←→ 或 WASD）
- [x] 空格键暂停/继续
- [x] 重新开始按钮
- [x] 暂停/继续按钮

### 高级功能
- [x] 难度调整（Easy 200ms / Standard 100ms / Hard 50ms）
- [x] 最高分记录（自动保存和加载）
- [x] 实时分数显示
- [x] 游戏状态指示（Playing / Paused / Game Over）
- [x] 难度选择仅在游戏未开始或 Game Over 后可用

### MVVM 架构
- [x] INotifyPropertyChanged 属性变更通知
- [x] 数据绑定（Score, HighScore, Level, StatusText 等）
- [x] ViewModel 与 View 分离
- [x] 事件驱动渲染（RenderRequested）

---

## 编译和运行

### 前置条件
- Windows 10/11
- Visual Studio 2015+（推荐 2019+）
- .NET Framework 4.7.2

### 编译

**Visual Studio**：打开 `SnakeGame.sln`，按 `Ctrl + B`

**命令行**：
```bash
msbuild SnakeGame\SnakeGame.csproj /p:Configuration=Debug
```

### 运行
```bash
SnakeGame\bin\Debug\SnakeGame.exe
```

---

## 游戏规则

1. **初始状态**：蛇在中心 (12, 12)，长度 3，方向向右
2. **移动**：每帧前进一格，不能直接掉头
3. **进食**：蛇头碰到食物 → 蛇身 +1，分数 +10
4. **结束**：碰边界或碰自身
5. **最高分**：自动保存，重启应用后保留

---

## 键盘控制

| 操作 | 按键 |
|------|------|
| 向上移动 | ↑ 或 W |
| 向下移动 | ↓ 或 S |
| 向左移动 | ← 或 A |
| 向右移动 | → 或 D |
| 暂停/继续 | Space |

---

## 难度等级

| 难度 | 速度 | 说明 |
|------|------|------|
| Easy | 200ms | 适合初学者 |
| Standard | 100ms | 平衡难度 |
| Hard | 50ms | 挑战高手 |

---

## MVVM 架构说明

### 架构图

```
┌─────────────────────────────────────────────────────────────┐
│                        View (MainWindow)                     │
│  - XAML 数据绑定                                             │
│  - 帧渲染循环 (CompositionTarget.Rendering)                   │
│  - 键盘输入处理                                               │
│  - 难度选择事件转发                                             │
└───────────────────────────┬─────────────────────────────────┘
                            │ 数据绑定 + 事件
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                   ViewModel (MainViewModel)                   │
│  - 可观察属性 (Score, HighScore, Level, StatusText...)       │
│  - 游戏流程控制 (StartGame, Restart, TogglePause)            │
│  - 难度管理 (OnDifficultyChanged)                             │
│  - 事件转发 (RenderRequested)                                 │
└───────────────────────────┬─────────────────────────────────┘
                            │ 调用
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                        Model 层                              │
│  Snake / Food / GameEngine / Position / Direction / GameState│
└─────────────────────────────────────────────────────────────┘
```

### 数据绑定映射

| UI 元素 | 绑定目标 | 方式 |
|---------|---------|------|
| ScoreLabel | Score | 单向绑定 |
| HighScoreLabel | HighScore | 单向绑定 |
| LevelLabel | Level | 单向绑定 |
| StatusLabel | StatusText | 单向绑定 |
| StatusLabel.Foreground | StatusForeground | 单向绑定 |
| PauseButton.Content | PauseButtonText | 单向绑定 |
| DifficultyCombo.IsEnabled | IsDifficultyEnabled | 单向绑定 |
| DifficultyCombo | SelectionChanged 事件 | 事件驱动 |

### 游戏状态流转

```
启动 / Restart → Paused（可选难度）
    ↓ 按方向键
Playing（难度锁定）
    ↓ 按空格
Paused（游戏中暂停，难度仍锁定）
    ↓ 按空格
Playing（继续）
    ↓ 碰撞
Game Over（解锁难度）
    ↓ 点 Restart
Paused（可选难度）
```

### ViewModel 关键方法

| 方法 | 说明 |
|------|------|
| Restart() | 重置游戏，进入 Paused，**解锁难度** |
| StartGame() | Paused → Playing，**锁定难度** |
| TogglePause() | 暂停/继续（不影响难度锁定） |
| OnDifficultyChanged() | 更新 MoveIntervalMs 和 Level |
| UpdateGame() | 转发到 GameEngine.UpdateGame() |

---

## 文件说明

### Models/Position.cs
二维坐标结构体，重写 Equals/GetHashCode/==/!=。

### Models/Direction.cs
枚举：Up, Down, Left, Right。

### Models/GameState.cs
枚举：Playing, GameOver, Paused。

### Core/Snake.cs
蛇的身体管理、移动逻辑、碰撞检测。初始位置 (12, 12)，长度 3。

### Core/Food.cs
食物的位置管理，随机生成时避开蛇身。

### Core/GameEngine.cs
游戏引擎：协调蛇和食物，管理游戏状态，提供 GameUpdated/GameOver 事件。

### ViewModels/MainViewModel.cs
MVVM 核心，实现 INotifyPropertyChanged。管理所有 UI 绑定属性，控制游戏流程和难度选择。

### Views/MainWindow.xaml
XAML 布局，使用数据绑定显示分数、状态等信息。ComboBox 通过 SelectionChanged 事件与 ViewModel 交互。

### Views/MainWindow.xaml.cs
代码隐藏，负责帧渲染循环（CompositionTarget.Rendering）、键盘输入处理、按钮点击事件。

### Rendering/GameRenderer.cs
在 Canvas 上绘制网格、蛇和食物。

### Services/ScoreManager.cs
最高分的持久化存储（通过 Properties.Settings）。

---

## 测试清单

- [ ] 蛇正确初始化（3节，中心位置）
- [ ] 四个方向移动正确
- [ ] 无法180度掉头
- [ ] 食物随机生成（不覆盖蛇身）
- [ ] 碰撞检测正常（边界、自身）
- [ ] 进食增加分数和蛇身长度
- [ ] 最高分正确保存和加载
- [ ] 难度调整改变游戏速度
- [ ] 暂停/继续功能正常
- [ ] 重新开始重置游戏状态
- [ ] **启动后可选择难度，按方向键后锁定**
- [ ] **Game Over 后可重新选择难度**
- [ ] **游戏中暂停时难度不可修改**

---

## 版本历史

| 版本 | 日期 | 说明 |
|------|------|------|
| 2.1 | 2026-04-24 | 修复难度选择逻辑，优化游戏状态流转 |
| 2.0 | 2026-04-24 | 重构为 MVVM 架构 |
| 1.0 | 2026-04-24 | 初始版本 |

---

**项目完成日期**：2026-04-24  
**上次更新**：2026-04-24
