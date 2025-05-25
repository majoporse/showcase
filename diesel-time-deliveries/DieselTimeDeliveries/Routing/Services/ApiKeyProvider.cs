using Microsoft.Extensions.DependencyInjection;
using RegistR.Attributes.Base;

namespace Routing.Services;

public class ApiKeyProvider : IApiKeyProvider
{
    public string Key { get; private set; }
    
    public ApiKeyProvider(string key)
    {
        Key = key;
    }
}