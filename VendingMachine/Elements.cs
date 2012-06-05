
namespace VendingMachine.Core
{
    public struct Product
    {
        private string _name;
        private int _price;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int Price
        {
            get { return _price; }
            set { _price = value; }
        }

        public Product(string name, int price)
        {
            this._name = name;
            this._price = price;
        }

        public override string ToString()
        {
            return string.Format("{0}({1})", _name, _price);
        }
    }

    public struct Coin
    {
        private int _value;

        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public Coin(int value) { this._value = value; }

        public override string ToString()
        {
            return string.Format("{0}p", _value);
        }
    }
}
