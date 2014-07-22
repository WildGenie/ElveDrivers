ElveDrivers
===========

This repository contains C# .NET Device Drivers for the Elve Home Automation software package
from http://codecoretechnologies.com. At this time, Elve drivers must target the .NET 3.5 framework
so .NET 4 backports such as the Task Parallel Library have been used for some drivers. The
ILMerge NuGet package is used to allow easy deployment of a single combined DLL per driver to the server.

You will need the Elve SDK installed to compile the drivers.

The following drivers in this repository are stable:

	GeoFancy - The Geofancy iOS application provides GeoFencing notifications.

	RainforestEagleDriver - The Rainforest Eagle bridge provides updates on household power usage.

	Pushover - A client for the Pushover.net service to push notifications to phones and other devices.

The following drivers in this repository are still in active development:

	NestDriver - Uses the Nest API to control your thermostat (in development).

	PhilipsHueDriver - Uses the REST API for the Philips Hue Bridge to control lighting (in development).

	ZWave - A ZWave driver for the Z-Stick
