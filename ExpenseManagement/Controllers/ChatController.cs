using ExpenseManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost]
    public async Task<ActionResult<ChatResponse>> Post([FromBody] ChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest("Message is required");
        }

        var response = await _chatService.ProcessMessageAsync(request.Message);
        return Ok(new ChatResponse { Response = response });
    }
}

public class ChatRequest
{
    public string Message { get; set; } = string.Empty;
}

public class ChatResponse
{
    public string Response { get; set; } = string.Empty;
}
