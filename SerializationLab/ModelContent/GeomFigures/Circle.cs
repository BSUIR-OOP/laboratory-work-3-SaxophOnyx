using System;

namespace SerializationLab
{
    public class Circle : CentrifiedFigure
    {
        public double Radius { get; set; }


        public Circle(Point center, double radius)
            : base(center)
        {
            Radius = radius;
        }

        public Circle(Point center, Point point)
            : base(center)
        {
            Radius = Math.Round(Math.Sqrt(Math.Pow((center.X - point.X), 2) + Math.Pow((center.Y - point.Y), 2)));
        }

        public Circle()
            : base()
        {
            Radius = 0;
        }
    }
}
