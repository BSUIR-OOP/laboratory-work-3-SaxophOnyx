namespace SerializationLab
{
    public abstract class AbstractFigure
    {
        public Color OutlineColor { get; set; }

        public double OutlineThickness { get; set; }


        public AbstractFigure()
        {
            OutlineColor = new Color();
            OutlineThickness = 1;
        }
    }
}
