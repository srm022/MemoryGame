using MemoryGame.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MemoryGame
{
    class Tile
    {
        public Button tileButton;
        bool isOccupied = false;
        System.Windows.Controls.Image tileImage;
        int matchKey;

        public void SetMatchKey(int value)
        {
            matchKey = value;
        }

        public int GetMatchKey()
        {
            return matchKey;
        }

        public Tile()
        {
            tileButton = new Button();
        }

        public System.Windows.Controls.Image GetTileImage()
        {
            return tileImage;
        }

        public void SetTileImage(string img)
        {
            tileImage = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri(img)),
                VerticalAlignment = System.Windows.VerticalAlignment.Center
            };

            SetOccupied(true);
        }

        public void ImageHidden()
        {
            tileButton.Content = " ";
        }

        public void ImageVisible()
        {
            tileButton.Content = tileImage;
        }

        public bool IsOccupied()
        {
            return isOccupied;
        }

        public void SetOccupied(bool status)
        {
            isOccupied = status;
        }
    }
}
