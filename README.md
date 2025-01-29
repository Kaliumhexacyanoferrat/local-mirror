# Local Mirror

Simple docker service to mirror large objects served by a HTTP server. 

Features & Distinction:

- Caches large responses of an upstream server on local disc or in memory
- Automatically applies compression, if applicable (e.g. on large JSON responses)
- Supports the `Vary` header on upstream responses
- There is no cache invalidation (a file will always be served once fetched from the upstream)

## Usage

Basic usage:

```bash
docker run -d -p 8080:8080 -e TARGET=http://server.to/mirror/ -e CACHE_MODE=persistent genhttp/local-mirror

wget http://localhost:8080/some/large.blob
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
      - TARGET=http://server.to/mirror/
      - CACHE_MODE=persistent # or memory
```

### Cache Modes

By default, the service will store all responses in `/app/cache/`. You can mount this path as a volume to keep
the cache when re-creating the container.

Setting `CACHE_MODE` to `memory` will keep all objects in memory until the container is restarted. There is no
memory limit applied so this mode should only be used in a safe environment where we know how big the cache might grow.