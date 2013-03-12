using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FeInwork.core.physics
{
    public struct PhysicsData
    {
        private Vector2 minimumVelocity;
        private Vector2 maximumVelocity;
        private float weight;
        private float runningImpulse;
        private float airImpulse;
        private float jumpImpulse;

        public PhysicsData(Vector2 minimumVelocity, Vector2 maximumVelocity, float weight, float runningImpulse, float airImpulse, float jumpImpulse)
        {
            this.minimumVelocity = minimumVelocity;
            this.maximumVelocity = maximumVelocity;
            this.weight = weight;
            this.runningImpulse = runningImpulse;
            this.airImpulse = airImpulse;
            this.jumpImpulse = jumpImpulse;
        }

        public Vector2 MinimumVelocity
        {
            get { return minimumVelocity; }
            set { minimumVelocity = value; }
        }

        public Vector2 MaximumVelocity
        {
            get { return maximumVelocity; }
            set { maximumVelocity = value; }
        }

        public float Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        public float RunningImpulse
        {
            get { return runningImpulse; }
            set { runningImpulse = value; }
        }

        public float AirImpulse
        {
            get { return airImpulse; }
            set { airImpulse = value; }
        }

        public float JumpImpulse
        {
            get { return jumpImpulse; }
            set { jumpImpulse = value; }
        }
        
    }
}
