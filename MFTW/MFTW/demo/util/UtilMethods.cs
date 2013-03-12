using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FeInwork.FeInwork.util
{
    public static class UtilMethods
    {
        public static float wrapDegrees(float degrees)
        {
            while (degrees < 0) degrees += 360;
            while (degrees > 360) degrees -= 360;
            return degrees;
        }

        public static Vector2 angleToDirection(double angle)
        {
            return new Vector2((float)Math.Sin(angle), -(float)Math.Cos(angle));
        }

        public static double directionToAngle(ref Vector2 direction)
        {
            return Math.Atan2(direction.X, -direction.Y);
        }

        public static double limitAngle8Directions(double angle)
        {
            throw new NotImplementedException();
        }

        public static double limitAngle16Directions(double angle)
        {
            throw new NotImplementedException();
        }

        public static float logicToDrawAngle(double logicAngle)
        {
            float degrees = MathHelper.ToDegrees((float)(logicAngle));
            degrees -= 90;
            degrees = wrapDegrees(degrees);
            logicAngle = MathHelper.ToRadians(degrees);
            return (float)logicAngle;
        }

        public static double restrictAngleSide(double angle, bool right)
        {
            bool isFacingRight = isAngleFacingRight(angle);
            if ((right && !isFacingRight) 
                || !right && isFacingRight)
            {
                Vector2 direction = angleToDirection(angle);
                direction.X *= -1;
                angle = directionToAngle(ref direction);
            }
            return angle;
        }

        public static double restrictAngleBetween(double angle0, double angleA, double angleB)
        {
            Vector2 directionA = angleToDirection(angleA);
            Vector2 directionB = angleToDirection(angleB);
            double degreesAB = Math.Abs(degreesBetweenTwoVectors(directionA, directionB));
            if (degreesAB > 180) degreesAB = (360 - degreesAB);

            Vector2 direction0 = angleToDirection(angle0);
            double degrees0A = Math.Abs(degreesBetweenTwoVectors(direction0, directionA));
            if (degrees0A > 180) degrees0A = (360 - degrees0A);
            double degrees0B = Math.Abs(degreesBetweenTwoVectors(direction0, directionB));
            if (degrees0B > 180) degrees0B = (360 - degrees0B);

            if (degreesAB >= degrees0A && degreesAB >= degrees0B)
            {
                return angle0;
            }

            if (degrees0A < degrees0B) return angleA;
            else return angleB;
        }

        public static bool isAngleFacingRight(double logicAngle)
        {
            float degrees = MathHelper.ToDegrees((float)(logicAngle));
            degrees = wrapDegrees(degrees);
            return degrees == 360 || degrees <= 180;
        }

        public static bool isAngleFacingLeft(double logicAngle)
        {
            float degrees = MathHelper.ToDegrees((float)(logicAngle));
            degrees = wrapDegrees(degrees);
            return degrees == 0 || degrees >= 180;
        }

        public static double degreesBetweenTwoVectors(Vector2 vectorA, Vector2 vectorB)
        {
            return (Math.Atan2(vectorA.Y, vectorA.X) - Math.Atan2(vectorB.Y, vectorB.X)) * (180 / Math.PI);
        }

        public static Dictionary<ElementType, float> getGenericDamageMultipliers()
        {
            Dictionary<ElementType, float> elementMultipliers = new Dictionary<ElementType, float>();
            elementMultipliers.Add(ElementType.NONE, DamageMultipliers.NORMAL_DAMAGE);
            elementMultipliers.Add(ElementType.FIRE, DamageMultipliers.NORMAL_DAMAGE);
            elementMultipliers.Add(ElementType.WIND, DamageMultipliers.NORMAL_DAMAGE);
            elementMultipliers.Add(ElementType.WATER, DamageMultipliers.NORMAL_DAMAGE);
            elementMultipliers.Add(ElementType.EARTH, DamageMultipliers.NORMAL_DAMAGE);
            return elementMultipliers;
        }

        public static Dictionary<ElementType, float> getDamageMultipliers(float neutralMultiplier, 
            float fireMultiplier, float windMultiplier, float waterMultiplier, float earthMultiplier)
        {
            Dictionary<ElementType, float> elementMultipliers = new Dictionary<ElementType, float>();
            elementMultipliers.Add(ElementType.NONE, neutralMultiplier);
            elementMultipliers.Add(ElementType.FIRE, fireMultiplier);
            elementMultipliers.Add(ElementType.WIND, windMultiplier);
            elementMultipliers.Add(ElementType.WATER, waterMultiplier);
            elementMultipliers.Add(ElementType.EARTH, earthMultiplier);
            return elementMultipliers;
        }
    }
}
