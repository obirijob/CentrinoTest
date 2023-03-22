using System;
namespace CentrinoTest.Models
{
	public class DOPizza
	{
		public string size { get; set; }
		public double price { get; set; }
		public string priceF => this.price.ToString("C");
		public double totalPrice => this.price + getTotalPrice(this.toppings);
		public string totalPriceF => this.totalPrice.ToString("C");
        public List<DOPizzaTopping> toppings { get; set; }

        private static double getTotalPrice(List<DOPizzaTopping> tps)
		{
			double cost = 0;

			foreach (DOPizzaTopping t in tps) {
				cost += t.price;
			}

			return cost;
		}

    }

	public class DOPizzaTopping
	{
		public int id { get; set; }
		public string label { get; set; }
		public double price { get; set; }
		public string priceF => this.price.ToString("C");

		
	}

	public class DetailedOrder
	{
		public int id { get; set; }
		public string orderfor { get; set; }
		public double pizzaCost => getPizzaCost(this.pizzas);
		public string pizzaCostF => this.pizzaCost.ToString("C");
        public double toppingsCost => getToppingsCost(this.pizzas);
		public string toppingsCostF => this.toppingsCost.ToString("C");
		public double subtotalCost => this.pizzaCost + this.toppingsCost;
		public string subtotalCostF => this.subtotalCost.ToString("C");
		public double vat => this.subtotalCost * 0.16;
		public string vatF => this.vat.ToString("C");
		public double totalCost => this.subtotalCost + vat;
		public string totalCostF => this.totalCost.ToString("C");
		public List<DOPizza> pizzas { get; set; }

		private static double getPizzaCost(List<DOPizza> pizzas)
		{
			double cost = 0;
			foreach (DOPizza p in pizzas)
			{
				cost += p.price;
			}
			return cost;
		}

		private static double getToppingsCost(List<DOPizza> pizzas)
		{
			double cost = 0;

			foreach(DOPizza p in pizzas)
			{
				foreach(DOPizzaTopping t in p.toppings)
				{
					cost += t.price;
				}
			}
			return cost;
		}
	}
}

