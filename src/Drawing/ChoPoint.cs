namespace Cinchoo.Core.Drawing
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Diagnostics;

	#endregion NameSpaces

	/// <summary>
	/// Defines a point in two dimensional space with whole numbers.
	/// </summary>
	public struct ChoPoint : IEquatable<ChoPoint>
	{
		/// <summary>
		/// Initialize an instance of this class using the supplied coordinates.
		/// </summary>
		/// <param name="x">X coordinate of point.</param>
		/// <param name="y">Y coordinate of point.</param>
		[DebuggerStepThrough]
		public ChoPoint(Int32 x, Int32 y)
			: this()
		{
			X = x;
			Y = y;
		}

		/// <summary>
		/// Determines whether the supplied object is an instance of this class representing the same location in two dimensional space.
		/// </summary>
		/// <param name="obj">The object to be compared.</param>
		/// <returns>true if the supplied object is equal to this instance; false otherwise.</returns>
		[DebuggerStepThrough]
		public override Boolean Equals(Object obj)
		{
			if (obj == null || obj.GetType() != this.GetType())
				return false;

			return this == (ChoPoint)obj;
		}

		[DebuggerStepThrough]
		public Boolean Equals(ChoPoint other)
		{
			return this == other;
		}

		/// <summary>
		/// Determines whether two Points represent the same location in two dimensional space.
		/// </summary>
		/// <param name="lhs">The first Point to be compared.</param>
		/// <param name="rhs">The second Point to be compared.</param>
		/// <returns>true if the two Points represent the same location; false otherwise.</returns>
		[DebuggerStepThrough]
		public static Boolean operator ==(ChoPoint lhs, ChoPoint rhs)
		{
			return lhs.X == rhs.X && lhs.Y == rhs.Y;
		}

		/// <summary>
		/// Determines whether two Points do not represent the same location in two dimensional space.
		/// </summary>
		/// <param name="lhs">The first Point to be compared.</param>
		/// <param name="rhs">The second Point to be compared.</param>
		/// <returns>true if the two Points do not represent the same location; false otherwise.</returns>
		[DebuggerStepThrough]
		public static Boolean operator !=(ChoPoint lhs, ChoPoint rhs)
		{
			return lhs.X != rhs.X || lhs.Y != rhs.Y;
		}

		/// <summary>
		/// Adds two instances of this class together.
		/// </summary>
		/// <param name="lhs">The first Point to be added together.</param>
		/// <param name="rhs">The second Point to be added together.</param>
		/// <returns>A new instance of this class where each coordinate is the sum of the same coordinate of the supplied instances.</returns>
		[DebuggerStepThrough]
		public static ChoPoint operator +(ChoPoint lhs, ChoPoint rhs)
		{
			return new ChoPoint(lhs.X + rhs.X, lhs.Y + rhs.Y);
		}

		/// <summary>
		/// Subtracts an instance of this class from another instance.
		/// </summary>
		/// <param name="lhs">The Point on the left side of the minus sign.</param>
		/// <param name="rhs">The Point on the right side of the minus sign.</param>
		/// <returns>A new instance of this class where each coordinate is the difference of the same coordinate of the supplied instances.</returns>
		[DebuggerStepThrough]
		public static ChoPoint operator -(ChoPoint lhs, ChoPoint rhs)
		{
			return new ChoPoint(lhs.X - rhs.X, lhs.Y - rhs.Y);
		}

		/// <summary>
		/// Scales an instance of this class by a scalar value.
		/// </summary>
		/// <param name="lhs">The Point instance to be scaled.</param>
		/// <param name="scalar">The scalar value.</param>
		/// <returns>A new instance of this class with each coordinate equal to the same coordinate of the supplied Point mulitplied by the scalar value.</returns>
		[DebuggerStepThrough]
		public static ChoPoint operator *(ChoPoint lhs, Int32 scalar)
		{
			return new ChoPoint(lhs.X * scalar, lhs.Y * scalar);
		}

		/// <summary>
		/// Scales an instance of this class by a scalar value.
		/// </summary>
		/// <param name="lhs">The Point instance to be scaled.</param>
		/// <param name="scalar">The scalar value.</param>
		/// <returns>A new instance of this class with each coordinate equal to the same coordinate of the supplied Point divided by the scalar value.</returns>
		[DebuggerStepThrough]
		public static ChoPoint operator /(ChoPoint lhs, Int32 scalar)
		{
			return new ChoPoint(lhs.X / scalar, lhs.Y / scalar);
		}

		/// <summary>
		/// Generates a hash code based off the value of this instance.
		/// </summary>
		/// <returns>The hash code of this instance.</returns>
		[DebuggerStepThrough]
		public override Int32 GetHashCode()
		{
			return X ^ Y;
		}

		/// <summary>
		/// Generates a System.String whose value is an representation of this instance.
		/// </summary>
		/// <returns>A System.String representation of this instance.</returns>
		[DebuggerStepThrough]
		public override String ToString()
		{
			return X + ", " + Y;
		}

		/// <summary>
		/// The X coordinate of the location this instance represents in space.
		/// </summary>
		/// <returns>The X coordinate.</returns>
		public Int32 X
		{
			get { return m_x; }

			set { m_x = value; }
		}

		/// <summary>
		/// The Y coordinate of the location this instance represents in space.
		/// </summary>
		/// <returns>The Y coordinate.</returns>
		public Int32 Y
		{
			get { return m_y; }

			set { m_y = value; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_x;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_y;

		#endregion
	}
}
