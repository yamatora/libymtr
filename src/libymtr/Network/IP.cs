using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace libymtr.Network {
    public class IP {
        /// <summary>
        /// Get Local IPAddress
        /// </summary>
        /// <returns></returns>
        public static IPAddress? GetLocalIP() {
            foreach (var iface in NetworkInterface.GetAllNetworkInterfaces()) {
                //  Ethernet or Wi-Fi
                var type = iface.NetworkInterfaceType;
                if (type != NetworkInterfaceType.Ethernet && type != NetworkInterfaceType.Wireless80211) {
                    continue;
                }
                foreach (var ip in iface.GetIPProperties().UnicastAddresses) {
                    if (ip.Address.AddressFamily != AddressFamily.InterNetwork || ip.PrefixOrigin != PrefixOrigin.Dhcp) {
                        continue;
                    }
                    return ip.Address;
                }
            }
            return null;
        }
        public static string? GetBroadcastAddress() {
            foreach (var iface in NetworkInterface.GetAllNetworkInterfaces()) {
                //  Ethernet or Wi-Fi
                var type = iface.NetworkInterfaceType;
                if (type != NetworkInterfaceType.Ethernet && type != NetworkInterfaceType.Wireless80211) {
                    continue;
                }
                foreach (var ip in iface.GetIPProperties().UnicastAddresses) {
                    if (ip.Address.AddressFamily != AddressFamily.InterNetwork || ip.PrefixOrigin != PrefixOrigin.Dhcp) {
                        continue;
                    }
                    string[] local = ip.Address.ToString().Split('.');
                    string[] smask = ip.IPv4Mask.ToString().Split('.');
                    string[] result = new string[4];
                    for (int i = 0; i < 4; i++) {
                        byte bresult = (byte)(byte.Parse(local[i]) | ~byte.Parse(smask[i]));
                        result[i] = bresult.ToString();
                    }
                    return Generic.ConcatByChar('.', result);
                }
            }
            return null;
        }
    }
}
