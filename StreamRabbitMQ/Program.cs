using RabbitMQ.Stream.Client;
using RabbitMQ.Stream.Client.Reliable;
using System.Buffers;
using System.Net;
using System.Text;

async Task StreamTest() {

    var config = new StreamSystemConfig
    {
        UserName = "dev-user",
        Password = "dev123",
        VirtualHost = "/",
        Endpoints = new List<EndPoint> { new IPEndPoint(IPAddress.Loopback, 5552) }
    };

      var system = await StreamSystem.Create(config);
      const string stream = "stream-qu";
      string reference = "DotNet 6";

    var consumerConfig = new ConsumerConfig(system, stream)
    {
        Reference = reference,
        MessageHandler = async (sourceStream, consumer, messageContext, message) => {
            Console.WriteLine($" message comming from {sourceStream} data: {Encoding.Default.GetString(message.Data.Contents.ToArray())} - consumed");
            Console.WriteLine();
            await consumer.StoreOffset(messageContext.Offset);
            await Task.Delay(1000);
        }

    };

    try
    {
        var offset = await system.QueryOffset(reference, stream);
        consumerConfig.OffsetSpec = new OffsetTypeOffset(offset + 1);
    }
    catch {
        Console.WriteLine($"the consumer {consumerConfig.Reference} diidn't store offset {stream}");
    }

    var consumer = await Consumer.Create(consumerConfig);

    Console.WriteLine($" Press To stop");
    Console.WriteLine($" =============================================");
    Console.ReadLine();



    await consumer.Close();
    await system.Close();


}

await StreamTest();