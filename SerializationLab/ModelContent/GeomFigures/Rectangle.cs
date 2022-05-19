using System;

namespace SerializationLab
{
    public class Rectangle : CentrifiedFigure
    {
        public double Width { get; set; }

        public double Height { get; set; }


        public Rectangle(Point center, double width, double height)
            : base(center)
        {
            Width = width;
            Height = height;
        }

        public Rectangle(Point point1, Point point2)
            : base(new Point((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2))
        {
            Width = Math.Abs(point2.X - point1.X);
            Height = Math.Abs(point2.Y - point1.Y);
        }

        public Rectangle()
            : base()
        {
            Width = 0;
            Height = 0;
        }
    }
}
