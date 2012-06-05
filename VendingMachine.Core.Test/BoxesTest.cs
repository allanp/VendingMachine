using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VendingMachine.Core.Test
{
    [TestClass]
    public class BoxesTest
    {
        [TestMethod]
        public void IsEmptyTest()
        {
            /// ProductBox
            ProductBox pBox = new ProductBox("temp product box", 40, 20);

            Assert.IsTrue(pBox.IsEmpty);

            string productName = Guid.NewGuid().ToString();
            int productPrice = Guid.NewGuid().GetHashCode();

            pBox.AddProduct(new Product(productName, productPrice), 0);
            Assert.IsTrue(pBox.IsEmpty);

            Assert.AreEqual(0, pBox[new Product(productName, productPrice)]);

            pBox.AddProduct(new Product(productName, productPrice), 10);
            Assert.IsFalse(pBox.IsEmpty);
            Assert.AreEqual(10, pBox[new Product(productName, productPrice)]);


            /// CoinBox
            CoinBox cBox = new CoinBox("temp coin box");
            Assert.IsTrue(cBox.IsEmpty);

            cBox.AddCoins(new Coin(1), 0);
            Assert.IsTrue(cBox.IsEmpty);
            cBox.AddCoins(new Coin(2), 0);
            Assert.IsTrue(cBox.IsEmpty);
            cBox.AddCoins(new Coin(5), 0);
            Assert.IsTrue(cBox.IsEmpty);

            cBox.AddCoins(new Coin(1), 1);
            Assert.IsFalse(cBox.IsEmpty);

            cBox.Clear();
            Assert.IsTrue(cBox.IsEmpty);
        }
    }
}
