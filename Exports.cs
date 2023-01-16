/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LiquidCtlAfterburnerPlugin
{
    /// <summary>
    /// Native DLL Exports
    /// Implementing plugin interface according to SDK samples included with MSI Afterburner 4.6.3
    /// </summary>
    public class Exports
    {
        private static readonly List<SensorSource> _devices = new List<SensorSource>();
        private static Boolean hasBeenInitialized = false;
        private static Task LiquidCtlCliPollTask;

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
                return (uint)_devices.First().SensorCount;
            }
            catch (Exception e)
            {
                try
                {
                    File.AppendAllText(System.Reflection.Assembly.GetExecutingAssembly().Location + ".log", $"{DateTime.Now:s}: {e}\r\n");
                }
                catch { }

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
                _devices.First().FillDescription((int)dwIndex, ref pDesc);

                return true;
            }
            catch (Exception e)
            {
                try
                {
                    File.AppendAllText(System.Reflection.Assembly.GetExecutingAssembly().Location + ".log", $"{DateTime.Now:s}: {e}\r\n");
                }
                catch { }

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
                if (LiquidCtlCliPollTask == null || LiquidCtlCliPollTask.IsCompleted)
                    LiquidCtlCliPollTask = Task.Run(() => _devices.First()._device.LoadJSON());
                return _devices.First().SensorValue((int)dwIndex);
            }
            catch (Exception e)
            {
                try
                {
                    File.AppendAllText(System.Reflection.Assembly.GetExecutingAssembly().Location + ".log", $"{DateTime.Now:s}: {e}\r\n");
                }
                catch { }

                return 0f;
            }
        }

        private static void Initialize()
        {
            File.AppendAllText(System.Reflection.Assembly.GetExecutingAssembly().Location + ".log", $"{DateTime.Now:s}: Plugin started\r\n");
            LiquidctlCLIWrapper.Initialize();
            List<LiquidctlStatusJSON> input = LiquidctlCLIWrapper.ReadStatus();
            foreach (LiquidctlStatusJSON liquidctl in input)
            {
                LiquidctlDevice device = new LiquidctlDevice(liquidctl);
                File.AppendAllText(System.Reflection.Assembly.GetExecutingAssembly().Location + ".log", $"{DateTime.Now:s}: {device.GetDeviceInfo()}\r\n");
                _devices.Add(new SensorSource(device));
            }
            hasBeenInitialized = true;
        }
    }
}
