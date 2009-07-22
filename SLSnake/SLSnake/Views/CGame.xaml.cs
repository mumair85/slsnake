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
using System.Windows.Shapes;
using System.Diagnostics;
using SLSnake.Elements;

namespace SLSnake.Views
{
    public partial class CGame : ChildWindow
    {
        public CGame()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(CGame_Loaded);
        }

        #region GameLoop

        #region GameLoop
        int _NumTicksPerLoop = 10;
        int _TickCount, _TickCountElapsed;
        bool _IsProcessCalled = false;
        #endregion

        #region FPS display
        TextBlock _FPS_TextBlock = new TextBlock() { Foreground = new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0, 0)), FontSize = 16.0 };
        int _FrameRate, _LastTime;
        #endregion

        GameLoopHandler _h;

        void CGame_Loaded(object sender, RoutedEventArgs e)
        {
            #region GameLogic Init
            _h = new GameLoopHandler(this, this.LayoutRoot, this._floor_canvas, this._food_canvas, this._snake_canvas);
            if (_h.Init()) return;
            #endregion

            #region FPS
            this.LayoutRoot.Children.Add(_FPS_TextBlock);
            Canvas.SetLeft(_FPS_TextBlock, 10);
            Canvas.SetTop(_FPS_TextBlock, 10);
            Canvas.SetZIndex(_FPS_TextBlock, 99999);
            #endregion

            #region GameLoop
            var sb = new Storyboard();
            sb.Completed += new EventHandler(sb_Completed);
            sb.Begin();
            #endregion
        }

        void sb_Completed(object sender, EventArgs e)
        {
            #region FPS
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
        /// return True to break
        /// </summary>
        bool Init();

        /// <summary>
        /// return True to break
        /// </summary>
        bool Process();
    }
    #endregion

    #region GameLoopHandler
    public class GameLoopHandler : IGameLoopHandler
    {
        #region Constructor
        public GameLoopHandler(Control kbCatcher, Canvas baseCanvas, Canvas floorCanvas, Canvas foodCanvas, Canvas snakeCanvas)
        {
            this.KBCatcher = kbCatcher;
            this.BaseCanvas = baseCanvas;
            this.FloorCanvas = floorCanvas;
            this.FoodCanvas = foodCanvas;
            this.SnakeCanvas = snakeCanvas;

            this.KBCatcher.KeyDown += new KeyEventHandler(canvas_KeyDown);
            this.KBCatcher.KeyUp += new KeyEventHandler(canvas_KeyUp);
            this.BaseCanvas.MouseLeftButtonDown += new MouseButtonEventHandler(canvas_MouseLeftButtonDown);
            this.BaseCanvas.MouseLeftButtonUp += new MouseButtonEventHandler(canvas_MouseLeftButtonUp);
            this.BaseCanvas.MouseMove += new MouseEventHandler(canvas_MouseMove);

            //this.PressedKeys = new Queue<Key>(10);
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
            if (this.Snake.Process()) return true;
            if (this.Food.Process()) return true;
            if (this.Floor.Process()) return true;

            return false;
        }
        #endregion

        #region Properties

        public Control KBCatcher { get; private set; }

        public Canvas BaseCanvas { get; private set; }
        public Canvas FloorCanvas { get; private set; }
        public Canvas FoodCanvas { get; private set; }
        public Canvas SnakeCanvas { get; private set; }

        public short MapWidth { get; private set; }
        public short MapHeight { get; private set; }

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
        public short MouseGridPositionY { get; private set; }
        public short MouseGridPositionX { get; private set; }
        //public Queue<Key> PressedKeys { get; private set; }

        #endregion

        #region kb, mouse handle

        private void canvas_KeyDown(object sender, KeyEventArgs e)
        {
            // this game only support 4-way orientation, so ....

            if (e.Key == Key.Left)
            {
                IsLeftKeyDown = true;
                IsDownKeyDown = IsRightKeyDown = IsUpKeyDown = false;
            }
            else if (e.Key == Key.Down)
            {
                IsDownKeyDown = true;
                IsLeftKeyDown = IsRightKeyDown = IsUpKeyDown = false;
            }
            else if (e.Key == Key.Right)
            {
                IsRightKeyDown = true;
                IsLeftKeyDown = IsDownKeyDown = IsUpKeyDown = false;
            }
            else if (e.Key == Key.Up)
            {
                IsUpKeyDown = true;
                IsLeftKeyDown = IsDownKeyDown = IsRightKeyDown = false;
            }
            else if (e.Key == Key.Space)
            {
                IsSpaceKeyDown = true;
            }

            //PressedKeys.Enqueue(e.Key);
        }

        private void canvas_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A) IsLeftKeyDown = false;
            else if (e.Key == Key.S) IsDownKeyDown = false;
            else if (e.Key == Key.D) IsRightKeyDown = false;
            else if (e.Key == Key.W) IsUpKeyDown = false;
            else if (e.Key == Key.Space) IsSpaceKeyDown = false;
        }

        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsMouseLeftKeyDown = true;
        }

        private void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsMouseLeftKeyDown = false;
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            MousePosition = e.GetPosition(BaseCanvas);

            MouseGridPositionX = (short)(MousePosition.X / MapWidth);
            MouseGridPositionY = (short)(MousePosition.Y / MapHeight);
        }
        #endregion
    }
    #endregion

    #region Snake
    public partial class Snake : IGameLoopHandler
    {
        private List<SnakeTile> _body = new List<SnakeTile>();

        private GameLoopHandler _h;
        public Snake(GameLoopHandler gl)
        {
            _h = gl;
        }
        public bool Init()
        {
            // todo: custom length, location

            for (int i = 0; i < 7; i++)
            {
                _body.Add(new SnakeTile(_h.SnakeCanvas)
                {
                    X = 20,
                    Y = (short)(10 - i),
                    Orientation = TileOrientations.Bottom
                });
            }

            return false;
        }
        public bool Process()
        {
            var head = _body[0];
            if (_h.IsLeftKeyDown && _h.IsUpKeyDown)
            {
                head.Orientation = TileOrientations.LeftTop;
            }
            else if (_h.IsRightKeyDown && _h.IsUpKeyDown)
            {
                head.Orientation = TileOrientations.RightTop;
            }
            else if (_h.IsLeftKeyDown && _h.IsDownKeyDown)
            {
                head.Orientation = TileOrientations.LeftBottom;
            }
            else if (_h.IsRightKeyDown && _h.IsDownKeyDown)
            {
                head.Orientation = TileOrientations.RightBottom;
            }
            else if (_h.IsLeftKeyDown)
            {
                head.Orientation = TileOrientations.Left;
            }
            else if (_h.IsRightKeyDown)
            {
                head.Orientation = TileOrientations.Right;
            }
            else if (_h.IsUpKeyDown)
            {
                head.Orientation = TileOrientations.Top;
            }
            else if (_h.IsDownKeyDown)
            {
                head.Orientation = TileOrientations.Bottom;
            }

            if (head.IsSmoothMoving)
            {
                foreach (var body in _body) body.Go();
            }
            else
            {
                for (int i = 1; i < _body.Count; i++)
                {
                    var current = _body[i];
                    var previous = _body[i - 1];
                    if (current.Location != previous.Location)
                    {
                        var a = current.Location;
                        var b = previous.Location;
                        if (a.X < b.X) current.Orientation = TileOrientations.Right;
                        else if (a.X > b.X) current.Orientation = TileOrientations.Left;
                        else if (a.Y < b.Y) current.Orientation = TileOrientations.Bottom;
                        else if (a.Y > b.Y) current.Orientation = TileOrientations.Top;
                    }
                }

                var nextLocation = head.GetNextLocation();

                var foods = _h.Food;
                var food = foods.Get(nextLocation);
                while (food != null)
                {
                    _body.Insert(0, new SnakeTile(_h.SnakeCanvas) { Location = nextLocation, Orientation = head.Orientation });
                    foods.Eat(food);
                    foods.Grow(3);
                    nextLocation = _body[0].GetNextLocation();
                    food = foods.Get(nextLocation);
                }

                var blocks = _h.Floor;

                if (!blocks.EatCheck(nextLocation))
                {
                    foreach (var body in _body) body.Go();
                }
            }
            return false;
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

        private List<FoodTile> _foods = new List<FoodTile>();
        private List<Location> _spaces = new List<Location>();
        private Random _rnd = new Random(Environment.TickCount);

        public bool Init()
        {
            // register empty locations
            for (short i = 1; i < _h.MapWidth - 1; i++)
                for (short j = 1; j < _h.MapHeight - 1; j++)
                    _spaces.Add(new Location { X = i, Y = j });

            // grow foods
            Grow(30);

            return false;
        }

        /// <summary>
        /// rand create some foods ( sync _spaces )
        /// </summary>
        public void Grow(int num)
        {
            // todo: 避开蛇身

            if (num > _spaces.Count)
            {
                foreach (var location in _spaces)
                {
                    _foods.Add(new FoodTile(_h.FoodCanvas) { Location = location, Opacity = 0.5 });
                }
                _spaces.Clear();
            }
            else
            {
                for (int i = 0; i < num; i++)
                {
                    var idx = _rnd.Next(_spaces.Count);
                    var location = _spaces[idx];
                    _spaces.RemoveAt(idx);
                    _foods.Add(new FoodTile(_h.FoodCanvas) { Location = location, Opacity = 0.5 });
                }
            }
        }

        /// <summary>
        /// remove food from foods ( sync _spaces )
        /// </summary>
        public void Eat(FoodTile food)
        {
            _spaces.Add(food.Location);
            _h.FoodCanvas.Children.Remove(food);
            _foods.Remove(food);
        }

        /// <summary>
        /// check location food exists
        /// </summary>
        public FoodTile Get(Location location)
        {
            foreach (var food in _foods) if (food.Location == location) return food;
            return null;
        }

        /// <summary>
        /// turn around (always)
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
            // init  floor & wall
            for (short i = 0; i < _h.MapWidth; i++)
            {
                for (short j = 0; j < _h.MapHeight; j++)
                {
                    var f = new FloorTile(_h.FloorCanvas) { X = i, Y = j };
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
            //if (_wall.Any(o => { return o.Location == ?.Location; }))
            //{
            // return true;
            //}
            return false;
        }

        public bool EatCheck(Location location)
        {
            foreach (var wall in _wall) if (wall.Location == location) return true;
            return false;
        }
    }
    #endregion
}
