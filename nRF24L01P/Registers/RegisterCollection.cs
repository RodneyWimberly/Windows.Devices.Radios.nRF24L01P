using System;
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

        public void SaveAll(bool force = false)
        {
            foreach (T register in Values.Where(r => force || r.IsDirty))
                register.Save();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} Values:\r\n\r\n", GetType().Name);
            foreach (T register in Values)
            {
                try
                {
                    sb.AppendFormat("{0}\r\n\t{1}\r\n\r\n", register.Name, register);
                }
                catch (Exception ex)
                {
                    sb.AppendFormat("{0}\r\n\tError:{1}\r\n\r\n", register.Name, ex.Message);
                }
            }

            return sb.ToString();
        }
    }
}
