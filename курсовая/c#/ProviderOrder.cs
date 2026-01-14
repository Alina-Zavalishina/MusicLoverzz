using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Курсовая_работа_Музыкальный_магазин_с_
{
    public class ProviderOrder
    {
        private const int MAX_ORDER_ITEMS = 100;

        // Основная информация о заказе поставщику
        private string numberProviderOrder;
        private string provider;
        private int[] articles = new int[MAX_ORDER_ITEMS];
        private int[] quantities = new int[MAX_ORDER_ITEMS];
        private int itemCount;
        private string deliveryAddress;
        private string status;

        public ProviderOrder()
        {
            // Инициализация массивов артикулов и количеств
            for (int i = 0; i < MAX_ORDER_ITEMS; i++)
            {
                articles[i] = 0;
                quantities[i] = 0;
            }
            // Установка базовой информации о заказе поставщику
            numberProviderOrder = GenerateOrderNumber();
            status = "Создан";
            deliveryAddress = "Склад магазина";
            provider = "Основной поставщик";
            itemCount = 0;
        }

        // Геттер для получения номера заказа поставщику
        public string GetNumberProviderOrder() => numberProviderOrder;

        // Метод для формирования заказа
        public void AddProvider()
        {
            Console.WriteLine("\n=== ДОБАВЛЕНИЕ ТОВАРА В ЗАКАЗ ПОСТАВЩИКУ ===");
            DisplayClientOrders();

            if (!File.Exists("orders.txt"))
            {
                Console.WriteLine("Файл заказов клиентов не найден!");
                return;
            }

            string[] lines = File.ReadAllLines("orders.txt");
            int orderCount = lines.Count(line => !string.IsNullOrEmpty(line));

            if (orderCount == 0)
            {
                Console.WriteLine("Нет заказов клиентов!");
                return;
            }

            Console.Write($"\nВыберите номер заказа для отправки поставщику (1-{orderCount}): ");
            if (!int.TryParse(Console.ReadLine(), out int selectedOrder) ||
                selectedOrder < 1 || selectedOrder > orderCount)
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

        // Метод добавления конкретного заказа к поставщику
        private bool AddOrderToProvider(int orderIndex)
        {
            if (!File.Exists("orders.txt"))
                return false;

            string[] lines = File.ReadAllLines("orders.txt");
            int currentIndex = 0;

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;

                if (currentIndex == orderIndex)
                {
                    string[] tokens = line.Split('|');

                    if (tokens.Length >= 5)
                    {
                        Console.WriteLine($"\nДобавляем заказ клиента: {tokens[0]}");

                        // Обрабатываем товары в заказе
                        for (int i = 4; i < tokens.Length - 1; i += 2)
                        {
                            if (itemCount >= MAX_ORDER_ITEMS)
                            {
                                Console.WriteLine("Достигнуто максимальное количество товаров!");
                                break;
                            }

                            if (int.TryParse(tokens[i], out int article) &&
                                int.TryParse(tokens[i + 1], out int quantity))
                            {
                                // Проверяем, есть ли уже такой товар в заказе поставщику
                                bool alreadyExists = false;
                                for (int j = 0; j < itemCount; j++)
                                {
                                    if (articles[j] == article)
                                    {
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
                                    itemCount++;
                                }

                                // Получаем название товара из файла products.txt
                                string productName = "Неизвестный товар";
                                if (File.Exists("products.txt"))
                                {
                                    foreach (string productLine in File.ReadAllLines("products.txt"))
                                    {
                                        if (string.IsNullOrEmpty(productLine)) continue;

                                        string[] productTokens = productLine.Split('|');
                                        if (productTokens.Length >= 2 &&
                                            int.TryParse(productTokens[0], out int prodArticle) &&
                                            prodArticle == article)
                                        {
                                            productName = productTokens[1];
                                            break;
                                        }
                                    }
                                }

                                Console.WriteLine($"Добавлен товар: {productName} (арт: {article}) - {quantity} шт.");
                            }
                        }
                    }
                    return true;
                }
                currentIndex++;
            }
            return false;
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
            int orderNumber = 1;

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;

                string[] tokens = line.Split('|');

                if (tokens.Length >= 4)
                {
                    Console.WriteLine($"{orderNumber}. Заказ №{tokens[0]} | Клиент: {tokens[1]} | " +
                                     $"Статус: {tokens[2]} | Стоимость: {tokens[3]} руб.");

                    if (tokens.Length > 4)
                    {
                        Console.Write("   Товары: ");
                        for (int i = 4; i < tokens.Length - 1; i += 2)
                        {
                            if (int.TryParse(tokens[i], out int article) &&
                                int.TryParse(tokens[i + 1], out int quantity))
                            {
                                string productName = "Неизвестный";
                                if (File.Exists("products.txt"))
                                {
                                    foreach (string productLine in File.ReadAllLines("products.txt"))
                                    {
                                        if (string.IsNullOrEmpty(productLine)) continue;

                                        string[] productTokens = productLine.Split('|');
                                        if (productTokens.Length >= 2 &&
                                            int.TryParse(productTokens[0], out int prodArticle) &&
                                            prodArticle == article)
                                        {
                                            productName = productTokens[1];
                                            break;
                                        }
                                    }
                                }
                                Console.Write($"{productName}({quantity} шт.) ");
                            }
                        }
                        Console.WriteLine();
                    }
                }
                orderNumber++;
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

            for (int i = 0; i < itemCount; i++)
            {
                string productName = "Неизвестный товар";
                if (File.Exists("products.txt"))
                {
                    foreach (string productLine in File.ReadAllLines("products.txt"))
                    {
                        if (string.IsNullOrEmpty(productLine)) continue;

                        string[] productTokens = productLine.Split('|');
                        if (productTokens.Length >= 2 &&
                            int.TryParse(productTokens[0], out int prodArticle) &&
                            prodArticle == articles[i])
                        {
                            productName = productTokens[1];
                            break;
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

            // Создаем копию заказа и добавляем в магазин
            // Note: В C# это будет зависеть от реализации Shop класса
            // musicShop.AddProviderOrder(new ProviderOrder(this.Clone()));

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

            if (status != "Отправлен поставщику")
            {
                Console.WriteLine("Заказ еще не отправлен поставщику! Сначала отправьте заказ.");
                return;
            }

            Console.WriteLine($"Поставщик '{provider}' доставил товары:");

            for (int i = 0; i < itemCount; i++)
            {
                string productName = "Неизвестный товар";
                if (File.Exists("products.txt"))
                {
                    foreach (string productLine in File.ReadAllLines("products.txt"))
                    {
                        if (string.IsNullOrEmpty(productLine)) continue;

                        string[] productTokens = productLine.Split('|');
                        if (productTokens.Length >= 2 &&
                            int.TryParse(productTokens[0], out int prodArticle) &&
                            prodArticle == articles[i])
                        {
                            productName = productTokens[1];
                            break;
                        }
                    }
                }

                Console.WriteLine($"• {productName} (арт.{articles[i]}) - {quantities[i]} шт.");

                // Обновляем количество товара на складе
                if (!File.Exists("products.txt"))
                    continue;

                List<string> updatedLines = new List<string>();
                bool updated = false;

                foreach (string line in File.ReadAllLines("products.txt"))
                {
                    if (string.IsNullOrEmpty(line))
                    {
                        updatedLines.Add(line);
                        continue;
                    }

                    string[] tokens = line.Split('|');

                    if (tokens.Length == 10)
                    {
                        try
                        {
                            int currentArticle = int.Parse(tokens[0]);

                            if (currentArticle == articles[i])
                            {
                                int newQuantity = int.Parse(tokens[7]) + quantities[i];
                                tokens[7] = newQuantity.ToString();
                                tokens[8] = newQuantity > 0 ? "1" : "0";
                                updated = true;
                                Console.WriteLine($"   Обновлено количество: {tokens[7]} шт.");
                            }

                            updatedLines.Add(string.Join("|", tokens));
                        }
                        catch
                        {
                            updatedLines.Add(line);
                        }
                    }
                    else
                    {
                        updatedLines.Add(line);
                    }
                }

                if (updated)
                {
                    File.WriteAllLines("products.txt", updatedLines);
                }
            }

            Console.WriteLine("Информация о складе успешно обновлена!");
            status = "Выполнен";
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
            Random rand = new Random();
            int randomNum = rand.Next(10000);
            return "PROV" + randomNum.ToString();
        }
    }
}
