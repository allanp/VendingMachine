using System;

namespace VendingMachine.Core
{
    public enum MachineStatus
    {
        Unknow,
        Init,
        Running,
        InTransaction,
        OutOfOrder,
        Repairing,
        Shutdown
    }

    public class Machine
    {
        public const string ProductBox = "ProductBox";
        public const string ReceiveCoinBox = "ReceiveCoinBox";
        public const string StoreCoinBox = "StoreCoinBox";

        protected static Product _none = new Product("none", 0);

        protected ProductBox _pBox;
        protected CoinBox _rBox;
        protected CoinBox _sBox;
        protected IMessenger _msg;
        protected MachineStatus _status;

        public MachineStatus Status
        {
            get { return _status; }
            protected set { _status = value; }
        }
        
        public Machine(ProductBox prodcutBox, CoinBox rBox, CoinBox sBox, IMessenger messenger)
        {
            this._pBox = prodcutBox;
            this._rBox = rBox;
            this._sBox = sBox;
            this._msg = messenger;

            this._status = MachineStatus.Init;
        }

        #region - Public methods -

        public void Start()
        {
            if (_pBox == null || _pBox.IsEmpty)
            {
                this._status = MachineStatus.OutOfOrder;
                if (_msg != null)
                    _msg.Show("Mahince is out of order.");
                
                return;
            }

            if (_sBox == null || _sBox.IsEmpty)
            {
                this._status = MachineStatus.OutOfOrder;

                if (_msg != null)
                    _msg.Show("Mahince is out of order.");

                return;
            }

            if (_rBox == null) 
                _rBox = new CoinBox(ReceiveCoinBox);
            _rBox.Clear();

            if (_msg != null)
                _msg.Show("Mahince has started.");

            this._status = MachineStatus.Running;

            if (_msg != null)
                _msg.Show("Mahince is running.");
        }

        public void Shutdown()
        {
            if (_status != MachineStatus.Running)
            {
                _msg.Show("Cannot shutdown.");
                return;
            }

            _status = MachineStatus.Shutdown;
            _msg.Show("Shutdown completed.");
        }

        public void InsertCoin(Coin coin)
        {
            if (_status != MachineStatus.Running)
                throw new Exception("The machine is not running.");

            if (_rBox == null) _rBox = new CoinBox(ReceiveCoinBox);

            _rBox.AddCoins(coin, 1);

            _msg.Show(string.Format("Coin {0} has inserted.", coin));
        }

        public void CancelPurchase()
        {
            DoPurchase(_none);
        }

        public bool DoPurchase(Product product)
        {
            if (_pBox == null) throw new InvalidOperationException("_pBox");

            bool purchased = false;

            MachineStatus oldStatus = _status;
            _status = MachineStatus.InTransaction;

            CoinBox changes = _rBox;
            try
            {
                if (product.Equals(_none))
                    throw new Exception();

                int count = _pBox[product];
                if (count < 0)
                    throw new Exception("No such product is avaiable.");

                if (count == 0)
                    throw new Exception("The product is out of sale.");

                int changesValue = _rBox.TotalValue - product.Price;

                if (changesValue < 0)
                    throw new Exception("Not enough money.");

                changes = OnMakeChanges(changesValue);

                if (changes == null)
                {
                    changes = _rBox;
                    throw new Exception("Cannot return changes.");
                }

                DeliverProduct(product);

                purchased = true;
            }
            catch (Exception ex)
            {
                if (_msg != null)
                    _msg.Show(ex.Message);
            }
            finally
            {
                ReturnChanges(changes);

                _status = oldStatus;
            }

            return purchased;
        }

        public void DisplayProducts()
        {
            var sb = new System.Text.StringBuilder();

            sb.AppendLine("Products");
            sb.AppendLine(_pBox.ToString());

            if (_msg != null)
            {
                _msg.Show(sb.ToString());
            }
        }

        #endregion // - Public methods -

        #region - Protected methods -

        protected virtual CoinBox OnMakeChanges(int changesValue)
        {
            return MakeChangesCore(changesValue);
        }

        #endregion // - Protected methods -

        #region - Private methods -

        private void DeliverProduct(Product product)
        {
            if (_pBox[product] <= 0)
                throw new InvalidOperationException("product");

            if (_msg != null)
                _msg.Show(string.Format("Delivering product: {0}", product));

            _pBox[product]--;
        }

        private void ReturnChanges(CoinBox changes)
        {
            if (changes == null)
                throw new ArgumentNullException("changes");

            if (_msg != null)
            {
                if (changes.IsEmpty)
                {
                    _msg.Show("No changes.");
                }
                else
                {
                    _msg.Show(string.Format("Changes: {0}", changes));        
                }
            }

            _rBox.Clear();
        }

        private CoinBox MakeChangesCore(int changesValue)
        {
            CoinBox totalCoins = _sBox + _rBox;

            CoinBox changes = new CoinBox("changes");

            int temp = changesValue;
            foreach (var coin in totalCoins.OrderByCoinValue(true))
            {
                if (temp == 0)
                    break;

                while (temp > 0)
                {
                    if (coin.Key.Value > temp || coin.Value == 0)
                        break;

                    totalCoins.RemoveCoins(coin.Key, 1);
                    changes.AddCoins(coin.Key, 1);

                    temp -= coin.Key.Value;

                    if (temp == 0)
                        break;
                }
            }

            if (temp != 0)
            {
                /// not enough coins to make changes
                return null;
            }
            else
            {
                /// changes has made

                _sBox = totalCoins;
                
                return changes;
            }
        }

        #endregion // - Private methods -
    }
}
