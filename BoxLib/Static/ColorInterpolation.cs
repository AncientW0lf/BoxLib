using System;
using System.Collections.Generic;
using System.Drawing;

namespace BoxLib.Static
{
	public static class ColorUtils
	{
		public static List<Color> RgbLinearInterpolate(Color start, Color end, int colorCount)
		{
			var ret = new List<Color>();

			// linear interpolation lerp (r,a,b) = (1-r)*a + r*b = (1-r)*(ax,ay,az) + r*(bx,by,bz)
			for (int n = 0; n < colorCount; n++)
			{
				double r = n / (double)(colorCount - 1);
				double nr = 1.0 - r;
				double A = nr * start.A + r * end.A;
				double R = nr * start.R + r * end.R;
				double G = nr * start.G + r * end.G;
				double B = nr * start.B + r * end.B;

				ret.Add(Color.FromArgb((byte)A, (byte)R, (byte)G, (byte)B));
			}

			return ret;
		}
		public static List<Color> RgbLinearInterpolate(Color start, Color middle, Color end, int colorCount)
		{
			var ret = new List<Color>();

			if (colorCount <= 1)
				return ret;

			int size = colorCount / 2;
			int remaining = colorCount - size + 1;

			List<Color> list1 = RgbLinearInterpolate(start, middle, size);
			List<Color> list2 = RgbLinearInterpolate(middle, end, remaining);
			list2.RemoveAt(0);

			ret.AddRange(list1);
			ret.AddRange(list2);

			return ret;
		}
		public static Color HsvToRgb(double h, double s, double v)
		{
			int hi = (int)Math.Floor(h / 60.0) % 6;
			double f = h / 60.0 - Math.Floor(h / 60.0);

			double p = v * (1.0 - s);
			double q = v * (1.0 - f * s);
			double t = v * (1.0 - (1.0 - f) * s);

            Color ret = hi switch
            {
                0 => GetRgb(v, t, p),
                1 => GetRgb(q, v, p),
                2 => GetRgb(p, v, t),
                3 => GetRgb(p, q, v),
                4 => GetRgb(t, p, v),
                5 => GetRgb(v, p, q),
                _ => Color.FromArgb(0xFF, 0x00, 0x00, 0x00)
            };
            return ret;
		}
		public static Color GetRgb(double r, double g, double b)
		{
			return Color.FromArgb(255, (byte)(r * 255.0), (byte)(g * 255.0), (byte)(b * 255.0));
		}
	}
}
