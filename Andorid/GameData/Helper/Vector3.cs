/////////////////////////////////////////////////////////////////////////////
//
// 3D Math Primer for Games and Graphics Development
//
// Vector3.h - Declarations for 3D vector class
//
// Visit gamemath.com for the latest version of this file.
//
// For additional comments, see Chapter 6.
//
/////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;

namespace GameData.Helper
{
    class Vector3
    {
    // Public representation:  Not many options here.

	    float x;
        public float X {get{return x;} set{x = value;}}
        float y;
        public float Y {get{return y;} set{y = value;}}
        float z;
        public float Z {get{return z;} set{z = value;}}

    // Constructors
        
	    // Construct given three values

	    public Vector3(float nx, float ny, float nz) 
        {
            x = nx;
            y = ny;
            z = nz;
        }

    // Standard object maintenance

	    // Check for equality

	    public static bool operator ==(Vector3 a, Vector3 b) 
        {
		    return b.X==a.X && b.Y==a.Y && b.Z==a.Z;
	    }

	    public static bool operator !=(Vector3 a, Vector3 b) 
        {
		    return b.X!=a.X || b.Y!=a.Y || b.Z!=a.Z;
	    }


    // Vector operations

	    // Set the vector to zero

	    public void zero() { x = y = z = 0.0f; }

	    // Unary minus returns the negative of the vector

	    public static Vector3 operator -(Vector3 a) { return new Vector3(-a.X,-a.Y,-a.Z); }

	    // Binary + and - add and subtract vectors

	    public static Vector3 operator +(Vector3 a, Vector3 b) 
        {
		    return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
	    }

	    public static Vector3 operator -(Vector3 b, Vector3 a) 
        {
		    return new Vector3(b.X - a.X, b.Y - a.Y, b.Z - a.Z);
	    }

	    // Multiplication and division by scalar

	    public static Vector3 operator *(Vector3 a, float f) 
        {
		    return new Vector3(a.X*f, a.Y*f, a.Z*f);
	    }

	    public static Vector3 operator /(Vector3 a, float f) 
        {
		    float	oneOverA = 1.0f / f; // NOTE: no check for divide by zero here
		    return new Vector3(a.X*oneOverA, a.Y*oneOverA, a.Z*oneOverA);
	    }


	    // Normalize the vector

	    public void	normalize() 
        {
		    double magSq = x*x + y*y + z*z;
		    if (magSq > 0.0f) 
            { // check for divide-by-zero
			    float oneOverMag = (float) (1.0f / Math.Sqrt(magSq));
			    x *= oneOverMag;
			    y *= oneOverMag;
			    z *= oneOverMag;
		    }
	    }

	    // Vector dot product.  We overload the standard
	    // multiplication symbol to do this

	    public static float operator *(Vector3 a, Vector3 b) 
        {
		    return a.X*b.X + a.Y*b.Y + a.Z*b.Z;
	    }

        /////////////////////////////////////////////////////////////////////////////
        //
        // Nonmember functions
        //
        /////////////////////////////////////////////////////////////////////////////

        // Compute the magnitude of a vector

        public static float vectorMag(Vector3 a) {
	        return (float)Math.Sqrt(a.X*a.X + a.Y*a.Y + a.Z*a.Z);
        }

        // Compute the cross product of two vectors

        public static Vector3 crossProduct(Vector3 a, Vector3 b) 
        {
	        return new Vector3(
		        a.Y*b.Z - a.Z*b.Y,
		        a.Z*b.X - a.X*b.Z,
		        a.X*b.Y - a.Y*b.X
	        );
        }

        // Scalar on the left multiplication, for symmetry

        public static Vector3 operator *(float k, Vector3 v) 
        {
	        return new Vector3(k*v.X, k*v.Y, k*v.Z);
        }

        // Compute the distance between two points

        public static float distance(Vector3 a, Vector3 b) 
        {
	        float dx = a.X - b.X;
	        float dy = a.X - b.X;
	        float dz = a.X - b.X;
	        return (float)Math.Sqrt(dx*dx + dy*dy + dz*dz);
        }

        // Compute the distance between two points, squared.  Often useful
        // when comparing distances, since the square root is slow

        public static float distanceSquared(Vector3 a, Vector3 b) 
        {
	        float dx = a.X - b.X;
	        float dy = a.Y - b.Y;
	        float dz = a.Z - b.Z;
	        return dx*dx + dy*dy + dz*dz;
        }
    }
}
