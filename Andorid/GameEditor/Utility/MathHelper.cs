using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GameEditor.Utility
{
    public class MathHelper
    {
        //Compute the dot product AB ⋅ BC
        public static float Dot(PointF A, PointF B, PointF C)
        {
            PointF AB = B - new SizeF(A);
            PointF BC = C - new SizeF(B);
            return AB.X * BC.X + AB.Y * BC.Y;
        }

        //Compute the cross product AB x AC
        public static float Cross(PointF A, PointF B, PointF C)
        {
            PointF AB = B - new SizeF(A);
            PointF AC = C - new SizeF(A);
            return AB.X * AC.Y - AB.Y * AC.X;
        }

        //Compute the distance from A to B
        public static double DistanceSqr(PointF A, PointF B)
        {
            float d1 = A.X - B.X;
            float d2 = A.Y - B.Y;
            return d1 * d1 + d2 * d2;
        }

        //Compute the distance from A to B
        public static double Distance(PointF A, PointF B)
        {
            return Math.Sqrt(DistanceSqr(A, B));
        }

        //Compute the distance from AB to C
        //if isSegment is true, AB is a segment, not a line.
        public static double LinePointDistSqr(PointF A, PointF B, PointF C, bool isSegment)
        {
            if (isSegment)
            {
                if (Dot(A, B, C) > 0.0f)
                    return DistanceSqr(B, C);

                if (Dot(B, A, C) > 0.0f)
                    return DistanceSqr(A, C);
            }

            float c = Cross(A, B, C);
            return c * c / DistanceSqr(A, B);
        }

        public static double LinePointDist(PointF A, PointF B, PointF C, bool isSegment)
        {
            return Math.Sqrt(LinePointDistSqr(A, B, C, isSegment));
        }

        public static double AngleBetween(PointF A, PointF B, PointF C)
        {
            float c = Cross(A, B, C);
            double dis = Distance(A, B);
            double height = c / dis;
            return Math.Atan2(height, dis);
        }
    }
}
