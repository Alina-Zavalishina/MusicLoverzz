using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace MusicShopSystem
{
    class Program
    {

        private static Basket userBasket = new Basket();// Корзина текущего пользователя
        //магазин с названием и адресом
        private static Shop musicShop = new Shop("Музыкальный магазин 'MusicLoverzz'", "г. Барнаул, ул. Ленина, 1");
        private static Client currentClient = new Client("USER001"); // Текущий клиент системы

        static void Main(string[] args)//запуск при старте программы
        {
            // Установка русской локали
            CultureInfo.CurrentCulture = new CultureInfo("ru-RU");
            CultureInfo.CurrentUICulture = new CultureInfo("ru-RU");
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            int choice; // Переменная для хранения выбора пользователя
            do
            {
                Console.WriteLine("\n=== ДОБРО ПОЖАЛОВАТЬ В 'MUSICLOVERZZ'! ===");
                Console.WriteLine("Выберите режим работы:");
                Console.WriteLine("1. Режим администратора");
                Console.WriteLine("2. Режим пользователя");
                Console.WriteLine("3. Управление магазином");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите действие: ");

                // Чтение и валидация ввода пользователя
                // TryParse возвращает false если ввод не является числом
                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Неверный ввод! Попробуйте снова.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        AdminMode();// Запуск режима администратора
                        break;
                    case 2:
                        UserMode();// Запуск режима пользователя
                        break;
                    case 3:
                        ShopManagementMode();// Запуск управления магазином
                        break;
                    case 0:
                        Console.WriteLine("Выход из программы...");
                        break;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
            } while (choice != 0);
        }

        // Метод проверки пароля администратора
        private static bool CheckAdminPassword()
        {
            Console.Write("Введите пароль администратора: ");
            string password = Console.ReadLine();// Чтение пароля из консоли


            if (password == "26102006")
            {
                Console.WriteLine("Доступ разрешен!");
                return true;
            }
            else
            {
                Console.WriteLine("Неверный пароль!");
                return false;
            }
        }

        // Режим администратора
        private static void AdminMode()
        {
            if (!CheckAdminPassword())
            {
                return;// Выход если пароль неверный
            }

            int choice;// Переменная для выбора в меню администратора
            do
            {
                Console.WriteLine("\n=== РЕЖИМ АДМИНИСТРАТОРА ===");
                Console.WriteLine("1. Добавить товар");
                Console.WriteLine("2. Показать информацию о товарах");
                Console.WriteLine("3. Удалить товары со склада");
                Console.WriteLine("4. Редактировать товары");
                Console.WriteLine("5. Найти товары по артикулу");
                Console.WriteLine("6. Заказ у поставщика");
                Console.WriteLine("0. Выход в главное меню");
                Console.Write("Выберите действие: ");

                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Неверный ввод!");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        AddProducts();// Добавление новых товаров
                        break;
                    case 2:
                        Product.DisplayProductsTable("products.txt");//метод для отображения таблицы товаров
                        break;
                    case 3:
                        Product.DeleteProductsFromFile("products.txt");//удаление товаров
                        break;
                    case 4:
                        Product.EditProductsInFile("products.txt");//редактирование
                        break;
                    case 5:
                        Product.FindProductsByArticle("products.txt");//поиск
                        break;
                    case 6:
                        ProviderOrderMode();//заказ у поставщика
                        break;
                    case 0:
                        Console.WriteLine("Выход из режима администратора...");
                        break;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
            } while (choice != 0);
        }

        // Заказ товаров у поставщиков
        private static void ProviderOrderMode()
        {
            int choice;//номер выбора с клавиатуры
            ProviderOrder providerOrder = new ProviderOrder();// Создание объекта для работы с заказами поставщикам


            do
            {
                Console.WriteLine("\n=== ЗАКАЗ У ПОСТАВЩИКА ===");
                Console.WriteLine("1. Добавить товар в заказ (из заказов клиентов)");
                Console.WriteLine("2. Отправить заказ поставщику");
                Console.WriteLine("3. Обновить информацию о товарах");
                Console.WriteLine("0. Назад");
                Console.Write("Выберите действие: ");

                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Неверный ввод!");
                    continue;// Возврат к началу цикла при некорректном вводе
                }

                switch (choice)
                {
                    case 1:
                        providerOrder.AddProvider();// Добавление товара в заказ поставщику на основе заказов клиентов
                        break;
                    case 2:
                        providerOrder.Sending();// Отправка сформированного заказа поставщику
                        break;
                    case 3:
                        providerOrder.NewInfoStock();// Обновление информации о товарах на складе после получения заказа
                        break;
                    case 0:
                        Console.WriteLine("Выход из режима заказа поставщика...");
                        break;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
            } while (choice != 0);
        }

        // Режим пользователя
        private static void UserMode()
        {
            int choice;// Переменная для выбора действия в пользовательском меню
            do
            {
                Console.WriteLine("\n=== РЕЖИМ ПОЛЬЗОВАТЕЛЯ ===");
                Console.WriteLine("1. Поиск товаров");
                Console.WriteLine("2. Корзина товаров");
                Console.WriteLine("3. Оформить заказ");
                Console.WriteLine("4. Оставить отзыв");
                Console.WriteLine("0. Выход в главное меню");
                Console.Write("Выберите действие: ");

                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Неверный ввод!");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        UserSearchMode();// Переход в режим поиска товаров
                        break;
                    case 2:
                        UserBasketMode(); // Переход в режим работы с корзиной
                        break;
                    case 3:
                        UserOrderMode();// Переход к оформлению заказа
                        break;
                    case 4:
                        currentClient.GetReview();// Вызов метода оставления отзыва у текущего клиента
                        break;
                    case 0:
                        Console.WriteLine("Выход из режима пользователя...");
                        break;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
            } while (choice != 0);
        }

        // Режим поиска товаров
        private static void UserSearchMode()
        {
            int choice;// Переменная для выбора типа поиска
            Search search = new Search();

            do
            {
                Console.WriteLine("\n=== РЕЖИМ ПОИСКА ===");
                Console.WriteLine("1. Поиск по названию");
                Console.WriteLine("2. Поиск по артикулу");
                Console.WriteLine("3. Поиск по бренду");
                Console.WriteLine("4. Поиск по стране-производителю");
                Console.WriteLine("5. Поиск по категории");
                Console.WriteLine("6. Поиск по типу товара");
                Console.WriteLine("7. Поиск по цене");
                Console.WriteLine("8. Универсальный поиск");
                Console.WriteLine("9. Показать историю поиска");
                Console.WriteLine("0. Назад");
                Console.Write("Выберите действие: ");

                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Неверный ввод! Пожалуйста, введите число от 0 до 9.");
                    continue;
                }

                string searchString;// Для текстовых критериев поиска
                // Для поиска по артикулу (целое число)
                // Для поиска по диапазону цен
                int searchArticle, minPrice, maxPrice;

                switch (choice)
                {
                    case 1:
                        Console.Write("Введите название для поиска: ");
                        searchString = Console.ReadLine();// Чтение строки поиска
                        search.SearchByName("products.txt", searchString);// Выполнение поиска
                        search.DisplayResults();// Отображение результатов
                        break;
                    case 2:// Поиск по артикулу (уникальному идентификатору)
                        // Цикл с проверкой корректности ввода артикула
                        while (true)
                        {
                            Console.Write("Введите артикул для поиска: ");
                            if (int.TryParse(Console.ReadLine(), out searchArticle))// Проверка что введено число
                            {
                                break;
                            }
                            Console.WriteLine("Неверный ввод! Пожалуйста, введите целое число.");
                        }
                        search.SearchByArticle("products.txt", searchArticle);
                        search.DisplayResults();
                        break;
                    case 3: // Поиск по бренду производителя
                        Console.Write("Введите бренд для поиска: ");
                        searchString = Console.ReadLine();
                        search.SearchByBrand("products.txt", searchString);
                        search.DisplayResults();
                        break;
                    case 4:// Поиск по стране производства
                        Console.Write("Введите страну-производитель для поиска: ");
                        searchString = Console.ReadLine();
                        search.SearchByCountry("products.txt", searchString);
                        search.DisplayResults();
                        break;
                    case 5:// Поиск по категории товара
                        Console.Write("Введите категорию для поиска: ");
                        searchString = Console.ReadLine();
                        search.SearchByCategory("products.txt", searchString);
                        search.DisplayResults();
                        break;
                    case 6:// Поиск по типу товара
                        Console.Write("Введите тип товара для поиска: ");
                        searchString = Console.ReadLine();
                        search.SearchByType("products.txt", searchString);
                        search.DisplayResults();
                        break;
                    case 7:// Поиск по диапазону цен
                        while (true)
                        {
                            Console.Write("Введите минимальную цену: ");
                            if (int.TryParse(Console.ReadLine(), out minPrice))
                            {
                                break;
                            }
                            Console.WriteLine("Неверный ввод! Пожалуйста, введите целое число.");
                        }
                        while (true)
                        {
                            Console.Write("Введите максимальную цену: ");
                            if (int.TryParse(Console.ReadLine(), out maxPrice))
                            {
                                break;
                            }
                            Console.WriteLine("Неверный ввод! Пожалуйста, введите целое число.");
                        }
                        search.SearchByPrice("products.txt", minPrice, maxPrice);
                        search.DisplayResults();
                        break;
                    case 8:// Универсальный поиск
                        Search.UniversalSearch("products.txt");
                        break;
                    case 9:// Просмотр истории поисковых запросов
                        search.DisplaySearchHistory();
                        break;
                    case 0:
                        Console.WriteLine("Выход из режима поиска...");
                        break;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
            } while (choice != 0);
        }

        // Режим работы с корзиной
        private static void UserBasketMode()
        {
            int choice;
            do
            {
                Console.WriteLine("\n=== КОРЗИНА ТОВАРОВ ===");
                Console.WriteLine("1. Посмотреть цену товаров в корзине");
                Console.WriteLine("2. Количество товаров в корзине");
                Console.WriteLine("3. Показать содержимое корзины");
                Console.WriteLine("4. Добавить товар в корзину по артикулу");
                Console.WriteLine("5. Удалить товар из корзины");
                Console.WriteLine("6. Изменить количество товара в корзине");
                Console.WriteLine("7. Очистить корзину");
                Console.WriteLine("0. Назад");
                Console.Write("Выберите действие: ");

                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Неверный ввод!");
                    continue;
                }

                switch (choice)
                {
                    case 1:// Просмотр общей стоимости корзины
                        double totalCost = userBasket.GetBasketCost();//получение стоимости всей корзины
                        Console.WriteLine("\n=== ОБЩАЯ СТОИМОСТЬ КОРЗИНЫ ===");
                        Console.WriteLine($"Общая стоимость: {totalCost:F2} руб.");
                        Console.WriteLine("===============================");
                        break;
                    case 2:// Просмотр общего количества товаров в корзине
                        uint totalCount = userBasket.GetBasketCount();
                        Console.WriteLine("\n=== КОЛИЧЕСТВО ТОВАРОВ В КОРЗИНЕ ===");
                        Console.WriteLine($"Общее количество товаров: {totalCount} шт.");
                        Console.WriteLine("====================================");
                        break;
                    case 3:// Отображение подробного содержимого корзины
                        userBasket.DisplayBasket();
                        break;
                    case 4:// Добавление нового товара в корзину по артикулу
                        AddProductToBasketByArticle(userBasket);
                        break;
                    case 5: // Удаление товара из корзины
                        if (userBasket.IsEmpty())// Проверка на пустую корзину
                        {
                            Console.WriteLine("Корзина пуста!");
                            break;
                        }
                        userBasket.DisplayBasket();// Показ содержимого для выбора
                        Console.Write("Введите номер товара для удаления: ");
                        if (int.TryParse(Console.ReadLine(), out int productIndex))// Чтение номера
                        {
                            userBasket.DeleteBasket(productIndex);// Удаление выбранного товара
                        }
                        else
                        {
                            Console.WriteLine("Неверный ввод!");
                        }
                        break;
                    case 6:// Изменение количества конкретного товара в корзине
                        if (userBasket.IsEmpty())
                        {
                            Console.WriteLine("Корзина пуста!");
                            break;
                        }
                        userBasket.DisplayBasket();
                        Console.Write("Введите номер товара для изменения: ");
                        if (!int.TryParse(Console.ReadLine(), out int index))
                        {
                            Console.WriteLine("Неверный ввод!");
                            break;
                        }
                        Console.Write("Введите новое количество: ");
                        if (!int.TryParse(Console.ReadLine(), out int newQuantity))// Чтение нового количества
                        {
                            Console.WriteLine("Неверный ввод!");
                            break;
                        }
                        userBasket.UpdateQuantity(index, newQuantity); // Обновление количества
                        break;
                    case 7:// Полная очистка корзины
                        userBasket.ClearBasket();
                        Console.WriteLine("Корзина очищена!");
                        break;
                    case 0:
                        Console.WriteLine("Выход из режима корзины...");
                        break;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
            } while (choice != 0);
        }

        // Оформление заказа
        private static void UserOrderMode()
        {
            Console.WriteLine("\n=== ОФОРМЛЕНИЕ ЗАКАЗА ===");
            // Проверка, не пуста ли корзина
            // Если корзина пуста, оформление заказа невозможно
            if (userBasket.IsEmpty())
            {
                Console.WriteLine("Корзина пуста! Добавьте товары перед оформлением заказа.");
                Console.WriteLine("Перейдите в раздел 'Корзина товаров' для добавления товаров.");
                return;
            }


            Console.WriteLine("\nСодержимое вашей корзины:");
            userBasket.DisplayBasket();// Вывод списка товаров с ценами и количеством

            // Подтверждение оформления заказа
            Console.Write("\nВы уверены, что хотите оформить заказ? (y/n): ");
            string confirmation = Console.ReadLine().ToLower();

            if (confirmation != "y")// Если пользователь не подтвердил
            {
                Console.WriteLine("Оформление заказа отменено.");
                return;// Прерывание оформления
            }

            string clientId = "USER001";

            // создание копии корзины
            Basket tempBasket = new Basket(); // Новая пустая корзина
            for (int i = 0; i < userBasket.GetProductCount(); i++)// Перебор всех товаров
            {
                Product product = userBasket.GetProduct(i);// Получение товара по индексу
                int quantity = userBasket.GetQuantity(i);// Получение количества товара
                if (product != null)
                {
                    tempBasket.AddBasket(product, quantity);// Добавление копии в временную корзину
                }
            }

            // создание объекта заказа
            Order clientOrder = new Order(clientId, tempBasket);

            // оформление заказа
            musicShop.CreateOrder(clientId, userBasket);

            // сохранение заказа в истории
            currentClient.GetOrderHistory(clientOrder);

            Console.WriteLine("\n==================================");
            Console.WriteLine("ЗАКАЗ УСПЕШНО ОФОРМЛЕН!");
            Console.WriteLine($"Номер заказа: {clientOrder.GetNumberOrder()}");// Уникальный номер заказа
            Console.WriteLine($"Дата: {clientOrder.GetDateOrder()}");// Дата оформления
            Console.WriteLine($"Сумма: {clientOrder.GetCostOrder():F2} руб.");// Общая сумма
            Console.WriteLine($"Количество товаров: {clientOrder.GetCountOrder()} шт.");// Общее количество
            Console.WriteLine("==================================");

            // показ состава заказа
            Console.WriteLine("\nСостав заказа:");
            Console.WriteLine("---------------------");
            for (int i = 0; i < clientOrder.GetItemCount(); i++)// Перебор всех позиций заказа
            {
                Product product = clientOrder.GetOrderItem(i);// Получение товара
                int quantity = clientOrder.GetQuantity(i);// Получение количества
                if (product != null) // Проверка наличия товара
                {
                    // Вывод информации о позиции заказа
                    Console.WriteLine($"- {product.GetName()} (арт. {product.GetArticle()}) - {quantity} шт. x {product.GetPrice()} руб.");
                }
            }

            Console.WriteLine("\nТеперь вы можете оставить отзыв о товарах");
            Console.WriteLine("в разделе 'Оставить отзыв'.");
        }

        // Управление магазином
        private static void ShopManagementMode()
        {
            if (!CheckAdminPassword())
            {
                return;
            }

            int choice;
            do
            {
                Console.WriteLine("\n=== УПРАВЛЕНИЕ МАГАЗИНОМ ===");
                Console.WriteLine("1. Информация о магазине");
                Console.WriteLine("2. Показать каталог товаров");
                Console.WriteLine("3. Добавить товар в каталог");
                Console.WriteLine("4. Удалить товар из каталог");
                Console.WriteLine("5. Показать активные заказы");
                Console.WriteLine("6. Показать заказы поставщикам");
                Console.WriteLine("7. Создать отчет по продажам");
                Console.WriteLine("8. Изменить отчет по продажам");
                Console.WriteLine("9. Показать отчеты по продажам");
                Console.WriteLine("10. Показать все отзывы");
                Console.WriteLine("11. Расчет средней оценки товара");
                Console.WriteLine("0. Назад");
                Console.Write("Выберите действие: ");

                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Неверный ввод!");
                    continue;
                }

                switch (choice)
                {
                    case 1:// Просмотр основной информации о магазине
                        musicShop.ShowShopInfo();
                        break;
                    case 2:
                        musicShop.DisplayCatalog();// Отображение полного каталога товаров
                        break;
                    case 3:
                        musicShop.AddProduct();// Добавление нового товара в каталог
                        break;
                    case 4:
                        musicShop.DeleteProduct();// Удаление товара из каталога
                        break;
                    case 5:
                        musicShop.DisplayOrders(); // Просмотр активных заказов клиентов
                        break;
                    case 6:
                        musicShop.DisplayProviderOrders(); // Просмотр заказов поставщикам на пополнение склада
                        break;
                    case 7:
                        musicShop.CreateSalesReport(); // Создание нового отчета о продажах
                        break;
                    case 8:
                        musicShop.RemakeSalesReport();// Редактирование существующего отчета о продажах
                        break;
                    case 9:
                        musicShop.DisplaySalesReports();// Просмотр всех созданных отчетов о продажах
                        break;
                    case 10:
                        Review.PublicReview();// Просмотр всех отзывов о товарах
                        break;
                    case 11:// Расчет средней оценки конкретного товара
                        Console.WriteLine("\n=== РАСЧЕТ СРЕДНЕЙ ОЦЕНКИ ТОВАРА ===");
                        Console.Write("Введите название товара: ");
                        Console.ReadLine(); // Очищаем буфер
                        string productName = Console.ReadLine();// Чтение названия товара
                        Review.AverageRating(productName);// Вызов метода расчета средней оценки
                        break;
                    case 0:
                        Console.WriteLine("Выход из управления магазином...");
                        break;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
            } while (choice != 0);
        }

        // Добавление товаров
        private static void AddProducts()
        {
            Console.WriteLine("\n--- ДОБАВЛЕНИЕ ТОВАРОВ ---");
            int productCount;// Переменная для хранения количества добавляемых товаров

            //проверка ввода
            while (true)
            {
                Console.Write("Введите количество товаров для добавления: ");
                if (!int.TryParse(Console.ReadLine(), out productCount))
                {
                    Console.WriteLine("Ошибка! Введите целое число.");
                    continue;
                }

                if (productCount < 0)
                {
                    Console.WriteLine("Ошибка! Количество не может быть отрицательным. Попробуйте снова.");
                    continue;
                }

                if (productCount == 0)
                {
                    Console.WriteLine("Добавление товаров отменено.");
                    return;
                }

                if (!Product.ValidateNumber(productCount, 10))
                {
                    Console.WriteLine("Ошибка! Количество товаров не должно превышать 10 цифр.");
                    continue;
                }

                break;
            }
            // Цикл добавления указанного количества товаров
            for (int i = 0; i < productCount; i++)
            {
                Console.WriteLine($"\n=== Товар {i + 1} из {productCount} ===");
                Product product = new Product();// Создание нового объекта товара
                product.InputFromKeyboard();// Вызов метода для ввода данных о товаре с клавиатуры
                Console.WriteLine("\nТовар успешно добавлен!");// Подтверждение успешного добавления
                product.DisplayInfo();// Отображение информации о добавленном товаре
                product.WriteToFile("products.txt");
                Console.WriteLine("-------------------------------------------");
            }
            // Итоговое сообщение о количестве добавленных товаров
            Console.WriteLine($"\nУспешно добавлено {productCount} товаров!");
        }

        // Добавление товара в корзину по артикулу
        private static void AddProductToBasketByArticle(Basket basket)
        {
            Console.Write("Введите артикул товара: ");
            if (!int.TryParse(Console.ReadLine(), out int article))// Преобразование ввода в число
            {
                Console.WriteLine("Неверный ввод!");
                return;
            }

            Console.Write("Введите количество: ");// Запрос количества товара
            if (!int.TryParse(Console.ReadLine(), out int quantity))
            {
                Console.WriteLine("Неверный ввод!");
                return;
            }

            if (quantity <= 0)// Проверка что количество положительное
            {
                Console.WriteLine("Количество должно быть положительным числом!");
                return;
            }

            try
            {// Использование StreamReader для чтения файла
                using (StreamReader file = new StreamReader("products.txt"))
                {
                    string line; // Переменная для хранения прочитанной строки
                    bool found = false;// Флаг для отслеживания найден ли товар

                    while ((line = file.ReadLine()) != null)
                    {
                        if (string.IsNullOrEmpty(line)) continue;

                        string[] tokens = line.Split('|'); // Разделение строки на части по разделителю '|'
                        if (tokens.Length >= 10)
                        {
                            if (int.TryParse(tokens[0], out int currentArticle) && currentArticle == article)
                            {
                                found = true;// Товар найден
                                             // Создание объекта Product на основе данных из файла
                                Product product = new Product(
                                    currentArticle,// Артикул
                                    tokens[1],// Название
                                    tokens[2],// Бренд
                                    tokens[3],// Страна-производитель
                                    tokens[4],// Категория
                                    tokens[5],// Тип товара
                                    tokens[6],// Описание
                                    uint.Parse(tokens[7]),// Количество на складе
                                    int.Parse(tokens[9])// Цена
                                );
                                basket.AddBasket(product, quantity);// Добавление товара в корзину
                                Console.WriteLine($"Товар '{tokens[1]}' добавлен в корзину!");
                                break;
                            }
                        }
                    }

                    if (!found)// Если товар не был найден после прочтения всего файла
                    {
                        Console.WriteLine($"Товар с артикулом {article} не найден!");
                    }
                }
            }
            catch (FileNotFoundException)// Обработка исключения если файл не найден
            {
                Console.WriteLine("Файл с товарами не найден!");
            }
            catch (Exception ex)// Обработка любых других исключений
            {
                Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
            }
        }
    }
}