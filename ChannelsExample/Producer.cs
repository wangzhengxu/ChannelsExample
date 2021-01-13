using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ChannelsExample
{
    public class Producer
    {
        private readonly ChannelWriter<string> _writer;

        public Producer(ChannelWriter<string> writer)
        {
            _writer = writer;
        }
        public async Task PublishAsync(string msg, CancellationToken cancellationToken = default)
        {
            await _writer.WriteAsync(msg, cancellationToken);
            Logger.ToConsole($"published a message:{msg}",ConsoleColor.DarkGray);
        }
        public async Task TryPublishAsync(string msg, CancellationToken cancellationToken = default)
        {
            while (await _writer.WaitToWriteAsync(cancellationToken))
            {
                if (_writer.TryWrite(msg))
                {
                    Logger.ToConsole($"published a message:{msg}", ConsoleColor.DarkGray);
                    return;
                }

                Logger.ToConsole("Unsuccessful writing, wait for the next opportunity", ConsoleColor.DarkGray);
            }
        }
    }
}