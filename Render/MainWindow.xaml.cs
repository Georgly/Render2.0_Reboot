using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Render
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int clickCount = 0;
        Device model;
        public MainWindow()
        {
            InitializeComponent();
            model = new Device(myCanvas);
        }

        private void _3D_Render_Load(object sender, RoutedEventArgs e)//не испльзуется
        {
        }

        private void openFileBt_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFD = new OpenFileDialog();
            openFD.ShowDialog();
            clickCount = 1;
            model.OpenFile(openFD.FileName);
        }

        private void drawModelBt_Click(object sender, RoutedEventArgs e)
        {
            //myCanvas.Children.Clear();
            model.Render((float)zoomSlider.Value);
            //if (clickCount != 3)
            //{
            //    clickCount++;
            //}
            //else
            //{
            //    clickCount = 0;
            //    clickCount++;
            //}
            //switch (clickCount)
            //{
            //    case 1:
            //        {
            //            model.Render();
            //            break;
            //        }
            //    //case 2:
            //    //    {
            //    //        model.RenderLine();
            //    //        break;
            //    //    }
            //    //case 3:
            //    //    {
            //    //        model.DrawModelFill(myCanva);
            //    //        break;
            //    //    }
            //    default:
            //        break;
            //}
        }

        private void upBt_Click(object sender, RoutedEventArgs e)
        {
            //model.VerticalMove(1, clickCount);
        }

        private void leftBt_Click(object sender, RoutedEventArgs e)
        {
            //model.HorizontalMove(-1, clickCount);
        }

        private void rightBt_Click(object sender, RoutedEventArgs e)
        {
            //model.HorizontalMove(1, clickCount);
        }

        private void downBt_Click(object sender, RoutedEventArgs e)
        {
            //model.VerticalMove(-1, clickCount);
        }

        private void turnBt_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(xCoord.IsChecked) == 0 && Convert.ToInt32(yCoord.IsChecked) == 0 && Convert.ToInt32(zCoord.IsChecked) == 0)
            {
                MessageBox.Show("Задайте ось(-и) вращения");
            }
            else
            {
                model.Rotation(/*clickCount, */Convert.ToInt32(xCoord.IsChecked), Convert.ToInt32(yCoord.IsChecked), Convert.ToInt32(zCoord.IsChecked));
            }
        }

        private void myCanva_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //myCanvas.Children.Clear();
            zoomSlider.Value += (e.Delta > 0) ? 0.2 : -0.2;
            model.ZoomModel(/*clickCount, */(float)zoomSlider.Value);
        }

        private void _3D_Render_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
