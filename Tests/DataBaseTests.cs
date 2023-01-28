using System.IO;
using System.Linq;
using DemoSolution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewVariant.Exceptions;
using NewVariant.Interfaces;
using NewVariant.Models;

namespace Tests {
    [TestClass]
    public class DataBaseTests {
        public DataBaseTests() {
            _dataBase = new DataBase();
            _dataAccessLayer = new DataAccessLayer();
            GenerateData();
        }

        [TestMethod]
        public void AllGoodsOfLongestNameBuyerTest() {
            Good[] expected = {
                _goods[0],
                _goods[1],
                _goods[5]
            };

            CollectionAssert.AreEquivalent(expected, _dataAccessLayer.GetAllGoodsOfLongestNameBuyer(_dataBase).ToArray());
        }

        [TestMethod]
        public void MostExpensiveGoodCategoryTest() {
            Assert.AreEqual("Electronics", _dataAccessLayer.GetMostExpensiveGoodCategory(_dataBase));
        }

        [TestMethod]
        public void MinimumSalesCityTest() {
            Assert.AreEqual("Quahog", _dataAccessLayer.GetMinimumSalesCity(_dataBase));
        }

        [TestMethod]
        public void MostPopularGoodBuyersTest() {
            Buyer[] expected = {
                _buyers[0],
                _buyers[1]
            };
            
            CollectionAssert.AreEquivalent(expected, _dataAccessLayer.GetMostPopularGoodBuyers(_dataBase).ToArray());
        }

        [TestMethod]
        public void MinimumNumberOfShopsInCountryTest() {
            Assert.AreEqual(1, _dataAccessLayer.GetMinimumNumberOfShopsInCountry(_dataBase));
        }
        
        [TestMethod]
        public void OtherCitySalesTest() {
            Sale[] expected = {
                _sales[2],
                _sales[3],
                _sales[6],
                _sales[7],
                _sales[8],
                _sales[9]
            };

            CollectionAssert.AreEquivalent(expected, _dataAccessLayer.GetOtherCitySales(_dataBase).ToArray());
        }

        [TestMethod]
        public void TotalSalesValueTest() {
            Assert.AreEqual(2139, _dataAccessLayer.GetTotalSalesValue(_dataBase));
        }

        [TestMethod]
        public void SerializationTest() {
            const string fileName = "shops.json";
            var path = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            _dataBase.Serialize<Shop>(path);
            _dataBase.Deserialize<Shop>(path);
            CollectionAssert.AreEquivalent(_shops, _dataBase.GetTable<Shop>().ToArray());
            File.Delete(path);
        }

        [TestMethod]
        public void AbsentTableTypeExceptionTest() {
            Assert.ThrowsException<DataBaseException>(() => _dataBase.GetTable<FakeEntity>());
        }

        [TestMethod]
        public void InsertionToAbsentTableTest() {
            Assert.ThrowsException<DataBaseException>(() => _dataBase.InsertInto(() => new FakeEntity()));
        }

        [TestMethod]
        public void DoubleTableCreationExceptionTest() {
            Assert.ThrowsException<DataBaseException>(() => _dataBase.GetTable<FakeEntity>());
        }

        private void GenerateData() {
            _dataBase.CreateTable<Shop>();
            var supermarket = new Shop("Seven eleven", "Quahog", "USA");
            var sport = new Shop("Sport master", "Moscow", "Russia");
            var electronics = new Shop("Xiaomi", "Saint Petersburg", "Russia");
            _shops = new []{supermarket, sport, electronics};
            foreach (var shop in _shops) 
                _dataBase.InsertInto(() => shop);
            
            _dataBase.CreateTable<Good>();
            var milk = new Good("Milk", supermarket.Id, "Dairy", 5);
            var cookies = new Good("Cookies", supermarket.Id, "Sweets", 8);
            var tShirt = new Good("tShirt", sport.Id, "Clothes", 30);
            var sneakers = new Good("Sneakers", sport.Id, "Clothes", 70);
            var phone = new Good("Phone", electronics.Id, "Electronics", 150);
            var laptop = new Good("Laptop", electronics.Id, "Electronics", 800);
            _goods = new[] {milk, cookies, tShirt, sneakers, phone, laptop};
            foreach (var good in _goods) {
                _dataBase.InsertInto(() => good);
            }

            _dataBase.CreateTable<Buyer>();
            var peter = new Buyer("Peter", "Griffin", "Quahog", "USA");
            var john = new Buyer("John", "Johnson", "Dakota", "USA");
            var ivan = new Buyer("Ivan", "Ivanov", "Moscow", "Russia");
            var li = new Buyer("Li", "Wong", "Beijing", "China");
            _buyers = new []{peter, john, ivan, li};
            foreach (var buyer in _buyers) {
                _dataBase.InsertInto(() => buyer);
            }

            _dataBase.CreateTable<Sale>();
            var sale0 = new Sale(peter.Id, supermarket.Id, milk.Id, 3);
            var sale1 = new Sale(peter.Id, supermarket.Id, cookies.Id, 5);
            var sale2 = new Sale(peter.Id, electronics.Id, laptop.Id, 1);
            var sale3 = new Sale(john.Id, electronics.Id, phone.Id, 2);
            var sale4 = new Sale(ivan.Id, sport.Id, tShirt.Id, 2);
            var sale5 = new Sale(ivan.Id, sport.Id, sneakers.Id, 1);
            var sale6 = new Sale(ivan.Id, supermarket.Id, milk.Id, 2);
            var sale7 = new Sale(john.Id, supermarket.Id, cookies.Id, 3);
            var sale8 = new Sale(li.Id, electronics.Id, phone.Id, 5);
            var sale9 = new Sale(li.Id, sport.Id, sneakers.Id, 1);
            _sales = new[] {sale0, sale1, sale2, sale3, sale4, sale5, sale6, sale7, sale8, sale9};
            foreach (var sale in _sales) {
                _dataBase.InsertInto(() => sale);
            }
        }
        
        private readonly IDataBase _dataBase;
        private readonly IDataAccessLayer _dataAccessLayer;

        private Shop[] _shops = null!;
        private Good[] _goods = null!;
        private Sale[] _sales = null!;
        private Buyer[] _buyers = null!;
    }
}