using System;
using System.Collections.Generic;
using System.Linq;

namespace LiquidCtlAfterburnerPlugin
{
    internal class LiquidctlDevice
    {
        public LiquidctlDevice(LiquidctlStatusJSON output)
        {
            address = output.address;
            name = output.description;
            status = output.status;
        }

        public string address;
        public string name;
        public List<LiquidctlStatusJSON.StatusRecord> status = new List<LiquidctlStatusJSON.StatusRecord>();

        public void LoadJSON()
        {
            try
            {
                LiquidctlStatusJSON output = LiquidctlCLIWrapper.ReadStatus(address).First();
                UpdateFromJSON(output);
            }
            catch (InvalidOperationException)
            {
                throw new Exception($"Device {address} not showing up");
            }
        }

        public void UpdateFromJSON(LiquidctlStatusJSON output)
        {
            foreach (LiquidctlStatusJSON.StatusRecord status in this.status) {
                status.value = output.status.Find(s => s.key == status.key).value;
            }
        }

        public String GetDeviceInfo() {
            String ret = $"Device @ {address}";
            foreach (LiquidctlStatusJSON.StatusRecord status in this.status) {
                ret += $", {status.key}: {status.value}";
            }
            return ret;
        }
    }
}
