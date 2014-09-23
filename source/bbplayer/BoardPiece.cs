using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
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
        public const int Width = 40;
        public const int Height = 40;

        private readonly Color pieceAverageColor;
        private readonly int[] luminanceHistogram;

        private BoardPiece(RootBoardPiece rootPiece, PieceWeight weight)
            : this(rootPiece, weight, string.Format("{0} {1}", rootPiece.ToString().PascalCaseToWord(), weight))
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
                this.pieceAverageColor = Color.Black;
                this.luminanceHistogram = new int[256];
            }
            else
            {
                this.pieceAverageColor = ColorUtility.GetAveragePieceColor(image, 0, 0);
                this.luminanceHistogram = ColorUtility.GetLuminanceHistogram(image, 0, 0);
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
            if (stream == null)
                return null;

            return new Bitmap(GetImageStream());
        }

        public Stream GetImageStream()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var imageStream = assembly.GetManifestResourceStream("bbplayer.images." + ImageFileName);

            if (imageStream == null)
                return null;

            return imageStream;
        }

        public MatchData MatchAgainst(BoardPieceMetrics boardPieceMetrics)
        {
            double colorDistance = ColorUtility.GetColorDistance(this.pieceAverageColor, boardPieceMetrics.AverageColor);
            double luminanceDistance = ColorUtility.GetLuminanceDistance(this.pieceAverageColor, boardPieceMetrics.AverageLuminance);
            double histogramDistance = ColorUtility.GetHistogramDistance(this.luminanceHistogram, boardPieceMetrics.LuminanceHistogram);

            return new MatchData(colorDistance, luminanceDistance, histogramDistance);
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

        public class BoardPieceMetrics
        {
            public BoardPieceMetrics(Color averageColor, Color averageLuminance, int[] luminanceHistogram)
            {
                AverageColor = averageColor;
                AverageLuminance = averageLuminance;
                LuminanceHistogram = luminanceHistogram;
            }

            public Color AverageColor { get; private set; }
            public Color AverageLuminance { get; private set; }
            public int[] LuminanceHistogram { get; private set; }
        }

        public static BoardPieceMetrics GetBoardPieceMetrics(Bitmap board, int pieceX, int pieceY)
        {
            int topLeftX = pieceX*Width;
            int topLeftY = pieceY*Height;

            var averageColor = ColorUtility.GetAveragePieceColor(board, topLeftX, topLeftY);
            var averageLuminance = ColorUtility.GetAveragePieceLuminance(board, topLeftX, topLeftY);
            var luminanceHistogram = ColorUtility.GetLuminanceHistogram(board, topLeftX, topLeftY);

            return new BoardPieceMetrics(averageColor, averageLuminance, luminanceHistogram);
        }

        public static IList<MatchPair> FindMatches(Bitmap board, int pieceX, int pieceY)
        {
            var pieceMetrics = GetBoardPieceMetrics(board, pieceX, pieceY);

            var matches = new List<MatchPair>();

            matches.Add(new MatchPair(Unknown.MatchAgainst(pieceMetrics), Unknown));

            matches.Add(new MatchPair(HyperCube.MatchAgainst(pieceMetrics), HyperCube));

            matches.Add(new MatchPair(RedSquare.MatchAgainst(pieceMetrics), RedSquare));
            matches.Add(new MatchPair(OrangeDecahedron.MatchAgainst(pieceMetrics), OrangeDecahedron));
            matches.Add(new MatchPair(PurpleTriangle.MatchAgainst(pieceMetrics), PurpleTriangle));
            matches.Add(new MatchPair(GreenSphere.MatchAgainst(pieceMetrics), GreenSphere));
            matches.Add(new MatchPair(BlueDiamond.MatchAgainst(pieceMetrics), BlueDiamond));
            matches.Add(new MatchPair(GrayRock.MatchAgainst(pieceMetrics), GrayRock));
            matches.Add(new MatchPair(YellowSquare.MatchAgainst(pieceMetrics), YellowSquare));

            matches.Add(new MatchPair(RedSquare_Power.MatchAgainst(pieceMetrics), RedSquare_Power));
            matches.Add(new MatchPair(OrangeDecahedron_Power.MatchAgainst(pieceMetrics), OrangeDecahedron_Power));
            matches.Add(new MatchPair(PurpleTriangle_Power.MatchAgainst(pieceMetrics), PurpleTriangle_Power));
            matches.Add(new MatchPair(GreenSphere_Power.MatchAgainst(pieceMetrics), GreenSphere_Power));
            matches.Add(new MatchPair(BlueDiamond_Power.MatchAgainst(pieceMetrics), BlueDiamond_Power));
            matches.Add(new MatchPair(GrayRock_Power.MatchAgainst(pieceMetrics), GrayRock_Power));
            matches.Add(new MatchPair(YellowSquare_Power.MatchAgainst(pieceMetrics), YellowSquare_Power));

            matches.Add(new MatchPair(RedSquare_Multiplier.MatchAgainst(pieceMetrics), RedSquare_Multiplier));
            matches.Add(new MatchPair(OrangeDecahedron_Multiplier.MatchAgainst(pieceMetrics), OrangeDecahedron_Multiplier));
            matches.Add(new MatchPair(PurpleTriangle_Multiplier.MatchAgainst(pieceMetrics), PurpleTriangle_Multiplier));
            matches.Add(new MatchPair(GreenSphere_Multiplier.MatchAgainst(pieceMetrics), GreenSphere_Multiplier));
            matches.Add(new MatchPair(BlueDiamond_Multiplier.MatchAgainst(pieceMetrics), BlueDiamond_Multiplier));
            matches.Add(new MatchPair(GrayRock_Multiplier.MatchAgainst(pieceMetrics), GrayRock_Multiplier));
            matches.Add(new MatchPair(YellowSquare_Multiplier.MatchAgainst(pieceMetrics), YellowSquare_Multiplier));
            
            matches.Add(new MatchPair(RedSquare_Fire.MatchAgainst(pieceMetrics), RedSquare_Fire));
            matches.Add(new MatchPair(OrangeDecahedron_Fire.MatchAgainst(pieceMetrics), OrangeDecahedron_Fire));
            matches.Add(new MatchPair(PurpleTriangle_Fire.MatchAgainst(pieceMetrics), PurpleTriangle_Fire));
            matches.Add(new MatchPair(GreenSphere_Fire.MatchAgainst(pieceMetrics), GreenSphere_Fire));
            matches.Add(new MatchPair(BlueDiamond_Fire.MatchAgainst(pieceMetrics), BlueDiamond_Fire));
            matches.Add(new MatchPair(GrayRock_Fire.MatchAgainst(pieceMetrics), GrayRock_Fire));
            matches.Add(new MatchPair(YellowSquare_Fire.MatchAgainst(pieceMetrics), YellowSquare_Fire));

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
            if (matchPairs.Any(m => m.Weight.ColorDistance < 6))
                return matchPairs.First(m => m.Weight.ColorDistance < 6);
            
            if (matchPairs.Any(m => m.Weight.LuminanceDistance < 6))
                return matchPairs.First(m => m.Weight.LuminanceDistance < 6);

//            if (matchPairs.Any(m => m.Weight.HistogramDistance < 100))
//                return matchPairs.First(m => m.Weight.HistogramDistance < 100);

            return matchPairs
                .OrderBy(m => m.Weight.ColorDistance)
                .ThenBy(m => m.Weight.LuminanceDistance)
                .ThenBy(m => m.Weight.HistogramDistance)
                .First();
        }
    }
}