using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.FeInwork.util;
using Microsoft.Xna.Framework;

namespace FeInwork.core.util
{
    public struct DialogParameters
    {
        /// <summary>
        /// Maximo de lineas a mostra en un cuadro de dialogo.
        /// !Implementado!
        /// </summary>
        private int maxLines;
        /// <summary>
        /// Alineacion horizontal de la caja de texto.
        /// !Implementado!
        /// </summary>
        private GameConstants.HorizontalAlign horizontalAlign;
        /// <summary>
        /// Alineacion vertical en pantalla para la caja de texto.
        /// !Implementado!
        /// </summary>
        private GameConstants.VerticalAlign verticalAlign;
        /// <summary>
        /// Para saber si el ancho del cuadro de texto debe ser relativo al ancho del texto en si o 
        /// si debe tener un ancho fijo al ser mostrado.
        /// Esto es bueno ponerlo true para pop ups y mensajes simples como un cartel centrado en pantalla etc.
        /// !Implementado!
        /// </summary>
        private bool isAdaptToText;
        /// <summary>
        /// Espacio entre la caja de text y el texto dentro de ella.
        /// !Implementado!
        /// </summary>
        private int horizontalMargin;
        /// <summary>
        /// Espacio vertical entre el texto y su caja.
        /// !Implementado!
        /// </summary>
        private int verticalMargin;
        /// <summary>
        /// Coordenada en X que se puede especificar manualmente.
        /// Solo se usa si el tipo de horizontal align es FreeX.
        /// !Implementado!
        /// </summary>
        private float freeX;
        /// <summary>
        /// Coordenada Y que se puede especificar manualmente.
        /// Solo se usa si el tipo de vertical align es FreeY.
        /// !Implementado!
        /// </summary>
        private float freeY;
        /// <summary>
        /// Si esta propiedad no esta en 0 entonces el dialogo actual espera la cantidad establecida
        /// y deberia bloquear el control para que se pueda continuar una vez haya pasadolos los segundos.
        /// </summary>
        private int blockSeconds;
        /// <summary>
        /// True si se quiere que luego del bloqueo la siguente parte del dialogo se cambie automaticamente.
        /// De lo contrario se tendria que presionar el botn para continuar.
        /// </summary>
        private bool isAutoNextAfterBlock;
        /// <summary>
        /// Strings de los asset a cargar para mostrar en esta parte del dialogo.
        /// Pueden ser varios y se acomodaran de acuerdo al array de posiciones!
        /// </summary>
        private string[] customAssets;
        /// <summary>
        /// Array que contiene las posiciones para cada elemento del array de customAssets.
        /// Si no existe un equivalente en este array entonces se usa la posicion defecto que es 
        /// a la izq.
        /// </summary>
        private Vector2[] assetsPositions;
        /// <summary>
        /// Especifica la cantidad de pixeles que aparecera sobre el cuadro de dialogo.
        /// Si es 0 entonces el DialogRenderer divide la img entre 2 y eso sera lo que se muestre.
        /// </summary>
        private float avatarPixelsToShow;
        /// <summary>
        /// Escala del texto.
        /// </summary>
        private float scale;

        /// <summary>
        /// Crea un struct de parametros con valores de defecto.
        /// </summary>
        /// <param name="fake">Este parametro no hace nada! soo es para crear el struct con ciertos valores de defecto</param>
        public DialogParameters(bool fake)
        {
            // defecto de align
            this.maxLines = 4;
            this.verticalAlign = GameConstants.VerticalAlign.Bottom;
            this.horizontalAlign = GameConstants.HorizontalAlign.Default;
            this.isAdaptToText = false;
            this.verticalMargin = 10;
            this.horizontalMargin = 10;
            this.freeX = 0f;
            this.freeY = 0f;
            this.blockSeconds = 0;
            this.isAutoNextAfterBlock = false;
            this.customAssets = null;
            this.assetsPositions = null;
            this.avatarPixelsToShow = 0f;
            this.scale = 1;
        }

        public int MaxLines { get { return this.maxLines; } set { this.maxLines = value; } }

        public GameConstants.HorizontalAlign HorizontalAlign { get { return this.horizontalAlign; } set { this.horizontalAlign = value; } }

        public GameConstants.VerticalAlign VerticalAlign { get { return this.verticalAlign; } set { this.verticalAlign = value; } }

        public bool IsAdaptToText { get { return this.isAdaptToText; } set { this.isAdaptToText = value; } }

        public int HorizontalMargin { get { return this.horizontalMargin; } set { this.horizontalMargin = value; } }

        public int VerticalMargin { get { return this.verticalMargin; } set { this.verticalMargin = value; } }

        public float FreeX { get { return this.freeX; } set { this.freeX = value; } }

        public float FreeY { get { return this.freeY; } set { this.freeY = value; } }

        public int BlockSeconds { get { return this.blockSeconds; } set { this.blockSeconds = value; } }

        public bool IsAutoNextAfterBlock { get { return this.isAutoNextAfterBlock; } set { this.isAutoNextAfterBlock = value; } }

        public float AvatarPixelsToShow { get { return this.avatarPixelsToShow; } set { this.avatarPixelsToShow = value; } }

        public float Scale { get { return this.scale; } set { this.scale = value; } }
    }
}
