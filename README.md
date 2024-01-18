QR Code generator app
=====================

## Usage

Use the `qr` console app to safely generate a QR code offline without relying on any external web services.

The root command encodes text as a QR code.

```
echo -n "foo" | qr --format svg > foo.svg
```

The wifi command produces a QR code that contains a WiFi login.

```
qr wifi $SSID --format png --output wifi.png
```

The app supports SVG and PNG output formats. Use -h or --help option to display a more detailed usage information.

## Building the app

Use .NET SDK 8 to build the app. The native build toolchain is needed to publish the app for the given platform in the native AOT mode. See <https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/> for more details.

1. Navigate to src/qr folder.
2. Execute `dotnet build` to build the app.
3. Execute `dotnet publish` to publish the app. The fitting native build toolchain is needed.

## Notes

I've created the app to be able to safely create a QR code containing WiFi credentials and to test the .NET AOT publish mode. Thus, the app is a command line application published as a single binary file that does not depend on external libraries.
