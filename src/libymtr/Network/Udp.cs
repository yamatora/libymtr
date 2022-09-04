using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace libymtr.Network {
    internal class Udp {
        public static async Task Broadcast(byte[] data, int port) {
            string? dst = IP.GetBroadcastAddress();
            if (dst == null) {
                return;
            }
            await SingleShot(data, IPAddress.Parse(dst), port);
        }
        public static async Task SingleShot(byte[] data, IPAddress dst, int port) {
            using (UdpClient client = new UdpClient(dst.ToString(), port)) {
                await client.SendAsync(data, data.Length);
            }
        }
    }
}
