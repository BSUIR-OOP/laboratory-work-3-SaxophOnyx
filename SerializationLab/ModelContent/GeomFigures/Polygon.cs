using System.Collections.Generic;

namespace SerializationLab
{
    public class Polygon : ConnectedFigure
    {
        public SerializableList<Point> Points { get; set; }


        public Polygon(IEnumerable<Point> points)
        {
            Points = new SerializableList<Point>();
            foreach (var item in points)
                Points.Add(item);
        }

        public Polygon()
        {
            Points = new SerializableList<Point>();
        }
    }
}
