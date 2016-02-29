using System;

namespace BlinkStickDotNet.Filters
{
    public class BlinkStickAdapter : AbstractFilter
    {
        private BlinkStick m_blinkStick;

        public BlinkStickAdapter(BlinkStick blinkStick)
        {
            m_blinkStick = blinkStick;

            BlinkStickDeviceEnum deviceType = BlinkStick.BlinkStickDeviceFromSerial(blinkStick.Serial);
            int count = -1;
            switch (deviceType)
            {

                case BlinkStickDeviceEnum.BlinkStickNano:
                    count = 2;
                    break;

                case BlinkStickDeviceEnum.BlinkStick:
                    count = 1;
                    break;

                case BlinkStickDeviceEnum.BlinkStickStrip:
                case BlinkStickDeviceEnum.BlinkStickSquare:
                    count = 8;
                    break;
            }
            if (count < 0)
                throw new NotSupportedException(string.Format("{0} is not supported", deviceType));

            m_colors = new RgbColorF[count];
            updateColorsFromBlinkstick();
        }

        private void updateColorsFromBlinkstick()
        {
            byte[] colorData;
            if (!m_blinkStick.GetColors(out colorData))
            {
                for (int i = 0; i < m_colors.Length; ++i)
                    m_colors[i] = new RgbColorF();
            }
            else
            {
                for (int i = 0; i < m_colors.Length; ++i)
                    m_colors[i] = new RgbColorF(
                        colorData[3 * i + 0] / 255d,
                        colorData[3 * i + 1] / 255d,
                        colorData[3 * i + 2] / 255d
                        );
            }
        }

        public override RgbColorF[] GetAll()
        {
            RgbColorF[] rv = new RgbColorF[m_colors.Length];
            m_colors.CopyTo(rv, 0);
            return rv;
        }

        public override void SetAll(RgbColorF[] all)
        {
            all.CopyTo(m_colors, 0);
            byte[] colorData = new byte[3 * all.Length];
            for(int i = 0; i < all.Length; ++i)
            {
                colorData[3 * i + 0] = (byte)(all[i].G * 255d);
                colorData[3 * i + 1] = (byte)(all[i].R * 255d);
                colorData[3 * i + 2] = (byte)(all[i].B * 255d);
            }
            m_blinkStick.SetColors(0, colorData);
        }

        public override RgbColorF this[int index]
        {
            get
            {
                return m_colors[index];
            }

            set
            {
                m_colors[index] = value;
                m_blinkStick.SetColor(0, (byte)index, (byte)(value.R * 255d), (byte)(value.G * 255d), (byte)(value.B * 255d));
            }
        }
    }
}