using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Курсовая_работа_Музыкальный_магазин_с_
{
    public class Shop
    {
        private const int MAX_PRODUCTS = 1000; // максимальное количество товаров в каталоге
        private const int MAX_ORDERS = 1000; // максимальное количество активных заказов
        private const int MAX_PROVIDER_ORDERS = 100; // максимальное количество заказов поставщикам
        private const int MAX_SALES_REPORTS = 50; // максимальное количество отчетов о продажах

        private string shopName; // название магазина
        private string addressShop; // адрес магазина
        private Product[] catalogProducts; // массив товаров
        private int productCount; // текущее количество товаров в каталоге
        private Order[] activeOrders; // массив активных заказов клиентов
        private int orderCount; // текущее количество активных заказов
        private ProviderOrder[] providerOrders; // массив заказов поставщикам
        private int providerOrderCount; // текущее количество заказов поставщикам
        private SalesReport[] salesReports; // массив отчетов о продажах
        private int salesReportCount; // текущее количество отчетов

        public Shop()
        {
            shopName = "Музыкальный магазин 'MusicLoverzz'";
            addressShop = "г. Барнаул, ул. Ленина, 1";
            InitializeArrays();
            LoadProductsFromFile("products.txt");
            LoadSalesReports();
        }

        // Конструктор с параметрами класса Shop
        public Shop(string name, string address)
        {
            shopName = name;
            addressShop = address;
            InitializeArrays();
            LoadProductsFromFile("products.txt");
            LoadSalesReports();
        }

        // Метод инициализации массивов
        private void InitializeArrays()
        {
            catalogProducts = new Product[MAX_PRODUCTS];
            activeOrders = new Order[MAX_ORDERS];
            providerOrders = new ProviderOrder[MAX_PROVIDER_ORDERS];
            salesReports = new SalesReport[MAX_SALES_REPORTS];
            productCount = 0;
            orderCount = 0;
            providerOrderCount = 0;
            salesReportCount = 0;
        }

        private void LoadProductsFromFile(string filename)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine("Файл товаров не найден! Создайте сначала товары через режим администратора.");
                return;
            }

            try
            {
                string[] lines = File.ReadAllLines(filename);
                foreach (string line in lines)
                {
                    if (string.IsNullOrEmpty(line) || productCount >= MAX_PRODUCTS)
                        continue;

                    string[] tokens = line.Split('|');
                    if (tokens.Length == 10)
                    {
                        try
                        {
                            int article = int.Parse(tokens[0]);
                            string name = tokens[1];
                            string brand = tokens[2];
                            string country = tokens[3];
                            string category = tokens[4];
                            string type = tokens[5];
                            string description = tokens[6];

                            // Исправление: uint.Parse может выбросить исключение если tokens[7] пустая или не число
                            uint quantity = 0;
                            if (!string.IsNullOrEmpty(tokens[7]) && uint.TryParse(tokens[7], out uint parsedQuantity))
                            {
                                quantity = parsedQuantity;
                            }

                            // Исправление: проверка на null/empty перед сравнением
                            bool status = false;
                            if (!string.IsNullOrEmpty(tokens[8]))
                            {
                                status = (tokens[8] == "1");
                            }

                            // Исправление: int.Parse может выбросить исключение
                            int price = 0;
                            if (!string.IsNullOrEmpty(tokens[9]) && int.TryParse(tokens[9], out int parsedPrice))
                            {
                                price = parsedPrice;
                            }

                            // Проверяет находится ли товар в файле
                            bool exists = false;
                            for (int i = 0; i < productCount; i++)
                            {
                                if (catalogProducts[i] != null && catalogProducts[i].GetArticle() == article)
                                {
                                    exists = true;
                                    break;
                                }
                            }

                            if (!exists)
                            {
                                catalogProducts[productCount] = new Product(
                                    article, name, brand, country, category,
                                    type, description, quantity, price
                                );
                                productCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            // Логируем ошибку для отладки, но продолжаем обработку других строк
                            Console.WriteLine($"Ошибка при разборе строки: {line}. Ошибка: {ex.Message}");
                            continue;
                        }
                    }
                }
                Console.WriteLine($"Загружено {productCount} товаров из файла {filename}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла {filename}: {ex.Message}");
            }
        }

        // Отчет по продажам за последние 7 дней
        private void LoadSalesReports()
        {
            for (int daysAgo = 0; daysAgo < 7 && salesReportCount < MAX_SALES_REPORTS; daysAgo++)
            {
                DateTime date = DateTime.Now.AddDays(-daysAgo);
                string filename = $"sales_report_{date:dd.MM.yyyy}.txt";

                if (File.Exists(filename))
                {
                    SalesReport report = new SalesReport(date.ToString("dd.MM.yyyy"));
                    if (report.LoadFromFile(filename))
                    {
                        salesReports[salesReportCount] = report;
                        salesReportCount++;
                    }
                }
            }
        }

        public void AddProduct()
        {
            if (productCount >= MAX_PRODUCTS)
            {
                Console.WriteLine($"Каталог товаров переполнен! Максимум {MAX_PRODUCTS} товаров.");
                return;
            }

            Console.WriteLine("\n=== ДОБАВЛЕНИЕ НОВОГО ТОВАРА ===");
            Product newProduct = new Product();
            newProduct.InputFromKeyboard();

            for (int i = 0; i < productCount; i++)
            {
                if (catalogProducts[i].GetArticle() == newProduct.GetArticle())
                {
                    Console.WriteLine($"Ошибка: товар с артикулом {newProduct.GetArticle()} уже существует!");
                    return;
                }
            }

            catalogProducts[productCount] = newProduct;
            productCount++;
            newProduct.WriteToFile("products.txt");

            Console.WriteLine("Товар успешно добавлен в каталог и файл!");
        }

        public void DeleteProduct()
        {
            if (productCount == 0)
            {
                Console.WriteLine("Каталог товаров пуст!");
                return;
            }

            DisplayCatalog();

            Console.Write("Введите артикул товара для удаления: ");
            if (!int.TryParse(Console.ReadLine(), out int articleToDelete))
            {
                Console.WriteLine("Ошибка ввода!");
                return;
            }

            int index = FindProductIndex(articleToDelete);
            if (index == -1)
            {
                Console.WriteLine($"Товар с артикулом {articleToDelete} не найден!");
                return;
            }

            Product.DeleteProductByArticle("products.txt", articleToDelete);

            string productName = catalogProducts[index].GetName();

            for (int i = index; i < productCount - 1; i++)
            {
                catalogProducts[i] = catalogProducts[i + 1];
            }
            catalogProducts[productCount - 1] = null;
            productCount--;

            Console.WriteLine($"Товар '{productName}' успешно удален из каталога и файла!");
        }

        public void CreateOrder(string clientId, Basket basket)
        {
            if (orderCount >= MAX_ORDERS)
            {
                Console.WriteLine("Достигнуто максимальное количество активных заказов!");
                return;
            }

            if (basket.IsEmpty())
            {
                Console.WriteLine("Корзина пуста!");
                return;
            }

            // Проверка наличия товаров на складе
            for (int i = 0; i < basket.GetProductCount(); i++)
            {
                Product basketProduct = basket.GetProduct(i);
                int basketQuantity = basket.GetQuantity(i);

                int productIndex = FindProductIndex(basketProduct.GetArticle());
                if (productIndex == -1)
                {
                    Console.WriteLine($"Товар '{basketProduct.GetName()}' не найден в каталоге!");
                    return;
                }

                if (catalogProducts[productIndex].GetQuantityStock() < basketQuantity)
                {
                    Console.WriteLine($"Недостаточно товара '{basketProduct.GetName()}' на складе. " +
                        $"Доступно: {catalogProducts[productIndex].GetQuantityStock()}, " +
                        $"требуется: {basketQuantity}");
                    return;
                }
            }

            Order newOrder = new Order(clientId, basket);

            if (newOrder.Payment())
            {
                newOrder.ChangeStatus("Оплачен");
                activeOrders[orderCount] = newOrder; // добавляем в массив активных заказов
                orderCount++;

                // Уменьшаем количество товаров на складе
                for (int i = 0; i < basket.GetProductCount(); i++)
                {
                    Product basketProduct = basket.GetProduct(i);
                    int basketQuantity = basket.GetQuantity(i);
                    UpdateProductStock(basketProduct.GetArticle(), -basketQuantity);
                }

                newOrder.SaveToFile(); // Записывает заказ в файл orders.txt для истории
                Console.WriteLine("\n=== ЗАКАЗ УСПЕШНО ОФОРМЛЕН И ДОБАВЛЕН В СИСТЕМУ ===");
                newOrder.InfoOrder();
                basket.ClearBasket();
            }
            else
            {
                newOrder.ChangeStatus("Ошибка оплаты");
                Console.WriteLine("Ошибка при оформлении заказа!");
            }
        }

        public void ShowShopInfo()
        {
            Console.WriteLine("\n=== ИНФОРМАЦИЯ О МАГАЗИНЕ ===");
            Console.WriteLine($"Название: {shopName}");
            Console.WriteLine($"Адрес: {addressShop}");
            Console.WriteLine($"Количество товаров в каталоге: {productCount}");
            Console.WriteLine($"Активных заказов: {orderCount}");
            Console.WriteLine($"Заказов у поставщиков: {providerOrderCount}");
            Console.WriteLine($"Отчетов по продажам: {salesReportCount}");
            Console.WriteLine("=================================");
        }

        public void DisplayCatalog()
        {
            Console.WriteLine("\n=== КАТАЛОГ ТОВАРОВ МАГАЗИНА ===");
            if (productCount == 0)
            {
                Console.WriteLine("Каталог пуст!");
                return;
            }

            Console.WriteLine($"Всего товаров: {productCount}");
            for (int i = 0; i < productCount; i++)
            {
                Console.WriteLine($"\n--- Товар {i + 1} ---");
                catalogProducts[i].DisplayInfo();
            }
        }

        public void DisplayOrders()
        {
            Console.WriteLine("\n=== АКТИВНЫЕ ЗАКАЗЫ МАГАЗИНА ===");
            if (orderCount == 0)
            {
                Console.WriteLine("Нет активных заказов!");
                Console.WriteLine("Сначала оформите заказы через режим пользователя.");
                return;
            }

            Console.WriteLine($"Всего активных заказов: {orderCount}");
            for (int i = 0; i < orderCount; i++)
            {
                Console.WriteLine($"\n--- Заказ {i + 1} ---");
                activeOrders[i].InfoOrder();
            }
        }

        public void DisplayProviderOrders()
        {
            Console.WriteLine("\n=== ЗАКАЗЫ У ПОСТАВЩИКОВ ===");
            if (providerOrderCount == 0)
            {
                Console.WriteLine("Нет заказов у поставщиков!");
                Console.WriteLine("Чтобы создать заказ поставщику:");
                Console.WriteLine("1. Оформите заказы через режим пользователя");
                Console.WriteLine("2. Перейдите в Режим администратора -> Заказ у поставщика");
                Console.WriteLine("3. Добавьте заказы клиентов в заказ поставщику");
                return;
            }

            for (int i = 0; i < providerOrderCount; i++)
            {
                Console.WriteLine($"\n--- Заказ поставщику {i + 1} ---");
                Console.WriteLine($"Номер заказа: {providerOrders[i].GetNumberProviderOrder()}");
            }
        }

        public void DisplaySalesReports()
        {
            Console.WriteLine("\n=== ОТЧЕТЫ ПО ПРОДАЖАМ ===");
            if (salesReportCount == 0)
            {
                Console.WriteLine("Нет созданных отчетов!");
                return;
            }

            for (int i = 0; i < salesReportCount; i++)
            {
                Console.WriteLine($"\n--- Отчет {i + 1} ---");
                salesReports[i].DisplayReport();
            }
        }

        public void CreateSalesReport()
        {
            if (salesReportCount >= MAX_SALES_REPORTS)
            {
                Console.WriteLine("Достигнуто максимальное количество отчетов!");
                return;
            }

            SalesReport newReport = new SalesReport();
            newReport.GetReport();
            salesReports[salesReportCount] = newReport;
            salesReportCount++;
        }

        public void RemakeSalesReport()
        {
            if (salesReportCount == 0)
            {
                Console.WriteLine("Нет созданных отчетов для изменения!");
                return;
            }

            DisplaySalesReports();

            Console.Write("Введите номер отчета для изменения: ");
            if (!int.TryParse(Console.ReadLine(), out int reportIndex))
            {
                Console.WriteLine("Неверный номер отчета!");
                return;
            }

            if (reportIndex < 1 || reportIndex > salesReportCount)
            {
                Console.WriteLine("Неверный номер отчета!");
                return;
            }

            salesReports[reportIndex - 1].RemakeReport();
        }

        public void AddProviderOrder(ProviderOrder providerOrder)
        {
            if (providerOrderCount >= MAX_PROVIDER_ORDERS)
            {
                Console.WriteLine("Достигнуто максимальное количество заказов поставщикам!");
                return;
            }

            providerOrders[providerOrderCount] = providerOrder;
            providerOrderCount++;

            Console.WriteLine("Заказ поставщику добавлен в систему магазина!");
        }

        public string GetShopName() { return shopName; }
        public string GetAddress() { return addressShop; }
        public int GetProductCount() { return productCount; }
        public int GetOrderCount() { return orderCount; }

        private int FindProductIndex(int article)
        {
            for (int i = 0; i < productCount; i++)
            {
                if (catalogProducts[i].GetArticle() == article)
                {
                    return i;
                }
            }
            return -1;
        }

        private void UpdateProductStock(int article, int quantityChange)
        {
            int index = FindProductIndex(article);
            if (index != -1)
            {
                // Получаем текущее количество и сохраняем для возможного отката
                uint currentQuantity = catalogProducts[index].GetQuantityStock();

                // Приводим к long для безопасных вычислений (избегаем переполнения)
                long newQuantityLong = (long)currentQuantity + quantityChange;

                // Проверяем, что результат не отрицательный и не превышает uint.MaxValue
                if (newQuantityLong < 0)
                {
                    Console.WriteLine($"Предупреждение: количество товара '{catalogProducts[index].GetName()}' " +
                        $"станет отрицательным ({newQuantityLong}). Операция отменена.");
                    return;
                }

                if (newQuantityLong > uint.MaxValue)
                {
                    Console.WriteLine($"Предупреждение: количество товара '{catalogProducts[index].GetName()}' " +
                        $"превысит максимальное допустимое значение. Операция отменена.");
                    return;
                }

                uint newQuantity = (uint)newQuantityLong;

                // Обновляем количество в памяти
                catalogProducts[index].SetQuantityStock(newQuantity);

                try
                {
                    // Создаем временный файл для безопасной записи
                    string tempFile = Path.GetTempFileName();

                    using (StreamWriter file = new StreamWriter(tempFile, false, Encoding.UTF8))
                    {
                        for (int i = 0; i < productCount; i++)
                        {
                            if (catalogProducts[i] != null)
                            {
                                file.WriteLine($"{catalogProducts[i].GetArticle()}|" +
                                    $"{catalogProducts[i].GetName()}|" +
                                    $"{catalogProducts[i].GetBrand()}|" +
                                    $"{catalogProducts[i].GetProducerCountry()}|" +
                                    $"{catalogProducts[i].GetCategory()}|" +
                                    $"{catalogProducts[i].GetTypeProduct()}|" +
                                    $"{catalogProducts[i].GetDescription()}|" +
                                    $"{catalogProducts[i].GetQuantityStock()}|" +
                                    $"{(catalogProducts[i].GetStatusStock() ? "1" : "0")}|" +
                                    $"{catalogProducts[i].GetPrice()}");
                            }
                        }
                    }

                    // Заменяем оригинальный файл временным
                    File.Copy(tempFile, "products.txt", true);
                    File.Delete(tempFile);

                    Console.WriteLine($"Количество товара '{catalogProducts[index].GetName()}' обновлено. " +
                        $"Было: {currentQuantity}, изменено на: {quantityChange}, стало: {newQuantity}");
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("Ошибка доступа к файлу! Проверьте права доступа к файлу products.txt.");
                    catalogProducts[index].SetQuantityStock(currentQuantity); // Откат изменений
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Ошибка ввода-вывода при сохранении данных: {ex.Message}");
                    catalogProducts[index].SetQuantityStock(currentQuantity); // Откат изменений
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Неожиданная ошибка при сохранении данных: {ex.Message}");
                    catalogProducts[index].SetQuantityStock(currentQuantity); // Откат изменений
                }
            }
            else
            {
                Console.WriteLine($"Товар с артикулом {article} не найден в каталоге!");
            }
        }
    }
}