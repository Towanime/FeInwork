using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.core.util;
using Microsoft.Xna.Framework;

namespace FeInwork.core.interfaces
{
    /// <summary>
    /// Interface a implementar por los diferentes clases que seran renderizadas para dialogo.
    /// De esta manera se puede reutilizar el renderer existente.
    /// No puede haber mas de un IDialogProvider acivo al mismo tiempo a menos que se implemente otro renderer aparte
    /// pero el que existe es un singleton (DialogRenderer) ya que usa los mismos datos no hay necesidad de crear un rederer 
    /// diferente para cada IDialogProvider.
    /// </summary>
    public interface IDialogProvider
    {
        /// <summary>
        /// Inicializa el manejo de un dialogo con el index en 0 generalmente. Ver DialogManager.cs
        /// </summary>
        /// <param name="dialog"></param>
        void beginDialog(string text, DialogParameters[] dialog);

        /// <summary>
        /// Utilizado por el DialogRenderer para saber que parte del dialogo debe dibujar y sus parametros.
        /// </summary>
        /// <returns></returns>
        DialogParameters getCurrentDialogParameter();

        /// <summary>
        /// Necesaria para saber cuando dibujar o no el texto.
        /// </summary>
        bool Enabled { set; get; }

        Rectangle SafeArea { get; }
    }
}
