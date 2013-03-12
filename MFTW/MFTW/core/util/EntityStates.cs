using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FeInwork.FeInwork.util
{
    public class EntityState
    {
        #region Generales.
        // indica si la entidad esta en modo trance 
        public const int TranceMode = 1;
        // indica si esta disponible par realizar acciones 
        public const int IsAvailable = 2;
        public const int OnlyTranceVisible = 3;
        public const int IsCasting = 4;
        #endregion

        #region Fisicas
        // indica si esta en el aire
        public const int OnAir = 101;
        public const int isGrabbed = 102;
        #endregion Fisicas

        #region Actor
        // indica si esta en el aire
        public const int Attacking = 201;
        public const int Dead = 202;
        public const int Invincible = 203;
        public const int Grabbing = 204;
        public const int Running = 205;
        public const int Focusing = 206;
        public const int Drinking = 207;
        #endregion Actor

        #region Drawing
        // indica si esta visible o no
        public const int Visible = 301;
        public const int FacingRight = 302;
        public const int Halo = 303;
        public const int Weak = 304;
        #endregion Drawing                

        #region Etiquetas
        public const int WaterDrop = 401;
        #endregion Etiquetas
    }
}
