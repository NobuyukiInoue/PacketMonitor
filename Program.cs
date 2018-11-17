using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;

public class Program
{
    public void Main()
    {
        //インターフェースのアドレス取得            
        var he = Dns.GetHostEntry(Dns.GetHostName());
        var addr = he.AddressList.Where((h) => h.AddressFamily == AddressFamily.InterNetwork).ToList();

        Socket socket;

        // 管理者権限での実行が必要
        try
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
        }
        catch
        {
            Console.Write("You need Administrator user privileges to execute this program.");
            return;
        }

        socket.Bind(new IPEndPoint(addr[0], 0));
        socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AcceptConnection, 1);

        byte[] ib = new byte[] { 1, 0, 0, 0 };
        byte[] ob = new byte[] { 0, 0, 0, 0 };
        socket.IOControl(IOControlCode.ReceiveAll, ib, ob);//SIO_RCVALL
        byte[] buf = new byte[4096];
        int i = 0;

        while (true)
        {
            IAsyncResult iares = socket.BeginReceive(buf, 0, buf.Length, SocketFlags.None, null, null);
            var len = socket.EndReceive(iares);
            Console.WriteLine("[{0}] Protocol={1} src={2} dst={3} TTL={4} Len={5}"
                                    , i++, Proto(buf[9]), Ip(buf, 12), Ip(buf, 16), buf[8], len);
        }
    }

    private string Ip(byte[] buf, int i)
    {
        return string.Format("{0}.{1}.{2}.{3}", buf[i], buf[i + 1], buf[i + 2], buf[i + 3]);
    }

    private string Proto(byte b)
    {
        switch (b)
        {
            case 1:
                return "ICMP";
            case 2:
                return "IGMP";
            case 4:
                return "IP";
            case 6:
                return "TCP";
            case 17:
                return "UDP";
            case 41:
                return "IPv6";
            case 47:
                return "GRE";
            case 50:
                return "ESP";
            case 51:
                return "AH";
            case 58:
                return "IPv6-ICMP";
            case 88:
                return "EIGRP";
            case 89:
                return "OSP";
            case 112:
                return "VRRP";
            case 115:
                return "L2TP";
            default:
                return "Other";
        }
    }
}
