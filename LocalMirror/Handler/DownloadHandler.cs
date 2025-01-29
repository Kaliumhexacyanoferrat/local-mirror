using GenHTTP.Api.Content;
using GenHTTP.Api.Protocol;
using System.Net;

namespace LocalMirror.Handler;

public class DownloadHandler : IHandler
{
    private readonly HttpClient _Client;

    private string Target { get; }

    public DownloadHandler(string target)
    {
        Target = target.EndsWith('/') ? target : target + '/';

        var handler = new SocketsHttpHandler
        {
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.None,
            ConnectTimeout = TimeSpan.FromSeconds(10)
        };

        _Client = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(60)
        };
    }

    public ValueTask PrepareAsync() => new();

    public async ValueTask<IResponse?> HandleAsync(IRequest request)
    {
        try
        {
            var file = request.Target.GetRemaining().ToString(true);

            var response = await _Client.GetAsync(Target + file);

            if (response == null)
            {
                return request.Respond().Status(ResponseStatus.NoContent).Build();
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ProviderException(ResponseStatus.InternalServerError, "Upstream did not return a file response.");
            }

            var tempFile = Path.GetTempFileName();

            using (var targetStream = File.OpenWrite(tempFile))
            {
                await response.Content.CopyToAsync(targetStream);
            }

            return request.Respond()
                          .Content(new TempFileContent(tempFile))
                          .Build();
        }
        catch (OperationCanceledException e)
        {
            throw new ProviderException(ResponseStatus.GatewayTimeout, "The gateway did not respond in time.", e);
        }
        catch (HttpRequestException e)
        {
            throw new ProviderException(ResponseStatus.BadGateway, "Unable to retrieve a response from the gateway.", e);
        }
    }

}
