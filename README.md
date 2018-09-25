# PiBootstrapper
Windows app to configure Raspbian SD card image before first boot

[Download](https://github.com/t1m0thyj/PiBootstrapper/releases/latest)

![Screenshot](/screenshot.png)

Setting up Wi-Fi on a Raspberry Pi with a display directly connected is usually easy to do in Raspbian. However, if you want to setup a headless Pi or connect to a university's enterprise network, this requires editing configuration files manually and is less straightforward. This app lets you set up Wi-Fi right after flashing an SD card image for your Pi while the SD card is still in your PC.

PiBootstrapper can automatically generate the Wi-Fi configuration for both personal and enterprise networks and also enable SSH (Secure Shell) login. After bootstrapping the Pi with this app, it should connect to Wi-Fi right away when it boots and be accessible remotely over the network without any further setup necessary.

This app is written in C# and only runs on Windows, but you can manually configure a headless Pi setup on Linux or Mac. See [here](https://raspberrypi.stackexchange.com/a/57023/42551) for instructions on how to create `wpa_supplicant.conf` and `ssh` files on the SD card boot partition (also [here](https://raspberrypi.stackexchange.com/a/24670/42551) if you need to configure `wpa_supplicant.conf` for an enterprise network). If you want to set up more than just Wi-Fi and SSH on the SD card image, try the cross-platform app [PiBakery](http://www.pibakery.org/) which allows all sorts of customizations.

Wi-Fi passwords are hashed by the app before storing them on the Pi rather than being written as plain text, allowing you to safely edit `wpa_supplicant.conf` in public without revealing your password. If you want to manually hash the passwords, you can use the [`wpa_passphrase`](https://linux.die.net/man/8/wpa_passphrase) command for WPA Personal networks, or see [here](https://unix.stackexchange.com/a/278948/190213) for WPA Enterprise networks.
