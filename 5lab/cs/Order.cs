using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace MusicShopSystem
{
    public class Order : IDisposable
    {
        // Константы
        private const int MAX_ORDER_ITEMS = 100; // максимальное количество различных товаров в заказе

        // Поля класса
        private string numberOrder;              // номер заказа
        private string clientOrder;              // идентификатор клиента
        private Product[] orderItems;           // массив товаров в заказе
        private int[] quantities;               // массив количеств для каждого товара
        private int itemCount;                  // количество различных товаров в заказе
        private int countOrder;                 // общее количество
        private double costOrder;               // общая стоимость
        private string statusOrder;             // статус заказа
        private string payWay;                  // способ оплаты
        private string shopOrder;               // магазин для получения
        private string dateOrder;               // дата и время создания заказа

        private bool disposed = false;          // Флаг для отслеживания освобождения ресурсов

        // Конструктор по умолчанию
        public Order()
        {
            itemCount = 0;
            countOrder = 0;
            costOrder = 0.0;
            orderItems = new Product[MAX_ORDER_ITEMS];
            quantities = new int[MAX_ORDER_ITEMS];

            for (int i = 0; i < MAX_ORDER_ITEMS; i++)
            {
                orderItems[i] = null;
                quantities[i] = 0;
            }

            numberOrder = GenerateOrderNumber();
            statusOrder = "Ожидает оплаты";
            dateOrder = GetCurrentDate();
            shopOrder = "Музыкальный магазин 'MusicLoverzz'";
            payWay = "Не выбран";
        }

        // Конструктор с параметрами (клиент и корзина)
        public Order(string clientId, Basket basket)
        {
            clientOrder = clientId;
            itemCount = 0;
            countOrder = 0;
            costOrder = 0.0;
            orderItems = new Product[MAX_ORDER_ITEMS];
            quantities = new int[MAX_ORDER_ITEMS];

            for (int i = 0; i < MAX_ORDER_ITEMS; i++)
            {
                orderItems[i] = null;
                quantities[i] = 0;
            }

            numberOrder = GenerateOrderNumber();
            statusOrder = "Ожидает оплаты";
            dateOrder = GetCurrentDate();
            shopOrder = "Музыкальный магазин 'MusicLoverzz'";
            payWay = "Не выбран";

            CopyProductsFromBasket(basket); // копирование информации из корзины
        }


        ~Order()
        {
            Dispose(false);
        }

        // Реализация интерфейса IDisposable для явного освобождения ресурсов
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Отменяем вызов финализатора
        }

        // Защищенный метод для освобождения ресурсов
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Освобождаем управляемые ресурсы - товары в заказе
                    for (int i = 0; i < itemCount; i++)
                    {
                        if (orderItems[i] != null)
                        {
                            // Если Product реализует IDisposable
                            if (orderItems[i] is IDisposable disposableProduct)
                            {
                                disposableProduct.Dispose();
                            }
                            orderItems[i] = null;
                        }
                    }

                    // Обнуляем массивы
                    if (orderItems != null)
                    {
                        Array.Clear(orderItems, 0, orderItems.Length);
                    }

                    if (quantities != null)
                    {
                        Array.Clear(quantities, 0, quantities.Length);
                    }
                }

                // Освобождаем неуправляемые ресурсы (если есть)
                // В данном классе их нет

                disposed = true;
            }
        }

        // Метод копирования товаров из корзины в заказ
        private void CopyProductsFromBasket(Basket basket)
        {
            if (basket.IsEmpty()) // проверка не пустая ли корзина
            {
                return; // если корзина пуста - выходим
            }

            // Копируем основную информацию из корзины
            itemCount = basket.GetProductCount();    // количество различных товаров
            countOrder = (int)basket.GetBasketCount(); // общее количество единиц товара
            costOrder = basket.GetBasketCost();      // общая стоимость

            // Копируем каждый товар из корзины в заказ
            for (int i = 0; i < itemCount; i++)
            {
                // Получаем товар из корзины
                Product basketProduct = basket.GetProduct(i);

                // Проверяем что товар существует
                if (basketProduct != null)
                {
                    // Создаем ПОЛНУЮ КОПИЮ товара
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

                    // Копируем количество товара
                    quantities[i] = basket.GetQuantity(i);
                }
            }
        }

        // Статический метод для создания заказа 
        public static bool MakeOrder(string clientId, Basket basket)
        {
            // Проверка что корзина не пустая
            if (basket.IsEmpty())
            {
                Console.WriteLine("Корзина пуста! Добавьте товары перед оформлением заказа.");
                return false; // возвращаем false если корзина пуста
            }

            // Запрос подтверждения у пользователя
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

            // Создаем новый заказ из корзины
            Order newOrder = new Order(clientId, basket);

            // Проверяем оплату
            if (newOrder.Payment())
            {
                // Если оплата прошла успешно
                newOrder.ChangeStatus("Оплачен");
                Console.WriteLine("\n=== ЗАКАЗ УСПЕШНО ОФОРМЛЕН ===");
                newOrder.InfoOrder(); // показываем информацию о заказе
                newOrder.SaveToFile(); // сохраняем заказ в файл

                // Очищаем корзину
                basket.ClearBasket();

                newOrder.Dispose(); // освобождаем ресурсы
                return true;
            }
            else
            {
                newOrder.ChangeStatus("Ошибка оплаты");
                Console.WriteLine("Ошибка при оформлении заказа!");
                newOrder.Dispose(); // освобождаем ресурсы
                return false;
            }
        }

        // Метод сохранения заказа в файл
        public void SaveToFile()
        {
            if (disposed)
                throw new ObjectDisposedException("Order", "Объект уже освобожден");

            // Открываем файл для добавления
            using (StreamWriter file = new StreamWriter("orders.txt", true, Encoding.UTF8))
            {
                // Записываем основную информацию о заказе
                file.Write($"{numberOrder}|{clientOrder}|{statusOrder}|{costOrder:F2}");

                // Записываем информацию о каждом товаре в заказе
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

        // Метод изменения статуса заказа
        public void ChangeStatus(string newStatus)
        {
            if (disposed)
                throw new ObjectDisposedException("Order", "Объект уже освобожден");

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

        // Метод обработки оплаты заказа
        public bool Payment()
        {
            if (disposed)
                throw new ObjectDisposedException("Order", "Объект уже освобожден");

            Console.WriteLine("\n=== ОПЛАТА ЗАКАЗА ===");
            Console.WriteLine($"Сумма к оплате: {costOrder:F2} руб.");

            int paymentChoice = 0; // Инициализируем значением по умолчанию
            bool validChoice = false;

            while (!validChoice)
            {
                Console.WriteLine("Выберите способ оплаты:");
                Console.WriteLine("1. Банковская карта");
                Console.WriteLine("2. Электронный кошелек");
                Console.WriteLine("3. Наложенный платеж");
                Console.Write("Ваш выбор: ");

                string input = Console.ReadLine();
                if (int.TryParse(input, out int choice))
                {
                    if (choice >= 1 && choice <= 3)
                    {
                        paymentChoice = choice; // Присваиваем значение
                        validChoice = true; // выбор корректен
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


            switch (paymentChoice)
            {
                case 1:
                    payWay = "Банковская карта";
                    break;
                case 2:
                    payWay = "Электронный кошелек";
                    break;
                case 3:
                    payWay = "Наложенный платеж";
                    break;
            }

            Console.WriteLine("Обработка платежа...");
            Console.WriteLine($"Способ оплаты: {payWay}");
            Console.WriteLine("Оплата прошла успешно!");

            return true;
        }

        // Метод вывода полной информации о заказе
        public void InfoOrder()
        {
            if (disposed)
                throw new ObjectDisposedException("Order", "Объект уже освобожден");

            Console.WriteLine("\n=== ИНФОРМАЦИЯ О ЗАКАЗЕ ===");
            Console.WriteLine($"Номер заказа: {numberOrder}");
            Console.WriteLine($"Клиент: {clientOrder}");
            Console.WriteLine($"Дата заказа: {dateOrder}");
            Console.WriteLine($"Статус: {statusOrder}");
            Console.WriteLine($"Способ оплаты: {payWay}");
            Console.WriteLine($"Количество товаров: {countOrder}");
            Console.WriteLine($"Общая стоимость: {costOrder:F2} руб.");
            Console.WriteLine($"Магазин: {shopOrder}");

            // Вывод состава заказа если есть товары
            if (itemCount > 0)
            {
                Console.WriteLine("\nСостав заказа:");
                Console.WriteLine("-----------------------------------");

                // Перебор всех товаров в заказе
                for (int i = 0; i < itemCount; i++)
                {
                    if (orderItems[i] != null)
                    {
                        // Вывод информации о каждом товаре
                        double itemTotal = quantities[i] * orderItems[i].GetPrice();
                        Console.WriteLine($"{i + 1}. {orderItems[i].GetName()} - " +
                                         $"{quantities[i]} шт. x {orderItems[i].GetPrice()} руб. = " +
                                         $"{itemTotal:F2} руб.");
                    }
                }

                Console.WriteLine("-----------------------------------");
            }

            Console.WriteLine("==================================");
        }

        // Метод генерации уникального номера заказа
        private string GenerateOrderNumber()
        {
            Random random = new Random();
            int randomNum = random.Next(10000); // генерация случайного числа от 0 до 9999
            return "ORD" + randomNum.ToString("D4");
        }

        // Метод получения текущей даты и времени
        private string GetCurrentDate()
        {
            return DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        }

        // Свойства для доступа к полям
        public string NumberOrder
        {
            get
            {
                if (disposed)
                    throw new ObjectDisposedException("Order", "Объект уже освобожден");
                return numberOrder;
            }
        }

        public string ClientOrder
        {
            get
            {
                if (disposed)
                    throw new ObjectDisposedException("Order", "Объект уже освобожден");
                return clientOrder;
            }
        }

        public string StatusOrder
        {
            get
            {
                if (disposed)
                    throw new ObjectDisposedException("Order", "Объект уже освобожден");
                return statusOrder;
            }
        }

        public double CostOrder
        {
            get
            {
                if (disposed)
                    throw new ObjectDisposedException("Order", "Объект уже освобожден");
                return costOrder;
            }
        }

        public int CountOrder
        {
            get
            {
                if (disposed)
                    throw new ObjectDisposedException("Order", "Объект уже освобожден");
                return countOrder;
            }
        }

        public string DateOrder
        {
            get
            {
                if (disposed)
                    throw new ObjectDisposedException("Order", "Объект уже освобожден");
                return dateOrder;
            }
        }

        public string PayWay
        {
            get
            {
                if (disposed)
                    throw new ObjectDisposedException("Order", "Объект уже освобожден");
                return payWay;
            }
        }

        public string ShopOrder
        {
            get
            {
                if (disposed)
                    throw new ObjectDisposedException("Order", "Объект уже освобожден");
                return shopOrder;
            }
        }

        // Методы для совместимости с C++ кодом
        public string GetNumberOrder()
        {
            if (disposed)
                throw new ObjectDisposedException("Order", "Объект уже освобожден");
            return numberOrder;
        }

        public string GetClientOrder()
        {
            if (disposed)
                throw new ObjectDisposedException("Order", "Объект уже освобожден");
            return clientOrder;
        }

        public string GetStatusOrder()
        {
            if (disposed)
                throw new ObjectDisposedException("Order", "Объект уже освобожден");
            return statusOrder;
        }

        public double GetCostOrder()
        {
            if (disposed)
                throw new ObjectDisposedException("Order", "Объект уже освобожден");
            return costOrder;
        }

        public int GetCountOrder()
        {
            if (disposed)
                throw new ObjectDisposedException("Order", "Объект уже освобожден");
            return countOrder;
        }

        public string GetDateOrder()
        {
            if (disposed)
                throw new ObjectDisposedException("Order", "Объект уже освобожден");
            return dateOrder;
        }

        public string GetPayWay()
        {
            if (disposed)
                throw new ObjectDisposedException("Order", "Объект уже освобожден");
            return payWay;
        }

        public string GetShopOrder()
        {
            if (disposed)
                throw new ObjectDisposedException("Order", "Объект уже освобожден");
            return shopOrder;
        }

        // Метод получения количества различных товаров в заказе
        public int GetItemCount()
        {
            if (disposed)
                throw new ObjectDisposedException("Order", "Объект уже освобожден");
            return itemCount;
        }

        // Метод получения товара по индексу с проверкой границ массива
        public Product GetOrderItem(int index)
        {
            if (disposed)
                throw new ObjectDisposedException("Order", "Объект уже освобожден");

            if (index >= 0 && index < itemCount)
            {
                return orderItems[index];
            }
            return null;
        }

        // Метод получения количества товара по индексу с проверкой границ массива
        public int GetQuantity(int index)
        {
            if (disposed)
                throw new ObjectDisposedException("Order", "Объект уже освобожден");

            if (index >= 0 && index < itemCount)
            {
                return quantities[index];
            }
            return 0;
        }


        public void Close()
        {
            Dispose();
        }
    }
}
