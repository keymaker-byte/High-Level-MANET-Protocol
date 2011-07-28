using System;
using System.Collections.Generic;
using System.Text;

namespace CommLayer
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
        /// <param name="eventHandler"></param>
        /// <param name="param"></param>
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
                    EventHandler.DynamicInvoke(new object[] { Param });
                }
                else
                {
                    EventHandler.DynamicInvoke(null);
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
