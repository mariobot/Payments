namespace Hyip_Payments.Web.Client.Services
{
    /// <summary>
    /// Service for managing the shopping cart
    /// </summary>
    public interface ICartService
    {
        event Action? OnChange;
        void AddItem(int productId, string productName, decimal unitPrice, int quantity = 1);
        List<CartItem> GetItems();
        int GetItemCount();
        void UpdateQuantity(int productId, int quantity);
        void RemoveItem(int productId);
        void ClearCart();
        decimal GetTotal();
    }

    public class CartService : ICartService
    {
        private readonly List<CartItem> _cartItems = new();
        public event Action? OnChange;

        public void AddItem(int productId, string productName, decimal unitPrice, int quantity = 1)
        {
            var existingItem = _cartItems.FirstOrDefault(x => x.ProductId == productId);
            
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                _cartItems.Add(new CartItem
                {
                    ProductId = productId,
                    ProductName = productName,
                    UnitPrice = unitPrice,
                    Quantity = quantity
                });
            }

            NotifyStateChanged();
        }

        public List<CartItem> GetItems()
        {
            return _cartItems.ToList();
        }

        public int GetItemCount()
        {
            return _cartItems.Sum(x => x.Quantity);
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var item = _cartItems.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                if (quantity <= 0)
                {
                    _cartItems.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }
                NotifyStateChanged();
            }
        }

        public void RemoveItem(int productId)
        {
            var item = _cartItems.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                _cartItems.Remove(item);
                NotifyStateChanged();
            }
        }

        public void ClearCart()
        {
            _cartItems.Clear();
            NotifyStateChanged();
        }

        public decimal GetTotal()
        {
            return _cartItems.Sum(x => x.Total);
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Total => UnitPrice * Quantity;
    }
}
