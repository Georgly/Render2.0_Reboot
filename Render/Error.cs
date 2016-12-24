using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Render
{
    class Error
    {
        public static void NoFile()
        {
            string err = ("Error");
            string main = ("File not found");
            MessageBox.Show(main, err);
        }

        public static void EnterError()
        {
            string err = ("Error");
            string main = ("Input or Output Error");
            MessageBox.Show(main, err);
        }

        public static void SomeError()
        {
            string err = ("Error");
            string main = ("ERROR!");
            MessageBox.Show(main, err);
        }

        public static void NullList()
        {
            string err = ("Error");
            string main = ("Nothing to draw");
            MessageBox.Show(main, err);
        }
    }
}
