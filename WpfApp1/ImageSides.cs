using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    enum Side
    {
        Top,
        Bot,
        Left,
        Right
    }
    struct Option
    {
        public ImageSides img;
        public Side side;
        public double similarity;
    }
    class ImageSides
    {
        public Bitmap image { private set; get; }

        Dictionary<Side, Color[]> map;

        List<Option> top_map;
        List<Option> bot_map;
        List<Option> left_map;
        List<Option> right_map;
        public ImageSides(Bitmap image)
        {
            this.image = image;
            map = new Dictionary<Side, Color[]>();
            top_map = new List<Option>();
            bot_map = new List<Option>();
            left_map = new List<Option>();
            right_map = new List<Option>();

            map.Add(Side.Top, Colors(image.Width, 0, true));
            map.Add(Side.Bot, Colors(image.Width, image.Height-1, true));
            map.Add(Side.Left, Colors(image.Height, image.Width-1, false));
            map.Add(Side.Right, Colors(image.Height, 0, false));
        }

        //we have image from this side
        public void LockSide(Side side)
        {
            map.Remove(side);
        }

        public Option GetMaxSimilarety()
        {
            var top = top_map.OrderBy(x => x.similarity).First();
            var bot = bot_map.OrderBy(x => x.similarity).First();
            var left = left_map.OrderBy(x => x.similarity).First();
            var right = right_map.OrderBy(x => x.similarity).First();

            double min = Double.MaxValue;
            var rezultOpt = new Option(); 
            foreach (var keyValue in map)
            {
                switch (keyValue.Key)
                {
                    case Side.Top:
                        {
                            if (min > top.similarity) { min = top.similarity; rezultOpt = top; }
                            break;
                        }
                    case Side.Bot:
                        {
                            if (min > bot.similarity) { min = bot.similarity; rezultOpt = bot; }
                            break;
                        }
                    case Side.Left:
                        {
                            if (min > left.similarity) { min = left.similarity; rezultOpt = left; }
                            break;
                        }
                    case Side.Right:
                        {
                            if (min > right.similarity) { min = right.similarity; rezultOpt = right; }
                            break;
                        }
                }
            }

            return rezultOpt;
        }

        public void MakeOption(ImageSides imageSides)
        {
            foreach (var keyValuePair in map)
            {
                switch (keyValuePair.Key)
                {
                    case Side.Top:
                    {
                        top_map.AddRange(CalculateOptionForEachSide(keyValuePair.Value, imageSides));                        
                        break;
                    }
                    case Side.Bot:
                    {
                        bot_map.AddRange(CalculateOptionForEachSide(keyValuePair.Value, imageSides));
                        break;
                    }
                    case Side.Left:
                    {
                        left_map.AddRange(CalculateOptionForEachSide(keyValuePair.Value, imageSides));
                        break;
                    }
                    case Side.Right:
                    {
                        right_map.AddRange(CalculateOptionForEachSide(keyValuePair.Value, imageSides));
                        break;
                    }
                }
            }
        }

        private List<Option> CalculateOptionForEachSide(Color[] value, ImageSides imageSidesExt)
        {
            var options = new List<Option>();
            foreach (var pairExternal in imageSidesExt.map)
            {
                if (pairExternal.Value.Length == value.Length)
                    options.Add(new Option()
                    {
                        img = imageSidesExt,
                        side = pairExternal.Key,
                        similarity = CalculateSimularity(value, pairExternal.Value)
                    });
            }
            return options;
        }

        private double CalculateSimularity(Color[] internals, Color[] externals)
        {
            var simul = 0.0;
            for (int i = 0; i < internals.Length; i++)
                simul += Math.Abs(internals[i].ToArgb() - externals[i].ToArgb());
            return simul/internals.Length;
        }

        private Color[] Colors(int numverOfMembers, int constNum, bool topBot)
        {
            Color[] colors = new Color[numverOfMembers];
            for (int x = 0; x < numverOfMembers; x++)
                colors[x] = (topBot)? image.GetPixel(x, constNum) : image.GetPixel(constNum, x);
            return colors;
        }
    }
}
