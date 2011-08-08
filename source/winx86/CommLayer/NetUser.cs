using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using NetLayer;

namespace CommLayer
{
    /// <summary>
    /// Clase para enumeración de posibles calidades de señal de los usuarios remotos
    /// </summary>
    public static class NetUserQuality
    {
        /// <summary>
        /// Constante para el estado normal
        /// </summary>
        public const Int32 NORMAL = 1;

        /// <summary>
        /// Constante para el estado bajo
        /// </summary>
        public const Int32 LOW = 2;

        /// <summary>
        /// Constante para el estado critico
        /// </summary>
        public const Int32 CRITICAL = 3;
    }

    /// <summary>
    /// Clase para enumeración de posibles estados de tráfico de los usuarios remotos
    /// </summary>
    public static class CommunicationQuality
    {
        /// <summary>
        /// Constante para el estado normal
        /// </summary>
        public const int NORMAL = 1;

        /// <summary>
        /// Constante para el estado con sobrecargar de trafico
        /// </summary>
        public const int OVERLOADED = 2;

        /// <summary>
        /// Constante para el estado critico
        /// </summary>
        public const int CRITICAL = 3;
    }


    /// <summary>
    /// Datos de usuario dentro de la MANET
    /// </summary>
    public class NetUser
    {
        /// <summary>
        /// Id del usuario, esta variable se serializa
        /// </summary>
        private Guid _id;

        /// <summary>
        /// Nombre del usuario, esta variable se serializa
        /// </summary>
        private String _name;

        /// <summary>
        /// Dirección IP, esta variable se serializa
        /// </summary>
        private IPAddress _ip;

        /// <summary>
        /// Array con los id's de la vecindad TCP, esta variable se serializa
        /// </summary>
        private Guid[] _neighborhoodIds;

        /// <summary>
        /// La calidad de la señal, NO se serializa
        /// </summary>
        private Int32 _signalQuality;

        /// <summary>
        /// Valor de señal restante, NO se serializa
        /// </summary>
        private Int32 _timeout;

        /// <summary>
        /// Saltos UDP que se encuentra del usuario remoto, NO se serializa
        /// </summary>
        private Int32 _jumpsAway;

        /// <summary>
        /// Tiempo de espera restante para intentar una conexion TCP con este usuario remoto, NO se serializa
        /// </summary>
        private Int32 _waitTimeOut;

        /// <summary>
        /// El estado de la señal
        /// </summary>
        private Int32 _state;

        /// <summary>
        /// El grupo al que pertenece
        /// </summary>
        private byte[] _upLayerData;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public NetUser()
        {
            this.Name = System.Environment.MachineName;
            this.SignalQuality = NetUserQuality.NORMAL;
            this.NeighborhoodIds = new Guid[0];
            this.JumpsAway = 0;
            this.WaitTimeOut = 0;
            this.State = CommunicationQuality.NORMAL;
            this.UpLayerData = new byte[0];
        }

        /// <summary>
        /// Constructor parametrizado, para uso interno, no se debe usar directamente
        /// </summary>
        /// <param name="id">El id del usuario</param>
        /// <param name="name">El nombre del usuario</param>
        /// <param name="ip">La dirección IP del usuario</param>
        /// <param name="neighborhood">Un array con los ids de los nodos vecinos al usuario</param>
        /// <param name="netData">Los datos de configuración</param>
        internal NetUser(Guid id, String name, IPAddress ip, Guid[] neighborhood, NetData netData)
        {
            this.Id = id;
            this.Name = name;
            this.Ip = ip;
            this.NeighborhoodIds = neighborhood;
            this.SignalQuality = NetUserQuality.NORMAL;
            setTimeOut(netData.QualityMaxNetUser, netData);
            this.JumpsAway = 0;
            this.WaitTimeOut = 0;
            this.State = CommunicationQuality.NORMAL;
            this.UpLayerData = new byte[0];
        }

        /// <summary>
        /// Nombre a mostrar
        /// </summary>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Dirección Ip
        /// </summary>
        public IPAddress Ip
        {
            get { return _ip; }
            set { _ip = value; }
        }

        /// <summary>
        /// Calidad de la señal
        /// </summary>
        public Int32 SignalQuality
        {
            get { return _signalQuality; }
            set { _signalQuality = value; }
        }

        /// <summary>
        /// El numero único de indentificación del usuario
        /// </summary>
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Los nodos vecinos del usuario (array de los ids de los usuarios con conexión directa TCP)
        /// </summary>
        public Guid[] NeighborhoodIds
        {
            get { return _neighborhoodIds; }
            set { _neighborhoodIds = value; }
        }

        /// <summary>
        /// Tiempo restante para que la calidad de la señal llegue a 0
        /// </summary>
        public Int32 Timeout
        {
            get { return _timeout; }
        }

        /// <summary>
        /// Cantidad de saltos UDP que se encuentra el usuario remoto
        /// </summary>
        public Int32 JumpsAway
        {
            get { return _jumpsAway; }
            set { _jumpsAway = value; }
        }

        /// <summary>
        /// Tiempo de espera para intentar conectar por TCP
        /// </summary>
        internal Int32 WaitTimeOut
        {
            get { return _waitTimeOut; }
            set { _waitTimeOut = value; }
        }

        /// <summary>
        /// La calidad del estado de comunicación (señal)
        /// </summary>
        public Int32 State
        {
            get { return _state; }
            set { _state = value; }
        }

        /// <summary>
        /// El grupo al que pertenece
        /// </summary>
        public byte[] UpLayerData
        {
            get { return _upLayerData; }
            set { _upLayerData = value; }
        }

        /// <summary>
        /// Chequea que el usuario ha sido detectado, incrementando su valor de calidad
        /// Método para uso interno
        /// </summary>
        /// <param name="netData">Los parámetros de configuración de red</param>
        internal void qualityUp(NetData netData)
        {
            setTimeOut(Timeout + netData.QualityRiseNetUser, netData);
        }

        /// <summary>
        /// Decrementa la calidad de señal, método para uso interno
        /// </summary>
        internal void qualityDown(NetData netData)
        {
            setTimeOut(Timeout - 1, netData);
        }

        /// <summary>
        /// Setea el time Out del este usuario para medir calidad de señal
        /// </summary>
        /// <param name="newTimeOut">El nuevo timeout</param>
        /// <param name="netData">Los datos de configuración de red</param>
        internal void setTimeOut(Int32 newTimeOut, NetData netData) 
        {
            if (newTimeOut > netData.QualityMaxNetUser)
            {
                _timeout = netData.QualityMaxNetUser;
            }
            else
            {
                _timeout = newTimeOut;
            }

            if (_timeout > netData.QualityNormalNetUser)
            {
                this.SignalQuality = NetUserQuality.NORMAL;
            }
            else if (_timeout > netData.QualityLowNetUser)
            {
                this.SignalQuality = NetUserQuality.LOW;
            }
            else
            {
                this.SignalQuality = NetUserQuality.CRITICAL;
            }
        }

        /// <summary>
        /// Selecciona un nuevo ID, método para uso interno
        /// </summary>
        internal void pickNewId()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Fabrica una representación del usuario como String
        /// </summary>
        /// <returns>El nombre del usuario y su numero IP</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
