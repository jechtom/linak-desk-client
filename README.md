# Linak / IKEA IDÅSEN Desk Controller - Bluetooth C# Library

This is library and a CLI (command line interface) tool to control Linak desks with bluetooth support. This unit is used in IKEA IDÅSEN desks. It should work fine with other LINAK desks, but I have not tested it.

Remark: Only IKEA IDÅSEN desks got interface made by LINAK. Cheaper desks from IKEA use different manufacturer.

Written in .NET/C#.

Pull requests welcome.

## Reverse Engineering

See [protocol.md](./protocol.md).

## Howto Use It

Compile it. And run `LinakDeskCli.exe`. Parameters:

```
# show help
LinakDeskCli.exe

# scan bluetooth devices
LinakDeskCli.exe scan

# list already paired bluetooth devices
LinakDeskCli.exe list

# connect and show info about Linak desk (where FFFFFFFFFFFF is ID of the desk)
LinakDeskCli.exe info FFFFFFFFFFFF

# move desk to first height position
LinakDeskCli.exe move FFFFFFFFFFFF 1
```

Remark: Use app to do initialy pairing and to setup memory positions. Currently this is not supported by CLI.

## Setting Powershell Alias

You can create command aliases like `desk-up` or `desk-down` to perform move command.

```powershell
# Edit profile file
notepad $PROFILE

# Add any custom functions you need. For example:
Function desk-down() { & 'E:\programs\linak-desk\LinakDeskCli' move FFFFFFFFFFFF 1 }
Function desk-up() { & 'E:\programs\linak-desk\LinakDeskCli' move FFFFFFFFFFFF 2 }
```

See it in action:

[![](https://img.youtube.com/vi/OsilI1ORzdc/0.jpg)](https://www.youtube.com/watch?v=OsilI1ORzdc)

## Sample Outputs

Sample `info` command output:

```
> LinakDeskCli.exe info FFFFFFFFFFFF
Connecting...
Height and speed: Height: 14,08cm; Speed: 0
Desk "Desk 1234" (Id=<<REDACTED>>, DeskType=Desk)
Capabilities: MemorySize=3;Flags=[BluetoothAllowed]
Memory: [[1]=>{IsSet: True; Height: 14,06cm}, [2]=>{IsSet: True; Height: 48,58cm}, [3]=>{IsSet: True; Height: 51,40cm}]
Offset: 271,37cm
Position: Height: 14,08cm; Speed: 0
Connected.
Done.
```

Sample `move` command output:

```
> LinakDeskCli.exe move FFFFFFFFFFFF 2
Found. Connecting...
Height and speed: Height: 46,90cm; Speed: 0
Desk "Desk 1234" (Id=<<REDACTED>>, DeskType=Desk)
Capabilities: MemorySize=3;Flags=[BluetoothAllowed]
Memory: [[1]=>{IsSet: True; Height: 14,06cm}, [2]=>{IsSet: True; Height: 48,58cm}, [3]=>{IsSet: True; Height: 51,40cm}]
Offset: 271,37cm
Position: Height: 46,90cm; Speed: 0
Connected.
Moving to memory position 2.
Height and speed: Height: 46,92cm; Speed: 704
Height and speed: Height: 46,95cm; Speed: 1072
Height and speed: Height: 46,99cm; Speed: 1360
Height and speed: Height: 47,05cm; Speed: 1712
Height and speed: Height: 47,12cm; Speed: 2080
Height and speed: Height: 47,21cm; Speed: 2448
Height and speed: Height: 47,31cm; Speed: 2800
Height and speed: Height: 47,40cm; Speed: 3088
Height and speed: Height: 47,52cm; Speed: 3456
Height and speed: Height: 47,66cm; Speed: 3616
Height and speed: Height: 47,79cm; Speed: 3680
Height and speed: Height: 47,93cm; Speed: 3712
Height and speed: Height: 48,05cm; Speed: 3648
Done.
```

## Credits

Based on work from:

* [Regul777/linak-desk-cli](https://github.com/Regul777/linak-desk-cli)
* [anetczuk/linak_bt_desk](https://github.com/anetczuk/linak_bt_desk)
* [zewelor/linak_bt_desk](https://github.com/zewelor/linak_bt_desk)