using GenHTTP.Api.Content;

using GenHTTP.Modules.Layouting;
using GenHTTP.Modules.ReverseProxy;
using GenHTTP.Modules.ServerCaching;
using GenHTTP.Modules.ServerCaching.Provider;

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

        // todo: add compression

        var cache = CreateCache().Invalidate(false);

        var proxy = Proxy.Create()
                         .Upstream(target)
                         .Add(cache);

        var app = Layout.Create()
                        .Add(proxy);

        return app;
    }

    private static ServerCacheHandlerBuilder CreateCache()
    {
        var cacheMode = Environment.GetEnvironmentVariable("CACHE_MODE") ?? "persistent";

        return (string.Compare(cacheMode, "persistent", true) == 0) ? ServerCache.Persistent("./cache/") : ServerCache.Memory();
    }

}
