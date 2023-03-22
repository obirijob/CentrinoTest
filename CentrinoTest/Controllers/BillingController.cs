using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using MySql.Data.MySqlClient;
using CentrinoTest.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CentrinoTest.Controllers
{
    [Route("api")]
    [ApiController]
    public class BillingController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public BillingController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("pizzas")]
        public JsonResult GetPizzas()
        {
            string q = "select * from pizzas";


            DataTable dt = new DataTable();
            string dsource = _configuration.GetConnectionString("pizzaDB");
            MySqlDataReader rdr;
            using (MySqlConnection con = new MySqlConnection(dsource))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(q, con))
                {
                    rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                    rdr.Close();
                    con.Close();
                }
            }

            return new JsonResult(dt);
        }

        [HttpGet("toppings")]
        public JsonResult GetToppings()
        {
            string q = "select * from toppings";

            DataTable dt = new DataTable();
            string dsource = _configuration.GetConnectionString("pizzaDB");
            MySqlDataReader rdr;
            using (MySqlConnection con = new MySqlConnection(dsource))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(q, con))
                {
                    rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                    rdr.Close();
                    con.Close();
                }
            }

            return new JsonResult(dt);
        }

        [HttpGet("orders")]
        public JsonResult GetOrdersFull()
        {
            List<DetailedOrder> dorder = new List<DetailedOrder>();
            foreach(DataRow r in GetOrders().Rows)
            {
                dorder.Add(getDetailedOrder(Int16.Parse(r["id"].ToString())));
            }
            return new JsonResult(dorder);
        }

        public DataTable GetOrders()
        {
            string q = "select * from orders order by id desc";

            DataTable dt = new DataTable();
            string dsource = _configuration.GetConnectionString("pizzaDB");
            MySqlDataReader rdr;
            using (MySqlConnection con = new MySqlConnection(dsource))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(q, con))
                {
                    rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                    rdr.Close();
                    con.Close();
                }
            }

            return dt;
        }

        [HttpGet("orders/{id}")]
        public JsonResult GetOrderItems([FromRoute] int id)
        {
            DataTable dt = getOrderItems(id);
            return new JsonResult(dt);
        }

        public DataTable getOrderItems(int id)
        {
            string q = $"select pizzas.id, pizzaorders.orderid, pizzas.price, pizzas.size, pizzaorders.pizzaid from pizzas, pizzaorders where pizzaorders.pizzaid = pizzas.id and pizzaorders.orderid = '{id}'";

            DataTable dt = new DataTable();
            string dsource = _configuration.GetConnectionString("pizzaDB");
            MySqlDataReader rdr;
            using (MySqlConnection con = new MySqlConnection(dsource))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(q, con))
                {
                    rdr = cmd.ExecuteReader();
                    dt.Load(rdr);

                    rdr.Close();
                    con.Close();
                }
            }

            return dt;
        }

        public DetailedOrder getDetailedOrder(int id)
        {
            string dsource = _configuration.GetConnectionString("pizzaDB");
            DetailedOrder detailedOrder = new DetailedOrder();

            MySqlDataReader rdr;
            using (MySqlConnection con = new MySqlConnection(dsource))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand($"select * from orders where id = '{id}' limit 1", con))
                {
                    rdr = cmd.ExecuteReader();
                    rdr.Read();
                    detailedOrder.id = id;
                    detailedOrder.orderfor = rdr.GetString("orderfor");
                    List<DOPizza> dOPizzas = new List<DOPizza>();
                    foreach(DataRow r in getOrderItems(id).Rows)
                    {
                        DOPizza dop = new DOPizza();
                        dop.size = r["size"].ToString();
                        dop.price = Double.Parse(r["price"].ToString());
                        List<DOPizzaTopping> dOPizzaToppings = new List<DOPizzaTopping>();
                        foreach (DataRow rr in getToppings(Int16.Parse(r["pizzaid"].ToString())).Rows)
                        {
                            Console.WriteLine(rr["label"].ToString() + " is the label");
                            DOPizzaTopping topping = getPizzaTopping(Int16.Parse(rr["id"].ToString()), dop.size);
                            dOPizzaToppings.Add(topping);
                        }
                        dop.toppings = dOPizzaToppings;
                        dOPizzas.Add(dop);
                    }
                    detailedOrder.pizzas = dOPizzas;
                    rdr.Close();
                }
                con.Close();
            }

            return detailedOrder;
        }



        public DOPizza getPizzaDetail(int id)
        {
            string dsource = _configuration.GetConnectionString("pizzaDB");
            DOPizza dopizza = new DOPizza();
            // get order details
            MySqlDataReader rdr;
            using (MySqlConnection con = new MySqlConnection(dsource))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand($"select * from pizzas where id = '{id}' limit 1", con))
                {        
                    rdr = cmd.ExecuteReader();
                    rdr.Read();
                    dopizza.size = rdr.GetString("size");
                    dopizza.price = rdr.GetDouble("price");
                    
                    rdr.Close();
                }
                con.Close();
            }

            return dopizza;
        }

        [HttpGet("ordersfull/{id}")]
        public JsonResult FullDetails([FromRoute] int id)
        {
            DetailedOrder order = getDetailedOrder(id);
            return new JsonResult(order);

        }

        [HttpGet("orderitems/{id}")]
        public JsonResult GetOrderItemToppings([FromRoute] int id)
        {
            DataTable dt = getToppings(id);
            return new JsonResult(dt);
        }

        public DataTable getToppings(int id)
        {
            string q = @$"
                            SELECT toppings.id, toppings.label, toppings.small, toppings.medium, toppings.large 
                            from toppings, pizzaordertoppings 
                            where pizzaordertoppings.toppingid = toppings.id and pizzaordertoppings.pizzaorderid = '{id}'
                        ";

            DataTable dt = new DataTable();
            string dsource = _configuration.GetConnectionString("pizzaDB");
            MySqlDataReader rdr;
            using (MySqlConnection con = new MySqlConnection(dsource))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(q, con))
                {
                    rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                    rdr.Close();
                    con.Close();
                }
            }

            return dt;
        }

        public DOPizzaTopping getPizzaTopping(int id, string size)
        {
            Console.WriteLine(id + ", " + size);
            string dsource = _configuration.GetConnectionString("pizzaDB");
            DOPizzaTopping dopizza = new DOPizzaTopping();
            // get order details
            MySqlDataReader rdr;
            using (MySqlConnection con = new MySqlConnection(dsource))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand($"select * from toppings where id = '{id}' limit 1", con))
                {
                    rdr = cmd.ExecuteReader();
                    rdr.Read();
                    dopizza.id = rdr.GetInt16("id");
                    dopizza.label = rdr.GetString("label");
                    dopizza.price = rdr.GetDouble(size.ToLower());
                    rdr.Close();
                }
                con.Close();
            }

            return dopizza;
        }

        [HttpPost("order")]
        public JsonResult NewOrder(Order order)
        {
            string q = "insert into orders(orderfor) values(@orderfor)";

            string dsource = _configuration.GetConnectionString("pizzaDB");

            DataTable dt = new DataTable();
            MySqlDataReader rdr;
            using (MySqlConnection con = new MySqlConnection(dsource))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(q, con))
                {
                    cmd.Parameters.AddWithValue("@orderfor", order.orderfor);
                    rdr = cmd.ExecuteReader();
                    rdr.Close();
                    using (MySqlCommand cmd2 = new MySqlCommand("select * from orders order by id desc limit 1", con))
                    {

                        rdr = cmd2.ExecuteReader();
                        dt.Load(rdr);
                        con.Close();
                    }
                    
                }
            }

            return new JsonResult(dt);
        }

        [HttpPost("orderitem")]
        public JsonResult NewOrderPizza(PizzaOrder order)
        {
            string q = "insert into pizzaorders(pizzaid, orderid) values(@pizzaid, @orderid)";

            string dsource = _configuration.GetConnectionString("pizzaDB");

            DataTable dt = new DataTable();
            MySqlDataReader rdr;
            using (MySqlConnection con = new MySqlConnection(dsource))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(q, con))
                {
                    cmd.Parameters.AddWithValue("@pizzaid", order.pizzaid);
                    cmd.Parameters.AddWithValue("@orderid", order.orderid);

                    rdr = cmd.ExecuteReader();
                    rdr.Close();
                    using (MySqlCommand cmd2 = new MySqlCommand("select * from pizzaorders order by id desc limit 1", con))
                    {
                        rdr = cmd2.ExecuteReader();
                        dt.Load(rdr);
                        con.Close();
                    }

                }
            }

            return new JsonResult(dt);
        }

        [HttpPost("orderitemtopping")]
        public JsonResult NewOrderPizzaTopping(PizzaOrderTopping order)
        {
            string q = "insert into pizzaordertoppings(toppingid, pizzaorderid) values(@toppingid, @pizzaorderid)";

            string dsource = _configuration.GetConnectionString("pizzaDB");

            DataTable dt = new DataTable();
            MySqlDataReader rdr;
            using (MySqlConnection con = new MySqlConnection(dsource))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(q, con))
                {
                    cmd.Parameters.AddWithValue("@toppingid", order.toppingid);
                    cmd.Parameters.AddWithValue("@pizzaorderid", order.pizzaorderid);

                    rdr = cmd.ExecuteReader();
                    rdr.Close();
                    using (MySqlCommand cmd2 = new MySqlCommand("select * from pizzaordertoppings order by id desc limit 1", con))
                    {
                        rdr = cmd2.ExecuteReader();
                        dt.Load(rdr);
                        con.Close();
                    }

                }
            }

            return new JsonResult(dt);
        }

    }
}

