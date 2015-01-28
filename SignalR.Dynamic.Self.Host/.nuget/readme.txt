SignalR.Dynamic.Self.Host has been added to your project. The following example illustrates how to get a self hosted server up and running.
The Admin URL below is the one to use for setting up configuation values for publisher that need configuration values(most will!)

The Host URL is the one that will be used by client applications to consume real time data over SignalR

You can navigate to <HostRoot>/signalr/hubs to view the SignalR hubs for each of the registered modules.
NOTE: Using the AdminURL will need a URL reservation when self hosting since this uses Nancy:
THe API will attempt to do this for you on startup.

You can do the same yourself by running the following:
netsh http add urlacl url=http://+:<yourport>/nancy/ user=Everyone

The reservation can be viewed using:
netsh http show urlacl url=http://+:<yourport>/nancy/

The reservation can be deleted using:
netsh http delete urlacl url=http://+:<yourport>/nancy/

A SignalRDynamicHostBootStrapper.cs has been added to the project that illustrates how to start the server.
You can start an instance by adding the following to your Program:

class Program
{
	static void Main(string[] args)
	{
		SignalRDynamicHostBootStrapper bootStrapper = new SignalRDynamicHostBootStrapper();
		bootStrapper.Start();
	}
}
