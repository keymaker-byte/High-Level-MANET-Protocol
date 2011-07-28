using System;
using System.Collections.Generic;
using System.Text;

namespace NetLayer
{
    /// <summary>
    /// Interfaz para eventos de comunicación generados en NetHandler
    /// </summary>
    public interface CommHandlerI
    {
        /// <summary>
        /// Se gatilla cuando debe comenzar la comunicación
        /// </summary>
        void startNetworkingHandler();
        /// <summary>
        /// Se gatilla cuando se debe detener la comunicación 
        /// </summary>
        void stopNetworkingHandler();
        /// <summary>
        /// Se gatilla cuando se reseteará la coneción
        /// </summary>
        void resetNetworkingHandler();
        /// <summary>
        /// Se gatilla cuando ocurre un error en la red
        /// </summary>
        /// <param name="e">La excepción generada</param>
        void errorNetworkingHandler(Exception e);
        /// <summary>
        /// Se gatilla para enviar información del estado de la red.
        /// </summary>
        /// <param name="message">El mensaje envíado</param>
        void informationNetworkingHandler(String message);
    }
}
