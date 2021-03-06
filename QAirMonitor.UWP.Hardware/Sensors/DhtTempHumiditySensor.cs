﻿using QAirMonitor.Abstract.Sensors;
using QAirMonitor.Domain.Models;
using Sensors.Dht;
using System;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace QAirMonitor.Hardware.UWP.Sensors
{
    public class DhtTempHumiditySensor : ITempHumiditySensor<ReadingModel>
    {
        private GpioPin _sensorDataPin;
        private IDht _dhtSensor;

        public DhtTempHumiditySensor()
        {
            _sensorDataPin = GpioController.GetDefault().OpenPin(4, GpioSharingMode.Exclusive);
            _dhtSensor = new Dht22(_sensorDataPin, GpioPinDriveMode.Input);
        }

        public async Task<ReadingModel> GetReadingAsync()
        {
            var reading = new ReadingModel();
            try
            {
                DhtReading hdtReading = new DhtReading();

                hdtReading = await _dhtSensor.GetReadingAsync(250).AsTask();

                if (hdtReading.IsValid)
                {
                    reading.Temperature = hdtReading.Temperature;
                    reading.Humidity = hdtReading.Humidity;
                    reading.ReadingDateTime = DateTime.Now;

                    return reading;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                // TODO: Log failure.
                return null;
            }
        }
    }
}
