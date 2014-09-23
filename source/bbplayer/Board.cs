using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Brushes = System.Drawing.Brushes;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace bbplayer
{
    class Board
    {
        private readonly BoardPosition[,] boardPositions;
        private Bitmap boardImage;

        public Board()
        {
            boardPositions = new BoardPosition[8,8];
        }

        public Bitmap BoardImage { get { return this.boardImage; } }

        public int UpdateBoardImage(Bitmap bitmap)
        {
            this.boardImage = bitmap;
            return this.RecalculateBoardPieces();
        }

        private int RecalculateBoardPieces()
        {
            int unknownCount = 0;

            var tasks = new List<Task>();

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    int topLeftX = BoardPiece.Width*x;
                    int topLeftY = BoardPiece.Height*y;

                    var matches = BoardPiece.FindMatches(this.boardImage, x, y);
                    
                    var closestMatch = matches.GetClosestMatch().BoardPiece;

                    //_board[y, x].Facade.Fill = new SolidColorBrush(ConvertToMediaColor(averageColor));
                    this.boardPositions[y, x].Facade.ToolTip = closestMatch.Name;

                    if (closestMatch.GetImage() != null)
                        this.boardPositions[y, x].Facade.Fill = new ImageBrush(ImageUtility.BitmapToImageSource(closestMatch.GetImage(), ImageFormat.Bmp));
                    
                    
                    if (closestMatch == BoardPiece.Unknown)
                    {
                        unknownCount++;

                        this.boardPositions[y, x].Facade.Stroke = System.Windows.Media.Brushes.Red;
                        this.boardPositions[y, x].Facade.StrokeThickness = 3;
                    }
                    else
                    {
                        this.boardPositions[y, x].Facade.Stroke = System.Windows.Media.Brushes.Black;
                        this.boardPositions[y, x].Facade.StrokeThickness = 1;
                    }

                    this.boardPositions[y, x].SetPiece(closestMatch, new Point(topLeftX, topLeftY), y, x);
                }
            }

            return unknownCount;
        }

        public void SetFacade(Rectangle facade, int x, int y)
        {
            boardPositions[y, x].Facade = facade;
        }

        public BoardPosition this[int y, int x]
        {
            get { return boardPositions[y, x]; }
            set { boardPositions[y, x] = value; }
        }

        public BoardPosition TopOf(BoardPosition position)
        {
            if (position == null || position.ArrayPosition.Y == 0)
                return null;

            return this[position.ArrayPosition.Y - 1, position.ArrayPosition.X];
        }

        public BoardPosition RightOf(BoardPosition position)
        {
            if (position == null || position.ArrayPosition.X == 7)
                return null;

            return this[position.ArrayPosition.Y, position.ArrayPosition.X + 1];
        }

        public BoardPosition BottomOf(BoardPosition position)
        {
            if (position == null || position.ArrayPosition.Y == 7)
                return null;

            return this[position.ArrayPosition.Y + 1, position.ArrayPosition.X];
        }

        public BoardPosition LeftOf(BoardPosition position)
        {
            if (position == null || position.ArrayPosition.X == 0)
                return null;

            return this[position.ArrayPosition.Y, position.ArrayPosition.X - 1];
        }

        public Solution[] FindSolutions()
        {
            var solutionFinder = new NaiveBestSolutionFinder(this);
            var solutions = solutionFinder.FindSolutions();

            return solutions.ToArray();
        }
    }
}
