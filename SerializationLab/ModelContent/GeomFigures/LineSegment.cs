namespace SerializationLab
{
    public class LineSegment : AbstractFigure
    {
        public Point A { get; set; }

        public Point B { get; set; }


        public LineSegment(Point a, Point b)
            : base()
        {
            A = a;
            B = b;
        }

        public LineSegment()
            : base()
        {
            A = new Point();
            B = new Point();
        }
    }
}
