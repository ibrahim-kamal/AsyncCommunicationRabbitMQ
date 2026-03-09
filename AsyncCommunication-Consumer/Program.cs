using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


async Task Example3() {
    var factory = new ConnectionFactory(){ HostName = "localhost" };
    using var connection = await factory.CreateConnectionAsync();
    using var channel = await connection.CreateChannelAsync();

    var consumer = new AsyncEventingBasicConsumer(channel);

    consumer.ReceivedAsync += async (model, ea) => {
        var body = ea.Body.ToArray();
        
        var message = Encoding.UTF8.GetString(body);

        //await channel.BasicRejectAsync(ea.DeliveryTag, false);

        Console.WriteLine(message);
    };

    await channel.BasicConsumeAsync("dead-Queue", false, consumer);

    Console.ReadLine();



}

await Example3();