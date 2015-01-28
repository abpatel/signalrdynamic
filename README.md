# signalrdynamic
SignalRDynamic is based on ASP.NET SignalR.
The motivation behind this library is to simplify building connectors to external systems and
surface real time from these systems via SignalR.

The connectors are better known as <b>Publishers</b> which publish (broadcast) messages to a <i>Topic</i>

# Setup a Publisher
To create a Publisher, create a class library project, add references to
<i>SignalR.Dynamic.API.Common</i> and <i>SignalR.Dynamic.API.Interfaces</i> and implement the <code>IPublisher</code> interface like so:

Example:<br />

<code>

    [Metadata(Topic = "SampleTopic", Roles="Role1,Role2")]  //Set the topic and auth roles

    public class SamplePublisher : IPublisher
    {

        private Timer t = null;
        private IRelay relay = null;
        private IConfiguration configuration = null;

        public SamplePublisher(IRelay relay, IConfiguration configuration)
        {
            this.relay = relay;
            this.configuration = configuration;
        }

        public void Initialize()
        {
            //Perform Initialization to connect to external system here
            Debug.WriteLine("Initializing Publisher");

            //Obtain Configuration infomation if needed
            var config = configuration.GetConfiguration().Where(s => s.Topic == "SampleTopic").ToArray();
            Debug.WriteLine("Configuration");
            Array.ForEach(config, c => Debug.WriteLine(c));
        }
        public void Publish()
        {
            //simulate some events and broadcast
            t = new Timer(_ =>
            {
            relay.RelayMessage(new Message
            {
            Topic = "SampleTopic",
            Properties = new
            {
            SomeProperty1 = "Prop1",
            SomeProperty2 = "Prop2"    //Add any properties you want
            }
            });
            }, null, 0, 3000);
        }
        public void OnConfigurationChange(params SettingChangeInfo[] settingChangeInfos)
        {
            //Handle config changes
            var changedSettings = settingChangeInfos.Where(s => s.Setting.Topic == "SampleTopic");
            foreach (var setting in changedSettings)
            {
                Debug.WriteLine("{0} was {1}", setting.Setting.Key, setting.ChangeType);
            }

        }

    }
</code>

# Deploy Publisher to Host
Once the publisher is built, copy the DLL and PDB to the output directory of the host.

Currently there are 2 hosts:

- Console based self host build using Owin
- ASPNET MVC 5 based WebHost


The host reads the <i>Metadata</i> attribute and dynamically generates the SignalR hubs behind the scenes.
The hosts are setup currently to use Windows auth. and are CORS enabled.

# Nuget support
The Self host project is a set of libraries that have been wrapped up into a nupkg and can be
installed into  console application.

There will be nuget support for adding Publishers shortly and will be made available via nuget.
