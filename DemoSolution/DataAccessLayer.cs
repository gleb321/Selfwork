using System.Collections.Generic;
using System.Linq;
using NewVariant.Interfaces;
using NewVariant.Models;

namespace DemoSolution {
    public class DataAccessLayer : IDataAccessLayer {
        public IEnumerable<Good> GetAllGoodsOfLongestNameBuyer(IDataBase dataBase) {
            var goods = 
                from good in dataBase.GetTable<Good>()
                let goodIds = (from sale in dataBase.GetTable<Sale>() 
                    let buyerId = dataBase.GetTable<Buyer>().OrderByDescending(buyer => buyer.Name.Length).First().Id
                    where sale.BuyerId == buyerId
                    select sale.GoodId).ToList()
                where goodIds.Contains(good.Id)
                select good;

            return goods;
        }

        public string? GetMostExpensiveGoodCategory(IDataBase dataBase) {
            var category = 
                dataBase.GetTable<Good>()
                    .MaxBy(good => good.Price)
                    ?.Category;

            return category;
        }

        public string? GetMinimumSalesCity(IDataBase dataBase) {
            var city =
                (from sale in dataBase.GetTable<Sale>()
                    join good in dataBase.GetTable<Good>() on sale.GoodId equals good.Id
                    join shop in dataBase.GetTable<Shop>() on sale.ShopId equals shop.Id
                    select new {shop.City, Value = sale.GoodCount * good.Price})
                .GroupBy(payment => payment.City)
                .Select(group => new {City = group.Key, Total = group.Sum(payment => payment.Value)})
                .MinBy(citySales => citySales.Total)?.City;
            
            return city;
        }

        public IEnumerable<Buyer> GetMostPopularGoodBuyers(IDataBase dataBase) {
            var buyers =
                from buyer in dataBase.GetTable<Buyer>()
                let buyerIds = (from sale in dataBase.GetTable<Sale>()
                        group sale by sale.GoodId).OrderByDescending(
                        goodGroup => goodGroup
                            .Select(sl => sl.GoodCount)
                            .Sum())
                    .First()
                    .Select(sale => sale.BuyerId)
                    .ToList()
                where buyerIds.Contains(buyer.Id)
                select buyer;

            return buyers;
        }

        public int GetMinimumNumberOfShopsInCountry(IDataBase dataBase) {
            var counter =
                (from shop in dataBase.GetTable<Shop>()
                    group shop by shop.Country
                    into shopCountry
                    orderby shopCountry.Count()
                    select shopCountry.Count())
                .First();

            return counter;
        }

        public IEnumerable<Sale> GetOtherCitySales(IDataBase dataBase) {
            var sales = 
                from sale in dataBase.GetTable<Sale>() 
                join buyer in dataBase.GetTable<Buyer>() on sale.BuyerId equals buyer.Id
                join shop in dataBase.GetTable<Shop>() on sale.ShopId equals shop.Id
                where buyer.City != shop.City
                select sale;

            return sales;
        }

        public long GetTotalSalesValue(IDataBase dataBase) {
            var total =
                (from sale in dataBase.GetTable<Sale>()
                    join good in dataBase.GetTable<Good>() on sale.GoodId equals good.Id
                    join shop in dataBase.GetTable<Shop>() on sale.ShopId equals shop.Id
                    select sale.GoodCount * good.Price)
                .Sum();
            
            return total;
        }
    }
}