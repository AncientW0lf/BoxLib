﻿using System;
using System.Collections.Generic;
using System.Drawing;

namespace BoxLib.Scripts
{
	public static class ColorUtils
	{
		public static List<Color> RgbLinearInterpolate(Color start, Color end, int colorCount)
		{
			List<Color> ret = new List<Color>();

			// linear interpolation lerp (r,a,b) = (1-r)*a + r*b = (1-r)*(ax,ay,az) + r*(bx,by,bz)
			for (int n = 0; n < colorCount; n++)
			{
				double r = (double)n / (double)(colorCount - 1);
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
			//if (colorCount % 2 == 0)
				//throw new ArgumentException("colorCount should be and odd number. Currently it is: " + colorCount);

			var ret = new List<Color>();

			if (colorCount == 0)
				return ret;

			int size = (colorCount + 1) / 2;

			List<Color> res = RgbLinearInterpolate(start, middle, size);
			if (res.Count > 0 && colorCount % 2 == 0)
			{
				res.RemoveAt(res.Count - 1);
			}
			else
			{
				byte resRed1 = res[res.Count - 1].R;
				byte resGreen1 = res[res.Count - 1].G;
				byte resBlue1 = res[res.Count - 1].B;
				byte resRed2 = res[res.Count - 2].R;
				byte resGreen2 = res[res.Count - 2].G;
				byte resBlue2 = res[res.Count - 2].B;

				res[res.Count - 1] = Color.FromArgb((resRed1 + resRed2) / 2, 
													(resGreen1 + resGreen2) / 2,
													(resBlue1 + resBlue2) / 2);
			}

			ret.AddRange(res);
			ret.AddRange(RgbLinearInterpolate(middle, end, size));

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
