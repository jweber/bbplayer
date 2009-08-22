using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace bbplayer
{
    class SolutionFinder
    {
        private Board _board;

        public SolutionFinder( Board board )
        {
            _board = board;
        }

        public Solution FindSolution()
        {
            for ( int y = 7; y >= 0; y-- )
            {
                for ( int x = 0; x < 8; x++ )
                {
                    var position = _board[y, x];
                    if ( MoveRightSolves( position ) )
                        return new Solution
                        {
                            ArrayPosition1 = position.ArrayPosition,
                            ArrayPosition2 = new Point( position.ArrayPosition.X + 1, position.ArrayPosition.Y )
                        };

                    if ( MoveLeftSolves( position ) )
                        return new Solution
                        {
                            ArrayPosition1 = position.ArrayPosition,
                            ArrayPosition2 = new Point( position.ArrayPosition.X - 1, position.ArrayPosition.Y )
                        };

                    if ( MoveUpSolves( position ) )
                        return new Solution
                        {
                            ArrayPosition1 = position.ArrayPosition,
                            ArrayPosition2 = new Point( position.ArrayPosition.X, position.ArrayPosition.Y - 1 )
                        };

                    if ( MoveDownSolves( position ) )
                        return new Solution
                        {
                            ArrayPosition1 = position.ArrayPosition,
                            ArrayPosition2 = new Point( position.ArrayPosition.X, position.ArrayPosition.Y + 1 )
                        };

                }
            }

            return null;
        }

        bool MoveRightSolves( BoardPosition position )
        {
            var firstRightOf = _board.RightOf( position );
            
            if ( firstRightOf == null || firstRightOf.Piece == position.Piece )
                return false;

            var secondRightOf = _board.RightOf( firstRightOf );
            var thirdRightOf = _board.RightOf( secondRightOf );

            var firstNewTopOf = _board.TopOf( firstRightOf );
            var secondNewTopOf = _board.TopOf( firstNewTopOf );

            var firstNewBottomOf = _board.BottomOf( firstRightOf );
            var secondNewBottomOf = _board.BottomOf( firstNewBottomOf );

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

            return alignsRight || alignsTop || alignsBottom || alignsTopBottom;
        }

        bool MoveLeftSolves( BoardPosition position )
        {
            var firstLeftOf = _board.LeftOf( position );
            
            if ( firstLeftOf == null || firstLeftOf.Piece == position.Piece )
                return false;

            var secondLeftOf = _board.LeftOf( firstLeftOf );
            var thirdLeftOf = _board.LeftOf( secondLeftOf );

            var firstNewTopOf = _board.TopOf( firstLeftOf );
            var secondNewTopOf = _board.TopOf( firstNewTopOf );

            var firstNewBottomOf = _board.BottomOf( firstLeftOf );
            var secondNewBottomOf = _board.BottomOf( firstNewBottomOf );

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

            return alignsLeft || alignsTop || alignsBottom || alignsTopBottom;
        }

        bool MoveUpSolves( BoardPosition position )
        {
            var firstTopOf = _board.TopOf( position );

            if ( firstTopOf == null || firstTopOf.Piece == position.Piece )
                return false;

            var secondTopOf = _board.TopOf( firstTopOf );
            var thirdTopOf = _board.TopOf( secondTopOf );

            var firstNewLeftOf = _board.LeftOf( firstTopOf );
            var secondNewLeftOf = _board.LeftOf( firstNewLeftOf );

            var firstNewRightOf = _board.RightOf( firstTopOf );
            var secondNewRightOf = _board.RightOf( firstNewRightOf );

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

            return alignsTop || alignsLeft || alignsRight || alignsLeftRight;
        }

        bool MoveDownSolves( BoardPosition position )
        {
            var firstBottomOf = _board.BottomOf( position );

            if ( firstBottomOf == null || firstBottomOf.Piece == position.Piece )
                return false;

            var secondBottomOf = _board.BottomOf( firstBottomOf );
            var thirdBottomOf = _board.BottomOf( secondBottomOf );

            var firstNewLeftOf = _board.LeftOf( firstBottomOf );
            var secondNewLeftOf = _board.LeftOf( firstNewLeftOf );

            var firstNewRightOf = _board.RightOf( firstBottomOf );
            var secondNewRightOf = _board.RightOf( firstNewRightOf );

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

            return alignsBottom || alignsLeft || alignsRight || alignsLeftRight;
        }

    }
}
