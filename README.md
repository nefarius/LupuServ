# LupuServ

E-Mail to SMS Gateway service for Lupusec XT1 alarm system

## Motivation

I happened to know an owner of the very early central alarm system Lupusec XT1 from the German company [Lupus Electronics](https://www.lupus-electronics.de/en/), which has been declared End Of Life several years ago and doesn't receive firmware updates anymore. It appears, that the manufacturer isn't particularly proud of this early revision, as it has become impossible to find resources about it on the company websites (except for the **XT1 Plus** which is *not* the system I'm talking about, but a newer platform).

The system offers a few methods of notifying the outside world about an alarm event, the one in question being able to send a classic SMS message to a maximum of two phone numbers. APIs tend to change or become deprecated, and without firmware updates there isn't much one can do. Which is exactly what happened here; the SMS gateway implementation broke and thanks to this thing not provide any logs or sources, who knows why. This leaves us with a few different channels to grab events from: Contact-ID - which I have no idea what that is and what to do with it - and E-Mail.

I decided to abuse the E-Mail functionality to receive alarm (and status change) events by faking an SMTP-Server, which in turn transforms the mail body and sends it to an SMS gateway in proxy. The application uses .NET 5.0 and is designed as a Worker compatible with Docker, so it should run on any supported Linux system.

## Supported Systems

The solution has been developed for and tested with the Lupusec XT1 (**not** Plus!) central station running Firmware version **1.0.89** and LupuServ hosted on a Raspberry Pi 2 Model B Rev 1.1 (ARM32). The only SMS gateway provider implemented is [CM.com](https://www.cm.com/about-cm/) so you need a registered account and a minimum balance required for the Messaging gateway channel.

## Limitations

Some shortcuts have been taken on purpose while developing this proxy application. As of now I have no plans to rectify them or focus on additional features.

- SMTP-Server doesn't use authentication, it accepts any username and password
  - This solution is meant to be operated in a firewall-protected private network, and without forced TLS protection plaintext authentication is a useless "security" measure anyway. It can be added quite easily though, consult the documentation of the SMP library.
- SMS gateway failures are logged, but no other notification happens
  - SMS delivery errors can occur, like if the credit of the used account is depleted, this should be delivered to technical personnel via some way.
- No rate limits or other protection to mitigate SMS-bombing
  - Again, this service should not be hosted outside of a protected network, so it shouldn't be an issue. Bear in mind though, that there is no rate limit so if multiple alarm mails arrive in a short amount of time, it could lead to some unintended message spam.

## How to set up

- Register an account with [CM.com](https://www.cm.com/)
  - Don't forget to respond to verification SMS and mail
  - Add a balance of at least 15€ (as of time of writing) to unlock the Messaging gateway channel (which allows sending messages)
  - Get the Product token/API key for the Messaging gateway
- Build and deploy this solution to a system of your choice
  - For example, install Docker CE on a Raspberry Pi 2 and use the provided compose file to permanently run it as a container. There's plenty documentation out there on how to do that so I will not go into details here.
    1) Build: `docker built -t lupuserv:latest .`
    2) Run: `docker run --name lupuserv -p 2025:2025 -v "${PWD}/appsettings.json:/app/appsettings.json" -d --restart unless-stopped lupuserv:latest`
- Rename `appsettings.example.json` to `appsettings.json`, adjust content according to your environment and restart the service
- Configure the E-Mail settings on the XT1 web interface as shown below (I only had access to a German UI so it might look different on your system):
  ![Settings](./assets/ygJiBqVo8R.png)
- You can use the Test E-Mail function and should be able to see it appear in the logs

## Sources & 3rd party credits

- [CM Text SDK](https://github.com/cmdotcom/text-sdk-dotnet)
- [MimeKit](https://github.com/jstedfast/MimeKit)
- [SmtpServer](https://github.com/cosullivan/SmtpServer)
