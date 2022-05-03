namespace LinakDeskClient
{
    public class LinakBluetoothDevice
    {
        public LinakBluetoothDevice(string deviceId, string name)
        {
            DeviceId = deviceId ?? throw new System.ArgumentNullException(nameof(deviceId));
            Name = name;
        }

        public string DeviceId { get; private set; }
        public string Name { get; private set; }

        public override string ToString() => $"Id={DeviceId} Name=\"{Name}\"";
    }
}