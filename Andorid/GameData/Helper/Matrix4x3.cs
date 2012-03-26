/////////////////////////////////////////////////////////////////////////////
//
// 3D Math Primer for Games and Graphics Development
//
// Matrix4x3.h - Declarations for class Matrix4x3
//
// Visit gamemath.com for the latest version of this file.
//
// For more details, see Matrix4x3.cpp
//
/////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace GameData.Helper
{
    //---------------------------------------------------------------------------
    // class Matrix4x3
    //
    // Implement a 4x3 transformation matrix.  This class can represent
    // any 3D affine transformation.

    class Matrix4x3
    {
        /////////////////////////////////////////////////////////////////////////////
    //
    // Notes:
    //
    // See Chapter 11 for more information on class design decisions.
    //
    //---------------------------------------------------------------------------
    //
    // MATRIX ORGANIZATION
    //
    // The purpose of this class is so that a user might perform transformations
    // without fiddling with plus or minus signs or transposing the matrix
    // until the output "looks right."  But of course, the specifics of the
    // internal representation is important.  Not only for the implementation
    // in this file to be correct, but occasionally direct access to the
    // matrix variables is necessary, or beneficial for optimization.  Thus,
    // we document our matrix conventions here.
    //
    // We use row vectors, so multiplying by our matrix looks like this:
    //
    //               | m11 m12 m13 |
    //     [ x y z ] | m21 m22 m23 | = [ x' y' z' ]
    //               | m31 m32 m33 |
    //               | tx  ty  tz  |
    //
    // Strict adherance to linear algebra rules dictates that this
    // multiplication is actually undefined.  To circumvent this, we can
    // consider the input and output vectors as having an assumed fourth
    // coordinate of 1.  Also, since we cannot technically invert a 4x3 matrix
    // according to linear algebra rules, we will also assume a rightmost
    // column of [ 0 0 0 1 ].  This is shown below:
    //
    //                 | m11 m12 m13 0 |
    //     [ x y z 1 ] | m21 m22 m23 0 | = [ x' y' z' 1 ]
    //                 | m31 m32 m33 0 |
    //                 | tx  ty  tz  1 |
    //
    // In case you have forgotten your linear algebra rules for multiplying
    // matrices (which are described in section 7.1.6 and 7.1.7), see the
    // definition of operator* for the expanded computations.
    //
    /////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////
    //
    // Matrix4x3 class members
    //
    /////////////////////////////////////////////////////////////////////////////
        	    // The values of the matrix.  Basically the upper 3x3 portion
	    // contains a linear transformation, and the last row is the
	    // translation portion.  See the Matrix4x3.cpp for more
	    // details.

	    float	m11, m12, m13;
	    float	m21, m22, m23;
	    float	m31, m32, m33;
	    float	tx,  ty,  tz;
        public float M11{ get{ return m11;} set{ m11 = value; }}
        public float M12{ get{ return m12;} set{ m12 = value; }}
        public float M13{ get{ return m13;} set{ m13 = value; }}
        public float M21{ get{ return m21;} set{ m21 = value; }}
        public float M22{ get{ return m22;} set{ m22 = value; }}
        public float M23{ get{ return m23;} set{ m23 = value; }}
        public float M31{ get{ return m31;} set{ m31 = value; }}
        public float M32{ get{ return m32;} set{ m32 = value; }}
        public float M33{ get{ return m33;} set{ m33 = value; }}
        public float TX{ get{ return tx;} set{ tx = value; }}
        public float TY{ get{ return ty;} set{ ty = value; }}
        public float TZ { get{ return tz;} set{ tz = value; }}
    //---------------------------------------------------------------------------
    // identity
    //
    // Set the matrix to identity

    public void	identity() {
	    m11 = 1.0f; m12 = 0.0f; m13 = 0.0f;
	    m21 = 0.0f; m22 = 1.0f; m23 = 0.0f;
	    m31 = 0.0f; m32 = 0.0f; m33 = 1.0f;
	    tx  = 0.0f; ty  = 0.0f; tz  = 1.0f;
    }

    //---------------------------------------------------------------------------
    // zeroTranslation
    //
    // Zero the 4th row of the matrix, which contains the translation portion.

    public void	zeroTranslation() {
	    tx = ty = tz = 0.0f;
    }

    //---------------------------------------------------------------------------
    // setTranslation
    //
    // Sets the translation portion of the matrix in vector form

    public void	setTranslation(Vector3 d) {
	    tx = d.X; ty = d.Y; tz = d.Z;
    }

    //---------------------------------------------------------------------------
    // setTranslation
    //
    // Sets the translation portion of the matrix in vector form

    public void	setupTranslation(Vector3 d) {

	    // Set the linear transformation portion to identity

	    m11 = 1.0f; m12 = 0.0f; m13 = 0.0f;
	    m21 = 0.0f; m22 = 1.0f; m23 = 0.0f;
	    m31 = 0.0f; m32 = 0.0f; m33 = 1.0f;

	    // Set the translation portion

	    tx = d.X; ty = d.Y; tz = d.Z;
    }

    //---------------------------------------------------------------------------
    // setupLocalToParent
    //
    // Setup the matrix to perform a local -> parent transformation, given
    // the position and orientation of the local reference frame within the
    // parent reference frame.
    //
    // A very common use of this will be to construct a object -> world matrix.
    // As an example, the transformation in this case is straightforward.  We
    // first rotate from object space into inertial space, then we translate
    // into world space.
    //
    // We allow the orientation to be specified using either euler angles,
    // or a RotationMatrix

    public void	setupLocalToParent(Vector3 pos, EulerAngles orient) {

	    // Create a rotation matrix.

	    RotationMatrix orientMatrix = new RotationMatrix();
	    orientMatrix.setup(orient);

	    // Setup the 4x3 matrix.  Note: if we were really concerned with
	    // speed, we could create the matrix directly into these variables,
	    // without using the temporary RotationMatrix object.  This would
	    // save us a function call and a few copy operations.

	    setupLocalToParent(pos, orientMatrix);
    }

    public void	setupLocalToParent(Vector3 pos, RotationMatrix orient) {

	    // Copy the rotation portion of the matrix.  According to
	    // the comments in RotationMatrix.cpp, the rotation matrix
	    // is "normally" an inertial->object matrix, which is
	    // parent->local.  We want a local->parent rotation, so we
	    // must transpose while copying

	    m11 = orient.M11; m12 = orient.M21; m13 = orient.M31;
	    m21 = orient.M12; m22 = orient.M22; m23 = orient.M32;
	    m31 = orient.M13; m32 = orient.M23; m33 = orient.M33;

	    // Now set the translation portion.  Translation happens "after"
	    // the 3x3 portion, so we can simply copy the position
	    // field directly

	    tx = pos.X; ty = pos.Y; tz = pos.Z;
    }

    //---------------------------------------------------------------------------
    // setupParentToLocal
    //
    // Setup the matrix to perform a parent -> local transformation, given
    // the position and orientation of the local reference frame within the
    // parent reference frame.
    //
    // A very common use of this will be to construct a world -> object matrix.
    // To perform this transformation, we would normally FIRST transform
    // from world to inertial space, and then rotate from inertial space into
    // object space.  However, out 4x3 matrix always translates last.  So
    // we think about creating two matrices T and R, and then concatonating
    // M = TR.
    //
    // We allow the orientation to be specified using either euler angles,
    // or a RotationMatrix

    public void	setupParentToLocal(Vector3 pos, EulerAngles orient) {

	    // Create a rotation matrix.

	    RotationMatrix orientMatrix = new RotationMatrix();
	    orientMatrix.setup(orient);

	    // Setup the 4x3 matrix.

	    setupParentToLocal(pos, orientMatrix);
    }

    public void	setupParentToLocal(Vector3 pos, RotationMatrix orient) {

	    // Copy the rotation portion of the matrix.  We can copy the
	    // elements directly (without transposing) according
	    // to the layout as commented in RotationMatrix.cpp

	    m11 = orient.M11; m12 = orient.M12; m13 = orient.M13;
	    m21 = orient.M21; m22 = orient.M22; m23 = orient.M23;
	    m31 = orient.M31; m32 = orient.M32; m33 = orient.M33;

	    // Now set the translation portion.  Normally, we would
	    // translate by the negative of the position to translate
	    // from world to inertial space.  However, we must correct
	    // for the fact that the rotation occurs "first."  So we
	    // must rotate the translation portion.  This is the same
	    // as create a translation matrix T to translate by -pos,
	    // and a rotation matrix R, and then creating the matrix
	    // as the concatenation of TR

	    tx = -(pos.X*m11 + pos.Y*m21 + pos.Z*m31);
	    ty = -(pos.X*m12 + pos.Y*m22 + pos.Z*m32);
	    tz = -(pos.X*m13 + pos.Y*m23 + pos.Z*m33);
    }

    //---------------------------------------------------------------------------
    // setupRotate
    //
    // Setup the matrix to perform a rotation about a cardinal axis
    //
    // The axis of rotation is specified using a 1-based index:
    //
    //	1 => rotate about the x-axis
    //	2 => rotate about the y-axis
    //	3 => rotate about the z-axis
    //
    // theta is the amount of rotation, in radians.  The left-hand rule is
    // used to define "positive" rotation.
    //
    // The translation portion is reset.
    //
    // See 8.2.2 for more info.

    public void	setupRotate(int axis, float theta) {

	    // Get sin and cosine of rotation angle

	    float	s, c;
	    MathUtil.sinCos(out s, out c, theta);

	    // Check which axis they are rotating about

	    switch (axis) {

		    case 1: // Rotate about the x-axis

			    m11 = 1.0f; m12 = 0.0f; m13 = 0.0f;
			    m21 = 0.0f; m22 = c;    m23 = s;
			    m31 = 0.0f; m32 = -s;   m33 = c;
			    break;

		    case 2: // Rotate about the y-axis

			    m11 = c;    m12 = 0.0f; m13 = -s;
			    m21 = 0.0f; m22 = 1.0f; m23 = 0.0f;
			    m31 = s;    m32 = 0.0f; m33 = c;
			    break;

		    case 3: // Rotate about the z-axis

			    m11 = c;    m12 = s;    m13 = 0.0f;
			    m21 = -s;   m22 = c;    m23 = 0.0f;
			    m31 = 0.0f; m32 = 0.0f; m33 = 1.0f;
			    break;

		    default:

			    // bogus axis index

                Trace.Assert(false, "bogus axis index");
                break;
	    }

	    // Reset the translation portion

	    tx = ty = tz = 0.0f;
    }

    //---------------------------------------------------------------------------
    // setupRotate
    //
    // Setup the matrix to perform a rotation about an arbitrary axis.
    // The axis of rotation must pass through the origin.
    //
    // axis defines the axis of rotation, and must be a unit vector.
    //
    // theta is the amount of rotation, in radians.  The left-hand rule is
    // used to define "positive" rotation.
    //
    // The translation portion is reset.
    //
    // See 8.2.3 for more info.

    public void	setupRotate(Vector3 axis, float theta) {

	    // Quick sanity check to make sure they passed in a unit vector
	    // to specify the axis

        Trace.Assert(Math.Abs(axis * axis - 1.0f) < .01f, "error");

	    // Get sin and cosine of rotation angle

	    float	s, c;
	    MathUtil.sinCos(out s, out c, theta);

	    // Compute 1 - cos(theta) and some common subexpressions

	    float	a = 1.0f - c;
	    float	ax = a * axis.X;
	    float	ay = a * axis.Y;
	    float	az = a * axis.Z;

	    // Set the matrix elements.  There is still a little more
	    // opportunity for optimization due to the many common
	    // subexpressions.  We'll let the compiler handle that...

	    m11 = ax*axis.X + c;
	    m12 = ax*axis.Y + axis.Z*s;
	    m13 = ax*axis.Z - axis.Y*s;

	    m21 = ay*axis.X - axis.Z*s;
	    m22 = ay*axis.Y + c;
	    m23 = ay*axis.Z + axis.X*s;

	    m31 = az*axis.X + axis.Y*s;
	    m32 = az*axis.Y - axis.X*s;
	    m33 = az*axis.Z + c;

	    // Reset the translation portion

	    tx = ty = tz = 0.0f;
    }

    //---------------------------------------------------------------------------
    // fromQuaternion
    //
    // Setup the matrix to perform a rotation, given the angular displacement
    // in quaternion form.
    //
    // The translation portion is reset.
    //
    // See 10.6.3 for more info.

    public void	fromQuaternion(Quaternion q) {

	    // Compute a few values to optimize common subexpressions

	    float	ww = 2.0f * q.W;
	    float	xx = 2.0f * q.X;
	    float	yy = 2.0f * q.Y;
	    float	zz = 2.0f * q.Z;

	    // Set the matrix elements.  There is still a little more
	    // opportunity for optimization due to the many common
	    // subexpressions.  We'll let the compiler handle that...

	    m11 = 1.0f - yy*q.Y - zz*q.Z;
	    m12 = xx*q.Y + ww*q.Z;
	    m13 = xx*q.Z - ww*q.X;

	    m21 = xx*q.Y - ww*q.Z;
	    m22 = 1.0f - xx*q.X - zz*q.Z;
	    m23 = yy*q.Z + ww*q.X;

	    m31 = xx*q.Z + ww*q.Y;
	    m32 = yy*q.Z - ww*q.X;
	    m33 = 1.0f - xx*q.X - yy*q.Y;

	    // Reset the translation portion

	    tx = ty = tz = 0.0f;
    }

    //---------------------------------------------------------------------------
    // setupScale
    //
    // Setup the matrix to perform scale on each axis.  For uniform scale by k,
    // use a vector of the form Vector3(k,k,k)
    //
    // The translation portion is reset.
    //
    // See 8.3.1 for more info.

    public void	setupScale(Vector3 s) {

	    // Set the matrix elements.  Pretty straightforward

	    m11 = s.X;  m12 = 0.0f; m13 = 0.0f;
	    m21 = 0.0f; m22 = s.Y;  m23 = 0.0f;
	    m31 = 0.0f; m32 = 0.0f; m33 = s.Z;

	    // Reset the translation portion

	    tx = ty = tz = 0.0f;
    }

    //---------------------------------------------------------------------------
    // setupScaleAlongAxis
    //
    // Setup the matrix to perform scale along an arbitrary axis.
    //
    // The axis is specified using a unit vector.
    //
    // The translation portion is reset.
    //
    // See 8.3.2 for more info.

    public void	setupScaleAlongAxis(Vector3 axis, float k) 
    {

	    // Quick sanity check to make sure they passed in a unit vector
	    // to specify the axis

        Trace.Assert(Math.Abs(axis * axis - 1.0f) < .01f, "error");

	    // Compute k-1 and some common subexpressions

	    float	a = k - 1.0f;
	    float	ax = a * axis.X;
	    float	ay = a * axis.Y;
	    float	az = a * axis.Z;

	    // Fill in the matrix elements.  We'll do the common
	    // subexpression optimization ourselves here, since diagonally
	    // opposite matrix elements are equal

	    m11 = ax*axis.X + 1.0f;
	    m22 = ay*axis.Y + 1.0f;
	    m32 = az*axis.Z + 1.0f;

	    m12 = m21 = ax*axis.Y;
	    m13 = m31 = ax*axis.Z;
	    m23 = m32 = ay*axis.Z;

	    // Reset the translation portion

	    tx = ty = tz = 0.0f;
    }

    //---------------------------------------------------------------------------
    // setupShear
    //
    // Setup the matrix to perform a shear
    //
    // The type of shear is specified by the 1-based "axis" index.  The effect
    // of transforming a point by the matrix is described by the pseudocode
    // below:
    //
    //	axis == 1  =>  y += s*x, z += t*x
    //	axis == 2  =>  x += s*y, z += t*y
    //	axis == 3  =>  x += s*z, y += t*z
    //
    // The translation portion is reset.
    //
    // See 8.6 for more info.

    public void	setupShear(int axis, float s, float t) {

	    // Check which type of shear they want

	    switch (axis) {

		    case 1: // Shear y and z using x

			    m11 = 1.0f; m12 = s;    m13 = t;
			    m21 = 0.0f; m22 = 1.0f; m23 = 0.0f;
			    m31 = 0.0f; m32 = 0.0f; m33 = 1.0f;
			    break;

		    case 2: // Shear x and z using y

			    m11 = 1.0f; m12 = 0.0f; m13 = 0.0f;
			    m21 = s;    m22 = 1.0f; m23 = t;
			    m31 = 0.0f; m32 = 0.0f; m33 = 1.0f;
			    break;

		    case 3: // Shear x and y using z

			    m11 = 1.0f; m12 = 0.0f; m13 = 0.0f;
			    m21 = 0.0f; m22 = 1.0f; m23 = 0.0f;
			    m31 = s;    m32 = t;    m33 = 1.0f;
			    break;

		    default:

			    // bogus axis index

                Trace.Assert(false, "error");
                break;
	    }

	    // Reset the translation portion

	    tx = ty = tz = 0.0f;
    }

    //---------------------------------------------------------------------------
    // setupProject
    //
    // Setup the matrix to perform a projection onto a plane passing
    // through the origin.  The plane is perpendicular to the
    // unit vector n.
    //
    // See 8.4.2 for more info.

    public void	setupProject(Vector3 n) {

	    // Quick sanity check to make sure they passed in a unit vector
	    // to specify the axis

        Trace.Assert(Math.Abs(n * n - 1.0f) < .01f, "error");

	    // Fill in the matrix elements.  We'll do the common
	    // subexpression optimization ourselves here, since diagonally
	    // opposite matrix elements are equal

	    m11 = 1.0f - n.X*n.X;
	    m22 = 1.0f - n.Y*n.Y;
	    m33 = 1.0f - n.Z*n.Z;

	    m12 = m21 = -n.X*n.Y;
	    m13 = m31 = -n.X*n.Z;
	    m23 = m32 = -n.Y*n.Z;

	    // Reset the translation portion

	    tx = ty = tz = 0.0f;
    }

    //---------------------------------------------------------------------------
    // setupReflect
    //
    // Setup the matrix to perform a reflection about a plane parallel
    // to a cardinal plane.
    //
    // axis is a 1-based index which specifies the plane to project about:
    //
    //	1 => reflect about the plane x=k
    //	2 => reflect about the plane y=k
    //	3 => reflect about the plane z=k
    //
    // The translation is set appropriately, since translation must occur if
    // k != 0
    //
    // See 8.5 for more info.

    public void	setupReflect(int axis, float k) {

	    // Check which plane they want to reflect about

	    switch (axis) {

		    case 1: // Reflect about the plane x=k

			    m11 = -1.0f; m12 =  0.0f; m13 =  0.0f;
			    m21 =  0.0f; m22 =  1.0f; m23 =  0.0f;
			    m31 =  0.0f; m32 =  0.0f; m33 =  1.0f;

			    tx = 2.0f * k;
			    ty = 0.0f;
			    tz = 0.0f;

			    break;

		    case 2: // Reflect about the plane y=k

			    m11 =  1.0f; m12 =  0.0f; m13 =  0.0f;
			    m21 =  0.0f; m22 = -1.0f; m23 =  0.0f;
			    m31 =  0.0f; m32 =  0.0f; m33 =  1.0f;

			    tx = 0.0f;
			    ty = 2.0f * k;
			    tz = 0.0f;

			    break;

		    case 3: // Reflect about the plane z=k

			    m11 =  1.0f; m12 =  0.0f; m13 =  0.0f;
			    m21 =  0.0f; m22 =  1.0f; m23 =  0.0f;
			    m31 =  0.0f; m32 =  0.0f; m33 = -1.0f;

			    tx = 0.0f;
			    ty = 0.0f;
			    tz = 2.0f * k;

			    break;

		    default:

			    // bogus axis index

                Trace.Assert(false, "error");
                break;
	    }

    }

    //---------------------------------------------------------------------------
    // setupReflect
    //
    // Setup the matrix to perform a reflection about an arbitrary plane
    // through the origin.  The unit vector n is perpendicular to the plane.
    //
    // The translation portion is reset.
    //
    // See 8.5 for more info.

    public void	setupReflect(Vector3 n) {

	    // Quick sanity check to make sure they passed in a unit vector
	    // to specify the axis

        Trace.Assert(Math.Abs(n * n - 1.0f) < .01f, "error");

	    // Compute common subexpressions

	    float	ax = -2.0f * n.X;
	    float	ay = -2.0f * n.Y;
	    float	az = -2.0f * n.Z;

	    // Fill in the matrix elements.  We'll do the common
	    // subexpression optimization ourselves here, since diagonally
	    // opposite matrix elements are equal

	    m11 = 1.0f + ax*n.X;
	    m22 = 1.0f + ay*n.Y;
	    m32 = 1.0f + az*n.Z;

	    m12 = m21 = ax*n.Y;
	    m13 = m31 = ax*n.Z;
	    m23 = m32 = ay*n.Z;

	    // Reset the translation portion

	    tx = ty = tz = 0.0f;
    }

        //---------------------------------------------------------------------------
        // Vector * Matrix4x3
        //
        // Transform the point.  This makes using the vector class look like it
        // does with linear algebra notation on paper.
        //
        // We also provide a *= operator, as per C convention.
        //
        // See 7.1.7


        public static Vector3	operator*(Vector3 p, Matrix4x3 m) {

	        // Grind through the linear algebra.

	        return new Vector3(
		        p.X*m.m11 + p.Y*m.m21 + p.Z*m.m31 + m.TX,
		        p.X*m.m12 + p.Y*m.m22 + p.Z*m.m32 + m.TY,
		        p.X*m.m13 + p.Y*m.m23 + p.Z*m.m33 + m.TZ
	        );
        }

        //---------------------------------------------------------------------------
        // Matrix4x3 * Matrix4x3
        //
        // Matrix concatenation.  This makes using the vector class look like it
        // does with linear algebra notation on paper.
        //
        // We also provide a *= operator, as per C convention.
        //
        // See 7.1.6

        public static Matrix4x3 operator*(Matrix4x3 a, Matrix4x3 b) {

	        Matrix4x3 r = new Matrix4x3();

	        // Compute the upper 3x3 (linear transformation) portion

	        r.M11 = a.M11*b.M11 + a.M12*b.M21 + a.M13*b.M31;
	        r.M12 = a.M11*b.M12 + a.M12*b.M22 + a.M13*b.M32;
	        r.M13 = a.M11*b.M13 + a.M12*b.M23 + a.M13*b.M33;

	        r.M21 = a.M21*b.M11 + a.M22*b.M21 + a.M23*b.M31;
	        r.M22 = a.M21*b.M12 + a.M22*b.M22 + a.M23*b.M32;
	        r.M23 = a.M21*b.M13 + a.M22*b.M23 + a.M23*b.M33;

	        r.M31 = a.M31*b.M11 + a.M32*b.M21 + a.M33*b.M31;
	        r.M32 = a.M31*b.M12 + a.M32*b.M22 + a.M33*b.M32;
	        r.M33 = a.M31*b.M13 + a.M32*b.M23 + a.M33*b.M33;

	        // Compute the translation portion

	        r.TX = a.TX*b.M11 + a.TY*b.M21 + a.TZ*b.M31 + b.TX;
	        r.TY = a.TX*b.M12 + a.TY*b.M22 + a.TZ*b.M32 + b.TY;
	        r.TZ = a.TX*b.M13 + a.TY*b.M23 + a.TZ*b.M33 + b.TZ;

	        // Return it.  Ouch - involves a copy constructor call.  If speed
	        // is critical, we may need a seperate function which places the
	        // result where we want it...

	        return r;
        }

        //---------------------------------------------------------------------------
        // determinant
        //
        // Compute the determinant of the 3x3 portion of the matrix.
        //
        // See 9.1.1 for more info.

        float	determinant(Matrix4x3 m) {
	        return
		          m.M11 * (m.M22*m.M33 - m.M23*m.M32)
		        + m.M12 * (m.M23*m.M31 - m.M21*m.M33)
		        + m.M13 * (m.M21*m.M32 - m.M22*m.M31);
        }

        //---------------------------------------------------------------------------
        // inverse
        //
        // Compute the inverse of a matrix.  We use the classical adjoint divided
        // by the determinant method.
        //
        // See 9.2.1 for more info.

        Matrix4x3 inverse(Matrix4x3 m) {

	        // Compute the determinant

	        float	det = determinant(m);

	        // If we're singular, then the determinant is zero and there's
	        // no inverse

            Trace.Assert(Math.Abs(det) > 0.000001f, "error");

	        // Compute one over the determinant, so we divide once and
	        // can *multiply* per element

	        float	oneOverDet = 1.0f / det;

	        // Compute the 3x3 portion of the inverse, by
	        // dividing the adjoint by the determinant

	        Matrix4x3	r = new Matrix4x3();

	        r.M11 = (m.M22*m.m33 - m.M23*m.M32) * oneOverDet;
	        r.M12 = (m.M13*m.m32 - m.M12*m.M33) * oneOverDet;
	        r.M13 = (m.M12*m.m23 - m.M13*m.M22) * oneOverDet;

	        r.M21 = (m.M23*m.m31 - m.M21*m.M33) * oneOverDet;
	        r.M22 = (m.M11*m.m33 - m.M13*m.M31) * oneOverDet;
	        r.M23 = (m.M13*m.m21 - m.M11*m.M23) * oneOverDet;

	        r.M31 = (m.M21*m.m32 - m.M22*m.M31) * oneOverDet;
	        r.M32 = (m.M12*m.m31 - m.M11*m.M32) * oneOverDet;
	        r.M33 = (m.M11*m.m22 - m.M12*m.M21) * oneOverDet;

	        // Compute the translation portion of the inverse

	        r.tx = -(m.TX*r.M11 + m.TY*r.M21 + m.TZ*r.M31);
	        r.ty = -(m.TX*r.M12 + m.TY*r.M22 + m.TZ*r.M32);
	        r.tz = -(m.TX*r.M13 + m.TY*r.M23 + m.TZ*r.M33);

	        // Return it.  Ouch - involves a copy constructor call.  If speed
	        // is critical, we may need a seperate function which places the
	        // result where we want it...

	        return r;
        }

        //---------------------------------------------------------------------------
        // getTranslation
        //
        // Return the translation row of the matrix in vector form

        Vector3	getTranslation(Matrix4x3 m) {
	        return new Vector3(m.TX, m.TY, m.TZ);
        }

        //---------------------------------------------------------------------------
        // getPositionFromParentToLocalMatrix
        //
        // Extract the position of an object given a parent -> local transformation
        // matrix (such as a world -> object matrix)
        //
        // We assume that the matrix represents a rigid transformation.  (No scale,
        // skew, or mirroring)

        Vector3	getPositionFromParentToLocalMatrix(Matrix4x3 m) {

	        // Multiply negative translation value by the
	        // transpose of the 3x3 portion.  By using the transpose,
	        // we assume that the matrix is orthogonal.  (This function
	        // doesn't really make sense for non-rigid transformations...)

	        return new Vector3(
		        -(m.TX*m.M11 + m.TY*m.M12 + m.TZ*m.M13),
		        -(m.TX*m.M21 + m.TY*m.M22 + m.TZ*m.M23),
		        -(m.TX*m.M31 + m.TY*m.M32 + m.TZ*m.M33)
	        );
        }

        //---------------------------------------------------------------------------
        // getPositionFromLocalToParentMatrix
        //
        // Extract the position of an object given a local -> parent transformation
        // matrix (such as an object -> world matrix)

        Vector3	getPositionFromLocalToParentMatrix(Matrix4x3 m) {

	        // Position is simply the translation portion

	        return new Vector3(m.TX, m.TY, m.TZ);
        }
    }
}
