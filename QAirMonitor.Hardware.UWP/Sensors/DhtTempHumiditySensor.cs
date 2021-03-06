﻿using QAirMonitor.Abstract.Sensors;
using QAirMonitor.Business.Logging;
using QAirMonitor.Domain.Enums;
using QAirMonitor.Domain.Sensors;
using Sensors.Dht;
using System;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace QAirMonitor.Hardware.UWP.Sensors
{
    public class DhtTempHumiditySensor : ITempHumiditySensor<TempHumidityReadingResult>
    {
        private GpioPin _sensorDataPin;
        private IDht _dhtSensor;

        public DhtTempHumiditySensor()
        {
            _sensorDataPin = GpioController.GetDefault().OpenPin(4, GpioSharingMode.Exclusive);
            _dhtSensor = new Dht22(_sensorDataPin, GpioPinDriveMode.Input);
        }

        public async Task<TempHumidityReadingResult> GetReadingAsync()
        {
            var reading = new TempHumidityReadingResult();
            try
            {
                DhtReading hdtReading = new DhtReading();

                hdtReading = await _dhtSensor.GetReadingAsync(500).AsTask();

                reading.Temperature = hdtReading.Temperature;
                reading.Humidity = hdtReading.Humidity;
                reading.ReadingDateTime = DateTime.Now;
                reading.Attempts = hdtReading.RetryCount + 1;
                reading.IsSuccessful = hdtReading.IsValid;

                if (hdtReading.IsValid)
                {
                    await Logger.LogAsync($"{nameof(DhtTempHumiditySensor)}", $"Successful reading. {reading.Temperature:0.0}°C, {reading.Humidity:0.0}%, {reading.Attempts} attempt(s).");
                }
                else
                {
                    await Logger.LogAsync($"{nameof(DhtTempHumiditySensor)}", $"Failed reading. {reading.Attempts} attempt(s).", AuditLogEventType.Warning);
                }

                return reading;
            }
            catch (Exception e)
            {
                await Logger.LogExceptionAsync(nameof(DhtTempHumiditySensor), e);
                return reading;
            }
        }
    }
}
