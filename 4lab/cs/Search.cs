
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MusicShopSystem
{
    public class Search
    {
        // Константы
        private const int MAX_RESULTS = 1000;  // для поиска
        private const int MAX_HISTORY = 10;    // для истории поиска

        // Структура для хранения результатов поиска
        private struct SearchResult
        {
            public int Article;
            public string Name;
            public string Brand;
            public string ProducerCountry;
            public string Category;
            public string TypeProduct;
            public string Description;
            public uint QuantityStock;
            public bool StatusStock;
            public int Price;
        }

        // Поля класса
        private string request;                 // запрос на поиск товара
        private int resultCount;                // количество найденных товаров
        private SearchResult[] searchResults;   // массив результатов поиска
        private string[] searchHistory;         // история поиска
        private int historyCount;               // счетчик записи в историю

        // Конструктор
        public Search()
        {
            resultCount = 0;
            historyCount = 0;
            searchResults = new SearchResult[MAX_RESULTS];
            searchHistory = new string[MAX_HISTORY];
            request = "";
        }

        // Вспомогательный метод: преобразование строки в нижний регистр
        private string ToLower(string str)
        {
            return str.ToLower();
        }

        // Очистка результатов поиска
        private void ClearResults()
        {
            resultCount = 0;
        }

        // Добавление в историю поиска
        private void AddToHistory(string searchInfo)
        {
            if (historyCount >= MAX_HISTORY) // Проверка переполнения истории
            {
                // Сдвигаем историю
                for (int i = 0; i < MAX_HISTORY - 1; i++)
                {
                    searchHistory[i] = searchHistory[i + 1]; // Перемещаем запись
                }
                historyCount = MAX_HISTORY - 1;// Устанавливаем счетчик на предпоследнюю позицию
            }

            searchHistory[historyCount] = searchInfo;// Сохранение информации о поиске
            historyCount++;
        }

        // Показать историю поиска
        public void DisplaySearchHistory()
        {
            Console.WriteLine("\n=== ИСТОРИЯ ПОИСКА ===");

            if (historyCount == 0)
            {
                Console.WriteLine("История поиска пуста!");
                Console.WriteLine("==========================");
                return;
            }

            Console.WriteLine($"Последние {historyCount} запросов:");
            Console.WriteLine("==========================");

            for (int i = historyCount - 1; i >= 0; i--)
            {
                Console.WriteLine($"Запрос #{historyCount - i}: {searchHistory[i]}");
            }

            Console.WriteLine("==========================");
        }

        // Поиск по названию
        public void SearchByName(string filename, string name)
        {
            ClearResults();

            if (!File.Exists(filename))
            {
                Console.WriteLine($"Файл {filename} не найден!");
                return;
            }

            string searchNameLower = ToLower(name);  // Приведение поискового запроса к нижнему регистру
            string[] lines = File.ReadAllLines(filename); // Чтение всех строк из файла

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;// Пропуск пустых строк
                if (resultCount >= MAX_RESULTS) break;  // Прерывание при достижении максимума

                string[] tokens = line.Split('|');
                if (tokens.Length >= 10)
                {
                    try
                    {
                        string productName = tokens[1];// Получение названия товара из файла
                        string productNameLower = ToLower(productName);// Приведение к нижнему регистру

                        if (productNameLower.Contains(searchNameLower))// Проверка содержит ли название товара искомую подстроку
                        {
                            searchResults[resultCount] = new SearchResult
                            {
                                Article = int.Parse(tokens[0]),
                                Name = tokens[1],
                                Brand = tokens[2],
                                ProducerCountry = tokens[3],
                                Category = tokens[4],
                                TypeProduct = tokens[5],
                                Description = tokens[6],
                                QuantityStock = uint.Parse(tokens[7]),
                                StatusStock = tokens[8] == "1",
                                Price = int.Parse(tokens[9])
                            };
                            resultCount++;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            DisplayResultCount();
            string historyEntry = $"По названию: '{name}' - найдено: {resultCount}";
            AddToHistory(historyEntry);
        }

        // Поиск по артикулу
        public void SearchByArticle(string filename, int article)
        {
            ClearResults();

            if (!File.Exists(filename))
            {
                Console.WriteLine($"Файл {filename} не найден!");
                return;
            }

            string[] lines = File.ReadAllLines(filename);

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                if (resultCount >= MAX_RESULTS) break;

                string[] tokens = line.Split('|');
                if (tokens.Length >= 1)
                {
                    try
                    {
                        int productArticle = int.Parse(tokens[0]);

                        if (productArticle == article)
                        {
                            if (tokens.Length >= 10)
                            {
                                searchResults[resultCount] = new SearchResult
                                {
                                    Article = productArticle,
                                    Name = tokens[1],
                                    Brand = tokens[2],
                                    ProducerCountry = tokens[3],
                                    Category = tokens[4],
                                    TypeProduct = tokens[5],
                                    Description = tokens[6],
                                    QuantityStock = uint.Parse(tokens[7]),
                                    StatusStock = tokens[8] == "1",
                                    Price = int.Parse(tokens[9])
                                };
                                resultCount++;
                            }
                            break;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            DisplayResultCount();
            string historyEntry = $"По артикулу: {article} - найдено: {resultCount}";
            AddToHistory(historyEntry);
        }

        // Поиск по бренду
        public void SearchByBrand(string filename, string brand)
        {
            ClearResults();

            if (!File.Exists(filename))
            {
                Console.WriteLine($"Файл {filename} не найден!");
                return;
            }

            string searchBrandLower = ToLower(brand);
            string[] lines = File.ReadAllLines(filename);

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                if (resultCount >= MAX_RESULTS) break;

                string[] tokens = line.Split('|');
                if (tokens.Length >= 10)
                {
                    try
                    {
                        string productBrand = tokens[2];
                        string productBrandLower = ToLower(productBrand);

                        if (productBrandLower.Contains(searchBrandLower))
                        {
                            searchResults[resultCount] = new SearchResult
                            {
                                Article = int.Parse(tokens[0]),
                                Name = tokens[1],
                                Brand = tokens[2],
                                ProducerCountry = tokens[3],
                                Category = tokens[4],
                                TypeProduct = tokens[5],
                                Description = tokens[6],
                                QuantityStock = uint.Parse(tokens[7]),
                                StatusStock = tokens[8] == "1",
                                Price = int.Parse(tokens[9])
                            };
                            resultCount++;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            DisplayResultCount();
            string historyEntry = $"По бренду: '{brand}' - найдено: {resultCount}";
            AddToHistory(historyEntry);
        }

        // Поиск по стране-производителю
        public void SearchByCountry(string filename, string country)
        {
            ClearResults();

            if (!File.Exists(filename))
            {
                Console.WriteLine($"Файл {filename} не найден!");
                return;
            }

            string searchCountryLower = ToLower(country);
            string[] lines = File.ReadAllLines(filename);

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                if (resultCount >= MAX_RESULTS) break;

                string[] tokens = line.Split('|');
                if (tokens.Length >= 10)
                {
                    try
                    {
                        string productCountry = tokens[3];
                        string productCountryLower = ToLower(productCountry);

                        if (productCountryLower.Contains(searchCountryLower))
                        {
                            searchResults[resultCount] = new SearchResult
                            {
                                Article = int.Parse(tokens[0]),
                                Name = tokens[1],
                                Brand = tokens[2],
                                ProducerCountry = tokens[3],
                                Category = tokens[4],
                                TypeProduct = tokens[5],
                                Description = tokens[6],
                                QuantityStock = uint.Parse(tokens[7]),
                                StatusStock = tokens[8] == "1",
                                Price = int.Parse(tokens[9])
                            };
                            resultCount++;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            DisplayResultCount();
            string historyEntry = $"По стране: '{country}' - найдено: {resultCount}";
            AddToHistory(historyEntry);
        }

        // Поиск по категории
        public void SearchByCategory(string filename, string category)
        {
            ClearResults();

            if (!File.Exists(filename))
            {
                Console.WriteLine($"Файл {filename} не найден!");
                return;
            }

            string searchCategoryLower = ToLower(category);
            string[] lines = File.ReadAllLines(filename);

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                if (resultCount >= MAX_RESULTS) break;

                string[] tokens = line.Split('|');
                if (tokens.Length >= 10)
                {
                    try
                    {
                        string productCategory = tokens[4];
                        string productCategoryLower = ToLower(productCategory);

                        if (productCategoryLower.Contains(searchCategoryLower))
                        {
                            searchResults[resultCount] = new SearchResult
                            {
                                Article = int.Parse(tokens[0]),
                                Name = tokens[1],
                                Brand = tokens[2],
                                ProducerCountry = tokens[3],
                                Category = tokens[4],
                                TypeProduct = tokens[5],
                                Description = tokens[6],
                                QuantityStock = uint.Parse(tokens[7]),
                                StatusStock = tokens[8] == "1",
                                Price = int.Parse(tokens[9])
                            };
                            resultCount++;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            DisplayResultCount();
            string historyEntry = $"По категории: '{category}' - найдено: {resultCount}";
            AddToHistory(historyEntry);
        }

        // Поиск по типу товара
        public void SearchByType(string filename, string type)
        {
            ClearResults();

            if (!File.Exists(filename))
            {
                Console.WriteLine($"Файл {filename} не найден!");
                return;
            }

            string searchTypeLower = ToLower(type);
            string[] lines = File.ReadAllLines(filename);

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                if (resultCount >= MAX_RESULTS) break;

                string[] tokens = line.Split('|');
                if (tokens.Length >= 10)
                {
                    try
                    {
                        string productType = tokens[5];
                        string productTypeLower = ToLower(productType);

                        if (productTypeLower.Contains(searchTypeLower))
                        {
                            searchResults[resultCount] = new SearchResult
                            {
                                Article = int.Parse(tokens[0]),
                                Name = tokens[1],
                                Brand = tokens[2],
                                ProducerCountry = tokens[3],
                                Category = tokens[4],
                                TypeProduct = tokens[5],
                                Description = tokens[6],
                                QuantityStock = uint.Parse(tokens[7]),
                                StatusStock = tokens[8] == "1",
                                Price = int.Parse(tokens[9])
                            };
                            resultCount++;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            DisplayResultCount();
            string historyEntry = $"По типу: '{type}' - найдено: {resultCount}";
            AddToHistory(historyEntry);
        }

        // Поиск по цене
        public void SearchByPrice(string filename, int minPrice, int maxPrice)
        {
            ClearResults();

            if (!File.Exists(filename))
            {
                Console.WriteLine($"Файл {filename} не найден!");
                return;
            }

            string[] lines = File.ReadAllLines(filename);

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                if (resultCount >= MAX_RESULTS) break;

                string[] tokens = line.Split('|');
                if (tokens.Length >= 10)
                {
                    try
                    {
                        int productPrice = int.Parse(tokens[9]);

                        if (productPrice >= minPrice && productPrice <= maxPrice)
                        {
                            searchResults[resultCount] = new SearchResult
                            {
                                Article = int.Parse(tokens[0]),
                                Name = tokens[1],
                                Brand = tokens[2],
                                ProducerCountry = tokens[3],
                                Category = tokens[4],
                                TypeProduct = tokens[5],
                                Description = tokens[6],
                                QuantityStock = uint.Parse(tokens[7]),
                                StatusStock = tokens[8] == "1",
                                Price = productPrice
                            };
                            resultCount++;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            DisplayResultCount();
            string historyEntry = $"По цене: от {minPrice} до {maxPrice} - найдено: {resultCount}";
            AddToHistory(historyEntry);
        }

        // Универсальный поиск (статический метод)
        public static void UniversalSearch(string filename)
        {
            Console.WriteLine("\n=== УНИВЕРСАЛЬНЫЙ ПОИСК ===");
            Console.WriteLine("Выберите критерии поиска (введите номера через запятую):");
            Console.WriteLine("1. По названию");
            Console.WriteLine("2. По артикулу");
            Console.WriteLine("3. По бренду");
            Console.WriteLine("4. По стране-производителю");
            Console.WriteLine("5. По категории");
            Console.WriteLine("6. По типу товара");
            Console.WriteLine("7. По цене");
            Console.WriteLine("Пример: 2,3 (поиск по артикулу и бренду)");
            Console.Write("Ваш выбор: ");

            string criteriaInput = Console.ReadLine();//чтение ввода пользователя
            bool[] selectedCriteria = new bool[7];
            int criteriaCount = 0;

            string[] tokens = criteriaInput.Split(',');// Разбиение введенной строки по запятым
            foreach (string token in tokens)
            {// Попытка преобразовать каждый токен в число и проверка что оно в диапазоне 1-7
                if (int.TryParse(token.Trim(), out int criterion) && criterion >= 1 && criterion <= 7)
                {
                    selectedCriteria[criterion - 1] = true;// Установка флага для выбранного критерия
                    criteriaCount++;// Увеличение счетчика выбранных критериев
                }
            }

            if (criteriaCount == 0)
            {
                Console.WriteLine("Не выбрано ни одного критерия поиска!");
                return;
            }

            // Объявление переменных для хранения поисковых запросов
            string searchName = "", searchBrand = "", searchCountry = "", searchCategory = "", searchType = "";
            int searchArticle = 0, minPrice = 0, maxPrice = 0;

            // Запрос критериев поиска
            for (int i = 0; i < 7; i++)
            {
                if (selectedCriteria[i])
                {
                    switch (i + 1)
                    {
                        case 1:
                            Console.Write("Введите название для поиска: ");
                            searchName = Console.ReadLine();
                            break;
                        case 2:
                            while (true)
                            {
                                Console.Write("Введите артикул для поиска: ");
                                if (int.TryParse(Console.ReadLine(), out searchArticle))
                                    break;
                                Console.WriteLine("Неверный ввод! Пожалуйста, введите целое число.");
                            }
                            break;
                        case 3:
                            Console.Write("Введите бренд для поиска: ");
                            searchBrand = Console.ReadLine();
                            break;
                        case 4:
                            Console.Write("Введите страну-производитель для поиска: ");
                            searchCountry = Console.ReadLine();
                            break;
                        case 5:
                            Console.Write("Введите категорию для поиска: ");
                            searchCategory = Console.ReadLine();
                            break;
                        case 6:
                            Console.Write("Введите тип товара для поиска: ");
                            searchType = Console.ReadLine();
                            break;
                        case 7:
                            while (true)
                            {
                                Console.Write("Введите минимальную цену: ");
                                if (int.TryParse(Console.ReadLine(), out minPrice))
                                    break;
                                Console.WriteLine("Неверный ввод! Пожалуйста, введите целое число.");
                            }
                            while (true)
                            {
                                Console.Write("Введите максимальную цену: ");
                                if (int.TryParse(Console.ReadLine(), out maxPrice))
                                    break;
                                Console.WriteLine("Неверный ввод! Пожалуйста, введите целое число.");
                            }
                            break;
                    }
                }
            }

            // Выполнение универсального поиска
            Search finalResults = new Search();

            if (!File.Exists(filename))
            {
                Console.WriteLine($"Файл {filename} не найден!");
                return;
            }

            string[] lines = File.ReadAllLines(filename);
            // Основной цикл поиска: проверка каждой строки файла
            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                if (finalResults.resultCount >= MAX_RESULTS) break;

                string[] lineTokens = line.Split('|');
                if (lineTokens.Length >= 10)
                {
                    try
                    {
                        bool matchesAll = true;// Флаг: удовлетворяет ли товар ВСЕМ выбранным критериям

                        // Проверяем товар по всем выбранным критериям поиска
                        for (int i = 0; i < 7 && matchesAll; i++)
                        {
                            if (selectedCriteria[i])
                            {
                                switch (i + 1)
                                {
                                    case 1:
                                        if (!lineTokens[1].ToLower().Contains(searchName.ToLower()))
                                            matchesAll = false;
                                        break;
                                    case 2:
                                        if (int.Parse(lineTokens[0]) != searchArticle)
                                            matchesAll = false;
                                        break;
                                    case 3:
                                        if (!lineTokens[2].ToLower().Contains(searchBrand.ToLower()))
                                            matchesAll = false;
                                        break;
                                    case 4:
                                        if (!lineTokens[3].ToLower().Contains(searchCountry.ToLower()))
                                            matchesAll = false;
                                        break;
                                    case 5:
                                        if (!lineTokens[4].ToLower().Contains(searchCategory.ToLower()))
                                            matchesAll = false;
                                        break;
                                    case 6:
                                        if (!lineTokens[5].ToLower().Contains(searchType.ToLower()))
                                            matchesAll = false;
                                        break;
                                    case 7:
                                        int price = int.Parse(lineTokens[9]);
                                        if (price < minPrice || price > maxPrice)
                                            matchesAll = false;
                                        break;
                                }
                            }
                        }

                        // Если товар удовлетворяет ВСЕМ выбранным критериям
                        if (matchesAll)
                        { // Добавление товара в массив результатов
                            finalResults.searchResults[finalResults.resultCount] = new SearchResult
                            {
                                Article = int.Parse(lineTokens[0]),
                                Name = lineTokens[1],
                                Brand = lineTokens[2],
                                ProducerCountry = lineTokens[3],
                                Category = lineTokens[4],
                                TypeProduct = lineTokens[5],
                                Description = lineTokens[6],
                                QuantityStock = uint.Parse(lineTokens[7]),
                                StatusStock = lineTokens[8] == "1",
                                Price = int.Parse(lineTokens[9])
                            };
                            finalResults.resultCount++;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            Console.WriteLine("\n=== РЕЗУЛЬТАТЫ УНИВЕРСАЛЬНОГО ПОИСКА ===");
            if (finalResults.resultCount == 0)
            {
                Console.WriteLine("Товары не найдены!");
            }
            else
            {
                Console.WriteLine($"Найдено товаров, удовлетворяющих ВСЕМ критериям: {finalResults.resultCount}");
                finalResults.DisplayResultCount();
                finalResults.DisplayResults();
            }
        }

        // Показать количество найденных товаров
        public int DisplayResultCount()
        {
            Console.WriteLine("\n=== КОЛИЧЕСТВО НАЙДЕННЫХ ТОВАРОВ ===");
            Console.WriteLine($"Найдено товаров: {resultCount}");
            Console.WriteLine("=====================================");
            return resultCount;
        }

        // Отобразить результаты поиска
        public void DisplayResults()
        {
            if (resultCount == 0)
            {
                Console.WriteLine("Товары не найдены!");
                return;
            }

            Console.WriteLine("\n=== РЕЗУЛЬТАТЫ ПОИСКА ===");
            Console.WriteLine($"Найдено товаров: {resultCount}");

            for (int i = 0; i < resultCount; i++)
            {
                Console.WriteLine($"\n--- Товар {i + 1} ---");
                Console.WriteLine($"Артикул: {searchResults[i].Article}");
                Console.WriteLine($"Название: {searchResults[i].Name}");
                Console.WriteLine($"Бренд: {searchResults[i].Brand}");
                Console.WriteLine($"Страна: {searchResults[i].ProducerCountry}");
                Console.WriteLine($"Категория: {searchResults[i].Category}");
                Console.WriteLine($"Тип: {searchResults[i].TypeProduct}");
                Console.WriteLine($"Описание: {searchResults[i].Description}");
                Console.WriteLine($"Количество: {searchResults[i].QuantityStock}");
                Console.WriteLine($"Статус: {(searchResults[i].StatusStock ? "В наличии" : "Отсутствует")}");
                Console.WriteLine($"Цена: {searchResults[i].Price}");
            }

            Console.WriteLine("==========================");
        }
    }
}