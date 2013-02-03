using Microsoft.Xna.Framework;

namespace BouncingBalls
{
    /// <summary>
    /// Parts of this class was taken from buildcurve.cpp sample code to help define the curve using Casteljau's algorithm
    /// </summary>
    class Casteljau
    {
        #region The 4 points needed to define a segment        
        private Vector3 p0; //  midPoint
        private Vector3 p1; //  endPoint
        private Vector3 p2; //  nextStartPoint
        private Vector3 p3; //  nextMidPoint
        #endregion

        public float segmentLength { get; private set; }
        public float normalLength { get; set; }

        public Casteljau(Vector3 midPoint, Vector3 endPoint, Vector3 nextStartPoint, Vector3 nextMidPoint)
        {
            p0 = midPoint;
            p1 = endPoint;
            p2 = nextStartPoint;
            p3 = nextMidPoint;
            getLength();
        }


        public Vector3 getPosition(float t)
        {
            Vector3 q0 = CurvePlotting.Interpolate(p0, p1, t);
            Vector3 q1 = CurvePlotting.Interpolate(p1, p2, t);
            Vector3 q2 = CurvePlotting.Interpolate(p2, p3, t);
            Vector3 r0 = CurvePlotting.Interpolate(q0, q1, t);
            Vector3 r1 = CurvePlotting.Interpolate(q1, q2, t);
            return CurvePlotting.Interpolate(r0, r1, t);

        }

        private void getLength()
        {
            int segmentDivisions = 100; // could increase/decrease this number for a more accurate length
            Vector3 pointA = p0;
            float distance = 0;
            for (int i = 1; i <= segmentDivisions; i++)
            {
                Vector3 pointB = getPosition((float)i / (float)segmentDivisions);
                distance += (pointB - pointA).Length();
                pointA = pointB;
            }

            segmentLength = distance;

        }



        


        
    }
}
