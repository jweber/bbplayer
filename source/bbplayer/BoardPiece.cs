﻿using System.Windows.Media;

namespace bbplayer
{
    class BoardPiece
    {
        enum RootBoardPiece
        {
            None,
            RedSquare,
            OrangeDecahedron,
            PurpleTriangle,
            GreenSphere,
            BlueDiamond,
            GrayRock,
            YellowSquare,
            Any
        }

        private BoardPiece( RootBoardPiece rootBoardPiece, string name, decimal weight, Color color )
        {
            _rootBoardPiece = rootBoardPiece;
            Name = name;
            Weight = weight;
            Color = color;
        }

        private RootBoardPiece _rootBoardPiece;

        public Color Color { get; private set; }
        public string Name { get; private set; }
        public decimal Weight { get; private set; }

        public static BoardPiece FindMatch( Color color )
        {
            var c2 = System.Drawing.Color.FromArgb( color.A, color.R, color.G, color.B );
            float hue = c2.GetHue();
            float saturation = c2.GetSaturation();
            float brightness = c2.GetBrightness();

            if ( hue == 0 && saturation > 0 && saturation < 1 )
                return RedSquare;

            if ( hue >= 340 && hue < 350 && saturation <= 1 )
                return RedSquare_Multiplier;

            if ( hue == 0 && saturation == 0 && brightness > 0.85 && brightness < 1 )
                return GrayRock;

            if ( hue == 0 && saturation == 0 && brightness <= 0.85 )
                return GrayRock_Multiplier;

            if ( hue > 26 && hue < 40 )
                return OrangeDecahedron;

            if ( hue == 300 )
                return PurpleTriangle;

            if ( hue > 270 && hue < 300 )
                return PurpleTriangle_Multiplier;

            if ( hue == 120 )
                return GreenSphere;

            if ( hue > 120 && hue < 128 )
                return GreenSphere_Multiplier;

            if ( hue > 200 && hue < 215 && brightness >= .6 )
                return BlueDiamond;

            if ( hue > 200 && hue < 240 && brightness < .6 )
                return BlueDiamond_Multiplier;

            if ( hue > 58 && hue < 61 && saturation < .7 )
                return YellowSquare;

            if ( hue > 58 && hue < 61 && saturation >= .7 )
                return YellowSquare_Multiplier;

            if ( hue == 0 && saturation == 0 && brightness == 1 )
                return HyperCube;

            return new BoardPiece( RootBoardPiece.None, "Unknown", 0, Color.FromArgb( 255, 255, 255, 255 ) );            
        }

        public static BoardPiece RedSquare = new BoardPiece( RootBoardPiece.RedSquare, "Red Square", 1, Color.FromArgb( 255, 254, 100, 100 ) );
        public static BoardPiece OrangeDecahedron = new BoardPiece( RootBoardPiece.OrangeDecahedron, "Orange Decahedron", 1, Color.FromArgb( 255, 241, 125, 36 ) );
        public static BoardPiece PurpleTriangle = new BoardPiece( RootBoardPiece.PurpleTriangle, "Purple Triangle", 1, Color.FromArgb( 255, 213, 28, 213 ) );
        public static BoardPiece GreenSphere = new BoardPiece( RootBoardPiece.GreenSphere, "Green Sphere", 1, Color.FromArgb( 255, 35, 207, 35 ) );
        public static BoardPiece BlueDiamond = new BoardPiece( RootBoardPiece.BlueDiamond, "Blue Diamond", 1, Color.FromArgb( 255, 15, 96, 203 ) );
        public static BoardPiece GrayRock = new BoardPiece( RootBoardPiece.GrayRock, "Gray Rock", 1, Color.FromArgb( 255, 251, 251, 251 ) );
        public static BoardPiece YellowSquare = new BoardPiece( RootBoardPiece.YellowSquare, "Yellow Square", 1, Color.FromArgb( 255, 209, 207, 54 ) );

        public static BoardPiece RedSquare_Multiplier = new BoardPiece( RootBoardPiece.RedSquare, "Red Square Multiplier", 5, Color.FromArgb( 255, 157, 2, 51 ) );
        public static BoardPiece PurpleTriangle_Multiplier = new BoardPiece( RootBoardPiece.PurpleTriangle, "Purple Triangle Multiplier", 5, System.Windows.Media.Color.FromArgb( 255, 108, 1, 156 ) );
        public static BoardPiece GreenSphere_Multiplier = new BoardPiece( RootBoardPiece.GreenSphere, "Green Sphere Multiplier", 5, Color.FromArgb( 255, 0, 168, 11 ) );
        public static BoardPiece BlueDiamond_Multiplier = new BoardPiece( RootBoardPiece.BlueDiamond, "Blue Diamond Multiplier", 5, System.Windows.Media.Color.FromArgb( 255, 44, 87, 254 ) );
        public static BoardPiece GrayRock_Multiplier = new BoardPiece( RootBoardPiece.GrayRock, "Gray Rock Multiplier", 5, Color.FromArgb( 255, 125, 125, 125 ) );
        public static BoardPiece YellowSquare_Multiplier = new BoardPiece( RootBoardPiece.YellowSquare, "Yellow Square Multiplier", 5, Color.FromArgb( 255, 184, 182, 25 ) );

        public static BoardPiece HyperCube = new BoardPiece( RootBoardPiece.Any, "Hypercube", 10, Color.FromArgb( 255, 255, 255, 255 ) );

        #region Equality

        public bool Equals( BoardPiece other )
        {
            if ( ReferenceEquals( null, other ) )
            {
                return false;
            }
            if ( ReferenceEquals( this, other ) )
            {
                return true;
            }
            return Equals( other._rootBoardPiece, _rootBoardPiece );
        }

        public override bool Equals( object obj )
        {
            if ( ReferenceEquals( null, obj ) )
            {
                return false;
            }
            if ( ReferenceEquals( this, obj ) )
            {
                return true;
            }
            if ( obj.GetType() != typeof(BoardPiece) )
            {
                return false;
            }
            return Equals( (BoardPiece) obj );
        }

        public override int GetHashCode()
        {
            return _rootBoardPiece.GetHashCode();
        }

        public static bool operator ==( BoardPiece left, BoardPiece right )
        {
            return left.Equals( right );
        }

        public static bool operator !=( BoardPiece left, BoardPiece right )
        {
            return ! left.Equals( right );
        }

        #endregion

    }
}