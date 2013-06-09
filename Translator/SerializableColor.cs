using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Translator
{
    [Serializable]
    public struct SerializableColor
    {
        public byte A;
        public byte R;
        public byte G;
        public byte B;

        public SerializableColor(byte a, byte r, byte g, byte b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }

        public SerializableColor(Color color)
            : this(color.A, color.R, color.G, color.B)
        {
        }

        public static implicit operator SerializableColor(Color color)
        {
            return new SerializableColor(color);
        }

        public static implicit operator Color(SerializableColor colour)
        {
            return Color.FromArgb(colour.A, colour.R, colour.G, colour.B);
        }
    }
}
