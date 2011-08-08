using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Management;
using System.Net.NetworkInformation;
using SystemInterop;
using System.Threading;

namespace NetLayer
{
    
    /// <summary>
    /// Clase con métodos de comunicación con el sistema operativo
    /// </summary>
    public class SystemHandler
    {

        /// <summary>
        /// Cambia la IP del sistema operativo al estádo dinámico
        /// Si ocurre un error se arroja la excepción
        /// </summary>
        internal static void setDinamicIP(NetworkAdapter networkAdapter)
        {
            if (networkAdapter == null)
            {
                throw new Exception("no hay un adaptador de red seleccionado");
            }
            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();
            foreach (ManagementObject objMO in objMOC)
            {
                try
                {
                    if (((UInt32)objMO["Index"]) == networkAdapter.Index)
                    {
                        //metodo que pertenecen a Win32_NetworkAdapterConfiguration
                        ManagementBaseObject enableDinamic = objMO.GetMethodParameters("EnableDHCP");
                        ManagementBaseObject result = objMO.InvokeMethod("EnableDHCP", enableDinamic, null);
                        if (((UInt32)result["returnValue"]) != 0)
                        {
                            throw new Exception("EnableDHCP: " + getManagementMessage(((UInt32)result["returnValue"])));
                        }
                        break;
                    }
                }
                catch (ThreadAbortException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    throw new Exception("error en el adaptador de red: " + objMO["Description"] + ": " + e.Message);
                }
            }
        }


        /// <summary>
        /// Cambia la Ip del sistema operativo a la configuración de red adhoc
        /// Si ocurre un error se arroja la excepción
        /// </summary>
        internal static void setStaticIP(NetworkAdapter networkAdapter, String ip, String subnetMask)
        {
            if (networkAdapter == null)
            {
                throw new Exception("no hay un adaptador de red seleccionado");
            }

            //Invoca los metodos nativos de Windows
            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();
            foreach (ManagementObject objMO in objMOC)
            {
                if (((UInt32)objMO["Index"]) == networkAdapter.Index)
                {
                    //metodo que pertenecen a Win32_NetworkAdapterConfiguration
                    ManagementBaseObject enableStatic = objMO.GetMethodParameters("EnableStatic");
                    //parametros EnableStatic
                    enableStatic["IPAddress"] = new string[] { ip };
                    enableStatic["SubnetMask"] = new string[] { subnetMask };
                    //ejecuta EnableStatic
                    ManagementBaseObject result = objMO.InvokeMethod("EnableStatic", enableStatic, null);
                    if (((UInt32)result["returnValue"]) != 0)
                    {
                        throw new Exception("EnableStatic: " + getManagementMessage(((UInt32)result["returnValue"])));
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Cambia la Ip del sistema operativo a la configuración de red adhoc, segundo metodo
        /// Si ocurre un error se arroja la excepción
        /// </summary>
        internal static void setStaticIP2(NetworkAdapter networkAdapter, String ip, String subnetMask)
        {
            if (networkAdapter == null)
            {
                throw new Exception("no hay un adaptador de red seleccionado");
            }

            //Invoca los metodos nativos de Windows
            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();
            foreach (ManagementObject objMO in objMOC)
            {
                if (((UInt32)objMO["Index"]) == networkAdapter.Index)
                {
                    //metodo que pertenecen a Win32_NetworkAdapterConfiguration
                    ManagementBaseObject enableStatic = objMO.GetMethodParameters("EnableStatic");
                    //parametros EnableStatic
                    enableStatic["IPAddress"] = new string[] { ip };
                    enableStatic["SubnetMask"] = new string[] { subnetMask };
                    //ejecuta EnableStatic
                    ManagementBaseObject result = objMO.InvokeMethod("EnableStatic", enableStatic, null);
                    if (((UInt32)result["returnValue"]) != 0)
                    {
                        throw new Exception("EnableStatic: " + getManagementMessage(((UInt32)result["returnValue"])));
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Cambia el estado del adaptador de red a encendido
        /// Si ocurre un error se arroja la excepción
        /// </summary>
        internal static void enableIpAdapter(NetworkAdapter networkAdapter)
        {
            if (networkAdapter == null)
            {
                throw new Exception("no hay un adaptador de red seleccionado");
            }
            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapter");
            ManagementObjectCollection objMOC = objMC.GetInstances();
            foreach (ManagementObject objMO in objMOC)
            {
                try
                {
                    if (((UInt32)objMO["Index"]) == networkAdapter.Index)
                    {
                        //enable
                        ManagementBaseObject enable = objMO.GetMethodParameters("Enable");
                        ManagementBaseObject resultEnable = objMO.InvokeMethod("Enable", enable, null);
                        if (((UInt32)resultEnable["returnValue"]) != 0)
                        {
                            throw new Exception("EnableNetworkAdapter: " + (UInt32)resultEnable["returnValue"]);
                        }
                        break;
                    }
                }
                catch (ThreadAbortException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    throw new Exception("error en el adaptador de red: " + objMO["Description"] + ": " + e.Message);
                }
            }
        }

        /// <summary>
        /// Cambia el estado del adaptador de red a apagado
        /// Si ocurre un error se arroja la excepción
        /// </summary>
        internal static void disableIpAdapter(NetworkAdapter networkAdapter)
        {
            if (networkAdapter == null)
            {
                throw new Exception("no hay un adaptador de red seleccionado");
            }
            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapter");
            ManagementObjectCollection objMOC = objMC.GetInstances();
            foreach (ManagementObject objMO in objMOC)
            {
                try
                {
                    if (((UInt32)objMO["Index"]) == networkAdapter.Index)
                    {
                        //disable
                        ManagementBaseObject disable = objMO.GetMethodParameters("Disable");
                        ManagementBaseObject resultDisable = objMO.InvokeMethod("Disable", disable, null);
                        if (((UInt32)resultDisable["returnValue"]) != 0)
                        {
                            throw new Exception("DisableNetworkAdapter: " + (UInt32)resultDisable["returnValue"]);
                        }
                        break;
                    }
                }
                catch (ThreadAbortException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    throw new Exception("error en el adaptador de red: " + objMO["Description"] + ": " + e.Message);
                }
            }
        }

        /// <summary>
        /// Obtiene el estado DAD de la Ip asignada a TCP
        /// </summary>
        /// <returns>un estado de la clase IpState</returns>
        internal static int getIpState(NetworkAdapter networkAdapter, IPAddress ipAdress)
        {
            int ipState = IpState.NOTFOUND;
            
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                if (adapter.Id.Equals(networkAdapter.Id))
                {
                    IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                    UnicastIPAddressInformationCollection uniCast = adapterProperties.UnicastAddresses;
                    foreach (UnicastIPAddressInformation uni in uniCast)
                    {
                        if (uni.Address.ToString().Equals(ipAdress.ToString()))
                        {
                            switch (uni.DuplicateAddressDetectionState)
                            {
                                case DuplicateAddressDetectionState.Deprecated:
                                    {
                                        ipState = IpState.INVALID;
                                        break;
                                    }
                                case DuplicateAddressDetectionState.Duplicate:
                                    {
                                        ipState = IpState.DUPLICATE;
                                        break;
                                    }
                                case DuplicateAddressDetectionState.Invalid:
                                    {
                                        ipState = IpState.INVALID;
                                        break;
                                    }
                                case DuplicateAddressDetectionState.Preferred:
                                    {
                                        ipState = IpState.VALID;
                                        break;
                                    }
                                case DuplicateAddressDetectionState.Tentative:
                                    {
                                        ipState = IpState.INVALID;
                                        break;
                                    }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
            return ipState;
        }

        /// <summary>
        /// Obtiene el numero de bytes enviados por el adaptador
        /// </summary>
        /// <param name="networkAdapter">el adaptador de red</param>
        /// <returns>el numero de bytes enviados</returns>
        internal static Int64 getAliveValue(NetworkAdapter networkAdapter)
        {
            Int64 n = 0;
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                if (adapter.Id.Equals(networkAdapter.Id))
                {
                    n = adapter.GetIPv4Statistics().NonUnicastPacketsReceived;
                    break;
                }
            }
            return n;
        }

        /// <summary>
        /// Obtiene el numero de bytes enviados por el adaptador
        /// </summary>
        /// <param name="networkAdapter">el adaptador de red</param>
        /// <returns>el numero de bytes enviados</returns>
        internal static bool isOperative(NetworkAdapter networkAdapter)
        {
            bool b = false;

            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                if (adapter.Id.Equals(networkAdapter.Id))
                {
                    b = (adapter.OperationalStatus == OperationalStatus.Up);
                    break;
                }
            }
            return b;
        }

        /// <summary>
        /// Busca el adaptador de red WIFI
        /// </summary>
        /// <returns>El adaptador de red Wifi</returns>
        public static NetworkAdapter getWifiAdapter()
        {
            try
            {
                WlanClient wLanClient = new WlanClient();
                foreach (WlanClient.WlanInterface wlanIface in wLanClient.Interfaces)
                {
                    if (wlanIface.NetworkInterface != null)
                    {
                        NetworkAdapter adapter = new NetworkAdapter();
                        adapter.Id = wlanIface.NetworkInterface.Id;
                        adapter.Description = wlanIface.InterfaceDescription;
                        ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
                        ManagementObjectCollection objMOC = objMC.GetInstances();
                        foreach (ManagementObject objMO in objMOC)
                        {
                            String id = (String)objMO["SettingID"];
                            if(adapter.Id.Equals(id))
                            {
                                adapter.Index = (UInt32)objMO["Index"];
                                return adapter;
                            }
                        }
                        
                    }
                }
                return null;
            }
            catch (ThreadAbortException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Retorna una lista de adaptadores de red disponibles en la máquina
        /// Si ocurre un error se arroja una Excepción
        /// </summary>
        /// <returns></returns>
        public static List<NetworkAdapter> getNetworkAdapters()
        {
            try
            {
                List<NetworkAdapter> adapters = new List<NetworkAdapter>();
                ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection objMOC = objMC.GetInstances();
                foreach (ManagementObject objMO in objMOC)
                {
                    if(true)
                    {
                        NetworkAdapter adapter = new NetworkAdapter();
                        adapter.Id = (String)objMO["SettingID"];
                        adapter.Description = (String)objMO["Description"];
                        adapter.Index = (UInt32)objMO["Index"];
                        adapters.Add(adapter);
                    }
                }
                return adapters;
            }
            catch (ThreadAbortException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Obtiene el texto de un error de sistema de windows a partir del código de error
        /// Si ocurre un error se arroja una Excepción
        /// </summary>
        /// <param name="index">el código de error</param>
        /// <returns>El texto con la descripción del error</returns>
        private static String getManagementMessage(UInt32 index)
        {
            try
            {
                switch (index)
                {
                    case 0:
                        {
                            return "Successful completion, no reboot required.";
                        }
                    case 1:
                        {
                            return "Successful completion, reboot required.";
                        }
                    case 64:
                        {
                            return "Method not supported on this platform.";
                        }
                    case 65:
                        {
                            return "Unknown failure.";
                        }
                    case 66:
                        {
                            return "Invalid subnet mask.";
                        }
                    case 67:
                        {
                            return "An error occurred while processing an instance that was returned.";
                        }
                    case 68:
                        {
                            return "Invalid input parameter.";
                        }
                    case 69:
                        {
                            return "More than five gateways specified.";
                        }
                    case 70:
                        {
                            return "Invalid IP address.";
                        }
                    case 71:
                        {
                            return "Invalid gateway IP address.";
                        }
                    case 72:
                        {
                            return "An error occurred while accessing the registry for the requested information.";
                        }
                    case 73:
                        {
                            return "Invalid domain name.";
                        }
                    case 74:
                        {
                            return "Invalid host name.";
                        }
                    case 75:
                        {
                            return "No primary or secondary WINS server defined.";
                        }
                    case 76:
                        {
                            return "Invalid file.";
                        }
                    case 77:
                        {
                            return "Invalid system path.";
                        }
                    case 78:
                        {
                            return "File copy failed.";
                        }
                    case 79:
                        {
                            return "Invalid security parameter.";
                        }
                    case 80:
                        {
                            return "Unable to configure TCP/IP service.";
                        }
                    case 81:
                        {
                            return "Unable to configure DHCP service.";
                        }
                    case 82:
                        {
                            return "Unable to renew DHCP lease.";
                        }
                    case 83:
                        {
                            return "Unable to release DHCP lease.";
                        }
                    case 84:
                        {
                            return "IP not enabled on adapter.";
                        }
                    case 85:
                        {
                            return "IPX not enabled on adapter.";
                        }
                    case 86:
                        {
                            return "Frame or network number bounds error.";
                        }
                    case 87:
                        {
                            return "Invalid frame type.";
                        }
                    case 88:
                        {
                            return "Invalid network number.";
                        }
                    case 89:
                        {
                            return "Duplicate network number.";
                        }
                    case 90:
                        {
                            return "Parameter out of bounds.";
                        }
                    case 91:
                        {
                            return "Access denied.";
                        }
                    case 92:
                        {
                            return "Out of memory.";
                        }
                    case 93:
                        {
                            return "Already exists.";
                        }
                    case 94:
                        {
                            return "Path, file, or object not found";
                        }
                    case 95:
                        {
                            return "Unable to notify service.";
                        }
                    case 96:
                        {
                            return "Unable to notify DNS service.";
                        }
                    case 97:
                        {
                            return "Interface not configurable.";
                        }
                    case 98:
                        {
                            return "Not all DHCP leases could be released or renewed.";
                        }
                    case 100:
                        {
                            return "DHCP not enabled on adapter.";
                        }
                    default:
                        {
                            return "Unknown error. Win32Code: " + index;
                        }
                }
            }
            catch (ThreadAbortException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
