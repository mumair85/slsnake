using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using SLSnake.Elements;

namespace SLSnake
{
    public partial class Game : Page
    {
        #region Constructor
        public Game()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(Game_Loaded);
        }
        ~Game()
        {
            Debug.WriteLine("disposed");
        }
        #endregion

        #region OnNavigatedTo
        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
        #endregion

        #region GameLoop

        #region GameLoop 相关
        int _NumTicksPerLoop = 50;	//一秒２０次
        int _TickCount, _TickCountElapsed;
        bool _IsProcessCalled = false;
        #endregion

        #region FPS 显示相关
        TextBlock _FPS_TextBlock = new TextBlock() { Foreground = new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0, 0)) };
        int _FrameRate, _LastTime;
        #endregion

        GameLoopHandler _h;

        void Game_Loaded(object sender, RoutedEventArgs e)
        {
            #region GameLogic Init
            _h = new GameLoopHandler(this.LayoutRoot);
            if (_h.Init()) return;
            #endregion

            #region FPS 显示相关
            this.LayoutRoot.Children.Add(_FPS_TextBlock);
            Canvas.SetLeft(_FPS_TextBlock, 10);
            Canvas.SetTop(_FPS_TextBlock, 10);
            Canvas.SetZIndex(_FPS_TextBlock, 99999);
            #endregion

            #region GameLoop 相关
            var sb = new Storyboard();
            sb.Completed += new EventHandler(sb_Completed);
            sb.Begin();
            #endregion
        }

        void sb_Completed(object sender, EventArgs e)
        {
            #region FPS 显示相关
            if (_LastTime > DateTime.Now.Millisecond)
            {

                _FPS_TextBlock.Text = "FPS: " + _FrameRate.ToString();
                _FrameRate = 0;

                _LastTime = DateTime.Now.Millisecond;
            }
            else
            {

                _LastTime = DateTime.Now.Millisecond;

                _FrameRate++;

            }
            #endregion

            #region GameLoop 相关
            if (!_IsProcessCalled)
            {
                _IsProcessCalled = true;
                if (_h.Process()) return;      // Call GameLogic's Process Method
            }
            _TickCountElapsed = Environment.TickCount - _TickCount;
            if (_TickCountElapsed >= _NumTicksPerLoop)
            {
                _IsProcessCalled = false;
                _TickCount = Environment.TickCount;
            }
            ((Storyboard)sender).Begin();
            #endregion
        }

        #endregion
    }

    #region IGameLoopHandler
    public interface IGameLoopHandler
    {
        /// <summary>
        /// 进入游戏循环前的初始化，返回 true 表示中断操作
        /// </summary>
        bool Init();

        /// <summary>
        /// 每个游戏循环的被调函数，返回 true 表示中断操作
        /// </summary>
        bool Process();
    }
    #endregion

    #region GameLoopHandler
    public class GameLoopHandler : IGameLoopHandler
    {
        #region Constructor
        public GameLoopHandler(Canvas canvas)
        {
            this.BaseCanvas = canvas;
            canvas.KeyDown += new KeyEventHandler(canvas_KeyDown);
            canvas.KeyUp += new KeyEventHandler(canvas_KeyUp);
            canvas.MouseLeftButtonDown += new MouseButtonEventHandler(canvas_MouseLeftButtonDown);
            canvas.MouseLeftButtonUp += new MouseButtonEventHandler(canvas_MouseLeftButtonUp);
            canvas.MouseMove += new MouseEventHandler(canvas_MouseMove);
        }
        #endregion

        #region Init
        public bool Init()
        {
            this.MapWidth = 40;
            this.MapHeight = 25;

            this.Floor = new Floor(this);
            this.Snake = new Snake(this);
            this.Food = new Food(this);

            this.Floor.Init();
            this.Snake.Init();
            this.Food.Init();

            return false;
        }
        #endregion

        #region Process
        public bool Process()
        {
            this.Snake.Process();
            this.Food.Process();
            this.Floor.Process();

            return false;
        }
        #endregion

        #region Properties

        public Canvas BaseCanvas { get; private set; }

        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }

        public Snake Snake { get; private set; }
        public Food Food { get; private set; }
        public Floor Floor { get; private set; }

        public bool IsLeftKeyDown { get; private set; }
        public bool IsUpKeyDown { get; private set; }
        public bool IsDownKeyDown { get; private set; }
        public bool IsRightKeyDown { get; private set; }
        public bool IsSpaceKeyDown { get; private set; }
        public bool IsMouseLeftKeyDown { get; private set; }
        public Point MousePosition { get; private set; }
        public int MouseGridPositionY { get; private set; }
        public int MouseGridPositionX { get; private set; }

        #endregion

        #region kb, mouse handle

        private void canvas_KeyDown(object sender, KeyEventArgs e)
        {
            // 映射键盘按键按下信息到属性
            if (e.Key == Key.A) IsLeftKeyDown = true;
            else if (e.Key == Key.S) IsDownKeyDown = true;
            else if (e.Key == Key.D) IsRightKeyDown = true;
            else if (e.Key == Key.W) IsUpKeyDown = true;
            else if (e.Key == Key.Space) IsSpaceKeyDown = true;
        }

        private void canvas_KeyUp(object sender, KeyEventArgs e)
        {
            // 映射键盘按键弹起信息到属性
            if (e.Key == Key.A) IsLeftKeyDown = false;
            else if (e.Key == Key.S) IsDownKeyDown = false;
            else if (e.Key == Key.D) IsRightKeyDown = false;
            else if (e.Key == Key.W) IsUpKeyDown = false;
            else if (e.Key == Key.Space) IsSpaceKeyDown = false;
        }

        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 映射鼠标按钮按下信息到属性
            IsMouseLeftKeyDown = true;
        }

        private void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 映射鼠标按钮弹起信息到属性
            IsMouseLeftKeyDown = false;
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            // 映射鼠标当前位置到属性
            MousePosition = e.GetPosition(BaseCanvas);

            // 将鼠标位置换算为格子坐标
            MouseGridPositionX = (int)MousePosition.X / MapWidth;
            MouseGridPositionY = (int)MousePosition.Y / MapHeight;
        }
        #endregion
    }
    #endregion

    #region Snake
    public partial class Snake : IGameLoopHandler
    {
        private GameLoopHandler _h;
        public Snake(GameLoopHandler gl)
        {
            _h = gl;
        }
        public bool Init()
        {
            // todo
            return false;
        }
        public bool Process()
        {
            // todo
            return false;
        }


        private List<Tile> _body = new List<Tile>();
        public enum SnakeOrientation
        {
            Up, Down, Left, Right
        }

        public void Move()
        {
            if (_h.IsLeftKeyDown && _h.IsUpKeyDown)
            {
                _body[0].Orientation = TileOrientations.LeftTop;
            }
            else if (_h.IsRightKeyDown && _h.IsUpKeyDown)
            {
                _body[0].Orientation = TileOrientations.RightTop;
            }
            else if (_h.IsLeftKeyDown && _h.IsDownKeyDown)
            {
                _body[0].Orientation = TileOrientations.LeftBottom;
            }
            else if (_h.IsRightKeyDown && _h.IsDownKeyDown)
            {
                _body[0].Orientation = TileOrientations.RightBottom;
            }
            else if (_h.IsLeftKeyDown)
            {
                _body[0].Orientation = TileOrientations.Left;
            }
            else if (_h.IsRightKeyDown)
            {
                _body[0].Orientation = TileOrientations.Right;
            }
            else if (_h.IsUpKeyDown)
            {
                _body[0].Orientation = TileOrientations.Top;
            }
            else if (_h.IsDownKeyDown)
            {
                _body[0].Orientation = TileOrientations.Bottom;
            }
            //if (_IsSmoothMoving) return;
            //if (!MoveCheck()) return;

            //// 前进
            //this.前进();
        }
    }
    #endregion

    #region Food
    public partial class Food : IGameLoopHandler
    {
        private GameLoopHandler _h;

        public Food(GameLoopHandler gl)
        {
            _h = gl;
        }

        public bool Init()
        {
            // todo : create foods
            _foods.Add(new FoodTile(_h.BaseCanvas) { X = 3, Y = 5 });
            _foods.Add(new FoodTile(_h.BaseCanvas) { X = 7, Y = 7 });

            return false;
        }

        private List<Tile> _foods = new List<Tile>();

        /// <summary>
        /// 原地转圈
        /// </summary>
        public bool Process()
        {
            CmdCounter = 0;									//复位指令计数器

            if (ChangeOrientation(TileOrientations.Bottom)) return false;
            if (ChangeOrientation(TileOrientations.LeftBottom)) return false;
            if (ChangeOrientation(TileOrientations.Left)) return false;
            if (ChangeOrientation(TileOrientations.LeftTop)) return false;
            if (ChangeOrientation(TileOrientations.Top)) return false;
            if (ChangeOrientation(TileOrientations.RightTop)) return false;
            if (ChangeOrientation(TileOrientations.Right)) return false;
            if (ChangeOrientation(TileOrientations.RightBottom)) return false;

            CmdPoint = 1;									//跳转到第一条指令

            return false;
        }

        int CmdCounter, CmdPoint = 1;

        public bool ChangeOrientation(TileOrientations o)
        {
            if (++CmdCounter != CmdPoint) return false;		//自增编号，并判断是否执行当前方法，不执行则返回 false
            CmdPoint = CmdCounter + 1;


            foreach (var food in _foods) food.Orientation = o;


            return true;									//退出指令序列
        }

    }
    #endregion

    #region Floor
    public partial class Floor : IGameLoopHandler
    {
        private GameLoopHandler _h;
        private List<Tile> _wall = new List<Tile>();

        public Floor(GameLoopHandler gl)
        {
            _h = gl;
        }

        public bool Init()
        {
            // 初始化 地板 和 墙
            for (int i = 0; i < _h.MapWidth; i++)
            {
                for (int j = 0; j < _h.MapHeight; j++)
                {
                    var f = new FloorTile(_h.BaseCanvas) { X = i, Y = j };
                    if (i == 0 || j == 0 || i == _h.MapWidth - 1 || j == _h.MapHeight - 1)
                    {
                        f.Opacity = 1;
                        _wall.Add(f);
                    }
                    else f.Opacity = 0.05;
                }
            }
            return false;
        }

        public bool Process()
        {
            // todo
            //if (_wall.Any(o => { return o.X == ?.X && o.Y == ?.Y; }))
            //{
            // return true;
            //}
            return false;
        }
    }
    #endregion
}