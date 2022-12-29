using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using NeuralSharp;
using NeuralSharp.Activation;
using NeuralSharp.Generators;
using NeuralSharp.Genetic;
using Snake;
using Spectre.Console.Cli;

namespace Trainer.Commands;

internal sealed class
    Test3Command : Command<Test3Command.Settings>
{
    private readonly IWeightGenerator _weightGenerator;
    private readonly IBiasGenerator _biasGenerator;
    private readonly IActivationFunction _activationFunction;
    private readonly NetworkTrainer _evolution;

    public Test3Command(IWeightGenerator w, IBiasGenerator b, IActivationFunction activationFunction, NetworkTrainer evolution)
    {
        this._weightGenerator = w;
        this._biasGenerator = b;
        _activationFunction = activationFunction;
        _evolution = evolution;
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        Console.WriteLine("#####################################");
        Console.WriteLine($"Running test 3 with {settings.Values} values for {settings.Iterations} iterations.");
        Console.WriteLine("#####################################");

        Run().GetAwaiter().GetResult();

        return 0;
    }

    public async Task Run()
    {
        var layers = new int[] { 24, 32, 24, 4 };
        var fileName = $"network_{string.Join("_", layers)}.json";
        var fileNetwork = new FileNetworkIo(fileName);
        var networkConfig = await fileNetwork.Load();
        NeuralNetwork network;
        if (networkConfig != null)
        {
            network = new NeuralNetwork(networkConfig);
        }
        else
        {
            network = new NeuralNetwork(_weightGenerator, _biasGenerator, layers);
        }

        var evolutionConfig = new NetworkTrainerConfig(10000, 100, 20);
        await _evolution.Run(network, evolutionConfig, Run, fileNetwork);
    }

    public float Run(NeuralNetwork network)
    {
        var game = new SnakeScene(_activationFunction, network);
        var i = 0;
        while (game.SnakeGame.Running)
        {
            game.Loop(i++);
        }

        return game.SnakeGame.Fitness;
    }

    public sealed class Settings : CommandSettings
    {
        [Description("Values.")]
        [CommandArgument(0, "[values]")]
        public int Values { get; init; }

        [Description("Iteration.")]
        [CommandArgument(0, "[iterations]")]
        public int Iterations { get; init; }
    }
}

