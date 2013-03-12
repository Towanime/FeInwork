using System;
using System.Collections.Generic;
using System.Text;
using FeInwork.Core.Util;
using FeInwork.FeInwork.util;

namespace FeInwork.Core.Interfaces
{
    public interface IStateContainer
    {
        /// <summary>
        /// Agrega un nuevo estado a una entidad que por defecto esta inactivo (false).
        /// Si se intenta agregar un estado que ya existe simplemente se ignora 
        /// pero si se desea un nuevo valor utilizar el otro metodo para "setearlo"
        /// </summary>
        /// <param name="state">Estado a registrar</param>
        void addState(int state);

        /// <summary>
        /// Agrega un nuevo estado a una entidad y asigna el valor inicial.
        /// Si se intenta agregar un estado que ya existe simplemente se ignora 
        /// pero si se desea un nuevo valor utilizar el otro metodo para "setearlo"
        /// </summary>
        /// <param name="state">Estado a registrar</param>
        /// <param name="value">Valor inical del estado para la entidad</param>
        void addState(int state, bool value);

        /// <summary>
        /// Devuelve el valor del estado indicado al llamar al metodo.
        /// </summary>
        /// <param name="state">Estado a buscar</param>
        /// <returns>True = Estado activo.
        /// False = Estado inactivo.</returns>
        bool getState(int state);

        /// <summary>
        /// Cambia el valor actual de un estado al indicado.
        /// </summary>
        /// <param name="state">Estado a cambiar</param>
        /// <param name="newValue">Nuevo valor del estado indicado</param>
        /// <param name="notify">True para enviar evento al tener nuevo valor unicamente.</param>
        void changeState(int state, bool newValue, bool notify);

        /// <summary>
        /// Remueve un estado de la entidad y envia evento indicado.
        /// 
        /// Not sure si se deberia elminar y ya o verificar dependencias.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        bool removeState(int state);

        /// <summary>
        /// Revisa si el objeto tiene dicho estado
        /// </summary>
        /// <param name="entityState"></param>
        /// <returns></returns>
        bool containsState(int entityState);

        /// <summary>
        /// Devuelve una lista con todos los estados (sin valor)
        /// que tiene el contenedor
        /// </summary>
        /// <returns></returns>
        int[] getStateList();
    }
}
