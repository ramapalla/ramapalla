# Ollama Local Setup with Docker Compose

Run Ollama locally in a secure, containerized environment using Docker Compose.

## Project Overview and Description

This project provides a Docker Compose configuration to run Ollama (an open-source large language model inference platform) locally on your machine. It includes:

- **Security hardening** - Restricted privileges, capability dropping, and read-only filesystem
- **Local network isolation** - Ollama runs on localhost only (127.0.0.1:11434)
- **Persistent model storage** - Models are stored locally for reuse
- **Resource limits** - Memory and CPU constraints to prevent system overload
- **Easy management** - Simple Docker Compose commands for start, stop, and updates

Perfect for developers and researchers who want to run large language models locally without exposing services to the network.

## Installation Instructions

### Prerequisites

- Docker installed on your system
- Docker Compose (usually included with Docker Desktop)
- At least 8GB of free disk space for models

### Quick Start

1. Clone or download the repository
   ```
   git clone [repository-url]
   cd ollama
   ```

2. Start Ollama with Docker Compose
   ```
   docker-compose up -d
   ```

3. Wait for the container to start and verify it's running
   ```
   docker-compose ps
   ```

4. Pull your first model (example: llama2)
   ```
   docker exec ollama_secure ollama pull llama2
   ```

5. Test the setup by querying the model
   ```
   curl http://127.0.0.1:11434/api/generate -d '{"model": "llama2", "prompt": "Why is the sky blue?"}'
   ```

### Stop Ollama

To stop the service:
```
docker-compose down
```

## Configuration Options

The Docker Compose configuration includes several customizable settings:

### Environment Variables

- **OLLAMA_HOST**: Server binding address (default: 0.0.0.0 internally)
- **OLLAMA_KEEP_ALIVE**: Time models stay loaded in memory (default: 5m)
- **OLLAMA_MAX_LOADED_MODELS**: Number of models to keep loaded simultaneously (default: 1)
- **OLLAMA_MODELS**: Directory where models are stored (default: /root/.ollama/models)

### Resource Limits

Adjust these based on your system capabilities:

```yaml
mem_limit: 8g          # Maximum memory (adjust for your system)
memswap_limit: 8g      # Maximum memory swap
cpus: "4"              # Number of CPU cores
```

### Storage Configuration

Models are stored in a local `./models` directory:

```yaml
volumes:
  - ./models:/root/.ollama/models
```

This creates a `models` folder in your project directory where all downloaded models will be stored.

### Security Settings

The configuration includes hardened security options:

- `no-new-privileges`: Prevents privilege escalation
- Dropped all unnecessary capabilities
- Added only needed capabilities: NET_BIND_SERVICE, CHOWN, SETUID, SETGID
- Temporary space allocated in tmpfs (512MB)

## Troubleshooting

[To be updated]

---

**Author**: [Your Name]  
**License**: [License Type]
