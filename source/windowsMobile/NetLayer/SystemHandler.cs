using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using SystemInteropCompact;
using System.Threading;
using OpenNETCF.Net.NetworkInformation;
using OpenNETCF.Net;
using OpenNETCF.Win32;
using OpenNETCF;
using System.IO;
using OpenNETCF.WindowsMobile;

namespace NetLayerCompact
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
        internal static bool resetAdapter(NetData netData)
        {
            Radios radios = Radios.GetRadios();
            foreach (IRadio radio in radios)
            {
                if (radio.RadioType.Equals(RadioType.WiFi))
                {
                    WiFiRadio wifiRadio = (WiFiRadio)radio;
                    wifiRadio.RadioState = RadioState.Off;
                    wifiRadio.RadioState = RadioState.On;
                }
            }

            INetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (INetworkInterface networkInterface in networkInterfaces)
            {
                if (netData.NetworkAdapter.Name.Equals(networkInterface.Name))
                {
                    networkInterface.Rebind();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Cambia la IP del sistema operativo al estádo dinámico
        /// Si ocurre un error se arroja la excepción
        /// </summary>
        internal static bool resetAdapter2(NetData netData)
        {
            INetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (INetworkInterface networkInterface in networkInterfaces)
            {
                try
                {
                    networkInterface.Rebind();
                    break;
                }
                catch (Exception)
                {
                }
            }
            return true;
        }

        /// <summary>
        /// Cambia la IP del sistema operativo al estádo dinámico
        /// Si ocurre un error se arroja la excepción
        /// </summary>
        internal static void turnOnAdapter(NetData netData, Boolean forced)
        {
            Radios radios = Radios.GetRadios();
            foreach (IRadio radio in radios)
            {
                if (radio.RadioType.Equals(RadioType.WiFi))
                {
                    WiFiRadio wifiRadio = (WiFiRadio)radio;
                    if (forced)
                    {
                        wifiRadio.RadioState = RadioState.On;
                    }
                    else if (wifiRadio.RadioState == RadioState.Off)
                    {
                        wifiRadio.RadioState = RadioState.On;
                    }
                }
            }
        }

        /// <summary>
        /// Cambia la IP del sistema operativo al estádo dinámico
        /// Si ocurre un error se arroja la excepción
        /// </summary>
        internal static void turnOffAdapter(NetData netData)
        {
            Radios radios = Radios.GetRadios();
            foreach (IRadio radio in radios)
            {
                if (radio.RadioType.Equals(RadioType.WiFi))
                {
                    WiFiRadio wifiRadio = (WiFiRadio)radio;
                    wifiRadio.RadioState = RadioState.Off;
                }
            }
        }


        /// <summary>
        /// Cambia la Ip del sistema operativo a la configuración de red adhoc
        /// Si ocurre un error se arroja la excepción
        /// </summary>
        internal static void setRegistryIP(NetData netData)
        {
            string regName = "\\comm\\" + netData.NetworkAdapter.Name + "\\Parms\\Tcpip";
            RegistryKey tcpipkey = Registry.LocalMachine.OpenSubKey(regName, true);
            tcpipkey.SetValue("EnableDHCP", (UInt32)0);
            tcpipkey.SetValue("IPAddress", netData.IpTcpListener.ToString());
            tcpipkey.SetValue("SubnetMask", netData.SubnetMask);
            tcpipkey.Close();
        }

        /// <summary>
        /// Cambia la IP del sistema operativo al estádo dinámico
        /// Si ocurre un error se arroja la excepción
        /// </summary>
        internal static void setRegistryDHCP(NetData netData)
        {
            string regName = "\\comm\\" + netData.NetworkAdapter.Name + "\\Parms\\Tcpip";
            RegistryKey tcpipkey = Registry.LocalMachine.OpenSubKey(regName, true);
            tcpipkey.SetValue("EnableDHCP", (UInt32)1);
            tcpipkey.Close();
        }

        /// <summary>
        /// Cambia la IP del sistema operativo al estádo dinámico
        /// Si ocurre un error se arroja la excepción
        /// </summary>
        internal static bool rebindAdapter(NetData netData)
        {
            INetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (INetworkInterface networkInterface in networkInterfaces)
            {
                if (netData.NetworkAdapter.Name.Equals(networkInterface.Name))
                {
                    WirelessZeroConfigNetworkInterface adapter = (WirelessZeroConfigNetworkInterface)networkInterface;
                    adapter.Unbind();
                    adapter.Bind();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Obtiene el estado DAD de la Ip asignada a TCP
        /// </summary>
        /// <returns>un estado de la clase IpState</returns>
        internal static int getIpState(NetworkAdapter networkAdapter, IPAddress ipAdress)
        {
            try
            {
                int ipState = IpState.NOTFOUND;
                INetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (INetworkInterface networkInterface in networkInterfaces)
                {
                    if (networkAdapter.Name.Equals(networkInterface.Name))
                    {
                        UnicastIPAddressInformationCollection uaddresses = networkInterface.GetIPProperties().UnicastAddresses;
                        foreach (UnicastIPAddressInformation unicastAddress in uaddresses)
                        {
                            if (ipAdress.ToString().Equals(unicastAddress.Address.ToString()))
                            {
                                switch (unicastAddress.DuplicateAddressDetectionState)
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
                                            ipState = IpState.VALID;
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
        /// Busca el adaptador de red WIFI
        /// </summary>
        /// <returns>El adaptador de red Wifi</returns>
        public static NetworkAdapter getWifiAdapter()
        {
            try
            {
                Radios radios = Radios.GetRadios();
                foreach (IRadio radio in radios)
                {
                    if (radio.RadioType.Equals(RadioType.WiFi))
                    {
                        WiFiRadio wifiRadio = (WiFiRadio)radio;

                        NetworkAdapter adapter = new NetworkAdapter();
                        adapter.Name = wifiRadio.DeviceName;
                        adapter.Name = adapter.Name.Substring(adapter.Name.IndexOf("\\") + 1);
                        adapter.Description = wifiRadio.DisplayName;

                        INetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                        foreach (INetworkInterface networkInterface in networkInterfaces)
                        {
                            try
                            {
                                WirelessZeroConfigNetworkInterface Wadapter = (WirelessZeroConfigNetworkInterface)networkInterface;
                                adapter.Name = Wadapter.Name;
                                adapter.Description = Wadapter.Description;
                                break;
                            }
                            catch (Exception)
                            {
                                adapter.Description += "interface NOT FOUND";
                            }
                        }

                        return adapter;
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
                List<NetworkAdapter> networkAdapters = new List<NetworkAdapter>();
                INetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (INetworkInterface networkInterface in networkInterfaces)
                {
                    try
                    {
                        WirelessZeroConfigNetworkInterface adapter = (WirelessZeroConfigNetworkInterface)networkInterface;
                        NetworkAdapter networkAdapter = new NetworkAdapter();
                        networkAdapter.Name = adapter.Name;
                        networkAdapter.Description = adapter.Description;
                        networkAdapters.Add(networkAdapter);
                    }
                    catch (Exception)
                    {
                    }
                }
                return networkAdapters;
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
