using System;
using System.Collections.Generic;
using System.Text;
using FeInwork.Core.Util;
using FeInwork.FeInwork.util;
using Microsoft.Xna.Framework;

namespace FeInwork.Core.Interfaces
{
    public interface IPropertyContainer
    {
        /// <summary>
        /// Metodo para obtener una propiedad, SIEMPRE regresa algo, no puede regresar nulls 
        /// para evitar esto se valida en el metodo addComponent y change property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        T getProperty<T>(int property);

        /// <summary>
        /// Metodo para obtener una propiedad, SIEMPRE regresa algo, no puede regresar nulls 
        /// para evitar esto se valida en el metodo addComponent y change property.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        int getIntProperty(int property);

        /// <summary>
        /// Metodo para obtener una propiedad, SIEMPRE regresa algo, no puede regresar nulls 
        /// para evitar esto se valida en el metodo addComponent y change property.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        float getFloatProperty(int property);

        /// <summary>
        /// Metodo para obtener una propiedad, SIEMPRE regresa algo, no puede regresar nulls 
        /// para evitar esto se valida en el metodo addComponent y change property.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        bool getBoolProperty(int property);

        /// <summary>
        /// Metodo para obtener una propiedad, SIEMPRE regresa algo, no puede regresar nulls 
        /// para evitar esto se valida en el metodo addComponent y change property.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        Vector2 getVectorProperty(int property);

        /// <summary>
        /// Agrega una propiedad al diccionario y su valor inicial.
        /// NO debe ser null el valor por defecto.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        void addProperty<T>(int property, T value);

        /// <summary>
        /// Agrega una propiedad al diccionario y su valor inicial.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        void addIntProperty(int property, int value);

        /// <summary>
        /// Agrega una propiedad al diccionario y su valor inicial.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        void addFloatProperty(int property, float value);

        /// <summary>
        /// Agrega una propiedad al diccionario y su valor inicial.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        void addBoolProperty(int property, bool value);

        /// <summary>
        /// Agrega una propiedad al diccionario y su valor inicial.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        void addVectorProperty(int property, Vector2 value);

        /// <summary>
        /// Cambia el valor de una propiedad.
        /// Puede tirar una ArgumentNullException si el nuevo valor es nulo.
        /// Si la propiedad no existe se tira una excepcion, pues, toda propiedad que se usara DEBERA ser registrada
        /// al momento de la creacion de los componentes o en algun otro caso como agregar props dinamicamente.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName">Nombre de propiedad a modificar.</param>
        /// <param name="newValue">Nuevo valor a asignar.</param>
        /// <param name="notify">Lanza un evento a los interesados en esta propiedad.</param>
        void changeProperty<T>(int property, T newValue, bool notify);

        void changeIntProperty(int property, int newValue, bool notify);

        void changeFloatProperty(int property, float newValue, bool notify);

        void changeBoolProperty(int property, bool newValue, bool notify);

        void changeVectorProperty(int property, Vector2 newValue, bool notify);

        /// <summary>
        /// Remueve una propiedad del contenedor.
        /// Este metodo deberia ser llamado al destruir un componente o cuando cierto componente
        /// no usara mas una propiedad, sin embargo puede que otro componente este interesado un dicha propiedad
        /// y se debe validar para que no se elimine.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        bool removeProperty(int property);

        /// <summary>
        /// Revisa si el objeto tiene dicha propiedad, solo propiedades objeto que no sean Vector2!!
        /// </summary>
        /// <param name="entityState"></param>
        /// <returns></returns>
        bool containsProperty(int property);

        bool containsIntProperty(int property);

        bool containsFloatProperty(int property);

        bool containsBoolProperty(int property);

        bool containsVectorProperty(int property);

        /// <summary>a
        /// Devuelve una lista con todas las propiedades (sin valor)
        /// que tiene el contenedor
        /// </summary>
        /// <returns></returns>
        int[] getPropertyList();
    }
}
