using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ChannelsExample
{
    public class Consumer<T>
    {
        private readonly ChannelReader<T> _reader;

        public Consumer(ChannelReader<T> reader)
        {
            _reader = reader;
        }
        public async Task ConsumeDataAsync(CancellationToken cancellationToken = default)
        {

            while (await _reader.WaitToReadAsync(cancellationToken))
            {
                if (_reader.TryRead(out var message))
                {
                   Logger.ToConsole($"consumer received msg:{message}",ConsoleColor.DarkGreen);
                }
            }
        }
    }
}