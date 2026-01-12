using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace MusicShopSystem
{
    public class Shop : IDisposable
    {
        // Константы
        private const int MAX_PRODUCTS = 1000; // максимальное количество товаров в каталоге
        private const int MAX_ORDERS = 1000; // максимальное количество активных заказов
        private const int MAX_PROVIDER_ORDERS = 100; // максимальное количество заказов поставщикам
        private const int MAX_SALES_REPORTS = 50; // максимальное количество отчетов о продажах

        // Поля класса
        private string shopName; // название магазина
        private string addressShop; // адрес магазина
        private Product[] catalogProducts; // массив указателей на товары
        private int productCount; // текущее количество товаров в каталоге
        private Order[] activeOrders; // массив указателей на активные заказы клиентов
        private int orderCount; // текущее количество активных заказов
        private ProviderOrder[] providerOrders; // массив указателей на заказы поставщикам
        private int providerOrderCount; // текущее количество заказов поставщикам
        private SalesReport[] salesReports; // массив указателей на отчеты о продажах
        private int salesReportCount; // текущее количество отчетов
        private bool disposed = false; // Флаг для отслеживания освобождения ресурсов

        // Конструктор по умолчанию
        public Shop()
        {
            this.shopName = "Музыкальный магазин 'MusicLoverzz'";
            this.addressShop = "г. Барнаул, ул. Ленина, 1";
            this.productCount = 0;
            this.orderCount = 0;
            this.providerOrderCount = 0;
            this.salesReportCount = 0;

            this.catalogProducts = new Product[MAX_PRODUCTS];
            this.activeOrders = new Order[MAX_ORDERS];
            this.providerOrders = new ProviderOrder[MAX_PROVIDER_ORDERS];
            this.salesReports = new SalesReport[MAX_SALES_REPORTS];

            this.InitializeArrays();
            this.LoadProductsFromFile("products.txt");
            this.LoadSalesReports();
        }

        // Конструктор с параметрами
        public Shop(string name, string address)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Название магазина не может быть пустым", nameof(name));
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Адрес магазина не может быть пустым", nameof(address));
            }

            this.shopName = name;
            this.addressShop = address;
            this.productCount = 0;
            this.orderCount = 0;
            this.providerOrderCount = 0;
            this.salesReportCount = 0;

            this.catalogProducts = new Product[MAX_PRODUCTS];
            this.activeOrders = new Order[MAX_ORDERS];
            this.providerOrders = new ProviderOrder[MAX_PROVIDER_ORDERS];
            this.salesReports = new SalesReport[MAX_SALES_REPORTS];

            this.InitializeArrays();
            this.LoadProductsFromFile("products.txt");
            this.LoadSalesReports();
        }

        ~Shop()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Проходим по всем элементам
                    foreach (var disposable in this.catalogProducts?.OfType<IDisposable>() ?? Enumerable.Empty<IDisposable>())
                    {
                        disposable.Dispose();
                    }

                    foreach (var disposable in this.activeOrders?.OfType<IDisposable>() ?? Enumerable.Empty<IDisposable>())
                    {
                        disposable.Dispose();
                    }

                    foreach (var disposable in this.providerOrders?.OfType<IDisposable>() ?? Enumerable.Empty<IDisposable>())
                    {
                        disposable.Dispose();
                    }

                    foreach (var disposable in this.salesReports?.OfType<IDisposable>() ?? Enumerable.Empty<IDisposable>())
                    {
                        disposable.Dispose();
                    }

                    // Обнуляем ссылки
                    this.catalogProducts = null;
                    this.activeOrders = null;
                    this.providerOrders = null;
                    this.salesReports = null;
                }

                this.disposed = true;
            }
        }

        // Метод инициализации массивов
        private void InitializeArrays()
        {
            // Инициализируем массив товаров
            for (int i = 0; i < MAX_PRODUCTS; i++)
            {
                this.catalogProducts[i] = null;
            }

            // Инициализируем массив заказов клиентов
            for (int i = 0; i < MAX_ORDERS; i++)
            {
                this.activeOrders[i] = null;
            }

            // Инициализируем массив заказов поставщикам
            for (int i = 0; i < MAX_PROVIDER_ORDERS; i++)
            {
                this.providerOrders[i] = null;
            }

            // Инициализируем массив отчетов о продажах
            for (int i = 0; i < MAX_SALES_REPORTS; i++)
            {
                this.salesReports[i] = null;
            }
        }

        // Метод загрузки товаров из файла
        private void LoadProductsFromFile(string filename)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name, "Объект магазина уже освобожден");
            }

            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentException("Имя файла не может быть пустым", nameof(filename));
            }

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

                    if (this.productCount >= MAX_PRODUCTS)
                    {
                        Console.WriteLine($"Достигнуто максимальное количество товаров ({MAX_PRODUCTS}). Остальные товары не загружены.");
                        break;
                    }

                    string[] tokens = line.Split('|');

                    if (tokens.Length < 10)
                    {
                        throw new FormatException($"Неверный формат строки в файле товаров: {line}");
                    }

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
                        for (int i = 0; i < this.productCount; i++)
                        {
                            if (this.catalogProducts[i] != null && this.catalogProducts[i].GetArticle() == article)
                            {
                                exists = true;
                                break;
                            }
                        }

                        if (!exists)
                        {
                            this.catalogProducts[this.productCount] = new Product(
                                article, name, brand, country, category,
                                type, description, quantity, price
                            );
                            this.productCount++;
                        }
                    }
                    catch (FormatException ex)
                    {
                        throw new FormatException($"Ошибка формата данных в строке: {line}", ex);
                    }
                    catch (OverflowException ex)
                    {
                        throw new OverflowException($"Переполнение числового значения в строке: {line}", ex);
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new IOException($"Нет доступа к файлу товаров: {filename}", ex);
            }
            catch (IOException ex)
            {
                throw new IOException($"Ошибка чтения файла товаров: {filename}", ex);
            }
        }

        // Метод загрузки отчетов по продажам
        private void LoadSalesReports()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name, "Объект магазина уже освобожден");
            }

            // Отчет по продажам за последние 7 дней
            for (int daysAgo = 0; daysAgo < 7 && this.salesReportCount < MAX_SALES_REPORTS; daysAgo++)
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
                            this.salesReports[this.salesReportCount] = report;
                            this.salesReportCount++;
                        }
                    }
                    catch (Exception ex) when (ex is IOException || ex is FormatException)
                    {
                        Console.WriteLine($"Ошибка загрузки отчета {filename}: {ex.Message}");
                    }
                }
            }
        }

        // Метод добавления нового товара в каталог
        public void AddProduct()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name, "Объект магазина уже освобожден");
            }

            if (this.productCount >= MAX_PRODUCTS)
            {
                throw new InvalidOperationException($"Каталог товаров переполнен! Максимум {MAX_PRODUCTS} товаров.");
            }

            Console.WriteLine("\n=== ДОБАВЛЕНИЕ НОВОГО ТОВАРА ===");

            Product newProduct = new Product();
            newProduct.InputFromKeyboard();

            // Проверяем, нет ли уже товара с таким артикулом
            for (int i = 0; i < this.productCount; i++)
            {
                if (this.catalogProducts[i].GetArticle() == newProduct.GetArticle())
                {
                    throw new InvalidOperationException($"Товар с артикулом {newProduct.GetArticle()} уже существует!");
                }
            }

            this.catalogProducts[this.productCount] = newProduct;
            this.productCount++;

            try
            {
                newProduct.WriteToFile("products.txt");
                Console.WriteLine("Товар успешно добавлен в каталог и файл!");
            }
            catch (IOException ex)
            {
                // Откатываем добавление товара при ошибке записи
                this.catalogProducts[this.productCount - 1] = null;
                this.productCount--;
                throw new IOException("Не удалось записать товар в файл", ex);
            }
        }

        // Метод удаления товара из каталога
        public void DeleteProduct()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name, "Объект магазина уже освобожден");
            }

            if (this.productCount == 0)
            {
                throw new InvalidOperationException("Каталог товаров пуст!");
            }

            this.DisplayCatalog();

            Console.Write("Введите артикул товара для удаления: ");

            if (!int.TryParse(Console.ReadLine(), out int articleToDelete))
            {
                throw new FormatException("Неверный формат артикула. Введите целое число.");
            }

            int index = this.FindProductIndex(articleToDelete);

            if (index == -1)
            {
                throw new KeyNotFoundException($"Товар с артикулом {articleToDelete} не найден в каталоге!");
            }

            try
            {
                if (!Product.DeleteProductByArticle("products.txt", articleToDelete))
                {
                    throw new IOException($"Не удалось удалить товар с артикулом {articleToDelete} из файла");
                }

                string productName = this.catalogProducts[index].GetName();

                // Удаляем из массива каталога
                for (int i = index; i < this.productCount - 1; i++)
                {
                    this.catalogProducts[i] = this.catalogProducts[i + 1];
                }

                this.catalogProducts[this.productCount - 1] = null;
                this.productCount--;

                Console.WriteLine($"Товар '{productName}' успешно удален из каталога и файла!");
            }
            catch (IOException ex)
            {
                throw new IOException("Ошибка при удалении товара из файла", ex);
            }
        }

        // Метод создания заказа
        public void CreateOrder(string clientId, Basket basket)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name, "Объект магазина уже освобожден");
            }

            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentException("Идентификатор клиента не может быть пустым", nameof(clientId));
            }

            if (basket == null)
            {
                throw new ArgumentNullException(nameof(basket), "Корзина не может быть null");
            }

            if (this.orderCount >= MAX_ORDERS)
            {
                throw new InvalidOperationException($"Достигнуто максимальное количество активных заказов ({MAX_ORDERS})!");
            }

            if (basket.IsEmpty())
            {
                throw new InvalidOperationException("Корзина пуста! Добавьте товары перед оформлением заказа.");
            }

            // Проверяем наличие товаров на складе
            for (int i = 0; i < basket.GetProductCount(); i++)
            {
                Product basketProduct = basket.GetProduct(i);
                int basketQuantity = basket.GetQuantity(i);

                if (basketProduct == null)
                {
                    throw new InvalidOperationException("В корзине обнаружен null-товар");
                }

                int productIndex = this.FindProductIndex(basketProduct.GetArticle());

                if (productIndex == -1)
                {
                    throw new KeyNotFoundException($"Товар '{basketProduct.GetName()}' не найден в каталоге магазина!");
                }

                if (this.catalogProducts[productIndex].GetQuantityStock() < (uint)basketQuantity)
                {
                    throw new InvalidOperationException(
                        $"Недостаточно товара '{basketProduct.GetName()}' на складе. " +
                        $"Доступно: {this.catalogProducts[productIndex].GetQuantityStock()}, " +
                        $"требуется: {basketQuantity}"
                    );
                }
            }

            Order newOrder = new Order(clientId, basket);

            try
            {
                if (newOrder.Payment())
                {
                    newOrder.ChangeStatus("Оплачен");
                    this.activeOrders[this.orderCount] = newOrder; // добавляем в массив активных заказов
                    this.orderCount++;

                    // Уменьшаем количество товаров на складе
                    for (int i = 0; i < basket.GetProductCount(); i++)
                    {
                        Product basketProduct = basket.GetProduct(i);
                        int basketQuantity = basket.GetQuantity(i);
                        this.UpdateProductStock(basketProduct.GetArticle(), -basketQuantity);
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
                    newOrder.Dispose();
                }
            }
            catch (Exception ex)
            {

                newOrder.Dispose();
                throw new InvalidOperationException("Ошибка при создании заказа", ex);
            }
        }

        // Метод вывода информации о магазине
        public void ShowShopInfo()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name, "Объект магазина уже освобожден");
            }

            Console.WriteLine("\n=== ИНФОРМАЦИЯ О МАГАЗИНЕ ===");
            Console.WriteLine($"Название: {this.shopName}");
            Console.WriteLine($"Адрес: {this.addressShop}");
            Console.WriteLine($"Количество товаров в каталоге: {this.productCount}");
            Console.WriteLine($"Активных заказов: {this.orderCount}");
            Console.WriteLine($"Заказов у поставщиков: {this.providerOrderCount}");
            Console.WriteLine($"Отчетов по продажам: {this.salesReportCount}");
            Console.WriteLine("=================================");
        }

        // Метод отображения каталога товаров
        public void DisplayCatalog()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name, "Объект магазина уже освобожден");
            }

            Console.WriteLine("\n=== КАТАЛОГ ТОВАРОВ МАГАЗИНА ===");

            if (this.productCount == 0)
            {
                Console.WriteLine("Каталог пуст!");
                return;
            }

            Console.WriteLine($"Всего товаров: {this.productCount}");

            for (int i = 0; i < this.productCount; i++)
            {
                Console.WriteLine($"\n--- Товар {i + 1} ---");
                this.catalogProducts[i].DisplayInfo();
            }
        }

        // Метод отображения активных заказов
        public void DisplayOrders()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name, "Объект магазина уже освобожден");
            }

            Console.WriteLine("\n=== АКТИВНЫЕ ЗАКАЗЫ МАГАЗИНА ===");

            if (this.orderCount == 0)
            {
                Console.WriteLine("Нет активных заказов!");
                Console.WriteLine("Сначала оформите заказы через режим пользователя.");
                return;
            }

            Console.WriteLine($"Всего активных заказов: {this.orderCount}");

            for (int i = 0; i < this.orderCount; i++)
            {
                Console.WriteLine($"\n--- Заказ {i + 1} ---");
                this.activeOrders[i].InfoOrder();
            }
        }

        // Метод отображения заказов поставщикам
        public void DisplayProviderOrders()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name, "Объект магазина уже освобожден");
            }

            Console.WriteLine("\n=== ЗАКАЗЫ У ПОСТАВЩИКОВ ===");

            if (this.providerOrderCount == 0)
            {
                Console.WriteLine("Нет заказов у поставщиков!");
                Console.WriteLine("Чтобы создать заказ поставщику:");
                Console.WriteLine("1. Оформите заказы через режим пользователя");
                Console.WriteLine("2. Перейдите в Режим администратора -> Заказ у поставщика");
                Console.WriteLine("3. Добавьте заказы клиентов в заказ поставщику");
                return;
            }

            for (int i = 0; i < this.providerOrderCount; i++)
            {
                Console.WriteLine($"\n--- Заказ поставщику {i + 1} ---");
                Console.WriteLine($"Номер заказа: {this.providerOrders[i].GetNumberProviderOrder()}");
            }
        }

        // Метод отображения отчетов по продажам
        public void DisplaySalesReports()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name, "Объект магазина уже освобожден");
            }

            Console.WriteLine("\n=== ОТЧЕТЫ ПО ПРОДАЖАМ ===");

            if (this.salesReportCount == 0)
            {
                Console.WriteLine("Нет созданных отчетов!");
                return;
            }

            for (int i = 0; i < this.salesReportCount; i++)
            {
                Console.WriteLine($"\n--- Отчет {i + 1} ---");
                this.salesReports[i].DisplayReport();
            }
        }

        // Метод создания нового отчета о продажам
        public void CreateSalesReport()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name, "Объект магазина уже освобожден");
            }

            if (this.salesReportCount >= MAX_SALES_REPORTS)
            {
                throw new InvalidOperationException($"Достигнуто максимальное количество отчетов ({MAX_SALES_REPORTS})!");
            }

            SalesReport newReport = new SalesReport();

            try
            {
                newReport.GetReport();
                this.salesReports[this.salesReportCount] = newReport;
                this.salesReportCount++;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ошибка при создании отчета о продажах", ex);
            }
        }

        // Метод пересчета существующего отчета
        public void RemakeSalesReport()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name, "Объект магазина уже освобожден");
            }

            if (this.salesReportCount == 0)
            {
                throw new InvalidOperationException("Нет созданных отчетов для изменения!");
            }

            this.DisplaySalesReports();

            Console.Write("Введите номер отчета для изменения: ");

            if (!int.TryParse(Console.ReadLine(), out int reportIndex))
            {
                throw new FormatException("Неверный формат номера отчета. Введите целое число.");
            }

            if (reportIndex < 1 || reportIndex > this.salesReportCount)
            {
                throw new ArgumentOutOfRangeException(nameof(reportIndex),
                    $"Неверный номер отчета! Допустимый диапазон: 1-{this.salesReportCount}");
            }

            try
            {
                this.salesReports[reportIndex - 1].RemakeReport();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ошибка при изменении отчета", ex);
            }
        }

        // Метод добавления заказа поставщику в систему магазина
        public void AddProviderOrder(ProviderOrder providerOrder)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name, "Объект магазина уже освобожден");
            }

            if (providerOrder == null)
            {
                throw new ArgumentNullException(nameof(providerOrder), "Заказ поставщика не может быть null");
            }

            if (this.providerOrderCount >= MAX_PROVIDER_ORDERS)
            {
                throw new InvalidOperationException($"Достигнуто максимальное количество заказов поставщикам ({MAX_PROVIDER_ORDERS})!");
            }

            this.providerOrders[this.providerOrderCount] = providerOrder;
            this.providerOrderCount++;

            Console.WriteLine("Заказ поставщику добавлен в систему магазина!");
        }

        // Метод поиска индекса товара по артикулу
        private int FindProductIndex(int article)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name, "Объект магазина уже освобожден");
            }

            for (int i = 0; i < this.productCount; i++)
            {
                if (this.catalogProducts[i].GetArticle() == article)
                {
                    return i;
                }
            }

            return -1;
        }

        // Метод обновления количества товара на складе
        private void UpdateProductStock(int article, int quantityChange)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name, "Объект магазина уже освобожден");
            }

            int index = this.FindProductIndex(article);

            if (index == -1)
            {
                throw new KeyNotFoundException($"Товар с артикулом {article} не найден в каталоге");
            }

            uint currentQuantity = this.catalogProducts[index].GetQuantityStock();

            if (quantityChange < 0 && (uint)(-quantityChange) > currentQuantity)
            {
                throw new InvalidOperationException(
                    $"Нельзя уменьшить количество товара {this.catalogProducts[index].GetName()} " +
                    $"ниже 0. Текущее количество: {currentQuantity}, запрошено уменьшение: {-quantityChange}"
                );
            }

            uint newQuantity = currentQuantity + (uint)quantityChange;
            this.catalogProducts[index].SetQuantityStock(newQuantity);

            // Обновляем файл товаров
            this.UpdateProductsFile();
        }

        // Метод обновления файла товаров
        private void UpdateProductsFile()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name, "Объект магазина уже освобожден");
            }

            try
            {
                using (StreamWriter file = new StreamWriter("products.txt"))
                {
                    for (int i = 0; i < this.productCount; i++)
                    {
                        file.WriteLine($"{this.catalogProducts[i].GetArticle()}|" +
                                      $"{this.catalogProducts[i].GetName()}|" +
                                      $"{this.catalogProducts[i].GetBrand()}|" +
                                      $"{this.catalogProducts[i].GetProducerCountry()}|" +
                                      $"{this.catalogProducts[i].GetCategory()}|" +
                                      $"{this.catalogProducts[i].GetTypeProduct()}|" +
                                      $"{this.catalogProducts[i].GetDescription()}|" +
                                      $"{this.catalogProducts[i].GetQuantityStock()}|" +
                                      $"{(this.catalogProducts[i].GetStatusStock() ? 1 : 0)}|" +
                                      $"{this.catalogProducts[i].GetPrice()}");
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new IOException("Нет доступа к файлу товаров для записи", ex);
            }
            catch (IOException ex)
            {
                throw new IOException("Ошибка обновления файла товаров", ex);
            }
        }

        // Свойства для доступа к полям
        public string ShopName
        {
            get
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException(this.GetType().Name, "Объект магазина уже освобожден");
                }
                return this.shopName;
            }
        }

        public string Address
        {
            get
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException(this.GetType().Name, "Объект магазина уже освобожден");
                }
                return this.addressShop;
            }
        }

        public int ProductCount
        {
            get
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException(this.GetType().Name, "Объект магазина уже освобожден");
                }
                return this.productCount;
            }
        }

        public int OrderCount
        {
            get
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException(this.GetType().Name, "Объект магазина уже освобожден");
                }
                return this.orderCount;
            }
        }

        public string GetShopName()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name, "Объект магазина уже освобожден");
            }
            return this.shopName;
        }

        public string GetAddress()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name, "Объект магазина уже освобожден");
            }
            return this.addressShop;
        }

        public int GetProductCount()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name, "Объект магазина уже освобожден");
            }
            return this.productCount;
        }

        public int GetOrderCount()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name, "Объект магазина уже освобожден");
            }
            return this.orderCount;
        }
    }
}
