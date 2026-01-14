using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Курсовая_работа_Музыкальный_магазин_с_
{
    public class Search
    {
        private string request;
        private int resultCount;

        private const int MAX_RESULTS = 1000;
        private const int MAX_HISTORY = 10;

        private struct SearchResult
        {
            public int article;
            public string name;
            public string brand;
            public string producerCountry;
            public string category;
            public string typeProduct;
            public string description;
            public uint quantityStock;
            public bool statusStock;
            public int price;
        }

        private SearchResult[] searchResults;
        private string[] searchHistory;
        private int historyCount;

        public Search()
        {
            request = "";
            resultCount = 0;
            historyCount = 0;
            searchResults = new SearchResult[MAX_RESULTS];
            searchHistory = new string[MAX_HISTORY];
        }

        // Основные методы поиска
        public void SearchByName(string filename, string name)
        {
            ClearResults();
            request = "Поиск по названию: '" + name + "'";

            try
            {
                using (StreamReader file = new StreamReader(filename))
                {
                    string line;
                    string searchName = Trim(name);

                    while ((line = file.ReadLine()) != null && resultCount < MAX_RESULTS)
                    {
                        if (string.IsNullOrEmpty(line)) continue;

                        string[] parts = line.Split('|');
                        if (parts.Length >= 10)
                        {
                            try
                            {
                                string productName = parts[1];

                                if (Contains(productName, searchName))
                                {
                                    searchResults[resultCount].article = int.Parse(parts[0]);
                                    searchResults[resultCount].name = parts[1];
                                    searchResults[resultCount].brand = parts[2];
                                    searchResults[resultCount].producerCountry = parts[3];
                                    searchResults[resultCount].category = parts[4];
                                    searchResults[resultCount].typeProduct = parts[5];
                                    searchResults[resultCount].description = parts[6];
                                    searchResults[resultCount].quantityStock = uint.Parse(parts[7]);
                                    searchResults[resultCount].statusStock = (parts[8] == "1");
                                    searchResults[resultCount].price = int.Parse(parts[9]);
                                    resultCount++;
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Файл {filename} не найден!");
                return;
            }

            string historyEntry = "По названию: '" + name + "' - найдено: " + resultCount;
            AddToHistory(historyEntry);

            DisplayResultCount();
        }

        public void SearchByArticle(string filename, int article)
        {
            ClearResults();
            request = "Поиск по артикулу: " + article;

            try
            {
                using (StreamReader file = new StreamReader(filename))
                {
                    string line;

                    while ((line = file.ReadLine()) != null && resultCount < MAX_RESULTS)
                    {
                        if (string.IsNullOrEmpty(line)) continue;

                        string[] parts = line.Split('|');
                        if (parts.Length >= 10)
                        {
                            try
                            {
                                int productArticle = int.Parse(parts[0]);
                                if (productArticle == article)
                                {
                                    searchResults[resultCount].article = int.Parse(parts[0]);
                                    searchResults[resultCount].name = parts[1];
                                    searchResults[resultCount].brand = parts[2];
                                    searchResults[resultCount].producerCountry = parts[3];
                                    searchResults[resultCount].category = parts[4];
                                    searchResults[resultCount].typeProduct = parts[5];
                                    searchResults[resultCount].description = parts[6];
                                    searchResults[resultCount].quantityStock = uint.Parse(parts[7]);
                                    searchResults[resultCount].statusStock = (parts[8] == "1");
                                    searchResults[resultCount].price = int.Parse(parts[9]);
                                    resultCount++;
                                    break;
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Файл {filename} не найден!");
                return;
            }

            string historyEntry = "По артикулу: " + article + " - найдено: " + resultCount;
            AddToHistory(historyEntry);

            DisplayResultCount();
        }

        public void SearchByBrand(string filename, string brand)
        {
            ClearResults();
            request = "Поиск по бренду: '" + brand + "'";

            try
            {
                using (StreamReader file = new StreamReader(filename))
                {
                    string line;
                    string searchBrand = Trim(brand);

                    while ((line = file.ReadLine()) != null && resultCount < MAX_RESULTS)
                    {
                        if (string.IsNullOrEmpty(line)) continue;

                        string[] parts = line.Split('|');
                        if (parts.Length >= 10)
                        {
                            try
                            {
                                string productBrand = parts[2];

                                if (Contains(productBrand, searchBrand))
                                {
                                    searchResults[resultCount].article = int.Parse(parts[0]);
                                    searchResults[resultCount].name = parts[1];
                                    searchResults[resultCount].brand = parts[2];
                                    searchResults[resultCount].producerCountry = parts[3];
                                    searchResults[resultCount].category = parts[4];
                                    searchResults[resultCount].typeProduct = parts[5];
                                    searchResults[resultCount].description = parts[6];
                                    searchResults[resultCount].quantityStock = uint.Parse(parts[7]);
                                    searchResults[resultCount].statusStock = (parts[8] == "1");
                                    searchResults[resultCount].price = int.Parse(parts[9]);
                                    resultCount++;
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Файл {filename} не найден!");
                return;
            }

            string historyEntry = "По бренду: '" + brand + "' - найдено: " + resultCount;
            AddToHistory(historyEntry);

            DisplayResultCount();
        }

        public void SearchByCountry(string filename, string country)
        {
            ClearResults();
            request = "Поиск по стране: '" + country + "'";

            try
            {
                using (StreamReader file = new StreamReader(filename))
                {
                    string line;
                    string searchCountry = Trim(country);

                    while ((line = file.ReadLine()) != null && resultCount < MAX_RESULTS)
                    {
                        if (string.IsNullOrEmpty(line)) continue;

                        string[] parts = line.Split('|');
                        if (parts.Length >= 10)
                        {
                            try
                            {
                                string productCountry = parts[3];

                                if (Contains(productCountry, searchCountry))
                                {
                                    searchResults[resultCount].article = int.Parse(parts[0]);
                                    searchResults[resultCount].name = parts[1];
                                    searchResults[resultCount].brand = parts[2];
                                    searchResults[resultCount].producerCountry = parts[3];
                                    searchResults[resultCount].category = parts[4];
                                    searchResults[resultCount].typeProduct = parts[5];
                                    searchResults[resultCount].description = parts[6];
                                    searchResults[resultCount].quantityStock = uint.Parse(parts[7]);
                                    searchResults[resultCount].statusStock = (parts[8] == "1");
                                    searchResults[resultCount].price = int.Parse(parts[9]);
                                    resultCount++;
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Файл {filename} не найден!");
                return;
            }

            string historyEntry = "По стране: '" + country + "' - найдено: " + resultCount;
            AddToHistory(historyEntry);

            DisplayResultCount();
        }

        public void SearchByCategory(string filename, string category)
        {
            ClearResults();
            request = "Поиск по категории: '" + category + "'";

            try
            {
                using (StreamReader file = new StreamReader(filename))
                {
                    string line;
                    string searchCategory = Trim(category);

                    while ((line = file.ReadLine()) != null && resultCount < MAX_RESULTS)
                    {
                        if (string.IsNullOrEmpty(line)) continue;

                        string[] parts = line.Split('|');
                        if (parts.Length >= 10)
                        {
                            try
                            {
                                string productCategory = parts[4];

                                if (Contains(productCategory, searchCategory))
                                {
                                    searchResults[resultCount].article = int.Parse(parts[0]);
                                    searchResults[resultCount].name = parts[1];
                                    searchResults[resultCount].brand = parts[2];
                                    searchResults[resultCount].producerCountry = parts[3];
                                    searchResults[resultCount].category = parts[4];
                                    searchResults[resultCount].typeProduct = parts[5];
                                    searchResults[resultCount].description = parts[6];
                                    searchResults[resultCount].quantityStock = uint.Parse(parts[7]);
                                    searchResults[resultCount].statusStock = (parts[8] == "1");
                                    searchResults[resultCount].price = int.Parse(parts[9]);
                                    resultCount++;
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Файл {filename} не найден!");
                return;
            }

            string historyEntry = "По категории: '" + category + "' - найдено: " + resultCount;
            AddToHistory(historyEntry);

            DisplayResultCount();
        }

        public void SearchByType(string filename, string type)
        {
            ClearResults();
            request = "Поиск по типу: '" + type + "'";

            try
            {
                using (StreamReader file = new StreamReader(filename))
                {
                    string line;
                    string searchType = Trim(type);

                    while ((line = file.ReadLine()) != null && resultCount < MAX_RESULTS)
                    {
                        if (string.IsNullOrEmpty(line)) continue;

                        string[] parts = line.Split('|');
                        if (parts.Length >= 10)
                        {
                            try
                            {
                                string productType = parts[5];

                                if (Contains(productType, searchType))
                                {
                                    searchResults[resultCount].article = int.Parse(parts[0]);
                                    searchResults[resultCount].name = parts[1];
                                    searchResults[resultCount].brand = parts[2];
                                    searchResults[resultCount].producerCountry = parts[3];
                                    searchResults[resultCount].category = parts[4];
                                    searchResults[resultCount].typeProduct = parts[5];
                                    searchResults[resultCount].description = parts[6];
                                    searchResults[resultCount].quantityStock = uint.Parse(parts[7]);
                                    searchResults[resultCount].statusStock = (parts[8] == "1");
                                    searchResults[resultCount].price = int.Parse(parts[9]);
                                    resultCount++;
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Файл {filename} не найден!");
                return;
            }

            string historyEntry = "По типу: '" + type + "' - найдено: " + resultCount;
            AddToHistory(historyEntry);

            DisplayResultCount();
        }

        public void SearchByPrice(string filename, int minPrice, int maxPrice)
        {
            ClearResults();
            request = "Поиск по цене: от " + minPrice + " до " + maxPrice;

            try
            {
                using (StreamReader file = new StreamReader(filename))
                {
                    string line;

                    while ((line = file.ReadLine()) != null && resultCount < MAX_RESULTS)
                    {
                        if (string.IsNullOrEmpty(line)) continue;

                        string[] parts = line.Split('|');
                        if (parts.Length >= 10)
                        {
                            try
                            {
                                int productPrice = int.Parse(parts[9]);
                                if (productPrice >= minPrice && productPrice <= maxPrice)
                                {
                                    searchResults[resultCount].article = int.Parse(parts[0]);
                                    searchResults[resultCount].name = parts[1];
                                    searchResults[resultCount].brand = parts[2];
                                    searchResults[resultCount].producerCountry = parts[3];
                                    searchResults[resultCount].category = parts[4];
                                    searchResults[resultCount].typeProduct = parts[5];
                                    searchResults[resultCount].description = parts[6];
                                    searchResults[resultCount].quantityStock = uint.Parse(parts[7]);
                                    searchResults[resultCount].statusStock = (parts[8] == "1");
                                    searchResults[resultCount].price = productPrice;
                                    resultCount++;
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Файл {filename} не найден!");
                return;
            }

            string historyEntry = "По цене: от " + minPrice + " до " + maxPrice + " - найдено: " + resultCount;
            AddToHistory(historyEntry);

            DisplayResultCount();
        }

        // Универсальный поиск
        public void UniversalSearch(string filename)
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

            string criteriaInput = Console.ReadLine();
            bool[] selectedCriteria = new bool[7];
            int criteriaCount = 0;

            string[] tokens = criteriaInput.Split(',');
            foreach (string token in tokens)
            {
                string trimmedToken = Trim(token);
                if (int.TryParse(trimmedToken, out int criterion) && criterion >= 1 && criterion <= 7)
                {
                    selectedCriteria[criterion - 1] = true;
                    criteriaCount++;
                }
            }

            if (criteriaCount == 0)
            {
                Console.WriteLine("Не выбрано ни одного критерия поиска!");
                return;
            }

            // Сбор параметров поиска
            string[] searchParams = new string[7];
            int searchArticle = 0;
            int minPrice = 0, maxPrice = 0;

            for (int i = 0; i < 7; i++)
            {
                if (selectedCriteria[i])
                {
                    switch (i + 1)
                    {
                        case 1:
                            Console.Write("Введите название для поиска: ");
                            searchParams[0] = Console.ReadLine();
                            break;
                        case 2:
                            while (true)
                            {
                                Console.Write("Введите артикул для поиска: ");
                                string input = Console.ReadLine();
                                if (int.TryParse(Trim(input), out searchArticle))
                                {
                                    break;
                                }
                                Console.WriteLine("Неверный ввод! Пожалуйста, введите целое число.");
                            }
                            break;
                        case 3:
                            Console.Write("Введите бренд для поиска: ");
                            searchParams[2] = Console.ReadLine();
                            break;
                        case 4:
                            Console.Write("Введите страну-производитель для поиска: ");
                            searchParams[3] = Console.ReadLine();
                            break;
                        case 5:
                            Console.Write("Введите категорию для поиска: ");
                            searchParams[4] = Console.ReadLine();
                            break;
                        case 6:
                            Console.Write("Введите тип товара для поиска: ");
                            searchParams[5] = Console.ReadLine();
                            break;
                        case 7:
                            while (true)
                            {
                                Console.Write("Введите минимальную цену: ");
                                string input = Console.ReadLine();
                                if (int.TryParse(Trim(input), out minPrice))
                                {
                                    break;
                                }
                                Console.WriteLine("Неверный ввод! Пожалуйста, введите целое число.");
                            }
                            while (true)
                            {
                                Console.Write("Введите максимальную цену: ");
                                string input = Console.ReadLine();
                                if (int.TryParse(Trim(input), out maxPrice))
                                {
                                    break;
                                }
                                Console.WriteLine("Неверный ввод! Пожалуйста, введите целое число.");
                            }
                            break;
                    }
                }
            }

            // Выполнение поиска
            Search finalResults = new Search();

            try
            {
                using (StreamReader file = new StreamReader(filename))
                {
                    string line;
                    while ((line = file.ReadLine()) != null && finalResults.resultCount < MAX_RESULTS)
                    {
                        if (string.IsNullOrEmpty(line)) continue;

                        string[] parts = line.Split('|');
                        if (parts.Length >= 10)
                        {
                            try
                            {
                                bool matchesAll = true;

                                for (int i = 0; i < 7 && matchesAll; i++)
                                {
                                    if (selectedCriteria[i])
                                    {
                                        switch (i + 1)
                                        {
                                            case 1:
                                                if (!Contains(parts[1], searchParams[0]))
                                                {
                                                    matchesAll = false;
                                                }
                                                break;
                                            case 2:
                                                if (int.Parse(parts[0]) != searchArticle)
                                                {
                                                    matchesAll = false;
                                                }
                                                break;
                                            case 3:
                                                if (!Contains(parts[2], searchParams[2]))
                                                {
                                                    matchesAll = false;
                                                }
                                                break;
                                            case 4:
                                                if (!Contains(parts[3], searchParams[3]))
                                                {
                                                    matchesAll = false;
                                                }
                                                break;
                                            case 5:
                                                if (!Contains(parts[4], searchParams[4]))
                                                {
                                                    matchesAll = false;
                                                }
                                                break;
                                            case 6:
                                                if (!Contains(parts[5], searchParams[5]))
                                                {
                                                    matchesAll = false;
                                                }
                                                break;
                                            case 7:
                                                int price = int.Parse(parts[9]);
                                                if (price < minPrice || price > maxPrice)
                                                {
                                                    matchesAll = false;
                                                }
                                                break;
                                        }
                                    }
                                }

                                if (matchesAll)
                                {
                                    finalResults.searchResults[finalResults.resultCount].article = int.Parse(parts[0]);
                                    finalResults.searchResults[finalResults.resultCount].name = parts[1];
                                    finalResults.searchResults[finalResults.resultCount].brand = parts[2];
                                    finalResults.searchResults[finalResults.resultCount].producerCountry = parts[3];
                                    finalResults.searchResults[finalResults.resultCount].category = parts[4];
                                    finalResults.searchResults[finalResults.resultCount].typeProduct = parts[5];
                                    finalResults.searchResults[finalResults.resultCount].description = parts[6];
                                    finalResults.searchResults[finalResults.resultCount].quantityStock = uint.Parse(parts[7]);
                                    finalResults.searchResults[finalResults.resultCount].statusStock = (parts[8] == "1");
                                    finalResults.searchResults[finalResults.resultCount].price = int.Parse(parts[9]);
                                    finalResults.resultCount++;
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Файл {filename} не найден!");
                return;
            }

            Console.WriteLine("\n=== РЕЗУЛЬТАТЫ УНИВЕРСАЛЬНОГО ПОИСКА ===");
            if (finalResults.resultCount == 0)
            {
                Console.WriteLine("Товары не найдены!");
            }
            else
            {
                // Создаем описание поиска для истории
                string[] criteriaDesc = new string[7];
                int descCount = 0;

                if (selectedCriteria[0])
                {
                    criteriaDesc[descCount++] = "названию: '" + searchParams[0] + "'";
                }
                if (selectedCriteria[1])
                {
                    criteriaDesc[descCount++] = "артикулу: " + searchArticle;
                }
                if (selectedCriteria[2])
                {
                    criteriaDesc[descCount++] = "бренду: '" + searchParams[2] + "'";
                }
                if (selectedCriteria[3])
                {
                    criteriaDesc[descCount++] = "стране: '" + searchParams[3] + "'";
                }
                if (selectedCriteria[4])
                {
                    criteriaDesc[descCount++] = "категории: '" + searchParams[4] + "'";
                }
                if (selectedCriteria[5])
                {
                    criteriaDesc[descCount++] = "типу: '" + searchParams[5] + "'";
                }
                if (selectedCriteria[6])
                {
                    criteriaDesc[descCount++] = "цене: " + minPrice + "-" + maxPrice;
                }

                string criteriaString = JoinStrings(criteriaDesc, descCount, ", ");

                string historyEntry = "Универсальный поиск по " + criteriaString +
                    " - найдено: " + finalResults.resultCount;

                finalResults.AddToHistory(historyEntry);

                Console.WriteLine("Найдено товаров, удовлетворяющих ВСЕМ критериям: " + finalResults.resultCount);
                finalResults.DisplayResults();
            }
        }

        // Методы отображения и управления
        public void DisplayResults()
        {
            if (resultCount == 0)
            {
                Console.WriteLine("\n————————————————————————");
                Console.WriteLine("По запросу \"" + request + "\" ничего не найдено.");
                Console.WriteLine("————————————————————————");
                return;
            }

            Console.WriteLine("\n═══════════════════════════════════════════════");
            Console.WriteLine("РЕЗУЛЬТАТЫ ПОИСКА (" + resultCount + " товаров)");
            Console.WriteLine("Запрос: \"" + request + "\"");
            Console.WriteLine("═══════════════════════════════════════════════\n");

            for (int i = 0; i < resultCount; i++)
            {
                var product = searchResults[i];

                Console.WriteLine("┌── ТОВАР №" + (i + 1) + " ───────────────────────");
                Console.WriteLine("│  Название:  " + product.name);
                Console.WriteLine("│  Бренд:     " + product.brand);
                Console.WriteLine("│  Цена:      " + product.price + " руб.");
                Console.Write("│  Наличие:   ");

                if (product.statusStock)
                {
                    Console.WriteLine(product.quantityStock + " шт. на складе");
                }
                else
                {
                    Console.WriteLine("Товар отсутствует");
                }

                // Вывод описания с правильным переносом
                if (!string.IsNullOrEmpty(product.description))
                {
                    Console.Write("│  Описание:  ");

                    string desc = product.description;
                    const int maxLineLength = 50;
                    int charsPrinted = 0;

                    // Перенос длинного описания
                    while (charsPrinted < desc.Length)
                    {
                        int remaining = desc.Length - charsPrinted;
                        int printLength = Math.Min(maxLineLength, remaining);

                        if (charsPrinted == 0)
                        {
                            Console.Write(desc.Substring(charsPrinted, printLength));
                        }
                        else
                        {
                            Console.Write("\n│             " + desc.Substring(charsPrinted, printLength));
                        }

                        charsPrinted += printLength;

                        if (remaining > maxLineLength && charsPrinted >= maxLineLength)
                        {
                            Console.Write("...");
                            break;
                        }
                    }
                    Console.WriteLine();
                }

                Console.WriteLine("└─────────────────────────────────────────");

                // Разделитель между товарами, кроме последнего
                if (i < resultCount - 1)
                {
                    Console.WriteLine();
                }
            }

            Console.WriteLine("\n═══════════════════════════════════════════════");

            // Полезные подсказки
            if (resultCount > 0)
            {
                Console.WriteLine("СОВЕТ: Для оформления заказа введите номер нужного товара.");

                // Проверяем, есть ли отсутствующие товары
                bool hasOutOfStock = false;
                for (int i = 0; i < resultCount; i++)
                {
                    if (!searchResults[i].statusStock)
                    {
                        hasOutOfStock = true;
                        break;
                    }
                }

                if (hasOutOfStock)
                {
                    Console.WriteLine("       Отсутствующие товары можно заказать у поставщика.");
                }
            }
            Console.WriteLine("═══════════════════════════════════════════════");
        }

        public int DisplayResultCount()
        {
            Console.WriteLine("\n=== КОЛИЧЕСТВО НАЙДЕННЫХ ТОВАРОВ ===");
            Console.WriteLine("Найдено товаров: " + resultCount);

            if (resultCount > 0)
            {
                string resultInfo = "Результаты поиска '" + request + "': ";
                resultInfo += resultCount + " товар";

                // Правильное склонение слова "товар"
                int lastTwoDigits = resultCount % 100;
                int lastDigit = resultCount % 10;

                if (lastTwoDigits >= 11 && lastTwoDigits <= 19)
                {
                    resultInfo += "ов";
                }
                else if (lastDigit == 1)
                {
                    // ничего не добавляем
                }
                else if (lastDigit >= 2 && lastDigit <= 4)
                {
                    resultInfo += "а";
                }
                else
                {
                    resultInfo += "ов";
                }

                Console.WriteLine(resultInfo);
            }

            Console.WriteLine("=====================================");
            return resultCount;
        }

        public void ClearResults()
        {
            resultCount = 0;
        }

        public void AddToHistory(string searchInfo)
        {
            if (historyCount >= MAX_HISTORY)
            {
                for (int i = 0; i < MAX_HISTORY - 1; i++)
                {
                    searchHistory[i] = searchHistory[i + 1];
                }
                historyCount = MAX_HISTORY - 1;
            }

            searchHistory[historyCount] = searchInfo;
            historyCount++;
        }

        public void DisplaySearchHistory()
        {
            Console.WriteLine("\n=== ИСТОРИЯ ПОИСКА ===");

            if (historyCount == 0)
            {
                Console.WriteLine("История поиска пуста!");
                Console.WriteLine("==========================");
                return;
            }

            Console.WriteLine("Последние " + historyCount + " запросов:");
            Console.WriteLine("==========================");

            for (int i = historyCount - 1; i >= 0; i--)
            {
                Console.WriteLine("Запрос #" + (historyCount - i) + ": " + searchHistory[i]);
            }
            Console.WriteLine("==========================");
        }

        // Новые методы для работы с историей
        public string GetSearchHistoryAsString()
        {
            if (historyCount == 0) return "История поиска пуста";

            string result = "История поиска (" + historyCount + " запросов):\n";
            for (int i = 0; i < historyCount; i++)
            {
                result += (i + 1) + ". " + searchHistory[i] + "\n";
            }
            return result;
        }

        public void ClearSearchHistory()
        {
            for (int i = 0; i < MAX_HISTORY; i++)
            {
                searchHistory[i] = "";
            }
            historyCount = 0;
            Console.WriteLine("История поиска очищена!");
        }

        public string GetLastSearchRequest()
        {
            return request;
        }

        public bool HasSearchHistory()
        {
            return historyCount > 0;
        }

        // Вспомогательные методы для работы со строками
        private string ToLower(string str)
        {
            return str.ToLower();
        }

        private string Trim(string str)
        {
            return str.Trim();
        }

        private bool Contains(string str, string substr)
        {
            if (string.IsNullOrEmpty(substr)) return true;
            return ToLower(str).Contains(ToLower(substr));
        }

        private string JoinStrings(string[] parts, int count, string delimiter)
        {
            if (count == 0) return "";

            string result = parts[0];
            for (int i = 1; i < count; i++)
            {
                result += delimiter + parts[i];
            }
            return result;
        }
    }
}
