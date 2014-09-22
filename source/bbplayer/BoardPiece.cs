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
        private Color _pieceAverageColor;
        private int[] _luminanceHistogram;

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
                this._pieceAverageColor = Color.Black;
                this._luminanceHistogram = new int[256];
            }
            else
            {
                this._pieceAverageColor = ColorUtility.GetAveragePieceColor(image, 0, 0);
                this._luminanceHistogram = ColorUtility.GetLuminanceHistogram(image, 0, 0);
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

        public MatchData MatchAgainst(Bitmap bitmap, int pieceX, int pieceY)
        {
            var averageColor = ColorUtility.GetAveragePieceColor(bitmap, pieceX, pieceY);
            double colorDistance = GetColorDistance(this._pieceAverageColor, averageColor);

            var averageLuminance = ColorUtility.GetAveragePieceLuminance(bitmap, pieceX, pieceY);
            double luminanceDistance = GetLuminanceDistance(this._pieceAverageColor, averageLuminance);

//            int[] histogram = ColorUtility.GetLuminanceHistogram(bitmap, pieceX, pieceY);
//            double histogramDistance = ColorUtility.GetHistogramDistance(this._luminanceHistogram, histogram);

            return new MatchData(colorDistance, luminanceDistance, double.MaxValue);
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

        public static IList<MatchPair> FindMatches(Bitmap board, int pieceX, int pieceY)
        {
            var matches = new List<MatchPair>();

            matches.Add(new MatchPair(Unknown.MatchAgainst(board, pieceX, pieceY), Unknown));

            matches.Add(new MatchPair(HyperCube.MatchAgainst(board, pieceX, pieceY), HyperCube));

            matches.Add(new MatchPair(RedSquare.MatchAgainst(board, pieceX, pieceY), RedSquare));
            matches.Add(new MatchPair(OrangeDecahedron.MatchAgainst(board, pieceX, pieceY), OrangeDecahedron));
            matches.Add(new MatchPair(PurpleTriangle.MatchAgainst(board, pieceX, pieceY), PurpleTriangle));
            matches.Add(new MatchPair(GreenSphere.MatchAgainst(board, pieceX, pieceY), GreenSphere));
            matches.Add(new MatchPair(BlueDiamond.MatchAgainst(board, pieceX, pieceY), BlueDiamond));
            matches.Add(new MatchPair(GrayRock.MatchAgainst(board, pieceX, pieceY), GrayRock));
            matches.Add(new MatchPair(YellowSquare.MatchAgainst(board, pieceX, pieceY), YellowSquare));

            matches.Add(new MatchPair(RedSquare_Power.MatchAgainst(board, pieceX, pieceY), RedSquare_Power));
            matches.Add(new MatchPair(OrangeDecahedron_Power.MatchAgainst(board, pieceX, pieceY), OrangeDecahedron_Power));
            matches.Add(new MatchPair(PurpleTriangle_Power.MatchAgainst(board, pieceX, pieceY), PurpleTriangle_Power));
            matches.Add(new MatchPair(GreenSphere_Power.MatchAgainst(board, pieceX, pieceY), GreenSphere_Power));
            matches.Add(new MatchPair(BlueDiamond_Power.MatchAgainst(board, pieceX, pieceY), BlueDiamond_Power));
            matches.Add(new MatchPair(GrayRock_Power.MatchAgainst(board, pieceX, pieceY), GrayRock_Power));
            matches.Add(new MatchPair(YellowSquare_Power.MatchAgainst(board, pieceX, pieceY), YellowSquare_Power));

            matches.Add(new MatchPair(RedSquare_Multiplier.MatchAgainst(board, pieceX, pieceY), RedSquare_Multiplier));
            matches.Add(new MatchPair(OrangeDecahedron_Multiplier.MatchAgainst(board, pieceX, pieceY), OrangeDecahedron_Multiplier));
            matches.Add(new MatchPair(PurpleTriangle_Multiplier.MatchAgainst(board, pieceX, pieceY), PurpleTriangle_Multiplier));
            matches.Add(new MatchPair(GreenSphere_Multiplier.MatchAgainst(board, pieceX, pieceY), GreenSphere_Multiplier));
            matches.Add(new MatchPair(BlueDiamond_Multiplier.MatchAgainst(board, pieceX, pieceY), BlueDiamond_Multiplier));
            matches.Add(new MatchPair(GrayRock_Multiplier.MatchAgainst(board, pieceX, pieceY), GrayRock_Multiplier));
            matches.Add(new MatchPair(YellowSquare_Multiplier.MatchAgainst(board, pieceX, pieceY), YellowSquare_Multiplier));
            
            matches.Add(new MatchPair(RedSquare_Fire.MatchAgainst(board, pieceX, pieceY), RedSquare_Fire));
            matches.Add(new MatchPair(OrangeDecahedron_Fire.MatchAgainst(board, pieceX, pieceY), OrangeDecahedron_Fire));
            matches.Add(new MatchPair(PurpleTriangle_Fire.MatchAgainst(board, pieceX, pieceY), PurpleTriangle_Fire));
            matches.Add(new MatchPair(GreenSphere_Fire.MatchAgainst(board, pieceX, pieceY), GreenSphere_Fire));
            matches.Add(new MatchPair(BlueDiamond_Fire.MatchAgainst(board, pieceX, pieceY), BlueDiamond_Fire));
            matches.Add(new MatchPair(GrayRock_Fire.MatchAgainst(board, pieceX, pieceY), GrayRock_Fire));
            matches.Add(new MatchPair(YellowSquare_Fire.MatchAgainst(board, pieceX, pieceY), YellowSquare_Fire));

            var orderedMatches = matches
                .OrderBy(m => m.Weight.ColorDistance)
                .ThenBy(m => m.Weight.LuminanceDistance)
                .ThenBy(m => m.Weight.HistogramDistance)
                .ToList();

            return orderedMatches;
        }

        public class MatchPair
        {
            public MatchPair( MatchData weight, BoardPiece piece )
            {
                Weight = weight;
                BoardPiece = piece;
            }

            public MatchData Weight { get; set; }
            public BoardPiece BoardPiece { get; set; }

            public override string ToString()
            {
                return Weight + ": " + BoardPiece.ToString();
            }
        }

        public class MatchData
        {
            public MatchData(double colorDistance, double luminanceDistance, double histogramDistance)
            {
                this.ColorDistance = colorDistance;
                this.LuminanceDistance = luminanceDistance;
                this.HistogramDistance = histogramDistance;
            }

            public double ColorDistance { get; private set; }
            public double LuminanceDistance { get; private set; }
            public double HistogramDistance { get; private set; }
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

            if (matchPairs.Any(m => m.Weight.ColorDistance < 6))
                return matchPairs.First(m => m.Weight.ColorDistance < 6);
            
            if (matchPairs.Any(m => m.Weight.LuminanceDistance < 6))
                return matchPairs.First(m => m.Weight.LuminanceDistance < 6);            

            return matchPairs
                .OrderBy(m => m.Weight.ColorDistance)
                .ThenBy(m => m.Weight.LuminanceDistance)
                .ThenBy(m => m.Weight.HistogramDistance)
                .First();
        }
    }
}