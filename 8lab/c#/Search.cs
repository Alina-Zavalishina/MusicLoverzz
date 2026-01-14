using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
namespace ShopSystem
{
    public class Search : IDisposable
    {
        private List<Product> searchResults = new List<Product>(); // Результаты поиска
        private List<string> searchHistory = new List<string>(); // История поисковых запросов

        // Конструктор
        public Search()
        {
            searchResults.Capacity = 100;
            searchHistory.Capacity = 50;
        }

        // Деструктор (реализация IDisposable)
        public void Dispose()
        {
            ClearResults();
        }

        private List<Product> LoadProductsFromFile(string filename)
        {
            var products = new List<Product>();

            if (!File.Exists(filename))
            {
                Console.WriteLine($"Файл {filename} не найден!");
                return products;
            }

            try
            {
                var lines = File.ReadAllLines(filename);

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var tokens = line.Split('|');

                    if (tokens.Length >= 10)
                    {
                        try
                        {

                            var product = new Product(
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
                            products.Add(product);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ошибка при создании продукта из строки: {line}");
                            Console.WriteLine($"Ошибка: {ex.Message}");
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
            }

            return products;
        }

        // Проверка соответствия критериям
        private bool MatchesCriteria(Product product, string field, string value)
        {
            if (product == null || string.IsNullOrEmpty(value)) return false;

            string productValue = field.ToLower() switch
            {
                "name" => product.GetName(),
                "brand" => product.GetBrand(),
                "country" => product.GetProducerCountry(),
                "category" => product.GetCategory(),
                "type" => product.GetTypeProduct(),
                _ => string.Empty
            };

            if (string.IsNullOrEmpty(productValue)) return false;

            return productValue.ToLower().Contains(value.ToLower());
        }

        // Проверка ценового диапазона
        private bool MatchesPriceRange(Product product, int minPrice, int maxPrice)
        {
            if (product == null) return false;
            int price = product.GetPrice();
            return price >= minPrice && price <= maxPrice;
        }

        // Поиск по названию
        public void SearchByName(string filename, string name)
        {
            ClearResults();
            var products = LoadProductsFromFile(filename);


            searchResults = products.Where(p =>
            {
                string productName = p.GetName().ToLower();
                string searchName = name.ToLower();
                return productName.Contains(searchName);
            }).ToList();

            SaveSearchToHistory("По названию: " + name);
        }

        // Поиск по артикулу
        public void SearchByArticle(string filename, int article)
        {
            ClearResults();
            var products = LoadProductsFromFile(filename);

            // LINQ FirstOrDefault аналогичен std::find_if
            var foundProduct = products.FirstOrDefault(p => p.GetArticle() == article);

            if (foundProduct != null)
            {
                searchResults.Add(foundProduct);
            }

            SaveSearchToHistory("По артикулу: " + article);
        }

        // Поиск по бренду
        public void SearchByBrand(string filename, string brand)
        {
            ClearResults();
            var products = LoadProductsFromFile(filename);

            searchResults = products.Where(p => MatchesCriteria(p, "brand", brand)).ToList();
            SaveSearchToHistory("По бренду: " + brand);
        }

        // Поиск по стране
        public void SearchByCountry(string filename, string country)
        {
            ClearResults();
            var products = LoadProductsFromFile(filename);

            searchResults = products.Where(p => MatchesCriteria(p, "country", country)).ToList();
            SaveSearchToHistory("По стране: " + country);
        }

        // Поиск по категории
        public void SearchByCategory(string filename, string category)
        {
            ClearResults();
            var products = LoadProductsFromFile(filename);

            searchResults = products.Where(p => MatchesCriteria(p, "category", category)).ToList();
            SaveSearchToHistory("По категории: " + category);
        }

        // Поиск по типу
        public void SearchByType(string filename, string type)
        {
            ClearResults();
            var products = LoadProductsFromFile(filename);

            searchResults = products.Where(p => MatchesCriteria(p, "type", type)).ToList();
            SaveSearchToHistory("По типу: " + type);
        }

        // Поиск по цене
        public void SearchByPrice(string filename, int minPrice, int maxPrice)
        {
            ClearResults();
            var products = LoadProductsFromFile(filename);

            searchResults = products.Where(p => MatchesPriceRange(p, minPrice, maxPrice)).ToList();
            SaveSearchToHistory($"По цене: {minPrice}-{maxPrice}");
        }

        // Универсальный поиск
        public void UniversalSearch(string filename)
        {
            ClearResults();
            var products = LoadProductsFromFile(filename);

            Console.WriteLine("\n=== УНИВЕРСАЛЬНЫЙ ПОИСК ===");
            Console.WriteLine("Введите критерии поиска (оставьте пустым для пропуска):");

            Console.Write("Название: ");
            string name = Console.ReadLine();

            Console.Write("Бренд: ");
            string brand = Console.ReadLine();

            Console.Write("Страна: ");
            string country = Console.ReadLine();

            Console.Write("Категория: ");
            string category = Console.ReadLine();

            Console.Write("Тип: ");
            string type = Console.ReadLine();

            Console.Write("Минимальная цена: ");
            string priceInput = Console.ReadLine();
            int minPrice = string.IsNullOrEmpty(priceInput) ? 0 : int.Parse(priceInput);

            Console.Write("Максимальная цена: ");
            priceInput = Console.ReadLine();
            int maxPrice = string.IsNullOrEmpty(priceInput) ? int.MaxValue : int.Parse(priceInput);

            searchResults = products.Where(p =>
            {
                bool matches = true;

                if (!string.IsNullOrEmpty(name))
                    matches &= MatchesCriteria(p, "name", name);
                if (!string.IsNullOrEmpty(brand))
                    matches &= MatchesCriteria(p, "brand", brand);
                if (!string.IsNullOrEmpty(country))
                    matches &= MatchesCriteria(p, "country", country);
                if (!string.IsNullOrEmpty(category))
                    matches &= MatchesCriteria(p, "category", category);
                if (!string.IsNullOrEmpty(type))
                    matches &= MatchesCriteria(p, "type", type);

                matches &= MatchesPriceRange(p, minPrice, maxPrice);

                return matches;
            }).ToList();

            SaveSearchToHistory("Универсальный поиск");
        }

        // Найти товар с минимальной ценой 
        public Product FindMinPriceProduct()
        {
            if (searchResults.Count == 0) return null;

            int minPrice = searchResults.Min(p => p.GetPrice());
            return searchResults.FirstOrDefault(p => p.GetPrice() == minPrice);
        }

        // Найти товар с максимальной ценой 
        public Product FindMaxPriceProduct()
        {
            if (searchResults.Count == 0) return null;

            int maxPrice = searchResults.Max(p => p.GetPrice());
            return searchResults.FirstOrDefault(p => p.GetPrice() == maxPrice);
        }

        // Фильтрация результатов 
        public List<Product> FilterByCriteria(Func<Product, bool> predicate)
        {
            return searchResults.Where(predicate).ToList();
        }

        // Трансформация результатов 
        public List<T> TransformProducts<T>(Func<Product, T> transformer)
        {
            return searchResults.Select(transformer).ToList();
        }

        // Проверка наличия хотя бы одного элемента 
        public bool AnyProductMatches(Func<Product, bool> predicate)
        {
            return searchResults.Any(predicate);
        }

        // Удаление элементов по условию 
        public void RemoveFromResults(Func<Product, bool> predicate)
        {
            var itemsToRemove = searchResults.Where(predicate).ToList();

            foreach (var item in itemsToRemove)
            {
                searchResults.Remove(item);
                item.Dispose(); // если Product реализует IDisposable
            }
        }

        // Копирование с условием
        private void CopyProducts(List<Product> source, List<Product> destination, Func<Product, bool> predicate = null)
        {
            if (predicate != null)
            {
                destination.AddRange(source.Where(predicate));
            }
            else
            {
                destination.AddRange(source);
            }
        }

        // Сортировка результатов 
        public void SortResults(bool ascending = true)
        {
            if (ascending)
            {
                searchResults = searchResults.OrderBy(p => p.GetPrice()).ToList();
            }
            else
            {
                searchResults = searchResults.OrderByDescending(p => p.GetPrice()).ToList();
            }
        }

        // Вывод результатов
        public void DisplayResults()
        {
            Console.WriteLine("\n=== РЕЗУЛЬТАТЫ ПОИСКА ===");

            if (searchResults.Count == 0)
            {
                Console.WriteLine("Ничего не найдено!");
                return;
            }

            Console.WriteLine($"Найдено товаров: {searchResults.Count}");

            int i = 1;
            foreach (var product in searchResults)
            {
                Console.WriteLine($"\n--- Товар {i++} ---");
                product.DisplayInfo();
            }

            var minProduct = FindMinPriceProduct();
            var maxProduct = FindMaxPriceProduct();

            if (minProduct != null && maxProduct != null)
            {
                Console.WriteLine("\n=== СТАТИСТИКА ===");
                Console.WriteLine($"Самый дешевый: {minProduct.GetName()} ({minProduct.GetPrice()} руб.)");
                Console.WriteLine($"Самый дорогой: {maxProduct.GetName()} ({maxProduct.GetPrice()} руб.)");
            }
        }

        // Очистка результатов
        public void ClearResults()
        {
            foreach (var product in searchResults)
            {
                product.Dispose(); // если Product реализует IDisposable
            }

            searchResults.Clear();
        }

        // Сохранение в историю
        public void SaveSearchToHistory(string query)
        {
            searchHistory.Add($"{query} | Найдено: {searchResults.Count}");

            if (searchHistory.Count > 10)
            {
                searchHistory.RemoveAt(0);
            }
        }

        // Показать историю поиска
        public void DisplaySearchHistory()
        {
            Console.WriteLine("\n=== ИСТОРИЯ ПОИСКА ===");

            if (searchHistory.Count == 0)
            {
                Console.WriteLine("История поиска пуста!");
                return;
            }

            int i = 1;
            for (int j = searchHistory.Count - 1; j >= 0; j--)
            {
                Console.WriteLine($"{i++}. {searchHistory[j]}");
            }
        }

        // Очистка истории
        public void ClearSearchHistory()
        {
            searchHistory.Clear();
        }

        // Геттеры
        public List<Product> GetResults() => searchResults;
        public int GetResultCount() => searchResults.Count;
        public List<string> GetHistory() => searchHistory;
    }
}
