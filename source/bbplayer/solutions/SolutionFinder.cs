using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Parallel = System.Threading.Tasks.Parallel;

namespace bbplayer.solutions
{
    class SolutionFinder : ISolutionFinder
    {
        private readonly Board board;

        public SolutionFinder(Board board)
        {
            this.board = board;
        }

        public Solution[] FindSolutions()
        {
            var solutions = new ConcurrentBag<Solution>();

            for (int i = 0; i < 64; i++)
            {
                int x = i%8;
                int y = i/8;

                BoardPosition position = board[y, x];
                solutions.Add(MoveRightSolves(position));
                solutions.Add(MoveDownSolves(position));
                solutions.Add(MoveLeftSolves(position));
                solutions.Add(MoveUpSolves(position));
            };

            var orderedSolutions = solutions.OrderByDescending(s => s.Weight);

            return orderedSolutions.ToArray();
        }

        private IEnumerable<BoardPosition> GetBoardPieces(BoardPosition piece, MainWindow.Surround direction)
        {
            Func<BoardPosition, BoardPosition> dir = p =>
            {
                switch (direction)
                {
                    case MainWindow.Surround.Top:
                        return this.board.TopOf(p);
                    case MainWindow.Surround.Right:
                        return this.board.RightOf(p);
                    case MainWindow.Surround.Bottom:
                        return this.board.BottomOf(p);
                    case MainWindow.Surround.Left:
                        return this.board.LeftOf(p);
                    default:
                        throw new ArgumentOutOfRangeException("direction");
                }
            };

            var borderingPiece = dir(piece);
            if (borderingPiece != null && borderingPiece.Piece == piece.Piece)
            {
                yield return borderingPiece;

                var borderingPiece2 = dir(borderingPiece);
                if (borderingPiece2 != null && borderingPiece2.Piece == piece.Piece)
                    yield return borderingPiece2;
            }
        }

        private Solution MoveRightSolves(BoardPosition pieceToMove, bool solveOnlyThisPiece = false)
        {
            
            var topPieces = GetBoardPieces(pieceToMove, MainWindow.Surround.Top).ToList();
            var bottomPieces = GetBoardPieces(pieceToMove, MainWindow.Surround.Bottom).ToList();
            var rightPieces = GetBoardPieces(pieceToMove, MainWindow.Surround.Right).ToList();

            var alignedPieces = topPieces.Concat(bottomPieces).Concat(rightPieces);

            var newLocation = new Point(pieceToMove.ArrayPosition.X + 1, pieceToMove.ArrayPosition.Y);
            
            var solution = new Solution(pieceToMove.ArrayPosition, newLocation);
            solution.Weight = alignedPieces.Sum(m => m.Piece.Weight);

//            if (!solveOnlyThisPiece)
//            {
//                var shiftedPiece = this.board.RightOf(pieceToMove);
//                if (shiftedPiece != null)
//                {
//                    var leftShiftedPieceSolution = MoveLeftSolves(shiftedPiece, true);
//                    solution.Weight += leftShiftedPieceSolution.Weight;
//                }
//            }
            
            return solution;
        }

        private Solution MoveDownSolves(BoardPosition pieceToMove, bool solveOnlyThisPiece = false)
        {
            var leftPieces = GetBoardPieces(pieceToMove, MainWindow.Surround.Left).ToList();
            var bottomPieces = GetBoardPieces(pieceToMove, MainWindow.Surround.Bottom).ToList();
            var rightPieces = GetBoardPieces(pieceToMove, MainWindow.Surround.Right).ToList();

            var alignedPieces = leftPieces.Concat(bottomPieces).Concat(rightPieces);

            var newLocation = new Point(pieceToMove.ArrayPosition.X + 1, pieceToMove.ArrayPosition.Y);
            
            var solution = new Solution(pieceToMove.ArrayPosition, newLocation);
            solution.Weight = alignedPieces.Sum(m => m.Piece.Weight);

//            if (!solveOnlyThisPiece)
//            {
//                var shiftedPiece = this.board.BottomOf(pieceToMove);
//                if (shiftedPiece != null)
//                {
//                    var shiftedPieceSolution = MoveUpSolves(shiftedPiece, true);
//                    solution.Weight += shiftedPieceSolution.Weight;
//                }
//            }

            return solution;
        }

        private Solution MoveLeftSolves(BoardPosition pieceToMove, bool solveOnlyThisPiece = false)
        {
            var topPieces = GetBoardPieces(pieceToMove, MainWindow.Surround.Top).ToList();
            var bottomPieces = GetBoardPieces(pieceToMove, MainWindow.Surround.Bottom).ToList();
            var leftPieces = GetBoardPieces(pieceToMove, MainWindow.Surround.Left).ToList();

            var alignedPieces = topPieces.Concat(bottomPieces).Concat(leftPieces);

            var newLocation = new Point(pieceToMove.ArrayPosition.X + 1, pieceToMove.ArrayPosition.Y);
            
            var solution = new Solution(pieceToMove.ArrayPosition, newLocation);
            solution.Weight = alignedPieces.Sum(m => m.Piece.Weight);

//            if (!solveOnlyThisPiece)
//            {
//                var shiftedPiece = this.board.LeftOf(pieceToMove);
//                if (shiftedPiece != null)
//                {
//                    var shiftedPieceSolution = MoveRightSolves(shiftedPiece, true);
//                    solution.Weight += shiftedPieceSolution.Weight;
//                }
//            }

            return solution;
        }
        private Solution MoveUpSolves(BoardPosition pieceToMove, bool solveOnlyThisPiece = false)
        {
            var topPieces = GetBoardPieces(pieceToMove, MainWindow.Surround.Top).ToList();
            var leftPieces = GetBoardPieces(pieceToMove, MainWindow.Surround.Left).ToList();
            var rightPieces = GetBoardPieces(pieceToMove, MainWindow.Surround.Right).ToList();

            var alignedPieces = topPieces.Concat(leftPieces).Concat(rightPieces);

            var newLocation = new Point(pieceToMove.ArrayPosition.X + 1, pieceToMove.ArrayPosition.Y);
            
            var solution = new Solution(pieceToMove.ArrayPosition, newLocation);
            solution.Weight = alignedPieces.Sum(m => m.Piece.Weight);

//            if (!solveOnlyThisPiece)
//            {
//                var shiftedPiece = this.board.TopOf(pieceToMove);
//                if (shiftedPiece != null)
//                {
//                    var shiftedPieceSolution = MoveDownSolves(shiftedPiece, true);
//                    solution.Weight += shiftedPieceSolution.Weight;
//                }
//            }

            return solution;
        }
    }
}
