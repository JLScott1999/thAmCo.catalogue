namespace ThAmCo.Catalogue.ControllerTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using ThAmCo.Catalogue.Controllers;
    using ThAmCo.Catalogue.Models;
    using ThAmCo.Catalogue.Services.Order;
    using ThAmCo.Catalogue.Services.Product;
    using ThAmCo.Catalogue.Services.Review;
    using ThAmCo.Catalogue.Services.StockManagement;
    using ThAmCo.Catalogue.ViewModels;
    using Xunit;

    public class CatalogueControllerTests
    {

        [Fact]
        public void Products_ReturnValue()
        {
            IProductService productService = new FakeProductService(new List<ProductModel>()
            {
                new ProductModel()
                {
                    Id = Guid.Parse("14D486C4-CEE6-4C26-B274-CC0E300B0B99"),
                    Name = "Test",
                    Description = "TestDesc"
                },
                new ProductModel()
                {
                    Id = Guid.Parse("12E74E96-F987-4B1D-9870-74C84A0A8965"),
                    Name = "TestTwo",
                    Description = "TestTwoDesc"
                }
            });
            IStockManagementService stockService = new FakeStockManagementService(new List<ProductStockModel>()
            {
                new ProductStockModel()
                {
                    Id = Guid.Parse("14D486C4-CEE6-4C26-B274-CC0E300B0B99"),
                    Stock = 10
                },
                new ProductStockModel()
                {
                    Id = Guid.Parse("12E74E96-F987-4B1D-9870-74C84A0A8965"),
                    Stock = 0
                }
            });
            CatalogueController controller = new CatalogueController(productService, stockService, null, null);
            IActionResult result = controller.Products();

            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            IEnumerable<ProductViewModel> model = Assert.IsAssignableFrom<IEnumerable<ProductViewModel>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());

            Assert.Equal(Guid.Parse("14D486C4-CEE6-4C26-B274-CC0E300B0B99"), model.ElementAt(0).Id);
            Assert.Equal("Test", model.ElementAt(0).Name);
            Assert.Equal("TestDesc", model.ElementAt(0).Description);
            Assert.Equal(10, model.ElementAt(0).Stock);
            Assert.Equal("In Stock", model.ElementAt(0).StockStatus);

            Assert.Equal(Guid.Parse("12E74E96-F987-4B1D-9870-74C84A0A8965"), model.ElementAt(1).Id);
            Assert.Equal("TestTwo", model.ElementAt(1).Name);
            Assert.Equal("TestTwoDesc", model.ElementAt(1).Description);
            Assert.Equal(0, model.ElementAt(1).Stock);
            Assert.Equal("Out of Stock", model.ElementAt(1).StockStatus);
        }

        [Fact]
        public void Products_ProductsServiceException()
        {
            Mock<IProductService> productService = new Mock<IProductService>();
            productService.Setup(x => x.GetProducts()).Throws(new Exception());
            IStockManagementService stockService = new FakeStockManagementService(new List<ProductStockModel>()
            {
                new ProductStockModel()
                {
                    Id = Guid.Parse("14D486C4-CEE6-4C26-B274-CC0E300B0B99"),
                    Stock = 10
                },
                new ProductStockModel()
                {
                    Id = Guid.Parse("12E74E96-F987-4B1D-9870-74C84A0A8965"),
                    Stock = 0
                }
            });
            CatalogueController controller = new CatalogueController(productService.Object, stockService, null, null);
            IActionResult result = controller.Products();

            StatusCodeResult viewResult = Assert.IsType<StatusCodeResult>(result);
        }

        [Fact]
        public void Products_StockManagementServiceException()
        {
            IProductService productService = new FakeProductService(new List<ProductModel>()
            {
                new ProductModel()
                {
                    Id = Guid.Parse("14D486C4-CEE6-4C26-B274-CC0E300B0B99"),
                    Name = "Test",
                    Description = "TestDesc"
                },
                new ProductModel()
                {
                    Id = Guid.Parse("12E74E96-F987-4B1D-9870-74C84A0A8965"),
                    Name = "TestTwo",
                    Description = "TestTwoDesc"
                }
            });
            Mock<IStockManagementService> stockService = new Mock<IStockManagementService>();
            stockService.Setup(x => x.GetProductsStock()).Throws(new Exception());
            CatalogueController controller = new CatalogueController(productService, stockService.Object, null, null);
            IActionResult result = controller.Products();
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            IEnumerable<ProductViewModel> model = Assert.IsAssignableFrom<IEnumerable<ProductViewModel>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());

            Assert.Equal(Guid.Parse("14D486C4-CEE6-4C26-B274-CC0E300B0B99"), model.ElementAt(0).Id);
            Assert.Equal("Test", model.ElementAt(0).Name);
            Assert.Equal("TestDesc", model.ElementAt(0).Description);
            Assert.Null(model.ElementAt(0).Stock);
            Assert.Equal("Unknown", model.ElementAt(0).StockStatus);

            Assert.Equal(Guid.Parse("12E74E96-F987-4B1D-9870-74C84A0A8965"), model.ElementAt(1).Id);
            Assert.Equal("TestTwo", model.ElementAt(1).Name);
            Assert.Equal("TestTwoDesc", model.ElementAt(1).Description);
            Assert.Null(model.ElementAt(1).Stock);
            Assert.Equal("Unknown", model.ElementAt(0).StockStatus);
        }

        [Fact]
        public void Product_ReturnValue()
        {
            IProductService productService = new FakeProductService(new List<ProductModel>()
            {
                new ProductModel()
                {
                    Id = Guid.Parse("14D486C4-CEE6-4C26-B274-CC0E300B0B99"),
                    Name = "Test",
                    Description = "TestDesc"
                }
            });
            IStockManagementService stockService = new FakeStockManagementService(new List<ProductStockModel>()
            {
                new ProductStockModel()
                {
                    Id = Guid.Parse("14D486C4-CEE6-4C26-B274-CC0E300B0B99"),
                    Stock = 10
                }
            });
            IReviewService reviewService = new FakeReviewService(new List<ProductReviewModel>()
            {
                new ProductReviewModel()
                {
                    ProductId = Guid.Parse("14D486C4-CEE6-4C26-B274-CC0E300B0B99"),
                    Date = DateTime.UtcNow,
                    Name = "Test",
                    Description = "Test Review"
                }
            });
            IOrderService orderService = new FakeOrderService(new List<ProductOrderModel>()
            {
                new ProductOrderModel()
                {
                    ProductId = Guid.Parse("14D486C4-CEE6-4C26-B274-CC0E300B0B99"),
                    DateTime = DateTime.Parse("2020-12-12 12:12")
                }
            });
            CatalogueController controller = new CatalogueController(productService, stockService, reviewService, orderService);
            IActionResult result = controller.Product(Guid.Parse("14D486C4-CEE6-4C26-B274-CC0E300B0B99"));

            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            ProductViewModel model = Assert.IsAssignableFrom<ProductViewModel>(viewResult.ViewData.Model);
            Assert.Equal("Test", model.Name);
            Assert.Equal("TestDesc", model.Description);
            Assert.Equal(10, model.Stock);
            Assert.Equal("In Stock", model.StockStatus);
            Assert.Single(model.Reviews);
            Assert.Equal(DateTime.Parse("2020-12-12 12:12"), model.LastOrdered);
        }

        [Fact]
        public void Product_ReturnValue_NotFound()
        {
            IProductService productService = new FakeProductService(new List<ProductModel>()
            {
                new ProductModel()
                {
                    Id = Guid.Parse("14D486C4-CEE6-4C26-B274-CC0E300B0B99"),
                    Name = "Test",
                    Description = "TestDesc"
                }
            });
            IStockManagementService stockService = new FakeStockManagementService(new List<ProductStockModel>()
            {
                new ProductStockModel()
                {
                    Id = Guid.Parse("14D486C4-CEE6-4C26-B274-CC0E300B0B99"),
                    Stock = 10
                }
            });
            IReviewService reviewService = new FakeReviewService(new List<ProductReviewModel>()
            {
                new ProductReviewModel()
                {
                    ProductId = Guid.Parse("14D486C4-CEE6-4C26-B274-CC0E300B0B99"),
                    Date = DateTime.UtcNow,
                    Name = "Test",
                    Description = "Test Review"
                }
            });
            IOrderService orderService = new FakeOrderService(new List<ProductOrderModel>()
            {
                new ProductOrderModel()
                {
                    ProductId = Guid.Parse("14D486C4-CEE6-4C26-B274-CC0E300B0B99"),
                    DateTime = DateTime.Parse("2020-12-12 12:12")
                }
            });
            CatalogueController controller = new CatalogueController(productService, stockService, reviewService, orderService);
            IActionResult result = controller.Product(Guid.Parse("12E74E96-F987-4B1D-9870-74C84A0A8965"));

            RedirectToActionResult viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Products", viewResult.ActionName);
        }

    }
}
