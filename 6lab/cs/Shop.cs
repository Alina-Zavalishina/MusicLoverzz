// Shop.cs
using System;
using System.IO;
using System.Collections.Generic;
using MusicStore;

namespace MusicStore
{
    public class Shop : IDisposable
    {
        // Константы
        private const int MAX_PRODUCTS = 1000;          // максимальное количество товаров в каталоге
        private const int MAX_ORDERS = 1000;           // максимальное количество активных заказов
        private const int MAX_PROVIDER_ORDERS = 100;   // максимальное количество заказов поставщикам
        private const int MAX_SALES_REPORTS = 50;      // максимальное количество отчетов о продажах

        // Поля класса
        private string shopName;                       // название магазина
        private string addressShop;                    // адрес магазина
        private Product[] catalogProducts;             // массив указателей на товары
        private int productCount;                      // текущее количество товаров в каталоге
        private Order[] activeOrders;                  // массив указателей на активные заказы клиентов
        private int orderCount;                        // текущее количество активных заказов
        private ProviderOrder[] providerOrders;        // массив указателей на заказы поставщикам
        private int providerOrderCount;                // текущее количество заказов поставщикам
        private SalesReport[] salesReports;            // массив указателей на отчеты о продажах
        private int salesReportCount;                  // текущее количество отчетов

        private bool disposed = false;                 // Флаг для отслеживания освобождения ресурсов

        // Конструктор по умолчанию
        public Shop()
        {
            shopName = "Музыкальный магазин 'MusicLoverzz'";
            addressShop = "г. Барнаул, ул. Ленина, 1";
            productCount = 0;
            orderCount = 0;
            providerOrderCount = 0;
            salesReportCount = 0;

            catalogProducts = new Product[MAX_PRODUCTS];
            activeOrders = new Order[MAX_ORDERS];
            providerOrders = new ProviderOrder[MAX_PROVIDER_ORDERS];
            salesReports = new SalesReport[MAX_SALES_REPORTS];

            InitializeArrays();
            LoadProductsFromFile("products.txt");
            LoadSalesReports();
        }

        // Конструктор с параметрами
        public Shop(string name, string address)
        {
            shopName = name;
            addressShop = address;
            productCount = 0;
            orderCount = 0;
            providerOrderCount = 0;
            salesReportCount = 0;

            catalogProducts = new Product[MAX_PRODUCTS];
            activeOrders = new Order[MAX_ORDERS];
            providerOrders = new ProviderOrder[MAX_PROVIDER_ORDERS];
            salesReports = new SalesReport[MAX_SALES_REPORTS];

            InitializeArrays();
            LoadProductsFromFile("products.txt");
            LoadSalesReports();
        }


        ~Shop()
        {
            Dispose(false);
        }

        // Реализация интерфейса IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Проходим по всем элементам, которые реализуют IDisposable
                    foreach (var disposable in catalogProducts?.OfType<IDisposable>() ?? Enumerable.Empty<IDisposable>())
                    {
                        disposable.Dispose();
                    }

                    foreach (var disposable in activeOrders?.OfType<IDisposable>() ?? Enumerable.Empty<IDisposable>())
                    {
                        disposable.Dispose();
                    }

                    foreach (var disposable in providerOrders?.OfType<IDisposable>() ?? Enumerable.Empty<IDisposable>())
                    {
                        disposable.Dispose();
                    }

                    foreach (var disposable in salesReports?.OfType<IDisposable>() ?? Enumerable.Empty<IDisposable>())
                    {
                        disposable.Dispose();
                    }

                    // Обнуляем ссылки
                    catalogProducts = null;
                    activeOrders = null;
                    providerOrders = null;
                    salesReports = null;
                }

                disposed = true;
            }
        }

        // Метод инициализации массивов
        private void InitializeArrays()
        {
            // Инициализируем массив товаров
            for (int i = 0; i < MAX_PRODUCTS; i++)
            {
                catalogProducts[i] = null;
            }

            // Инициализируем массив заказов клиентов
            for (int i = 0; i < MAX_ORDERS; i++)
            {
                activeOrders[i] = null;
            }

            // Инициализируем массив заказов поставщикам
            for (int i = 0; i < MAX_PROVIDER_ORDERS; i++)
            {
                providerOrders[i] = null;
            }

            // Инициализируем массив отчетов о продажах
            for (int i = 0; i < MAX_SALES_REPORTS; i++)
            {
                salesReports[i] = null;
            }
        }

        // Метод загрузки товаров из файла
        private void LoadProductsFromFile(string filename)
        {
            if (disposed)
                throw new ObjectDisposedException("Shop", "Объект уже освобожден");

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
                    if (string.IsNullOrEmpty(line)) continue;
                    if (productCount >= MAX_PRODUCTS) break;

                    string[] tokens = line.Split('|');
                    if (tokens.Length >= 10)
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
                            uint quantity = uint.Parse(tokens[7]);
                            bool status = tokens[8] == "1";
                            int price = int.Parse(tokens[9]);

                            // Проверяем, находится ли товар уже в каталоге
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
                        catch
                        {
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки товаров: {ex.Message}");
            }
        }

        // Метод загрузки отчетов по продажам
        private void LoadSalesReports()
        {
            if (disposed)
                throw new ObjectDisposedException("Shop", "Объект уже освобожден");

            // Отчет по продажам за последние 7 дней
            for (int daysAgo = 0; daysAgo < 7 && salesReportCount < MAX_SALES_REPORTS; daysAgo++)
            {
                DateTime reportDate = DateTime.Now.AddDays(-daysAgo);
                string filename = "sales_report_" + reportDate.ToString("dd.MM.yyyy") + ".txt";

                if (File.Exists(filename))
                {
                    try
                    {
                        SalesReport report = new SalesReport(reportDate.ToString("dd.MM.yyyy"));
                        if (report.LoadFromFile(filename))
                        {
                            salesReports[salesReportCount] = report;
                            salesReportCount++;
                        }
                    }
                    catch
                    {
                        // Пропускаем нечитаемые отчеты
                    }
                }
            }
        }

        // Метод добавления нового товара в каталог
        public void AddProduct()
        {
            if (disposed)
                throw new ObjectDisposedException("Shop", "Объект уже освобожден");

            if (productCount >= MAX_PRODUCTS)
            {
                Console.WriteLine($"Каталог товаров переполнен! Максимум {MAX_PRODUCTS} товаров.");
                return;
            }

            Console.WriteLine("\n=== ДОБАВЛЕНИЕ НОВОГО ТОВАРА ===");
            Product newProduct = new Product();
            newProduct.InputFromKeyboard();

            // Проверяем, нет ли уже товара с таким артикулом
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

        // Метод удаления товара из каталога
        public void DeleteProduct()
        {
            if (disposed)
                throw new ObjectDisposedException("Shop", "Объект уже освобожден");

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

            // Удаляем из массива каталога
            for (int i = index; i < productCount - 1; i++)
            {
                catalogProducts[i] = catalogProducts[i + 1];
            }

            catalogProducts[productCount - 1] = null;
            productCount--;

            Console.WriteLine($"Товар '{productName}' успешно удален из каталога и файла!");
        }

        // Метод создания заказа
        public void CreateOrder(string clientId, Basket basket)
        {
            if (disposed)
                throw new ObjectDisposedException("Shop", "Объект уже освобожден");

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

            // Проверяем наличие товаров на складе
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

                if (catalogProducts[productIndex].GetQuantityStock() < (uint)basketQuantity)
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

        // Метод вывода информации о магазине
        public void ShowShopInfo()
        {
            if (disposed)
                throw new ObjectDisposedException("Shop", "Объект уже освобожден");

            Console.WriteLine("\n=== ИНФОРМАЦИЯ О МАГАЗИНЕ ===");
            Console.WriteLine($"Название: {shopName}");
            Console.WriteLine($"Адрес: {addressShop}");
            Console.WriteLine($"Количество товаров в каталоге: {productCount}");
            Console.WriteLine($"Активных заказов: {orderCount}");
            Console.WriteLine($"Заказов у поставщиков: {providerOrderCount}");
            Console.WriteLine($"Отчетов по продажам: {salesReportCount}");
            Console.WriteLine("=================================");
        }

        // Метод отображения каталога товаров
        public void DisplayCatalog()
        {
            if (disposed)
                throw new ObjectDisposedException("Shop", "Объект уже освобожден");

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

        // Метод отображения активных заказов
        public void DisplayOrders()
        {
            if (disposed)
                throw new ObjectDisposedException("Shop", "Объект уже освобожден");

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

        // Метод отображения заказов поставщикам
        public void DisplayProviderOrders()
        {
            if (disposed)
                throw new ObjectDisposedException("Shop", "Объект уже освобожден");

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

        // Метод отображения отчетов по продажам
        public void DisplaySalesReports()
        {
            if (disposed)
                throw new ObjectDisposedException("Shop", "Объект уже освобожден");

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

        // Метод создания нового отчета о продажах
        public void CreateSalesReport()
        {
            if (disposed)
                throw new ObjectDisposedException("Shop", "Объект уже освобожден");

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

        // Метод пересчета существующего отчета
        public void RemakeSalesReport()
        {
            if (disposed)
                throw new ObjectDisposedException("Shop", "Объект уже освобожден");

            if (salesReportCount == 0)
            {
                Console.WriteLine("Нет созданных отчетов для изменения!");
                return;
            }

            DisplaySalesReports();

            Console.Write("Введите номер отчета для изменения: ");
            if (!int.TryParse(Console.ReadLine(), out int reportIndex))
            {
                Console.WriteLine("Неверный ввод!");
                return;
            }

            if (reportIndex < 1 || reportIndex > salesReportCount)
            {
                Console.WriteLine("Неверный номер отчета!");
                return;
            }

            salesReports[reportIndex - 1].RemakeReport();
        }

        // Метод добавления заказа поставщику в систему магазина
        public void AddProviderOrder(ProviderOrder providerOrder)
        {
            if (disposed)
                throw new ObjectDisposedException("Shop", "Объект уже освобожден");

            if (providerOrderCount >= MAX_PROVIDER_ORDERS)
            {
                Console.WriteLine("Достигнуто максимальное количество заказов поставщикам!");
                return;
            }

            providerOrders[providerOrderCount] = providerOrder;
            providerOrderCount++;
            Console.WriteLine("Заказ поставщику добавлен в систему магазина!");
        }

        // Метод поиска индекса товара по артикулу
        private int FindProductIndex(int article)
        {
            if (disposed)
                throw new ObjectDisposedException("Shop", "Объект уже освобожден");

            for (int i = 0; i < productCount; i++)
            {
                if (catalogProducts[i].GetArticle() == article)
                {
                    return i;
                }
            }
            return -1;
        }

        // Метод обновления количества товара на складе
        private void UpdateProductStock(int article, int quantityChange)
        {
            if (disposed)
                throw new ObjectDisposedException("Shop", "Объект уже освобожден");

            int index = FindProductIndex(article);
            if (index != -1)
            {
                uint newQuantity = catalogProducts[index].GetQuantityStock() + (uint)quantityChange;
                catalogProducts[index].SetQuantityStock(newQuantity);

                // Обновляем файл товаров
                UpdateProductsFile();
            }
        }

        // Метод обновления файла товаров
        private void UpdateProductsFile()
        {
            if (disposed)
                throw new ObjectDisposedException("Shop", "Объект уже освобожден");

            try
            {
                using (StreamWriter file = new StreamWriter("products.txt"))
                {
                    for (int i = 0; i < productCount; i++)
                    {
                        file.WriteLine($"{catalogProducts[i].GetArticle()}|" +
                                     $"{catalogProducts[i].GetName()}|" +
                                     $"{catalogProducts[i].GetBrand()}|" +
                                     $"{catalogProducts[i].GetProducerCountry()}|" +
                                     $"{catalogProducts[i].GetCategory()}|" +
                                     $"{catalogProducts[i].GetTypeProduct()}|" +
                                     $"{catalogProducts[i].GetDescription()}|" +
                                     $"{catalogProducts[i].GetQuantityStock()}|" +
                                     $"{(catalogProducts[i].GetStatusStock() ? 1 : 0)}|" +
                                     $"{catalogProducts[i].GetPrice()}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обновления файла товаров: {ex.Message}");
            }
        }

        // Свойства для доступа к полям
        public string ShopName
        {
            get
            {
                if (disposed)
                    throw new ObjectDisposedException("Shop", "Объект уже освобожден");
                return shopName;
            }
        }

        public string Address
        {
            get
            {
                if (disposed)
                    throw new ObjectDisposedException("Shop", "Объект уже освобожден");
                return addressShop;
            }
        }

        public int ProductCount
        {
            get
            {
                if (disposed)
                    throw new ObjectDisposedException("Shop", "Объект уже освобожден");
                return productCount;
            }
        }

        public int OrderCount
        {
            get
            {
                if (disposed)
                    throw new ObjectDisposedException("Shop", "Объект уже освобожден");
                return orderCount;
            }
        }


        public string GetShopName()
        {
            if (disposed)
                throw new ObjectDisposedException("Shop", "Объект уже освобожден");
            return shopName;
        }

        public string GetAddress()
        {
            if (disposed)
                throw new ObjectDisposedException("Shop", "Объект уже освобожден");
            return addressShop;
        }

        public int GetProductCount()
        {
            if (disposed)
                throw new ObjectDisposedException("Shop", "Объект уже освобожден");
            return productCount;
        }

        public int GetOrderCount()
        {
            if (disposed)
                throw new ObjectDisposedException("Shop", "Объект уже освобожден");
            return orderCount;
        }
    }
}