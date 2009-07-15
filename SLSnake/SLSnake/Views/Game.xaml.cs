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

        GameLogic _gl;

        void Game_Loaded(object sender, RoutedEventArgs e)
        {
            #region GameLogic Init
            _gl = new GameLogic(this.LayoutRoot);
            if (_gl.Init()) return;
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
                if (_gl.Process()) return;      // Call GameLogic's Process Method
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

    public class GameLogic : IGameLoopHandler
    {
        public GameLogic(Canvas canvas)
        {
            this.BaseCanvas = canvas;
            this.MapWidth = 40;
            this.MapHeight = 25;
        }

        public bool Init()
        {
            this.Floor = new Floor(this);
            this.Snake = new Snake(this);
            this.Food = new Food(this);

            return false;
        }

        public bool Process()
        {
            this.Snake.Process();
            this.Food.Process();
            this.Floor.Process();

            return false;
        }

        public Canvas BaseCanvas { get; private set; }
        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }

        public Snake Snake { get; private set; }
        public Food Food { get; private set; }
        public Floor Floor { get; private set; }
    }

    public partial class Snake : IGameLoopHandler
    {
        private GameLogic _gl;

        public Snake(GameLogic gl)
        {
            _gl = gl;
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
    }

    public partial class Food : IGameLoopHandler
    {
        private GameLogic _gl;

        public Food(GameLogic gl)
        {
            _gl = gl;
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
    }

    public partial class Floor : IGameLoopHandler
    {
        private GameLogic _gl;
        private List<Tile> _wall = new List<Tile>();

        public Floor(GameLogic gl)
        {
            _gl = gl;
        }

        public bool Init()
        {
            // 初始化地板
            for (int i = 0; i < _gl.MapWidth; i++)
            {
                for (int j = 0; j < _gl.MapHeight; j++)
                {
                    var f = new FloorTile(_gl.BaseCanvas) { X = i, Y = j };
                    if (i == 0 || j == 0 || i == _gl.MapWidth - 1 || j == _gl.MapHeight - 1)
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
}