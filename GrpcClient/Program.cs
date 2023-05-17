using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcServer;

namespace GrpcClient
{
    internal class Program
    {
        private static async Task Main()
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5000");

            var client = new Greeter.GreeterClient(channel);

            // try
            // {
            //     var reply = await client.SayHelloAsync(new HelloRequest
            //     {
            //         Name = "OTUS",
            //         Age = 25,
            //     });

            //     Console.WriteLine("Greeting: " + reply.Message);
            //     Console.WriteLine(" ");
            // }
            // catch (Exception ex)
            // {
            //     Console.WriteLine(ex.Message);
            //     Console.WriteLine(ex.InnerException);
            // }

            //return;

            var invoker = client.BidiHello();

            var readTask = Task.Run(async () =>
                        {
                            await foreach (var response in invoker.ResponseStream.ReadAllAsync())
                            {
                                Console.WriteLine("Greeting: " + response.Message);
                            }
                        });
            var t = Task.Run(async () =>
                        {
                            // try
                            // {
                                int counter = 0;
                                while (true)
                                {
                                    Console.WriteLine(counter);
                                    await invoker.RequestStream.WriteAsync(
                          new HelloRequest { Name = $"Im [{counter++}]" }
                        );

                                    //var c = Console.ReadKey();
                                    // if (c.Key == ConsoleKey.Enter)
                                    //     break;
                                    await Task.Delay(400);

                                    if(counter>10)
                                    break;
                                }
                            // }
                            // catch (Exception ex)
                            // {
                            //     Console.WriteLine(ex);
                            // }
                            //await invoker.RequestStream.CompleteAsync();
                        });

            Console.ReadKey();
        }
    }
}