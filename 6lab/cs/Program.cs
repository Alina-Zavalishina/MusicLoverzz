using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MusicStore
{
    class Program
    {
        // Глобальные переменные
        private static Basket userBasket = new Basket();
        private static Shop musicShop = new Shop("Музыкальный магазин 'MusicLoverzz'", "г. Барнаул, ул. Ленина, 1");
        private static Client currentClient = null;
        private static Admin currentAdmin = null;

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("===============================================");
            Console.WriteLine("   МУЗЫКАЛЬНЫЙ МАГАЗИН 'MUSICLOVERZZ'   ");
            Console.WriteLine("===============================================");

            LoginMenu();

            int choice;
            do
            {
                Console.WriteLine("\n=== ГЛАВНОЕ МЕНЮ ===");
                Console.WriteLine("1. Режим администратора");
                Console.WriteLine("2. Режим пользователя");
                Console.WriteLine("3. Управление магазином");
                Console.WriteLine("4. Сменить пользователя");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите действие: ");

                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Неверный ввод! Введите число.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        AdminMode();
                        break;
                    case 2:
                        UserMode();
                        break;
                    case 3:
                        ShopManagementMode();
                        break;
                    case 4:
                        LoginMenu();
                        break;
                    case 0:
                        Console.WriteLine("Выход из программы...");
                        currentClient = null;
                        currentAdmin = null;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
            } while (choice != 0);
        }

        // Функция проверки пароля администратора
        private static bool CheckAdminPassword()
        {
            Console.Write("Введите пароль администратора: ");
            string password = Console.ReadLine();

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

        // Меню входа/регистрации
        private static void LoginMenu()
        {
            int choice;
            do
            {
                Console.WriteLine("\n=== СИСТЕМА АВТОРИЗАЦИИ ===");
                Console.WriteLine("1. Войти как клиент");
                Console.WriteLine("2. Войти как администратор");
                Console.WriteLine("3. Зарегистрировать нового клиента");
                Console.WriteLine("4. Продолжить без авторизации");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите действие: ");

                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Неверный ввод!");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        Console.Write("Введите ID клиента (или нажмите Enter для нового): ");
                        string clientId = Console.ReadLine();

                        if (string.IsNullOrEmpty(clientId))
                        {
                            Console.WriteLine("\nКлиент не найден. Зарегистрируйтесь.");
                            RegisterNewClient();
                        }
                        else
                        {
                            Console.Write("Введите ваше имя: ");
                            string name = Console.ReadLine();

                            currentClient = new Client(clientId, name, "", "", "");
                            Console.WriteLine($"\nВход выполнен как клиент: {name} (ID: {clientId})");
                        }
                        return;
                    case 2:
                        if (CheckAdminPassword())
                        {
                            currentAdmin = new Admin("Главный Админ");
                            Console.WriteLine("\nВход выполнен как администратор");
                        }
                        return;
                    case 3:
                        RegisterNewClient();
                        return;
                    case 4:
                        Console.WriteLine("\nПродолжение без авторизации.");
                        Console.WriteLine("Некоторые функции могут быть ограничены.");
                        return;
                    case 0:
                        Console.WriteLine("Выход из программы...");
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
            } while (choice != 0);
        }

        // Регистрация нового клиента
        private static void RegisterNewClient()
        {
            Console.WriteLine("\n=== РЕГИСТРАЦИЯ НОВОГО КЛИЕНТА ===");

            Console.Write("Введите имя: ");
            string name = Console.ReadLine();

            Console.Write("Введите email: ");
            string email = Console.ReadLine();

            Console.Write("Введите телефон: ");
            string phone = Console.ReadLine();

            Console.Write("Введите адрес: ");
            string address = Console.ReadLine();

            currentClient = new Client(name, email, phone, address);

            Console.WriteLine("\n=== РЕГИСТРАЦИЯ УСПЕШНА! ===");
            Console.WriteLine($"Ваш ID: {currentClient.Id}");
            Console.WriteLine($"Добро пожаловать, {currentClient.Name}!");
        }

        // Режим администратора
        private static void AdminMode()
        {
            if (currentAdmin == null)
            {
                Console.WriteLine("\nСначала выполните вход как администратор!");
                return;
            }

            if (!CheckAdminPassword())
            {
                return;
            }

            int choice;
            do
            {
                Console.WriteLine("\n=== РЕЖИМ АДМИНИСТРАТОРА ===");
                Console.WriteLine("1. Добавить товар");
                Console.WriteLine("2. Показать информацию о товарах");
                Console.WriteLine("3. Удалить товары со склада");
                Console.WriteLine("4. Редактировать товары");
                Console.WriteLine("5. Найти товары по артикулу");
                Console.WriteLine("6. Заказ у поставщика");
                Console.WriteLine("7. Показать информацию администратора");
                Console.WriteLine("8. Управление правами доступа");
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
                        AddProducts();
                        break;
                    case 2:
                        Product.DisplayProductsTable("products.txt");
                        break;
                    case 3:
                        Product.DeleteProductsFromFile("products.txt");
                        break;
                    case 4:
                        Product.EditProductsInFile("products.txt");
                        break;
                    case 5:
                        Product.FindProductsByArticle("products.txt");
                        break;
                    case 6:
                        ProviderOrderMode();
                        break;
                    case 7:
                        if (currentAdmin != null)
                        {
                            currentAdmin.DisplayInfo();
                            currentAdmin.ShowPermissions();
                        }
                        break;
                    case 8:
                        if (currentAdmin != null)
                        {
                            Console.WriteLine("\n=== УПРАВЛЕНИЕ ПРАВАМИ ДОСТУПА ===");
                            Console.WriteLine("1. Добавить право доступа");
                            Console.WriteLine("2. Удалить право доступа");
                            Console.WriteLine("3. Показать все права");
                            Console.Write("Ваш выбор: ");

                            if (int.TryParse(Console.ReadLine(), out int permChoice))
                            {
                                if (permChoice == 1)
                                {
                                    Console.Write("Введите право доступа: ");
                                    string perm = Console.ReadLine();
                                    currentAdmin.AddPermission(perm);
                                }
                                else if (permChoice == 2)
                                {
                                    Console.Write("Введите право доступа для удаления: ");
                                    string perm = Console.ReadLine();
                                    currentAdmin.RemovePermission(perm);
                                }
                                else if (permChoice == 3)
                                {
                                    currentAdmin.ShowPermissions();
                                }
                            }
                        }
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

        // Режим пользователя
        private static void UserMode()
        {
            if (currentClient == null)
            {
                Console.WriteLine("\nСначала выполните вход как клиент!");
                return;
            }

            int choice;
            do
            {
                Console.WriteLine("\n=== РЕЖИМ ПОЛЬЗОВАТЕЛЯ ===");
                Console.WriteLine("1. Поиск товаров");
                Console.WriteLine("2. Корзина товаров");
                Console.WriteLine("3. Оформить заказ");
                Console.WriteLine("4. Оставить отзыв на музыкальный товар");
                Console.WriteLine("5. Показать информацию клиента");
                Console.WriteLine("6. Рассчитать скидку");
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
                        UserSearchMode();
                        break;
                    case 2:
                        UserBasketMode();
                        break;
                    case 3:
                        UserOrderMode();
                        break;
                    case 4:
                        if (currentClient != null)
                        {
                            Console.WriteLine("\n=== ОСТАВИТЬ ОТЗЫВ НА МУЗЫКАЛЬНЫЙ ТОВАР ===");

                            Console.Write("Введите название товара: ");
                            string productName = Console.ReadLine();

                            Console.Write("Введите артикул товара: ");
                            string productArticle = Console.ReadLine();

                            Console.Write("Введите категорию (Гитары, Клавишные, Ударные и т.д.): ");
                            string category = Console.ReadLine();

                            Console.Write("Введите бренд: ");
                            string brand = Console.ReadLine();

                            Console.Write("Введите оценку (1-5): ");
                            int rating = int.Parse(Console.ReadLine());

                            Console.Write("Введите комментарий: ");
                            string comment = Console.ReadLine();

                            Console.Write("Есть ли фото? (y/n): ");
                            char hasPhotosChar = Console.ReadKey().KeyChar;
                            Console.WriteLine();
                            bool hasPhotos = (hasPhotosChar == 'y' || hasPhotosChar == 'Y');

                            Console.Write("Сколько месяцев используете товар? ");
                            int monthsOfUsage = int.Parse(Console.ReadLine());

                            Console.Write("Рекомендуете товар? (y/n): ");
                            char recommendChar = Console.ReadKey().KeyChar;
                            Console.WriteLine();
                            bool recommend = (recommendChar == 'y' || recommendChar == 'Y');

                            MusicProductReview musicReview = new MusicProductReview(
                                currentClient.Id,
                                productName,
                                productArticle,
                                category,
                                brand,
                                rating,
                                comment,
                                hasPhotos,
                                monthsOfUsage,
                                recommend
                            );

                            Console.WriteLine("\n=== ДОБАВЛЕНИЕ ДОСТОИНСТВ ===");
                            Console.WriteLine("Введите достоинства товара (по одному, пустая строка - завершить):");
                            while (true)
                            {
                                Console.Write("Достоинство: ");
                                string pro = Console.ReadLine();
                                if (string.IsNullOrEmpty(pro)) break;
                                musicReview.AddPro(pro);
                            }

                            Console.WriteLine("\n=== ДОБАВЛЕНИЕ НЕДОСТАТКОВ ===");
                            Console.WriteLine("Введите недостатки товара (по одному, пустая строка - завершить):");
                            while (true)
                            {
                                Console.Write("Недостаток: ");
                                string con = Console.ReadLine();
                                if (string.IsNullOrEmpty(con)) break;
                                musicReview.AddCon(con);
                            }

                            musicReview.SaveToFile();

                            Console.WriteLine("\n=== ИНФОРМАЦИЯ О СОЗДАННОМ ОТЗЫВЕ ===");
                            Console.WriteLine($"Тип отзыва: {musicReview.GetReviewType()}");
                            Console.WriteLine($"Предмет отзыва: {musicReview.GetSubject()}");
                            Console.WriteLine($"Вес отзыва в рейтинге: {musicReview.CalculateWeight()}");
                            Console.WriteLine($"Отзыв валиден: {(musicReview.Validate() ? "Да" : "Нет")}");
                            Console.WriteLine($"Общая оценка качества: {musicReview.GetOverallQuality()}/10");

                            Console.WriteLine("\nОтзыв успешно сохранен!");
                        }
                        break;
                    case 5:
                        if (currentClient != null)
                        {
                            currentClient.DisplayInfo();
                            Console.WriteLine($"Ваша скидка: {currentClient.CalculateDiscount():F1}%");
                        }
                        break;
                    case 6:
                        if (currentClient != null)
                        {
                            Console.WriteLine($"Ваша скидка составляет: {currentClient.CalculateDiscount():F1}%");
                        }
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
                Console.WriteLine("4. Удалить товар из каталога");
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
                    case 1:
                        musicShop.ShowShopInfo();
                        break;
                    case 2:
                        musicShop.DisplayCatalog();
                        break;
                    case 3:
                        musicShop.AddProduct();
                        break;
                    case 4:
                        musicShop.DeleteProduct();
                        break;
                    case 5:
                        musicShop.DisplayOrders();
                        break;
                    case 6:
                        musicShop.DisplayProviderOrders();
                        break;
                    case 7:
                        musicShop.CreateSalesReport();
                        break;
                    case 8:
                        musicShop.RemakeSalesReport();
                        break;
                    case 9:
                        musicShop.DisplaySalesReports();
                        break;
                    case 10:
                        Review.DisplayAllReviews();
                        break;
                    case 11:
                        Console.WriteLine("\n=== РАСЧЕТ СРЕДНЕЙ ОЦЕНКИ ТОВАРА ===");
                        Console.Write("Введите название товара: ");
                        string productName = Console.ReadLine();
                        Review.AverageRating(productName);
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

        private static void AddProducts()
        {
            Console.WriteLine("\n--- ДОБАВЛЕНИЕ ТОВАРОВ ---");
            int productCount;

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

            for (int i = 0; i < productCount; i++)
            {
                Console.WriteLine($"\n=== Товар {i + 1} из {productCount} ===");
                Product product = new Product();
                product.InputFromKeyboard();
                Console.WriteLine("\nТовар успешно добавлен!");
                product.DisplayInfo();
                product.WriteToFile("products.txt");
                Console.WriteLine("---------------------------------------------");
            }
            Console.WriteLine($"\nУспешно добавлено {productCount} товаров!");
        }

        private static void ProviderOrderMode()
        {
            ProviderOrder providerOrder = new ProviderOrder();
            int choice;

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
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        providerOrder.AddProvider();
                        break;
                    case 2:
                        providerOrder.Sending();
                        break;
                    case 3:
                        providerOrder.NewInfoStock();
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

        private static void UserSearchMode()
        {
            Search search = new Search();
            int choice;

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

                switch (choice)
                {
                    case 1:
                        Console.Write("Введите название для поиска: ");
                        string searchString = Console.ReadLine();
                        search.SearchByName("products.txt", searchString);
                        search.DisplayResults();
                        break;
                    case 2:
                        Console.Write("Введите артикул для поиска: ");
                        if (int.TryParse(Console.ReadLine(), out int searchArticle))
                        {
                            search.SearchByArticle("products.txt", searchArticle);
                            search.DisplayResults();
                        }
                        else
                        {
                            Console.WriteLine("Неверный ввод! Пожалуйста, введите целое число.");
                        }
                        break;
                    case 3:
                        Console.Write("Введите бренд для поиска: ");
                        searchString = Console.ReadLine();
                        search.SearchByBrand("products.txt", searchString);
                        search.DisplayResults();
                        break;
                    case 4:
                        Console.Write("Введите страну-производитель для поиска: ");
                        searchString = Console.ReadLine();
                        search.SearchByCountry("products.txt", searchString);
                        search.DisplayResults();
                        break;
                    case 5:
                        Console.Write("Введите категорию для поиска: ");
                        searchString = Console.ReadLine();
                        search.SearchByCategory("products.txt", searchString);
                        search.DisplayResults();
                        break;
                    case 6:
                        Console.Write("Введите тип товара для поиска: ");
                        searchString = Console.ReadLine();
                        search.SearchByType("products.txt", searchString);
                        search.DisplayResults();
                        break;
                    case 7:
                        Console.Write("Введите минимальную цену: ");
                        if (int.TryParse(Console.ReadLine(), out int minPrice))
                        {
                            Console.Write("Введите максимальную цену: ");
                            if (int.TryParse(Console.ReadLine(), out int maxPrice))
                            {
                                search.SearchByPrice("products.txt", minPrice, maxPrice);
                                search.DisplayResults();
                            }
                        }
                        break;
                    case 8:
                        Search.UniversalSearch("products.txt");
                        break;
                    case 9:
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
                    case 1:
                        double totalCost = userBasket.GetBasketCost();
                        Console.WriteLine("\n=== ОБЩАЯ СТОИМОСТЬ КОРЗИНЫ ===");
                        Console.WriteLine($"Общая стоимость: {totalCost:F2} руб.");
                        Console.WriteLine("===============================");
                        break;
                    case 2:
                        uint totalCount = userBasket.GetBasketCount();
                        Console.WriteLine("\n=== КОЛИЧЕСТВО ТОВАРОВ В КОРЗИНЕ ===");
                        Console.WriteLine($"Общее количество товаров: {totalCount} шт.");
                        Console.WriteLine("====================================");
                        break;
                    case 3:
                        userBasket.DisplayBasket();
                        break;
                    case 4:
                        AddProductToBasketByArticle(userBasket);
                        break;
                    case 5:
                        if (userBasket.IsEmpty())
                        {
                            Console.WriteLine("Корзина пуста!");
                            break;
                        }
                        userBasket.DisplayBasket();
                        Console.Write("Введите номер товара для удаления: ");
                        if (int.TryParse(Console.ReadLine(), out int productIndex))
                        {
                            userBasket.DeleteBasket(productIndex);
                        }
                        break;
                    case 6:
                        if (userBasket.IsEmpty())
                        {
                            Console.WriteLine("Корзина пуста!");
                            break;
                        }
                        userBasket.DisplayBasket();
                        Console.Write("Введите номер товара для изменения: ");
                        if (int.TryParse(Console.ReadLine(), out productIndex))
                        {
                            Console.Write("Введите новое количество: ");
                            if (int.TryParse(Console.ReadLine(), out int newQuantity))
                            {
                                userBasket.UpdateQuantity(productIndex, newQuantity);
                            }
                        }
                        break;
                    case 7:
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

        private static void UserOrderMode()
        {
            Console.WriteLine("\n=== ОФОРМЛЕНИЕ ЗАКАЗА ===");
            if (userBasket.IsEmpty())
            {
                Console.WriteLine("Корзина пуста! Добавьте товары перед оформлением заказа.");
                Console.WriteLine("Перейдите в раздел 'Корзина товаров' для добавления товаров.");
                return;
            }

            if (currentClient == null)
            {
                Console.WriteLine("Сначала выполните вход как клиент!");
                return;
            }

            Console.WriteLine("\nСодержимое вашей корзины:");
            userBasket.DisplayBasket();

            Console.Write("\nВы уверены, что хотите оформить заказ? (y/n): ");
            char confirmation = Console.ReadKey().KeyChar;
            Console.WriteLine();

            if (confirmation != 'y' && confirmation != 'Y')
            {
                Console.WriteLine("Оформление заказа отменено.");
                return;
            }

            string clientId = currentClient.Id;

            // Передаем заказ в магазин
            musicShop.CreateOrder(clientId, userBasket);

            Console.WriteLine("\n==================================");
            Console.WriteLine("ЗАКАЗ УСПЕШНО ОФОРМЛЕН!");
            Console.WriteLine("==================================");

            Console.Write("\nХотите оставить отзыв о купленных товарах? (y/n): ");
            confirmation = Console.ReadKey().KeyChar;
            Console.WriteLine();

            if (confirmation == 'y' || confirmation == 'Y')
            {
                Console.WriteLine("Перейдите в раздел 'Оставить отзыв на музыкальный товар' в меню пользователя.");
            }
        }

        private static void AddProductToBasketByArticle(Basket basket)
        {
            Console.Write("Введите артикул товара: ");
            if (!int.TryParse(Console.ReadLine(), out int article))
            {
                Console.WriteLine("Неверный ввод артикула!");
                return;
            }

            Console.Write("Введите количество: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
            {
                Console.WriteLine("Количество должно быть положительным числом!");
                return;
            }

            if (!File.Exists("products.txt"))
            {
                Console.WriteLine("Файл с товарами не найден!");
                return;
            }

            bool found = false;
            string[] lines = File.ReadAllLines("products.txt");

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;

                string[] tokens = line.Split('|');
                if (tokens.Length == 10)
                {
                    try
                    {
                        int currentArticle = int.Parse(tokens[0]);
                        if (currentArticle == article)
                        {
                            found = true;
                            Product product = new Product(
                                int.Parse(tokens[0]),
                                tokens[1],
                                tokens[2],
                                tokens[3],
                                tokens[4],
                                tokens[5],
                                tokens[6],
                                uint.Parse(tokens[7]),
                                int.Parse(tokens[9])
                            );
                            basket.AddBasket(product, quantity);
                            Console.WriteLine($"Товар '{tokens[1]}' добавлен в корзину!");
                            break;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            if (!found)
            {
                Console.WriteLine($"Товар с артикулом {article} не найден!");
            }
        }
    }
}