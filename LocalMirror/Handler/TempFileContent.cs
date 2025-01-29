using GenHTTP.Api.Protocol;

namespace LocalMirror.Handler;

public class TempFileContent(string file) : IResponseContent, IDisposable
{
    private readonly FileInfo _Info = new(file);

    public ulong? Length => (ulong)_Info.Length;

    public ValueTask<ulong?> CalculateChecksumAsync() => ValueTask.FromResult((ulong?)null);

    public async ValueTask WriteAsync(Stream target, uint bufferSize)
    {
        using var stream = File.OpenRead(file);

        await stream.CopyToAsync(target, (int)bufferSize);
    }

    public void Dispose()
    {
        _Info.Delete();
    }

}
