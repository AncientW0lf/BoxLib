using System;

namespace BoxLib.Containers
{
	public struct LineF : IEquatable<LineF>
	{
		public float StartX { get; set; }

		public float StartY { get; set; }

		public float EndX { get; set; }

		public float EndY { get; set; }

		public LineF(float startX, float startY, float endX, float endY)
		{
			StartX = startX;
			StartY = startY;
			EndX = endX;
			EndY = endY;
		}

		/// <inheritdoc />
		public bool Equals(LineF other)
		{
			if (!StartX.Equals(other.StartX))
				return false;
			if (!StartY.Equals(other.StartY))
				return false;
			if (!EndX.Equals(other.EndX))
				return false;
			if (!EndY.Equals(other.EndY))
				return false;

			return true;
		}
	}
}