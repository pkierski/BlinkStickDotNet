#region License
// Copyright 2013 by Agile Innovative Ltd
//
// This file is part of BlinkStick.HID library.
//
// BlinkStick.HID library is free software: you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published by the Free 
// Software Foundation, either version 3 of the License, or (at your option) any 
// later version.
//		
// BlinkStick.HID library is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with 
// BlinkStick.HID library. If not, see http://www.gnu.org/licenses/.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using HidSharp;

namespace BlinkStick.Hid
{
    /// <summary>
    /// Main class to access Blinkstick HID devices.
    /// </summary>
	public class BlinkstickHid : IDisposable
    {
        #region Private Properties
        protected const int VendorId = 0x20A0;
        protected const int ProductId = 0x41E5;

        private HidDevice device;
        private HidStream stream;

        private bool disposed = false;

        protected bool connectedToDriver = false;

        private bool _RequiresSoftwareColorPatch = false;
        #endregion

        #region Device Properties
        /// <summary>
        /// Gets a value indicating whether this <see cref="BlinkStick.Hid.BlinkstickHid"/> is connected.
        /// </summary>
        /// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
        public Boolean Connected {
            get {
                return connectedToDriver;
            }
        }

        /// <summary>
        /// Returns the serial number of BlinkStick.
        /// BSnnnnnn-1.0
        /// ||  |    | |- Software minor version
        /// ||  |    |--- Software major version
        /// ||  |-------- Denotes sequential number
        /// ||----------- Denotes BlinkStick device
        /// 
        /// Software version defines the capabilities of the device
        /// </summary>
        /// <value>The serial.</value>
        public String Serial {
            get {
                return device.SerialNumber;
            }
        }

        private int _VersionMajor = -1;

        /// <summary>
        /// Gets the major version number from serial number.
        /// </summary>
        /// <value>The major version  number.</value>
        public int VersionMajor {
            get {
                if (_VersionMajor == -1)
                {
                    try
                    {
                        _VersionMajor = Convert.ToInt32(this.Serial.Substring(this.Serial.Length - 1, 1));
                    }
                    catch
                    {
                        _VersionMajor = 0;
                    }
                }

                return _VersionMajor;
            }
        }

        private int _VersionMinor = -1;

        /// <summary>
        /// Gets the minor version number from serial number.
        /// </summary>
        /// <value>The version minor.</value>
        public int VersionMinor {
            get {
                if (_VersionMinor == -1)
                {
                    try
                    {
                        _VersionMinor = Convert.ToInt32(this.Serial.Substring(this.Serial.Length - 3, 1));
                    }
                    catch
                    {
                        _VersionMinor = 0;
                    }
                }

                return _VersionMinor;
            }
        }

        /// <summary>
        /// Gets the name of the manufacturer.
        /// </summary>
        /// <value>The name of the manufacturer.</value>
        public String ManufacturerName {
            get {
                return device.Manufacturer;
            }
        }

        /// <summary>
        /// Gets the product name of the device.
        /// </summary>
        /// <value>The name of the product.</value>
        public String ProductName {
            get {
                return device.ProductName;
            }
        }

        private String _InfoBlock1;
        /// <summary>
        /// Gets or sets the name of the device (InfoBlock1).
        /// </summary>
        /// <value>The name.</value>
        public String InfoBlock1 {
            get {
                if (_InfoBlock1 == null) {
                    GetInfoBlock (2, out _InfoBlock1);
                }

                return _InfoBlock1;
            }
            set {
                if (_InfoBlock1 != value)
                {
                    _InfoBlock1 = value;
                    SetInfoBlock(2, _InfoBlock1);
                }
            }
        }

        private String _InfoBlock2;
        /// <summary>
        /// Gets or sets the data of the device (InfoBlock2).
        /// </summary>
        /// <value>The data.</value>
        public String InfoBlock2 {
            get {
                if (_InfoBlock2 == null) {
                    GetInfoBlock (3, out _InfoBlock2);
                }

                return _InfoBlock2;
            }
            set {
                if (_InfoBlock2 != value)
                {
                    _InfoBlock2 = value;
                    SetInfoBlock(3, _InfoBlock2);
                }
            }
        }
        #endregion

        #region Constructor/Destructor
        /// <summary>
        /// Initializes a new instance of the BlinkstickHid class.
        /// </summary>
        public BlinkstickHid()
        {
        }

        /// <summary>
        /// Closes the connection to the device.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Closes any connected devices.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if(!this.disposed)
            {
                if(disposing)
                {
                    CloseDevice();
                }

                disposed = true;
            }
        }

        /// <summary>
        /// Destroys instance and frees device resources (if not freed already)
        /// </summary>
        ~BlinkstickHid()
        {
            Dispose(false);
        }
        #endregion

        #region Device Open/Close functions
        /// <summary>
        /// Attempts to connect to a BlinkStick device.
        /// 
        /// After a successful connection, a DeviceAttached event will normally be sent.
        /// </summary>
        /// <returns>True if a Blinkstick device is connected, False otherwise.</returns>
        public bool OpenDevice ()
        {
            bool result;

            this._VersionMajor = -1;
            this._VersionMinor = -1;

            if (this.device == null) {
                HidDeviceLoader loader = new HidDeviceLoader();
                HidDevice adevice = loader.GetDevices(VendorId, ProductId).FirstOrDefault();
                result = OpenDevice (adevice);
            } else {
                result = OpenCurrentDevice();
            }

            CheckRequiresSoftwareColorPatch();

            return result;
        }

        /// <summary>
        /// Opens the device.
        /// </summary>
        /// <returns><c>true</c>, if device was opened, <c>false</c> otherwise.</returns>
        /// <param name="adevice">Pass the parameter of HidDevice to open it directly</param>
        public bool OpenDevice(HidDevice adevice)
        {
            if (adevice != null)
            {
                this.device = adevice;

                return OpenCurrentDevice();
            }

            return false;
        }

        /// <summary>
        /// Opens the current device.
        /// </summary>
        /// <returns><c>true</c>, if current device was opened, <c>false</c> otherwise.</returns>
        private bool OpenCurrentDevice()
        {
            connectedToDriver = true;
            device.TryOpen(out stream);

            return true;
        }

        /// <summary>
        /// Closes the connection to the device.
        /// </summary>
        public void CloseDevice()
        {
            stream.Close();
            device = null;
            connectedToDriver = false;
        }
        #endregion

        #region Helper functions for InfoBlocks
        /// <summary>
        /// Sets the info block.
        /// </summary>
        /// <param name="id">2 - InfoBlock1, 3 - InfoBlock2</param>
        /// <param name="data">Maximum 32 bytes of data</param>
        private void SetInfoBlock (byte id, string data)
        {
            SetInfoBlock(id, Encoding.ASCII.GetBytes(data));
        }

        private Boolean GetInfoBlock (byte id, out string data)
        {
            byte[] dataBytes;
            Boolean result = GetInfoBlock (id, out dataBytes);

            if (result) {
                for (int i = 1; i < dataBytes.Length; i++) {
                    if (dataBytes [i] == 0) {
                        Array.Resize (ref dataBytes, i);
                        break;
                    }
                }

                data = Encoding.ASCII.GetString (dataBytes, 1, dataBytes.Length - 1);
            } else {
                data = "";
            }

            return result;
        }

        protected void SetInfoBlock (byte id, byte[] data)
        {
            if (id == 2 || id == 3) {
                if (data.Length > 32)
                {
                    Array.Resize(ref data, 32);
                }
                else if (data.Length < 32)
                {
                    int size = data.Length;

                    Array.Resize(ref data, 32);

                    //pad with zeros
                    for (int i = size; i < 32; i++)
                    {
                        data[i] = 0;
                    }
                }

                Array.Resize(ref data, 33);


                for (int i = 32; i >0; i--)
                {
                    data[i] = data[i-1];
                }

                data[0] = id;

                stream.SetFeature(data);
            } else {
                throw new Exception("Invalid info block id");
            }
        }

        /// <summary>
        /// Gets the info block.
        /// </summary>
        /// <returns><c>true</c>, if info block was received, <c>false</c> otherwise.</returns>
        /// <param name="id">Identifier.</param>
        /// <param name="data">Data.</param>
        public Boolean GetInfoBlock (byte id, out byte[] data)
        {
            if (id == 2 || id == 3) {
                data = new byte[33];
                data[0] = id;

                if (connectedToDriver)
                {
                    stream.GetFeature(data, 0, data.Length);
                    return true;
                }
                else
                {
                    data = new byte[0];
                    return false;
                }
            } else {
                throw new Exception("Invalid info block id");
            }
        }
        #endregion

        #region Color manipulation functions
        /// <summary>
        /// Sets the color of the led.
        /// </summary>
        /// <param name="color">Must be in #rrggbb format</param>
        public void SetColor(String color)
        {
            SetColor(RgbColor.FromString(color));
        }

        /// <summary>
        /// Sets the color of the led.
        /// </summary>
        /// <param name="color">Color as RgbColor class.</param>
        public void SetColor(RgbColor color)
        {
            SetColor(color.R, color.G, color.B);
        }

        /// <summary>
        /// Sets the color of the led.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        public void SetColor(byte r, byte g, byte b)
        {
            if (connectedToDriver)
            {
                if (_RequiresSoftwareColorPatch)
                {
                    byte cr, cg, cb;
                    if (GetColor(out cr, out cg, out cb))
                    {
                        if (r == cg && g == cr && b == cb)
                        {
                            if (cr > 0)
                            {
                                stream.SetFeature(new byte[4] { 1, (byte)(cr - 1), cg, cb });
                            }
                            else if (cg > 0)
                            {
                                stream.SetFeature(new byte[4] { 1, cr, (byte)(cg - 1), cb });
                            }
                        }
                    }
                }

                stream.SetFeature(new byte[4] {1, r, g, b});
            }
        }

        /// <summary>
        /// Gets the color of the led.
        /// </summary>
        /// <returns><c>true</c>, if led color was received, <c>false</c> otherwise.</returns>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        public Boolean GetColor (out byte r, out byte g, out byte b)
        {
            byte[] report = new byte[33]; 
            report[0] = 1;

            if (connectedToDriver) {
                stream.GetFeature(report, 0, 33);

                r = report [1];
                g = report [2];
                b = report [3];

                return true;
            } else {
                r = 0;
                g = 0;
                b = 0;

                return false;
            }
        }

        /// <summary>
        /// Turn BlinkStick off.
        /// </summary>
        public void TurnOff()
        {
            SetColor(0, 0, 0);
        }
        #endregion

        #region Color manipulation functions for BlinkStick Pro
        /// <summary>
        /// Sets the color of the led.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        public void SetColor(byte channel, byte index, byte r, byte g, byte b)
        {
            if (connectedToDriver)
            {
                stream.SetFeature(new byte[6] {5, channel, index, r, g, b} );
            }
        }

        /// <summary>
        /// Sets the color of the led.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        public void SetColor(byte channel, byte index, string color)
        {
            SetColor(channel, index, RgbColor.FromString(color));
        }

        /// <summary>
        /// Sets the color of the led.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Color parameter as RgbColor class instance</param>
        public void SetColor(byte channel, byte index, RgbColor color)
        {
            SetColor(channel, index, color.R, color.B, color.G);
        }

        /// <summary>
        /// Send a packet of data to LEDs
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="reportData">Report data must be a byte array in the following format: [g0, r0, b0, g1, r1, b1, g2, r2, b2 ...]</param>
        public void SetColors(byte channel, byte[] colorData)
        {
            int max_leds = 64;
            byte reportId = 9;

            //Automatically determine the correct report id to send the data to
            if (colorData.Length <= 8 * 3)
            {
                max_leds = 8;
                reportId = 6;
            }
            else if (colorData.Length <= 16 * 3)
            {
                max_leds = 16;
                reportId = 7;
            }
            else if (colorData.Length <= 32 * 3)
            {
                max_leds = 32;
                reportId = 8;
            }
            else if (colorData.Length <= 64 * 3)
            {
                max_leds = 64;
                reportId = 9;
            }
            else if (colorData.Length <= 128 * 3)
            {
                max_leds = 64;
                reportId = 10;
            }

            byte [] data = new byte[max_leds * 3 + 2];
            data[0] = reportId;
            data[1] = channel; // chanel index

            for (int i = 0; i < Math.Min(colorData.Length, data.Length - 2); i++)
            {
                data[i + 2] = colorData[i];
            }

            for (int i = colorData.Length + 2; i < data.Length; i++)
            {
                data[i] = 0;
            }

            stream.SetFeature(data);

            if (reportId == 10)
            {
                for (int i = 0; i < Math.Min(data.Length - 2, colorData.Length - 64 * 3); i++)
                {
                    data[i + 2] = colorData[64 * 3 + i];
                } 

                for (int i = colorData.Length + 2 - 64 * 3; i < data.Length; i++)
                {
                    data[i] = 0;
                }

                data[0] = (byte)(reportId + 1);

                stream.SetFeature(data);
            }
        } 

        /// <summary>
        /// Gets led data.
        /// </summary>
        /// <returns><c>true</c>, if led data was received, <c>false</c> otherwise.</returns>
        /// <param name="data">LED data as an array of colors [G0, R0, B0, G1, R1, B1 ...]</param>
        public Boolean GetColors (out byte[] colorData)
        {
            if (connectedToDriver)
            {
                colorData = new byte[3 * 8 * 8 + 1];
                colorData[0] = 9;
                stream.GetFeature(colorData, 0, colorData.Length);
                return true;
            }
            else
            {
                colorData = new byte[0];
                return false;
            }

        }

        #endregion

        #region BlinkStick Pro mode selection
        /// <summary>
        /// Sets the mode for BlinkStick Pro.
        /// </summary>
        /// <param name="mode">0 - Normal, 1 - Inverse, 2 - WS2812</param>
        public void SetMode(byte mode)
        {
            if (connectedToDriver)
            {
                stream.SetFeature(new byte[2] {4, mode});
            }
        }
        #endregion

        #region Static Functions to initialize BlinkSticks
        /// <summary>
        /// Find all BlinkStick devices.
        /// </summary>
        /// <returns>The devices.</returns>
        public static BlinkstickHid[] AllDevices ()
		{
            List<BlinkstickHid> result = new List<BlinkstickHid>();

            HidDeviceLoader loader = new HidDeviceLoader();
            foreach (HidDevice adevice in loader.GetDevices(VendorId, ProductId).ToArray())
            {
                BlinkstickHid hid = new BlinkstickHid();
                hid.device = adevice;
                result.Add(hid);
            }

            return result.ToArray();      
        }

        /// <summary>
        /// Find first BlinkStick.
        /// </summary>
        /// <returns>BlinkStickHid device if found, otherwise null if no devices found</returns>
        public static BlinkstickHid FirstDevice()
        {
            BlinkstickHid[] devices = AllDevices();

            return devices.Length > 0 ? devices[0] : null;
        }

        /// <summary>
        /// Finds BlinkStick by serial number.
        /// </summary>
        /// <returns>BlinkStickHid device if found, otherwise null if no devices found</returns>
        /// <param name="serial">Serial number to search for</param>
        public static BlinkstickHid FindBySerial(String serial)
        {
            foreach (BlinkstickHid device in AllDevices())
            {
                if (device.Serial == serial)
                    return device;
            }

            return null;
        }
        #endregion

        #region Misc helper functions
        /// <summary>
        /// Checks if BlinkStick requires software color patch due to hardware bug.
        /// </summary>
        /// <returns><c>true</c>, if requires software color patch, <c>false</c> otherwise.</returns>
        private void CheckRequiresSoftwareColorPatch()
        {
            _RequiresSoftwareColorPatch = VersionMajor == 1 && VersionMinor >= 1 && VersionMinor <= 3;
        }
        #endregion
	}
}

