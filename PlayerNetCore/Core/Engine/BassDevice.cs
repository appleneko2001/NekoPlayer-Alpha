using ManagedBass;
using System;
using System.Collections.Generic;
using System.Text;

namespace NekoPlayer.Core.Engine
{
    /// <summary>
    /// Device info encapsulation.
    /// And get devices function implements.
    /// </summary>
    public sealed class BassDevice : IBassDevice
    {
        #region Private members
        DeviceInfo devInfo;
        int identicator;
        #endregion
        private BassDevice(DeviceInfo info, int id)
        {
            if (!info.IsEnabled)
                throw new AccessViolationException("Device is not available for use or create object for reference.");
            identicator = id;
            devInfo = info;
        }
        public bool IsDefault => devInfo.IsDefault;
        public string DeviceName => devInfo.Name;
        public string DeviceId => devInfo.Driver;
        public string DeviceType => devInfo.Type.ToString();
        public int Identicator => identicator;



        public static IEnumerable<BassDevice> GetPlaybackDevices()
        {
            List<BassDevice> list = new List<BassDevice>();
            int dCount = Bass.DeviceCount;
            for (int i = 0; i < dCount; i++)
            {
                var dev = Bass.GetDeviceInfo(i);
                if (dev.IsEnabled && dev.Type != ManagedBass.DeviceType.Microphone)
                {
                    BassDevice obj = new BassDevice(dev, i);
                    list.Add(obj);
                }
            }
            return list;
        }
        public static IBassDevice GetDefaultDevice()
        {
            foreach (var dev in GetPlaybackDevices())
            {
                if (dev.IsDefault)
                {
                    return dev;
                }
            }
            return null;
        }
        public static IBassDevice GetBassDevice(string devId)
        {

            if (devId != null && devId.Length == 0)
                devId = null;
            foreach (var dev in GetPlaybackDevices())
            {
                if (dev.DeviceId == devId)
                {
                    return dev;
                }
            }
            return null;
        }
    }
}
