using System;
using System.Collections.Generic;
using System.Linq;

namespace VendingMachine.Core
{
    public abstract class StoreBox<T> where T : struct
    {
        protected string _name;
        protected Dictionary<T, int> _items;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public virtual bool IsEmpty
        {
            get
            {
                if (_items == null || _items.Count == 0)
                    return true;

                return _items.Values.Sum() == 0;
            }
        }

        protected StoreBox(string name)
        {
            this._name = name;
            this._items = new Dictionary<T, int>();
        }

        public virtual int this[T item]
        {
            get
            {
                if (_items != null)
                    return _items[item];
                return -1;
            }
            internal set
            {
                if (_items == null || !_items.ContainsKey(item))
                    throw new ArgumentException("item");

                _items[item] = value;
            }
        }

        public void Clear()
        {
            if (_items != null)
                _items.Clear();
        }

        public override string ToString()
        {
            if (!IsEmpty)
            {
                var sb = new System.Text.StringBuilder();
                var lastItem = _items.Last();

                foreach (var item in _items)
                {
                    sb.AppendFormat("{0}: {1}", item.Key.ToString(), item.Value);

                    if (!item.Equals(lastItem))
                    {
                        sb.Append("; ");
                    }
                    else
                    {
                        sb.Append(".");
                    }
                }
                sb.AppendLine();

                return sb.ToString();
            }
            else
            {
                return "Empty";
            }
        }
    }

    public class ProductBox : StoreBox<Product>
    {
        public int this[string productName]
        {
            get
            {
                if (_items != null)
                {
                    foreach (var item in _items)
                    {
                        if (string.Equals(productName, item.Key.Name))
                            return item.Value;
                    }
                }
                return -1;
            }
        }

        private int _typesMax;
        private int _eachMax;

        public ProductBox(string name, int typesMax, int eachMax)
            : base(name)
        {
            this._typesMax = typesMax;
            this._eachMax = eachMax;
        }

        public void AddProduct(Product product, int count)
        {
            if (!this._items.ContainsKey(product))
            {
                if (this._items.Count + 1 > _typesMax)
                    throw new InvalidOperationException("Too many types of products");

                this._items.Add(product, 0);
            }

            if (_items[product] + count > _eachMax)
                throw new InvalidOperationException("Too many number of the product.");

            this._items[product] += count;
        }
    }

    public class CoinBox : StoreBox<Coin>
    {
        public override int this[Coin coin]
        {
            get
            {
                if (_items != null && _items.ContainsKey(coin))
                    return _items[coin];
                return -1;
            }
            internal set
            {
                if (_items == null)
                    throw new InvalidOperationException();

                if (!_items.ContainsKey(coin))
                    _items.Add(coin, 0);
                _items[coin] = value;
            }
        }

        public int TotalValue
        {
            get
            {
                int total = 0;
                if (_items != null)
                {
                    foreach (var c in _items)
                        total += c.Key.Value * c.Value;
                }
                return total;
            }
        }

        public CoinBox(string name) : base(name) { }

        public bool RemoveCoins(Coin coin, int count)
        {
            if (this._items[coin] <= 0)
                return false;

            this._items[coin] -= count;
            return true;
        }

        public void AddCoins(Coin coin, int count)
        {
            if (!this._items.ContainsKey(coin))
                this._items.Add(coin, 0);

            this._items[coin] += count;
        }

        internal IOrderedEnumerable<KeyValuePair<Coin, int>> OrderByCoinValue(bool descending)
        {
            if (descending)
            {
                return _items.OrderByDescending(c => c.Key.Value);
            }
            else
            {
                return _items.OrderBy(c => c.Key.Value);
            }
        }

        public static CoinBox operator +(CoinBox cb1, CoinBox cb2)
        {
            if (cb1 == null && cb2 == null)
                throw new ArgumentNullException("s1");

            CoinBox cb = CoinBox.DeepCopy(cb1);

            if (cb2 == null || cb2.IsEmpty)
                return cb;

            foreach (var c2 in cb2._items)
            {
                if (!cb1._items.ContainsKey(c2.Key))
                {
                    cb._items.Add(c2.Key, c2.Value);
                }
                else
                {
                    cb._items[c2.Key] += c2.Value;
                }
            }

            return cb;
        }

        private static CoinBox DeepCopy(CoinBox cb1)
        {
            CoinBox coinBox = new CoinBox(cb1.Name);
            foreach (var c in cb1._items)
            {
                coinBox.AddCoins(c.Key, c.Value);
            }
            return coinBox;
        }
    }
}
