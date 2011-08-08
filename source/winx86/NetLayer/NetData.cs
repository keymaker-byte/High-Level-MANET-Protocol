using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Net;

namespace NetLayer
{
    /// <summary>
    /// Clase que enumero los sistemas operativos soportados y que establece el modo de funcionamiento de la conexión de red inalámbrica
    /// </summary>
    public static class OpSystemType
    {
        /// <summary>
        /// Windows XP Service Pack 3
        /// </summary>
        public const Int32 WINXPSP3 = 1;

        /// <summary>
        /// Windows Vista
        /// </summary>
        public const Int32 WIN7 = 2;
    }

    /// <summary>
    /// Clase para los datos de configuraciòn necesarios que involucran una RED
    /// </summary>
    public class NetData
    {
        private IPAddress _ipTcpListener;
        private String _ipUcpMulticast;
        private Int32 _tcpPort;
        private Int32 _udpPort;
        private String _subnetMask;
        private NetworkAdapter _networkAdapter;
        private String _adhocNetworkName;
        private Int32 _opSystem;
        private Int32 _waitTimeWiFi;
        private Int32 _waitForStart;
        private Int32 _waitForWifi;
        private Int32 _waitTimeStart;
        private Int32 _timeOutWriteTCP;
        private Int32 _timeIntervalTimer;
        private Int32 _waitForTimerClose;
        private Int32 _waitForTCPConnection;
        private Int32 _waitForAck;
        private Int32 _maxMessagesProcess;
        private Int32 _qualityRiseNetUser;
        private Int32 _qualityMaxNetUser;
        private Int32 _qualityNormalNetUser;
        private Int32 _qualityLowNetUser;

        private Int32 _lolinessTimeOut;
        private Int32 _sendFailsToDisconnect;
        private Int32 _tcpConnectTimeOut;
        private Int32 _stateCritical;
        private Int32 _stateOverloaded;
        private Int32 _statePathNN;
        private Int32 _statePathNL;
        private Int32 _statePathNC;
        private Int32 _statePathON;
        private Int32 _statePathOL;
        private Int32 _statePathOC;
        private Int32 _statePathCN;
        private Int32 _statePathCL;
        private Int32 _statePathCC;
        private Int32 _statePathNotFound;
        

        /// <summary>
        /// Constructor, inicializa los valores por omisión de la configuración.
        /// </summary>
        public NetData()
        {
            pickNewIp();
            IpUdpMulticast = "224.0.0.2";
            TcpPort = 30001;
            UdpPort = 30002;
            SubnetMask = "255.255.0.0";
            NetworkAdapter = SystemHandler.getWifiAdapter();
            AdhocNetworkName = "HLMP-MANET";
            OpSystem = OpSystemType.WINXPSP3;
            WaitTimeWiFi = 15000;
            WaitTimeStart = 4000; 
            WaitForStart = 10;
            WaitForWifi = 3;
            TimeOutWriteTCP = 2000;
            TimeIntervalTimer = 1000;
            WaitForTimerClose = 20;
            WaitForTCPConnection = 20;
            WaitForAck = 10;
            MaxMessagesProcess = 2500;
            QualityRiseNetUser = 5;
            QualityMaxNetUser = 25;
            QualityNormalNetUser = 15;
            QualityLowNetUser = 5;
            LolinessTimeOut = 15;
            SendFailsToDisconnect = 5;
            TcpConnectTimeOut = 2000;
            StateCritical = 20;
            StateOverloaded = 10;
            StatePathNN = 1;
            StatePathNL = 2;
            StatePathNC = 4;
            StatePathON = 10;
            StatePathOL = 20;
            StatePathOC = 40;
            StatePathCN = 100;
            StatePathCL = 200;
            StatePathCC = 400;
            StatePathNotFound = 1000000;
            
        }

        /// <summary>
        /// Ip delegada para realizar conexiones TCP
        /// </summary>
        public IPAddress IpTcpListener
        {
            get { return _ipTcpListener; }
            set { _ipTcpListener = value; }
        }

        /// <summary>
        /// Puerto delegado para levantar servidor TCP
        /// </summary>
        public Int32 TcpPort
        {
            get { return _tcpPort; }
            set { _tcpPort = value; }
        }

        /// <summary>
        /// Ip delegada para enviar y recibir mensajes multicast UDP
        /// </summary>
        public String IpUdpMulticast
        {
            get { return _ipUcpMulticast; }
            set { _ipUcpMulticast = value; }
        }

        /// <summary>
        /// Puerto delegado para escuchar y enviar mensajes multicast UDP
        /// </summary>
        public Int32 UdpPort
        {
            get { return _udpPort; }
            set { _udpPort = value; }
        }

        /// <summary>
        /// Mascara de Sub Red delegada para realizar conexiones TCP
        /// </summary>
        public String SubnetMask
        {
            get { return _subnetMask; }
            set { _subnetMask = value; }
        }

        /// <summary>
        /// Adaptador de red seleccionado para conectarse a la red inalámbrica
        /// </summary>
        public NetworkAdapter NetworkAdapter
        {
            get { return _networkAdapter; }
            set { _networkAdapter = value; }
        }

        /// <summary>
        /// Determina una nueva IP de manera aleatoria, en el rango de mascara de red 255.255.0.0
        /// </summary>
        /// <returns></returns>
        public void pickNewIp()
        {
            Random r = new Random(DateTime.Now.Millisecond);
            IpTcpListener =  IPAddress.Parse("170.160" + "." + r.Next(1, 255) + "." + r.Next(1, 255));
        }

        /// <summary>
        /// El nombre de la red adhoc que se debe crear/unir
        /// </summary>
        public String AdhocNetworkName
        {
            get { return _adhocNetworkName; }
            set { _adhocNetworkName = value; }
        }

        /// <summary>
        /// El sistema operativo 
        /// </summary>
        public Int32 OpSystem
        {
            get { return _opSystem; }
            set { _opSystem = value; }
        }

        /// <summary>
        /// Tiempo que se espera luego de enviar una llamada de conexion a red inalambrica, para intentar enviar otra
        /// en caso de que no haya habido un evento de conexión
        /// </summary>
        public Int32 WaitTimeWiFi
        {
            get { return _waitTimeWiFi; }
            set { _waitTimeWiFi = value; }
        }

        /// <summary>
        /// Tiempo que se espera para intentar configurar la IP o levantar los servicios TCP en caso de intentos fallidos
        /// </summary>
        public Int32 WaitTimeStart
        {
            get { return _waitTimeStart; }
            set { _waitTimeStart = value; }
        }

        /// <summary>
        /// Veces que se intenta configurar la Ip o levantar los servicios TCP, en caso de intentos fallidos
        /// </summary>
        public Int32 WaitForStart
        {
            get { return _waitForStart; }
            set { _waitForStart = value; }
        }

        /// <summary>
        /// Veces que se intenta conectar a Wifi Sin Exito
        /// </summary>
        public Int32 WaitForWifi
        {
            get { return _waitForWifi; }
            set { _waitForWifi = value; }
        }

        /// <summary>
        /// TimeOut para escribir en un Socket TCP
        /// </summary>
        public Int32 TimeOutWriteTCP
        {
            get { return _timeOutWriteTCP; }
            set { _timeOutWriteTCP = value; }
        }

        /// <summary>
        /// Intervalo de tiempo para el timer
        /// </summary>
        public Int32 TimeIntervalTimer
        {
            get { return _timeIntervalTimer; }
            set { _timeIntervalTimer = value; }
        }

        /// <summary>
        /// Veces que se intentará para esperar al cierre del timer
        /// </summary>
        public Int32 WaitForTimerClose
        {
            get { return _waitForTimerClose; }
            set { _waitForTimerClose = value; }
        }

        /// <summary>
        /// Veces que se espera para intentar una conección TCP a un NetUser que cumple los requerimientos
        /// </summary>
        public Int32 WaitForTCPConnection
        {
            get { return _waitForTCPConnection; }
            set { _waitForTCPConnection = value; }
        }

        /// <summary>
        /// Veces que se espera para reenviar un mensaje safe no confirmado
        /// </summary>
        public Int32 WaitForAck
        {
            get { return _waitForAck; }
            set { _waitForAck = value; }
        }

        /// <summary>
        /// El numero maximo de mensajes que se procesan por intervalo de tiempo
        /// </summary>
        public Int32 MaxMessagesProcess
        {
            get { return _maxMessagesProcess; }
            set { _maxMessagesProcess = value; }
        }

        /// <summary>
        /// Inncremento a la calidad de señal que se le hace al netUSer al recibir un mensaje i'm alive
        /// </summary>
        public Int32 QualityRiseNetUser
        {
            get { return _qualityRiseNetUser; }
            set { _qualityRiseNetUser = value; }
        }

        /// <summary>
        /// Calidad de señal maxima de un netuser
        /// </summary>
        public Int32 QualityMaxNetUser
        {
            get { return _qualityMaxNetUser; }
            set { _qualityMaxNetUser = value; }
        }

        /// <summary>
        /// Calidad de señal normal de un netuser
        /// </summary>
        public Int32 QualityNormalNetUser
        {
            get { return _qualityNormalNetUser; }
            set { _qualityNormalNetUser = value; }
        }

        /// <summary>
        /// Calidad de señal baja de un netuser
        /// </summary>
        public Int32 QualityLowNetUser
        {
            get { return _qualityLowNetUser; }
            set { _qualityLowNetUser = value; }
        }

        /// <summary>
        /// Tiempo de espera para determinar caso de loliness
        /// </summary>
        public Int32 LolinessTimeOut
        {
            get { return _lolinessTimeOut; }
            set { _lolinessTimeOut = value; }
        }

        /// <summary>
        /// Fallos para solicitar una desconexión a la maquina remota destino
        /// </summary>
        public Int32 SendFailsToDisconnect
        {
            get { return _sendFailsToDisconnect; }
            set { _sendFailsToDisconnect = value; }
        }

        /// <summary>
        /// Tiempo de espera para conexion TCP
        /// </summary>
        public Int32 TcpConnectTimeOut
        {
            get { return _tcpConnectTimeOut; }
            set { _tcpConnectTimeOut = value; }
        }

        /// <summary>
        /// Valor para el estado critico
        /// </summary>
        public Int32 StateCritical
        {
            get { return _stateCritical; }
            set { _stateCritical = value; }
        }

        /// <summary>
        /// Valor para el estado sobrecargado
        /// </summary>
        public Int32 StateOverloaded
        {
            get { return _stateOverloaded; }
            set { _stateOverloaded = value; }
        }

        /// <summary>
        /// Valor de peso para el estado normal normal
        /// </summary>
        public Int32 StatePathNN
        {
            get { return _statePathNN; }
            set { _statePathNN = value; }
        }

        /// <summary>
        /// Valor de peso para el estado normal low
        /// </summary>
        public Int32 StatePathNL
        {
            get { return _statePathNL; }
            set { _statePathNL = value; }
        }

        /// <summary>
        /// Valor de peso para el estado normal critical
        /// </summary>
        public Int32 StatePathNC
        {
            get { return _statePathNC; }
            set { _statePathNC = value; }
        }

        /// <summary>
        /// Valor de peso para el estado overloaded normal
        /// </summary>
        public Int32 StatePathON
        {
            get { return _statePathON; }
            set { _statePathON = value; }
        }

        /// <summary>
        /// Valor de peso para el estado overloaded low
        /// </summary>
        public Int32 StatePathOL
        {
            get { return _statePathOL; }
            set { _statePathOL = value; }
        }

        /// <summary>
        /// Valor de peso para el estado overloaded critical
        /// </summary>
        public Int32 StatePathOC
        {
            get { return _statePathOC; }
            set { _statePathOC = value; }
        }

        /// <summary>
        /// Valor de peso para el estado critical normal
        /// </summary>
        public Int32 StatePathCN
        {
            get { return _statePathCN; }
            set { _statePathCN = value; }
        }

        /// <summary>
        /// Valor de peso para el estado critical low
        /// </summary>
        public Int32 StatePathCL
        {
            get { return _statePathCL; }
            set { _statePathCL = value; }
        }

        /// <summary>
        /// Valor de peso para estado critical crtical
        /// </summary>
        public Int32 StatePathCC
        {
            get { return _statePathCC; }
            set { _statePathCC = value; }
        }

        /// <summary>
        /// Valor de peso para cuando no se encuentra un camino
        /// </summary>
        public Int32 StatePathNotFound
        {
            get { return _statePathNotFound; }
            set { _statePathNotFound = value; }
        }

        
    }
}
