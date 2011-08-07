using System;
using System.Collections.Generic;
using System.Text;
using OpenNETCF.Reflection;

namespace CommLayerCompact
{
    /// <summary>
    /// Representa un evento
    /// </summary>
    internal class Event
    {
        /// <summary>
        /// El evento
        /// </summary>
        private Delegate _eventHandler;

        /// <summary>
        /// El evento
        /// </summary>
        public Delegate EventHandler
        {
            get { return _eventHandler; }
            set { _eventHandler = value; }
        }

        /// <summary>
        /// El parametro
        /// </summary>
        private Object _param;

        /// <summary>
        /// El parametro
        /// </summary>
        public Object Param
        {
            get { return _param; }
            set { _param = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventHandler">el evento</param>
        /// <param name="param">el parametro</param>
        public Event(Delegate eventHandler, Object param)
        {
            EventHandler = eventHandler;
            Param = param;
        }

        /// <summary>
        /// Ejecuta el evento
        /// </summary>
        public void execute()
        {
            try
            {
                if (Param != null)
                {
                    EventHandler.GetType().GetMethod("Invoke").Invoke(EventHandler, new object[] { Param });
                }
                else
                {
                    EventHandler.GetType().GetMethod("Invoke").Invoke(EventHandler, null);
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
