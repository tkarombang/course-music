using System;
using System.Collections.Generic;
using System.Linq;
using Cart.Models;

namespace Frontend.Services
{
    public class CartService
    {
        private readonly List<CartItem> _cart = new();
        public event Action? OnChange;

        public IReadOnlyList<CartItem> GetCart() => _cart;

        public void AddToCart(CartItem item)
        {
            var existing = _cart.FirstOrDefault(c => c.IdCourse  == item.IdCourse );
            if (existing != null)
                existing.Quantity += item.Quantity;
            else
                _cart.Add(item);

            NotifyStateChanged();
        }

        public void RemoveFromCart(int id)
        {
            var item = _cart.FirstOrDefault(c => c.IdCourse  == id);
            if (item != null)
                _cart.Remove(item);

            NotifyStateChanged();
        }

        public void ClearCart()
        {
            _cart.Clear();
            NotifyStateChanged();
        }

        // ✅ Perbaikan di sini
        public decimal GetTotal()
        {
            // Jika menggunakan checkbox (IsSelected)
            return _cart.Where(c => c.IsSelected)
                        .Sum(c => c.Price * c.Quantity);
        }

        // ✅ Jika ingin menghitung total semua item tanpa memperhatikan IsSelected
        public decimal GetTotalAll()
        {
            return _cart.Sum(c => c.Price * c.Quantity);
        }

        public void NotifyStateChanged() => OnChange?.Invoke();
    }
}
