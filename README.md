DnsExitClient
=============

This is a client written in C# which will update your dynamic ip changes to dnsexit.com. The program works in windows with .NET or linux with mono. If setup as an automatic task please be sensible and do not execute too often (5 minutes at least). 

Configuration information is stored in the App.Config.

Linux
------
Mono compilation sample:
dmcs DnsExitClient.cs -r:System.Web,System.Configuration,System.Net

/etc/crontab
*/5 *   * * *   root    /usr/local/bin/dnsexitclient/DnsExitClient.exe > /dev/null

