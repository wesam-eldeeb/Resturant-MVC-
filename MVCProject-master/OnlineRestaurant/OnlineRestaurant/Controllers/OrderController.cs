using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineRestaurant.Models;
using OnlineRestaurant.Repository;
using OnlineRestaurant.ViewModels;
using TequliasRestaurant.Models;


namespace OnlineRestaurant.Controllers
{
    public class OrderController : Controller
    {
        Generic_Repository<Order> order_repo;
        Generic_Repository<Product> product_repo;
        private UserManager<ApplicationUser> UserManager;

        public OrderController(Generic_Repository<Order> orderrepo, Generic_Repository<Product> productrepo, UserManager<ApplicationUser> UserManager)


        {
            this.order_repo = orderrepo;
            this.product_repo = productrepo;
            this.UserManager = UserManager;

        }
        [HttpPost]

        public IActionResult AddOrderItem(int ProductId, int Quantity)
        {
            Product product = product_repo.GetById(ProductId);
            if (product == null)
            {
                return NotFound();
            }


            var model = HttpContext.Session.Get<OrderVM>("OrderVM") ?? new OrderVM()
            {
                products = product_repo.GetAll(),
                orderItemVMs = new List<OrderItemVM>()
            };



            OrderItemVM orderItemVM = model.orderItemVMs.FirstOrDefault(orderVM => orderVM.ProductId == ProductId);

            if (orderItemVM != null)
            {
                orderItemVM.Quantity += Quantity;
            }
            else
            {
                model.orderItemVMs.Add(
                    new OrderItemVM()
                    {
                        ProductId = ProductId,
                        Quantity = Quantity,
                        Price = product.Price,
                        ProductName = product.Name
                    });
            }
            model.TotalAmount = model.orderItemVMs.Sum(s => s.Price * s.Quantity);
            HttpContext.Session.Set("OrderVM", model);

            return RedirectToAction("CreateOrder", model);
        }
        public IActionResult TestSession()
        {
            var sessionw = HttpContext.Session.Get<OrderVM>("OrderVM");
            if (sessionw != null)
            {
                return Content($"Session is{sessionw.orderItemVMs.Count()} ");
            }
            return Content("Not Found");
        }
        [Authorize]
        //[HttpPost]

        public async Task<IActionResult> CreateOrder()
        {
            var orderVM = HttpContext.Session.Get<OrderVM>("OrderVM") ?? new OrderVM()
            {
                products = product_repo.GetAll(),
                orderItemVMs = new List<OrderItemVM>()
            };

            return View(orderVM);
        }
        //[HttpPost]

        public IActionResult Cart()
        {
            var model = HttpContext.Session.Get<OrderVM>("OrderVM");
            if (model != null)
            {
                return View(model);
            }
            return RedirectToAction("CreateOrder");
        }
        public IActionResult PlaceOrder()
        {
            var model = HttpContext.Session.Get<OrderVM>("OrderVM");
            if (model != null ||model.orderItemVMs.Count()!=0)
            {
                Order order = new Order(){
                    UserId = UserManager.GetUserId(User),
                    Date = DateTime.Now,
                    TotalAmount = model.TotalAmount,
                    OrderItems=new List<OrderItem>()
                    
                };

                foreach (var item in model.orderItemVMs)
                {
                    order.OrderItems.Add(new OrderItem()
                    {
                        ProductId = item.ProductId,
                        Price = item.Price,
                        Quantity = item.Quantity,

                    });
                }

               
                order_repo.Add(order);
                return View(order);
            }
            return RedirectToAction("CreateOrder");
        }
    }
}
