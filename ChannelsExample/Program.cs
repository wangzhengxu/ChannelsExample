using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ChannelsExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const int maxMessagesToBuffer = 10;
            var channel = Channel.CreateBounded<string>(maxMessagesToBuffer);
            var cancelSource = new CancellationTokenSource();
            var cancelToken = cancelSource.Token;
            var tasks = new List<Task>(DispatchConsumers(channel, 1, cancelToken))
            {
                ProduceAsync(channel, 5, 1, cancelSource)
            };
            await Task.WhenAll(tasks);
            cancelSource.Cancel();
            Logger.ToConsole("Done!");
        }

        private static Task[] DispatchConsumers(Channel<string,string> channel, int consumerCount,
            CancellationToken cancellationToken)
        {
            return Enumerable.Range(1, consumerCount)
                .Select(i => new Consumer(channel.Reader).ConsumeDataAsync(cancellationToken)).ToArray();
        }

        private static async Task ProduceAsync(Channel<string,string> channel, int needToSendCount, int producerCount,
            CancellationTokenSource tokenSource)
        {
            var producers = Enumerable.Range(1, producerCount)
                .Select(i => new Producer(channel.Writer))
                .ToArray();
            var index = 0;
            var tasks = Enumerable.Range(1, needToSendCount)
                .Select(i =>
                {
                    index = ++index % producerCount;
                    var producer = producers[index];
                    return producer.TryPublishAsync($"message {i}", tokenSource.Token);
                })
                .ToArray();
            await Task.WhenAll(tasks);
            Logger.ToConsole("done publishing, closing writer",ConsoleColor.DarkGray);
            channel.Writer.Complete();

        }

    }
}
