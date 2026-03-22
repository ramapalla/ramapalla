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


## GitHub Authentication

### Adding GitHub Credentials to Dockhand

To enable pulling private Docker images or repositories from GitHub Container Registry (GHCR):

#### Step 1: Generate GitHub Personal Access Token

1. Go to [GitHub Settings → Developer Settings → Personal Access Tokens](https://github.com/settings/tokens)
2. Click **Generate new token** → **Generate new token (classic)**
3. Set the following scopes:
   - `read:packages` - Read container packages
   - `write:packages` - Push container packages
   - `delete:packages` - Delete packages
4. Copy the generated token and store it securely

#### Step 2: Configure in Docker

Before running Dockhand, authenticate with GHCR:

```powershell
# Login to GitHub Container Registry
docker login ghcr.io -u YOUR_GITHUB_USERNAME -p YOUR_PERSONAL_ACCESS_TOKEN

# Or use Docker credentials file
echo YOUR_PERSONAL_ACCESS_TOKEN | docker login ghcr.io -u YOUR_GITHUB_USERNAME --password-stdin
```

#### Step 3: Add to Docker Compose

Update your docker-compose.yaml to include registry authentication:

```yaml
services:
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
      - ~/.docker/config.json:/root/.docker/config.json:ro  # Mount Docker credentials
    networks:
      - socket-proxy
      - default
```

#### Step 4: Access Private Images in Dockhand

Once authenticated, you can pull private images from GitHub Container Registry:
- Image format: `ghcr.io/your-username/your-repo:tag`
- Dockhand will use the mounted credentials to authenticate


## Deploying from GitHub Repositories

### Pull Docker-Compose from GitHub and Deploy

#### Step 1: Clone Repository Containing Docker-Compose Files

```powershell
# Clone your repository
git clone https://github.com/ramapalla/ramapalla.git
cd ramapalla
```

#### Step 2: Navigate to Service Directory

Each service (ollama, openwebui, etc.) has its own docker-compose.yaml:

```powershell
# Example: Deploy Ollama service
cd ollama
```

#### Step 3: Deploy Using Dockhand UI

1. Open Dockhand at `http://localhost:3000`
2. Navigate to **Services** or **Compose** section
3. Click **New Service** or **Import Compose File**
4. Either:
   - Paste the contents of your docker-compose.yaml
   - Upload/Link to the GitHub raw file URL
5. Click **Deploy** to start the service

#### Step 4: Monitor Deployment

```powershell
# From terminal
docker compose logs -f

# Or use Dockhand UI Logs tab to view output
```

#### Deploying from GitHub URL (Advanced)

To deploy directly from GitHub using raw content URL:

```powershell
# Get raw docker-compose.yaml from GitHub
curl -O https://raw.githubusercontent.com/ramapalla/ramapalla/main/ollama/Docker-compose.yaml

# Deploy using Docker
docker compose -f Docker-compose.yaml up -d
```

Or in Dockhand, use the raw URL file feature:
- URL: `https://raw.githubusercontent.com/ramapalla/ramapalla/main/ollama/Docker-compose.yaml`


## Docker-Compose Management in Dockhand UI

### Create a New Docker-Compose Service

#### Step 1: Access Compose Editor

1. Open Dockhand dashboard at `http://localhost:3000`
2. Go to **Compose** or **Services** section
3. Click **Create New Compose File** or **New Service**

#### Step 2: Define Your Service

Use the visual editor or YAML editor to define services:

**Example - Adding a Simple Service:**

```yaml
version: "3.8"
services:
  my-app:
    image: nginx:latest
    container_name: my-app
    restart: unless-stopped
    ports:
      - "8080:80"
    volumes:
      - ./data:/usr/share/nginx/html
    networks:
      - default
```

#### Step 3: Deploy the Service

1. Click **Validate** to check for syntax errors
2. Review the configuration
3. Click **Deploy** to start containers

#### Step 4: Manage Running Services

Once deployed, use Dockhand to:
- **View logs**: Click service → **Logs** tab
- **Restart**: Click **Restart** button
- **Stop**: Click **Stop** to gracefully stop
- **Remove**: Click **Remove** to delete containers
- **Edit**: Modify configuration and redeploy

### Edit Existing Docker-Compose Files

#### Step 1: Locate Service in Dockhand

1. Go to **Compose** → Select your service
2. Click **Edit** or **View Configuration**

#### Step 2: Modify Configuration

- Add/remove services
- Update environment variables
- Change port mappings
- Adjust volume mounts

#### Step 3: Save and Redeploy

1. Click **Save Changes**
2. Choose **Redeploy** to apply changes
3. Monitor the **Events** log for deployment status

### View and Export Docker-Compose

- Click **Export** to download the current compose configuration
- Share with team members or commit to version control


## Git Integration Workflow

### Version Control for Docker-Compose Configurations

#### Step 1: Configure Git Repository

```powershell
# Initialize git (if not already done)
git init

# Configure user identity
git config --global user.name "Your Name"
git config --global user.email "your.email@example.com"

# Add remote repository
git remote add origin https://github.com/ramapalla/ramapalla.git
```

#### Step 2: Create Service-Specific Branches

For each service, create dedicated branches:

```powershell
# Feature branch for service configuration
git checkout -b feature/service-ollama
git checkout -b feature/service-openwebui
```

#### Step 3: Commit Docker-Compose Changes

```powershell
# After making changes in Dockhand
git status
git add ollama/Docker-compose.yaml
git add openwebui/docker-compose.yaml
git commit -m "Update: Configured ollama with memory limits and volumes"
```

#### Step 4: Push to GitHub

```powershell
# Push to your branch
git push origin feature/service-ollama

# Create Pull Request for review
# Go to GitHub and create PR: feature/service-ollama → main
```

#### Step 5: Pull Latest Configuration from GitHub

Keep your local deployment synchronized:

```powershell
# Fetch latest changes
git fetch origin

# Switch to main branch
git checkout main

# Pull latest configuration
git pull origin main

# Redeploy services with new configuration
cd ollama
docker compose down
docker compose up -d
```

### Using Dockhand with Version Control

**Workflow for Team Collaboration:**

1. **Developer makes changes in Dockhand UI**
2. **Export configuration** from Dockhand
3. **Commit to Git** with meaningful message
4. **Push to feature branch** on GitHub
5. **Create Pull Request** for review
6. **After approval**, merge to main
7. **Production deployment** pulls latest from main

### Sync Dockhand with Remote Repository

To keep Dockhand configuration in sync with GitHub:

```powershell
# After pulling latest changes
git pull origin main

# If docker-compose changed, redeploy
docker compose down
docker compose up -d

# Verify in Dockhand UI
# Navigate to Services and confirm updates
```

### Best Practices for Git Integration

- **Commit messages**: Use descriptive messages (e.g., "Update: Added resource limits to ollama")
- **Branch naming**: Use `feature/`, `bugfix/`, `hotfix/` prefixes
- **Before merging**: Test changes in Dockhand first
- **Tag releases**: Create version tags after merge to main: `git tag -a v1.0.0 -m "Release v1.0.0"`


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
