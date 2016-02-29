using System;
using System.Collections.Generic;

namespace BlinkStickDotNet.Filters
{
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

    public interface IFilter : IList<RgbColorF>
    {
        void SetAll(RgbColorF[] all);
        RgbColorF[] GetAll();
    }

}
