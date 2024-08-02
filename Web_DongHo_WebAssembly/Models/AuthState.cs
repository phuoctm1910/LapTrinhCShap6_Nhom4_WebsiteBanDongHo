using System;

namespace Web_DongHo_WebAssembly.Models
{
    public class AuthState
    {
        public event Action OnChange;

        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                _isLoggedIn = value;
                NotifyStateChanged();
            }
        }

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                NotifyStateChanged();
            }
        }

        private int _cartItemCount;
        public int CartItemCount
        {
            get => _cartItemCount;
            set
            {
                _cartItemCount = value;
                NotifyStateChanged();
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        public void UpdateCartItemCount(int count)
        {
            CartItemCount = count;
            NotifyStateChanged();
        }
    }
}
