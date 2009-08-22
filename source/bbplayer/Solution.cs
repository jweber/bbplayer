using System.Drawing;

namespace bbplayer
{
    class Solution
    {
        public Solution()
        {}

        public Solution( Point initialPosition, Point moveToPosition )
        {
            ArrayPosition1 = initialPosition;
            ArrayPosition2 = moveToPosition;
        }

        public decimal Weight { get; set; }

        public Point ArrayPosition1 { get; set; }
        public Point ArrayPosition2 { get; set; }
    }
}
