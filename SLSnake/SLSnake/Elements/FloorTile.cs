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
using System.Windows.Media.Imaging;

namespace SLSnake.Elements
{
    public partial class FloorTile : Tile
    {
        public FloorTile(Canvas p) : base(p) { }
        public FloorTile(Canvas p, double speedRatio) : base(p, speedRatio) { }

        private static ImageBrush _ImageBrush = new ImageBrush()
        {
            ImageSource = new BitmapImage(new Uri(@"/Images/floor.png", UriKind.Relative)),
            AlignmentX = AlignmentX.Left,
            AlignmentY = AlignmentY.Top,
            Stretch = Stretch.UniformToFill
        };

        protected override ImageBrush _FrameAnim_ImageBrush
        {
            get
            {
                return _ImageBrush;
            }
        }

        public override int Z
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// 地板的方向需要特殊实现  // todo
        /// </summary>
        public override TileOrientations Orientation
        {
            get
            {
                return _orientation;
            }
            set
            {
                _orientation = value;
            }
        }
    }
}
