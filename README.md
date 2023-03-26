# LiquidCTL Plugin for MSI Afterburner

This is a monitoring plugin for [MSI Afterburner](https://www.msi.com/Landing/afterburner) that exposes generic AIO data provided by [LiquidCtl](https://github.com/LiquidCtl/LiquidCtl).
Codebase is heavily inspired by https://github.com/ts-korhonen/LibreHardwareMonitorAfterburnerPlugin and https://github.com/SuspiciousActivity/FanControl.Liquidctl, given my lack of knowledge in the C# / Microsoft ecosystem, huge thanks to them.

## Requirements

* MSI Afterburner (version 4.6.4 used in development)
* .Net Framefork >= 4.8

## Installing

Download latest release of `LiquidCtl.dll` [here](https://github.com/Koli0842/LiquidCtlAfterburnerPlugin/releases) and place in into `Plugins/Monitoring` of MSI Afterburner installation folder.

Download the latest release of [SuspiciousActivity's liquidctl fork](https://github.com/SuspiciousActivity/liquidctl) and place it next to the dll from the last step.

Additionally you need to put `Newtonsoft.Json.dll` into the root folder of MSI Afterburner.

E.g. `C:\Program Files (x86)\MSI Afterburner\Plugins\Monitoring`

## Setup

Start MSI Afterburner and go to `Settings > Monitoring` and click `[...]` button next to `Active hardware monitoring graphs`.

In the list of `Active plugin modules` select and activate the checkmark next to `LiquidCtl.dll`.

Setup button is disabled, current implementation exposes all hardware reported by LiquidCtl.

Afterburner should now be populated with discovered sensors.

## Uninstalling

Exit MSI Afterburner and delete `LiquidCtl.dll` you installed earlier.

In the same folder delete `LiquidCtl.dll.log` if they exist. You can clean up `Newtonsoft.Json.dll` aswell from the root folder aswell.

## License

The plugin source code is licensed under [Mozilla Public License 2.0](https://mozilla.org/MPL/2.0/)
