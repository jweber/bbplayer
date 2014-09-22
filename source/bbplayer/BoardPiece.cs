using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using iLandMan.Utility;

namespace bbplayer
{
    enum RootBoardPiece
    {
        Unknown,
        RedSquare,
        OrangeDecahedron,
        PurpleTriangle,
        GreenSphere,
        BlueDiamond,
        GrayRock,
        YellowSquare,
        Any
    }

    enum PieceWeight
    {
        None = 0,
        Normal = 1,
        Power = 3,
        Multiplier = 5,
        Fire = 8,
        HyperCube = 10
    }

    class BoardPiece
    {
        private ColorPoints _pieceColorPoints;
        private Color _pieceAverageColor;

        private BoardPiece( RootBoardPiece rootPiece, PieceWeight weight )
            : this( rootPiece, weight, string.Format( "{0} {1}", rootPiece.ToString().PascalCaseToWord(), weight ) )
        {}

        private BoardPiece(RootBoardPiece rootPiece, PieceWeight weight, string name)
        {
            this.RootBoardPiece = rootPiece;
            this.PieceWeight = weight;
            this.Name = name;

            string pieceName = RootBoardPiece.ToString().PascalCaseToWord().Replace(' ', '-');

            if (name == "Hypercube")
            {
                this.ImageFileName = "hypercube.bmp";
            }
            else
            {
                this.ImageFileName = string.Format("{0}_{1}.bmp", PieceWeight, pieceName).ToLower();
            }

            var image = GetImage();
            if (image == null)
            {
                _pieceColorPoints = new ColorPoints(System.Drawing.Color.Black);
            }
            else
            {
                this._pieceAverageColor = ColorUtility.GetAveragePieceColor(image, 0, 0);
                //_pieceColorPoints = new ColorPoints(image);
            }
        }

        public string Name { get; private set;  }
        public string ImageFileName { get; private set; }
        public RootBoardPiece RootBoardPiece { get; private set; }
        public PieceWeight PieceWeight { get; private set; }
        public int Weight { get { return (int) PieceWeight; } }

        public Bitmap GetImage()
        {
            var stream = GetImageStream();
            if ( stream == null )
                return null;

            return new Bitmap( GetImageStream() );
        }

        public Stream GetImageStream()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var imageStream = assembly.GetManifestResourceStream( "bbplayer.images." + ImageFileName );

            if ( imageStream == null )
                return null;

            return imageStream;
        }

        public Tuple<double, double> MatchAgainst(Color averageColor)
        {
            double colorDistance = GetColorDistance(averageColor, this._pieceAverageColor);
            double luminanceDistance = GetLuminanceDistance(averageColor, this._pieceAverageColor);
            return new Tuple<double, double>(colorDistance, luminanceDistance);
        }

        public Tuple<double, double> MatchAgainst( ColorPoints colors )
        {
            var firstAverage = GetAverageColor( colors );
            var secondAverage = GetAverageColor( _pieceColorPoints );

            double centerDistance = GetLuminanceDistance( colors.Center, _pieceColorPoints.Center );
            double averageDistance = GetLuminanceDistance( firstAverage, secondAverage );

            return new Tuple<double, double>( centerDistance, averageDistance );
        }

        public static Color GetAverageColor( ColorPoints colorPoints )
        {
            return GetAverageColor( new[] 
            {
                colorPoints.Center, colorPoints.TopMost, colorPoints.TopMiddle, colorPoints.BottomMost, colorPoints.BottomMiddle,
                colorPoints.LeftMost, colorPoints.LeftMost
            } );            
        }

        public static double GetColorDifference(Color first, Color second)
        {
            return (Math.Max(first.R, second.R) - Math.Min(first.R, second.R))
                   + (Math.Max(first.G, second.G) - Math.Min(first.G, second.G))
                   + (Math.Max(first.B, second.B) - Math.Min(first.B, second.B));
        }

        public static double GetColorDistance( Color first, Color second )
        {
            double distance = Math.Sqrt( Math.Pow( first.R - second.R, 2 )
                                         + Math.Pow( first.G - second.G, 2 )
                                         + Math.Pow( first.B - second.B, 2 ) );

            return distance;
        }


        public static double GetLuminanceDistance( Color first, Color second )
        {
            double distance = Math.Sqrt( Math.Pow( first.GetHue() - second.GetHue(), 2 )
                                         + Math.Pow( first.GetSaturation() - second.GetSaturation(), 2 )
                                         + Math.Pow( first.GetBrightness() - second.GetBrightness(), 2 ) );

            return distance;
        }

        public static Color GetAverageColor( ICollection<Color> colors )
        {
            //Used for tally
            int r = 0;
            int g = 0;
            int b = 0;

            foreach ( Color color in colors )
            {
                r += color.R;
                g += color.G;
                b += color.B;
            }

            //Calculate average
            r /= colors.Count;
            g /= colors.Count;
            b /= colors.Count;

            return Color.FromArgb( r, g, b );
        }

        #region Factories

        public static BoardPiece Unknown = new BoardPiece(RootBoardPiece.Unknown, PieceWeight.None, "Unknown");

        public static BoardPiece RedSquare = new BoardPiece(RootBoardPiece.RedSquare, PieceWeight.Normal);
        public static BoardPiece OrangeDecahedron = new BoardPiece(RootBoardPiece.OrangeDecahedron, PieceWeight.Normal);
        public static BoardPiece PurpleTriangle = new BoardPiece(RootBoardPiece.PurpleTriangle, PieceWeight.Normal);
        public static BoardPiece GreenSphere = new BoardPiece(RootBoardPiece.GreenSphere, PieceWeight.Normal);
        public static BoardPiece BlueDiamond = new BoardPiece(RootBoardPiece.BlueDiamond, PieceWeight.Normal);
        public static BoardPiece GrayRock = new BoardPiece(RootBoardPiece.GrayRock, PieceWeight.Normal);
        public static BoardPiece YellowSquare = new BoardPiece(RootBoardPiece.YellowSquare, PieceWeight.Normal);

        public static BoardPiece RedSquare_Power = new BoardPiece(RootBoardPiece.RedSquare, PieceWeight.Power);
        public static BoardPiece OrangeDecahedron_Power = new BoardPiece(RootBoardPiece.OrangeDecahedron, PieceWeight.Power);
        public static BoardPiece PurpleTriangle_Power = new BoardPiece(RootBoardPiece.PurpleTriangle, PieceWeight.Power);
        public static BoardPiece GreenSphere_Power = new BoardPiece(RootBoardPiece.GreenSphere, PieceWeight.Power);
        public static BoardPiece BlueDiamond_Power = new BoardPiece(RootBoardPiece.BlueDiamond, PieceWeight.Power);
        public static BoardPiece GrayRock_Power = new BoardPiece(RootBoardPiece.GrayRock, PieceWeight.Power);
        public static BoardPiece YellowSquare_Power = new BoardPiece(RootBoardPiece.YellowSquare, PieceWeight.Power);

        public static BoardPiece RedSquare_Multiplier = new BoardPiece(RootBoardPiece.RedSquare, PieceWeight.Multiplier);
        public static BoardPiece OrangeDecahedron_Multiplier = new BoardPiece(RootBoardPiece.OrangeDecahedron, PieceWeight.Multiplier);
        public static BoardPiece PurpleTriangle_Multiplier = new BoardPiece(RootBoardPiece.PurpleTriangle, PieceWeight.Multiplier);
        public static BoardPiece GreenSphere_Multiplier = new BoardPiece(RootBoardPiece.GreenSphere, PieceWeight.Multiplier);
        public static BoardPiece BlueDiamond_Multiplier = new BoardPiece(RootBoardPiece.BlueDiamond, PieceWeight.Multiplier);
        public static BoardPiece GrayRock_Multiplier = new BoardPiece(RootBoardPiece.GrayRock, PieceWeight.Multiplier);
        public static BoardPiece YellowSquare_Multiplier = new BoardPiece(RootBoardPiece.YellowSquare,PieceWeight.Multiplier);

        public static BoardPiece RedSquare_Fire = new BoardPiece(RootBoardPiece.RedSquare, PieceWeight.Fire);
        public static BoardPiece OrangeDecahedron_Fire = new BoardPiece(RootBoardPiece.OrangeDecahedron, PieceWeight.Fire);
        public static BoardPiece PurpleTriangle_Fire = new BoardPiece(RootBoardPiece.PurpleTriangle, PieceWeight.Fire);
        public static BoardPiece GreenSphere_Fire = new BoardPiece(RootBoardPiece.GreenSphere, PieceWeight.Fire);
        public static BoardPiece BlueDiamond_Fire = new BoardPiece(RootBoardPiece.BlueDiamond, PieceWeight.Fire);
        public static BoardPiece GrayRock_Fire = new BoardPiece(RootBoardPiece.GrayRock, PieceWeight.Fire);
        public static BoardPiece YellowSquare_Fire = new BoardPiece(RootBoardPiece.YellowSquare,PieceWeight.Fire);

        public static BoardPiece HyperCube = new BoardPiece(RootBoardPiece.Any, PieceWeight.HyperCube, "Hypercube");

        #endregion

        public static IList<MatchPair> FindMatches(Color averageColor)
        {
            var matches = new List<MatchPair>();

            matches.Add(new MatchPair(Unknown.MatchAgainst(averageColor), Unknown));

            matches.Add(new MatchPair(HyperCube.MatchAgainst(averageColor), HyperCube));

            matches.Add(new MatchPair(RedSquare.MatchAgainst(averageColor), RedSquare));
            matches.Add(new MatchPair(OrangeDecahedron.MatchAgainst(averageColor), OrangeDecahedron));
            matches.Add(new MatchPair(PurpleTriangle.MatchAgainst(averageColor), PurpleTriangle));
            matches.Add(new MatchPair(GreenSphere.MatchAgainst(averageColor), GreenSphere));
            matches.Add(new MatchPair(BlueDiamond.MatchAgainst(averageColor), BlueDiamond));
            matches.Add(new MatchPair(GrayRock.MatchAgainst(averageColor), GrayRock));
            matches.Add(new MatchPair(YellowSquare.MatchAgainst(averageColor), YellowSquare));

            matches.Add(new MatchPair(RedSquare_Power.MatchAgainst(averageColor), RedSquare_Power));
            matches.Add(new MatchPair(OrangeDecahedron_Power.MatchAgainst(averageColor), OrangeDecahedron_Power));
            matches.Add(new MatchPair(PurpleTriangle_Power.MatchAgainst(averageColor), PurpleTriangle_Power));
            matches.Add(new MatchPair(GreenSphere_Power.MatchAgainst(averageColor), GreenSphere_Power));
            matches.Add(new MatchPair(BlueDiamond_Power.MatchAgainst(averageColor), BlueDiamond_Power));
            matches.Add(new MatchPair(GrayRock_Power.MatchAgainst(averageColor), GrayRock_Power));
            matches.Add(new MatchPair(YellowSquare_Power.MatchAgainst(averageColor), YellowSquare_Power));

            matches.Add(new MatchPair(RedSquare_Multiplier.MatchAgainst(averageColor), RedSquare_Multiplier));
            matches.Add(new MatchPair(OrangeDecahedron_Multiplier.MatchAgainst(averageColor), OrangeDecahedron_Multiplier));
            matches.Add(new MatchPair(PurpleTriangle_Multiplier.MatchAgainst(averageColor), PurpleTriangle_Multiplier));
            matches.Add(new MatchPair(GreenSphere_Multiplier.MatchAgainst(averageColor), GreenSphere_Multiplier));
            matches.Add(new MatchPair(BlueDiamond_Multiplier.MatchAgainst(averageColor), BlueDiamond_Multiplier));
            matches.Add(new MatchPair(GrayRock_Multiplier.MatchAgainst(averageColor), GrayRock_Multiplier));
            matches.Add(new MatchPair(YellowSquare_Multiplier.MatchAgainst(averageColor), YellowSquare_Multiplier));
            
            matches.Add(new MatchPair(RedSquare_Fire.MatchAgainst(averageColor), RedSquare_Fire));
            matches.Add(new MatchPair(OrangeDecahedron_Fire.MatchAgainst(averageColor), OrangeDecahedron_Fire));
            matches.Add(new MatchPair(PurpleTriangle_Fire.MatchAgainst(averageColor), PurpleTriangle_Fire));
            matches.Add(new MatchPair(GreenSphere_Fire.MatchAgainst(averageColor), GreenSphere_Fire));
            matches.Add(new MatchPair(BlueDiamond_Fire.MatchAgainst(averageColor), BlueDiamond_Fire));
            matches.Add(new MatchPair(GrayRock_Fire.MatchAgainst(averageColor), GrayRock_Fire));
            matches.Add(new MatchPair(YellowSquare_Fire.MatchAgainst(averageColor), YellowSquare_Fire));

            var orderedMatches = matches
                .OrderBy(m => m.Weight.Second)
                .ThenBy(m => m.Weight.First)
                .ToList();

            return orderedMatches;
        }

        public static BoardPiece FindMatch(ColorPoints colorPoints)
        {
            var matches = new List<MatchPair>();

            matches.Add(new MatchPair(Unknown.MatchAgainst(colorPoints), Unknown));

            matches.Add(new MatchPair(HyperCube.MatchAgainst(colorPoints), HyperCube));

            matches.Add(new MatchPair(RedSquare.MatchAgainst(colorPoints), RedSquare));
            matches.Add(new MatchPair(OrangeDecahedron.MatchAgainst(colorPoints), OrangeDecahedron));
            matches.Add(new MatchPair(PurpleTriangle.MatchAgainst(colorPoints), PurpleTriangle));
            matches.Add(new MatchPair(GreenSphere.MatchAgainst(colorPoints), GreenSphere));
            matches.Add(new MatchPair(BlueDiamond.MatchAgainst(colorPoints), BlueDiamond));
            matches.Add(new MatchPair(GrayRock.MatchAgainst(colorPoints), GrayRock));
            matches.Add(new MatchPair(YellowSquare.MatchAgainst(colorPoints), YellowSquare));

            matches.Add(new MatchPair(RedSquare_Power.MatchAgainst(colorPoints), RedSquare_Power));
            matches.Add(new MatchPair(OrangeDecahedron_Power.MatchAgainst(colorPoints), OrangeDecahedron_Power));
            matches.Add(new MatchPair(PurpleTriangle_Power.MatchAgainst(colorPoints), PurpleTriangle_Power));
            matches.Add(new MatchPair(GreenSphere_Power.MatchAgainst(colorPoints), GreenSphere_Power));
            matches.Add(new MatchPair(BlueDiamond_Power.MatchAgainst(colorPoints), BlueDiamond_Power));
            matches.Add(new MatchPair(GrayRock_Power.MatchAgainst(colorPoints), GrayRock_Power));
            matches.Add(new MatchPair(YellowSquare_Power.MatchAgainst(colorPoints), YellowSquare_Power));

            matches.Add(new MatchPair(RedSquare_Fire.MatchAgainst(colorPoints), RedSquare_Fire));
            matches.Add(new MatchPair(OrangeDecahedron_Fire.MatchAgainst(colorPoints), OrangeDecahedron_Fire));
            matches.Add(new MatchPair(PurpleTriangle_Fire.MatchAgainst(colorPoints), PurpleTriangle_Fire));
            matches.Add(new MatchPair(GreenSphere_Fire.MatchAgainst(colorPoints), GreenSphere_Fire));
            matches.Add(new MatchPair(BlueDiamond_Fire.MatchAgainst(colorPoints), BlueDiamond_Fire));
            matches.Add(new MatchPair(GrayRock_Fire.MatchAgainst(colorPoints), GrayRock_Fire));
            matches.Add(new MatchPair(YellowSquare_Fire.MatchAgainst(colorPoints), YellowSquare_Fire));

            var orderedMatches = matches.OrderBy(m => m.Weight.First).ThenBy(m => m.Weight.Second).ToList();
            var piece = orderedMatches.First();

            return piece.BoardPiece;
        }

        public class MatchPair
        {
            public MatchPair( Tuple<double, double> weight, BoardPiece piece )
            {
                Weight = weight;
                BoardPiece = piece;
            }

            public Tuple<double, double> Weight { get; set; }
            public BoardPiece BoardPiece { get; set; }

            public override string ToString()
            {
                return Weight + ": " + BoardPiece.ToString();
            }
        }

        public override string ToString()
        {
            return Name;
        }

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
            return Equals( other.RootBoardPiece, RootBoardPiece );
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
            return RootBoardPiece.GetHashCode();
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

    internal static class MatchPairExtensions
    {
        public static BoardPiece.MatchPair GetClosestMatch(this ICollection<BoardPiece.MatchPair> matchPairs)
        {
            //return matchPairs.OrderBy(m => m.Weight.First + m.Weight.Second).First();

            if (matchPairs.Any(m => m.Weight.First < 6))
                return matchPairs.First(m => m.Weight.First < 6);
            
            if (matchPairs.Any(m => m.Weight.Second < 6))
                return matchPairs.First(m => m.Weight.Second < 6);

            if (matchPairs.Any(m => m.Weight.First + m.Weight.Second < 15))
                return matchPairs.First(m => m.Weight.First + m.Weight.Second < 15);

            return matchPairs
                .OrderBy(m => m.Weight.First)
                .ThenBy(m => m.Weight.Second)
                .First();
        }
    }
}