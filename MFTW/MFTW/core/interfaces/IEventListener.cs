using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Base;
using FeInwork.Core.Util;

namespace FeInwork.Core.Interfaces
{
    // interface principal que deben implementar todos los listeners!
    public interface IEventListener
    {
        /// People, esta interface debe heredada por los listeners(otras interfaces) mas especificos que seran implementados por
        /// las clases que les interese escuchar cualquier evento.
        /// *****!! otra regla es que las interfaces que hereden a esta es decir los listeners mas concretos
        /// deben DEBEN DEbeNNNNNNNN tener por lo menos un metodo llamado "invoke" que reciba un parametro de algun derivado
        /// de IEvent/AbstractEvent.
        /// Ejemplo:
        /// La interface PropertyChangeListener hereda a esta y declara su respectivo metodo invoke que recibe el tipo de 
        /// evento que se supone ese listener debe escuchar... en este caso PropertyChangeEvent.
        /// Ver los ejemplos con Trance Mode tambien para mejor comprension.
        /// 
        /// BTWWWWWWWWWWW, por ahora cuando vayan a registrar un Listener en el EventManager con el metodod addlistener
        /// por favor estar seguro que el tipo de evento que pasan como primer parametro corresponde al tipo de evento 
        /// que el listener escucha y recibe en su metodo invoke!
    }
}
