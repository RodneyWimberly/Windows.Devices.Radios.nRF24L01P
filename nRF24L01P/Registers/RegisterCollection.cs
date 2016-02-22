using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    public class RegisterCollection<T> : Dictionary<byte, T>, IRegisterCollection<T> where T : IRegisterBase
    {
        public void LoadAll()
        {
            foreach (T register in Values)
                register.Load();
        }

        public void ResetAll()
        {
            foreach (T register in Values)
                register.Reset();
        }

        public void SaveAll(bool force = false)
        {
            foreach (T register in Values.Where(r => force || r.IsDirty))
                register.Save();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (T register in Values)
                sb.AppendFormat("{0}\r\n{1}\r\n", register.Name, register);
            return sb.ToString();
        }
    }
}
