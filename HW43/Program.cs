using System;
using System.Collections.Generic;

namespace HW43
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int buyersCount = 10;

            Supermarket supermarket = new Supermarket();
            supermarket.FillBuyers(buyersCount);

            supermarket.Work();
        }
    }

    class Utils
    {
        private static Random s_random = new Random();

        public static int GenerateRandomNumber(int minValue, int maxValue)
        {
            return s_random.Next(minValue, maxValue + 1);
        }

        public static int GenerateRandomIndex(int minValue, int maxValue)
        {
            return s_random.Next(minValue, maxValue);
        }

        public static void GetColoredText(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    class Supermarket
    {
        private int _money = 0;
        private Queue<Buyer> _buyers = new Queue<Buyer>();
        private List<Product> _productsForSale = new List<Product>();

        public Supermarket()
        {
            _productsForSale.Add(new Product("Хлеб", 280));
            _productsForSale.Add(new Product("Молоко", 390));
            _productsForSale.Add(new Product("Сыр", 500));
            _productsForSale.Add(new Product("Колбаса", 1600));
        }

        public void FillBuyers(int countBuyers)
        {
            int minMoneyValue = 100;
            int maxMoneyValue = 4000;

            int minProductValue = 1;
            int maxProductValue = 6;

            for (int i = 0; i < countBuyers; i++)
            {
                Buyer buyer = new Buyer(Utils.GenerateRandomNumber(minMoneyValue, maxMoneyValue));

                int productCount = Utils.GenerateRandomNumber(minProductValue, maxProductValue);

                for (int j = 0; j < productCount; j++)
                {
                    int index = Utils.GenerateRandomIndex(0, _productsForSale.Count);

                    buyer.AddToBasket(_productsForSale[index]);
                }

                _buyers.Enqueue(buyer);
            }
        }

        public void Work()
        {
            while (_buyers.Count > 0)
            {
                Buyer buyer = _buyers.Dequeue();

                Console.Write($"Клиентов в очереди: {_buyers.Count}.\n\n");

                Console.WriteLine($"Список покупок клиента: ");
                buyer.ShowBasket();
                Console.WriteLine();

                buyer.RemoveExcess();

                Trade(buyer);

                Console.WriteLine("Клиент ушел. Нажмите любую клавишу...");
                Console.ReadKey();
                Console.Clear();
            }

            Console.WriteLine($"\nРабота завершена. Заработано денег: {_money}");
        }

        private void Trade(Buyer buyer)
        {
            int basketTotal = buyer.GetBasketTotal();

            if (buyer.CanPay(buyer.GetBasketTotal()) && buyer.BasketCount > 0)
            {
                buyer.Pay(basketTotal);
                buyer.MoveBasketToBag();

                _money += basketTotal;

                Utils.GetColoredText("Покупка прошла успешно.\n\n==========||==========\n", ConsoleColor.Green);

                Console.WriteLine("Клиент купил:");
                buyer.ShowBag();
                Console.WriteLine($"Сумма покупки: {basketTotal}, осталось денег: {buyer.Money}");

            }
            else
            {
                Utils.GetColoredText("У клиента недостаточно средств даже на один товар :(" , ConsoleColor.Red);
            }
        }
    }

    class Buyer
    {
        private List<Product> _basket = new List<Product>();
        private List<Product> _bag = new List<Product>();

        public Buyer(int money)
        {
            Money = money;
        }

        public int Money { get; private set; }
        public int BasketCount => _basket.Count;
      

        public void RemoveExcess()
        {
            int total = GetBasketTotal();

            while (total > Money)
            {
                Product removedProduct = RemoveRandomFromBasket();

                Console.WriteLine($"Недостаточно денег. Клиент убрал товар из корзины: {removedProduct.Name}");

                total = GetBasketTotal();
            }
        }

        public void AddToBasket(Product product)
        {
            _basket.Add(product);
        }

        public void ShowBasket()
        {
            for (int i = 0; i < _basket.Count; i++)
                Console.WriteLine($"{i + 1}). {_basket[i].Show()}");
        }

        public void ShowBag()
        {
            for (int i = 0; i < _bag.Count; i++)
                Console.WriteLine($"{i + 1}). {_bag[i].Show()}");
        }

        public int GetBasketTotal()
        {
            int total = 0;

            for (int i = 0; i < _basket.Count; i++)
                total += _basket[i].Price;

            return total;
        }


        public Product RemoveRandomFromBasket()
        {
            int index = Utils.GenerateRandomIndex(0, _basket.Count);

            Product product = _basket[index];

            _basket.RemoveAt(index);
            return product;
        }

        public bool CanPay(int totalSum)
        {
            return totalSum <= Money;
        }

        public void Pay(int totalSum)
        {
            Money -= totalSum;
        }

        public void MoveBasketToBag()
        {
            for (int i = 0; i < _basket.Count; i++)
                _bag.Add(_basket[i]);

            _basket.Clear();
        }
    }

    class Product
    {
        public Product(string name, int price)
        {
            Name = name;
            Price = price;
        }

        public string Name { get; private set; }
        public int Price { get; private set; }

        public string Show()
        {
            return $"Товар {Name} по цене {Price} тенге.";
        }
    }
}
