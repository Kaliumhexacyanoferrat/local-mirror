# Local Mirror

Docker service to create a network-local mirror of a remote server providing files via HTTP.

![CI](https://github.com/Kaliumhexacyanoferrat/local-mirror/workflows/CI/badge.svg) [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=local-mirror&metric=coverage)](https://sonarcloud.io/dashboard?id=local-mirror) [![Docker Hub](https://img.shields.io/docker/pulls/genhttp/local-mirror.svg)](https://hub.docker.com/r/genhttp/local-mirror)

## Features

- Caches responses of an upstream server on local disc or in memory
- Serves more than 100k of requests per second, depending on the size
- Simple setup, just pass the upstream URL
- Enables `zstd`, `br` and `gzip` compression with maximum compression level
- Enables download progress (by sending a `Content-Length`)
- Enables download resumption (via range requests)
- Enables client-side caching (via `eTag`)

Out of scope:

- No cache invalidation or expiration (a file will always be served once fetched from the upstream)
- TLS is not supported and should be terminated by some reverse proxy in front

## Usage

Basic usage:

```bash
docker run -d -p 8080:8080 -e TARGET=https://cdn.jsdelivr.net/npm/ genhttp/local-mirror

wget http://localhost:8080/jquery/dist/jquery.min.js
```

Docker compose syntax:

```yaml
services:
  mirror:
    image: genhttp/local-mirror
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
