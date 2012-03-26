using System;
using System.Collections.Generic;
using System.Text;

namespace GameData.Helper
{
    //---------------------------------------------------------------------------
    // class EulerAngles
    //
    // This class represents a heading-pitch-bank Euler angle triple.
    class EulerAngles
    {
        // Straightforward representation.  Store the three angles, in
        // radians

        float heading;
        public float Heading { get{ return heading;} set {heading = value;}}
        float pitch;
        public float Pitch { get{return pitch;} set{pitch = value;}}
        float bank;
        public float Bank { get{return bank;} set{bank = value;}}

        // Set to identity triple (all zeros)

        void identity() { pitch = bank = heading = 0.0f; }

        /////////////////////////////////////////////////////////////////////////////
        //
        // Notes:
        //
        // See Chapter 11 for more information on class design decisions.
        //
        // See section 10.3 for more information on the Euler angle conventions
        // assumed.
        //
        /////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////
        //
        // global data
        //
        /////////////////////////////////////////////////////////////////////////////

        // The global "identity" Euler angle constant.  Now we may not know exactly
        // when this object may get constructed, in relation to other objects, so
        // it is possible for the object to be referenced before it is initialized.
        // However, on most implementations, it will be zero-initialized at program
        // startup anyway, before any other objects are constructed.

        //const EulerAngles kEulerAnglesIdentity(0.0f, 0.0f, 0.0f);

        /////////////////////////////////////////////////////////////////////////////
        //
        // class EulerAngles Implementation
        //
        /////////////////////////////////////////////////////////////////////////////

        //---------------------------------------------------------------------------
        // canonize
        //
        // Set the Euler angle triple to its "canonical" value.  This does not change
        // the meaning of the Euler angles as a representation of Orientation in 3D,
        // but if the angles are for other purposes such as angular velocities, etc,
        // then the operation might not be valid.
        //
        // See section 10.3 for more information.

        void	canonize() {

	        // First, wrap pitch in range -pi ... pi

	        pitch = MathUtil.wrapPi(pitch);

	        // Now, check for "the back side" of the matrix, pitch outside
	        // the canonical range of -pi/2 ... pi/2

	        if (pitch < -MathUtil.kPiOver2) {
		        pitch = -MathUtil.kPi - pitch;
		        heading += MathUtil.kPi;
		        bank += MathUtil.kPi;
	        } else if (pitch > MathUtil.kPiOver2) {
		        pitch = MathUtil.kPi - pitch;
		        heading += MathUtil.kPi;
		        bank += MathUtil.kPi;
	        }

	        // OK, now check for the gimbel lock case (within a slight
	        // tolerance)

	        if (Math.Abs(pitch) > MathUtil.kPiOver2 - 1e-4) {

		        // We are in gimbel lock.  Assign all rotation
		        // about the vertical axis to heading

		        heading += bank;
		        bank = 0.0f;

	        } else {

		        // Not in gimbel lock.  Wrap the bank angle in
		        // canonical range 

		        bank = MathUtil.wrapPi(bank);
	        }

	        // Wrap heading in canonical range

	        heading = MathUtil.wrapPi(heading);
        }

        //---------------------------------------------------------------------------
        // fromObjectToInertialQuaternion
        //
        // Setup the Euler angles, given an object->inertial rotation quaternion
        //
        // See 10.6.6 for more information.

        void	fromObjectToInertialQuaternion(Quaternion q) {

	        // Extract sin(pitch)

	        float sp = -2.0f * (q.Y*q.Z - q.W*q.X);

	        // Check for Gimbel lock, giving slight tolerance for numerical imprecision

	        if (Math.Abs(sp) > 0.9999f) {

		        // Looking straight up or down

		        pitch = MathUtil.kPiOver2 * sp;

		        // Compute heading, slam bank to zero

		        heading = (float)Math.Atan2(-q.X*q.Z + q.W*q.Y, 0.5f - q.Y*q.Y - q.Z*q.Z);
		        bank = 0.0f;

	        } else {

		        // Compute angles.  We don't have to use the "safe" asin
		        // function because we already checked for range errors when
		        // checking for Gimbel lock

		        pitch	= (float)Math.Asin(sp);
		        heading	= (float)Math.Atan2(q.X*q.Z + q.W*q.Y, 0.5f - q.X*q.X - q.Y*q.Y);
		        bank	= (float)Math.Atan2(q.X*q.Y + q.W*q.Z, 0.5f - q.X*q.X - q.Z*q.Z);
	        }
        }

        //---------------------------------------------------------------------------
        // fromInertialToObjectQuaternion
        //
        // Setup the Euler angles, given an inertial->object rotation quaternion
        //
        // See 10.6.6 for more information.

        void	fromInertialToObjectQuaternion(Quaternion q) {

	        // Extract sin(pitch)

	        float sp = -2.0f * (q.Y*q.Z + q.W*q.X);

	        // Check for Gimbel lock, giving slight tolerance for numerical imprecision

	        if (Math.Abs(sp) > 0.9999f) {

		        // Looking straight up or down

		        pitch = MathUtil.kPiOver2 * sp;

		        // Compute heading, slam bank to zero

		        heading = (float)Math.Atan2(-q.X*q.Z - q.W*q.Y, 0.5f - q.Y*q.Y - q.Z*q.Z);
		        bank = 0.0f;

	        } else {

		        // Compute angles.  We don't have to use the "safe" asin
		        // function because we already checked for range errors when
		        // checking for Gimbel lock

		        pitch	= (float)Math.Asin(sp);
		        heading	= (float)Math.Atan2(q.X*q.Z - q.W*q.Y, 0.5f - q.X*q.X - q.Y*q.Y);
		        bank	= (float)Math.Atan2(q.X*q.Y - q.W*q.Z, 0.5f - q.X*q.X - q.Z*q.Z);
	        }
        }

        //---------------------------------------------------------------------------
        // fromObjectToWorldMatrix
        //
        // Setup the Euler angles, given an object->world transformation matrix.
        //
        // The matrix is assumed to be orthogonal.  The translation portion is
        // ignored.
        //
        // See 10.6.2 for more information.

        void	fromObjectToWorldMatrix(Matrix4x3 m) {

	        // Extract sin(pitch) from m32.

	        float	sp = -m.M32;

	        // Check for Gimbel lock
	
	        if (Math.Abs(sp) > 9.99999f) {

		        // Looking straight up or down

		        pitch = MathUtil.kPiOver2 * sp;

		        // Compute heading, slam bank to zero

		        heading = (float)Math.Atan2(-m.M23, m.M11);
		        bank = 0.0f;

	        } else {

		        // Compute angles.  We don't have to use the "safe" asin
		        // function because we already checked for range errors when
		        // checking for Gimbel lock

		        heading = (float)Math.Atan2(m.M31, m.M33);
		        pitch = (float)Math.Asin(sp);
		        bank = (float)Math.Atan2(m.M12, m.M22);
	        }
        }

        //---------------------------------------------------------------------------
        // fromWorldToObjectMatrix
        //
        // Setup the Euler angles, given a world->object transformation matrix.
        //
        // The matrix is assumed to be orthogonal.  The translation portion is
        // ignored.
        //
        // See 10.6.2 for more information.

        void	fromWorldToObjectMatrix(Matrix4x3 m) {

	        // Extract sin(pitch) from m23.

	        float	sp = -m.M23;

	        // Check for Gimbel lock
	
	        if (Math.Abs(sp) > 9.99999f) {

		        // Looking straight up or down

		        pitch = MathUtil.kPiOver2 * sp;

		        // Compute heading, slam bank to zero

		        heading = (float)Math.Atan2(-m.M31, m.M11);
		        bank = 0.0f;

	        } else {

		        // Compute angles.  We don't have to use the "safe" asin
		        // function because we already checked for range errors when
		        // checking for Gimbel lock

		        heading = (float)Math.Atan2(m.M13, m.M33);
		        pitch = (float)Math.Asin(sp);
		        bank = (float)Math.Atan2(m.M21, m.M22);
	        }
        }

        //---------------------------------------------------------------------------
        // fromRotationMatrix
        //
        // Setup the Euler angles, given a rotation matrix.
        //
        // See 10.6.2 for more information.

        void	fromRotationMatrix(RotationMatrix m) {

	        // Extract sin(pitch) from m23.

	        float	sp = -m.M23;

	        // Check for Gimbel lock
	
	        if (Math.Abs(sp) > 9.99999f) {

		        // Looking straight up or down

		        pitch = MathUtil.kPiOver2 * sp;

		        // Compute heading, slam bank to zero

		        heading = (float)Math.Atan2(-m.M31, m.M11);
		        bank = 0.0f;

	        } else {

		        // Compute angles.  We don't have to use the "safe" asin
		        // function because we already checked for range errors when
		        // checking for Gimbel lock

		        heading = (float)Math.Atan2(m.M13, m.M33);
		        pitch = (float)Math.Asin(sp);
		        bank = (float)Math.Atan2(m.M21, m.M22);
	        }
        }
    }
}
