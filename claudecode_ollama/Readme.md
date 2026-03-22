# ClaudeCode + Ollama Configuration

This guide explains how to configure the **ClaudeCode** extension in Visual Studio Code to work with a local Ollama
server running inside a container. It also covers aliasing models, choosing context window and coding quality settings,
and provides a sample `settings.json` snippet to paste into your workspace or user settings.

---
## 🧩 Overview

- **Goal**: use the Anthropic-compatible ClaudeCode extension with models served by Ollama.
- **Assumption**: you already have an Ollama container (see other folders in this repository) exposing the API on
  `http://127.0.0.1:11434`.

## 🚀 Starting Ollama with aliases

If you want to refer to a model by a custom name (alias), configure the container to create the alias at startup.
The `ollama/Docker-compose.yaml` file in this repo includes an example entrypoint that spins up the server and then
copies a base model to an alias called `my-mistral` (you can substitute any model you have downloaded):

```yaml
services:
  ollama:
    # ... existing configuration ...

    # ── Auto-create model aliases on startup ──────────────────────
    entrypoint: >
      sh -c "
      ollama serve &
      sleep 10 &&
      ollama cp mistral my-mistral &&
      # example using qwen2.5-coder:7b
      ollama cp qwen2.5-coder:7b qwen-coder &&
      wait
      "
```

To create additional aliases, simply chain `ollama cp` commands separated by `&&`:

```yaml
sleep 10 &&
ollama cp mistral my-mistral &&
ollama cp llama2 "coding-window-large" &&
ollama cp "neural-chat" "chat-base"
```

- `source-model` is the name shipped by Ollama (`mistral`, `llama2`, ...).
- `alias-name` is the friendly name you will use with ClaudeCode.

## ⚙️ VS Code SETTINGS for ClaudeCode

Add the following JSON snippet to your **User** or **Workspace** settings
(`.vscode/settings.json`) to point the extension at your local Ollama host and avoid login prompts:

```json
{
  "claudeCode.preferredLocation": "panel",
 "claudeCode.environmentVariables": [
    {
      "name": "ANTHROPIC_BASE_URL",
      "value": "http://127.0.0.1:11434"
    },
    {
      "name": "ANTHROPIC_AUTH_TOKEN",
      "value": "ollama"
    },
    {
      "name": "ANTHROPIC_API_KEY",
      "value": ""
    },
    {
      "name": "CLAUDE_CODE_DISABLE_EXPERIMENTAL_BETAS",
      "value": "1"
    }
  ],
  "claudeCode.disableLoginPrompt": true
}
```

### 🧠 Mode choice: context window vs. coding quality

- **Context window mode**: Selecting a model alias that you created specifically for
  larger context (e.g. `coding-window-large`) helps when you need the assistant to see more of your file or repo.
  Use models with higher token limits like `llama2` or `neural-chat` and copy them to a clearly named alias.
- **Coding quality mode**: If you instead want a model tuned for accurate, concise code generation, use
  a separate alias (e.g. `my-mistral` or `coding-high-quality`) and switch between them by updating the
  `claudeCode.model` setting or command palette value in ClaudeCode.

#### 🚨 Root cause of "missing model" errors

When using Ollama, the ClaudeCode extension still defaults to `claude-sonnet-4-6` as its model name. Ollama
doesn't provide that model, so every request fails with a model-not-found error. You can fix this by either:

1. **Creating an Ollama alias** named exactly `claude-sonnet-4-6` that points to a real model, e.g.:
   ```sh
   ollama cp mistral claude-sonnet-4-6
   ```
2. **Overriding the default model** in ClaudeCode via settings or environment variables (see above snippet
   where you could add `"claudeCode.model": "my-mistral"`).

Making one of these adjustments ensures the extension stops hitting the nonexistent default.

> **Tip:** you can run two VS Code windows with different settings or use workspace settings to toggle between
> aliases quickly while experimenting.

## 🛠️ Troubleshooting

### ❌ Ollama not responding
- Ensure the container is running (`docker ps` or `docker-compose up -d`).
- Verify the port is correct (`curl http://127.0.0.1:11434/v1/models`).

### 🔄 Alias not found
- Check `ollama list` inside the container (`docker exec ollama_secure ollama list`).
- Rebuild aliases by restarting the container or running `docker exec ollama_secure ollama cp ...` manually.

### 🗜️ Context limit warning
If the model returns errors about too many tokens, create a new alias pointing to a model with a larger
window or reduce the prompt size.

---

**Author:** Your Name

**License:** MIT
