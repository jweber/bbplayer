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
        private readonly Board originalBoard;

        public SolutionFinder(Board board)
        {
            this.originalBoard = board;
        }

        public Solution[] FindSolutions(Solution previousSolution)
        {
            var solutions = new ConcurrentBag<Solution>();

            for (int i = 0; i < 64; i++)
            {
                int x = i%8;
                int y = i/8;

                BoardPosition position = originalBoard[y, x];
                solutions.Add(MoveRightSolves(position));
                solutions.Add(MoveDownSolves(position));
                solutions.Add(MoveLeftSolves(position));
                solutions.Add(MoveUpSolves(position));
            };

            var orderedSolutions = solutions
                .Where(m => m.Weight > 1)
                .Select(this.ApplySolutionLocationWeight)
                .Select(s => this.ApplyDistanceFromPreviousSolutionWeight(s, previousSolution))
                .OrderByDescending(s => s.Weight);

            return orderedSolutions.ToArray();
        }

        /// <summary>
        /// Favor solutions near the bottom of the board
        /// </summary>
        /// <param name="solution"></param>
        private Solution ApplySolutionLocationWeight(Solution solution)
        {
            solution.Weight += solution.ArrayPosition2.Y/8m;
            return solution;
        }

        private Solution ApplyDistanceFromPreviousSolutionWeight(Solution solution, Solution previousSolution)
        {
            if (previousSolution == null)
                return solution;

            int xDistance = Math.Abs(solution.ArrayPosition2.X - previousSolution.ArrayPosition2.X);
            int yDistance = solution.ArrayPosition2.Y - previousSolution.ArrayPosition2.Y;

            solution.Weight += xDistance/4m;
            solution.Weight += yDistance/8m;
            return solution;
        }

        private IEnumerable<BoardPosition> GetBoardPieces(IVirtualBoard board, BoardPosition piece, MainWindow.Surround direction)
        {
            Func<BoardPosition, BoardPosition> dir = p =>
            {
                switch (direction)
                {
                    case MainWindow.Surround.Top:
                        return board.TopOf(p);
                    case MainWindow.Surround.Right:
                        return board.RightOf(p);
                    case MainWindow.Surround.Bottom:
                        return board.BottomOf(p);
                    case MainWindow.Surround.Left:
                        return board.LeftOf(p);
                    default:
                        throw new ArgumentOutOfRangeException("direction");
                }
            };

            var borderingPiece = dir(piece);
            if (borderingPiece != null && borderingPiece.Piece.RootBoardPiece == piece.Piece.RootBoardPiece)
            {
                yield return borderingPiece;

                var borderingPiece2 = dir(borderingPiece);
                if (borderingPiece2 != null && borderingPiece2.Piece.RootBoardPiece == piece.Piece.RootBoardPiece)
                    yield return borderingPiece2;
            }
        }

        private Solution MoveRightSolves(BoardPosition pieceToMove, bool solveOnlyThisPiece = false)
        {
            var boardCopy = this.originalBoard.Clone();
            var newPiece = boardCopy[pieceToMove.ArrayPosition.Y, pieceToMove.ArrayPosition.X];

            boardCopy.PerformMove(newPiece, MainWindow.Surround.Right);

            var topPieces = GetBoardPieces(boardCopy, newPiece, MainWindow.Surround.Top).ToList();
            var bottomPieces = GetBoardPieces(boardCopy, newPiece, MainWindow.Surround.Bottom).ToList();
            var rightPieces = GetBoardPieces(boardCopy, newPiece, MainWindow.Surround.Right).ToList();

            var intersectingPieces = topPieces.Concat(bottomPieces).ToList();
            var joiningPieces = rightPieces;

            decimal weight = 0;
            if (intersectingPieces.Count() >= 2)
                weight += intersectingPieces.Sum(m => m.Piece.Weight);

            if (joiningPieces.Count >= 2)
                weight += joiningPieces.Sum(m => m.Piece.Weight);
            
            var solution = new Solution(pieceToMove.ArrayPosition, newPiece.ArrayPosition);
            solution.Weight = weight;

            if (!solveOnlyThisPiece)
            {
                var shiftedPiece = this.originalBoard.RightOf(pieceToMove);
                if (shiftedPiece != null)
                {
                    var leftShiftedPieceSolution = MoveLeftSolves(shiftedPiece, true);
                    solution.Weight += leftShiftedPieceSolution.Weight;
                }
            }
            
            return solution;
        }

        private Solution MoveDownSolves(BoardPosition pieceToMove, bool solveOnlyThisPiece = false)
        {
            var boardCopy = this.originalBoard.Clone();
            var newPiece = boardCopy[pieceToMove.ArrayPosition.Y, pieceToMove.ArrayPosition.X];
            
            boardCopy.PerformMove(newPiece, MainWindow.Surround.Bottom);

            var leftPieces = GetBoardPieces(boardCopy, newPiece, MainWindow.Surround.Left).ToList();
            var bottomPieces = GetBoardPieces(boardCopy, newPiece, MainWindow.Surround.Bottom).ToList();
            var rightPieces = GetBoardPieces(boardCopy, newPiece, MainWindow.Surround.Right).ToList();

            var intersectingPieces = leftPieces.Concat(rightPieces).ToList();
            var joiningPieces = bottomPieces;

            decimal weight = 0;
            if (intersectingPieces.Count() >= 2)
                weight += intersectingPieces.Sum(m => m.Piece.Weight);

            if (joiningPieces.Count >= 2)
                weight += joiningPieces.Sum(m => m.Piece.Weight);
            
            var solution = new Solution(pieceToMove.ArrayPosition, newPiece.ArrayPosition);
            solution.Weight = weight;

            if (!solveOnlyThisPiece)
            {
                var shiftedPiece = this.originalBoard.BottomOf(pieceToMove);
                if (shiftedPiece != null)
                {
                    var shiftedPieceSolution = MoveUpSolves(shiftedPiece, true);
                    solution.Weight += shiftedPieceSolution.Weight;
                }
            }

            return solution;
        }

        private Solution MoveLeftSolves(BoardPosition pieceToMove, bool solveOnlyThisPiece = false)
        {
            var boardCopy = this.originalBoard.Clone();
            var newPiece = boardCopy[pieceToMove.ArrayPosition.Y, pieceToMove.ArrayPosition.X];
            
            boardCopy.PerformMove(newPiece, MainWindow.Surround.Left);

            var topPieces = GetBoardPieces(boardCopy, newPiece, MainWindow.Surround.Top).ToList();
            var bottomPieces = GetBoardPieces(boardCopy, newPiece, MainWindow.Surround.Bottom).ToList();
            var leftPieces = GetBoardPieces(boardCopy, newPiece, MainWindow.Surround.Left).ToList();

            var intersectingPieces = topPieces.Concat(bottomPieces).ToList();
            var joiningPieces = leftPieces;

            decimal weight = 0;
            if (intersectingPieces.Count() >= 2)
                weight += intersectingPieces.Sum(m => m.Piece.Weight);

            if (joiningPieces.Count >= 2)
                weight += joiningPieces.Sum(m => m.Piece.Weight);
            
            var solution = new Solution(pieceToMove.ArrayPosition, newPiece.ArrayPosition);
            solution.Weight = weight;

            if (!solveOnlyThisPiece)
            {
                var shiftedPiece = this.originalBoard.LeftOf(pieceToMove);
                if (shiftedPiece != null)
                {
                    var shiftedPieceSolution = MoveRightSolves(shiftedPiece, true);
                    solution.Weight += shiftedPieceSolution.Weight;
                }
            }

            return solution;
        }
        private Solution MoveUpSolves(BoardPosition pieceToMove, bool solveOnlyThisPiece = false)
        {
            var boardCopy = this.originalBoard.Clone();
            var newPiece = boardCopy[pieceToMove.ArrayPosition.Y, pieceToMove.ArrayPosition.X];

            boardCopy.PerformMove(newPiece, MainWindow.Surround.Top);

            var topPieces = GetBoardPieces(boardCopy, newPiece, MainWindow.Surround.Top).ToList();
            var leftPieces = GetBoardPieces(boardCopy, newPiece, MainWindow.Surround.Left).ToList();
            var rightPieces = GetBoardPieces(boardCopy, newPiece, MainWindow.Surround.Right).ToList();

            var intersectingPieces = leftPieces.Concat(rightPieces).ToList();
            var joiningPieces = topPieces;

            decimal weight = 0;
            if (intersectingPieces.Count() >= 2)
                weight += intersectingPieces.Sum(m => m.Piece.Weight);

            if (joiningPieces.Count >= 2)
                weight += joiningPieces.Sum(m => m.Piece.Weight);
            
            var solution = new Solution(pieceToMove.ArrayPosition, newPiece.ArrayPosition);
            solution.Weight = weight;

            if (!solveOnlyThisPiece)
            {
                var shiftedPiece = this.originalBoard.TopOf(pieceToMove);
                if (shiftedPiece != null)
                {
                    var shiftedPieceSolution = MoveDownSolves(shiftedPiece, true);
                    solution.Weight += shiftedPieceSolution.Weight;
                }
            }

            return solution;
        }
    }
}
