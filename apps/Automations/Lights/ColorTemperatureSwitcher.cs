namespace HomeAssistantAutomations.apps.Automations.Lights
{
    public class ColorTemperatureSwitcher
    {
        private readonly int[] _availableColorTemperatures;
        private int _currentColorTemperatureIndex;
        private SwitchDirection _currentColorTemperatureSwitchDirection;

        public ColorTemperatureSwitcher(params int[] colorTemperatures)
        {
            _availableColorTemperatures = colorTemperatures;
        }

        public int GetNextColorTemperature()
        {
            int value = _availableColorTemperatures[_currentColorTemperatureIndex];

            _currentColorTemperatureIndex += (int)_currentColorTemperatureSwitchDirection;

            if (_currentColorTemperatureIndex == _availableColorTemperatures.Length - 1)
            {
                _currentColorTemperatureSwitchDirection = SwitchDirection.Down;
            }

            if (_currentColorTemperatureIndex == 0)
            {
                _currentColorTemperatureSwitchDirection = SwitchDirection.Up;
            }

            return value;
        }

        private enum SwitchDirection
        {
            Up = 1,
            Down = -1
        }
    }
}