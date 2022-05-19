using System;

namespace SerializationLab
{
    public class RegularPolygon : CentrifiedFigure
    {
        public int SidesCount { get; set; }

        public double SideLength { get; set; }


        public RegularPolygon(Point center, int sidesCount, double sideLength)
            : base(center)
        {
            SidesCount = sidesCount;
            SideLength = sideLength;
        }

        public RegularPolygon(Point center, Point point, int verticesNumber)
            : base(center)
        {
            SidesCount = verticesNumber;
            double diameter = 2 * Math.Round(Math.Sqrt(Math.Pow(center.X - point.X, 2) + Math.Pow(center.Y - point.Y, 2)));
            SideLength = diameter * Math.Sin(Math.PI / verticesNumber);
        }

        public RegularPolygon()
            : base()
        {
            SideLength = 0;
            SidesCount = 0;
        }
    }
}
