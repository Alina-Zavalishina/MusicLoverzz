using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Курсовая_работа_Музыкальный_магазин_с_
{
    // Глобальные переменные
    static class Globals
    {
        public static Basket userBasket = new Basket();
        public static Shop musicShop = new Shop("Музыкальный магазин 'MusicLoverzz'", "г. Барнаул, ул. Ленина, 1");
        public static Client currentClient = null;
        public static Admin currentAdmin = null;
    }

    class Program
    {
        // Прототипы функций (в C# не нужны, но для соответствия оставим комментарии)

        static void Main(string[] args)
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
                choice = int.Parse(Console.ReadLine());

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
                        Globals.currentClient = null;
                        Globals.currentAdmin = null;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
            } while (choice != 0);
        }

        // Функция проверки пароля администратора
        static bool CheckAdminPassword()
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
        static void LoginMenu()
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
                choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        {
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

                                Globals.currentClient = new Client(clientId, name, "", "", "");
                                Console.WriteLine("\nВход выполнен как клиент: " + name + " (ID: " + clientId + ")");
                            }
                            return;
                        }
                    case 2:
                        if (CheckAdminPassword())
                        {
                            Globals.currentAdmin = new Admin("Главный Админ");
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
        static void RegisterNewClient()
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

            Globals.currentClient = new Client(name, email, phone, address);

            Console.WriteLine("\n=== РЕГИСТРАЦИЯ УСПЕШНА! ===");
            Console.WriteLine("Ваш ID: " + Globals.currentClient.GetId());
            Console.WriteLine("Добро пожаловать, " + Globals.currentClient.GetName() + "!");
        }

        // Режим администратора
        static void AdminMode()
        {
            if (Globals.currentAdmin == null)
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
                Console.WriteLine("7. Управление отзывами");
                Console.WriteLine("8. Демонстрация сортировки отзывов");
                Console.WriteLine("9. Показать информацию администратора");
                Console.WriteLine("10. Управление правами доступа");
                Console.WriteLine("0. Выход в главное меню");
                Console.Write("Выберите действие: ");
                choice = int.Parse(Console.ReadLine());

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
                        AdminReviewManagement();
                        break;
                    case 8:
                        DemonstrateReviewSorting();
                        break;
                    case 9:
                        if (Globals.currentAdmin != null)
                        {
                            Globals.currentAdmin.DisplayInfo();
                            Globals.currentAdmin.ShowPermissions();
                        }
                        break;
                    case 10:
                        if (Globals.currentAdmin != null)
                        {
                            Console.WriteLine("\n=== УПРАВЛЕНИЕ ПРАВАМИ ДОСТУПА ===");
                            Console.WriteLine("1. Добавить право доступа");
                            Console.WriteLine("2. Удалить право доступа");
                            Console.WriteLine("3. Показать все права");
                            Console.Write("Ваш выбор: ");
                            int permChoice = int.Parse(Console.ReadLine());

                            if (permChoice == 1)
                            {
                                Console.Write("Введите право доступа: ");
                                string perm = Console.ReadLine();
                                Globals.currentAdmin.AddPermission(perm);
                            }
                            else if (permChoice == 2)
                            {
                                Console.Write("Введите право доступа для удаления: ");
                                string perm = Console.ReadLine();
                                Globals.currentAdmin.RemovePermission(perm);
                            }
                            else if (permChoice == 3)
                            {
                                Globals.currentAdmin.ShowPermissions();
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
        static void UserMode()
        {
            if (Globals.currentClient == null)
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
                choice = int.Parse(Console.ReadLine());

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
                        if (Globals.currentClient != null)
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
                            char hasPhotos = Console.ReadKey().KeyChar;
                            Console.WriteLine();

                            Console.Write("Сколько месяцев используете товар? ");
                            int monthsOfUsage = int.Parse(Console.ReadLine());

                            Console.Write("Рекомендуете товар? (y/n): ");
                            char recommend = Console.ReadKey().KeyChar;
                            Console.WriteLine();

                            MusicProductReview musicReview = new MusicProductReview(
                                Globals.currentClient.GetId(),
                                productName,
                                productArticle,
                                category,
                                brand,
                                rating,
                                comment,
                                (hasPhotos == 'y' || hasPhotos == 'Y'),
                                monthsOfUsage,
                                (recommend == 'y' || recommend == 'Y')
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

                            Console.Write("\nХотите оценить качество звука? (y/n): ");
                            char rateChoice = Console.ReadKey().KeyChar;
                            Console.WriteLine();
                            if (rateChoice == 'y' || rateChoice == 'Y')
                            {
                                Console.Write("Оценка качества звука (1-10): ");
                                int soundScore = int.Parse(Console.ReadLine());
                                musicReview.RateSoundQuality(soundScore);
                            }

                            Console.Write("Хотите оценить качество сборки? (y/n): ");
                            rateChoice = Console.ReadKey().KeyChar;
                            Console.WriteLine();
                            if (rateChoice == 'y' || rateChoice == 'Y')
                            {
                                Console.Write("Оценка качества сборки (1-10): ");
                                int buildScore = int.Parse(Console.ReadLine());
                                musicReview.RateBuildQuality(buildScore);
                            }

                            musicReview.SaveToFile();

                            Console.WriteLine("\n=== ИНФОРМАЦИЯ О СОЗДАННОМ ОТЗЫВЕ ===");
                            Console.WriteLine(musicReview.ExportToString());

                            Console.WriteLine("\nОтзыв успешно сохранен!");
                        }
                        break;
                    case 5:
                        if (Globals.currentClient != null)
                        {
                            Globals.currentClient.DisplayInfo();
                            Console.WriteLine("Ваша скидка: " + Globals.currentClient.CalculateDiscount() + "%");
                        }
                        break;
                    case 6:
                        if (Globals.currentClient != null)
                        {
                            Console.WriteLine("Ваша скидка составляет: " + Globals.currentClient.CalculateDiscount() + "%");
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
        static void ShopManagementMode()
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
                Console.WriteLine("12. Анализ отзывов");
                Console.WriteLine("0. Назад");
                Console.Write("Выберите действие: ");
                choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        Globals.musicShop.ShowShopInfo();
                        break;
                    case 2:
                        Globals.musicShop.DisplayCatalog();
                        break;
                    case 3:
                        Globals.musicShop.AddProduct();
                        break;
                    case 4:
                        Globals.musicShop.DeleteProduct();
                        break;
                    case 5:
                        Globals.musicShop.DisplayOrders();
                        break;
                    case 6:
                        Globals.musicShop.DisplayProviderOrders();
                        break;
                    case 7:
                        Globals.musicShop.CreateSalesReport();
                        break;
                    case 8:
                        Globals.musicShop.RemakeSalesReport();
                        break;
                    case 9:
                        Globals.musicShop.DisplaySalesReports();
                        break;
                    case 10:
                        Review<MusicProductReview>.DisplayAllReviews();
                        break;
                    case 11:
                        {
                            Console.WriteLine("\n=== РАСЧЕТ СРЕДНЕЙ ОЦЕНКИ ТОВАРА ===");
                            Console.Write("Введите название товара: ");
                            string productName = Console.ReadLine();
                            double avgRating = Review<MusicProductReview>.AverageRating(productName);
                            Console.WriteLine("Средняя оценка товара '" + productName + "': " + avgRating.ToString("F2") + "/5");
                            break;
                        }
                    case 12:
                        {
                            Console.WriteLine("\n=== АНАЛИЗ ОТЗЫВОВ ===");

                            List<MusicProductReview> allReviews = new List<MusicProductReview>();

                            // Чтение отзывов из файла
                            if (!File.Exists("reviews.txt"))
                            {
                                Console.WriteLine("Файл с отзывами не найден!");
                                break;
                            }

                            string[] lines = File.ReadAllLines("reviews.txt");
                            foreach (string line in lines)
                            {
                                if (string.IsNullOrEmpty(line)) continue;

                                string[] tokens = line.Split('|');
                                if (tokens.Length >= 9)
                                {
                                    try
                                    {
                                        MusicProductReview review = new MusicProductReview(
                                            tokens[2], tokens[3], tokens[4], tokens[5], tokens[6],
                                            int.Parse(tokens[7]), tokens[8]);
                                        review.SetIdReview(tokens[0]);
                                        review.SetDateReview(tokens[9]);
                                        allReviews.Add(review);
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                            }

                            if (allReviews.Count == 0)
                            {
                                Console.WriteLine("Нет отзывов для анализа.");
                                break;
                            }

                            Console.WriteLine("Всего отзывов: " + allReviews.Count);

                            int totalRating = allReviews.Sum(r => r.GetRating());
                            Console.WriteLine("Средний рейтинг: " + ((double)totalRating / allReviews.Count) + "/5");

                            int fiveStar = allReviews.Count(r => r.GetRating() == 5);
                            int oneStar = allReviews.Count(r => r.GetRating() == 1);

                            Console.WriteLine("5-звездочных отзывов: " + fiveStar);
                            Console.WriteLine("1-звездочных отзывов: " + oneStar);

                            int positiveCount = allReviews.Count(r => r.GetRating() >= 3);
                            int negativeCount = allReviews.Count(r => r.GetRating() < 3);

                            Console.WriteLine("Положительных отзывов (>=3): " + positiveCount);
                            Console.WriteLine("Отрицательных отзывов (<3): " + negativeCount);

                            Console.WriteLine("\n=== ТОП-3 ТОВАРА ПО РЕЙТИНГУ ===");

                            var sortedReviews = allReviews.OrderByDescending(r => r.GetRating()).Take(3);
                            foreach (var review in sortedReviews)
                            {
                                Console.WriteLine("- " + review.GetSubject() + ": " + review.GetRating() + "/5");
                            }

                            Console.WriteLine("\n=== АНАЛИЗ ПО КАТЕГОРИЯМ ===");

                            var categories = allReviews.Select(r => r.GetCategory()).Distinct();
                            Console.WriteLine("Всего категорий: " + categories.Count());

                            foreach (var category in categories)
                            {
                                var categoryReviews = allReviews.Where(r => r.GetCategory() == category).ToList();
                                if (categoryReviews.Count > 0)
                                {
                                    double avg = categoryReviews.Average(r => r.GetRating());
                                    Console.WriteLine("Категория '" + category + "': " + categoryReviews.Count +
                                        " отзывов, средний рейтинг: " + avg.ToString("F2") + "/5");
                                }
                            }

                            break;
                        }
                    case 0:
                        Console.WriteLine("Выход из управления магазином...");
                        break;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
            } while (choice != 0);
        }

        static void AddProducts()
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
                break;
            }

            for (int i = 0; i < productCount; i++)
            {
                Console.WriteLine("\n=== Товар " + (i + 1) + " из " + productCount + " ===");
                Product product = new Product();
                product.InputFromKeyboard();
                Console.WriteLine("\nТовар успешно добавлен!");
                product.DisplayInfo();
                product.WriteToFile("products.txt");
                Console.WriteLine("---------------------------------------------");
            }
            Console.WriteLine("\nУспешно добавлено " + productCount + " товаров!");
        }

        static void ProviderOrderMode()
        {
            int choice;
            ProviderOrder providerOrder = new ProviderOrder();
            do
            {
                Console.WriteLine("\n=== ЗАКАЗ У ПОСТАВЩИКА ===");
                Console.WriteLine("1. Добавить товар в заказ (из заказов клиентов)");
                Console.WriteLine("2. Отправить заказ поставщику");
                Console.WriteLine("3. Обновить информацию о товарах");
                Console.WriteLine("0. Назад");
                Console.Write("Выберите действие: ");
                choice = int.Parse(Console.ReadLine());

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

        static void UserSearchMode()
        {
            int choice;
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
                Console.WriteLine("10. Очистить историю поиска");
                Console.WriteLine("0. Назад");
                Console.Write("Выберите действие: ");
                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Неверный ввод! Пожалуйста, введите число от 0 до 10.");
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
                        int searchArticle = int.Parse(Console.ReadLine());
                        search.SearchByArticle("products.txt", searchArticle);
                        search.DisplayResults();
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
                        int minPrice = int.Parse(Console.ReadLine());
                        Console.Write("Введите максимальную цену: ");
                        int maxPrice = int.Parse(Console.ReadLine());
                        search.SearchByPrice("products.txt", minPrice, maxPrice);
                        search.DisplayResults();
                        break;
                    case 8:
                        search.UniversalSearch("products.txt");
                        break;
                    case 9:
                        search.DisplaySearchHistory();
                        break;
                    case 10:
                        search.ClearSearchHistory();
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

        static void UserBasketMode()
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
                Console.WriteLine("8. Применить алгоритмы сортировки к корзине");
                Console.WriteLine("0. Назад");
                Console.Write("Выберите действие: ");
                choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        {
                            double totalCost = Globals.userBasket.GetBasketCost();
                            Console.WriteLine("\n=== ОБЩАЯ СТОИМОСТЬ КОРЗИНЫ ===");
                            Console.WriteLine("Общая стоимость: " + totalCost + " руб.");

                            List<double> prices = new List<double>();
                            for (int i = 0; i < Globals.userBasket.GetProductCount(); i++)
                            {
                                Product product = Globals.userBasket.GetProduct(i);
                                if (product != null)
                                {
                                    prices.Add(product.GetPrice() * Globals.userBasket.GetQuantity(i));
                                }
                            }

                            if (prices.Count > 0)
                            {
                                Console.WriteLine("Самый дорогой товар: " + prices.Max() + " руб.");
                                Console.WriteLine("Самый дешевый товар: " + prices.Min() + " руб.");
                                Console.WriteLine("Средняя стоимость товара: " + prices.Average() + " руб.");
                            }

                            Console.WriteLine("===============================");
                            break;
                        }
                    case 2:
                        {
                            uint totalCount = Globals.userBasket.GetBasketCount();
                            Console.WriteLine("\n=== КОЛИЧЕСТВО ТОВАРОВ В КОРЗИНЕ ===");
                            Console.WriteLine("Общее количество товаров: " + totalCount + " шт.");
                            Console.WriteLine("====================================");
                            break;
                        }
                    case 3:
                        Globals.userBasket.DisplayBasket();
                        break;
                    case 4:
                        AddProductToBasketByArticle(Globals.userBasket);
                        break;
                    case 5:
                        if (Globals.userBasket.IsEmpty())
                        {
                            Console.WriteLine("Корзина пуста!");
                            break;
                        }
                        Globals.userBasket.DisplayBasket();
                        Console.Write("Введите номер товара для удаления: ");
                        int productIndex = int.Parse(Console.ReadLine());
                        Globals.userBasket.DeleteBasket(productIndex);
                        break;
                    case 6:
                        if (Globals.userBasket.IsEmpty())
                        {
                            Console.WriteLine("Корзина пуста!");
                            break;
                        }
                        Globals.userBasket.DisplayBasket();
                        Console.Write("Введите номер товара для изменения: ");
                        productIndex = int.Parse(Console.ReadLine());
                        Console.Write("Введите новое количество: ");
                        int newQuantity = int.Parse(Console.ReadLine());
                        Globals.userBasket.UpdateQuantity(productIndex, newQuantity);
                        break;
                    case 7:
                        Globals.userBasket.ClearBasket();
                        Console.WriteLine("Корзина очищена!");
                        break;
                    case 8:
                        {
                            Console.WriteLine("\n=== АЛГОРИТМЫ STL ДЛЯ КОРЗИНЫ ===");

                            List<Tuple<Product, int>> basketItems = new List<Tuple<Product, int>>();
                            for (int i = 0; i < Globals.userBasket.GetProductCount(); i++)
                            {
                                Product product = Globals.userBasket.GetProduct(i);
                                int quantity = Globals.userBasket.GetQuantity(i);
                                if (product != null)
                                {
                                    basketItems.Add(Tuple.Create(product, quantity));
                                }
                            }

                            if (basketItems.Count == 0)
                            {
                                Console.WriteLine("Корзина пуста!");
                                break;
                            }

                            Console.WriteLine("1. Сортировка товаров по цене (по возрастанию)");
                            basketItems = basketItems.OrderBy(item => item.Item1.GetPrice()).ToList();

                            Console.WriteLine("Отсортированные товары:");
                            foreach (var item in basketItems)
                            {
                                Console.WriteLine("- " + item.Item1.GetName() + ": " +
                                    item.Item1.GetPrice() + " руб. (x" + item.Item2 + ")");
                            }

                            Console.WriteLine("\n2. Поиск самого дорогого товара");
                            var maxItem = basketItems.OrderByDescending(item => item.Item1.GetPrice()).FirstOrDefault();
                            if (maxItem != null)
                            {
                                Console.WriteLine("Самый дорогой товар: " + maxItem.Item1.GetName() +
                                    " - " + maxItem.Item1.GetPrice() + " руб.");
                            }

                            Console.WriteLine("\n3. Подсчет товаров дороже 1000 руб.");
                            int expensiveCount = basketItems.Count(item => item.Item1.GetPrice() > 1000);
                            Console.WriteLine("Товаров дороже 1000 руб.: " + expensiveCount);

                            Console.WriteLine("\n4. Общая стоимость: ");
                            double total = basketItems.Sum(item => item.Item1.GetPrice() * item.Item2);
                            Console.WriteLine("Общая стоимость: " + total + " руб.");

                            break;
                        }
                    case 0:
                        Console.WriteLine("Выход из режима корзины...");
                        break;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
            } while (choice != 0);
        }

        static void UserOrderMode()
        {
            Console.WriteLine("\n=== ОФОРМЛЕНИЕ ЗАКАЗА ===");
            if (Globals.userBasket.IsEmpty())
            {
                Console.WriteLine("Корзина пуста! Добавьте товары перед оформлением заказа.");
                Console.WriteLine("Перейдите в раздел 'Корзина товаров' для добавления товаров.");
                return;
            }

            if (Globals.currentClient == null)
            {
                Console.WriteLine("Сначала выполните вход как клиент!");
                return;
            }

            Console.WriteLine("\nСодержимое вашей корзины:");
            Globals.userBasket.DisplayBasket();

            Console.Write("\nВы уверены, что хотите оформить заказ? (y/n): ");
            char confirmation = Console.ReadKey().KeyChar;
            Console.WriteLine();

            if (confirmation != 'y' && confirmation != 'Y')
            {
                Console.WriteLine("Оформление заказа отменено.");
                return;
            }

            string clientId = Globals.currentClient.GetId();

            // Создаем заказ
            Order clientOrder = new Order(clientId, Globals.userBasket);

            // Передаем заказ в магазин
            Globals.musicShop.CreateOrder(clientId, Globals.userBasket);

            Console.WriteLine("\n==================================");
            Console.WriteLine("ЗАКАЗ УСПЕШНО ОФОРМЛЕН!");
            Console.WriteLine("Номер заказа: " + clientOrder.GetNumberOrder());
            Console.WriteLine("Дата: " + clientOrder.GetDateOrder());
            Console.WriteLine("Сумма: " + clientOrder.GetCostOrder() + " руб.");
            Console.WriteLine("Количество товаров: " + clientOrder.GetCountOrder() + " шт.");
            Console.WriteLine("==================================");

            // Обновляем информацию о клиенте
            if (Globals.currentClient != null)
            {
                Globals.currentClient.UpdateTotalSpent(clientOrder.GetCostOrder());
                int loyaltyPoints = (int)(clientOrder.GetCostOrder() / 100); // 1 балл за каждые 100 руб.
                Globals.currentClient.UpdateLoyaltyPoints(loyaltyPoints);
                Console.WriteLine("Начислено бонусных баллов: " + loyaltyPoints);
                Console.WriteLine("Ваша новая скидка: " + Globals.currentClient.CalculateDiscount() + "%");
            }

            Console.Write("\nХотите оставить отзыв о купленных товарах? (y/n): ");
            confirmation = Console.ReadKey().KeyChar;
            Console.WriteLine();
            if (confirmation == 'y' || confirmation == 'Y')
            {
                Console.WriteLine("Перейдите в раздел 'Оставить отзыв на музыкальный товар' в меню пользователя.");
            }

            // Очищаем корзину после оформления заказа
            Globals.userBasket.ClearBasket();
        }

        static void AddProductToBasketByArticle(Basket basket)
        {
            try
            {
                Console.Write("Введите артикул товара: ");
                int article = int.Parse(Console.ReadLine());
                Console.Write("Введите количество: ");
                int quantity = int.Parse(Console.ReadLine());

                if (quantity <= 0)
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

                                // ИСПРАВЛЕНИЕ: используем uint.Parse вместо int.Parse
                                uint quantityStock = uint.Parse(tokens[7]);

                                // Проверяем, достаточно ли товара на складе
                                if (quantityStock < (uint)quantity)
                                {
                                    Console.WriteLine($"Недостаточно товара на складе! Доступно: {quantityStock}, запрошено: {quantity}");
                                    return;
                                }

                                Product product = new Product(
                                    int.Parse(tokens[0]),    // article
                                    tokens[1],              // name
                                    tokens[2],              // brand
                                    tokens[3],              // country
                                    tokens[4],              // category
                                    tokens[5],              // type
                                    tokens[6],              // description
                                    quantityStock,         // quantity (uint!)
                                    int.Parse(tokens[9])    // price
                                );

                                basket.AddBasket(product, quantity);
                                Console.WriteLine($"Товар '{tokens[1]}' добавлен в корзину!");
                                break;
                            }
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine($"Ошибка формата данных в строке: {line}");
                            continue;
                        }
                        catch (OverflowException)
                        {
                            Console.WriteLine($"Переполнение при чтении данных в строке: {line}");
                            continue;
                        }
                    }
                }

                if (!found)
                {
                    Console.WriteLine($"Товар с артикулом {article} не найден!");
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Ошибка ввода! Введите корректные числа.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }

        static void AdminReviewManagement()
        {
            if (Globals.currentAdmin == null)
            {
                Console.WriteLine("\nСначала выполните вход как администратор!");
                return;
            }

            Console.WriteLine("\n=== УПРАВЛЕНИЕ ОТЗЫВАМИ (АДМИНИСТРАТОР) ===");

            // Загружаем отзывы из файла как MusicProductReview
            List<MusicProductReview> reviews = new List<MusicProductReview>();

            if (!File.Exists("reviews.txt"))
            {
                Console.WriteLine("Файл с отзывами не найден!");
                return;
            }

            string[] lines = File.ReadAllLines("reviews.txt");
            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;

                string[] tokens = line.Split('|');
                if (tokens.Length >= 9)
                {
                    try
                    {
                        MusicProductReview review = new MusicProductReview(
                            tokens[2], tokens[3], tokens[4], tokens[5], tokens[6],
                            int.Parse(tokens[7]), tokens[8]
                        );

                        review.SetIdReview(tokens[0]);
                        review.SetDateReview(tokens[9]);

                                                if (tokens.Length > 10 && tokens[10] == "1")
                        {
                            review.VerifyReview();
                        }

                        reviews.Add(review);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Ошибка при чтении отзыва: " + e.Message);
                    }
                }
            }

            if (reviews.Count == 0)
            {
                Console.WriteLine("Нет отзывов для управления.");
                return;
            }

            int choice;
            do
            {
                Console.WriteLine("\n=== МЕНЮ УПРАВЛЕНИЯ ОТЗЫВАМИ ===");
                Console.WriteLine("Всего отзывов: " + reviews.Count);
                Console.WriteLine("1. Показать все отзывы");
                Console.WriteLine("2. Сортировать отзывы");
                Console.WriteLine("3. Найти отзыв с максимальным рейтингом");
                Console.WriteLine("4. Найти отзыв с минимальным рейтингом");
                Console.WriteLine("5. Фильтровать отзывы по рейтингу");
                Console.WriteLine("6. Проверить, есть ли проверенные отзывы");
                Console.WriteLine("7. Общая статистика отзывов");
                Console.WriteLine("8. Показать отзывы в виде таблицы");
                Console.WriteLine("0. Назад");
                Console.Write("Выберите действие: ");
                choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        {
                            Console.WriteLine("\n=== ВСЕ ОТЗЫВЫ ===");
                            for (int i = 0; i < reviews.Count; i++)
                            {
                                Console.WriteLine("\n--- Отзыв " + (i + 1) + " ---");
                                Console.WriteLine(reviews[i].ExportToString());
                            }
                            break;
                        }
                    case 2:
                        {
                            Console.WriteLine("\n=== СОРТИРОВКА ОТЗЫВОВ ===");
                            Console.WriteLine("Выберите критерий сортировки:");
                            Console.WriteLine("1. По дате (новые сначала)");
                            Console.WriteLine("2. По дате (старые сначала)");
                            Console.WriteLine("3. По рейтингу (высокий сначала)");
                            Console.WriteLine("4. По рейтингу (низкий сначала)");
                            Console.WriteLine("5. По полезности");
                            Console.WriteLine("6. Проверенные сначала");
                            Console.Write("Ваш выбор: ");

                            int sortChoice = int.Parse(Console.ReadLine());

                            List<MusicProductReview> sortedReviews = new List<MusicProductReview>(reviews);

                            switch (sortChoice)
                            {
                                case 1: // По дате (новые сначала)
                                    sortedReviews = sortedReviews.OrderByDescending(r => r.GetDateReview()).ToList();
                                    break;
                                case 2: // По дате (старые сначала)
                                    sortedReviews = sortedReviews.OrderBy(r => r.GetDateReview()).ToList();
                                    break;
                                case 3: // По рейтингу (высокий сначала)
                                    sortedReviews = sortedReviews.OrderByDescending(r => r.GetRating()).ToList();
                                    break;
                                case 4: // По рейтингу (низкий сначала)
                                    sortedReviews = sortedReviews.OrderBy(r => r.GetRating()).ToList();
                                    break;
                                case 5: // По полезности
                                    sortedReviews = sortedReviews.OrderByDescending(r => r.CalculateWeight()).ToList();
                                    break;
                                case 6: // Проверенные сначала
                                    sortedReviews = sortedReviews.OrderByDescending(r => r.GetIsVerified())
                                                                 .ThenByDescending(r => r.GetDateReview())
                                                                 .ToList();
                                    break;
                                default:
                                    Console.WriteLine("Неверный выбор!");
                                    continue;
                            }

                            Console.WriteLine("\n=== ОТСОРТИРОВАННЫЕ ОТЗЫВЫ (первые 5) ===");
                            int limit = Math.Min(sortedReviews.Count, 5);
                            for (int i = 0; i < limit; i++)
                            {
                                Console.WriteLine("\n--- Отзыв " + (i + 1) + " ---");
                                Console.WriteLine("Рейтинг: " + sortedReviews[i].GetRating() + "/5");
                                Console.WriteLine("Дата: " + sortedReviews[i].GetDateReview());
                                Console.WriteLine("Товар: " + sortedReviews[i].GetSubject());
                                Console.WriteLine("Проверен: " + (sortedReviews[i].GetIsVerified() ? "Да" : "Нет"));
                            }
                            break;
                        }
                    case 3:
                        {
                            if (reviews.Count == 0)
                            {
                                Console.WriteLine("Нет отзывов!");
                                break;
                            }

                            MusicProductReview maxRatingReview = reviews.OrderByDescending(r => r.GetRating()).First();

                            Console.WriteLine("\n=== ОТЗЫВ С МАКСИМАЛЬНЫМ РЕЙТИНГОМ ===");
                            Console.WriteLine("Рейтинг: " + maxRatingReview.GetRating() + "/5");
                            Console.WriteLine("Товар: " + maxRatingReview.GetSubject());
                            Console.WriteLine("Дата: " + maxRatingReview.GetDateReview());
                            Console.WriteLine("Клиент: " + maxRatingReview.GetClientId());
                            break;
                        }
                    case 4:
                        {
                            if (reviews.Count == 0)
                            {
                                Console.WriteLine("Нет отзывов!");
                                break;
                            }

                            MusicProductReview minRatingReview = reviews.OrderBy(r => r.GetRating()).First();

                            Console.WriteLine("\n=== ОТЗЫВ С МИНИМАЛЬНЫМ РЕЙТИНГОМ ===");
                            Console.WriteLine("Рейтинг: " + minRatingReview.GetRating() + "/5");
                            Console.WriteLine("Товар: " + minRatingReview.GetSubject());
                            Console.WriteLine("Дата: " + minRatingReview.GetDateReview());
                            Console.WriteLine("Клиент: " + minRatingReview.GetClientId());
                            break;
                        }
                    case 5:
                        {
                            Console.WriteLine("\n=== ФИЛЬТРАЦИЯ ОТЗЫВОВ ПО РЕЙТИНГУ ===");
                            Console.Write("Введите минимальный рейтинг (1-5): ");
                            int minRating = int.Parse(Console.ReadLine());

                            if (minRating < 1 || minRating > 5)
                            {
                                Console.WriteLine("Некорректный рейтинг!");
                                break;
                            }

                            List<MusicProductReview> filteredReviews = reviews.Where(r => r.GetRating() >= minRating).ToList();

                            Console.WriteLine("\nНайдено отзывов с рейтингом >= " + minRating + ": " + filteredReviews.Count);

                            if (filteredReviews.Count > 0)
                            {
                                int displayLimit = Math.Min(filteredReviews.Count, 5);
                                for (int i = 0; i < displayLimit; i++)
                                {
                                    Console.WriteLine("\n--- Отзыв " + (i + 1) + " ---");
                                    Console.WriteLine("Рейтинг: " + filteredReviews[i].GetRating() + "/5");
                                    Console.WriteLine("Товар: " + filteredReviews[i].GetSubject());
                                    Console.WriteLine("Дата: " + filteredReviews[i].GetDateReview());
                                }
                            }
                            break;
                        }
                    case 6:
                        {
                            bool hasVerified = reviews.Any(r => r.GetIsVerified());

                            Console.WriteLine("\n=== ПРОВЕРЕННЫЕ ОТЗЫВЫ ===");
                            Console.WriteLine("Есть проверенные отзывы: " + (hasVerified ? "Да" : "Нет"));

                            if (hasVerified)
                            {
                                int verifiedCount = reviews.Count(r => r.GetIsVerified());
                                Console.WriteLine("Количество проверенных отзывов: " + verifiedCount);
                            }
                            break;
                        }
                    case 7:
                        {
                            Console.WriteLine("\n=== СТАТИСТИКА ОТЗЫВОВ ===");

                            if (reviews.Count == 0)
                            {
                                Console.WriteLine("Нет отзывов для статистики!");
                                break;
                            }

                            Console.WriteLine("Общее количество отзывов: " + reviews.Count);

                            double avgRating = reviews.Average(r => r.GetRating());
                            int maxRating = reviews.Max(r => r.GetRating());
                            int minRating = reviews.Min(r => r.GetRating());
                            int verifiedCount = reviews.Count(r => r.GetIsVerified());

                            Console.WriteLine("Средний рейтинг: " + avgRating.ToString("F2") + "/5");
                            Console.WriteLine("Минимальный рейтинг: " + minRating + "/5");
                            Console.WriteLine("Максимальный рейтинг: " + maxRating + "/5");
                            Console.WriteLine("Количество проверенных отзывов: " + verifiedCount);

                            // Распределение по рейтингам
                            int[] ratingCount = new int[6];
                            foreach (var review in reviews)
                            {
                                int rating = review.GetRating();
                                if (rating >= 0 && rating <= 5)
                                {
                                    ratingCount[rating]++;
                                }
                            }

                            Console.WriteLine("\nРаспределение по рейтингам:");
                            for (int i = 5; i >= 1; i--)
                            {
                                Console.WriteLine(i + " звезд: " + ratingCount[i] + " (" +
                                    (ratingCount[i] * 100.0 / reviews.Count).ToString("F2") + "%)");
                            }
                            break;
                        }
                    case 8:
                        {
                            Console.WriteLine("\n=== ТАБЛИЦА ОТЗЫВОВ ===");
                            Console.WriteLine(string.Format("{0,-5}{1,-20}{2,-8}{3,-15}{4,-10}{5,-10}", 
                                "№", "Товар", "Рейтинг", "Дата", "Проверен", "Вес"));
                            Console.WriteLine(new string('-', 68));

                            for (int i = 0; i < reviews.Count; i++)
                            {
                                string productName = reviews[i].GetSubject();
                                if (productName.Length > 18)
                                {
                                    productName = productName.Substring(0, 15) + "...";
                                }

                                Console.WriteLine(string.Format("{0,-5}{1,-20}{2,-8}{3,-15}{4,-10}{5,-10:F2}",
                                    (i + 1),
                                    productName,
                                    reviews[i].GetRating() + "/5",
                                    reviews[i].GetDateReview(),
                                    reviews[i].GetIsVerified() ? "Да" : "Нет",
                                    reviews[i].CalculateWeight()));
                            }
                            break;
                        }
                    case 0:
                        Console.WriteLine("Выход из управления отзывами...");
                        break;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
            } while (choice != 0);
        }

        // Демонстрация сортировки отзывов
        static void DemonstrateReviewSorting()
        {
            Console.WriteLine("\n=== ДЕМОНСТРАЦИЯ АЛГОРИТМОВ СОРТИРОВКИ ОТЗЫВОВ ===");

            // Загружаем отзывы из файла
            List<MusicProductReview> reviews = new List<MusicProductReview>();

            if (!File.Exists("reviews.txt"))
            {
                Console.WriteLine("Файл 'reviews.txt' не найден!");
                Console.WriteLine("Сначала создайте отзывы через меню пользователя.");
                return;
            }

            string[] lines = File.ReadAllLines("reviews.txt");
            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;

                string[] tokens = line.Split('|');
                if (tokens.Length >= 9)
                {
                    try
                    {
                        MusicProductReview review = new MusicProductReview(
                            tokens[2], tokens[3], tokens[4], tokens[5], tokens[6],
                            int.Parse(tokens[7]), tokens[8]
                        );
                        review.SetIdReview(tokens[0]);
                        review.SetDateReview(tokens[9]);
                        if (tokens.Length > 10 && tokens[10] == "1")
                        {
                            review.VerifyReview();
                        }
                        reviews.Add(review);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            if (reviews.Count == 0)
            {
                Console.WriteLine("Нет отзывов для демонстрации!");
                return;
            }

            Console.WriteLine("Всего отзывов загружено: " + reviews.Count);

            // 1. Демонстрация сортировки по рейтингу
            Console.WriteLine("\n1. СОРТИРОВКА ПО РЕЙТИНГУ (высокий сначала):");
            var sortedByRating = reviews.OrderByDescending(r => r.GetRating()).Take(5).ToList();

            for (int i = 0; i < sortedByRating.Count; i++)
            {
                Console.WriteLine("  " + (i + 1) + ". " + sortedByRating[i].GetSubject() +
                    " - " + sortedByRating[i].GetRating() + "/5");
            }

            // 2. Демонстрация сортировки по дате
            Console.WriteLine("\n2. СОРТИРОВКА ПО ДАТЕ (новые сначала):");
            var sortedByDate = reviews.OrderByDescending(r => r.GetDateReview()).Take(5).ToList();

            for (int i = 0; i < sortedByDate.Count; i++)
            {
                Console.WriteLine("  " + (i + 1) + ". " + sortedByDate[i].GetDateReview() +
                    " - " + sortedByDate[i].GetSubject());
            }

            // 3. Демонстрация пользовательской сортировки по весу отзыва
            Console.WriteLine("\n3. ПОЛЬЗОВАТЕЛЬСКАЯ СОРТИРОВКА (по весу отзыва):");
            var sortedByWeight = reviews.OrderByDescending(r => r.CalculateWeight()).Take(5).ToList();

            for (int i = 0; i < sortedByWeight.Count; i++)
            {
                Console.WriteLine("  " + (i + 1) + ". Вес: " + sortedByWeight[i].CalculateWeight().ToString("F2") +
                    " - " + sortedByWeight[i].GetSubject());
            }

            // 4. Демонстрация фильтрации
            Console.WriteLine("\n4. ФИЛЬТРАЦИЯ ОТЗЫВОВ (рейтинг >= 4):");
            var highRatedReviews = reviews.Where(r => r.GetRating() >= 4).ToList();

            Console.WriteLine("   Найдено высокооцененных отзывов: " + highRatedReviews.Count);

            // 5. Демонстрация поиска
            Console.WriteLine("\n5. ПОИСК ОТЗЫВОВ (рейтинг = 5):");
            var perfectReview = reviews.FirstOrDefault(r => r.GetRating() == 5);

            if (perfectReview != null)
            {
                Console.WriteLine("   Найден отзыв с рейтингом 5: " + perfectReview.GetSubject());
            }
            else
            {
                Console.WriteLine("   Отзывов с рейтингом 5 не найдено");
            }

            // 6. Демонстрация статистики
            Console.WriteLine("\n6. СТАТИСТИКА ОТЗЫВОВ с помощью LINQ:");
            Console.WriteLine("   Всего отзывов: " + reviews.Count);

            if (reviews.Count > 0)
            {
                double avgRating = reviews.Average(r => r.GetRating());
                int maxRating = reviews.Max(r => r.GetRating());
                int minRating = reviews.Min(r => r.GetRating());
                int verifiedCount = reviews.Count(r => r.GetIsVerified());

                Console.WriteLine("   Средний рейтинг: " + avgRating.ToString("F2") + "/5");
                Console.WriteLine("   Проверенных отзывов: " + verifiedCount);
            }

            // 7. Демонстрация группировки по рейтингу
            Console.WriteLine("\n7. ГРУППИРОВКА ОТЗЫВОВ ПО РЕЙТИНГУ:");
            var groups = reviews.GroupBy(r => r.GetRating())
                               .OrderByDescending(g => g.Key)
                               .Where(g => g.Key >= 1 && g.Key <= 5);

            foreach (var group in groups)
            {
                Console.WriteLine("   Рейтинг " + group.Key + ": " + group.Count() + " отзывов");
            }

            Console.WriteLine("\n=== ДЕМОНСТРАЦИЯ ЗАВЕРШЕНА ===");
        }
    }
}