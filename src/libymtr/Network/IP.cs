using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace libymtr.Network {
    internal class IP {
        /// <summary>
        /// Get Local IPAddress
        /// </summary>
        /// <returns></returns>
        public static IPAddress? GetLocalIP() {
            foreach(var iface in NetworkInterface.GetAllNetworkInterfaces()) {
                //  Ethernet or Wi-Fi
                var type = iface.NetworkInterfaceType;
                if(type != NetworkInterfaceType.Ethernet && type != NetworkInterfaceType.Wireless80211) {
                    continue;
                }
                foreach(var ip in iface.GetIPProperties().UnicastAddresses) {
                    if(ip.Address.AddressFamily != AddressFamily.InterNetwork || ip.PrefixOrigin != PrefixOrigin.Dhcp) {
                        continue;
                    }
                    return ip.Address;
                }
            }
            return null;
        }
    }
}
