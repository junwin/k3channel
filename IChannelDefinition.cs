namespace K3Channel
{
    public interface IChannelDefinition
    {
        string ChannelName { get; set; }

        string HostName { get; set; }

        int Port { get; set; }

        string ExchangeName { get; set; }

        string Name { get; set; }

        string TypeName { get; set; }
    }
}