using System;

namespace BlinkStickDotNet.Filters
{
    /// <summary>
    /// Abstract class for implementation convinience.
    /// <para>
    /// Implement <see cref="ConvertOnGet(RgbColorF)"/> and <see cref="ConvertOnSet(RgbColorF)"/> in order to 
    /// create filter. <see cref="ConvertOnGet(RgbColorF)"/> should provide inverse transformation 
    /// to <see cref="ConvertOnSet(RgbColorF)"/>.
    /// </para>
    /// <para>
    /// If you want to change filtration parameters in run time you shold implement 
    /// specified properties similar to <seealso cref="BrightnessFilter.Brightness"/> 
    /// </para>
    /// </summary>
    public abstract class AbstractFilter : IFilter
    {
        /// <summary>
        /// Base constructor for typical filters
        /// </summary>
        /// <param name="underlyingFilter"></param>
        protected AbstractFilter(IFilter underlyingFilter)
        {
            m_underlyingFilter = underlyingFilter;
            m_colors = underlyingFilter.GetAll();
        }

        /// <summary>
        /// Constructor for special purposes - see: <see cref="BlinkStickAdapter"/>
        /// </summary>
        protected AbstractFilter()
        {
        }

        /// <summary>
        /// Cache for current colors. Useful in case of changing filtering parameters (ex: <see cref="BrightnessFilter"/>
        /// </summary>
        protected RgbColorF[] m_colors = null;
        protected IFilter m_underlyingFilter = null;

        public virtual void SetAll(RgbColorF[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            if (values.Length != Count)
                throw new ArgumentException(string.Format("Array with lenght {0} expected, {1} used", values.Length, Count));

            RgbColorF[] temp = new RgbColorF[values.Length];
            for (int i = 0; i < m_colors.Length; ++i)
            {
                m_colors[i] = values[i];
                temp[i] = ConvertOnSet(values[i]);
            }

            m_underlyingFilter.SetAll(temp);
        }

        public virtual RgbColorF[] GetAll()
        {
            RgbColorF[] underlyingValues = m_underlyingFilter.GetAll();
            RgbColorF[] values = new RgbColorF[Count];
            for (int i = 0; i < values.Length; ++i)
            {
                values[i] = ConvertOnGet(underlyingValues[i]);
                m_colors[i] = values[i];
            }
            return values;
        }

        public virtual RgbColorF this[int index]
        {
            get
            {
                RgbColorF underlyingValue = m_underlyingFilter[index];
                RgbColorF value = ConvertOnGet(underlyingValue);
                m_colors[index] = value;
                return value;
            }

            set
            {
                m_colors[index] = value;
                m_underlyingFilter[index] = ConvertOnSet(value);
            }
        }

        protected virtual RgbColorF ConvertOnGet(RgbColorF underlyingValue) { return underlyingValue; }
        protected virtual RgbColorF ConvertOnSet(RgbColorF value) { return value; }

        public int Count
        {
            get
            {
                return m_colors.Length;
            }
        }
    }
}
