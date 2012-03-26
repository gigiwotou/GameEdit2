using System;
using System.Collections.Generic;
using System.Text;

namespace GameData.Helper
{
    class RotationMatrix
    {
        // The 9 values of the matrix.  See RotationMatrix.cpp file for
	    // the details of the layout

	    float	m11, m12, m13;
	    float	m21, m22, m23;
	    float	m31, m32, m33;

        public float M11{ get{ return m11;} set{ m11 = value; }}
        public float M12{ get{ return m12;} set{ m12 = value; }}
        public float M13{ get{ return m13;} set{ m13 = value; }}
        public float M21{ get{ return m21;} set{ m21 = value; }}
        public float M22{ get{ return m22;} set{ m22 = value; }}
        public float M23{ get{ return m23;} set{ m23 = value; }}
        public float M31{ get{ return m31;} set{ m31 = value; }}
        public float M32{ get{ return m32;} set{ m32 = value; }}
        public float M33{ get{ return m33;} set{ m33 = value; }}
        /////////////////////////////////////////////////////////////////////////////
        //
        // class RotationMatrix
        //
        //---------------------------------------------------------------------------
        //
        // MATRIX ORGANIZATION
        //
        // A user of this class should rarely care how the matrix is organized.
        // However, it is of course important that internally we keep everything
        // straight.
        //
        // The matrix is assumed to be a rotation matrix only, and therefore
        // orthoganal.  The "forward" direction of transformation (if that really
        // even applies in this case) will be from inertial to object space.
        // To perform an object->inertial rotation, we will multiply by the
        // transpose.
        //
        // In other words:
        //
        // Inertial to object:
        //
        //                  | m11 m12 m13 |
        //     [ ix iy iz ] | m21 m22 m23 | = [ ox oy oz ]
        //                  | m31 m32 m33 |
        //
        // Object to inertial:
        //
        //                  | m11 m21 m31 |
        //     [ ox oy oz ] | m12 m22 m32 | = [ ix iy iz ]
        //                  | m13 m23 m33 |
        //
        // Or, using column vector notation:
        //
        // Inertial to object:
        //
        //     | m11 m21 m31 | | ix |	| ox |
        //     | m12 m22 m32 | | iy | = | oy |
        //     | m13 m23 m33 | | iz |	| oz |
        //
        // Object to inertial:
        //
        //     | m11 m12 m13 | | ox |	| ix |
        //     | m21 m22 m23 | | oy | = | iy |
        //     | m31 m32 m33 | | oz |	| iz |
        //
        /////////////////////////////////////////////////////////////////////////////

        //---------------------------------------------------------------------------
        // RotationMatrix::identity
        //
        // Set the matrix to the identity matrix

        public void identity() 
        {
	        m11 = 1.0f; m12 = 0.0f; m13 = 0.0f;
	        m21 = 0.0f; m22 = 1.0f; m23 = 0.0f;
	        m31 = 0.0f; m32 = 0.0f; m33 = 1.0f;
        }

        //---------------------------------------------------------------------------
        // RotationMatrix::setup
        //
        // Setup the matrix with the specified orientation
        //
        // See 10.6.1

        public void setup(EulerAngles orientation) 
        {

	        // Fetch sine and cosine of angles

	        float	sh,ch, sp,cp, sb,cb;
	        MathUtil.sinCos(out sh, out ch, orientation.Heading);
	        MathUtil.sinCos(out sp, out cp, orientation.Pitch);
	        MathUtil.sinCos(out sb, out cb, orientation.Bank);

	        // Fill in the matrix elements

	        m11 = ch * cb + sh * sp * sb;
	        m12 = -ch * sb + sh * sp * cb;
	        m13 = sh * cp;

	        m21 = sb * cp;
	        m22 = cb * cp;
	        m23 = -sp;

	        m31 = -sh * cb + ch * sp * sb;
	        m32 = sb * sh + ch * sp * cb;
	        m33 = ch * cp;
        }

        //---------------------------------------------------------------------------
        // RotationMatrix::fromInertialToObjectQuaternion
        //
        // Setup the matrix, given a quaternion that performs an inertial->object
        // rotation
        //
        // See 10.6.3

        public void fromInertialToObjectQuaternion(Quaternion q) 
        {

	        // Fill in the matrix elements.  This could possibly be
	        // optimized since there are many common subexpressions.
	        // We'll leave that up to the compiler...

	        m11 = 1.0f - 2.0f * (q.Y*q.Y + q.Z*q.Z);
	        m12 = 2.0f * (q.X*q.Y + q.W*q.Z);
	        m13 = 2.0f * (q.X*q.Z - q.W*q.Y);

	        m21 = 2.0f * (q.X*q.Y - q.W*q.Z);
	        m22 = 1.0f - 2.0f * (q.X*q.X + q.Z*q.Z);
	        m23 = 2.0f * (q.Y*q.Z + q.W*q.X);

	        m31 = 2.0f * (q.X*q.Z + q.W*q.Y);
	        m32 = 2.0f * (q.Y*q.Z - q.W*q.X);
	        m33 = 1.0f - 2.0f * (q.X*q.X + q.Y*q.Y);

        }

        //---------------------------------------------------------------------------
        // RotationMatrix::fromObjectToInertialQuaternion
        //
        // Setup the matrix, given a quaternion that performs an object->inertial
        // rotation
        //
        // See 10.6.3

        public void fromObjectToInertialQuaternion(Quaternion q) 
        {

	        // Fill in the matrix elements.  This could possibly be
	        // optimized since there are many common subexpressions.
	        // We'll leave that up to the compiler...

	        m11 = 1.0f - 2.0f * (q.Y*q.Y + q.Z*q.Z);
	        m12 = 2.0f * (q.X*q.Y - q.W*q.Z);
	        m13 = 2.0f * (q.X*q.Z + q.W*q.Y);

	        m21 = 2.0f * (q.X*q.Y + q.W*q.Z);
	        m22 = 1.0f - 2.0f * (q.X*q.X + q.Z*q.Z);
	        m23 = 2.0f * (q.Y*q.Z - q.W*q.X);

	        m31 = 2.0f * (q.X*q.Z - q.W*q.Y);
	        m32 = 2.0f * (q.Y*q.Z + q.W*q.X);
	        m33 = 1.0f - 2.0f * (q.X*q.X + q.Y*q.Y);
        }

        //---------------------------------------------------------------------------
        // RotationMatrix::inertialToObject
        //
        // Rotate a vector from inertial to object space

        public Vector3	inertialToObject(Vector3 v) 
        {

	        // Perform the matrix multiplication in the "standard" way.

	        return new Vector3(
		        m11*v.X + m21*v.Y + m31*v.Z,
		        m12*v.X + m22*v.Y + m32*v.Z,
		        m13*v.X + m23*v.Y + m33*v.Z
	        );
        }

        //---------------------------------------------------------------------------
        // RotationMatrix::objectToInertial
        //
        // Rotate a vector from object to inertial space

        public Vector3	objectToInertial(Vector3 v) 
        {

	        // Multiply by the transpose

	        return new Vector3(
		        m11*v.X + m12*v.Y + m13*v.Z,
		        m21*v.X + m22*v.Y + m23*v.Z,
		        m31*v.X + m32*v.Y + m33*v.Z
	        );
        }
    }
}
