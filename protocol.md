# Reverse Engineering LINAK Bluetooth Protocol

## Bluetooth Reference Services (IKEA IDASEN unit)

This is list of services, characteristics and descriptors of IKEA IDASEN bluetooth unit.

```
Device: Id=000000000000 <<REDACTED>>; Name="Desk 1234"
Server: Conencted=True; AutoConnect=False; Mtu=23; PreferredPhy=Le1m
Server hierarchy:
  Services (6):

    - Service IsPrimary=True; Uuid=00001800-0000-1000-8000-00805f9b34fb; HasCharacteristics=True; HasInlucededServices=False // GENERIC_ACCESS
      Characteristics (4):
        - Characteristic Uuid=00002a00-0000-1000-8000-00805f9b34fb; Value=0x4465736b<<REDACTED>> (Length=9B; Str="Desk 1234"); HasDescriptors=False // GENERIC_ACCESS/DEVICE_NAME (string; UTF8)
          Properties: Read, Write
        - Characteristic Uuid=00002a01-0000-1000-8000-00805f9b34fb; Value=0x0000 (Length=2B; Str=""); HasDescriptors=False // GENERIC_ACCESS/??? (unused, appearance?)
          Properties: Read
        - Characteristic Uuid=00002a04-0000-1000-8000-00805f9b34fb; Value=0x1000300000003200 (Length=8B; Str="►02"); HasDescriptors=False // GENERIC_ACCESS/MANUFACTURER (unused; manufacturer string; but on IKEA IDASEN is binary - why?)
          Properties: Read
        - Characteristic Uuid=00002aa6-0000-1000-8000-00805f9b34fb; Value=0x01 (Length=1B; Str="☺"); HasDescriptors=False // GENERIC_ACCESS/??? (unused)
          Properties: Read

    - Service IsPrimary=True; Uuid=00001801-0000-1000-8000-00805f9b34fb; HasCharacteristics=True; HasInlucededServices=False // GENERIC_ATTRIBUTE
      Characteristics (1):
        - Characteristic Uuid=00002a05-0000-1000-8000-00805f9b34fb; Value=[null]; HasDescriptors=True // GENERIC_ATTRIBUTE/SERVICE_CHANGED
          Properties: Indicate
          Descriptors (1):
            - Descriptor Uuid=00002902-0000-1000-8000-00805f9b34fb; Value=0x0000 (Length=2B; Str="")
    
    - Service IsPrimary=True; Uuid=99fa0001-338a-1024-8a49-009c0215f78a; HasCharacteristics=True; HasInlucededServices=False // CONTROL
      Characteristics (2):
        - Characteristic Uuid=99fa0002-338a-1024-8a49-009c0215f78a; Value=[null]; HasDescriptors=False // CONTROL/CONTROL
          Properties: WriteWithoutResponse, Write
        - Characteristic Uuid=99fa0003-338a-1024-8a49-009c0215f78a; Value=[failed to read]; HasDescriptors=True // CONTROL/ERROR
          Properties: Read, Notify
          Descriptors (1):
            - Descriptor Uuid=00002902-0000-1000-8000-00805f9b34fb; Value=0x0100 (Length=2B; Str="☺")
    
    - Service IsPrimary=True; Uuid=99fa0010-338a-1024-8a49-009c0215f78a; HasCharacteristics=True; HasInlucededServices=False // DPG
      Characteristics (1):
        - Characteristic Uuid=99fa0011-338a-1024-8a49-009c0215f78a; Value=0x01028c00 (Length=4B; Str="☺☻?"); HasDescriptors=True // DPG/DPG
          Properties: Read, WriteWithoutResponse, Write, Notify
          Descriptors (1):
            - Descriptor Uuid=00002902-0000-1000-8000-00805f9b34fb; Value=0x0100 (Length=2B; Str="☺")
    
    - Service IsPrimary=True; Uuid=99fa0020-338a-1024-8a49-009c0215f78a; HasCharacteristics=True; HasInlucededServices=False // REFERENCE_OUTPUT
      Characteristics (3):
        - Characteristic Uuid=99fa0021-338a-1024-8a49-009c0215f78a; Value=0x52120000 (Length=4B; Str="R↕"); HasDescriptors=True // REFERENCE_OUTPUT/HEIGHT_SPEED
          Properties: Read, Notify
          Descriptors (1):
            - Descriptor Uuid=00002902-0000-1000-8000-00805f9b34fb; Value=0x0100 (Length=2B; Str="☺")
        - Characteristic Uuid=99fa0029-338a-1024-8a49-009c0215f78a; Value=0x01 (Length=1B; Str="☺"); HasDescriptors=False // REFERENCE_OUTPUT/MASK
          Properties: Read
        - Characteristic Uuid=99fa002a-338a-1024-8a49-009c0215f78a; Value=0x01 (Length=1B; Str="☺"); HasDescriptors=False // REFERENCE_OUTPUT/??? (unused)
          Properties: Read
    
    - Service IsPrimary=True; Uuid=99fa0030-338a-1024-8a49-009c0215f78a; HasCharacteristics=True; HasInlucededServices=False // REFERENCE_INPUT
      Characteristics (1):
        - Characteristic Uuid=99fa0031-338a-1024-8a49-009c0215f78a; Value=[null]; HasDescriptors=False // REFERENCE_INPUT/CTRL1 (command "move to")
          Properties: WriteWithoutResponse, Write
```

## Characteristic REFERENCE_OUTPUT/HEIGHT_SPEED

Gives real-time information about desk position and speed. Can be read and will be notified on every change.

Little endian, 4B. Read and/or notify.

* 2B - unsigned 16bit int; desk position relative to zero position; in centimeters
* 2B - signed 16bit int; speed (negative = down, positive = up, 0 = no movement); unit unknown

Remark: This is read/notification only. Height cannot be set with this.

## Sending DPG Commands

DPG commands are commands sent to main control unit. Requests are sent to `DPG` characteristic of `DPG` service. Same characteristic is used to receive response as notification.

You should wait for response before sending another command.

Request format of simple commands (without content):

```
00     0x7F // fixed header
01     command ID
02     0x00
```

Request format of write commands (with content):

```
00     0x7F // fixed header
01     command ID
02     0x80
03..n  data
```

Response format of confirmation (without content):

```
00     0x01 // fixed header; if not set, message is invalid (can indicate some error)
01     0x00
```

Response format (with content):

```
00     0x01 // fixed header; if not set, message is invalid (can indicate some error)
01     (0x01..0xFF) // anything else than 0x00
02..n  data
```

Commands found (not all implemented/tested) [are here](./src/LinakDeskClient/DpgCommands/CommandTypes.cs).

## Moving the Desk

To start/stop movement send message to `CONTROL` characteristic on `CONTROL` service.

Commands:

```
# Move UP
00     0x47
01     0x00

# Move DOWN
00     0x46
01     0x00

# Stop movement
00     0xFF
01     0x00
```

You need to repeat move command until desired height is reached. Each move command will start or continue movement for about 0.6-1.0s. It is sufficient to send move command each 500ms interval for continual movement. 

You will need to check `HEIGHT_SPEED` and `ERROR` characteristics to react to current height and errors.

### Collision behavior

Movement will stop on desk collision. It will cause `ERROR` event. It will also ignore further move commands sent in quick succession. You need to wait at least full duration of one movement (2 seconds should be enough) before sending new move command.

## Global Events

You should consume events:

* `ERROR` characteristic of `CONTROL` service - this will propagate any issues; example: desk collistion during movement
* `HEIGHT_SPEED` characteristic of `REFERENCE_OUTPUT` service - to watch any changes of height and speed of the table

## Glossary

* DPG = Linak desk control panel