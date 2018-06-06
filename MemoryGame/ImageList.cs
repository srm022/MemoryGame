using MemoryGame.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MemoryGame
{
    static class ImageList
    {
        static List<string> imgList;

        static public void AddImagesToList()
        {
            string[] images = System.IO.Directory.GetFiles(System.Configuration.ConfigurationSettings.AppSettings["images128Path"]);
            imgList = new List<string>(images);
        }

        static public void DeleteImageFromList(int index)
        {
            imgList.RemoveAt(index);
        }

        static public string GetImage(int index)
        {
            return imgList[index];
        }

        static public List<string> GetImageList()
        {
            return imgList;
        }
    }
}
