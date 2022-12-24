using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NeuralSharp.Activation;
using NeuralSharp.Generators;
using NeuralSharp.Genetic;
using Spectre.Console.Cli;
using Trainer.Commands;
using Trainer.Infrastructure;

// Create a type registrar and register any dependencies.
// A type registrar is an adapter for a DI framework.
var services = new ServiceCollection();
// Services
services.AddTransient<IWeightGenerator, WeightGenerator>();
services.AddTransient<IBiasGenerator, BiasGenerator>();
services.AddTransient<IActivationFunction, Tanh>();
services.AddTransient<INeuralNetworkIo>(_ => new FileNetworkIo("./network.json"));
services.AddTransient<INetworkMutator, NetworkMutator>();
services.AddTransient<IFloatMutator, FloatMutator>();
services.AddTransient<IMutationDecider>(_ => new MutationDecider(0.002f));
services.AddTransient<NetworkTrainer>();

services.AddLogging(b => b
    .AddFilter("Microsoft", LogLevel.Warning)
    .AddFilter("System", LogLevel.Warning)
    .AddConsole());

var registrar = new TypeRegistrar(services);

// Create a new command app with the registrar
// and run it with the provided arguments.
var app = new CommandApp(registrar);
app.Configure(config =>
{
    config.AddCommand<Test3Command>("train")
        .WithDescription("Train snake network");


#if DEBUG
    config.PropagateExceptions();
    config.ValidateExamples();
#endif
});

return app.Run(args);