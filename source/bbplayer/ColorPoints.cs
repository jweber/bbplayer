using System;
using System.Collections.Generic;
using System.Drawing;

namespace bbplayer
{
    class ColorPoints
    {
        public const int TopMostOffset = -10;
        public const int TopMiddleOffset = -5;

        public const int BottomMostOffset = 10;
        public const int BottomMiddleOffset = 5;

        public const int LeftMostOffset = -10;
        public const int RightMostOffset = -10;


        public Color Center { get; set; }
        public Color TopMost { get; set; }
        public Color TopMiddle { get; set; }
        public Color BottomMost { get; set; }
        public Color BottomMiddle { get; set; }
        public Color LeftMost { get; set; }
        public Color RightMost { get; set; }

        public ColorPoints()
        {}

        public ColorPoints( Color color )
        {
            Center = color;
            
            TopMost = color;
            TopMiddle = color;

            BottomMost = color;
            BottomMiddle = color;

            LeftMost = color;
            RightMost = color;
        }

        public ColorPoints( Bitmap bitmap )
        {
            int centerX = ( bitmap.Width / 2 ) - 2;
            int centerY = ( bitmap.Height / 2 ) + 3;

            Center = bitmap.GetPixel( centerX, centerY );

            TopMost = bitmap.GetPixel( centerX, centerY + TopMostOffset );
            TopMiddle = bitmap.GetPixel( centerX, centerY + TopMiddleOffset );

            BottomMost = bitmap.GetPixel( centerX, centerY + BottomMostOffset );
            BottomMiddle = bitmap.GetPixel( centerX, centerY + BottomMiddleOffset );

            LeftMost = bitmap.GetPixel( centerX + LeftMostOffset, centerY );
            RightMost = bitmap.GetPixel( centerX + RightMostOffset, centerY );
        }
    }
}
