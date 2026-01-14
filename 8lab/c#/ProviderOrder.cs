
using System;
using System.IO;
using System.Collections.Generic;

namespace MusicStore
{
    public class ProviderOrder
    {
        // Константы
        private const int MAX_ORDER_ITEMS = 100; // максимальное количество товаров в заказе поставщику

        // Поля класса
        private string numberProviderOrder;    // уникальный номер заказа поставщику
        private string provider;               // название компании-поставщика
        private int[] articles;                // массив артикулов заказываемых товаров
        private int[] quantities;              // массив количеств для каждого товара
        private int itemCount;                 // текущее количество различных товаров в заказе
        private string deliveryAddress;        // адрес доставки от поставщика
        private string status;                 // текущий статус заказа

        // Конструктор класса
        public ProviderOrder()
        {
            itemCount = 0;
            articles = new int[MAX_ORDER_ITEMS];
            quantities = new int[MAX_ORDER_ITEMS];

            // Инициализация массивов артикулов и количеств
            for (int i = 0; i < MAX_ORDER_ITEMS; i++)
            {
                articles[i] = 0;    // обнуляем все артикулы
                quantities[i] = 0;  // обнуляем все количества
            }

            // Установка базовой информации о заказе поставщику
            numberProviderOrder = GenerateOrderNumber(); // генерация номера заказа
            status = "Создан";                          // начальный статус
            deliveryAddress = "Склад магазина";         // адрес доставки
            provider = "Основной поставщик";            // поставщик
        }

        // Метод добавления заказа поставщику на основе клиентских заказов
        public void AddProvider()
        {
            Console.WriteLine("\n=== ДОБАВЛЕНИЕ ТОВАРА В ЗАКАЗ ПОСТАВЩИКУ ===");
            DisplayClientOrders();

            if (!File.Exists("orders.txt"))
            {
                Console.WriteLine("Файл заказов клиентов не найден!");
                return;
            }

            // Подсчитываем количество заказов в файле
            string[] lines = File.ReadAllLines("orders.txt");
            int orderCount = 0;
            foreach (string line in lines)
            {
                if (!string.IsNullOrEmpty(line)) orderCount++;
            }

            if (orderCount == 0)
            {
                Console.WriteLine("Нет заказов клиентов!");
                return;
            }

            Console.Write($"\nВыберите номер заказа для отправки поставщику (1-{orderCount}): ");
            if (int.TryParse(Console.ReadLine(), out int selectedOrder))
            {
                if (selectedOrder < 1 || selectedOrder > orderCount)
                {
                    Console.WriteLine("Неверный выбор!");
                    return;
                }

                // Добавляем выбранный заказ к поставщику
                if (AddOrderToProvider(selectedOrder - 1))
                {
                    Console.WriteLine("Заказ успешно добавлен к поставщику!");
                    Console.WriteLine($"Номер заказа поставщика: {numberProviderOrder}");
                }
                else
                {
                    Console.WriteLine("Ошибка при добавлении заказа!");
                }
            }
            else
            {
                Console.WriteLine("Неверный ввод!");
            }
        }

        // Метод добавления конкретного заказа к поставщику
        private bool AddOrderToProvider(int orderIndex)
        {
            if (!File.Exists("orders.txt"))
            {
                return false;
            }

            string[] lines = File.ReadAllLines("orders.txt");
            if (orderIndex < 0 || orderIndex >= lines.Length)
            {
                return false;
            }

            string line = lines[orderIndex];
            if (string.IsNullOrEmpty(line))
            {
                return false;
            }

            string[] tokens = line.Split('|');
            if (tokens.Length < 5)
            {
                return false;
            }

            Console.WriteLine($"\nДобавляем заказ клиента: {tokens[0]}");

            // Обрабатываем товары в заказе
            for (int i = 4; i < tokens.Length - 1; i += 2)
            {
                if (itemCount >= MAX_ORDER_ITEMS)
                {
                    // Проверка на переполнение массива товаров
                    Console.WriteLine("Достигнуто максимальное количество товаров!");
                    break;
                }

                try
                {
                    // Извлекаем артикул и количество товара
                    int article = int.Parse(tokens[i]);
                    int quantity = int.Parse(tokens[i + 1]);

                    // Проверяем, есть ли уже такой товар в заказе поставщику
                    bool alreadyExists = false;
                    for (int j = 0; j < itemCount; j++)
                    {
                        if (articles[j] == article)
                        {
                            // Если товар уже есть - увеличиваем количество
                            quantities[j] += quantity;
                            alreadyExists = true;
                            break;
                        }
                    }

                    // Если товара нет - добавляем новый
                    if (!alreadyExists)
                    {
                        articles[itemCount] = article;
                        quantities[itemCount] = quantity;
                        itemCount++; // увеличиваем счетчик товаров
                    }

                    // Получаем название товара из файла products.txt
                    string productName = "Неизвестный товар";
                    if (File.Exists("products.txt"))
                    {
                        string[] productLines = File.ReadAllLines("products.txt");
                        foreach (string productLine in productLines)
                        {
                            string[] productTokens = productLine.Split('|');
                            if (productTokens.Length >= 2)
                            {
                                if (int.TryParse(productTokens[0], out int currentArticle) && currentArticle == article)
                                {
                                    productName = productTokens[1];
                                    break;
                                }
                            }
                        }
                    }

                    Console.WriteLine($"Добавлен товар: {productName} (арт: {article}) - {quantity} шт.");
                }
                catch
                {
                    continue;
                }
            }

            return true;
        }

        // Метод отображения списка клиентских заказов
        private void DisplayClientOrders()
        {
            if (!File.Exists("orders.txt"))
            {
                Console.WriteLine("Файл заказов клиентов не найден!");
                return;
            }

            Console.WriteLine("\n=== СПИСОК ЗАКАЗОВ КЛИЕНТОВ ===");
            string[] lines = File.ReadAllLines("orders.txt");
            int orderNumber = 1; // счетчик заказов для отображения

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;

                string[] tokens = line.Split('|');
                if (tokens.Length >= 4)
                {
                    // Выводим основную информацию о заказе
                    Console.WriteLine($"{orderNumber}. Заказ №{tokens[0]} | Клиент: {tokens[1]} " +
                                     $"| Статус: {tokens[2]} | Стоимость: {tokens[3]} руб.");

                    // Если есть товары в заказе
                    if (tokens.Length > 4)
                    {
                        Console.Write(" Товары: ");

                        // Обрабатываем пары "артикул|количество"
                        for (int i = 4; i < tokens.Length - 1; i += 2)
                        {
                            try
                            {
                                int article = int.Parse(tokens[i]);
                                int quantity = int.Parse(tokens[i + 1]);

                                // Ищем название товара по артикулу
                                string productName = "Неизвестный";
                                if (File.Exists("products.txt"))
                                {
                                    string[] productLines = File.ReadAllLines("products.txt");
                                    foreach (string productLine in productLines)
                                    {
                                        string[] productTokens = productLine.Split('|');
                                        if (productTokens.Length >= 2)
                                        {
                                            if (int.TryParse(productTokens[0], out int currentArticle) && currentArticle == article)
                                            {
                                                productName = productTokens[1];
                                                break;
                                            }
                                        }
                                    }
                                }

                                Console.Write($"{productName}({quantity} шт.) ");
                            }
                            catch
                            {
                                // Пропускаем некорректные данные
                            }
                        }
                        Console.WriteLine();
                    }
                }
                orderNumber++; // увеличиваем счетчик заказов
                Console.WriteLine();
            }
        }

        // Метод отправки заказа поставщику
        public bool Sending()
        {
            Console.WriteLine("\n=== ОТПРАВКА ЗАКАЗА ПОСТАВЩИКУ ===");

            if (itemCount == 0)
            {
                Console.WriteLine("Заказ пуст! Сначала добавьте товары через пункт 'Добавить товар в заказ'.");
                return false;
            }

            Console.WriteLine($"Текущий заказ поставщика №{numberProviderOrder}:");
            Console.WriteLine($"Количество позиций: {itemCount}");
            Console.Write("Товары: ");

            // Перебираем все товары в заказе поставщика
            for (int i = 0; i < itemCount; i++)
            {
                // Ищем название товара по артикулу
                string productName = "Неизвестный товар";
                if (File.Exists("products.txt"))
                {
                    string[] productLines = File.ReadAllLines("products.txt");
                    foreach (string productLine in productLines)
                    {
                        string[] productTokens = productLine.Split('|');
                        if (productTokens.Length >= 2)
                        {
                            if (int.TryParse(productTokens[0], out int currentArticle) && currentArticle == articles[i])
                            {
                                productName = productTokens[1];
                                break;
                            }
                        }
                    }
                }

                Console.Write($"{productName}({quantities[i]} шт.) ");
            }
            Console.WriteLine();

            // Запрашиваем подтверждение отправки
            Console.Write("\nВы уверены, что хотите отправить этот заказ поставщику? (y/n): ");
            char confirmation = Console.ReadKey().KeyChar;
            Console.WriteLine();

            if (confirmation != 'y' && confirmation != 'Y')
            {
                Console.WriteLine("Отправка отменена.");
                return false;
            }

            Console.WriteLine($"Отправка заказа поставщику '{provider}'...");
            Console.WriteLine($"Адрес доставки: {deliveryAddress}");
            Console.WriteLine("Заказ успешно отправлен поставщику!");

            status = "Отправлен поставщику";
            Console.WriteLine($"Статус заказа изменен на: '{status}'");



            return true;
        }

        // Метод обновления информации о товарах на складе после доставки
        public void NewInfoStock()
        {
            Console.WriteLine("\n=== ОБНОВЛЕНИЕ ИНФОРМАЦИИ О ТОВАРАХ ===");

            if (itemCount == 0)
            {
                Console.WriteLine("Нет товаров для обновления! Сначала добавьте товары и отправьте заказ.");
                return;
            }

            // Проверяем что заказ был отправлен поставщику
            if (status != "Отправлен поставщику")
            {
                Console.WriteLine("Заказ еще не отправлен поставщику! Сначала отправьте заказ.");
                return;
            }

            Console.WriteLine($"Поставщик '{provider}' доставил товары:");

            // Перебираем все товары в заказе
            for (int i = 0; i < itemCount; i++)
            {
                // Ищем название товара
                string productName = "Неизвестный товар";
                if (File.Exists("products.txt"))
                {
                    string[] productLines = File.ReadAllLines("products.txt");
                    foreach (string productLine in productLines)
                    {
                        string[] productTokens = productLine.Split('|');
                        if (productTokens.Length >= 2)
                        {
                            if (int.TryParse(productTokens[0], out int currentArticle) && currentArticle == articles[i])
                            {
                                productName = productTokens[1];
                                break;
                            }
                        }
                    }
                }

                Console.WriteLine($"• {productName} (арт.{articles[i]}) - {quantities[i]} шт.");

                // Обновляем количество товара на складе
                if (File.Exists("products.txt"))
                {
                    List<string> updatedLines = new List<string>();
                    bool updated = false;

                    string[] lines = File.ReadAllLines("products.txt");
                    foreach (string line in lines)
                    {
                        if (string.IsNullOrEmpty(line))
                        {
                            updatedLines.Add(line);
                            continue;
                        }

                        string[] tokens = line.Split('|');
                        if (tokens.Length >= 10)
                        {
                            try
                            {
                                int currentArticle = int.Parse(tokens[0]);

                                if (currentArticle == articles[i])
                                {
                                    // Увеличиваем количество на складе
                                    int newQuantity = int.Parse(tokens[7]) + quantities[i];
                                    tokens[7] = newQuantity.ToString();
                                    tokens[8] = newQuantity > 0 ? "1" : "0"; // Обновляем статус наличия
                                    updated = true;
                                    Console.WriteLine($" Обновлено количество: {tokens[7]} шт.");
                                }

                                // Записываем строку (обновленную или оригинальную)
                                updatedLines.Add(string.Join("|", tokens));
                            }
                            catch
                            {
                                updatedLines.Add(line); // При ошибке записываем исходную строку
                            }
                        }
                        else
                        {
                            updatedLines.Add(line); // Если не все поля - записываем как есть
                        }
                    }

                    if (updated)
                    {
                        File.WriteAllLines("products.txt", updatedLines);
                    }
                }
            }

            Console.WriteLine("Информация о складе успешно обновлена!");
            status = "Выполнен"; // Обновляем статус заказа
            Console.WriteLine("Статус заказа изменен на: 'Выполнен'");

            // Очищаем заказ после выполнения
            itemCount = 0;
            for (int i = 0; i < MAX_ORDER_ITEMS; i++)
            {
                articles[i] = 0;
                quantities[i] = 0;
            }
        }

        // Метод генерации уникального номера заказа поставщику
        private string GenerateOrderNumber()
        {
            Random random = new Random();
            int randomNum = random.Next(10000); // генерация случайного числа от 0 до 9999
            return "PROV" + randomNum.ToString("D4");
        }

        // Свойства для доступа к полям
        public string NumberProviderOrder
        {
            get { return numberProviderOrder; }
        }

        public string Provider
        {
            get { return provider; }
            set { provider = value; }
        }

        public string DeliveryAddress
        {
            get { return deliveryAddress; }
            set { deliveryAddress = value; }
        }

        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        public int ItemCount
        {
            get { return itemCount; }
        }


        public string GetNumberProviderOrder()
        {
            return numberProviderOrder;
        }

        // Метод для получения информации о заказе поставщика
        public void DisplayInfo()
        {
            Console.WriteLine("\n=== ИНФОРМАЦИЯ О ЗАКАЗЕ ПОСТАВЩИКУ ===");
            Console.WriteLine($"Номер заказа: {numberProviderOrder}");
            Console.WriteLine($"Поставщик: {provider}");
            Console.WriteLine($"Статус: {status}");
            Console.WriteLine($"Адрес доставки: {deliveryAddress}");
            Console.WriteLine($"Количество позиций: {itemCount}");

            if (itemCount > 0)
            {
                Console.WriteLine("\nТовары в заказе:");
                for (int i = 0; i < itemCount; i++)
                {
                    string productName = "Неизвестный товар";
                    if (File.Exists("products.txt"))
                    {
                        string[] productLines = File.ReadAllLines("products.txt");
                        foreach (string productLine in productLines)
                        {
                            string[] productTokens = productLine.Split('|');
                            if (productTokens.Length >= 2)
                            {
                                if (int.TryParse(productTokens[0], out int currentArticle) && currentArticle == articles[i])
                                {
                                    productName = productTokens[1];
                                    break;
                                }
                            }
                        }
                    }
                    Console.WriteLine($"{i + 1}. {productName} (арт. {articles[i]}) - {quantities[i]} шт.");
                }
            }

            Console.WriteLine("======================================");
        }
    }
}