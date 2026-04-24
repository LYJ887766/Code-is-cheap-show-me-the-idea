# 贪吃蛇游戏 - 设计文档

**项目名称**：Snake Game  
**框架**：WPF (.NET Framework 4.7.2)  
**架构模式**：MVVM (Model-View-ViewModel)  
**版本**：2.1  
**最后更新**：2026-04-24  

---

## 目录

1. [项目概述](#项目概述)
2. [游戏规则](#游戏规则)
3. [架构设计](#架构设计)
4. [模块详细设计](#模块详细设计)
5. [UI/UX 设计](#uiux-设计)
6. [数据流](#数据流)
7. [难度机制](#难度机制)
8. [技术选型](#技术选型)
9. [实现细节](#实现细节)

---

## 项目概述

### 功能描述

这是一个经典的贪吃蛇游戏的 WPF 实现，采用 MVVM 架构模式。玩家通过方向键控制蛇的移动，目标是吃掉食物来获得分数，同时避免碰撞。

### 核心特性

- **游戏区域**：25x25 的方块网格（500x500 像素）
- **基础速度**：100ms 每帧（可调节）
- **最高分记录**：自动保存和加载
- **难度等级**：3 个等级（Easy / Standard / Hard）
- **暂停/继续**：支持中途暂停
- **重新开始**：快速重启游戏

### 技术栈

| 组件 | 技术 |
|------|------|
| UI框架 | WPF (Windows Presentation Foundation) |
| 语言 | C# |
| .NET版本 | .NET Framework 4.7.2 |
| 架构模式 | MVVM |
| 绘制方式 | Canvas |
| 帧驱动 | CompositionTarget.Rendering (事件驱动) |
| 数据持久化 | App.config Settings |
| 属性通知 | INotifyPropertyChanged |

---

## 游戏规则

### 基本规则

1. **蛇的初始状态**：
   - 位置：游戏区域中心 (12, 12)
   - 长度：3 节身体
   - 初始方向：向右

2. **移动机制**：
   - 蛇每帧向当前方向前进一格
   - 无法直接掉头（向左时不能直接向右）
   - 玩家可以改变下一帧的方向

3. **进食与生长**：
   - 蛇头与食物重合时，蛇身增长 1 节
   - 分数增加 10 分
   - 新食物随机生成

4. **游戏结束条件**：
   - 蛇头碰撞游戏边界
   - 蛇头碰撞自身

5. **得分系统**：
   - 每吃一个食物：+10 分
   - 最高分自动保存

### 游戏状态流转

```
启动 / Restart
      ↓
    Paused（可选择难度）
      ↓ 按方向键
    Playing（难度锁定）
      ↓ 按空格
    Paused（暂停中）
      ↓ 按空格
    Playing（继续）
      ↓ 碰撞
    GameOver（解锁难度）
      ↓ 点 Restart
    Paused（可选择难度）
```

---

## 架构设计

### MVVM 架构

```
┌─────────────────────────────────────────────────────────────┐
│                        View (MainWindow)                     │
│  - XAML 数据绑定                                             │
│  - 用户输入处理 (键盘、按钮、下拉框)                           │
│  - 帧渲染循环 (CompositionTarget.Rendering)                   │
│  - 难度选择事件转发 (DifficultyCombo_SelectionChanged)         │
└───────────────────────────┬─────────────────────────────────┘
                            │ 数据绑定 + 事件
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                   ViewModel (MainViewModel)                   │
│  - INotifyPropertyChanged 接口                               │
│  - 可观察属性 (Score, HighScore, Level, StatusText...)       │
│  - 游戏流程控制 (StartGame, Restart, TogglePause)            │
│  - 事件转发 (RenderRequested)                                 │
│  - 难度管理 (OnDifficultyChanged)                             │
└───────────────────────────┬─────────────────────────────────┘
                            │ 调用
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                        Model 层                              │
│  ┌──────────┐  ┌──────────┐  ┌──────────────┐               │
│  │  Snake   │  │   Food   │  │ GameEngine   │               │
│  │ (蛇数据) │  │ (食物数据)│  │ (游戏逻辑)   │               │
│  └──────────┘  └──────────┘  └──────────────┘               │
│  ┌──────────┐  ┌──────────┐  ┌──────────────┐               │
│  │ Position │  │Direction │  │  GameState   │               │
│  └──────────┘  └──────────┘  └──────────────┘               │
└─────────────────────────────────────────────────────────────┘
```

### 文件结构

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
│   └── MainViewModel.cs             # 主窗口 ViewModel
├── Views/                           # 视图层
│   ├── MainWindow.xaml              # XAML 布局（数据绑定）
│   └── MainWindow.xaml.cs           # 代码隐藏（UI交互）
├── Rendering/                       # 渲染层
│   └── GameRenderer.cs              # Canvas 绘制
├── Services/                        # 服务层
│   └── ScoreManager.cs              # 最高分管理
├── App.xaml                         # 应用入口
├── App.xaml.cs
├── App.config                       # 配置文件
└── SnakeGame.csproj                 # 项目文件
```

### 类设计关系

```
MainViewModel (INotifyPropertyChanged)
├── 可观察属性
│   ├── Score: int
│   ├── HighScore: int
│   ├── Level: string
│   ├── StatusText: string
│   ├── StatusForeground: SolidColorBrush
│   ├── PauseButtonText: string
│   ├── IsDifficultyEnabled: bool
│   └── SelectedDifficultyIndex: int
├── 方法
│   ├── ChangeDirection(Direction)   → 转发到 GameEngine
│   ├── TogglePause()                → 切换暂停/继续
│   ├── Restart()                    → 重置游戏，进入 Paused，解锁难度
│   ├── StartGame()                  → 从 Paused 切到 Playing，锁定难度
│   ├── UpdateGame()                 → 转发到 GameEngine.UpdateGame()
│   └── OnDifficultyChanged()        → 更新 MoveIntervalMs 和 Level
├── 事件
│   ├── RenderRequested              → 通知 View 进行渲染
│   └── PropertyChanged              → 属性变更通知
└── 依赖
    ├── GameEngine (Model)
    ├── GameRenderer (Rendering)
    └── ScoreManager (Services)

GameEngine (Model)
├── 属性
│   ├── Snake, Food, Score, State
│   ├── GameWidth, GameHeight
│   ├── MoveIntervalMs
│   └── FrameStopwatch
├── 方法
│   ├── UpdateGame(), ChangeDirection()
│   ├── Reset(), Pause(), Resume(), TogglePause()
└── 事件
    ├── GameUpdated                  → 触发 ViewModel 更新 UI 和渲染
    └── GameOver                     → 触发 ViewModel 处理游戏结束
```

---

## 模块详细设计

### 1. Position 结构体

**文件**：`Models/Position.cs`

表示二维坐标，重写了 `Equals`、`GetHashCode` 和 `==`/`!=` 运算符。

### 2. Direction 枚举

**文件**：`Models/Direction.cs`

四个方向：`Up`、`Down`、`Left`、`Right`。

### 3. GameState 枚举

**文件**：`Models/GameState.cs`

三种状态：`Playing`、`GameOver`、`Paused`。

### 4. Snake 类

**文件**：`Core/Snake.cs`

| 属性 | 类型 | 说明 |
|------|------|------|
| Body | List\<Position\> | 蛇身体所有节点，Body[0] 为蛇头 |
| CurrentDirection | Direction | 当前移动方向 |
| NextDirection | Direction | 下一帧将采用的方向 |

| 方法 | 说明 |
|------|------|
| Move() | 向当前方向移动一格 |
| Grow() | 蛇身增长一节 |
| CheckCollisionWithWall(int w, int h) | 检测边界碰撞 |
| CheckCollisionWithSelf() | 检测自身碰撞 |
| IsAtPosition(Position pos) | 检查某位置是否被蛇身占据 |

### 5. Food 类

**文件**：`Core/Food.cs`

| 方法 | 说明 |
|------|------|
| Generate(int w, int h, Snake snake) | 在不与蛇身重叠的位置随机生成食物 |

### 6. GameEngine 类

**文件**：`Core/GameEngine.cs`

| 属性 | 类型 | 说明 |
|------|------|------|
| Snake | Snake | 蛇对象 |
| Food | Food | 食物对象 |
| Score | int | 当前分数 |
| State | GameState | 游戏状态 |
| MoveIntervalMs | double | 蛇移动间隔（毫秒）|
| FrameStopwatch | Stopwatch | 帧计时器 |

| 方法 | 说明 |
|------|------|
| UpdateGame() | 每帧更新：移动 → 碰撞检测 → 进食检测 |
| ChangeDirection(Direction) | 改变蛇的下一个方向 |
| Reset() | 重置到初始状态 |
| Pause() / Resume() / TogglePause() | 状态切换 |

| 事件 | 说明 |
|------|------|
| GameUpdated | 游戏状态已更新 |
| GameOver | 游戏结束 |

### 7. MainViewModel 类

**文件**：`ViewModels/MainViewModel.cs`

实现 `INotifyPropertyChanged` 接口，管理所有 UI 绑定属性和游戏流程控制。

**可观察属性**：

| 属性 | 类型 | 绑定目标 | 说明 |
|------|------|---------|------|
| Score | int | ScoreLabel | 当前分数 |
| HighScore | int | HighScoreLabel | 最高分 |
| Level | string | LevelLabel | 当前难度名称 |
| StatusText | string | StatusLabel | 状态文本 |
| StatusForeground | SolidColorBrush | StatusLabel.Foreground | 状态颜色 |
| PauseButtonText | string | PauseButton.Content | 按钮文本 |
| IsDifficultyEnabled | bool | DifficultyCombo.IsEnabled | 难度选择是否可用 |
| SelectedDifficultyIndex | int | DifficultyCombo (事件驱动) | 选中的难度索引 |

**关键方法**：

| 方法 | 说明 |
|------|------|
| Restart() | 重置游戏，进入 Paused 状态，**解锁难度选择** |
| StartGame() | 从 Paused 切到 Playing，**锁定难度选择** |
| TogglePause() | 暂停/继续切换（不影响难度锁定） |
| OnDifficultyChanged() | 根据 SelectedDifficultyIndex 更新 MoveIntervalMs 和 Level |

**难度选择控制逻辑**：

```
IsDifficultyEnabled = true   → 启动时、Restart 后、GameOver 后
IsDifficultyEnabled = false  → Playing 中、Paused 中（游戏中暂停）
```

### 8. GameRenderer 类

**文件**：`Rendering/GameRenderer.cs`

| 方法 | 说明 |
|------|------|
| DrawGame(Canvas, GameEngine) | 绘制完整游戏画面（网格 + 食物 + 蛇） |

### 9. ScoreManager 类

**文件**：`Services/ScoreManager.cs`

| 方法 | 说明 |
|------|------|
| TryUpdateHighScore(int) | 如果当前分数更高则保存 |
| SaveHighScore() / LoadHighScore() | 通过 Properties.Settings 持久化 |

---

## UI/UX 设计

### 窗口布局

```
┌─────────────────────────────────────────────┐
│ Snake Game                                  │
├─────────────────────────────────────────────┤
│ Score: 0   High Score: 90   Level: Standard │
├─────────────────────────────────────────────┤
│                                             │
│              ┌─────────────────┐            │
│              │  500x500 Canvas │            │
│              │  (游戏画布)     │            │
│              └─────────────────┘            │
│                                             │
├─────────────────────────────────────────────┤
│ [Resume] [Restart]    Ready to Play  Easy ▼ │
└─────────────────────────────────────────────┘
```

### 数据绑定映射

| UI 元素 | 绑定属性/事件 | 绑定方式 |
|---------|-------------|---------|
| ScoreLabel | Score | 单向 (ViewModel → View) |
| HighScoreLabel | HighScore | 单向 (ViewModel → View) |
| LevelLabel | Level | 单向 (ViewModel → View) |
| StatusLabel | StatusText | 单向 (ViewModel → View) |
| StatusLabel.Foreground | StatusForeground | 单向 (ViewModel → View) |
| PauseButton.Content | PauseButtonText | 单向 (ViewModel → View) |
| DifficultyCombo.IsEnabled | IsDifficultyEnabled | 单向 (ViewModel → View) |
| DifficultyCombo | SelectionChanged 事件 | 事件 → ViewModel.SelectedDifficultyIndex |

### 色彩方案

| 元素 | 颜色 |
|------|------|
| 背景 | 白色 (#FFFFFF) |
| 网格线 | 浅灰 (#E0E0E0) |
| 蛇头 | 深绿 (#2E7D32) |
| 蛇身 | 绿色 (#4CAF50) |
| 食物 | 红色 (#F44336) |
| 状态栏 | 蓝色 (#2196F3) |
| 暂停状态 | 橙色 (#FF9800) |
| 游戏结束 | 红色 (#F44336) |
| 正常状态 | 蓝色 (#2196F3) |

### 交互元素

**键盘控制**：
- ↑/W：向上（首次按下启动游戏）
- ↓/S：向下（首次按下启动游戏）
- ←/A：向左（首次按下启动游戏）
- →/D：向右（首次按下启动游戏）
- Space：暂停/继续

**按钮**：
- **Pause/Resume**：切换暂停状态
- **Restart**：重新开始游戏（回到可选难度的等待状态）

**下拉框**：
- **Difficulty**：仅在游戏未开始或 Game Over 后可操作

---

## 数据流

### 游戏启动流程

```
Window_Loaded
    ↓
ViewModel 构造 → GameEngine 初始化 → State = Playing
    ↓
Restart() → gameEngine.Reset() + Pause() → State = Paused
    ↓
DifficultyCombo.SelectedIndex = 1 → IsDifficultyEnabled = true
    ↓
用户选择难度 → DifficultyCombo_SelectionChanged
    ↓
viewModel.SelectedDifficultyIndex = index → OnDifficultyChanged()
    ↓
gameEngine.MoveIntervalMs 更新
    ↓
用户按方向键 → StartGame() → Resume() → IsDifficultyEnabled = false
    ↓
StartGameLoop() → CompositionTarget.Rendering 注册
```

### 帧更新流程

```
CompositionTarget.Rendering 触发
    ↓
OnFrameRendering → 检查 Stopwatch >= MoveIntervalMs
    ↓
viewModel.UpdateGame()
    ↓
gameEngine.UpdateGame()
    ↓
snake.Move() → 碰撞检测 → 进食检测
    ↓
GameUpdated 事件 → ViewModel.RenderRequested 事件
    ↓
View.ViewModel_RenderRequested → renderer.DrawGame(Canvas, engine)
    ↓
UI 刷新
```

### 最高分管理

```
Game Over → GameEngine.GameOver 事件
    ↓
ViewModel.GameEngine_GameOver()
    ↓
scoreManager.TryUpdateHighScore(score)
    ↓
Properties.Settings.Default.HighScore = score
    ↓
HighScore 属性更新 → UI 自动刷新
```

---

## 难度机制

### 难度等级

| 难度 | MoveIntervalMs | 特点 |
|------|---------------|------|
| **Easy** | 200ms | 蛇移动较慢，适合初学者 |
| **Standard** | 100ms | 平衡难度，推荐 |
| **Hard** | 50ms | 蛇移动快速，挑战高手 |

### 难度选择的可用时机

| 游戏状态 | IsDifficultyEnabled | 说明 |
|---------|-------------------|------|
| 启动 / Restart 后 | `true` | 用户可选择难度 |
| Playing | `false` | 游戏中锁定 |
| Paused（游戏中暂停） | `false` | 游戏中暂停不解锁 |
| Game Over | `true` | 允许重新选择 |

### 实现方式

难度选择通过 View 层 `DifficultyCombo_SelectionChanged` 事件驱动，手动同步到 ViewModel 的 `SelectedDifficultyIndex` 属性，再由 `OnDifficultyChanged()` 更新 `gameEngine.MoveIntervalMs`。

---

## 技术选型

### 为什么选择 MVVM？

- WPF 原生支持数据绑定，MVVM 能充分发挥框架优势
- ViewModel 可独立测试
- UI 逻辑与业务逻辑职责分离

### 为什么选择 Canvas？

- 高性能，适合高频刷新（100ms）
- 灵活绘制，支持自定义图形

### 为什么选择 CompositionTarget.Rendering？

- 与 WPF 渲染管线同步
- 比 DispatcherTimer 更精确
- 适合高频更新场景

### 为什么选择 App.config？

- 轻量级，无需外部依赖
- 适合简单的键值对存储（最高分）

---

## 实现细节

### 坐标系统

```
游戏逻辑坐标 (格子数)：
(0,0) ──→ X (25)
  │
  ↓
  Y (25)

像素坐标：
Position(x, y) → (x * 20, y * 20) 到 ((x+1) * 20, (y+1) * 20)
```

### 碰撞检测

**边界碰撞**：
```csharp
return head.X < 0 || head.X >= width || head.Y < 0 || head.Y >= height;
```

**自身碰撞**：
```csharp
for (int i = 1; i < Body.Count; i++)
    if (Body[i].Equals(head)) return true;
return false;
```

---

## 扩展建议

1. **游戏特性**：道具系统、障碍物、关卡、多人模式
2. **UI/UX**：主菜单、动画、音效、触摸控制
3. **架构优化**：ICommand 命令模式、依赖注入、单元测试
4. **存储**：排行榜、游戏存档、统计

---

## 版本历史

| 版本 | 日期 | 说明 |
|------|------|------|
| 2.1 | 2026-04-24 | 修复难度选择逻辑，优化游戏状态流转 |
| 2.0 | 2026-04-24 | 重构为 MVVM 架构 |
| 1.0 | 2026-04-24 | 初始版本 |

---

**文档完成时间**：2026-04-24  
**最后修改**：2026-04-24
