using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ChannelsExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var channel = Channel.CreateBounded<string>(10);
            var producer1=new Producer<string>(channel.Writer);
            var consumer1=new Consumer<string>(channel.Reader);
            var cancelSource = new CancellationTokenSource();
            var cancelToken = cancelSource.Token;
            var consumerTask = consumer1.ConsumeDataAsync(cancelToken);
            var producerTask = producer1.PublishAsync("hello", cancelToken);
            await producerTask.ContinueWith(_ => channel.Writer.Complete(), cancelToken);
            await consumerTask;
            Console.WriteLine("Done!");
        }
    }
}
