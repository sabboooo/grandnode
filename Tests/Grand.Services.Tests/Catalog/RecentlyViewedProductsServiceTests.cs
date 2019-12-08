using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grand.Core.Data;
using Grand.Core.Domain.Catalog;
using Grand.Services.Catalog;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Grand.Services.Tests.Catalog
{
    [TestClass]
    public class RecentlyViewedProductsServiceTests
    {
        private IProductService _productService;
        private CatalogSettings _catalogSettings;
        private IRepository<RecentlyViewedProduct> _recentlyViewedProducts;
        private IRepository<Product> _productRepository;
        private RecentlyViewedProductsService _recentlyViewedProductsService;


        [TestInitialize]
        public void TestInitialize()
        {
            _recentlyViewedProducts = new MongoDBRepositoryTest<RecentlyViewedProduct>();
            _productRepository = new MongoDBRepositoryTest<Product>();

            _catalogSettings = new CatalogSettings();

            _productService = new ProductService(null, _productRepository, null,
                null, null, null, null, 
                null, null, null, null,
                null, null, null, null,
                null, null, null, null, null);

            _recentlyViewedProducts.Insert(
                    new RecentlyViewedProduct()
                    {
                        CustomerId = "1",
                        CreatedOnUtc = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                        ProductId = "1"
                    }
            );

            _recentlyViewedProducts.Insert(
                new RecentlyViewedProduct() {
                    CustomerId = "1",
                    CreatedOnUtc = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                    ProductId = "2"
                }
            );

            _productRepository.Insert(
                new Product() {
                    Id = "2",
                    Published = true,
                }
            );

            _productRepository.Insert(
                new Product() {
                    Id = "1",
                    Published = false,
                }
            );

            _recentlyViewedProductsService = new RecentlyViewedProductsService(_productService, _catalogSettings, _recentlyViewedProducts);
        }


        [TestMethod]
        public async Task GetRecentlyViewedProducts_AllProductsArePublished_ListOfAllRecentlyViewedProducts()
        {
            IQueryable unpublishedProducts = _productRepository.Table.Where(p => p.Published == false);
            foreach (Product unpublishedProduct in unpublishedProducts)
            {
                unpublishedProduct.Published = true;
                _productRepository.Update(unpublishedProduct);
            }

            var products = await _recentlyViewedProductsService.GetRecentlyViewedProducts("1", 2);

            Assert.IsTrue(_recentlyViewedProducts.Table.Count(p => p.CustomerId == "1") == products.Count);

        }
    }
}
