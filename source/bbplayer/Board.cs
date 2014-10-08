using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using bbplayer.solutions;
using Brushes = System.Drawing.Brushes;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace bbplayer
{
    interface IVirtualBoard
    {
        BoardPosition this[int y, int x] { get; set; }
        BoardPosition TopOf(BoardPosition position);
        BoardPosition RightOf(BoardPosition position);
        BoardPosition BottomOf(BoardPosition position);
        BoardPosition LeftOf(BoardPosition position);
        Solution[] FindSolutions(Solution previousSolution);
        void PerformMove(BoardPosition piece, MainWindow.Surround move);
       
        IVirtualBoard Clone();
    }

    class Board : IVirtualBoard
    {
        private readonly BoardPosition[,] boardPositions;
        private Bitmap boardImage;

        public Board()
        {
            boardPositions = new BoardPosition[8,8];
        }

        public Point ScreenTopLeft { get; set; }

        public Bitmap BoardImage { get { return this.boardImage; } }

        public int UpdateBoardImage(Bitmap bitmap)
        {
            this.boardImage = bitmap;
            this.RecalculateBoardPieces();
            return this.UpdateBoardFacades();
        }

        public void PerformMove(BoardPosition piece, MainWindow.Surround move)
        {
            BoardPosition swappingPiece;
            switch (move)
            {
                case MainWindow.Surround.Top:
                    swappingPiece = this.TopOf(piece);
                    break;
                case MainWindow.Surround.Right:
                    swappingPiece = this.RightOf(piece);
                    break;
                case MainWindow.Surround.Bottom:
                    swappingPiece = this.BottomOf(piece);
                    break;
                case MainWindow.Surround.Left:
                    swappingPiece = this.LeftOf(piece);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("move");
            }

            if (swappingPiece == null)
                return;

            var movePiecePosition = piece.ArrayPosition;
            var swappingPosition = swappingPiece.ArrayPosition;

            swappingPiece.ArrayPosition = movePiecePosition;
            piece.ArrayPosition = swappingPosition;

            this[swappingPiece.ArrayPosition.Y, swappingPiece.ArrayPosition.X] = swappingPiece;
            this[piece.ArrayPosition.Y, piece.ArrayPosition.X] = piece;
        }

        public IVirtualBoard Clone()
        {
            var newBoard = new Board();
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    var newBoardPosition = new BoardPosition();
                    newBoardPosition.SetPiece(this[y, x].Piece, y, x);
                    newBoard[y, x] = newBoardPosition;
                }
            }

            return newBoard;
        }

        private void RecalculateBoardPieces()
        {
            object bitmapLocker = new object();
            object locker = new object();

            Parallel.For(0, 64, i =>
            {
                int x = i%8;
                int y = i/8;

                Bitmap bitmap;
                lock (bitmapLocker)
                {
                    bitmap = this.boardImage.Clone() as Bitmap;
                }
               
                var matches = BoardPiece.FindMatches(bitmap, x, y);                    
                var closestMatch = matches.GetClosestMatch().BoardPiece;

                lock (locker)
                {
                    this.boardPositions[y, x].SetPiece(closestMatch, y, x);
                }
            });
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

        public Solution[] FindSolutions(Solution previousSolution)
        {
            var solutionFinder = new SolutionFinder(this);
            var solutions = solutionFinder.FindSolutions(previousSolution);
            if (solutions == null)
                return new Solution[0];

            return solutions.ToArray();
        }
    }
}
