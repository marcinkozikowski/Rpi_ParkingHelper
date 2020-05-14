using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPi_ParkingHelper
{
    internal class LightController
    {
        private readonly int _green;
        private readonly int _yellow;
        private readonly int _red;
        private readonly GpioController _gpio;

        public int GreenLight => _green;
        public int YellowLight => _yellow;
        public int RedLight => _red;

        public LightController(GpioController gpio)
        {
            _green = 22;
            _yellow = 27;
            _red = 17;
            _gpio = gpio;
        }
        public LightController(GpioController gpio,int redLightPin,int yellowLightPin, int greenLightPin)
        {
            _green = greenLightPin;
            _yellow = yellowLightPin;
            _red = redLightPin;
            _gpio = gpio;
        }

        public void TurnOnLight(Lights light)
        {
            _gpio.Write((int)light, PinValue.Low);
        }
        public void TurnOffLight(Lights light)
        {
            _gpio.Write((int)light, PinValue.High);
        }

        public Lights InterprateDistance(double distance)
        {
            if (distance > 0 && distance < 20)
            {
                TurnOffLight(Lights.Green);
                TurnOffLight(Lights.Yellow);
                TurnOnLight(Lights.Red);
                return Lights.Red;
            }
            else if (distance > 20 && distance < 60)
            {
                TurnOffLight(Lights.Green);
                TurnOnLight(Lights.Yellow);
                TurnOffLight(Lights.Red);
                return Lights.Yellow;
            }
            else if (distance > 60)
            {
                TurnOnLight(Lights.Green);
                TurnOffLight(Lights.Yellow);
                TurnOffLight(Lights.Red);
                return Lights.Green;
            }
            return Lights.Red;
        }

        public void TutnOffAllLights()
        {
            TurnOffLight(Lights.Green);
            TurnOffLight(Lights.Yellow);
            TurnOffLight(Lights.Red);
        }
    }

    public enum Lights
    {
        Green=22,
        Yellow=27,
        Red=17,
    }
}
