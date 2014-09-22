using System;
using System.Drawing;
using AForge.Imaging;

namespace bbplayer
{
    public class ColorUtility
    {
        public static Color GetAveragePieceColor(Bitmap bitmap, int pieceTopLeftX, int pieceTopLeftY)
        {
            float totalR = 0,
                  totalG = 0,
                  totalB = 0;

            int width = 40,
                height = 40;

            if (bitmap.Width < width)
                width = bitmap.Width;

            if (bitmap.Height < height)
                height = bitmap.Height;

            int skippedPixels = 0;
            for (int bbY = pieceTopLeftY + 2; bbY < pieceTopLeftY + height - 10; bbY++)
            {
                for (int bbX = pieceTopLeftX + 10; bbX < pieceTopLeftX + width - 10; bbX++)
                {
                    Color pixelColor = bitmap.GetPixel(bbX, bbY);
                    
                    var red = pixelColor.R;
                    var green = pixelColor.G;
                    var blue = pixelColor.B;

                    totalR += red;
                    totalG += green;
                    totalB += blue;
                }
            }

            int pixelCount = (width*height);
            int averageR = Convert.ToInt32(totalR/pixelCount);
            int averageG = Convert.ToInt32(totalG/pixelCount);
            int averageB = Convert.ToInt32(totalB/pixelCount);

            Color averageColor = Color.FromArgb(
                255,
                averageR,
                averageG,
                averageB);

//            var averageColor = FromAhsb(
//                255,
//                totalR/pixelCount,
//                totalG/pixelCount,
//                totalB/pixelCount);

            return averageColor;

        }

        public static int[] GetLuminanceHistogram(Bitmap bitmap, int pieceTopLeftX, int pieceTopLeftY)
        {
            var mask = new byte[bitmap.Height, bitmap.Width];
            
            for (int x = pieceTopLeftX; x < pieceTopLeftX + Math.Min(bitmap.Width, 40); x++)
            {
                for (int y = pieceTopLeftY; y < pieceTopLeftY + Math.Min(bitmap.Height, 40); y++)
                {
                    mask[y, x] = 1;
                }
            }

            var st = new ImageStatisticsHSL(bitmap, mask);
            int[] luminanceValues = st.LuminanceWithoutBlack.Values;
            return luminanceValues;
        }

        public static double GetHistogramDistance(int[] first, int[] second)
        {
            double result = 0;
            for (int i = 0; i < first.Length; i++)
            {
                result += (first[i] - second[i])*(first[i] - second[i]);
            }

            result = Math.Sqrt(result);
            return result;
        }

        public static int[] SmoothHistogram(int[] originalValues)
        {
            int[] smoothedValues = new int[originalValues.Length];
            double[] mask = new double[] { 0.25, 0.5, 0.25 };

            for (int bin = 1; bin < originalValues.Length - 1; bin++)
            {
                double smoothedValue = 0;
                for (int i = 0; i < mask.Length; i++)
                {
                    smoothedValue += originalValues[bin - 1 + i]*mask[i];
                }
                smoothedValues[bin] = (int) smoothedValue;
            }

            return smoothedValues;
        }

        public static Color GetAveragePieceLuminance(Bitmap bitmap, int pieceTopLeftX, int pieceTopLeftY)
        {
            float totalH = 0,
                  totalS = 0,
                  totalB = 0;

            int width = 40,
                height = 40;

            if (bitmap.Width < width)
                width = bitmap.Width;

            if (bitmap.Height < height)
                height = bitmap.Height;

            int skippedPixels = 0;
            for (int bbY = pieceTopLeftY + 2; bbY < pieceTopLeftY + height - 10; bbY++)
            {
                for (int bbX = pieceTopLeftX + 10; bbX < pieceTopLeftX + width - 10; bbX++)
                {
                    Color pixelColor = bitmap.GetPixel(bbX, bbY);


                    var hue = pixelColor.GetHue();
                    var saturation = pixelColor.GetSaturation();
                    if (hue < 0.01 && saturation < 0.01)
                    {
                        skippedPixels++;
                        continue;
                    }

                    totalH += hue;
                    totalS += saturation;
                    totalB += pixelColor.GetBrightness();
                }
            }

            int pixelCount = (width*height);
            var averageColor = FromAhsb(
                255,
                totalH/pixelCount,
                totalS/pixelCount,
                totalB/pixelCount);

            return averageColor;
        }

        /// <summary>
        /// Creates a Color from alpha, hue, saturation and brightness.
        /// </summary>
        /// <param name="alpha">The alpha channel value.</param>
        /// <param name="hue">The hue value.</param>
        /// <param name="saturation">The saturation value.</param>
        /// <param name="brightness">The brightness value.</param>
        /// <returns>A Color with the given values.</returns>
        /// <see cref="http://stackoverflow.com/a/4106615"/>
        public static Color FromAhsb(int alpha, float hue, float saturation, float brightness)
        {
            if (0 > alpha
                || 255 < alpha)
            {
                throw new ArgumentOutOfRangeException(
                    "alpha",
                    alpha,
                    "Value must be within a range of 0 - 255.");
            }

            if (0f > hue
                || 360f < hue)
            {
                throw new ArgumentOutOfRangeException(
                    "hue",
                    hue,
                    "Value must be within a range of 0 - 360.");
            }

            if (0f > saturation
                || 1f < saturation)
            {
                throw new ArgumentOutOfRangeException(
                    "saturation",
                    saturation,
                    "Value must be within a range of 0 - 1.");
            }

            if (0f > brightness
                || 1f < brightness)
            {
                throw new ArgumentOutOfRangeException(
                    "brightness",
                    brightness,
                    "Value must be within a range of 0 - 1.");
            }

            if (0 == saturation)
            {
                return Color.FromArgb(
                    alpha,
                    Convert.ToInt32(brightness*255),
                    Convert.ToInt32(brightness*255),
                    Convert.ToInt32(brightness*255));
            }

            float fMax, fMid, fMin;
            int iSextant, iMax, iMid, iMin;

            if (0.5 < brightness)
            {
                fMax = brightness - (brightness*saturation) + saturation;
                fMin = brightness + (brightness*saturation) - saturation;
            }
            else
            {
                fMax = brightness + (brightness*saturation);
                fMin = brightness - (brightness*saturation);
            }

            iSextant = (int) Math.Floor(hue/60f);
            if (300f <= hue)
            {
                hue -= 360f;
            }

            hue /= 60f;
            hue -= 2f*(float) Math.Floor(((iSextant + 1f)%6f)/2f);
            if (0 == iSextant%2)
            {
                fMid = (hue*(fMax - fMin)) + fMin;
            }
            else
            {
                fMid = fMin - (hue*(fMax - fMin));
            }

            iMax = Convert.ToInt32(fMax*255);
            iMid = Convert.ToInt32(fMid*255);
            iMin = Convert.ToInt32(fMin*255);

            switch (iSextant)
            {
                case 1:
                    return Color.FromArgb(alpha, iMid, iMax, iMin);
                case 2:
                    return Color.FromArgb(alpha, iMin, iMax, iMid);
                case 3:
                    return Color.FromArgb(alpha, iMin, iMid, iMax);
                case 4:
                    return Color.FromArgb(alpha, iMid, iMin, iMax);
                case 5:
                    return Color.FromArgb(alpha, iMax, iMin, iMid);
                default:
                    return Color.FromArgb(alpha, iMax, iMid, iMin);
            }
        }
    }
}