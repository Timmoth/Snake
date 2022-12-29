using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Aptacode.AppFramework;
using Microsoft.AspNetCore.Components;
using NeuralSharp;
using NeuralSharp.Activation;
using NeuralSharp.Generators;
using NeuralSharp.Serialization;
using Snake;
using Snake.Behaviours;

namespace Demo.Pages;

public class IndexBase : ComponentBase
{
    [Inject] public HttpClient Client { get; set; }
    [Inject] public IActivationFunction ActivationFunction { get; set; }
    [Inject] public IBiasGenerator BiasGenerator { get; set; }
    [Inject] public IWeightGenerator WeightGenerator { get; set; }

    public SnakeScene Scene { get; set; }
    public SnakeBehaviour SnakeGame { get; set; }
    public bool Loading { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        var networkConfig = await Client.GetFromJsonAsync<NetworkConfig>("network.json");

        var network = new NeuralNetwork(networkConfig);

        Scene = new SnakeScene(ActivationFunction, network);

        Loading = false;
        await InvokeAsync(StateHasChanged);
        await base.OnInitializedAsync();
    }
}