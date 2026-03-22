# Open WebUI

A web-based user interface for interacting with local Ollama models

## Project Overview and Description

Open WebUI is a modern, user-friendly web interface designed to work with Ollama, allowing you to interact with locally-hosted AI models through your browser. This project provides an accessible way to chat with and manage local language models without needing to use command-line tools.

**Key Benefits:**
- Easy web-based access to local Ollama models
- Clean, intuitive user interface
- Support for multiple model selection and management
- Runs locally for complete privacy and control
- Works with Docker Compose for easy deployment

## Installation Instructions

### Prerequisites
- Docker and Docker Compose installed on your system
- Ollama running and configured (see ollama directory in this repository)
- Port 8080 available (or update the configuration)

### Setup with Docker Compose

1. Start the Open WebUI container:
   ```bash
   docker-compose up -d
   ```

2. Access the web interface:
   ```
   http://localhost:8080
   ```

3. The interface should connect to your local Ollama instance

### Local Installation

1. Clone or download this project
   ```bash
   cd openwebui
   ```

2. Install dependencies (if required by your setup)

3. Configure the connection to Ollama (see Configuration Options below)

4. Start the application according to your deployment method

## Configuration Options

### Environment Variables

Configure these settings in your `docker-compose.yaml` or environment:

- **`OLLAMA_BASE_URL`**: URL where Ollama is running (default: `http://ollama:11434`)
- **`WEBUI_PORT`**: Port for the web interface (default: `8080`)
- **`DOCUMENT_DIR`**: Directory for storing documents and chat history
- **`MODEL_NAME`**: Default model to use (optional)

### Docker Compose Configuration

Edit `docker-compose.yaml` to customize:

```yaml
services:
  openwebui:
    environment:
      - OLLAMA_BASE_URL=http://ollama:11434
      - WEBUI_PORT=8080
```

### Model Selection

Once the interface is running:
1. Open http://localhost:8080 in your browser
2. Select your desired model from the model dropdown
3. Begin chatting with the model

## Troubleshooting

### Issue 1: Cannot connect to Ollama
**Problem**: Web UI shows connection error to Ollama  
**Solution**: 
- Verify Ollama is running: `ollama status` or check Docker containers with `docker ps`
- Ensure `OLLAMA_BASE_URL` environmental variable is correctly set to match your Ollama instance
- Check that Ollama is accessible from the web UI container network

### Issue 2: Port is already in use
**Problem**: Docker-compose fails because port 8080 is already in use  
**Solution**: 
- Change the port in `docker-compose.yaml`: `ports: ["8081:8080"]`
- Kill the process using port 8080: `lsof -i :8080` (macOS/Linux) or `netstat -ano | findstr :8080` (Windows)
- Restart Docker Compose

### Issue 3: Models not loading
**Problem**: No models appear in the model selection dropdown  
**Solution**: 
- Ensure Ollama has downloaded models: Run `ollama list` to see available models
- Verify Ollama service is running and accessible
- Check Docker container logs: `docker-compose logs openwebui`

### Issue 4: Slow response times
**Problem**: Chat responses are very slow or timing out  
**Solution**: 
- Check system resources (CPU, RAM, GPU)
- Verify you're using an appropriate model size for your hardware
- Ensure no other heavy processes are interfering
- Increase Docker container memory limit if needed

---

**Related Projects**: See the main [README](../README.md) for information about other components (Ollama, DocHand, Claude Code Ollama)  
**License**: [Your License Here]  
**Last Updated**: March 2026
