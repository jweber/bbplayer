using System.Collections.Concurrent;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace bbplayer.solutions
{
   class SolutionPack
    {
       public SolutionPack(Point initialPosition, Point moveToPosition)
       {
           Solution1 = new Solution(initialPosition, moveToPosition);
           Solution2 = new Solution(initialPosition, moveToPosition);
           Solution3 = new Solution(initialPosition, moveToPosition);
           Solution4 = new Solution(initialPosition, moveToPosition);
           Solution5 = new Solution(initialPosition, moveToPosition);
           Solution6 = new Solution(initialPosition, moveToPosition);
           Solution7 = new Solution(initialPosition, moveToPosition);
       }

        public Solution Solution1 { get; set; }
        public Solution Solution2 { get; set; }
        public Solution Solution3 { get; set; }
        public Solution Solution4 { get; set; }

        public Solution Solution5 { get; set; }
        public Solution Solution6 { get; set; }
        public Solution Solution7 { get; set; }

        public void AddSolutions( ConcurrentBag<Solution> solutions )
        {
            solutions.Add(Solution1);
            solutions.Add(Solution2);
            solutions.Add(Solution3);
            solutions.Add(Solution4);
            solutions.Add(Solution5);
            solutions.Add(Solution6);
            solutions.Add(Solution7);
        }
    }

    class NaiveBestSolutionFinder : ISolutionFinder
    {
        private readonly Board board;

        public NaiveBestSolutionFinder(Board board)
        {
            this.board = board;
        }

        public Solution[] FindSolutions(Solution previousSolution)
        {
            var solutions = new ConcurrentBag<Solution>();

            Parallel.For(0, 64, i =>
            {
                int x = i%8;
                int y = i/8;

                var position = board[y, x];
                MoveRightSolves(position).AddSolutions(solutions);
                MoveLeftSolves(position).AddSolutions(solutions);
                MoveUpSolves(position).AddSolutions(solutions);
                MoveDownSolves(position).AddSolutions(solutions);
            });

            var orderedSolutions = solutions.OrderByDescending(s => s.Weight);

            return orderedSolutions.ToArray();
        }

        protected virtual SolutionPack MoveRightSolves( BoardPosition position )
        {
            var solutions = new SolutionPack( position.ArrayPosition,
                                              new Point( position.ArrayPosition.X + 1, position.ArrayPosition.Y ) );

            int hyperCubeWeight = ( position.Piece == BoardPiece.HyperCube ) ? (int) position.Piece.Weight : 0;

            var firstRightOf = board.RightOf( position );
            
            if ( firstRightOf == null || firstRightOf.Piece == position.Piece )
                return solutions;

            var secondRightOf = board.RightOf( firstRightOf );
            var thirdRightOf = board.RightOf( secondRightOf );

            var firstNewTopOf = board.TopOf( firstRightOf );
            var secondNewTopOf = board.TopOf( firstNewTopOf );

            var firstNewBottomOf = board.BottomOf( firstRightOf );
            var secondNewBottomOf = board.BottomOf( firstNewBottomOf );

            bool alignsRight = ( secondRightOf != null && thirdRightOf != null )
                               && ( position.Piece == secondRightOf.Piece )
                               && ( position.Piece == thirdRightOf.Piece );

            bool alignsTop = ( firstNewTopOf != null && secondNewTopOf != null )
                             && ( position.Piece == firstNewTopOf.Piece ) 
                             && ( position.Piece == secondNewTopOf.Piece );

            bool alignsBottom = ( firstNewBottomOf != null && secondNewBottomOf != null )
                                && ( position.Piece == firstNewBottomOf.Piece )
                                && ( position.Piece == secondNewBottomOf.Piece );

            bool alignsTopBottom = ( firstNewTopOf != null && firstNewBottomOf != null )
                                   && ( position.Piece == firstNewTopOf.Piece )
                                   && ( position.Piece == firstNewBottomOf.Piece );

            bool alignsTopBottom4a = ( firstNewTopOf != null && firstNewBottomOf != null && secondNewBottomOf != null )
                                     && position.Piece == firstNewTopOf.Piece
                                     && position.Piece == firstNewBottomOf.Piece
                                     && position.Piece == secondNewBottomOf.Piece;

            bool alignsTopBottom4b = ( firstNewTopOf != null && secondNewTopOf != null && firstNewBottomOf != null )
                                     && position.Piece == firstNewTopOf.Piece && position.Piece == secondNewTopOf.Piece
                                     && position.Piece == firstNewBottomOf.Piece;

            bool alignsTopBottom5 = alignsTopBottom4a && ( secondNewTopOf != null )
                                    && position.Piece == secondNewTopOf.Piece;

            if ( alignsRight )
            {
                solutions.Solution1.Weight = position.Piece.Weight + secondRightOf.Piece.Weight
                                             + thirdRightOf.Piece.Weight + hyperCubeWeight;
            }

            if ( alignsTop )
            {
                solutions.Solution2.Weight = position.Piece.Weight + firstNewTopOf.Piece.Weight
                                             + secondNewTopOf.Piece.Weight + hyperCubeWeight;
            }

            if ( alignsBottom )
            {
                solutions.Solution3.Weight = position.Piece.Weight + firstNewBottomOf.Piece.Weight
                                             + secondNewBottomOf.Piece.Weight + hyperCubeWeight;
            }

            if ( alignsTopBottom )
            {
                solutions.Solution4.Weight = position.Piece.Weight + firstNewTopOf.Piece.Weight
                                             + firstNewBottomOf.Piece.Weight + hyperCubeWeight;
            }

            if ( alignsTopBottom4a )
            {
                solutions.Solution5.Weight = position.Piece.Weight + firstNewTopOf.Piece.Weight
                                             + firstNewBottomOf.Piece.Weight + secondNewBottomOf.Piece.Weight + hyperCubeWeight;
            }

            if ( alignsTopBottom4b )
            {
                solutions.Solution6.Weight = position.Piece.Weight + firstNewTopOf.Piece.Weight
                                             + secondNewTopOf.Piece.Weight + firstNewBottomOf.Piece.Weight + hyperCubeWeight;
            }

            if ( alignsTopBottom5 )
            {
                solutions.Solution7.Weight = position.Piece.Weight + firstNewTopOf.Piece.Weight
                                             + secondNewTopOf.Piece.Weight + firstNewBottomOf.Piece.Weight
                                             + secondNewBottomOf.Piece.Weight + hyperCubeWeight;
            }

            return solutions;
        }

        protected virtual SolutionPack MoveLeftSolves( BoardPosition position )
        {
            var solutions = new SolutionPack( position.ArrayPosition,
                                              new Point( position.ArrayPosition.X - 1, position.ArrayPosition.Y ) );

            decimal hyperCubeWeight = ( position.Piece == BoardPiece.HyperCube ) ? position.Piece.Weight : 0;

            var firstLeftOf = board.LeftOf( position );
            
            if ( firstLeftOf == null || firstLeftOf.Piece == position.Piece )
                return solutions;

            var secondLeftOf = board.LeftOf( firstLeftOf );
            var thirdLeftOf = board.LeftOf( secondLeftOf );

            var firstNewTopOf = board.TopOf( firstLeftOf );
            var secondNewTopOf = board.TopOf( firstNewTopOf );

            var firstNewBottomOf = board.BottomOf( firstLeftOf );
            var secondNewBottomOf = board.BottomOf( firstNewBottomOf );

            bool alignsLeft = ( secondLeftOf != null && thirdLeftOf != null )
                               && ( position.Piece == secondLeftOf.Piece )
                               && ( position.Piece == thirdLeftOf.Piece );

            bool alignsTop = ( firstNewTopOf != null && secondNewTopOf != null )
                             && ( position.Piece == firstNewTopOf.Piece ) 
                             && ( position.Piece == secondNewTopOf.Piece );

            bool alignsBottom = ( firstNewBottomOf != null && secondNewBottomOf != null )
                                && ( position.Piece == firstNewBottomOf.Piece )
                                && ( position.Piece == secondNewBottomOf.Piece );

            bool alignsTopBottom = ( firstNewTopOf != null && firstNewBottomOf != null )
                                   && ( position.Piece == firstNewTopOf.Piece )
                                   && ( position.Piece == firstNewBottomOf.Piece );

            bool alignsTopBottom4a = ( firstNewTopOf != null && firstNewBottomOf != null && secondNewBottomOf != null )
                                     && position.Piece == firstNewTopOf.Piece
                                     && position.Piece == firstNewBottomOf.Piece
                                     && position.Piece == secondNewBottomOf.Piece;

            bool alignsTopBottom4b = ( firstNewTopOf != null && secondNewTopOf != null && firstNewBottomOf != null )
                                     && position.Piece == firstNewTopOf.Piece && position.Piece == secondNewTopOf.Piece
                                     && position.Piece == firstNewBottomOf.Piece;

            bool alignsTopBottom5 = alignsTopBottom4a && ( secondNewTopOf != null )
                                    && position.Piece == secondNewTopOf.Piece;

            if ( alignsLeft )
            {
                solutions.Solution1.Weight = position.Piece.Weight + secondLeftOf.Piece.Weight
                                             + thirdLeftOf.Piece.Weight + hyperCubeWeight;
            }

            if ( alignsTop )
            {
                solutions.Solution2.Weight = position.Piece.Weight + firstNewTopOf.Piece.Weight
                                             + secondNewTopOf.Piece.Weight + hyperCubeWeight;
            }

            if ( alignsBottom )
            {
                solutions.Solution3.Weight = position.Piece.Weight + firstNewBottomOf.Piece.Weight
                                             + secondNewBottomOf.Piece.Weight + hyperCubeWeight;
            }

            if ( alignsTopBottom )
            {
                solutions.Solution4.Weight = position.Piece.Weight + firstNewTopOf.Piece.Weight
                                             + firstNewBottomOf.Piece.Weight + hyperCubeWeight;
            }

            if ( alignsTopBottom4a )
            {
                solutions.Solution5.Weight = position.Piece.Weight + firstNewTopOf.Piece.Weight
                                             + firstNewBottomOf.Piece.Weight + secondNewBottomOf.Piece.Weight + hyperCubeWeight;
            }

            if ( alignsTopBottom4b )
            {
                solutions.Solution6.Weight = position.Piece.Weight + firstNewTopOf.Piece.Weight
                                             + secondNewTopOf.Piece.Weight + firstNewBottomOf.Piece.Weight + hyperCubeWeight;
            }

            if ( alignsTopBottom5 )
            {
                solutions.Solution7.Weight = position.Piece.Weight + firstNewTopOf.Piece.Weight
                                             + secondNewTopOf.Piece.Weight + firstNewBottomOf.Piece.Weight
                                             + secondNewBottomOf.Piece.Weight + hyperCubeWeight;
            }

            return solutions;
        }

        protected virtual SolutionPack MoveUpSolves( BoardPosition position )
        {
            var solutions = new SolutionPack( position.ArrayPosition,
                                              new Point( position.ArrayPosition.X, position.ArrayPosition.Y - 1 ) );

            decimal hyperCubeWeight = ( position.Piece == BoardPiece.HyperCube ) ? position.Piece.Weight : 0;

            var firstTopOf = board.TopOf( position );

            if ( firstTopOf == null || firstTopOf.Piece == position.Piece )
                return solutions;

            var secondTopOf = board.TopOf( firstTopOf );
            var thirdTopOf = board.TopOf( secondTopOf );

            var firstNewLeftOf = board.LeftOf( firstTopOf );
            var secondNewLeftOf = board.LeftOf( firstNewLeftOf );

            var firstNewRightOf = board.RightOf( firstTopOf );
            var secondNewRightOf = board.RightOf( firstNewRightOf );

            bool alignsTop = ( secondTopOf != null && thirdTopOf != null )
                             && ( position.Piece == secondTopOf.Piece ) 
                             && ( position.Piece == thirdTopOf.Piece );

            bool alignsLeft = ( firstNewLeftOf != null && secondNewLeftOf != null )
                              && ( position.Piece == firstNewLeftOf.Piece )
                              && ( position.Piece == secondNewLeftOf.Piece );

            bool alignsRight = ( firstNewRightOf != null && secondNewRightOf != null )
                               && ( position.Piece == firstNewRightOf.Piece )
                               && ( position.Piece == secondNewRightOf.Piece );

            bool alignsLeftRight = ( firstNewLeftOf != null && firstNewRightOf != null )
                                   && ( position.Piece == firstNewLeftOf.Piece )
                                   && ( position.Piece == firstNewRightOf.Piece );

            bool alignsLeftRight4a = ( firstNewLeftOf != null && firstNewRightOf != null && secondNewRightOf != null )
                                     && position.Piece == firstNewLeftOf.Piece
                                     && position.Piece == firstNewRightOf.Piece
                                     && position.Piece == secondNewRightOf.Piece;

            bool alignsLeftRight4b = ( firstNewLeftOf != null && secondNewLeftOf != null && firstNewRightOf != null )
                                     && position.Piece == firstNewLeftOf.Piece && position.Piece == secondNewLeftOf.Piece
                                     && position.Piece == firstNewRightOf.Piece;

            bool alignsLeftRight5 = alignsLeftRight4a && ( secondNewLeftOf != null )
                                    && position.Piece == secondNewLeftOf.Piece;

            if ( alignsTop )
            {
                solutions.Solution1.Weight = position.Piece.Weight + secondTopOf.Piece.Weight
                                             + thirdTopOf.Piece.Weight + hyperCubeWeight;
            }

            if ( alignsLeft )
            {
                solutions.Solution2.Weight = position.Piece.Weight + firstNewLeftOf.Piece.Weight
                                             + secondNewLeftOf.Piece.Weight + hyperCubeWeight;
            }

            if ( alignsRight )
            {
                solutions.Solution3.Weight = position.Piece.Weight + firstNewRightOf.Piece.Weight
                                             + secondNewRightOf.Piece.Weight + hyperCubeWeight;
            }

            if ( alignsLeftRight )
            {
                solutions.Solution4.Weight = position.Piece.Weight + firstNewLeftOf.Piece.Weight
                                             + firstNewRightOf.Piece.Weight + hyperCubeWeight;
            }

            if ( alignsLeftRight4a )
            {
                solutions.Solution5.Weight = position.Piece.Weight + firstNewLeftOf.Piece.Weight
                                             + firstNewRightOf.Piece.Weight + secondNewRightOf.Piece.Weight + hyperCubeWeight;
            }

            if ( alignsLeftRight4b )
            {
                solutions.Solution6.Weight = position.Piece.Weight + firstNewLeftOf.Piece.Weight
                                             + secondNewLeftOf.Piece.Weight + firstNewRightOf.Piece.Weight + hyperCubeWeight;
            }

            if ( alignsLeftRight5 )
            {
                solutions.Solution7.Weight = position.Piece.Weight + firstNewLeftOf.Piece.Weight
                                             + secondNewLeftOf.Piece.Weight + firstNewRightOf.Piece.Weight
                                             + secondNewRightOf.Piece.Weight + hyperCubeWeight;
            }

            return solutions;
        }

        protected virtual SolutionPack MoveDownSolves( BoardPosition position )
        {
            var solutions = new SolutionPack( position.ArrayPosition,
                                              new Point( position.ArrayPosition.X, position.ArrayPosition.Y + 1 ) );

            decimal hyperCubeWeight = ( position.Piece == BoardPiece.HyperCube ) ? position.Piece.Weight : 0;

            var firstBottomOf = board.BottomOf( position );

            if ( firstBottomOf == null || firstBottomOf.Piece == position.Piece )
                return solutions;

            var secondBottomOf = board.BottomOf( firstBottomOf );
            var thirdBottomOf = board.BottomOf( secondBottomOf );

            var firstNewLeftOf = board.LeftOf( firstBottomOf );
            var secondNewLeftOf = board.LeftOf( firstNewLeftOf );

            var firstNewRightOf = board.RightOf( firstBottomOf );
            var secondNewRightOf = board.RightOf( firstNewRightOf );

            bool alignsBottom = ( secondBottomOf != null && thirdBottomOf != null )
                             && ( position.Piece == secondBottomOf.Piece ) 
                             && ( position.Piece == thirdBottomOf.Piece );

            bool alignsLeft = ( firstNewLeftOf != null && secondNewLeftOf != null )
                              && ( position.Piece == firstNewLeftOf.Piece )
                              && ( position.Piece == secondNewLeftOf.Piece );

            bool alignsRight = ( firstNewRightOf != null && secondNewRightOf != null )
                               && ( position.Piece == firstNewRightOf.Piece )
                               && ( position.Piece == secondNewRightOf.Piece );

            bool alignsLeftRight = ( firstNewLeftOf != null && firstNewRightOf != null )
                                   && ( position.Piece == firstNewLeftOf.Piece )
                                   && ( position.Piece == firstNewRightOf.Piece );

            bool alignsLeftRight4a = ( firstNewLeftOf != null && firstNewRightOf != null && secondNewRightOf != null )
                                     && position.Piece == firstNewLeftOf.Piece
                                     && position.Piece == firstNewRightOf.Piece
                                     && position.Piece == secondNewRightOf.Piece;

            bool alignsLeftRight4b = ( firstNewLeftOf != null && secondNewLeftOf != null && firstNewRightOf != null )
                                     && position.Piece == firstNewLeftOf.Piece && position.Piece == secondNewLeftOf.Piece
                                     && position.Piece == firstNewRightOf.Piece;

            bool alignsLeftRight5 = alignsLeftRight4a && ( secondNewLeftOf != null )
                                    && position.Piece == secondNewLeftOf.Piece;

            if ( alignsBottom )
            {
                solutions.Solution1.Weight = position.Piece.Weight + secondBottomOf.Piece.Weight
                                             + thirdBottomOf.Piece.Weight + hyperCubeWeight;
            }

            if ( alignsLeft )
            {
                solutions.Solution2.Weight = position.Piece.Weight + firstNewLeftOf.Piece.Weight
                                             + secondNewLeftOf.Piece.Weight + hyperCubeWeight;
            }

            if ( alignsRight )
            {
                solutions.Solution3.Weight = position.Piece.Weight + firstNewRightOf.Piece.Weight
                                             + secondNewRightOf.Piece.Weight + hyperCubeWeight;
            }

            if ( alignsLeftRight )
            {
                solutions.Solution4.Weight = position.Piece.Weight + firstNewLeftOf.Piece.Weight
                                             + firstNewRightOf.Piece.Weight + hyperCubeWeight;
            }

            if ( alignsLeftRight4a )
            {
                solutions.Solution5.Weight = position.Piece.Weight + firstNewLeftOf.Piece.Weight
                                             + firstNewRightOf.Piece.Weight + secondNewRightOf.Piece.Weight + hyperCubeWeight;
            }

            if ( alignsLeftRight4b )
            {
                solutions.Solution6.Weight = position.Piece.Weight + firstNewLeftOf.Piece.Weight
                                             + secondNewLeftOf.Piece.Weight + firstNewRightOf.Piece.Weight + hyperCubeWeight;
            }

            if ( alignsLeftRight5 )
            {
                solutions.Solution7.Weight = position.Piece.Weight + firstNewLeftOf.Piece.Weight
                                             + secondNewLeftOf.Piece.Weight + firstNewRightOf.Piece.Weight
                                             + secondNewRightOf.Piece.Weight + hyperCubeWeight;
            }

            return solutions;
        }
    }
}
