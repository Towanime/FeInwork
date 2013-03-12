using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.core.Base;
using FeInwork.core.managers;
using Microsoft.Xna.Framework;
using FeInwork.Core.Base;
using Microsoft.Xna.Framework.Input;
using FeInwork.core.interfaces;
using FeInwork.core.renderers.util;
using Microsoft.Xna.Framework.Graphics;
using System.Text.RegularExpressions;
using FeInwork.FeInwork.util;

namespace FeInwork.core.util
{
    /// <summary>
    /// Clase que se encarga de manejar el flow de las conversaciones/dialogos del juego.
    /// Esta clase es un controller que itera sobre un array de lineas de dialogo que pueden tener
    /// muchos parametros diferentes.
    /// </summary>
    public class DialogManager : BaseControlComponent, IDialogProvider
    {
        /// <summary>
        /// Patron singleton
        /// </summary>
        private static DialogManager instance;
        /// <summary>
        /// Rectangulo donde se pueden dibujar las ventanas de texto.
        /// </summary>
        private Rectangle safeArea;
        /// <summary>
        /// Cantidad maxima de lineas a mostrar en un cuadro de texto.
        /// </summary>
        private int maxLines;
        /// <summary>
        /// Linea actual que se esta agregando al texto.
        /// </summary>
        private int currentLine;
        /// <summary>
        /// Parametros correspondientes al texto pasado.
        /// Se sacan cuando se encuentre un placeholder de la siguiente forma 
        /// $p1, esto sacara el item en el array -1! 
        /// </summary>
        private DialogParameters[] dialogParameters;
        /// <summary>
        /// Parametros a evaluar para un area de texto.
        /// </summary>
        private DialogParameters currentDialogParameter;
        // para saber que parametro de dialogo agarrar!
        // si es diferente de -1 se empieza otro cuadro de texto
        private int paramNumber = -1;
        /// <summary>
        /// Todo el texto que se dira en este dialogo, en este hay varios placeholders
        /// y escape character.
        /// </summary>
        private string dialogText;
        /// <summary>
        /// Font con la que se mostrara el texto, util aca para ver cuando miden las palabras
        /// antes de empezar a procesarlas.
        /// </summary>
        private SpriteFont font;
        /// <summary>
        /// Valor para saber cuando width se toma un espacio entre palabras.
        /// Podria se manejado mejor en float?
        /// </summary>
        private int spaceWidth;
        /// <summary>
        /// Para basarse en cuanto de alto tendra una linea!
        /// </summary>
        private int fontHeight;
        /// <summary>
        /// Para verificar si ya es necesario cambiar de linea!
        /// </summary>
        private int currentLineWidth;
        /// <summary>
        /// Arreglo con todas las palabras de un dialogo.
        /// Se recorreran una por una y luego a nivel de letra por letra
        /// pero se necesita calcular el width antes de intentar procesar la palabra
        /// para saber si cabe en lo ancho del cuadro de dialogo.
        /// </summary>
        private string[] words;
        /// <summary>
        /// para saber cuando verificar si agregar la palabra hace que se pase del
        /// safe area.
        /// </summary>
        private bool isWordTested;
        /// <summary>
        /// Index de la palabra que esta siendo procesada letra por letra.
        /// </summary>
        private int currentWordIndex;
        /// <summary>
        /// Letra a agregar al texto.
        /// </summary>
        private int currentChar;
        /// <summary>
        /// Especifica si en un update ya se puede agregar una letra al texto que se dibujara.
        /// </summary>
        private bool isReadyToMove;
        /// <summary>
        /// Para saber cuando se acaba  un dialogo!
        /// </summary>
        private bool isNoMoreWords;
        /// <summary>
        /// Esto es para saber cuando se han acabado 
        /// las lineas de un dialogo y no caben mas palabras.
        /// Si esta true entonces el player tiene que presionar
        /// "A" para continuar con la siguiente parte.
        /// </summary>
        private bool isWaitingInput;
        /// <summary>
        /// Texto a dibujar letra por letra.
        /// Al sacar este dato se coloca en false el valor paa mover a siguiente letra o palabra.
        /// </summary>
        private StringBuilder textToDraw;
        /// <summary>
        /// Valores extras a sumar cuando se valide el safe area y la palabra a ingresar.
        /// </summary>
        private int extraWidth;
        /// <summary>
        /// Para cambiar de letra por letra a palabra completa.
        /// </summary>
        private bool isFastForward;
        /// <summary>
        /// Para verificar si una palabra se ha terminado de imprimir.
        /// Se usa para controlar el fastforward! si es true entonces se puede 
        /// empezar a imprimir palabra por palabra en vez de letra por letra.
        /// </summary>
        private bool isWordCompleted;
        /// <summary>
        /// Para saber si llamar al metodo reconfig...
        /// </summary>
        private bool reconfig;
        /// <summary>
        /// True si el dialogo no puede aceptar input todavia.
        /// </summary>
        private bool isBlocked;
        /// <summary>
        /// Para ayuda a la logia de block.
        /// </summary>
        private Timer blockTimer;

        private DialogManager()
        {
            initialize();
        }

        public override void initialize()
        {
            isEnabled = false;
            textToDraw = new StringBuilder();
            safeArea = new Rectangle(25, 0, (int)Program.GAME.ResolutionWidth - 50,
                (int)Program.GAME.ResolutionHeight - 50);
            font = DialogRenderer.Instance.Font;
            spaceWidth = (int)font.MeasureString(" ").X;
            fontHeight = (int)font.MeasureString("Q").Y;
            blockTimer = new Timer();
        }

        public override void Update(GameTime gameTime)
        {
            if (isBlocked)
            {
                blockTimer.updateTime(gameTime);
                // verifica si ya pasaron esos segundos
                if (blockTimer.totalSecondsPassed(currentDialogParameter.BlockSeconds, true))
                {
                    isBlocked = false;
                    blockTimer.reset();
                    DialogRenderer.Instance.IsShowButtons = true;
                }
                else
                {
                    DialogRenderer.Instance.IsShowButtons = false;
                }
            }

            // velocidad!
            if (!isFastForward && InputManager.isNewPressKeyOrButton(Keys.A, Buttons.A))
            {
                isFastForward = true;
            }

            // verifica si ya termino este cuadro y espera presionar btn para el proximo
            if (!isBlocked && isWaitingInput)
            {
                if (currentDialogParameter.BlockSeconds > 0 &&
                               currentDialogParameter.IsAutoNextAfterBlock)
                {
                    if (!isNoMoreWords)
                    {
                        nextFragment();
                    }
                    else
                    {
                        endDialog();
                        return;
                    }
                }
                if (InputManager.isNewPressKeyOrButton(Keys.A, Buttons.A))
                {

                    if (!isNoMoreWords)
                    {
                        nextFragment();
                    }
                    else
                    {
                        endDialog();
                        return;
                    }
                    /*if (reconfig)
                    {
                        currentDialogParameter = dialogParameters[paramNumber];
                        reconfigForParameter();
                    }*/

                }
            }

            // solo empieza a meter letras o palabras si puede y no esta esperando por input para continuar
            if (!isWaitingInput && isReadyToMove)
            {
                // verifica si la palabra ya se verifico que cabe en esta linea!
                if (!isWordTested)
                {
                    // margenes y espacio en blanco, que mas?
                    // check this!

                    Vector2 size = font.MeasureString(words[currentWordIndex]);
                    if (currentLineWidth + extraWidth + size.X <
                        safeArea.Width - currentDialogParameter.HorizontalMargin)
                    {
                        isWordTested = true;
                        currentLineWidth += (int)size.X + spaceWidth;
                        extraWidth = 0;
                    }
                    else
                    {
                        // si ya no cabe pasar de linea e incrementar
                        // verificar lineas maxima a ver si puede agregar una nueva!
                        if (currentLine + 1 > maxLines)
                        {

                            // {
                            // si no se puede agregar then se pone a espera por input
                            isWaitingInput = true;
                            return;
                            // }
                        }
                        else
                        {
                            nextLine();
                            return;
                        }
                    }
                }

                // check for parameter!
                if (checkForParameter(words[currentWordIndex]))
                {
                    return;
                }

                // verifica si por palabra o letra
                if (isWordCompleted && isFastForward)
                {
                    // simplemente agrega palabra coz ya esta probada :D
                    textToDraw.Append(words[currentWordIndex] + " ");
                    isWordTested = false;
                    //currentChar = 0;
                    extraWidth += spaceWidth;
                    // ver si termina
                    evaluateEnd();
                }
                else
                {
                    // ahora letra por letra
                    if (currentChar == words[currentWordIndex].Length)
                    {
                        // si ya esta en ultimo char de la palabra then cambia el index!
                        textToDraw.Append(" ");
                        isWordTested = false;
                        currentChar = 0;
                        extraWidth += spaceWidth;
                        isWordCompleted = true;
                        // ver si termina
                        evaluateEnd();
                    }
                    else
                    {
                        // verifica si el char que corresponde no es especial!
                        // primero salto de linea
                        if (words[currentWordIndex][currentChar] == '\n')
                        {
                            textToDraw.Append("\n");
                            currentLineWidth = 0;
                            currentLine++;
                            currentWordIndex++;
                            currentChar++;
                        }
                        else
                        {
                            // si no es el ultimo then se agrega a texto!
                            textToDraw.Append(words[currentWordIndex][currentChar]);
                            extraWidth = 0;
                            currentChar++;
                            isWordCompleted = false;
                        }
                    }
                }
            }
        }

        private void nextLine()
        {
            textToDraw.Append("\n");
            currentLineWidth = 0;
            extraWidth = 0;
            currentLine++;
        }

        private void nextFragment()
        {
            // si hay mas plabras entonces se reinicia el cuadro de dialogo
            //para mostrar lo que venga
            textToDraw.Remove(0, textToDraw.Length);
            isWaitingInput = false;
            currentLine = 1;
            currentLineWidth = 0;
            isFastForward = false;
            isWordTested = false;
            isWordCompleted = false;
            // reconfigurar if
            if (reconfig)
            {
                currentDialogParameter = dialogParameters[paramNumber];
                reconfigForParameter();
                reconfig = false;
            }
        }

        private void evaluateEnd()
        {
            // si ya no tiene mas palabras entonces esto muere!
            if (currentWordIndex + 1 == words.Length)
            {
                isNoMoreWords = true;
                isWaitingInput = true;
            }
            else
            {
                currentWordIndex++;
            }
        }

        public void beginDialog(string text, DialogParameters[] dialog)
        {
            this.dialogParameters = dialog;
            this.currentDialogParameter = new DialogParameters(false);// temporal!!!!!!!! 
            reconfigForParameter();
            this.dialogText = Regex.Replace(text, @"\s+", " ");
            // separ por palabras y placeholders
            this.words = dialogText.Split(' ');
            // reset de indexes
            this.currentLineWidth = 0;
            this.currentWordIndex = 0;
            this.currentChar = 0;
            this.isWordTested = false;
            this.currentLine = 1;
            this.isReadyToMove = true;
            this.isFastForward = false;
            this.maxLines = currentDialogParameter.MaxLines;
            DialogRenderer.Instance.CurrentDialogProvider = this;
            ControlManager.Instance.addController(this);
        }

        // por ahora privado?
        private void endDialog()
        {
            dialogParameters = null;
            DialogRenderer.Instance.CurrentDialogProvider = null;
            textToDraw.Remove(0, textToDraw.Length);
            words = null;
            dialogText = null;
            isNoMoreWords = false;
            isWaitingInput = false;
            isWordCompleted = false;
            ControlManager.Instance.removeController(this);
        }

        public DialogParameters getCurrentDialogParameter()
        {
            return currentDialogParameter;
        }

        /// <summary>
        /// Verifica una palabra para ver si encuentra un parametro 
        /// que tiene que sacar y establecer en el cuadro de dialogo que siga.
        /// Posibles valores:
        /// $n (N= index del parametro a sacar del array de parametros)
        /// $e = Termina el cuadro de dialogo actual inmediatamente y lo hace esperar por input del usuario. 
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private bool checkForParameter(string word)
        {
            if (word[0] == '$' && word.Length > 1)
            {
                if (int.TryParse(word.Substring(1, word.Length - 1), out paramNumber))
                {
                    // verifica si el primer char es $
                    // ya se tiene el numero here encontes se saca el parametro del array
                    // se pone como current y se inicia un nuevo cuadro de texto!
                    // pero primero verifica si tiene ese parametro si no tira excepcion :/
                    if (paramNumber > dialogParameters.Length)
                    {
                        throw new Exception("Número de parametro para el dialogo no se encuentra. (IndexOutOfBoundsEx)");
                    }
                    //currentDialogParameter = dialogParameters[paramNumber];
                    // vuelve a crear ciertos objetos de acuerdo a un nuevo parametro de dialogo
                    currentChar = 0;
                    currentWordIndex++;
                    reconfig = true;
                    isWaitingInput = true;
                    //
                    return true;
                }
                else
                {
                    // si no es un numero entonces deberia ser un parametro de formato
                    // aqui usar returns en vez de break i guess
                    switch (word.Substring(1, word.Length - 1))
                    {
                        case "e": // fin del cuadro actual y espera por input
                            currentChar = 0;
                            currentWordIndex++;
                            isWaitingInput = true;
                            //
                            return true;

                        case "n": // salto de linea
                            currentChar = 0;
                            currentWordIndex++;
                            nextLine();
                            return true;

                        default:
                            return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Agarra el parametro actual y vuelve a crear obteos utiles
        /// de acuerdo al nuevo parametro...
        /// Como por ejemplo, si el dialogo tenia horizontal a la izq y ahora
        /// esta en el centro, se debe crear un nuevo safeArea
        /// </summary>
        private void reconfigForParameter()
        {
            // safearea
            Rectangle newSafeArea = new Rectangle();

            // verifica si debe ser bloqueado el dialogo
            if (currentDialogParameter.BlockSeconds > 0)
            {
                isBlocked = true;
            }

            if (currentDialogParameter.HorizontalAlign == GameConstants.HorizontalAlign.Default)
            {
                // default!
                // el width sera lo que resta de pantalla
                newSafeArea.Width = (int)Program.GAME.ResolutionWidth - 50;
                // new X
                newSafeArea.X = 25;
            }
            else if (currentDialogParameter.HorizontalAlign == GameConstants.HorizontalAlign.HorizontalCenter)
            {
                // si es en el centro entonces el cuadro no debe salirse de 
                // las mitades de ambos extremos :D
                newSafeArea.Width = ((int)Program.GAME.ResolutionWidth / 2)
                    + (currentDialogParameter.HorizontalMargin * 2);
                // new X
                newSafeArea.X = ((int)Program.GAME.ResolutionWidth / 4)
                    - currentDialogParameter.HorizontalMargin;
            }
            else if (currentDialogParameter.HorizontalAlign == GameConstants.HorizontalAlign.Left)
            {
                // el width sera HASTA la mitad de la pantalla
                newSafeArea.Width = ((int)Program.GAME.ResolutionWidth / 2)
                    + (currentDialogParameter.HorizontalMargin * 2);
                // new X
                newSafeArea.X = 25;
            }
            else if (currentDialogParameter.HorizontalAlign == GameConstants.HorizontalAlign.Right)
            {
                // el width sera la mitad de la pantalla
                newSafeArea.Width = ((int)Program.GAME.ResolutionWidth / 2)
                    + (currentDialogParameter.HorizontalMargin * 2);
                // new X
                newSafeArea.X = ((int)Program.GAME.ResolutionWidth / 2)
                    - (currentDialogParameter.HorizontalMargin * 4);
            }
            else
            {
                // else es free!
                if (currentDialogParameter.FreeX != 0)
                {
                    // no valida safe area ni nada
                    newSafeArea.X = (int)currentDialogParameter.FreeX;
                    // el width sera lo que queda de la pantalla
                    newSafeArea.Width = ((int)Program.GAME.ResolutionWidth - 50) - newSafeArea.X -
                        -(currentDialogParameter.HorizontalMargin * 2);
                }
                else
                {
                    // si es cero entonces se toma posicion defecto
                    newSafeArea.X = 25;
                    newSafeArea.Width = (int)Program.GAME.ResolutionWidth - 50;
                }
            }

            // ahora vertical!
            //vertical values now 
            if (currentDialogParameter.VerticalAlign == GameConstants.VerticalAlign.Top)
            {
                newSafeArea.Y = 25;
            }
            else if (currentDialogParameter.VerticalAlign == GameConstants.VerticalAlign.VerticalCenter)
            {
                // new Y
                int y = (int)Program.GAME.ResolutionHeight / 4;
                newSafeArea.Y = (y + (y / 2))
                    - currentDialogParameter.VerticalMargin;
            }
            else if (currentDialogParameter.VerticalAlign == GameConstants.VerticalAlign.Bottom)
            {
                // bottom
                newSafeArea.Y = (int)Program.GAME.ResolutionHeight - 50 - (fontHeight * currentDialogParameter.MaxLines);
            }
            else
            {
                // else es free!
                if (currentDialogParameter.FreeY != 0)
                {
                    // no valida safe area ni nada
                    newSafeArea.Y = (int)currentDialogParameter.FreeY;
                }
                else
                {
                    // si es cero entonces se toma posicion defecto
                    newSafeArea.Y = 25;
                }
            }

            safeArea = newSafeArea;
        }

        public static DialogManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DialogManager();
                }
                return instance;
            }
        }


        #region Properties

        public Rectangle SafeArea
        {
            get { return safeArea; }
            set { safeArea = value; }
        }

        public StringBuilder TextToDraw
        {
            get
            {
                this.isReadyToMove = true;
                return textToDraw;
            }
        }

        #endregion

    }
}
