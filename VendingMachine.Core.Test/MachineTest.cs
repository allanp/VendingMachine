using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;

namespace VendingMachine.Core.Test
{
    [TestClass]
    public class MachineTest
    {
        private TestMachine machine;
        private ProductBox pBox;
        private CoinBox sBox, rBox;

        
        
        private static readonly Coin one = new Coin(1), 
                                     five = new Coin(5), 
                                     ten = new Coin(10), 
                                     twenty = new Coin(20);

        private static readonly Product water = new Product("water", 10),   // name, price
                                        soda = new Product("soda", 15), 
                                        cola = new Product("cola", 25);

        internal class TestMessenger : IMessenger
        {
            public static readonly TestMessenger Default = new TestMessenger();

            List<string> messages = new List<string>();

            public void Show(string message)
            {
                messages.Add(string.Format("{0}:: {1}", DateTime.Now.ToString("yyyy-MM-dd H:m:ss"),message));
            }

            public void Clear()
            {
                messages.Add(string.Empty);
            }

            public void ViewLog()
            {
                MessageBox.Show(string.Join(Environment.NewLine, messages));
            }
        }

        internal class TestMachine : Machine
        {
            public TestMachine(ProductBox pBox, CoinBox rBox, CoinBox sBox, TestMessenger messenger) :
                base(pBox, rBox, sBox, messenger)
            {
            }

            internal CoinBox RBox { get { return _rBox; } }
            internal CoinBox SBox { get { return _sBox; } }
            internal ProductBox PBox { get { return _pBox; } }

            protected override CoinBox OnMakeChanges(int changesValue)
            {
                return base.OnMakeChanges(changesValue);
            }
        }

        [TestInitialize]
        public void TestInit()
        {
            pBox = new ProductBox("products", 40, 20); // name, typs of products, capacity of each product

            pBox.AddProduct(water, 20); // product, count
            pBox.AddProduct(soda, 20);
            pBox.AddProduct(cola, 20);

            ///  100 x 1p, 50 x 5p, 50 x 10p
            sBox = new CoinBox(Machine.StoreCoinBox);
            sBox.AddCoins(one, 100); // coin, count
            sBox.AddCoins(five, 50);
            sBox.AddCoins(ten, 50);

            rBox = new CoinBox(Machine.ReceiveCoinBox);
            rBox.Clear();

            machine = new TestMachine(pBox, rBox, sBox, TestMessenger.Default);

            machine.Start();
        }

        [TestMethod]
        public void MakeChangesCoreTest()
        {
            sBox.Clear();
            sBox.AddCoins(ten, 1);
            sBox.AddCoins(five, 1);

            bool purchased = false;

            /// Mahince contains: 5p:1;10p:1, and a customer inserts two twenty-coin, and purchase a cola(25)
            /// returns changes 10p+5p, and cola
            
            machine.InsertCoin(twenty);
            machine.InsertCoin(twenty);

            purchased = machine.DoPurchase(cola); // cola.price = 25

            Assert.IsTrue(purchased);
            Assert.AreEqual(19, pBox[cola]);

            Assert.IsTrue(machine.SBox[ten] <= 0);
            Assert.IsTrue(machine.SBox[five] <= 0);
            Assert.AreEqual(2, machine.SBox[twenty]);
            Assert.IsTrue(machine.RBox.IsEmpty);

            /// sBox: 5p: 0; 10p: 0; 20p: 2.
            /// Mahince contains: 5p:0;10p:0;20p:2, and a customer inserts one ten-coin, and purchase a apple(15)
            /// returns changes 10p, but not the apple
            
            machine.InsertCoin(ten);

            /// sBox: 5p: 0; 10p; 1; 20p: 2.

            purchased = machine.DoPurchase(soda); // soda.price = 15

            Assert.IsFalse(purchased);

            Assert.AreEqual(20, pBox[soda]);

            Assert.IsTrue(machine.RBox.IsEmpty);
            Assert.AreEqual(0, machine.SBox[ten]);
            Assert.AreEqual(2, machine.SBox[twenty]);

            TestMessenger.Default.ViewLog();
        }
    }
}
