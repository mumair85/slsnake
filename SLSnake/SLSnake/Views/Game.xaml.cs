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

namespace SLSnake
{
    public partial class Game : Page
    {
        public Game()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(Game_Loaded);
        }

        void Game_Loaded(object sender, RoutedEventArgs e)
        {
            //var ft = new SLSnake.Elements.FloorTile(this.LayoutRoot)
            //{
            //    X = 1,
            //    Y = 1
            //};
            ChildWindow errorWin = new ErrorWindow("img source", _img.Source.ToString());
            errorWin.Show();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}