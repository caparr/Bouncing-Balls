using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BouncingBalls
{
    /// <summary>
    /// This code is taken from the buildcurve sample code which uses Casteljau's algorithm to plot a curve given a set of points
    /// </summary>
    class CurvePlotting
    {
        int numberOfPoints;
        float totalLength;
        float[] normalizedDistanceTotal;

        public Vector3[] startPoints;
        public Vector3[] endPoints;
        public Vector3[] midPoints;
        public Casteljau[] casteljauPoints;

        public CurvePlotting(int aNumberOfPoints)
        {
            numberOfPoints = aNumberOfPoints;
            startPoints = new Vector3[numberOfPoints];
            endPoints = new Vector3[numberOfPoints];
            midPoints = new Vector3[numberOfPoints];
            casteljauPoints = new Casteljau[numberOfPoints - 1];
            normalizedDistanceTotal = new float[numberOfPoints];
            SetPoints();
        }
        
        
        private void SetPoints()
        {
            //  Randomnly generated start and end points
            //startPoints[0] = new Vector3((float)-24, (float)0, (float)0);
            //startPoints[1] = new Vector3((float)-15, (float)8, (float)0);
            //startPoints[2] = new Vector3((float)0, (float)8, (float)12);
            //startPoints[3] = new Vector3((float)8, (float)10, (float)12);
            //startPoints[4] = new Vector3((float)15, (float)0, (float)0);

            //endPoints[0] = new Vector3((float)-16, (float)0, (float)0);
            //endPoints[1] = new Vector3((float)-5, (float)15, (float)6);
            //endPoints[2] = new Vector3((float)0, (float)-8, (float)15);
            //endPoints[3] = new Vector3((float)15, (float)-10, (float)15);
            //endPoints[4] = new Vector3((float)25, (float)0, (float)0);

            for (int i = 0; i < numberOfPoints; i++)
            {
                startPoints[i] = new Vector3((float)Game1.randomNums.Next(40), (float)Game1.randomNums.Next(40), (float)Game1.randomNums.Next(40));
                endPoints[i] = new Vector3((float)Game1.randomNums.Next(40), (float)Game1.randomNums.Next(40), (float)Game1.randomNums.Next(40));
            }

            //  Finds all midpoints between start and end points
            for (int i = 0; i < numberOfPoints; i++)
            {
                midPoints[i] = Interpolate(startPoints[i], endPoints[i], 0.5f);
            }

            //  Determines the total length of the entire curve based on the length captured when applying Casteljau's algorithm to the 4 points
            for (int i = 0; i < casteljauPoints.Length; i++)
            {
                casteljauPoints[i] = new Casteljau(midPoints[i], endPoints[i], startPoints[i + 1], midPoints[i + 1]);
                totalLength += casteljauPoints[i].segmentLength;
            }

            
            //  Finds start point of each segment, 
            float cummulativeDistance = 0;
            normalizedDistanceTotal[0] = cummulativeDistance;

            for (int i = 0; i < casteljauPoints.Length; i++)
            {
                float normalizedLength = casteljauPoints[i].segmentLength / totalLength;
                cummulativeDistance += normalizedLength;
                normalizedDistanceTotal[i+1] = cummulativeDistance;
                casteljauPoints[i].normalLength = normalizedLength;

            }

        }

        public static Vector3 Interpolate(Vector3 startPoint, Vector3 endPoint, float t)
        {
            Vector3 point = startPoint + t * (endPoint - startPoint);
            return point;
        }

        public Vector3 GetPosition(float t)
        {
            int left = 0;
            int right = normalizedDistanceTotal.Length - 1;
            
            //  binary search to find out which segment the point is at
            while (right - left > 1)
            {
                int middle = (left + right) / 2;
                float d = normalizedDistanceTotal[middle];
                if (t > d)
                    left = middle;
                else
                    right = middle;
            }
            
            float r = (t - normalizedDistanceTotal[left]) / (normalizedDistanceTotal[right] - normalizedDistanceTotal[left]);
            return casteljauPoints[left].getPosition(r);            
        }


    }
}
