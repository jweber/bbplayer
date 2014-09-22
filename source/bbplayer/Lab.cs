namespace bbplayer
{
    public class Lab
    {
        public Lab(double L, double a, double b)
        {
            this.L = L;
            this.a = a;
            this.b = b;
        }

        public double L { get; private set; }
        public double a { get; private set; }
        public double b { get; private set; }
    }
}