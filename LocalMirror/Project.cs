using GenHTTP.Api.Content;

using GenHTTP.Modules.ClientCaching;
using GenHTTP.Modules.Compression;
using GenHTTP.Modules.IO;
using GenHTTP.Modules.Layouting;
using GenHTTP.Modules.ServerCaching;

using LocalMirror.Handler;

namespace LocalMirror;

public static class Project
{

    public static IHandlerBuilder Setup()
    {
        var target = "https://cdn.jsdelivr.net/npm/";

        var cache = ServerCache.Persistent("./cache/").Invalidate(false);

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

}
