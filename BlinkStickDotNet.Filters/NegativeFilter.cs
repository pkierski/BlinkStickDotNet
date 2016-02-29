namespace BlinkStickDotNet.Filters
{
    public class NegativeFilter : AbstractFilter
    {
        public NegativeFilter(IFilter f) : base(f)
        {
        }

        private bool m_enable = false;

        public bool Enable
        {
            get
            {
                m_colors = GetAll();
                return m_enable;
            }
            set
            {
                m_enable = true;
                SetAll(m_colors);
            }
        }

        private RgbColorF negative(RgbColorF v)
        {
            return new RgbColorF(1 - v.R, 1 - v.G, 1 - v.B);
        }

        protected override RgbColorF ConvertOnGet(RgbColorF underlyingValue)
        {
            if (m_enable)
                underlyingValue = negative(underlyingValue);
            return underlyingValue;
        }

        protected override RgbColorF ConvertOnSet(RgbColorF value)
        {
            return ConvertOnGet(value);
        }
    }

}