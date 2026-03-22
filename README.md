# 🤖 Local AI Stack – Ramapalla

A complete, containerized environment for running **large language models locally** with multiple interfaces and management tools. No cloud API calls, no external dependencies – just pure local AI power.

## 🎯 Project Overview

This repository is a **self-contained AI development environment** featuring:
- **Ollama** – Local LLM inference engine supporting Mistral, Llama2, Gemma, and other models
- **OpenWebUI** – Web-based chat interface for interactive model conversations
- **Dockhand** – Docker management dashboard for container, volume, and image monitoring
- **ClaudeCode Integration** – Configuration guide for VS Code ClaudeCode extension with local Ollama

**Privacy-first architecture**: All models and data stay on your local machine. Perfect for experimentation, development, and learning without cloud costs or external API usage.

## 📊 Architecture Overview

```
┌─────────────────────────────────────────────────┐
│    OLLAMA CORE (Local LLM Inference Engine)     │
│    Port 11434 | ~10GB+ of pre-loaded models     │
└──────────────────┬──────────────────────────────┘
                   │
     ┌─────────────┼──────────────┬──────────────┐
     │             │              │              │
  ┌──▼──┐    ┌─────▼──┐    ┌─────▼─────┐   ┌───▼──┐
  │ CLI │    │OpenWebUI│    │ClaudeCode  │   │Docker│
  │Test │    │ Port    │    │ VS Code    │   │Mgmt  │
  │     │    │ 8080    │    │ Extension  │   │UI    │
  └─────┘    └─────────┘    └────────────┘   └──────┘
```

## 🚀 Quick Start

### Prerequisites
- Docker & Docker Compose installed
- 10+ GB disk space for models
- Minimum 4 CPU cores, 8 GB RAM recommended

### Start All Services

```bash
# Start Ollama first
cd ollama
docker-compose up -d

# Start OpenWebUI (in another terminal)
cd ../openwebui
docker-compose up -d

# (Optional) Start Dockhand for Docker management
cd ../dochand
docker-compose up -d
```

### Access the Services

| Service | URL | Purpose |
|---------|-----|---------|
| **OpenWebUI** | http://localhost:8080 | Chat with AI models via web browser |
| **Dockhand** | http://localhost:3000 | Monitor and manage Docker containers |
| **Ollama API** | http://localhost:11434 | Direct API access for integrations |

## 📁 Project Structure

```
ramapalla/
├── ollama/                    # Ollama LLM inference engine
│   ├── Docker-compose.yaml    # Service configuration
│   ├── models/                # Pre-downloaded model files (~10GB+)
│   └── Readme.md              # Ollama setup documentation
│
├── openwebui/                 # Web UI for Ollama
│   ├── docker-compose.yaml    # Service configuration
│   └── ReadMe.md              # OpenWebUI setup documentation
│
├── dochand/                   # Docker management dashboard
│   ├── docker-compose.yaml    # Service configuration with socket proxy
│   └── ReadMe.md              # Dockhand setup documentation
│
├── claudecode_ollama/         # VS Code ClaudeCode + Ollama integration
│   └── Readme.md              # Setup guide for ClaudeCode extension
│
└── README.md                  # This file
```

## 🔧 Component Details

### Ollama – The AI Engine
- **What it does**: Runs large language models locally
- **Models included**: Mistral, Llama2, Gemma, and others
- **Port**: 11434
- **Storage**: Models stored in `ollama/models/blobs/` (~10GB+)
- **Security**: Configured with resource limits (8GB RAM, 4 CPU cores)

### OpenWebUI – Interactive Interface
- **What it does**: Provides a beautiful web chat interface for Ollama models
- **Port**: 8080
- **Connection**: Automatically connects to local Ollama instance
- **Features**: Model selection, chat history, temperature/parameter tuning

### Dockhand – Container Management
- **What it does**: Visual dashboard for Docker container, volume, and image management
- **Port**: 3000
- **Connection**: Uses socket proxy for secure Docker API access
- **Use case**: Monitor resource usage, manage containers without CLI

### ClaudeCode Integration
- **What it does**: Connects Anthropic's ClaudeCode VS Code extension to your local Ollama models
- **Benefit**: Use local AI in VS Code for code generation and assistance
- **No external API calls**: Everything stays local

## 💡 Usage Examples

### 1. Chat with a Model via Web UI
```bash
# Open browser to http://localhost:8080
# Select model from dropdown → Start chatting!
```

### 2. Test Ollama via CLI
```bash
curl http://localhost:11434/api/chat -d '{
  "model": "mistral",
  "messages": [{"role": "user", "content": "Hello!"}],
  "stream": false
}'
```

### 3. Configure ClaudeCode in VS Code
- See `claudecode_ollama/Readme.md` for configuration steps
- Point ClaudeCode to `http://localhost:11434`
- Use local Ollama for code suggestions

## 🛠️ Common Tasks

### Add a New Model
```bash
# Pull a model into Ollama
docker exec ollama ollama pull llama2

# Verify it loaded
docker exec ollama ollama list
```

### View Running Containers
```bash
# Via Docker CLI
docker ps

# Via Dockhand UI
# Open http://localhost:3000
```

### Stop All Services
```bash
docker-compose down          # In each service directory, or...
docker-compose -f ollama/docker-compose.yaml down
docker-compose -f openwebui/docker-compose.yaml down
docker-compose -f dochand/docker-compose.yaml down
```

## 🐛 Troubleshooting

### Services won't start
- Check Docker and Docker Compose are running
- Verify ports 11434, 8080, 3000 are available
- Check disk space (need 10+ GB for models)

### OpenWebUI can't connect to Ollama
- Ensure Ollama container is running: `docker ps | grep ollama`
- Check Ollama logs: `docker logs <ollama-container-id>`
- Verify network connectivity between containers

### Models not loading in OpenWebUI
- Verify models are downloaded: `docker exec ollama ollama list`
- Check Ollama is responding: `curl http://localhost:11434/api/tags`
- View individual README files in each component folder

## 📚 Individual Component Documentation

For detailed setup, configuration, and troubleshooting for each component:

- [**Ollama Setup**](ollama/Readme.md) – Model management, resource limits, advanced configuration
- [**OpenWebUI Setup**](openwebui/ReadMe.md) – Web interface configuration and features
- [**Dockhand Setup**](dochand/ReadMe.md) – Docker management dashboard
- [**ClaudeCode Integration**](claudecode_ollama/Readme.md) – VS Code extension setup

## 🎓 Learning Path

1. **Start with Ollama** – Understand how local LLM inference works
2. **Try OpenWebUI** – Experience the web interface for chatting
3. **Explore Dockhand** – Learn to manage containers visually
4. **Integrate with VS Code** – Use AI within your development workflow

## 🎯 Use Cases

✅ **Local AI Development** – Build and test AI features without cloud costs  
✅ **Model Experimentation** – Compare different models and parameters  
✅ **Privacy-First AI** – Sensitive data never leaves your machine  
✅ **Educational Projects** – Learn how LLMs work locally  
✅ **Offline AI** – Work with AI even without internet  
✅ **Cost Optimization** – No API usage fees, one-time compute investment  

## 📋 System Requirements

- **OS**: Windows, macOS, or Linux
- **Docker**: Latest version with Docker Compose
- **CPU**: 4+ cores (Intel/AMD x86, Apple Silicon supported)
- **RAM**: 8GB minimum, 16GB+ recommended
- **Storage**: 10GB+ for models (SSD preferred)
- **GPU** (optional): NVIDIA/AMD GPU support speeds up inference

## 📄 License & Author

**Author**: Rama Palla  
**Specialization**: Cloud & Automation Specialist | 15+ Years in IT

Skills: Azure | AKS | Terraform | ArgoCD | FluxCD | PowerShell | GitHub Actions | Prometheus | Grafana

---

**Last Updated**: March 2026  
**Status**: Active Development
