using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FeInwork.FeInwork.util
{
    public static class GameConstants
    {
        #region Alignments
        public enum HorizontalAlign
        {
            Left,
            HorizontalCenter,
            Right,
            FreeX,
            Default // default le da el comportamiento basico que es el cuadro a la izq
            //y que toma lo que puede de la ventana
        }

        public enum VerticalAlign
        {
            Top,
            VerticalCenter,
            Bottom,
            FreeY
        }
        #endregion

        #region GameValues
        public const float FOCUS_DAMAGE_MULTIPLIER = 1f;
        // velocidades de texto
        public const float TEXT_DIALOG_SPEED_LOW = 30;
        public const float TEXT_DIALOG_SPEED_MEDIUM = 15;
        public const float TEXT_DIALOG_SPEED_HIGH = 0;
        #endregion

        #region CollisionColorTags
        public static Color TANGIBLE_BODY_TAG = Color.Red;
        public static Color OPEN_DOOR_TAG = Color.GreenYellow;
        #endregion

        public const int BLOCK_SIZE = 46;

        public static int ROOM_OUTER_BORDER
        {
            get { return BLOCK_SIZE * 2; }
        }

        public static int CAMERA_INNER_BORDER
        {
            get { return 0; }
        }
    }

    public static class GameLayers
    {
        // 1f es el fondo hasta 0f que es el frente, todo lo demas se encuentra entre estos numeros
        // Se deja una diferencia de 0.2f entre cada capa just in case.
        public const float BACK_BACKGROUND = 1f;
        public const float MIDDLE_BACK_BACKGROUND = 0.7f;
        public const float MIDDLE_BACKGROUND = 0.5f;
        public const float MIDDLE_FRONT_BACKGROUND = 0.3f;
        public const float FRONT_BACKGROUND = 0.1f;
        // el area donde puede jugar se divide en tres tambien
        public const float BACK_PLAY_AREA = 0.4f;
        public const float MIDDLE_PLAY_AREA = 0.3f;
        public const float FRONT_PLAY_AREA = 0.2f;
        public const float TEXT_PLAY_AREA = 0.1f;
        // hud y mensajes?
        public const float BACK_HUD_AREA = 0.3f;
        public const float MIDDLE_HUD_AREA = 0.2F;
        public const float FRONT_HUD_AREA = 0.1f;
        public const float TEXT_HUD_AREA = 0.05f;
    }

    public static class GlobalIDs
    {
        public const string CAMERA_ENTITY_ID = "cameraEntity";
        public const string CAMERA_COLLISION_BODY_ID = "cameraCollisionBody";
        public const string MAIN_CHARACTER_ID = "mamaMia";
    }

    public static class GameAngles
    {
        public const double UPPER_ANGLE = Math.PI * 0;
        public const double UPPER_LEFT_ANGLE = Math.PI * 1.75;
        public const double LEFT_ANGLE = Math.PI * 1.5;
        public const double LOWER_LEFT_ANGLE = Math.PI * 1.25;
        public const double LOWER_ANGLE = Math.PI * 1;
        public const double LOWER_RIGHT_ANGLE = Math.PI * 0.75;
        public const double RIGHT_ANGLE = Math.PI * 0.5;
        public const double UPPER_RIGHT_ANGLE = Math.PI * 0.25;
    }

    public static class DamageMultipliers
    {
        public const float DOUBLE_HEAL = -2;
        public const float NORMAL_HEAL = -1;
        public const float LOW_HEAL = -0.5f;
        public const float NO_DAMAGE = 0;
        public const float LOW_DAMAGE = 0.5f;
        public const float NORMAL_DAMAGE = 1;
        public const float HIGH_DAMAGE = 2;
    }

    public static class AssetNames
    {
        public const string GOEMON_ASSET = "teststage/goemon";
        public const string WATERDROP_ASSET = "teststage/water_drop";
        public const string WATERBALL_ASSET = "teststage/water";
    }

    /// <summary>
    /// Enum de los tipos de alquimia
    /// Fuego  = 1
    /// Viento = 8
    /// Tierra = 2
    /// Agua = 4
    /// La alquimia se lograra con combinaciones de estos 4
    /// </summary>
    public enum ElementType
    {
        NONE = 0,
        FIRE = 1,
        WIND = 8,
        EARTH = 2,
        WATER = 4
    }

    public enum DrinkingType
    {
        REJUVENATION,
        AFFINITY_FIRE,
        AFFINITY_WATER,
        AFFINITY_EARTH,
        AFFINITY_WIND
    }
}
