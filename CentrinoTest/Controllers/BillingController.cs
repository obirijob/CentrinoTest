using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using MySql.Data.MySqlClient;
using CentrinoTest.Models;
using CentrinoTest.DBConnection;
using Microsoft.Extensions.Configuration;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CentrinoTest.Controllers
{
    [Route("api")]
    [ApiController]
    public class BillingController : ControllerBase
    { 

        [HttpGet("pizzas")]
        public JsonResult GetPizzas()
        {
            DataTable dt = DataConnect.getPizzas();

            return new JsonResult(dt);
        }

        [HttpGet("toppings")]
        public JsonResult GetToppings()
        {
            DataTable dt = DataConnect.getToppings();
            return new JsonResult(dt);
        }

        [HttpGet("orders")]
        public JsonResult GetOrdersFull()
        {
            List<DetailedOrder> dorder = new List<DetailedOrder>();
            foreach(DataRow r in DataConnect.GetOrders().Rows)
            {
                dorder.Add(DataConnect.getDetailedOrder(Int16.Parse(r["id"].ToString())));
            }
            return new JsonResult(dorder);
        }

        

        [HttpGet("orders/{id}")]
        public JsonResult GetOrderItems([FromRoute] int id)
        {
            DataTable dt = DataConnect.getOrderItems(id);
            return new JsonResult(dt);
        }

        

        [HttpGet("ordersfull/{id}")]
        public JsonResult FullDetails([FromRoute] int id)
        {
            DetailedOrder order = DataConnect.getDetailedOrder(id);
            return new JsonResult(order);

        }

        [HttpGet("orderitems/{id}")]
        public JsonResult GetOrderItemToppings([FromRoute] int id)
        {
            DataTable dt = DataConnect.getToppings(id);
            return new JsonResult(dt);
        }

        

        [HttpPost("order")]
        public JsonResult NewOrder(Order order)
        {
            DataTable dt = DataConnect.newOrder(order);

            return new JsonResult(dt);
        }

        [HttpPost("orderitem")]
        public JsonResult NewOrderPizza(PizzaOrder order)
        {
            DataTable dt = DataConnect.newOrderPizza(order);

            return new JsonResult(dt);
        }

        [HttpPost("orderitemtopping")]
        public JsonResult NewOrderPizzaTopping(PizzaOrderTopping order)
        {
            DataTable dt = DataConnect.newOrderPizzaTopping(order);

            return new JsonResult(dt);
        }

    }
}

