using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Курсовая_работа_Музыкальный_магазин_с_
{
    public class Order
    {
        private const int MAX_ORDER_ITEMS = 100;
        private string numberOrder;
        private string clientOrder;
        private Product[] orderItems;
        private int[] quantities;
        private int itemCount;
        private int countOrder;
        private double costOrder;
        private string statusOrder;
        private string payWay;
        private string shopOrder;
        private string dateOrder;

        // Конструкторы
        public Order()
        {
            orderItems = new Product[MAX_ORDER_ITEMS];
            quantities = new int[MAX_ORDER_ITEMS];
            itemCount = 0;
            countOrder = 0;
            costOrder = 0.0;

            numberOrder = GenerateOrderNumber();
            statusOrder = "Ожидает оплаты";
            dateOrder = GetCurrentDate();
            shopOrder = "Музыкальный магазин 'MusicLoverzz'";
            payWay = "Не выбран";
        }

        public Order(string clientId, Basket basket) : this()
        {
            clientOrder = clientId;
            CopyProductsFromBasket(basket);
        }

        // Деструктор
        ~Order()
        {
            ClearOrderItems();
        }

        public void ChangeStatus(string newStatus)
        {
            statusOrder = newStatus;
            if (newStatus == "Оплачен")
            {
                Console.WriteLine("\n=== ВАШ ЗАКАЗ УСПЕШНО ОФОРМЛЕН! ===");
                Console.WriteLine("Номер заказа: " + numberOrder);
                Console.WriteLine("Статус: " + statusOrder);
                Console.WriteLine("==================================");
            }
            else
            {
                Console.WriteLine("Статус заказа изменен на: " + statusOrder);
            }
        }

        public bool Payment()
        {
            Console.WriteLine("\n=== ОПЛАТА ЗАКАЗА ===");
            Console.WriteLine("Сумма к оплате: " + costOrder.ToString("F2") + " руб.");

            int paymentChoice = 0; // явная инициализация
            bool validChoice = false;

            while (!validChoice)
            {
                Console.WriteLine("Выберите способ оплаты:");
                Console.WriteLine("1. Банковская карта");
                Console.WriteLine("2. Электронный кошелек");
                Console.WriteLine("3. Наложенный платеж");
                Console.Write("Ваш выбор: ");

                string input = Console.ReadLine();

                if (int.TryParse(input, out paymentChoice))
                {
                    if (paymentChoice >= 1 && paymentChoice <= 3)
                    {
                        validChoice = true;
                    }
                    else
                    {
                        Console.WriteLine("Неверный выбор! Введите 1, 2 или 3.");
                    }
                }
                else
                {
                    Console.WriteLine("Ошибка! Введите число от 1 до 3.");
                }
            }


            if (paymentChoice == 1)
            {
                payWay = "Банковская карта";
            }
            else if (paymentChoice == 2)
            {
                payWay = "Электронный кошелек";
            }
            else 
            {
                payWay = "Наложенный платеж";
            }

            Console.WriteLine("Обработка платежа...");
            Console.WriteLine("Способ оплаты: " + payWay);
            Console.WriteLine("Оплата прошла успешно!");
            return true;
        }

        public void InfoOrder()
        {
            Console.WriteLine("\n=== ИНФОРМАЦИЯ О ЗАКАЗЕ ===");
            Console.WriteLine("Номер заказа: " + numberOrder);
            Console.WriteLine("Клиент: " + clientOrder);
            Console.WriteLine("Дата заказа: " + dateOrder);
            Console.WriteLine("Статус: " + statusOrder);
            Console.WriteLine("Способ оплаты: " + payWay);
            Console.WriteLine("Количество товаров: " + countOrder);
            Console.WriteLine("Общая стоимость: " + costOrder.ToString("F2") + " руб.");
            Console.WriteLine("Магазин: " + shopOrder);

            if (itemCount > 0)
            {
                Console.WriteLine("\nСостав заказа:");
                Console.WriteLine("------------------------------------");
                for (int i = 0; i < itemCount; i++)
                {
                    if (orderItems[i] != null)
                    {
                        Console.WriteLine($"{i + 1}. {orderItems[i].GetName()}" +
                                        $" - {quantities[i]} шт. x " +
                                        $"{orderItems[i].GetPrice():F2} руб. = " +
                                        $"{(quantities[i] * orderItems[i].GetPrice()):F2} руб.");
                    }
                }
                Console.WriteLine("------------------------------------");
            }
            Console.WriteLine("==================================");
        }

        public static bool MakeOrder(string clientId, Basket basket)
        {
            if (basket.IsEmpty())
            {
                Console.WriteLine("Корзина пуста! Добавьте товары перед оформлением заказа.");
                return false;
            }

            Console.WriteLine("\n=== ПОДТВЕРЖДЕНИЕ ЗАКАЗА ===");
            basket.DisplayBasket();
            Console.Write("\nВы точно хотите оформить заказ? (y/n): ");
            char confirmation = Console.ReadKey().KeyChar;
            Console.WriteLine();

            if (confirmation != 'y' && confirmation != 'Y')
            {
                Console.WriteLine("Оформление заказа отменено.");
                return false;
            }

            Order newOrder = new Order(clientId, basket);

            if (newOrder.Payment())
            {
                newOrder.ChangeStatus("Оплачен");
                Console.WriteLine("\n=== ЗАКАЗ УСПЕШНО ОФОРМЛЕН ===");
                newOrder.InfoOrder();
                newOrder.SaveToFile();
                basket.ClearBasket();
                return true;
            }
            else
            {
                newOrder.ChangeStatus("Ошибка оплаты");
                Console.WriteLine("Ошибка при оформлении заказа!");
                return false;
            }
        }

        public void SaveToFile()
        {
            using (StreamWriter file = new StreamWriter("orders.txt", true))
            {
                file.Write(numberOrder + "|" + clientOrder + "|" + statusOrder + "|" + costOrder.ToString("F2"));
                for (int i = 0; i < itemCount; i++)
                {
                    if (orderItems[i] != null)
                    {
                        file.Write("|" + orderItems[i].GetArticle() + "|" + quantities[i]);
                    }
                }
                file.WriteLine();
            }
        }

        // Геттеры
        public string GetNumberOrder() => numberOrder;
        public string GetClientOrder() => clientOrder;
        public string GetStatusOrder() => statusOrder;
        public double GetCostOrder() => costOrder;
        public int GetCountOrder() => countOrder;
        public string GetDateOrder() => dateOrder;
        public string GetPayWay() => payWay;
        public string GetShopOrder() => shopOrder;
        public int GetItemCount() => itemCount;

        public Product GetOrderItem(int index)
        {
            if (index >= 0 && index < itemCount)
            {
                return orderItems[index];
            }
            return null;
        }

        public int GetQuantity(int index)
        {
            if (index >= 0 && index < itemCount)
            {
                return quantities[index];
            }
            return 0;
        }

        private void CopyProductsFromBasket(Basket basket)
        {
            if (basket.IsEmpty())
            {
                return;
            }

            itemCount = basket.GetProductCount();
            countOrder = (int)basket.GetBasketCount();
            costOrder = basket.GetBasketCost();

            for (int i = 0; i < itemCount; i++)
            {
                Product basketProduct = basket.GetProduct(i);
                if (basketProduct != null)
                {
                    // Создаем новый Product вручную, копируя все поля
                    orderItems[i] = new Product(
                        basketProduct.GetArticle(),
                        basketProduct.GetName(),
                        basketProduct.GetBrand(),
                        basketProduct.GetProducerCountry(),
                        basketProduct.GetCategory(),
                        basketProduct.GetTypeProduct(),
                        basketProduct.GetDescription(),
                        basketProduct.GetQuantityStock(),
                        basketProduct.GetPrice()
                    );
                    quantities[i] = basket.GetQuantity(i);
                }
            }
        }

        private string GenerateOrderNumber()
        {
            Random rand = new Random();
            int randomNum = rand.Next(10000);
            return "ORD" + randomNum.ToString("D4");
        }

        private string GetCurrentDate()
        {
            return DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        }

        private void ClearOrderItems()
        {
            for (int i = 0; i < itemCount; i++)
            {
                orderItems[i] = null;
            }
            itemCount = 0;
        }
    }
}