using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace GameData.Helper
{
    //---------------------------------------------------------------------------
    // class Quaternion
    //
    // Implement a quaternion, for purposes of representing an angular
    // displacement (orientation) in 3D.
    class Quaternion
    {
        // The 4 values of the quaternion.  Normally, it will not
        // be necessary to manipulate these directly.  However,
        // we leave them public, since prohibiting direct access
        // makes some operations, such as file I/O, unnecessarily
        // complicated.

        float w, x, y, z;
        public float W { get{return w;} set{w = value;}}
        public float X { get{return x;} set{x = value;}}
        public float Y { get{return y;} set{y = value;}}
        public float Z { get{return z;} set{z = value;}}

        // Set to identity

        public void identity() { w = 1.0f; x = y = z = 0.0f; }

        
        /////////////////////////////////////////////////////////////////////////////
        //
        // global data
        //
        /////////////////////////////////////////////////////////////////////////////

        // The global identity quaternion.  Notice that there are no constructors
        // to the Quaternion class, since we really don't need any.

        //const Quaternion kQuaternionIdentity = {
        //    1.0f, 0.0f, 0.0f, 0.0f
        //};

        /////////////////////////////////////////////////////////////////////////////
        //
        // class Quaternion members
        //
        /////////////////////////////////////////////////////////////////////////////

        //---------------------------------------------------------------------------
        // setToRotateAboutX
        // setToRotateAboutY
        // setToRotateAboutZ
        // setToRotateAboutAxis
        //
        // Setup the quaternion to rotate about the specified axis

        public void setToRotateAboutX(float theta) 
        {

	        // Compute the half angle

	        float	thetaOver2 = theta * .5f;

	        // Set the values

	        w = (float)Math.Cos(thetaOver2);
	        x = (float)Math.Sin(thetaOver2);
	        y = 0.0f;
	        z = 0.0f;
        }

        public void setToRotateAboutY(float theta) 
        {

	        // Compute the half angle

	        float	thetaOver2 = theta * .5f;

	        // Set the values

	        w = (float)Math.Cos(thetaOver2);
	        x = 0.0f;
	        y = (float)Math.Sin(thetaOver2);
	        z = 0.0f;
        }

        public void setToRotateAboutZ(float theta) 
        {

	        // Compute the half angle

	        float	thetaOver2 = theta * .5f;

	        // Set the values

	        w = (float)Math.Cos(thetaOver2);
	        x = 0.0f;
	        y = 0.0f;
	        z = (float)Math.Sin(thetaOver2);
        }

        public void setToRotateAboutAxis(Vector3 axis, float theta) 
        {

	        // The axis of rotation must be normalized

            Trace.Assert(Math.Abs(Vector3.vectorMag(axis) - 1.0f) < .01f, "error");

	        // Compute the half angle and its sin

	        float	thetaOver2 = theta * .5f;
	        float	sinThetaOver2 = (float)Math.Sin(thetaOver2);

	        // Set the values

	        w = (float)Math.Cos(thetaOver2);
	        x = axis.X * sinThetaOver2;
	        y = axis.Y * sinThetaOver2;
	        z = axis.Z * sinThetaOver2;
        }

        //---------------------------------------------------------------------------
        // EulerAngles::setToRotateObjectToInertial
        //
        // Setup the quaternion to perform an object->inertial rotation, given the
        // orientation in Euler angle format
        //
        // See 10.6.5 for more information.

        public void setToRotateObjectToInertial(EulerAngles orientation) 
        {

	        // Compute sine and cosine of the half angles

	        float	sp, sb, sh;
	        float	cp, cb, ch;
	        MathUtil.sinCos(out sp, out cp, orientation.Pitch * 0.5f);
	        MathUtil.sinCos(out sb, out cb, orientation.Bank * 0.5f);
	        MathUtil.sinCos(out sh, out ch, orientation.Heading * 0.5f);

	        // Compute values

	        w =  ch*cp*cb + sh*sp*sb;
	        x =  ch*sp*cb + sh*cp*sb;
	        y = -ch*sp*sb + sh*cp*cb;
	        z = -sh*sp*cb + ch*cp*sb;
        }

        //---------------------------------------------------------------------------
        // EulerAngles::setToRotateInertialToObject
        //
        // Setup the quaternion to perform an object->inertial rotation, given the
        // orientation in Euler angle format
        //
        // See 10.6.5 for more information.

        public void setToRotateInertialToObject(EulerAngles orientation) 
        {

	        // Compute sine and cosine of the half angles

	        float	sp, sb, sh;
	        float	cp, cb, ch;
	        MathUtil.sinCos(out sp, out cp, orientation.Pitch * 0.5f);
	        MathUtil.sinCos(out sb, out cb, orientation.Bank * 0.5f);
	        MathUtil.sinCos(out sh, out ch, orientation.Heading * 0.5f);

	        // Compute values

	        w =  ch*cp*cb + sh*sp*sb;
	        x = -ch*sp*cb - sh*cp*sb;
	        y =  ch*sp*sb - sh*cb*cp;
	        z =  sh*sp*cb - ch*cp*sb;
        }

        //---------------------------------------------------------------------------
        // operator *
        //
        // Quaternion cross product, which concatonates multiple angular
        // displacements.  The order of multiplication, from left to right,
        // corresponds to the order that the angular displacements are
        // applied.  This is backwards from the *standard* definition of
        // quaternion multiplication.  See section 10.4.8 for the rationale
        // behind this deviation from the standard.

        public static Quaternion operator *(Quaternion b, Quaternion a) 
        {
	        Quaternion result = new Quaternion();

	        result.w = b.W*a.W - b.W*a.X - b.Y*a.Y - b.Z*a.Z;
	        result.x = b.W*a.X + b.X*a.W + b.Z*a.Y - b.Y*a.Z;
	        result.y = b.W*a.Y + b.Y*a.W + b.X*a.Z - b.Z*a.X;
	        result.z = b.W*a.Z + b.Z*a.W + b.Y*a.X - b.X*a.Y;

	        return result;
        }

        //---------------------------------------------------------------------------
        // normalize
        //
        // "Normalize" a quaternion.  Note that normally, quaternions
        // are always normalized (within limits of numerical precision).
        // See section 10.4.6 for more information.
        //
        // This function is provided primarily to combat floating point "error
        // creep," which can occur when many successive quaternion operations
        // are applied.

        public void normalize() {

	        // Compute magnitude of the quaternion

	        float	mag = (float)Math.Sqrt(w*w + x*x + y*y + z*z);

	        // Check for bogus length, to protect against divide by zero

	        if (mag > 0.0f) {

		        // Normalize it

		        float	oneOverMag = 1.0f / mag;
		        w *= oneOverMag;
		        x *= oneOverMag;
		        y *= oneOverMag;
		        z *= oneOverMag;

	        } else {

		        // Houston, we have a problem

                Trace.Assert(false, "error");

		        // In a release build, just slam it to something

		        identity();
	        }
        }

        //---------------------------------------------------------------------------
        // getRotationAngle
        //
        // Return the rotation angle theta

        public float getRotationAngle()
        {

	        // Compute the half angle.  Remember that w = (float)Math.Cos(theta / 2)

	        float thetaOver2 = MathUtil.safeAcos(w);

	        // Return the rotation angle

	        return thetaOver2 * 2.0f;
        }

        //---------------------------------------------------------------------------
        // getRotationAxis
        //
        // Return the rotation axis

        public Vector3	getRotationAxis() 
        {

	        // Compute sin^2(theta/2).  Remember that w = (float)Math.Cos(theta/2),
	        // and sin^2(x) + cos^2(x) = 1

	        float sinThetaOver2Sq = 1.0f - w*w;

	        // Protect against numerical imprecision

	        if (sinThetaOver2Sq <= 0.0f) {

		        // Identity quaternion, or numerical imprecision.  Just
		        // return any valid vector, since it doesn't matter

		        return new Vector3(1.0f, 0.0f, 0.0f);
	        }

	        // Compute 1 / (float)Math.Sin(theta/2)

	        float	oneOverSinThetaOver2 = 1.0f / (float)Math.Sqrt(sinThetaOver2Sq);

	        // Return axis of rotation

	        return new Vector3(
		        x * oneOverSinThetaOver2,
		        y * oneOverSinThetaOver2,
		        z * oneOverSinThetaOver2
	        );
        }

        /////////////////////////////////////////////////////////////////////////////
        //
        // Nonmember functions
        //
        /////////////////////////////////////////////////////////////////////////////

        //---------------------------------------------------------------------------
        // dotProduct
        //
        // Quaternion dot product.  We use a nonmember function so we can
        // pass quaternion expressions as operands without having "funky syntax"
        //
        // See 10.4.10

        public float dotProduct(Quaternion a, Quaternion b) {
	        return a.W*b.W + a.X*b.X + a.Y*b.Y + a.Z*b.Z;
        }

        //---------------------------------------------------------------------------
        // slerp
        //
        // Spherical linear interpolation.
        //
        // See 10.4.13

        public Quaternion slerp(Quaternion q0, Quaternion q1, float t) {

	        // Check for out-of range parameter and return edge points if so

	        if (t <= 0.0f) return q0;
	        if (t >= 1.0f) return q1;

	        // Compute "cosine of angle between quaternions" using dot product

	        float cosOmega = dotProduct(q0, q1);

	        // If negative dot, use -q1.  Two quaternions q and -q
	        // represent the same rotation, but may produce
	        // different slerp.  We chose q or -q to rotate using
	        // the acute angle.

	        float q1w = q1.w;
	        float q1x = q1.x;
	        float q1y = q1.y;
	        float q1z = q1.z;
	        if (cosOmega < 0.0f) {
		        q1w = -q1w;
		        q1x = -q1x;
		        q1y = -q1y;
		        q1z = -q1z;
		        cosOmega = -cosOmega;
	        }

	        // We should have two unit quaternions, so dot should be <= 1.0

            Trace.Assert(cosOmega < 1.1f, "error");

	        // Compute interpolation fraction, checking for quaternions
	        // almost exactly the same

	        float k0, k1;
	        if (cosOmega > 0.9999f) {

		        // Very close - just use linear interpolation,
		        // which will protect againt a divide by zero

		        k0 = 1.0f-t;
		        k1 = t;

	        } else {

		        // Compute the sin of the angle using the
		        // trig identity sin^2(omega) + cos^2(omega) = 1

		        float sinOmega = (float)Math.Sqrt(1.0f - cosOmega*cosOmega);

		        // Compute the angle from its sin and cosine

		        float omega = (float)Math.Atan2(sinOmega, cosOmega);

		        // Compute inverse of denominator, so we only have
		        // to divide once

		        float oneOverSinOmega = 1.0f / sinOmega;

		        // Compute interpolation parameters

		        k0 = (float)Math.Sin((1.0f - t) * omega) * oneOverSinOmega;
		        k1 = (float)Math.Sin(t * omega) * oneOverSinOmega;
	        }

	        // Interpolate

	        Quaternion result = new Quaternion();
	        result.x = k0*q0.x + k1*q1x;
	        result.y = k0*q0.y + k1*q1y;
	        result.z = k0*q0.z + k1*q1z;
	        result.w = k0*q0.w + k1*q1w;

	        // Return it

	        return result;
        }

        //---------------------------------------------------------------------------
        // conjugate
        //
        // Compute the quaternion conjugate.  This is the quaternian
        // with the opposite rotation as the original quaternian.  See 10.4.7

        public Quaternion conjugate(Quaternion q) {
	        Quaternion result = new Quaternion();

	        // Same rotation amount

	        result.w = q.w;

	        // Opposite axis of rotation

	        result.x = -q.x;
	        result.y = -q.y;
	        result.z = -q.z;

	        // Return it

	        return result;
        }

        //---------------------------------------------------------------------------
        // pow
        //
        // Quaternion exponentiation.
        //
        // See 10.4.12

        public Quaternion pow(Quaternion q, float exponent) {

	        // Check for the case of an identity quaternion.
	        // This will protect against divide by zero

	        if (Math.Abs(q.w) > .9999f) {
		        return q;
	        }

	        // Extract the half angle alpha (alpha = theta/2)

	        float	alpha = (float)Math.Acos(q.w);

	        // Compute new alpha value

	        float	newAlpha = alpha * exponent;

	        // Compute new w value

	        Quaternion result = new Quaternion();
	        result.w = (float)Math.Cos(newAlpha);

	        // Compute new xyz values

	        float	mult = (float)Math.Sin(newAlpha) / (float)Math.Sin(alpha);
	        result.x = q.x * mult;
	        result.y = q.y * mult;
	        result.z = q.z * mult;

	        // Return it

	        return result;
        }
    }
}
