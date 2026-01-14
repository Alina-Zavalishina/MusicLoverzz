using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Курсовая_работа_Музыкальный_магазин_с_
{
    public class Product
    {
        private int article;
        private string name;
        private string brand;
        private string producerCountry;
        private string category;
        private string typeProduct;
        private string description;
        private uint quantityStock;
        private bool statusStock;
        private int price;

        // Конструкторы
        public Product()
        {
            article = 0;
            name = "";
            brand = "";
            producerCountry = "";
            category = "";
            typeProduct = "";
            description = "";
            quantityStock = 0;
            statusStock = false;
            price = 0;
        }

        public Product(int art, string nm, string br, string country, string cat,
                      string type, string desc, uint quantity, int pr)
        {
            article = art;
            name = nm;
            brand = br;
            producerCountry = country;
            category = cat;
            typeProduct = type;
            description = desc;
            quantityStock = quantity;
            statusStock = quantity > 0;
            price = pr;
        }

        // Методы работы со складом
        public void AddStock()
        {
            InputFromKeyboard();
            Console.WriteLine("Товар успешно добавлен на склад!");
        }

        public bool DeleteStock()
        {
            if (quantityStock > 0)
            {
                quantityStock = 0;
                statusStock = false;
                Console.WriteLine("Товар удален со склада!");
                return true;
            }
            Console.WriteLine("Товар уже отсутствует на складе!");
            return false;
        }

        public void EditStock()
        {
            Console.WriteLine("=== Редактирование товара ===");
            InputFromKeyboard();
            Console.WriteLine("Товар успешно отредактирован!");
        }

        public bool FindStock()
        {
            return statusStock;
        }

        public void DisplayInfo()
        {
            Console.WriteLine("\n=== Информация о товаре ===");
            Console.WriteLine($"Артикул: {article}");
            Console.WriteLine($"Название: {name}");
            Console.WriteLine($"Бренд: {brand}");
            Console.WriteLine($"Страна-производитель: {producerCountry}");
            Console.WriteLine($"Категория: {category}");
            Console.WriteLine($"Тип товара: {typeProduct}");
            Console.WriteLine($"Описание: {description}");
            Console.WriteLine($"Количество на складе: {quantityStock}");
            Console.WriteLine($"Статус на складе: {(statusStock ? "В наличии" : "Отсутствует")}");
            Console.WriteLine($"Цена: {price}");
        }

        // Методы ввода/вывода данных
        public void InputFromKeyboard()
        {
            Console.WriteLine("\n=== Ввод данных о товаре ===");

            // Ввод артикула
            while (true)
            {
                Console.Write("Введите артикул: ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out article) && ValidateNumber(article, 10))
                {
                    break;
                }
                Console.WriteLine("Ошибка! Артикул должен быть неотрицательным числом до 10 цифр.");
            }

            // Ввод названия
            while (true)
            {
                Console.Write("Введите название: ");
                name = Console.ReadLine();
                if (ValidateNonEmpty(name) && ValidateString(name, 30))
                {
                    break;
                }
                Console.WriteLine("Ошибка! Название не может быть пустым и не должно превышать 30 символов.");
            }

            // Ввод бренда
            while (true)
            {
                Console.Write("Введите бренд: ");
                brand = Console.ReadLine();
                if (ValidateNonEmpty(brand) && ValidateString(brand, 30))
                {
                    break;
                }
                Console.WriteLine("Ошибка! Бренд не может быть пустым и не должен превышать 30 символов.");
            }

            // Ввод страны-производителя
            while (true)
            {
                Console.Write("Введите страну-производитель: ");
                producerCountry = Console.ReadLine();
                if (ValidateNonEmpty(producerCountry) && ValidateString(producerCountry, 30))
                {
                    break;
                }
                Console.WriteLine("Ошибка! Страна-производитель не может быть пустой и не должна превышать 30 символов.");
            }

            // Ввод категории
            while (true)
            {
                Console.Write("Введите категорию: ");
                category = Console.ReadLine();
                if (ValidateNonEmpty(category) && ValidateString(category, 30))
                {
                    break;
                }
                Console.WriteLine("Ошибка! Категория не может быть пустой и не должна превышать 30 символов.");
            }

            // Ввод типа товара
            while (true)
            {
                Console.Write("Введите тип товара: ");
                typeProduct = Console.ReadLine();
                if (ValidateNonEmpty(typeProduct) && ValidateString(typeProduct, 30))
                {
                    break;
                }
                Console.WriteLine("Ошибка! Тип товара не может быть пустым и не должен превышать 30 символов.");
            }

            // Ввод описания
            while (true)
            {
                Console.Write("Введите описание: ");
                description = Console.ReadLine();
                if (ValidateNonEmpty(description))
                {
                    break;
                }
                Console.WriteLine("Ошибка! Описание не может быть пустым.");
            }

            // Ввод количества на складе
            while (true)
            {
                Console.Write("Введите количество на складе: ");
                string input = Console.ReadLine();
                if (uint.TryParse(input, out quantityStock) && ValidateNumber(quantityStock, 10))
                {
                    break;
                }
                Console.WriteLine("Ошибка! Количество должно быть неотрицательным числом до 10 цифр.");
            }

            // Ввод статуса товара
            while (true)
            {
                Console.Write("Введите статус товара (1 - В наличии, 0 - Отсутствует): ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out int statusInput) && (statusInput == 0 || statusInput == 1))
                {
                    statusStock = (statusInput == 1);
                    break;
                }
                Console.WriteLine("Ошибка! Введите только 1 или 0.");
            }

            // Ввод цены
            while (true)
            {
                Console.Write("Введите цену: ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out price) && price >= 0 && ValidateNumber(price, 10))
                {
                    break;
                }
                Console.WriteLine("Ошибка! Цена не может быть отрицательной и должна быть числом до 10 цифр.");
            }
        }

        public void WriteToFile(string filename)
        {
            try
            {
                using (StreamWriter file = new StreamWriter(filename, true))
                {
                    file.WriteLine($"{article}|{name}|{brand}|{producerCountry}|{category}|{typeProduct}|{description}|{quantityStock}|{(statusStock ? 1 : 0)}|{price}");
                    Console.WriteLine($"Данные успешно записаны в файл: {filename}");
                }
            }
            catch
            {
                Console.WriteLine("Ошибка открытия файла для записи!");
            }
        }

        public void ReadFromFile(string filename)
        {
            try
            {
                using (StreamReader file = new StreamReader(filename))
                {
                    string line = file.ReadLine();
                    if (line != null)
                    {
                        string[] parts = line.Split('|');
                        if (parts.Length >= 10)
                        {
                            article = int.Parse(parts[0]);
                            name = parts[1];
                            brand = parts[2];
                            producerCountry = parts[3];
                            category = parts[4];
                            typeProduct = parts[5];
                            description = parts[6];
                            quantityStock = uint.Parse(parts[7]);
                            statusStock = parts[8] == "1";
                            price = int.Parse(parts[9]);
                            Console.WriteLine($"Данные успешно прочитаны из файла: {filename}");
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine("Ошибка открытия файла для чтения!");
            }
        }

        // Статические методы для работы с файлами
        public static void DisplayProductsTable(string filename)
        {
            var allProducts = LoadAllProductsSmart(filename);
            if (allProducts.Count == 0)
            {
                Console.WriteLine($"\nФайл {filename} не найден или пуст!");
                Console.WriteLine("Сначала добавьте товары через меню 'Добавить товар'");
                return;
            }

            Console.WriteLine("\n=== ИНФОРМАЦИЯ О ТОВАРАХ ===");
            for (int i = 0; i < allProducts.Count; i++)
            {
                Console.WriteLine($"\n--- Товар {i + 1} ---");
                allProducts[i].DisplayInfo();
            }

            Console.WriteLine($"\n=================================");
            Console.WriteLine($"Всего товаров: {allProducts.Count}");
            Console.WriteLine($"=================================");
        }

        public static void DeleteProductsFromFile(string filename)
        {
            Console.WriteLine("\n=== УДАЛЕНИЕ ТОВАРОВ СО СКЛАДА ===");
            var allProducts = LoadAllProductsSmart(filename);
            if (allProducts.Count == 0)
            {
                Console.WriteLine("Нет товаров для удаления!");
                return;
            }

            Console.WriteLine("\nСписок товаров:");
            for (int i = 0; i < allProducts.Count; i++)
            {
                Console.WriteLine($"\n--- Товар {i + 1} ---");
                allProducts[i].DisplayInfo();
            }

            int productCount;
            while (true)
            {
                Console.Write("\nСколько товаров хотите удалить? ");
                if (int.TryParse(Console.ReadLine(), out productCount))
                {
                    if (productCount < 0)
                    {
                        Console.WriteLine("Ошибка! Количество не может быть отрицательным.");
                        continue;
                    }
                    if (productCount == 0)
                    {
                        Console.WriteLine("Удаление отменено.");
                        return;
                    }
                    break;
                }
                Console.WriteLine("Ошибка! Введите целое число.");
            }

            Console.Write($"\nВы уверены, что хотите удалить {productCount} товар(ов)? (y/n): ");
            char confirmation = Console.ReadKey().KeyChar;
            Console.WriteLine();

            if (confirmation != 'y' && confirmation != 'Y')
            {
                Console.WriteLine("Удаление отменено.");
                return;
            }

            int successfullyDeleted = 0;
            for (int i = 0; i < productCount; i++)
            {
                Console.Write($"\nВведите артикул товара для удаления {i + 1}/{productCount}: ");
                if (int.TryParse(Console.ReadLine(), out int articleToDelete))
                {
                    if (DeleteProductByArticle(filename, articleToDelete))
                    {
                        successfullyDeleted++;
                    }
                }
            }

            Console.WriteLine("\n=== РЕЗУЛЬТАТ УДАЛЕНИЯ ===");
            Console.WriteLine($"Запрошено к удалению: {productCount} товар(ов)");
            Console.WriteLine($"Успешно удалено: {successfullyDeleted} товар(ов)");

            var updatedProducts = LoadAllProductsSmart(filename);
            Console.WriteLine("\n=== ОБНОВЛЕННЫЙ СПИСОК ТОВАРОВ ===");
            for (int i = 0; i < updatedProducts.Count; i++)
            {
                Console.WriteLine($"\n--- Товар {i + 1} ---");
                updatedProducts[i].DisplayInfo();
            }
        }

        public static bool DeleteProductByArticle(string filename, int articleToDelete)
        {
            var allProducts = LoadAllProductsSmart(filename);
            var productToDelete = allProducts.FirstOrDefault(p => p.GetArticle() == articleToDelete);
            if (productToDelete == null)
            {
                Console.WriteLine($"Товар с артикулом {articleToDelete} не найден!");
                return false;
            }

            Console.WriteLine($"Найден товар для удаления: {productToDelete.GetName()}");
            allProducts.Remove(productToDelete);

            try
            {
                using (StreamWriter outFile = new StreamWriter(filename))
                {
                    foreach (var product in allProducts)
                    {
                        outFile.WriteLine($"{product.GetArticle()}|{product.GetName()}|{product.GetBrand()}|{product.GetProducerCountry()}|{product.GetCategory()}|{product.GetTypeProduct()}|{product.GetDescription()}|{product.GetQuantityStock()}|{(product.GetStatusStock() ? 1 : 0)}|{product.GetPrice()}");
                    }
                }

                Console.WriteLine($"Товар с артикулом {articleToDelete} успешно удален!");
                return true;
            }
            catch
            {
                Console.WriteLine("Ошибка открытия файла для записи!");
                return false;
            }
        }

        public static void EditProductsInFile(string filename)
        {
            Console.WriteLine("\n=== РЕДАКТИРОВАНИЕ ТОВАРОВ ===");
            var allProducts = LoadAllProductsSmart(filename);
            if (allProducts.Count == 0)
            {
                Console.WriteLine("Нет товаров для редактирования!");
                return;
            }

            Console.WriteLine("\nСписок товаров:");
            for (int i = 0; i < allProducts.Count; i++)
            {
                Console.WriteLine($"\n--- Товар {i + 1} ---");
                allProducts[i].DisplayInfo();
            }

            int productCount;
            while (true)
            {
                Console.Write("\nСколько товаров хотите отредактировать? ");
                if (int.TryParse(Console.ReadLine(), out productCount))
                {
                    if (productCount < 0)
                    {
                        Console.WriteLine("Ошибка! Количество не может быть отрицательным.");
                        continue;
                    }
                    if (productCount == 0)
                    {
                        Console.WriteLine("Редактирование отменено.");
                        return;
                    }
                    break;
                }
                Console.WriteLine("Ошибка! Введите целое число.");
            }

            int successfullyEdited = 0;
            for (int i = 0; i < productCount; i++)
            {
                Console.Write($"\nВведите порядковый номер товара для редактирования {i + 1}/{productCount}: ");
                if (int.TryParse(Console.ReadLine(), out int productIndex))
                {
                    if (EditProductByIndex(filename, productIndex))
                    {
                        successfullyEdited++;
                    }
                }
            }

            Console.WriteLine("\n=== РЕЗУЛЬТАТ РЕДАКТИРОВАНИЯ ===");
            Console.WriteLine($"Запрошено к редактированию: {productCount} товар(ов)");
            Console.WriteLine($"Успешно отредактировано: {successfullyEdited} товар(ов)");

            var updatedProducts = LoadAllProductsSmart(filename);
            Console.WriteLine("\n=== ОБНОВЛЕННЫЙ СПИСОК ТОВАРОВ ===");
            for (int i = 0; i < updatedProducts.Count; i++)
            {
                Console.WriteLine($"\n--- Товар {i + 1} ---");
                updatedProducts[i].DisplayInfo();
            }
        }

        public static bool EditProductByIndex(string filename, int productIndex)
        {
            var allProducts = LoadAllProductsSmart(filename);
            if (productIndex < 1 || productIndex > allProducts.Count)
            {
                Console.WriteLine($"Неверный номер товара! Доступные номера: 1-{allProducts.Count}");
                return false;
            }

            int actualIndex = productIndex - 1;
            var productToEdit = allProducts[actualIndex];

            Console.WriteLine("\n=== ТЕКУЩАЯ ИНФОРМАЦИЯ О ТОВАРЕ ===");
            productToEdit.DisplayInfo();

            Console.WriteLine("\nКакие поля хотите отредактировать?");
            Console.Write("Введите номера полей через пробел (1-10): ");
            string fieldsInput = Console.ReadLine();
            bool[] editFields = new bool[10];

            string[] fieldNumbers = fieldsInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (string fieldStr in fieldNumbers)
            {
                if (int.TryParse(fieldStr, out int fieldNum) && fieldNum >= 1 && fieldNum <= 10)
                {
                    editFields[fieldNum - 1] = true;
                }
            }

            Console.WriteLine("\n=== ВВОД НОВЫХ ДАННЫХ ===");

            if (editFields[0]) // Артикул
            {
                while (true)
                {
                    Console.Write("Введите новый артикул: ");
                    string input = Console.ReadLine();
                    if (int.TryParse(input, out int newArticle) && ValidateNumber(newArticle, 10))
                    {
                        productToEdit.SetArticle(newArticle);
                        break;
                    }
                    Console.WriteLine("Ошибка! Артикул должен быть неотрицательным числом до 10 цифр.");
                }
            }

            if (editFields[1]) // Название
            {
                while (true)
                {
                    Console.Write("Введите новое название: ");
                    string newName = Console.ReadLine();
                    if (ValidateNonEmpty(newName) && ValidateString(newName, 30))
                    {
                        productToEdit.SetName(newName);
                        break;
                    }
                    Console.WriteLine("Ошибка! Название не может быть пустым и не должно превышать 30 символов.");
                }
            }

            if (editFields[2]) // Бренд
            {
                while (true)
                {
                    Console.Write("Введите новый бренд: ");
                    string newBrand = Console.ReadLine();
                    if (ValidateNonEmpty(newBrand) && ValidateString(newBrand, 30))
                    {
                        productToEdit.SetBrand(newBrand);
                        break;
                    }
                    Console.WriteLine("Ошибка! Бренд не может быть пустым и не должен превышать 30 символов.");
                }
            }

            if (editFields[3]) // Страна-производитель
            {
                while (true)
                {
                    Console.Write("Введите новую страну-производитель: ");
                    string newCountry = Console.ReadLine();
                    if (ValidateNonEmpty(newCountry) && ValidateString(newCountry, 30))
                    {
                        productToEdit.SetProducerCountry(newCountry);
                        break;
                    }
                    Console.WriteLine("Ошибка! Страна-производитель не может быть пустой и не должна превышать 30 символов.");
                }
            }

            if (editFields[4]) // Категория
            {
                while (true)
                {
                    Console.Write("Введите новую категорию: ");
                    string newCategory = Console.ReadLine();
                    if (ValidateNonEmpty(newCategory) && ValidateString(newCategory, 30))
                    {
                        productToEdit.SetCategory(newCategory);
                        break;
                    }
                    Console.WriteLine("Ошибка! Категория не может быть пустой и не должна превышать 30 символов.");
                }
            }

            if (editFields[5]) // Тип товара
            {
                while (true)
                {
                    Console.Write("Введите новый тип товара: ");
                    string newType = Console.ReadLine();
                    if (ValidateNonEmpty(newType) && ValidateString(newType, 30))
                    {
                        productToEdit.SetTypeProduct(newType);
                        break;
                    }
                    Console.WriteLine("Ошибка! Тип товара не может быть пустым и не должен превышать 30 символов.");
                }
            }

            if (editFields[6]) // Описание
            {
                while (true)
                {
                    Console.Write("Введите новое описание: ");
                    string newDescription = Console.ReadLine();
                    if (ValidateNonEmpty(newDescription))
                    {
                        productToEdit.SetDescription(newDescription);
                        break;
                    }
                    Console.WriteLine("Ошибка! Описание не может быть пустым.");
                }
            }

            if (editFields[7]) // Количество на складе
            {
                while (true)
                {
                    Console.Write("Введите новое количество на складе: ");
                    string input = Console.ReadLine();
                    if (uint.TryParse(input, out uint newQuantity) && ValidateNumber(newQuantity, 10))
                    {
                        productToEdit.SetQuantityStock(newQuantity);
                        break;
                    }
                    Console.WriteLine("Ошибка! Количество должно быть числом до 10 цифр.");
                }
            }

            if (editFields[8]) // Статус на складе
            {
                while (true)
                {
                    Console.Write("Введите новый статус товара (1 - В наличии, 0 - Отсутствует): ");
                    string input = Console.ReadLine();
                    if (int.TryParse(input, out int statusInput) && (statusInput == 0 || statusInput == 1))
                    {
                        productToEdit.SetStatusStock(statusInput == 1);
                        break;
                    }
                    Console.WriteLine("Ошибка! Введите только 1 или 0.");
                }
            }

            if (editFields[9]) // Цена
            {
                while (true)
                {
                    Console.Write("Введите новую цену: ");
                    string input = Console.ReadLine();
                    if (int.TryParse(input, out int newPrice) && newPrice >= 0 && ValidateNumber(newPrice, 10))
                    {
                        productToEdit.SetPrice(newPrice);
                        break;
                    }
                    Console.WriteLine("Ошибка! Цена не может быть отрицательной и должна быть числом до 10 цифр.");
                }
            }

            try
            {
                using (StreamWriter outFile = new StreamWriter(filename))
                {
                    foreach (var product in allProducts)
                    {
                        outFile.WriteLine($"{product.GetArticle()}|{product.GetName()}|{product.GetBrand()}|{product.GetProducerCountry()}|{product.GetCategory()}|{product.GetTypeProduct()}|{product.GetDescription()}|{product.GetQuantityStock()}|{(product.GetStatusStock() ? 1 : 0)}|{product.GetPrice()}");
                    }
                }

                Console.WriteLine($"\nТовар №{productIndex} успешно отредактирован!");
                return true;
            }
            catch
            {
                Console.WriteLine("Ошибка открытия файла для записи!");
                return false;
            }
        }

        public static void FindProductsByArticle(string filename)
        {
            Console.WriteLine("\n=== ПОИСК ТОВАРОВ ПО АРТИКУЛУ ===");
            int productCount;
            while (true)
            {
                Console.Write("Сколько товаров хотите найти? ");
                if (int.TryParse(Console.ReadLine(), out productCount))
                {
                    if (productCount < 0)
                    {
                        Console.WriteLine("Ошибка! Количество не может быть отрицательным.");
                        continue;
                    }
                    if (productCount == 0)
                    {
                        Console.WriteLine("Поиск отменен.");
                        return;
                    }
                    break;
                }
                Console.WriteLine("Ошибка! Введите целое число.");
            }

            List<int> articles = new List<int>();
            for (int i = 0; i < productCount; i++)
            {
                Console.Write($"\nВведите артикул товара для поиска {i + 1}/{productCount}: ");
                if (int.TryParse(Console.ReadLine(), out int articleToFind))
                {
                    articles.Add(articleToFind);
                }
            }

            var foundProducts = FindProductsByArticleSmart(filename, articles);
            Console.WriteLine("\n=== РЕЗУЛЬТАТ ПОИСКА ===");
            Console.WriteLine($"Запрошено к поиску: {productCount} товар(ов)");
            Console.WriteLine($"Успешно найдено: {foundProducts.Count} товар(ов)");

            for (int i = 0; i < foundProducts.Count; i++)
            {
                Console.WriteLine($"\n--- Найденный товар {i + 1} ---");
                foundProducts[i].DisplayInfo();
            }
        }

        public static bool FindProductByArticle(string filename, int articleToFind)
        {
            try
            {
                using (StreamReader file = new StreamReader(filename))
                {
                    string line;
                    bool found = false;

                    while ((line = file.ReadLine()) != null)
                    {
                        if (string.IsNullOrEmpty(line)) continue;

                        string[] parts = line.Split('|');
                        if (parts.Length >= 10 && int.TryParse(parts[0], out int currentArticle) && currentArticle == articleToFind)
                        {
                            found = true;
                            Console.WriteLine("\n=== НАЙДЕН ТОВАР ===");
                            Console.WriteLine($"Артикул: {parts[0]}");
                            Console.WriteLine($"Название: {parts[1]}");
                            Console.WriteLine($"Бренд: {parts[2]}");
                            Console.WriteLine($"Страна-производитель: {parts[3]}");
                            Console.WriteLine($"Категория: {parts[4]}");
                            Console.WriteLine($"Тип товара: {parts[5]}");
                            Console.WriteLine($"Описание: {parts[6]}");
                            Console.WriteLine($"Количество на складе: {parts[7]}");
                            Console.WriteLine($"Статус на складе: {(parts[8] == "1" ? "В наличии" : "Отсутствует")}");
                            Console.WriteLine($"Цена: {parts[9]}");
                            Console.WriteLine("=========================");
                            break;
                        }
                    }

                    if (!found)
                    {
                        Console.WriteLine($"Товар с артикулом {articleToFind} не найден!");
                        return false;
                    }

                    return true;
                }
            }
            catch
            {
                Console.WriteLine($"Файл {filename} не найден!");
                return false;
            }
        }

        // Статические методы валидации
        public static bool ValidateString(string str, int maxLength = 30)
        {
            return !string.IsNullOrEmpty(str) && str.Length <= maxLength;
        }

        public static bool ValidateNumber(long number, int maxDigits = 10)
        {
            if (number < 0) return false;
            string numStr = number.ToString();
            return numStr.Length <= maxDigits;
        }

        public static bool ValidateNonEmpty(string str)
        {
            return !string.IsNullOrEmpty(str);
        }

        // Методы для работы с умными указателями (в C# используем обычные ссылки)
        public static List<Product> LoadAllProductsSmart(string filename)
        {
            List<Product> products = new List<Product>();

            try
            {
                using (StreamReader file = new StreamReader(filename))
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        if (string.IsNullOrEmpty(line)) continue;

                        string[] parts = line.Split('|');
                        if (parts.Length >= 10)
                        {
                            try
                            {
                                var product = new Product(
                                    int.Parse(parts[0]),
                                    parts[1],
                                    parts[2],
                                    parts[3],
                                    parts[4],
                                    parts[5],
                                    parts[6],
                                    uint.Parse(parts[7]),
                                    int.Parse(parts[9])
                                );
                                products.Add(product);
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
            }

            return products;
        }

        public static Product CreateProductFromFileSmart(string filename, int articleToFind)
        {
            try
            {
                using (StreamReader file = new StreamReader(filename))
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        if (string.IsNullOrEmpty(line)) continue;

                        string[] parts = line.Split('|');
                        if (parts.Length >= 10 && int.TryParse(parts[0], out int currentArticle) && currentArticle == articleToFind)
                        {
                            return new Product(
                                int.Parse(parts[0]),
                                parts[1],
                                parts[2],
                                parts[3],
                                parts[4],
                                parts[5],
                                parts[6],
                                uint.Parse(parts[7]),
                                int.Parse(parts[9])
                            );
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Файл {filename} не найден!");
            }

            return null;
        }

        public static List<Product> FindProductsByNameSmart(string filename, string name)
        {
            List<Product> foundProducts = new List<Product>();

            try
            {
                using (StreamReader file = new StreamReader(filename))
                {
                    string line;
                    string searchNameLower = name.ToLower();

                    while ((line = file.ReadLine()) != null)
                    {
                        if (string.IsNullOrEmpty(line)) continue;

                        string[] parts = line.Split('|');
                        if (parts.Length >= 10)
                        {
                            string productName = parts[1];
                            string productNameLower = productName.ToLower();

                            if (productNameLower.Contains(searchNameLower))
                            {
                                try
                                {
                                    var product = new Product(
                                        int.Parse(parts[0]),
                                        parts[1],
                                        parts[2],
                                        parts[3],
                                        parts[4],
                                        parts[5],
                                        parts[6],
                                        uint.Parse(parts[7]),
                                        int.Parse(parts[9])
                                    );
                                    foundProducts.Add(product);
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Файл {filename} не найден!");
            }

            return foundProducts;
        }

        public static Product GetProductByIndexSmart(string filename, int index)
        {
            var allProducts = LoadAllProductsSmart(filename);
            if (index < 1 || index > allProducts.Count)
            {
                return null;
            }

            return allProducts[index - 1];
        }

        public static bool UpdateProductInFileSmart(string filename, Product updatedProduct)
        {
            if (updatedProduct == null)
            {
                Console.WriteLine("Ошибка: передан нулевой объект товара!");
                return false;
            }

            var allProducts = LoadAllProductsSmart(filename);
            bool found = false;

            for (int i = 0; i < allProducts.Count; i++)
            {
                if (allProducts[i].GetArticle() == updatedProduct.GetArticle())
                {
                    allProducts[i] = updatedProduct;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                allProducts.Add(updatedProduct);
            }

            try
            {
                using (StreamWriter outFile = new StreamWriter(filename))
                {
                    foreach (var product in allProducts)
                    {
                        outFile.WriteLine($"{product.GetArticle()}|{product.GetName()}|{product.GetBrand()}|{product.GetProducerCountry()}|{product.GetCategory()}|{product.GetTypeProduct()}|{product.GetDescription()}|{product.GetQuantityStock()}|{(product.GetStatusStock() ? 1 : 0)}|{product.GetPrice()}");
                    }
                }

                if (found)
                {
                    Console.WriteLine($"Товар с артикулом {updatedProduct.GetArticle()} успешно обновлен!");
                }
                else
                {
                    Console.WriteLine($"Товар с артикулом {updatedProduct.GetArticle()} успешно добавлен!");
                }

                return true;
            }
            catch
            {
                Console.WriteLine("Ошибка открытия файла для записи!");
                return false;
            }
        }

        public static Product CreateProductFromInputSmart()
        {
            var product = new Product();
            product.InputFromKeyboard();
            return product;
        }

        public static List<Product> FindProductsByArticleSmart(string filename, List<int> articles)
        {
            List<Product> foundProducts = new List<Product>();

            try
            {
                using (StreamReader file = new StreamReader(filename))
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        if (string.IsNullOrEmpty(line)) continue;

                        string[] parts = line.Split('|');
                        if (parts.Length >= 10 && int.TryParse(parts[0], out int currentArticle))
                        {
                            if (articles.Contains(currentArticle))
                            {
                                try
                                {
                                    var product = new Product(
                                        int.Parse(parts[0]),
                                        parts[1],
                                        parts[2],
                                        parts[3],
                                        parts[4],
                                        parts[5],
                                        parts[6],
                                        uint.Parse(parts[7]),
                                        int.Parse(parts[9])
                                    );
                                    foundProducts.Add(product);
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Файл {filename} не найден!");
            }

            return foundProducts;
        }

        // Геттеры
        public int GetArticle() => article;
        public string GetName() => name;
        public string GetBrand() => brand;
        public string GetProducerCountry() => producerCountry;
        public string GetCategory() => category;
        public string GetTypeProduct() => typeProduct;
        public string GetDescription() => description;
        public uint GetQuantityStock() => quantityStock;
        public bool GetStatusStock() => statusStock;
        public int GetPrice() => price;

        // Сеттеры
        public void SetArticle(int art) => article = art;
        public void SetName(string nm) => name = nm;
        public void SetBrand(string br) => brand = br;
        public void SetProducerCountry(string country) => producerCountry = country;
        public void SetCategory(string cat) => category = cat;
        public void SetTypeProduct(string type) => typeProduct = type;
        public void SetDescription(string desc) => description = desc;
        public void SetQuantityStock(uint quantity)
        {
            quantityStock = quantity;
            statusStock = (quantity > 0);
        }
        public void SetStatusStock(bool status) => statusStock = status;
        public void SetPrice(int pr) => price = pr;

        // Вспомогательные методы для работы со строками
        public static string ToLower(string str)
        {
            return str.ToLower();
        }
    }
}
