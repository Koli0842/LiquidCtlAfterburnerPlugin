/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace LiquidCtlAfterburnerPlugin
{
    /// <summary>
    /// Native DLL Exports
    /// Implementing plugin interface according to SDK samples included with MSI Afterburner 4.6.3
    /// </summary>
    public class Exports
    {
        private static readonly List<SensorSource> _devices = new List<SensorSource>();
        private static Dictionary<int, SensorSource> _devicesMapping = new Dictionary<int, SensorSource>();
        private static Dictionary<int, int> _indexOffsetMapping = new Dictionary<int, int>();
        private static Boolean hasBeenInitialized = false;

        [DllExport]
        public static bool SetupSource(uint dwIndex, IntPtr hWnd)
        {
            return false;
        }

        [DllExport]
        public static uint GetSourcesNum()
        {
            try
            {
                if (!hasBeenInitialized)
                    Initialize();
                return (uint)_devices.Select(d => d.SensorCount).Sum();
            }
            catch (Exception e)
            {
                Log(e.ToString());
                return 0u;
            }
        }

        [DllExport]
        public static bool GetSourceDesc(uint dwIndex, ref MonitoringSourceDesc pDesc)
        {
            try
            {
                if (!hasBeenInitialized)
                    Initialize();
                _devicesMapping[(int)dwIndex].FillDescription(GetOffsetIndex(dwIndex), ref pDesc);

                return true;
            }
            catch (Exception e)
            {
                Log(e.ToString());
                return false;
            }
        }

        [DllExport]
        public static float GetSourceData(uint dwIndex)
        {
            try
            {
                if (!hasBeenInitialized)
                    Initialize();

                _devicesMapping[(int)dwIndex].Reload();
                return _devicesMapping[(int)dwIndex].SensorValue(GetOffsetIndex(dwIndex));
            }
            catch (Exception e)
            {
                Log(e.ToString());
                return 0f;
            }
        }

        private static void Initialize()
        {
            Log("Plugin started");
            LiquidctlCLIWrapper.Initialize();
            List<LiquidctlStatusJSON> input = LiquidctlCLIWrapper.ReadStatus();
            foreach (LiquidctlStatusJSON liquidctl in input)
            {
                LiquidctlDevice device = new LiquidctlDevice(liquidctl);
                if (0 < device.status.Count)
                {
                    Log(device.GetDeviceInfo());
                    _devices.Add(new SensorSource(device));
                }
            }
            FillDeviceMapping();
            hasBeenInitialized = true;
        }

        private static void FillDeviceMapping()
        {
            using(IEnumerator<SensorSource> enumerator = _devices.GetEnumerator())
            {
                enumerator.MoveNext();
                int storedDeviceLastIndex = 0;
                int sensorCount = _devices.Select(d => d.SensorCount).Sum();
                for(int i = 0; i < sensorCount; i++)
                {
                    if (enumerator.Current.SensorCount + storedDeviceLastIndex == i)
                    {
                        enumerator.MoveNext();
                        storedDeviceLastIndex = i;
                    }
                    _devicesMapping.Add(i, enumerator.Current);
                    _indexOffsetMapping.Add(i, storedDeviceLastIndex);
                }
            }
        }

        private static int GetOffsetIndex(uint dwIndex)
        {
            return (int)dwIndex - _indexOffsetMapping[(int)dwIndex];
        }

        private static void Log(string message)
        {
            try
            {
                File.AppendAllText(System.Reflection.Assembly.GetExecutingAssembly().Location + ".log", $"{DateTime.Now:s}: {message}\r\n");
            }
            catch { }
        }
    }
}
