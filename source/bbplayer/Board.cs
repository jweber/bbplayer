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
            this.RecalculateBoardPieces();
            return this.UpdateBoardFacades();
        }

        private void RecalculateBoardPieces()
        {
            var tasks = new List<Task>();
            object locker = new object();

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    int y1 = y;
                    int x1 = x;
                    var bitmapClone = (Bitmap) this.boardImage.Clone();

                    var task = Task.Factory.StartNew(() =>
                    {
                        var matches = BoardPiece.FindMatches(bitmapClone, x1, y1);                    
                        var closestMatch = matches.GetClosestMatch().BoardPiece;

                        lock (locker)
                        {
                            this.boardPositions[y1, x1].SetPiece(closestMatch, y1, x1);
                        }
                    });

                    tasks.Add(task);
                }
            }

            Task.WaitAll(tasks.ToArray());
        }

        private int UpdateBoardFacades()
        {
            int unknownCount = 0;
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    var positionPiece = this.boardPositions[y, x].Piece;

                    this.boardPositions[y, x].Facade.ToolTip = positionPiece.Name;

                    if (positionPiece.GetImage() != null)
                        this.boardPositions[y, x].Facade.Fill =
                            new ImageBrush(ImageUtility.BitmapToImageSource(positionPiece.GetImage(), ImageFormat.Bmp));


                    if (positionPiece == BoardPiece.Unknown)
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
