using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using WebApi.Models;

[Route("api/logs")]
[ApiController]
public class LogController : ControllerBase
{
    private readonly RwagrillContext _context;

    public LogController(RwagrillContext context)
    {
        _context = context;
    }

    [HttpGet("get/{n?}")]
    public IActionResult GetLogs(int n = 10)
    {
        Console.WriteLine($"GetLogs called with n={n}");

        var logCount = _context.Logs.Count();
        Console.WriteLine($"Log count in database: {logCount}");

        if (logCount == 0)
        {
            return Ok(new List<Log>());
        }

        var logsToTake = Math.Min(n, logCount);
        var logs = _context.Logs
            .OrderByDescending(log => log.Timestamp)
            .Take(logsToTake)
            .ToList();

        Console.WriteLine($"Logs retrieved: {logs.Count}");

        return Ok(logs);
    }

    [HttpGet("count")]
    public IActionResult GetLogCount()
    {
        var count = _context.Logs.Count();
        return Ok(new { Count = count });
    }
}
