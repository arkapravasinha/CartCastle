namespace CartCastle.Domain
{
    public record LineItem
    {
        public LineItem(Guid productId,int qty, double pricePerQuantity)
        {
            ProductId = productId;
            Qty = qty;
            PricePerQuantity = pricePerQuantity;
        }
        public Guid ProductId { get; init; }
        public int Qty { get; init; }
        public double PricePerQuantity { get; init; }
    }
}
