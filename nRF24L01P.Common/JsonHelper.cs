using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Windows.Devices.Radios.nRF24L01P.Common
{
    public static class JsonHelper
    {
        public static string ToJson<T>(T data)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof (T));

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, data);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        public static T FromJson<T>(string json)
        {
            T returnValue = default(T);
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof (T));
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(bytes, 0, bytes.Length);
                returnValue = (T) serializer.ReadObject(ms);
            }

            return returnValue;
        }
    }
}
