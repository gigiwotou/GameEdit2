using System;
using System.Collections.Generic;
using System.Text;

namespace GameData.Helper
{
    class MathUtil
    {
        // Declare a global constant for pi and a few multiples.

        public const float kPi = 3.14159265f;
        public const float k2Pi = kPi * 2.0f;
        public const float kPiOver2 = kPi / 2.0f;
        public const float k1OverPi = 1.0f / kPi;
        public const float k1Over2Pi = 1.0f / k2Pi;
        public const float kPiOver180 = kPi / 180.0f;
        public const float k180OverPi = 180.0f / kPi;
        // Convert between degrees and radians

        public static float	degToRad(float deg) { return deg * kPiOver180; }
        public static float	radToDeg(float rad) { return rad * k180OverPi; }

        // Compute the sin and cosine of an angle.  On some platforms, if we know
        // that we need both values, it can be computed faster than computing
        // the two values seperately.

        public static void sinCos(out float returnSin, out float returnCos, float theta) {

	        // For simplicity, we'll just use the normal trig functions.
	        // Note that on some platforms we may be able to do better

	        returnSin = (float)Math.Sin(theta);
	        returnCos = (float)Math.Cos(theta);
        }

        // Convert between "field of view" and "zoom"  See section 15.2.4.
        // The FOV angle is specified in radians.

        public static float	fovToZoom(float fov) { return 1.0f / (float)Math.Tan(fov * .5f); }
        public static float	zoomToFov(float zoom) { return 2.0f * (float)Math.Atan(1.0f / zoom); }

        //const Vector3 kZeroVector(0.0f, 0.0f, 0.0f);

        //---------------------------------------------------------------------------
        // "Wrap" an angle in range -pi...pi by adding the correct multiple
        // of 2 pi

        public static float wrapPi(float theta) {
	        theta += kPi;
	        theta -= (float)Math.Floor(theta * k1Over2Pi) * k2Pi;
	        theta -= kPi;
	        return theta;
        }

        //---------------------------------------------------------------------------
        // safeAcos
        //
        // Same as acos(x), but if x is out of range, it is "clamped" to the nearest
        // valid value.  The value returned is in range 0...pi, the same as the
        // standard C acos() function

        public static float safeAcos(float x) 
        {

	        // Check limit conditions

	        if (x <= -1.0f) {
		        return kPi;
	        }
	        if (x >= 1.0f) {
		        return 0.0f;
	        }

	        // Value is in the domain - use standard C function

	        return (float)Math.Acos(x);
        }
    }
}
