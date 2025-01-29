using GenHTTP.Api.Content;

using GenHTTP.Modules.ClientCaching;
using GenHTTP.Modules.Compression;
using GenHTTP.Modules.IO;
using GenHTTP.Modules.Layouting;
using GenHTTP.Modules.ServerCaching;
using GenHTTP.Modules.ServerCaching.Provider;

using LocalMirror.Handler;

namespace LocalMirror;

public static class Project
{

    public static IHandlerBuilder Setup()
    {
        var target = Environment.GetEnvironmentVariable("TARGET");

        if (string.IsNullOrWhiteSpace(target) || !Uri.IsWellFormedUriString(target, UriKind.Absolute))
        {
            throw new ArgumentException("Target URL is not specified or not a well formed URI.");
        }

        var cache = CreateCache().Invalidate(false);

        var compression = CompressedContent.Default()
                                           .Level(System.IO.Compression.CompressionLevel.Optimal);

        var rangeSupport = RangeSupport.Create();

        var clientCache = ClientCache.Validation();

        var app = Layout.Create()
                        .Add(new DownloadHandler(target))
                        .Add(compression)
                        .Add(cache)
                        .Add(rangeSupport)
                        .Add(clientCache);

        return app;
    }

    private static ServerCacheHandlerBuilder CreateCache()
    {
        var cacheMode = Environment.GetEnvironmentVariable("CACHE_MODE") ?? "persistent";

        return (string.Compare(cacheMode, "persistent", true) == 0) ? ServerCache.Persistent("./cache/") : ServerCache.Memory();
    }

}
