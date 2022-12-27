using Google.Protobuf;
using Gooseai;
using Grpc.Core;
using Grpc.Net.Client;

var credentials = CallCredentials.FromInterceptor((_, metadata) =>
{
    var apiKey = Environment.GetEnvironmentVariable("STABILITY_KEY");
    if (string.IsNullOrEmpty(apiKey))
    {
        throw new Exception("Please specify STABILITY_KEY environment variable.");
    }

    metadata.Add("Authorization", $"Bearer {apiKey}");
    return Task.CompletedTask;
});

using var channel = GrpcChannel.ForAddress("https://grpc.stability.ai:443", new GrpcChannelOptions
{
    Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
});

var client = new GenerationService.GenerationServiceClient(channel);

var generation = client.Generate(new Request
{
    EngineId = "stable-diffusion-512-v2-1",
    Prompt =
    {
        new Prompt
        {
            Text =
                "A dream of a distant galaxy, concept art, matte painting, HQ, 4k"
        }
    },
    Image = new ImageParameters
    {
        Samples = 4,
    },
});

await foreach (var res in generation.ResponseStream.ReadAllAsync())
{
    foreach (var artifact in res.Artifacts)
    {
        if (artifact.FinishReason == FinishReason.Filter)
        {
            Console.WriteLine("Image filtered.");
        }
        else if (artifact.Type == ArtifactType.ArtifactImage)
        {
            var image = artifact.Binary.ToByteArray();
            var path = $"{Guid.NewGuid()}.png";
            await File.WriteAllBytesAsync(path, image);

            Console.WriteLine($"Image saved to {path}.");
        }
    }
}