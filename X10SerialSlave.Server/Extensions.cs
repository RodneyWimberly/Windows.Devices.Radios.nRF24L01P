using System.Text;

namespace X10SerialSlave.Server
{
    public static class Extensions
    {
        public static byte[] GetBytes(this string data)
        {
            return Encoding.ASCII.GetBytes(data);
        }
    }
}
