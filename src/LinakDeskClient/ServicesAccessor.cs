using InTheHand.Bluetooth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinakDeskClient
{
    public class ServicesAccessor
    {
        public async Task InitAsync(RemoteGattServer server)
        {
            var services = await server.GetPrimaryServicesAsync();

            GattService resolveRequiredService(BluetoothUuid serviceId) =>
                services.SingleOrDefault(s => s.Uuid == serviceId) ?? throw new InvalidOperationException($"Missing required service: {serviceId}.");

            async Task<GattCharacteristic> resolveRequiredCharacteristic(GattService service, BluetoothUuid characteristicId) =>
                await service.GetCharacteristicAsync(characteristicId) ?? throw new InvalidOperationException($"Missing required characteristic: {characteristicId} (service {service.Uuid}).");

            // resolve services
            GenericAccessService = resolveRequiredService(LinakUuids.Services.GENERIC_ACCESS);
            GenericAttributeService = resolveRequiredService(LinakUuids.Services.GENERIC_ATTRIBUTE);
            ControlService = resolveRequiredService(LinakUuids.Services.CONTROL);
            DpgService = resolveRequiredService(LinakUuids.Services.DPG);
            ReferenceOutputService = resolveRequiredService(LinakUuids.Services.REFERENCE_OUTPUT);
            ReferenceInputService = resolveRequiredService(LinakUuids.Services.REFERENCE_INPUT);

            // resolve characteriristics
            DeviceName = await resolveRequiredCharacteristic(GenericAccessService, LinakUuids.Characteristics.GENERIC_ACCESS__DEVICE_NAME);
            Dpg = await resolveRequiredCharacteristic(DpgService, LinakUuids.Characteristics.DPG__DPG);
            ServiceChanged = await resolveRequiredCharacteristic(GenericAttributeService, LinakUuids.Characteristics.GENERIC_ATTRIBUTE__SERVICE_CHANGED);
            Control = await resolveRequiredCharacteristic(ControlService, LinakUuids.Characteristics.CONTROL__CONTROL);
            Error = await resolveRequiredCharacteristic(ControlService, LinakUuids.Characteristics.CONTROL__ERROR);
            Ctrl1 = await resolveRequiredCharacteristic(ReferenceInputService, LinakUuids.Characteristics.REFERENCE_INPUT__CTRL1);
            HeightSpeed = await resolveRequiredCharacteristic(ReferenceOutputService, LinakUuids.Characteristics.REFERENCE_OUTPUT__HEIGHT_SPEED);
            Mask = await resolveRequiredCharacteristic(ReferenceOutputService, LinakUuids.Characteristics.REFERENCE_OUTPUT__MASK);
        }

        public GattService GenericAccessService { get; private set; }
        public GattService GenericAttributeService { get; private set; }
        public GattService ControlService { get; private set; }
        public GattService DpgService { get; private set; }
        public GattService ReferenceOutputService { get; private set; }
        public GattService ReferenceInputService { get; private set; }

        public GattCharacteristic DeviceName { get; private set; }
        public GattCharacteristic Dpg { get; private set; }
        public GattCharacteristic ServiceChanged { get; private set; }
        public GattCharacteristic Control { get; private set; }
        public GattCharacteristic Error { get; private set; }
        public GattCharacteristic Ctrl1 { get; private set; }
        public GattCharacteristic HeightSpeed { get; private set; }
        public GattCharacteristic Mask { get; private set; }
    }
}
