using RabbitMQ.Client;
using System.Text;

async Task example3() {
    var factory = new ConnectionFactory() { HostName = "localhost" };
    using var connection = await factory.CreateConnectionAsync();
    using var channel = await connection.CreateChannelAsync(
        new CreateChannelOptions(publisherConfirmationsEnabled: true, publisherConfirmationTrackingEnabled: false)
    );
    // publisherConfirmationsEnabled To acknowledgments  
    // publisherConfirmationTrackingEnabled if true that will give exception if there any exception,
    // you can use BasicReturnAsync event to handle exception [Exception happened from exchange but if exchange not found that will not return exception]

    var message = "Hello Developer!";
    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicReturnAsync += async (sender, args) => 
    {
        Console.WriteLine("{0} Reply Text {1}" , args.RoutingKey , args.ReplyText);
        Console.WriteLine("-------------------------------------------------------");
        await Task.CompletedTask;
    };

    channel.ChannelShutdownAsync += async (sender, args) => 
    {
        Console.WriteLine("Reply Text {0}" ,  args.ReplyText);
        Console.WriteLine("-------------------------------------------------------");
        await Task.CompletedTask;
    };
    channel.BasicAcksAsync += async (sender, args) => // publisherConfirmationsEnabled
    {
        Console.WriteLine("Delivery Tag ");
        Console.WriteLine("-------------------------------------------------------");
        await Task.CompletedTask;
    };



    await channel.BasicPublishAsync(exchange: "amq.fanout", routingKey: "Key111",  body: body, mandatory: true);


    Console.ReadLine();

}

await example3();