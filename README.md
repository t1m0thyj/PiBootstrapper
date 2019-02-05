# PiBootstrapper
Windows app to configure Raspbian SD card image before first boot

[Download](https://github.com/t1m0thyj/PiBootstrapper/releases/latest)

![Screenshot](/screenshot.png)

Setting up Wi-Fi on a Raspberry Pi with a display connected is usually easy to do in Raspbian. However, if you want to setup a headless Pi or connect to a university's enterprise network, this requires editing configuration files manually and is less straightforward. This app lets you set up Wi-Fi right after flashing an SD card image for your Pi while the SD card is still in your PC.

PiBootstrapper can automatically generate the Wi-Fi configuration for both personal and enterprise networks and also enable SSH (Secure Shell) login. After bootstrapping the Pi with this app, it should connect to Wi-Fi right away when it boots and be accessible remotely over the network without any further setup necessary.

This app only runs on Windows; follow the instructions below to manually setup a headless Pi on Linux or macOS. If you want to set up more than just Wi-Fi and SSH on the SD card image, try the cross-platform app [PiBakery](https://www.pibakery.org/) which allows all sorts of customizations.


## Manual Instructions

The following instructions explain how to manually configure Wi-Fi and enable SSH on a Raspbian SD card the same way that PiBootstrapper does.

**Warning:** If you have already booted your Pi and connected to Wi-Fi networks, creating a `wpa_supplicant.conf` file on the boot partition will overwrite any existing network configuration the next time it boots. In this case, it is recommended to edit the file `/etc/wpa_supplicant/wpa_supplicant.conf` directly on your Pi (requires *sudo* privileges). You can skip creating a new file at the beginning of the next section and only add the `network={...}` portion to the existing one.

### Configure Wi-Fi

Create a file called `wpa_supplicant.conf` on the SD card boot partition starting with the following lines:
```
ctrl_interface=DIR=/var/run/wpa_supplicant GROUP=netdev"
update_config=1
country={COUNTRY_CODE}

```
(Replace `{COUNTRY_CODE}` with your 2 character long ISO country code. A list of them can be found [here](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2#Officially_assigned_code_elements), some common ones are "US" for United States and "GB" for United Kingdom.)

After adding the lines shown above, configure your personal or enterprise wireless network as shown in the corresponding section below.

#### WPA/WPA2 Personal

Add the following lines to `wpa_supplicant.conf`:
```
network={
	ssid="{NETWORK_NAME}"
	scan_ssid=1
	key_mgmt=WPA-PSK
	psk="{PASSWORD}"
}
```
(Replace `{NETWORK_NAME}` and `{PASSWORD}` with the actual values.)

If you don't want the network password stored in plain text, run the command `wpa_passphrase {NETWORK_NAME} {PASSWORD}` and use the output as the password (without quote marks around it).

#### WPA/WPA2 Enterprise

Add the following lines to `wpa_supplicant.conf`:
```
network={
	ssid="{NETWORK_NAME}"
	scan_ssid=1
	key_mgmt=WPA-EAP
	eap=PEAP
	identity="{USERNAME}"
	password="{PASSWORD}"
	phase1="peaplabel=0"
	phase2="auth=MSCHAPV2"
}
```
(Replace `{NETWORK_NAME}`, `{USERNAME}`, and `{PASSWORD}` with the actual values.)

If you don't want the network password stored in plain text, run the command `echo -n {PASSWORD} | iconv -t utf16le | openssl md4` and use the output as the password with "hash:" added in front of it (and without quote marks around it). This will not work if the password is more than 14 characters long.

### Enable SSH

Create a blank file with no extension called `ssh` on the SD card boot partition.
