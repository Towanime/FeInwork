using System;
using System.Collections.Generic;
using System.Text;
using FeInwork.core.util;

namespace FeInwork.Core.Util
{
    public class EntityProperty
    {
        // coordenadas X y Y de una entidad en pantalla
        [Tipo("Vector2", false)]
        public const int Position = 0;
        // 
        [Tipo("float")]
        public const int Scale = 2;
        //
        [Tipo("int")]
        public const int Health = 3;

        [Tipo("int")]
        public const int HorizontalDirection = 4;
        /// <summary>
        /// Acción actual realizada
        /// </summary>
        [Tipo("string")]
        public const int Action = 5;

        [Tipo("int")]
        public const int VerticalDirection = 6;

        [Tipo("float")]
        public const int Rotation = 7;

        [Tipo("int")]
        public const int Age = 8;

        [Tipo("bool")]
        public const int Earth = 10;

        [Tipo("int")]
        public const int MaxDistance = 11;

        [Tipo("IEntity", false)]
        public const int ControllingAlchemy = 12;

        [Tipo("bool")]
        public const int Water = 13;

        [Tipo("int")]
        public const int PhysicalAttack = 14;

        [Tipo("int")]
        public const int PhysicalAttackBonus = 19;

        [Tipo("float")]
        public const int AffinityFire = 15;

        [Tipo("float")]
        public const int AffinityWater = 16;

        [Tipo("float")]
        public const int AffinityEarth = 17;

        [Tipo("float")]
        public const int AffinityWind = 16;

        [Tipo("float")]
        public const int DamageMultiplierBonus = 18;

        [Tipo("bool")]
        public const int Wind = 19;

        [Tipo("bool")]
        public const int Fire = 20;
    }
}