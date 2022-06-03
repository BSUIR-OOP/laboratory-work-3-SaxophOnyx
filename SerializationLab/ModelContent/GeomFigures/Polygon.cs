using System.Collections.Generic;

namespace SerializationLab
{
    public class Polygon : ConnectedFigure
    {
        public List<Point> Points { get; set; }


        public Polygon(IEnumerable<Point> points)
        {
            Points = new List<Point>();
            foreach (var item in points)
                Points.Add(item);
        }

        public Polygon()
        {
            Points = new List<Point>();
        }
    }
}
