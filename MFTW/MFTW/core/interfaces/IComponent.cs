using System;
using System.Collections.Generic;
using System.Text;

namespace FeInwork.Core.Interfaces
{
	public interface IComponent
	{
        /// <summary>
        /// Metodo para inicializar valores y otras cosas basicas del componente como registrar al component manager.
        /// </summary>
		void initialize();

        /// <summary>
        /// Entidad due;a de este componente.
        /// </summary>
		IEntity Owner
		{
			get;
		}
	}
}
