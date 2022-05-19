namespace SerializationLab
{
    public class Color
    {
        public byte Alpha { get; set; }

        public byte Red { get; set; }

        public byte Green { get; set; }

        public byte Blue { get; set; }


        public Color()
        {
            Alpha = 255;
            Red = 0;
            Green = 0;
            Blue = 0;
        }

        public Color (byte a, byte r, byte g, byte b)
        {
            Alpha = a;
            Red = r;
            Green = g;
            Blue = b;
        }
    }
}
