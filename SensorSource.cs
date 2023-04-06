/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace LiquidCtlAfterburnerPlugin
{
    internal class SensorSource
    {

        public readonly LiquidctlDevice _device;
        private Task LiquidCtlPollTask;

        public SensorSource(LiquidctlDevice device)
        {
            _device = device;
        }

        public void Reload()
        {
            if (LiquidCtlPollTask == null || LiquidCtlPollTask.IsCompleted)
                LiquidCtlPollTask = Task.Run(() => _device.LoadJSON());
        }

        public int SensorCount => _device.status.Count;

        public float SensorValue(int index)
        {
            return (float) _device.status[index].GetValueAsFloat();
        }

        public void FillDescription(int index, ref MonitoringSourceDesc desc)
        {
            LiquidctlStatusJSON.StatusRecord status = _device.status[index];

            desc.szName = $"{Regex.Match(_device.name, @"\w+ \w+").Groups[0].Value} {status.key}";

            desc.szUnits = status.unit;

            desc.szGroup = _device.name;

            desc.dwID = GetSensorPluginID();

            desc.dwInstance = 0;

            desc.fltMinLimit = 0f;

            desc.fltMaxLimit = GetSensorMaxLimit(status.unit);

            desc.szFormat = GetSensorFormat(status.unit);
        }

        private static uint GetSensorPluginID()
        {
            return 0x000000FF; //MONITORING_SOURCE_ID_PLUGIN_MISC
        }

        private static float GetSensorMaxLimit(string type)
        {
            switch(type) {
                case "°C": return 100.0f;
                case "rpm": return 8000.0f;
                default: return 1000.0f;
            }
        }

        private static string GetSensorFormat(string type)
        {
            switch(type) {
                case "°C": return "%.1f";
                case "rpm": return "%.0f";
                default: return "%.3f";
            }
        }
    }
}
