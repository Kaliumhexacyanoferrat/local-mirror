# Local Mirror

Docker service to create a network-local mirror of a remote server providing files via HTTP.

## Features

- Caches responses of an upstream server on local disc or in memory
- Serves more than 100k of requests per second, depending on the size
- Simple setup, just pass the upstream URL
- Automatically applies `zstd`, `br` and `gzip` compression with maximum compression level
- Supports download progress (by sending a `Content-Length`)
- Supports download resumption (via range requests)
- Enables client-side caching (via `eTag`)

Out of scope:

- No cache invalidation or expiration (a file will always be served once fetched from the upstream)
- TLS is not supported and should be terminated by some reverse proxy in front

## Usage

Basic usage:

```bash
docker run -d -p 8080:8080 -e TARGET=https://cdn.jsdelivr.net/npm/ -e CACHE_MODE=persistent genhttp/local-mirror

wget http://localhost:8080/jquery/dist/jquery.min.js
```

Docker compose syntax:

```yaml
services:
  mirror:
    image: genhttp/local-mirror
    restart: unless-stopped
    ports:
      - "8080:8080"
    environment:
      - TARGET=https://cdn.jsdelivr.net/npm/
      - CACHE_MODE=persistent # or memory
```

### Cache Modes

By default, the service will store all responses in `/app/cache/`. You can mount this path as a volume to keep
the cache when re-creating the container.

Setting `CACHE_MODE` to `memory` will keep all objects in memory until the container is restarted. There is no
memory limit applied so this mode should only be used in a safe environment where we know how big the cache might grow.

## Development

Local Mirror is implemented with the .NET SDK as a GenHTTP application, so it can be opened in
Visual Studio, Visual Studio Code or Rider.
