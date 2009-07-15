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
    public partial class FoodTile : Tile
    {
        public FoodTile(Canvas p) : base(p) { }
        public FoodTile(Canvas p, double speedRatio) : base(p, speedRatio) { }

        private static ImageBrush _ImageBrush = new ImageBrush()
        {
            ImageSource = new BitmapImage(new Uri(@"/Images/player.png", UriKind.Relative)),
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
                return this.Y + 100;
            }
        }
    }
}
