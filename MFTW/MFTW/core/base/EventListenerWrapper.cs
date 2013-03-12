using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Interfaces;

namespace FeInwork.core.Base
{
    // wrapper que contiene un listener y un campo id para la entidad especifica que debe escuchar
	public class EventListenerWrapper
	{
        /// <summary>
        /// Si este valor es null todos los eventos del tipo que escuche este listener seran enviados.
        /// Si el valor es el id de una entidad entonces se verifica al momento de enviar los eventos 
        /// y si corresponde se llama al listener guardado aca.
        /// *** 
        /// En el caso de eventos de colision este id es el de un objeto de colision especifico,
        /// puede ser un brazo o simplemente nada y escucha todos los eventos de cierto tipo
        /// </summary>
        public string entityToListenId;
        public string ownerEntityId;
        /// <summary>
        /// Listener a invocar ya este filtrado o no
        /// </summary>
        public IEventListener listener;

        /// <summary>
        /// True si el listener asociado quiere escuchar eventos en un momento determinado.
        /// False para no enviar un nuevo evento a este listener hasta que alguien lo active nuevamente.
        /// </summary>
        public bool isActivated;

        /// <summary>
        /// NO UTILIZAR ESTOS OBJETOS DIRECTAMENTE! - DEJARLOS AL MANEJADOR DE EVENTOS - 
        /// Este contructor asume que no se filtrara para cierta entidad!
        /// es decir todos los eventos que se originen que le interesen a este listener 
        /// seran escuchado no importa de donde vengan!
        /// </summary>
        /// <param name="listener"></param>
        public EventListenerWrapper(IEventListener listener, string ownerEntityId)
        {
            this.listener = listener;
            this.ownerEntityId = ownerEntityId;
        }

        /// <summary>
        /// Este constructor recibe el id de la entidad a la que se le quieren escuchar los 
        /// eventos.
        /// </summary>
        /// <param name="listener"></param>
        /// <param name="entityToListenId"></param>
        public EventListenerWrapper(IEventListener listener, string ownerEntityId, string entityToListenId)
        {
            this.listener = listener;
            this.ownerEntityId = ownerEntityId;
            this.entityToListenId = entityToListenId;
        }

        public void reset()
        {
            this.listener = null;
            this.ownerEntityId = null;
            this.entityToListenId = null;
        }

        /// <summary>
        /// Si este valor es null todos los eventos del tipo que escuche este listener seran enviados.
        /// Si el valor es el id de una entidad entonces se verifica al momento de enviar los eventos 
        /// y si corresponde se llama al listener guardado aca.
        /// *** 
        /// En el caso de eventos de colision este id es el de un objeto de colision especifico,
        /// puede ser un brazo o simplemente nada y escucha todos los eventos de cierto tipo
        /// </summary>
        /*public string EntityToListenId 
        {
            get { return this.entityToListenId; }
            set { this.entityToListenId = value; }
        }

        public string OwnerEntityId
        {
            get { return this.ownerEntityId; }
            set { this.ownerEntityId = value; }
        }

        /// <summary>
        /// Listener a invocar ya este filtrado o no
        /// </summary>
        public IEventListener Listener
        {
            get { return this.listener; }
            set { this.listener = value; }
        }

        /// <summary>
        /// Activa o desactiva un listener para recibir o no eventos.
        /// </summary>
        public bool IsActivated
        {
            set { this.isActivated = true; }
            get { return isActivated; }
        }*/
	}
}
