using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class RezultImage
    {
        List<ImageSides> images;
        public List<List<ImageSides>> imageSidesRezultImage { private set; get; }
        public RezultImage(List<ImageSides> images1)
        {
            this.images = images1;
            //Create dictionary with all imaging and best similarety they have
            Dictionary<ImageSides, Option> optionDictionary = new Dictionary<ImageSides, Option>();
            foreach (var imageSideBase in images)
                optionDictionary.Add(imageSideBase, imageSideBase.GetMaxSimilarety());

            //get the bigest similarety 
            var opt = optionDictionary.OrderBy(ImgSt => ImgSt.Value.similarity).First();

            //List of parts for rezult 
            imageSidesRezultImage = new List<List<ImageSides>>();
            imageSidesRezultImage.Add(new List<ImageSides>());
            imageSidesRezultImage.First().Add(opt.Key);

            AddImageToRezultList(opt.Key, opt.Value.img, opt.Value.side);
            images.Remove(opt.Key);
            images.Remove(opt.Value.img);
            
            Recursive();

            Bitmap imageRow = null;
            foreach (var list in imageSidesRezultImage)
            {
                var imageCol = new Bitmap(10,10);
                if (list.First().image != null)
                    imageCol = new Bitmap(list.First().image);
                
                for (int i = 1; i <= list.Count-1; i++)
                {
                    if(list[i].image != null)
                        imageCol = Connect(list[i].image, imageCol);
                    else
                        imageCol = Connect(new Bitmap(10, 10), imageCol);
                }

                if (imageRow == null)
                {
                    imageRow = imageCol;
                }
                else imageRow = Connect(imageRow, imageCol, true);
            }
        }

        public void Recursive()
        {
            if (images.Count != 0)
            {
                var dictForRezultParts = new Dictionary<ImageSides, Option>();
                foreach (var listImages in imageSidesRezultImage)
                    foreach (var images in listImages)
                        if(images != null) dictForRezultParts.Add(images,images.GetMaxSimilarety());

                //get the bigest similarety 
                var opt = dictForRezultParts.Where(x=> x.Value.img!=null).ToList().OrderBy(ImgSt => ImgSt.Value.similarity).First();
                AddImageToRezultList(opt.Key, opt.Value.img, opt.Value.side);
                images.RemoveAt(0);

                Recursive();
            }
            else
                return;
        }


        public void AddImageToRezultList(ImageSides imgBase, ImageSides imgNew, Side pos)
        {
            //coordinats
            int x = 0, y = 0; 

            //find location of base image
            foreach (var images1 in imageSidesRezultImage)
            {
                int loc = -1, iter = 0;
                foreach (var img in images1)
                {
                    if (img != null && img.image == imgBase.image)
                        loc = iter;
                    iter++;
                }
                    
                if (loc != -1)
                {
                    x = loc;
                    y = imageSidesRezultImage.FindIndex(i => i == images1);
                }                 
            }

            //position of new one to base
            switch (pos)
            {   //to add new image under base one
                case Side.Top: //Bot to the Base one
                    {
                        try
                        {
                            var sms = imageSidesRezultImage[y + 1]; // try if we have enouth lists under our
                            if (sms.Count > x)
                                imageSidesRezultImage[y + 1][x] = imgNew;
                            else
                            {
                                for (int i = imageSidesRezultImage[y + 1].Count; i <= x; i++)
                                    imageSidesRezultImage[y + 1].Add(null);
                                imageSidesRezultImage[y + 1][x] = imgNew;
                            }
                        }
                        catch (Exception)
                        {
                            imageSidesRezultImage.Insert(y+1, new List<ImageSides>());
                            if (x == 0)
                                imageSidesRezultImage[y + 1].Add(imgNew);
                            else
                            {
                                for (int i = 0; i <= x; i++)
                                    imageSidesRezultImage[y + 1].Add(null);
                                imageSidesRezultImage[y + 1][x] = imgNew;
                            }
                        }
                        y++;
                        LockSide(x, y);
                        break;
                    }
                case Side.Bot: //Top to the Base one
                    {
                        try
                        {
                            var sms = imageSidesRezultImage[y - 1]; // try if we have enouth lists over our
                            if (sms.Count-1 >= x)                   //we have place
                                imageSidesRezultImage[y - 1][x] = imgNew;
                            else
                            {                                       //we have row but no place under
                                for (int i = imageSidesRezultImage[y - 1].Count; i <= x; i++)
                                    imageSidesRezultImage[y - 1].Add(null);
                                imageSidesRezultImage[y - 1][x] = imgNew;
                            }
                        }
                        catch (Exception)                           // if y == 0
                        {
                            imageSidesRezultImage.Insert(0, new List<ImageSides>()); 
                            if (x == 0)
                                imageSidesRezultImage[y].Add(imgNew);
                            else
                            {
                                for (int i = 0; i < x; i++)
                                    imageSidesRezultImage[y].Add(null);
                                imageSidesRezultImage[y][x] = imgNew;
                            }
                        }
                        y--;
                        LockSide(x, y);              
                        break;
                    }
                case Side.Left: //Right to the Base one
                    {
                        try
                        {
                            var sms = imageSidesRezultImage[y][x + 1]; // try if we have enouth place in right side
                            imageSidesRezultImage[y][x + 1] = imgNew;
                        }
                        catch (Exception)
                        {
                            imageSidesRezultImage[y].Add(imgNew);
                        }

                        x++;
                        LockSide(x, y);
                        break;
                    }
                case Side.Right: //Left to the Base one
                    {
                        try
                        {
                            var sms = imageSidesRezultImage[y][x - 1]; // try if we have enouth space left to base one
                            imageSidesRezultImage[y][x - 1] = imgNew;
                        }
                        catch (Exception)   //case of x == 0
                        {
                            imageSidesRezultImage[y].Insert(0, imgNew);

                            foreach (var lists in imageSidesRezultImage)
                                if (imageSidesRezultImage.IndexOf(lists) != y)
                                    lists.Insert(0, null);
                        }

                        LockSide(x, y);
                        break;
                    }
            }
        }

        private void LockSide(int x, int y)
        {
            //lock top
            try
            {
                if (imageSidesRezultImage[y + 1][x] != null)
                {
                    imageSidesRezultImage[y + 1][x].LockSide(Side.Top);
                    imageSidesRezultImage[y][x].LockSide(Side.Bot);
                }
            }
            catch (Exception) { }

            try
            {
                if (imageSidesRezultImage[y - 1][x] != null)
                {
                    imageSidesRezultImage[y - 1][x].LockSide(Side.Bot);
                    imageSidesRezultImage[y][x].LockSide(Side.Top);
                }
            }
            catch (Exception) { }

            try
            {
                if (imageSidesRezultImage[y][x - 1] != null)
                {
                    imageSidesRezultImage[y][x - 1].LockSide(Side.Right);
                    imageSidesRezultImage[y][x].LockSide(Side.Left);
                }
            }
            catch (Exception) { }

            try
            {
                if (imageSidesRezultImage[y][x + 1] != null)
                {
                    imageSidesRezultImage[y][x + 1].LockSide(Side.Left);
                    imageSidesRezultImage[y][x].LockSide(Side.Right);
                }
            }
            catch (Exception) { }
        }

        public Bitmap Connect(Bitmap imageBit, Bitmap imageBit2, bool botSide = false)
        {
            Image img1 = imageBit;
            Image img2 = imageBit2;

            int width, height;
            if (botSide) 
            {
                 width = Math.Max(img1.Width,img2.Width);
                 height = img1.Height + img2.Height;
            }
            else
            {
                 width = img1.Width + img2.Width;
                 height = Math.Max(img1.Height, img2.Height);
            }

            Bitmap img3 = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(img3);

            g.Clear(Color.Black);
            g.DrawImage(img1, new Point(0, 0));
            if (botSide)
                g.DrawImage(img2, new Point(0, img1.Height));
            else
                g.DrawImage(img2, new Point(img1.Width, 0));

            img3.Save(@"C:\Users\profe\Desktop\image3.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            g.Dispose();

            return img3;
        }


    }
}
