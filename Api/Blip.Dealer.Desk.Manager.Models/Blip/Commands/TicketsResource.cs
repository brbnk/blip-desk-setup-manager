namespace Blip.Dealer.Desk.Manager.Models.Blip.Commands;

public sealed class TicketsResponse
{
  public IEnumerable<Ticket> Resource { get; set; }
}

public sealed class Ticket
{
  public string Id { get; set; }
}
