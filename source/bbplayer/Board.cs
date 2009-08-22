using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;

namespace bbplayer
{
    class Board
    {
        private BoardPosition[,] _boardPositions;

        public Board()
        {
            _boardPositions = new BoardPosition[8,8];
        }

        public void SetFacade( Rectangle facade, int x, int y )
        {
            _boardPositions[y, x].Facade = facade;
        }

        public BoardPosition this[int y, int x]
        {
            get { return _boardPositions[y, x]; }
            set { _boardPositions[y, x] = value; }
        }

        public BoardPosition TopOf( BoardPosition position )
        {
            if ( position == null || position.ArrayPosition.Y == 0 )
                return null;

            return this[position.ArrayPosition.Y - 1, position.ArrayPosition.X];
        }

        public BoardPosition RightOf( BoardPosition position )
        {
            if ( position == null || position.ArrayPosition.X == 7 )
                return null;

            return this[position.ArrayPosition.Y, position.ArrayPosition.X + 1];
        }

        public BoardPosition BottomOf( BoardPosition position )
        {
            if ( position == null || position.ArrayPosition.Y == 7 )
                return null;

            return this[position.ArrayPosition.Y + 1, position.ArrayPosition.X];
        }

        public BoardPosition LeftOf( BoardPosition position )
        {
            if ( position == null || position.ArrayPosition.X == 0 )
                return null;

            return this[position.ArrayPosition.Y, position.ArrayPosition.X - 1];
        }

    }
}
