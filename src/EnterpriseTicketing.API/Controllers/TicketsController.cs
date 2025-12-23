using Microsoft.AspNetCore.Mvc;
using EnterpriseTicketing.Application.Interfaces;
using EnterpriseTicketing.Domain.Entities;

namespace EnterpriseTicketing.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Ticket>>> GetDepartmentTickets(string department)
    {
        var tickets = await _ticketService.GetDepartmentTicketsAsync(department);
        return Ok(tickets);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Ticket>> GetTicket(Guid id)
    {
        var ticket = await _ticketService.GetTicketByIdAsync(id);
        if (ticket == null)
            return NotFound();

        return Ok(ticket);
    }

    [HttpPost]
    public async Task<ActionResult<Ticket>> CreateTicket(Ticket ticket)
    {
        var createdTicket = await _ticketService.CreateTicketAsync(ticket);
        return CreatedAtAction(nameof(GetTicket), new { id = createdTicket.Id }, createdTicket);
    }

    [HttpPut("{id}/assign")]
    public async Task<IActionResult> AssignTicket(Guid id, Guid assignedToId)
    {
        var result = await _ticketService.AssignTicketAsync(id, assignedToId);
        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpPut("{id}/close")]
    public async Task<IActionResult> CloseTicket(Guid id, string resolution)
    {
        var result = await _ticketService.CloseTicketAsync(id, resolution);
        if (!result)
            return NotFound();

        return NoContent();
    }
}