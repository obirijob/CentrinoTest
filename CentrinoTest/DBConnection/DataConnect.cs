using System;
using System.Configuration;
using CentrinoTest.Models;
using MySql.Data.MySqlClient;
using System.Data;

namespace CentrinoTest.DBConnection
{
	public class DataConnect
	{
        private static string dsource => "server=localhost;port=3306;database=pizza;user=root;password=401269014";

        public static DataTable GetOrders()
        {
            string q = "select * from orders order by id desc";

            DataTable dt = new DataTable();
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

        public static DataTable getOrderItems(int id)
        {
            string q = $"select pizzas.id, pizzaorders.orderid, pizzas.price, pizzas.size, pizzaorders.pizzaid from pizzas, pizzaorders where pizzaorders.pizzaid = pizzas.id and pizzaorders.orderid = '{id}'";

            DataTable dt = new DataTable();
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

        public static DetailedOrder getDetailedOrder(int id)
        {
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
                    foreach (DataRow r in getOrderItems(id).Rows)
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

        public static DataTable getToppings()
        {
            string q = "select * from toppings";

            DataTable dt = new DataTable();
            //string dsource = _configuration.GetConnectionString("pizzaDB");
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

        public static DataTable getPizzas()
        {
            string q = "select * from pizzas";


            DataTable dt = new DataTable();
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

        public static DataTable newOrder(Order order)
        {
            string q = "insert into orders(orderfor) values(@orderfor)";

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
            return dt;
        }

        public static DataTable newOrderPizza(PizzaOrder order)
        {
            string q = "insert into pizzaorders(pizzaid, orderid) values(@pizzaid, @orderid)";

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
            return dt;
        }

        public static DataTable newOrderPizzaTopping(PizzaOrderTopping order)
        {
            string q = "insert into pizzaordertoppings(toppingid, pizzaorderid) values(@toppingid, @pizzaorderid)";

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
            return dt;
        }

        public static DOPizza getPizzaDetail(int id)
        {
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

        public static DataTable getToppings(int id)
        {
            string q = @$"
                            SELECT toppings.id, toppings.label, toppings.small, toppings.medium, toppings.large 
                            from toppings, pizzaordertoppings 
                            where pizzaordertoppings.toppingid = toppings.id and pizzaordertoppings.pizzaorderid = '{id}'
                        ";

            DataTable dt = new DataTable();
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

        public static DOPizzaTopping getPizzaTopping(int id, string size)
        {
            Console.WriteLine(id + ", " + size);
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
    }
}

