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
            _canvas = canvas;
        }


        public bool Init()
        {
            _width = 40;
            _height = 25;

            // 初始化地板

            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    var f = new FloorTile(_canvas) { X = i, Y = j, Opacity = ((i == 0 || j == 0 || i == _width - 1 || j == _height - 1) ? 1 : 0.05) };
                }
            }


            return false;
        }

        public bool Process()
        {
            return false;
        }

        private Random _rnd = new Random(Environment.TickCount);
        private Canvas _canvas;
        List<FloorTile> _floors = new List<FloorTile>();

        private int _width;
        private int _height;

        protected int GridWidth
        {
            get { return _width; }
            set { _width = value; }
        }
        protected int GridHeight
        {
            get { return _height; }
            set { _height = value; }
        }
        public List<FloorTile> Floors
        {
            get { return _floors; }
            set { _floors = value; }
        }

        //List<RoadBreak> _roadbreaks = new List<RoadBreak>();

        //public List<RoadBreak> RoadBreaks
        //{
        //    get { return _roadbreaks; }
        //    set { _roadbreaks = value; }
        //}

        //List<Body> _bodies = new List<Body>();

        //public List<Body> Bodies
        //{
        //    get { return _bodies; }
        //    set { _bodies = value; }
        //}

        //List<Food> _foods = new List<Food>();

        //public List<Food> Foods
        //{
        //    get { return _foods; }
        //    set { _foods = value; }
        //}

        //public Game(Canvas baseCanvas)
        //{
        //    _baseCanvas = baseCanvas;

        //    Init();
        //}


    }
}