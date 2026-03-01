# Dockhand - Docker Container Management UI

Simple and elegant Docker container management dashboard.

## Project Overview and Description

Dockhand is a web-based interface for managing Docker containers, images, and volumes. It provides an easy-to-use dashboard for monitoring and controlling your Docker environment.

- **Real-time container management** - Start, stop, restart, and remove containers
- **Image management** - View and manage Docker images
- **Volume management** - Manage persistent storage
- **System monitoring** - Monitor Docker system resources
- **Security-first approach** - Uses socket-proxy for secure Docker API access

## Installation Instructions

### Linux / WSL2 Setup

#### Step 1: docker-compose.yaml Configuration

Use this configuration for Linux or WSL2:

```yaml
services:
  socket-proxy:
    image: tecnativa/docker-socket-proxy
    container_name: socket-proxy
    restart: unless-stopped
    environment:
      # Core functionality
      - CONTAINERS=1
      - IMAGES=1
      - NETWORKS=1
      - VOLUMES=1
      - EVENTS=1
      - POST=1
      - DELETE=1
      # Dashboard features
      - INFO=1
      - SYSTEM=1
      # Container control
      - ALLOW_START=1
      - ALLOW_STOP=1
      - ALLOW_RESTARTS=1
    volumes:
      - /var/run/docker.sock:/var/run/docker.sock:ro
    networks:
      - socket-proxy

  dockhand:
    image: fnsys/dockhand:latest
    container_name: dockhand
    restart: unless-stopped
    depends_on:
      - socket-proxy
    ports:
      - "3000:3000"
    volumes:
      - dockhand_data:/app/data
    networks:
      - socket-proxy
      - default

networks:
  socket-proxy:
    internal: true

volumes:
  dockhand_data:
```

#### Step 2: Start Docker Compose

```powershell
cd dochand
docker compose up -d
```

#### Step 3: Verify Services are Running

```powershell
docker ps
```

You should see both `socket-proxy` and `dockhand` containers running.

#### Step 4: Access Dockhand

Open your browser and navigate to:
```
http://localhost:3000
```


## Configuration Options

### Environment Variables

- `TZ` - Timezone setting (optional, e.g., `America/New_York`)

### Socket-Proxy Permissions

Control which Docker APIs socket-proxy exposes:

- `CONTAINERS=1` - List and manage containers
- `IMAGES=1` - Access image information
- `VOLUMES=1` - Manage persistent volumes
- `INFO=1` - System and docker info
- `EXEC=1` - Execute commands inside containers (set to 0 if not needed)
- `POST=1` - Allow write operations
- `DELETE=1` - Allow delete operations

## Why Direct Socket Mounting Fails

### ❌ This Configuration Does NOT Work

Many users try this approach on Windows and fail:

```yaml
services:
  dockhand:
    image: fnsys/dockhand:latest
    container_name: dockhand
    ports:
      - "3000:3000"
    volumes:
      - //./pipe/docker_engine://./pipe/docker_engine  # ❌ DOES NOT WORK!
      - dockhand_data:/app/data
    restart: unless-stopped
    environment:
      - TZ=America/New_York
volumes:
  dockhand_data:
```

**Why it fails:**
1. **Container isolation** - Containers cannot directly access Windows named pipes
2. **Permission issues** - No way to grant container permissions to the named pipe
3. **Environment setup fails** - Docker environment variables cannot be properly configured
4. **Network/IPC limitations** - Named pipes require special Windows IPC mechanisms not available in containers

### ✅ The Solution: Use Socket-Proxy

Socket-proxy solves all these issues by acting as a proper gateway:

**Benefits:**
- ✅ Works reliably on all platforms (Windows, Linux, macOS)
- ✅ No permission or environment setup issues
- ✅ Secure by default with granular API control
- ✅ Proper network isolation between containers
- ✅ All Docker API calls properly routed

Simply use the docker-compose.yaml provided in Step 1 with socket-proxy enabled.

## Troubleshooting

### Issue 1: Containers Not Starting
**Problem**: `docker compose up -d` fails or containers immediately stop
**Solution**: 
- Check logs: `docker logs dockhand` and `docker logs socket-proxy`
- Verify Docker Desktop is running
- Ensure ports 3000 is not already in use: `netstat -an | findstr :3000` (Windows) or `lsof -i :3000` (Linux)

### Issue 2: "Cannot connect to Docker daemon"
**Problem**: Dockhand shows connection errors
**Solution**: 
- Verify both containers are running: `docker ps`
- Check network connectivity: `docker network inspect socket-proxy`
- Restart services: `docker compose restart`

### Issue 3: Port 3000 Already in Use
**Problem**: Address already in use error
**Solution**: Change the port in docker-compose.yaml:
```yaml
ports:
  - "8080:3000"  # Access on http://localhost:8080
```

### Issue 4: Data Not Persisting
**Problem**: Dockhand loses data after container restart
**Solution**: Verify the volume exists:
```bash
docker volume ls | grep dockhand_data
docker volume inspect dockhand_data
```

### Issue 5: Trying to use named pipe mounting?
**Problem**: You want to use `//./pipe/docker_engine://./pipe/docker_engine`
**Solution**: 
❌ **Don't do this** - It doesn't work due to container isolation
✅ **Instead** - Use the socket-proxy configuration shown in Step 1
- It's the only reliable way to access Docker from containers on Windows
- No environment setup issues
- Properly handles permissions through the proxy gateway

---

## Architecture

The setup uses a **socket-proxy pattern** for security:

```
┌─────────────────────────────────────────────┐
│         Dockhand Web Interface              │
│         (Port 3000)                         │
└──────────────────┬──────────────────────────┘
                   │ Docker Network
┌──────────────────▼──────────────────────────┐
│   Socket-Proxy Container                    │
│   (Secure Docker API Gateway)               │
└──────────────────┬──────────────────────────┘
                   │ Unix Socket
┌──────────────────▼──────────────────────────┘
│   Docker Daemon                             │
│   (Host Engine)                             │
└─────────────────────────────────────────────┘
```

This architecture ensures:
- ✅ Dockhand never gets direct access to Docker socket
- ✅ Socket-proxy acts as a gateway with granular permissions
- ✅ Secure separation of concerns
- ✅ Easy permission management via environment variables

## How Socket-Proxy Works

The architecture uses socket-proxy as a **security gateway** between Dockhand and the Docker daemon:

**Benefits:**
- 🔒 **Least Privilege** - Dockhand never gets direct socket access
- 🎛️ **Granular Control** - Fine-tune what APIs each container can access
- 🛡️ **Isolation** - Separate networks prevent unintended access
- 📊 **Monitoring** - All Docker API calls go through proxy for audit trails

**Socket-Proxy Environment Flags:**
All permissions are controlled via simple `1`/`0` environment variables:
- Set to `1` to **allow** access to an API
- Set to `0` to **deny** access to an API
- Only expose the minimum APIs your container needs

For Windows users with Docker Desktop, use WSL2 backend for best compatibility with this setup.

---

**Author**: Rama Palla
**Last Updated**: March 2026
