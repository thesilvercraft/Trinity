namespace Trinity.Shared
{
    public class TrinityColor
    {
        public byte R;
        public byte G;
        public byte B;

        public TrinityColor(int color)
        {
            R = ((byte)(color >> 16 & 0xff));
            G = ((byte)(color >> 8 & 0xff));
            B = ((byte)(color & 0xff));
        }

        public TrinityColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }
    }
}