using RabbitMQ.Stream.Client;
using RabbitMQ.Stream.Client.Reliable;
using System.Buffers;
using System.Net;
using System.Text;

async Task StreamTest()
{

    var config = new StreamSystemConfig
    {
        UserName = "dev-user",
        Password = "dev123",
        VirtualHost = "/",
        Endpoints = new List<EndPoint> { new IPEndPoint(IPAddress.Loopback, 5552) }
    };

    var system = await StreamSystem.Create(config);
    const string stream = "stream-qu";

    var producerConfig = new ProducerConfig(system, stream)
    {
        Reference = "DotNet 6",
        ConfirmationHandler = Confirmation => {
            if (Confirmation.Status == ConfirmationStatus.Confirmed)
            {
                Console.WriteLine($"Message confirmed");
            }
            else
            {
                Console.WriteLine($"Message failed: {Confirmation.Status}");
            }
            return Task.CompletedTask;
        },

    };
    var producer = await Producer.Create(producerConfig);

    var i = 0;
    while(true)
    {
        i++;
        var message = new Message(Encoding.UTF8.GetBytes($"Hello Stream {i}"));
        await producer.Send(message);
        Console.WriteLine($"Sent message: Hello Stream {i}");
        await Task.Delay(1000);
    }

    Console.WriteLine($" Press To stop");
    Console.WriteLine($" =============================================");
    Console.ReadLine();



    await producer.Close();
    await system.Close();


}

await StreamTest();