using System;

namespace SerializationLab
{
    public class Ellipse : CentrifiedFigure
    {
        public double SemiaxisX { get; set; }

        public double SemiaxisY { get; set; }


        public Ellipse(Point center, double semiaxisX, double semiaxisY)
            : base(center)
        {
            SemiaxisX = semiaxisX;
            SemiaxisY = semiaxisY;
        }

        public Ellipse(Point point1, Point point2)
            : base(new Point((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2))
        {
            SemiaxisX = Math.Abs(point1.X - point2.X) / 2;
            SemiaxisY = Math.Abs(point1.Y - point2.Y) / 2;
        }

        public Ellipse()
            : base()
        {
            SemiaxisX = 0;
            SemiaxisY = 0;
        }
    }
}
