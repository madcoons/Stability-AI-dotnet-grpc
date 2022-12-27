# Grpc classes for Stability AI api

[![NuGet latest version](https://badgen.net/nuget/v/StabilityAI.Grpc/latest)](https://nuget.org/packages/StabilityAI.Grpc)

It is based on [this official repository](https://github.com/Stability-AI/api-interfaces).

# Example
```csharp
using Gooseai;
using Grpc.Net.Client;

namespace StabilityAI.Grpc;

public class Example
{
    public static void Test()
    {
        using var channel = GrpcChannel.ForAddress("https://localhost:7042");
        var client = new GenerationService.GenerationServiceClient(channel);
        .
        .
        .
    }
}
```
