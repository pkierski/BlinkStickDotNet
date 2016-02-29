namespace BlinkStickDotNet.Filters
{
    public class BrightnessFilter : AbstractFilter
    {
        public BrightnessFilter(IFilter underlyingFilter) : base(underlyingFilter)
        {
        }

        private double m_brightness = 1d;

        public double Brightness
        {
            get
            {
                m_colors = GetAll();
                return m_brightness;
            }

            set
            {
                m_brightness = value;
                SetAll(m_colors);
            }
        }

        protected override RgbColorF ConvertOnGet(RgbColorF underlyingValue)
        {
            return new RgbColorF(underlyingValue.R / m_brightness, underlyingValue.G / m_brightness, underlyingValue.B / m_brightness);
        }

        protected override RgbColorF ConvertOnSet(RgbColorF value)
        {
            return new RgbColorF(value.R * m_brightness, value.G * m_brightness, value.B * m_brightness);
        }
    }
}
