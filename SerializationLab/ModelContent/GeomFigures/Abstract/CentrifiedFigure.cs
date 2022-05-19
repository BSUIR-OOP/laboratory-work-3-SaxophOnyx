namespace SerializationLab
{
    public abstract class CentrifiedFigure : ConnectedFigure
    {
        public Point CenterPos { get; set; }


        public CentrifiedFigure(Point center)
            : base()
        {
            CenterPos = center;
        }

        public CentrifiedFigure()
            : base()
        {
            CenterPos = new Point();
        }
    }
}
