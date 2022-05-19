namespace SerializationLab
{
    public abstract class ConnectedFigure: AbstractFigure
    {
        public Color FillingColor { get; set; }


        public ConnectedFigure()
            : base()
        {
            FillingColor = new Color();
        }
    }
}
