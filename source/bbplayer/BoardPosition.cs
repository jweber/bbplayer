using System.Drawing;
using Rectangle=System.Windows.Shapes.Rectangle;

namespace bbplayer
{
    class BoardPosition
    {
        public BoardPosition()
        {}

        public BoardPosition( Rectangle facade )
        {
            Facade = facade;
        }

        public Rectangle Facade { get; set; }
        public BoardPiece Piece { get; private set; }
        public Point ArrayPosition { get; private set; }

        public void SetPiece(BoardPiece piece, int arrayY, int arrayX)
        {
            Piece = piece;
            ArrayPosition = new Point(arrayX, arrayY);
        }
    }
}
