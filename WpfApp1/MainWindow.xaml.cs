using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();

            if(result == CommonFileDialogResult.Ok)
                analyze(Directory.GetFiles(dialog.FileName, "*.*", SearchOption.AllDirectories).ToList());
        }

        public void analyze(List<string> lists)
        {
            //Create image objects from path
            var images = new List<ImageSides>();
            foreach (var imagePath in lists)
            {
                var imageSide = new ImageSides(new Bitmap(imagePath, true)); 
                images.Add(imageSide);
            }

            //create map of posible sides for each image with other ones
            foreach (var imageSideBase in images)
                foreach (var imageSide in images)
                    if (imageSideBase != imageSide)
                        imageSideBase.MakeOption(imageSide);

            //Create class to calculate others sides
            RezultImage rezultImage = new RezultImage(images);
            //
            

            /*
            switch (side)
            {
                case Side.Top:
                    {
                        
                        break;
                    }
                case Side.Bot:
                    {
                        break;
                    }
                case Side.Left:
                    {
                        break;
                    }
                case Side.Right:
                    {
                        break;
                    }
            }
            */
            //rezultImage.Connect(opt.Key, new ImageSides(opt.Value.img));
        }
    }

    public static class ImgSt
    {
        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
    }
}
