using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RPi_ParkingHelper
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private LightController _lightController;
        private DispatcherTimer _timer;
        private int[] _pinNumbers = new[] { 17, 27, 22 };
        private HCSR04 ultrasonicMeter;
        private GpioController _gpio;

        public MainPage()
        {
            this.InitializeComponent();
            if (InitGPIO())
            {
                InitTimer();
            }
        }

        private void InitTimer()
        {
            _lightController.TurnOffLight(RPi_ParkingHelper.Lights.Green);
            _lightController.TurnOffLight(RPi_ParkingHelper.Lights.Yellow);
            _lightController.TurnOffLight(RPi_ParkingHelper.Lights.Red);

            var distances = new Queue<Lights>();
            //var lights = new[] { RedLed, YellowLed, GreenLed };
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1200) };
            _timer.Tick += (s, e) =>
            {
                _timer.Stop();
                double distance = 0;
                var isValid = ultrasonicMeter.GetDistance(out distance);
                if(isValid)
                {
                    var li = _lightController.InterprateDistance(distance);
                    distances.Enqueue(li);
                }
                if(!IsStillMoving(distances))
                {
                    _lightController.TutnOffAllLights();
                }
                _timer.Start();
            };
            _timer.Start();
        }

        private bool IsStillMoving(Queue<Lights> distances)
        {
            if(distances.Count() == 10)
            {
                foreach (var li in distances)
                {
                    if (li != distances.ElementAt(0))
                    {
                        distances.Dequeue();
                        return true;
                    }
                }
                distances.Dequeue();
                return false;
            }
            return true;
        }

        private bool InitGPIO()
        {
            _gpio = new GpioController(PinNumberingScheme.Logical);
            _lightController = new LightController(_gpio);
            if (_gpio == null)
                return false;
            for (int i = 0; i < 3; i++)
            {
                _gpio.OpenPin(_pinNumbers[i],PinMode.Output);
                _gpio.Write(_pinNumbers[i], PinValue.High);
            }

            ultrasonicMeter = new HCSR04(_gpio, 23, 24, true);

            return true;
        }
    }

    
}
