# PiBootstrapper
Windows app to configure Raspbian SD card image before first boot

[Download](https://github.com/t1m0thyj/PiBootstrapper/releases/latest)

![Screenshot](/screenshot.png)

Setting up Wi-Fi on a Raspberry Pi with a display directly connected is usually easy to do in Raspbian. However, if you want to setup a headless Pi or connect to a university's enterprise network, this requires editing configuration files manually and is less straightforward. This app lets you set up Wi-Fi right after flashing an SD card image for your Pi while the SD card is still in your Windows PC. It can automatically generate the Wi-Fi configuration for both personal and enterprise networks and also enable SSH (Secure Shell) login. After bootstrapping the Pi with this app, it should connect to Wi-Fi right away when it boots and be accessible remotely over the network without any further setup necessary.
