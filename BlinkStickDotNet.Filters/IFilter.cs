namespace BlinkStickDotNet.Filters
{
    /// <summary>
    /// Immutable RGB color representation. Values range: 0..1d
    /// </summary>
    public class RgbColorF
    {
        public double R { get; private set; }
        public double G { get; private set; }
        public double B { get; private set; }

        public RgbColorF(double r, double g, double b)
        {
            R = r;
            G = g;
            B = b;
        }

        public RgbColorF()
        {
            R = 0d;
            G = 0d;
            B = 0d;
        }

        public RgbColorF(RgbColor r)
        {
            R = r.R / 255d;
            G = r.G / 255d;
            B = r.B / 255d;
        }

        public RgbColor ToRgbColor()
        {
            return RgbColor.FromRgb((int)(R * 255), (int)(G * 255), (int)(B * 255));
        }

    }

    /// <summary>
    /// Common interface for all filters in stack
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// Sets all LEDs with specified colors
        /// </summary>
        /// <param name="all">Colors to set</param>
        void SetAll(RgbColorF[] all);

        /// <summary>
        /// Get all LEDs colors as array with <see cref="Count"/> lenght
        /// </summary>
        /// <returns></returns>
        RgbColorF[] GetAll();

        /// <summary>
        /// Gets or sets specified LED
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        RgbColorF this[int index] { get; set; }

        /// <summary>
        /// Gets number of LEDs
        /// </summary>
        int Count { get; }
    }

}
