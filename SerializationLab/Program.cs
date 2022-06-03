using System.Collections.Generic;

namespace SerializationLab
{
    class Program
    {
        static void Main(string[] args)
        {
            List<AbstractFigure> list = new List<AbstractFigure>
            {
                new Circle(new Point(25, 50), 30)
                
                /*new Dot(15, 20),
                new Ellipse(new Point(120, 20), 40, 50),
                new LineSegment(new Point(20, 20), new Point(20, 50)),
                new Polygon(new List<Point>() { new Point(100, 25), new Point(75, 80), new Point(10, 110) }),
                new Rectangle(new Point(70, 120), 60, 30),
                new RegularPolygon(new Point(200, 120), 7, 15)*/
            };

            string dirPath = "D:\\jsonfiles";

            ConsoleEditor editor = new ConsoleEditor(new PropertyHadler(), new JsonSerializer(), new FilesHandler(dirPath), list);
            editor.Start();
        }
    }
}
