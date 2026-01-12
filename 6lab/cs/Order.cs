using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MusicStore
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

        // Конструктор по умолчанию
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

            // Инициализация массивов
            for (int i = 0; i < MAX_ORDER_ITEMS; i++)
            {
                orderItems[i] = null;
                quantities[i] = 0;
            }

            Console.WriteLine($"Вызван конструктор по умолчанию Order: {numberOrder}");
        }

        // Конструктор с параметрами
        public Order(string clientId, Basket basket)
        {
            orderItems = new Product[MAX_ORDER_ITEMS];
            quantities = new int[MAX_ORDER_ITEMS];
            itemCount = 0;
            countOrder = 0;
            costOrder = 0.0;
            clientOrder = clientId;
            numberOrder = GenerateOrderNumber();
            statusOrder = "Ожидает оплаты";
            dateOrder = GetCurrentDate();
            shopOrder = "Музыкальный магазин 'MusicLoverzz'";
            payWay = "Не выбран";

            // Инициализация массивов
            for (int i = 0; i < MAX_ORDER_ITEMS; i++)
            {
                orderItems[i] = null;
                quantities[i] = 0;
            }

            CopyProductsFromBasket(basket);
            Console.WriteLine($"Вызван конструктор с параметрами Order: {numberOrder}");
        }

        // ЗАПРЕЩЕННЫЙ конструктор копирования
        private Order(Order other)
        {
           
            throw new InvalidOperationException("Копирование Order запрещено!");
        }

        
        ~Order()
        {
            Console.WriteLine($"Вызван деструктор Order: {numberOrder}");
            ClearOrderItems();
        }

        // Методы
        public void ChangeStatus(string newStatus)
        {
            statusOrder = newStatus;
            if (newStatus == "Оплачен")
            {
                Console.WriteLine("\n=== ВАШ ЗАКАЗ УСПЕШНО ОФОРМЛЕН! ===");
                Console.WriteLine($"Номер заказа: {numberOrder}");
                Console.WriteLine($"Статус: {statusOrder}");
                Console.WriteLine("==================================");
            }
            else
            {
                Console.WriteLine($"Статус заказа изменен на: {statusOrder}");
            }
        }

        public bool Payment()
        {
            Console.WriteLine("\n=== ОПЛАТА ЗАКАЗА ===");
            Console.WriteLine($"Сумма к оплате: {costOrder:F2} руб.");

            int paymentChoice;
            bool validChoice = false;

            while (!validChoice)
            {
                Console.WriteLine("Выберите способ оплаты:");
                Console.WriteLine("1. Банковская карта");
                Console.WriteLine("2. Электронный кошелек");
                Console.WriteLine("3. Наложенный платеж");
                Console.Write("Ваш выбор: ");

                if (!int.TryParse(Console.ReadLine(), out paymentChoice))
                {
                    Console.WriteLine("Ошибка! Введите число от 1 до 3.");
                    continue;
                }

                if (paymentChoice >= 1 && paymentChoice <= 3)
                {
                    validChoice = true;
                }
                else
                {
                    Console.WriteLine("Неверный выбор! Введите 1, 2 или 3.");
                }
            }
            return true;
        }
        public void InfoOrder()
        {
            Console.WriteLine("\n=== ИНФОРМАЦИЯ О ЗАКАЗЕ ===");
            Console.WriteLine($"Номер заказа: {numberOrder}");
            Console.WriteLine($"Клиент: {clientOrder}");
            Console.WriteLine($"Дата заказа: {dateOrder}");
            Console.WriteLine($"Статус: {statusOrder}");
            Console.WriteLine($"Способ оплаты: {payWay}");
            Console.WriteLine($"Количество товаров: {countOrder}");
            Console.WriteLine($"Общая стоимость: {costOrder:F2} руб.");
            Console.WriteLine($"Магазин: {shopOrder}");

            if (itemCount > 0)
            {
                Console.WriteLine("\nСостав заказа:");
                Console.WriteLine("-----------------------------------------------");
                for (int i = 0; i < itemCount; i++)
                {
                    if (orderItems[i] != null)
                    {
                        Console.WriteLine($"{i + 1}. {orderItems[i].GetName()} - " +
                                          $"{quantities[i]} шт. x " +
                                          $"{orderItems[i].GetPrice():F2} руб. = " +
                                          $"{(quantities[i] * orderItems[i].GetPrice()):F2} руб.");
                    }
                }
                Console.WriteLine("-----------------------------------------------");
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

            // Создаем заказ
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
            try
            {
                using (StreamWriter file = File.AppendText("orders.txt"))
                {
                    file.Write($"{numberOrder}|{clientOrder}|{statusOrder}|{costOrder:F2}");

                    for (int i = 0; i < itemCount; i++)
                    {
                        if (orderItems[i] != null)
                        {
                            file.Write($"|{orderItems[i].GetArticle()}|{quantities[i]}");
                        }
                    }
                    file.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения заказа: {ex.Message}");
            }
        }

        // Геттеры
        public string NumberOrder => numberOrder;
        public string ClientOrder => clientOrder;
        public string StatusOrder => statusOrder;
        public double CostOrder => costOrder;
        public int CountOrder => countOrder;
        public string DateOrder => dateOrder;
        public string PayWay => payWay;
        public string ShopOrder => shopOrder;
        public int ItemCount => itemCount;

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

        // Приватные методы
        private void CopyProductsFromBasket(Basket basket)
        {
            if (basket.IsEmpty())
            {
                return;
            }

            itemCount = basket.ProductCount;
            countOrder = (int)basket.GetBasketCount(); // Исправлено: приведение типа
            costOrder = basket.GetBasketCost();

            for (int i = 0; i < itemCount; i++)
            {
                Product basketProduct = basket.GetProduct(i);
                if (basketProduct != null)
                {
                    // Создаем новый объект Product с теми же данными
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

        private void ClearOrderItems()
        {
            for (int i = 0; i < itemCount; i++)
            {
                orderItems[i] = null;
                quantities[i] = 0;
            }
            itemCount = 0;
        }

        private string GenerateOrderNumber()
        {
            Random rand = new Random();
            int randomNum = rand.Next(10000);
            return $"ORD{randomNum:D4}";
        }

        private string GetCurrentDate()
        {
            return DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        }
    }
}