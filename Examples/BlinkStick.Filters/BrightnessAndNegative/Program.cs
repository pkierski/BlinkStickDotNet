using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BlinkStickDotNet;
using BlinkStickDotNet.Filters;

namespace BrightnessAndNegative
{
    class Program
    {
        static void Main(string[] args)
        {
            BlinkStick blinkStickDevice = BlinkStick.FindFirst();
            if(blinkStickDevice != null && blinkStickDevice.OpenDevice())
            {
                // setup filters stack
                BlinkStickAdapter adapter = new BlinkStickAdapter(blinkStickDevice);
                NegativeFilter negativeFilter = new NegativeFilter(adapter);
                BrightnessFilter brightnessFilter = new BrightnessFilter(negativeFilter);

                // set random colors for all LEDs
                Random rand = new Random();
                for (int i = 0; i < brightnessFilter.Count; ++i)
                {
                    brightnessFilter[i] = new RgbColorF(rand.NextDouble(), rand.NextDouble(), rand.NextDouble());
                    System.Threading.Thread.Sleep(10);
                }

                // fade out and in
                Console.WriteLine("press any key to fade out and in...");
                Console.ReadKey();
                fadeOut(brightnessFilter);
                fadeIn(brightnessFilter);

                // negative
                Console.WriteLine("press any key to invert colors...");
                Console.ReadKey();
                negativeFilter.Enable = true;

                // fade in and out
                Console.WriteLine("press any key to fade in and out in negative...");
                Console.ReadKey();
                fadeIn(brightnessFilter);
                fadeOut(brightnessFilter);

                // back to positive
                Console.WriteLine("press any key to switch off color inversion...");
                Console.ReadKey();
                negativeFilter.Enable = false;

                // final fade out
                Console.WriteLine("press any key to fade out...");
                Console.ReadKey();
                fadeOut(brightnessFilter);

                blinkStickDevice.CloseDevice();
            }
        }

        private static void fadeOut(BrightnessFilter brightnessFilter, int steps = 100, int stepInterval = 20)
        {
            for (int i = 0; i <= steps; ++i)
            {
                brightnessFilter.Brightness = 1 - (double)i / steps;
                System.Threading.Thread.Sleep(stepInterval);
            }
        }

        private static void fadeIn(BrightnessFilter brightnessFilter, int steps = 100, int stepInterval = 20)
        {
            for (int i = 0; i <= steps; ++i)
            {
                brightnessFilter.Brightness = (double)i / steps;
                System.Threading.Thread.Sleep(stepInterval);
            }
        }
    }
}
