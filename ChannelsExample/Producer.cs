using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ChannelsExample
{
    public class Producer<T>
    {
        private readonly ChannelWriter<T> _writer;

        public Producer(ChannelWriter<T> writer)
        {
            _writer = writer;
        }
        public async Task PublishAsync(T t, CancellationToken cancellationToken = default)
        {
            await _writer.WriteAsync(t, cancellationToken);
            Logger.ToConsole($"published a message:{t}",ConsoleColor.DarkGray);
        }
    }
}