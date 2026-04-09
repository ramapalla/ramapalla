using GitHub.Copilot.SDK;
using Microsoft.Extensions.AI;

// ── 1. Start the client ──────────────────────────────────────────────────────
await using var client = new CopilotClient();
await client.StartAsync();

Console.WriteLine("Client started. Connecting to Ollama...\n");

// ── 2. Create a session pointed at local Ollama ──────────────────────────────
//   Change "codellama:13b" to whatever model you have pulled.
//   Run:  ollama list   to see what's available.
await using var session = await client.CreateSessionAsync(new SessionConfig
{
    Model               = "gemma4:e2b",
    OnPermissionRequest = PermissionHandler.ApproveAll,

    Provider = new ProviderConfig          // remove this block for Copilot subscription
    {
        Type    = "openai",                // Ollama speaks OpenAI-compatible API
        BaseUrl = "http://localhost:11434/v1",
        ApiKey  = "ollama"                 // Ollama ignores this value but field is required
    },

      SystemMessage = new SystemMessageConfig
    {
        Content = "You are a helpful .NET code assistant. Keep answers short.",
        Mode = SystemMessageMode.Replace
    },

    // ── 3. One simple tool — read a file ────────────────────────────────────
    //   The agent will call this itself when it needs to look at code.
    Tools =
    [
        AIFunctionFactory.Create(
            async (string path) =>
            {
                if (!File.Exists(path))
                    return $"File not found: {path}";
                return await File.ReadAllTextAsync(path);
            },
            "read_file",
            "Read the contents of a local file by path")
    ]
});

Console.WriteLine("Session ready. Type a question or 'quit' to exit.");
Console.WriteLine("Try: 'read the file ./hello.cs and tell me what it does'\n");

// ── 4. Simple REPL — type questions, see streamed answers ───────────────────
while (true)
{
    Console.Write("You: ");
    var input = Console.ReadLine()?.Trim();

    if (string.IsNullOrEmpty(input)) continue;
    if (input.Equals("quit", StringComparison.OrdinalIgnoreCase)) break;

    Console.Write("\nCopilot: ");

    var tcs = new TaskCompletionSource();

    session.On(evt =>
    {
        switch (evt)
        {
            case AssistantMessageDeltaEvent delta:
                Console.Write(delta.Data.DeltaContent);   // stream tokens as they arrive
                break;
            case SessionIdleEvent:
                Console.WriteLine("\n");
                tcs.TrySetResult();
                break;
            case SessionErrorEvent err:
                Console.WriteLine($"\n[Error] {err.Data.Message}\n");
                tcs.TrySetResult();
                break;
        }
    });

    await session.SendAsync(new MessageOptions { Prompt = input });
    await tcs.Task;
}

Console.WriteLine("Bye!");
