namespace Hyip_Payments.Web.Client.DTOs
{
    public class InvoiceItemDto
    {
        public int ProductId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total => Quantity * UnitPrice;
    }

    public class ProductSelectionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class InvoiceDto
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public string? Description { get; set; }
        public decimal TotalAmount { get; set; }
        public int? CustomerId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class AddInvoiceWithProductsCommand
    {
        public InvoiceDto Invoice { get; set; } = null!;
        public List<InvoiceItemDto> Items { get; set; } = new();
    }

    public class InvoiceWithItemsDto
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}
