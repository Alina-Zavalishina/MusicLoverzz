using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MusicShopSystem
{
    public class Product
    {

        private int article; // артикул
        private string name; // название
        private string brand; // бренд
        private string producerCountry; // страна-производитель
        private string category; // категория
        private string typeProduct; // тип
        private string description; // описание
        private uint quantityStock; // количество на складе
        private bool statusStock; // статус (в наличии/нет)
        private int price; // цена

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

        // Методы получения значений
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

        // Методы установки значений
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
            statusStock = (quantity > 0); // обновление статуса наличия
        }
        public void SetStatusStock(bool status) => statusStock = status;
        public void SetPrice(int pr) => price = pr;

        // Методы работы со складом
        public void AddStock()
        {
            InputFromKeyboard();
            Console.WriteLine("Товар успешно добавлен на склад!");
        }
        //удаление со склада
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
        //редактор товара
        public void EditStock()
        {
            Console.WriteLine("=== Редактирование товара ===");
            InputFromKeyboard();
            Console.WriteLine("Товар успешно отредактирован!");
        }
        // Поиск/проверка наличия товара на складе
        public bool FindStock() => statusStock;// возвращает текущий статус

        // Информация о товаре
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


        public static bool ValidateString(string str, int maxLength = 30)
        {
            return str.Length <= maxLength && !string.IsNullOrEmpty(str); // Проверяет что строка не null/пустая И длина не превышает maxLength
        }

        public static bool ValidateNumber(long number, int maxDigits = 10)
        {
            if (number < 0) return false;// Проверка на отрицательное число
            string numStr = number.ToString();// Преобразование числа в строку для подсчета цифр
            return numStr.Length <= maxDigits;// Проверка количества цифр
        }
        //проверяет что строка не null и не пустая
        public static bool ValidateNonEmpty(string str) => !string.IsNullOrEmpty(str);

        // Ввод данных с клавиатуры
        public void InputFromKeyboard()
        {
            Console.WriteLine("\n=== Ввод данных о товаре ===");

            // Ввод артикула
            while (true)
            {
                Console.Write("Введите артикул: ");
                if (int.TryParse(Console.ReadLine(), out int art))
                {
                    if (ValidateNumber(art, 10))
                    {
                        article = art;
                        break;
                    }
                    Console.WriteLine("Ошибка! Артикул должен быть неотрицательным числом до 10 цифр.");
                }
                else
                {
                    Console.WriteLine("Ошибка! Введите целое число.");
                }
            }

            // Ввод названия
            while (true)
            {
                Console.Write("Введите название: ");
                string nm = Console.ReadLine();
                if (ValidateNonEmpty(nm) && ValidateString(nm, 30))
                {
                    name = nm;
                    break;
                }
                Console.WriteLine("Ошибка! Название не может быть пустым и не должно превышать 30 символов.");
            }

            // Ввод бренда
            while (true)
            {
                Console.Write("Введите бренд: ");
                string br = Console.ReadLine();
                if (ValidateNonEmpty(br) && ValidateString(br, 30))
                {
                    brand = br;
                    break;
                }
                Console.WriteLine("Ошибка! Бренд не может быть пустым и не должен превышать 30 символов.");
            }

            // Ввод страны-производителя
            while (true)
            {
                Console.Write("Введите страну-производитель: ");
                string country = Console.ReadLine();
                if (ValidateNonEmpty(country) && ValidateString(country, 30))
                {
                    producerCountry = country;
                    break;
                }
                Console.WriteLine("Ошибка! Страна-производитель не может быть пустой и не должна превышать 30 символов.");
            }

            // Ввод категории
            while (true)
            {
                Console.Write("Введите категорию: ");
                string cat = Console.ReadLine();
                if (ValidateNonEmpty(cat) && ValidateString(cat, 30))
                {
                    category = cat;
                    break;
                }
                Console.WriteLine("Ошибка! Категория не может быть пустой и не должна превышать 30 символов.");
            }

            // Ввод типа товара
            while (true)
            {
                Console.Write("Введите тип товара: ");
                string type = Console.ReadLine();
                if (ValidateNonEmpty(type) && ValidateString(type, 30))
                {
                    typeProduct = type;
                    break;
                }
                Console.WriteLine("Ошибка! Тип товара не может быть пустым и не должен превышать 30 символов.");
            }

            // Ввод описания
            while (true)
            {
                Console.Write("Введите описание: ");
                string desc = Console.ReadLine();
                if (ValidateNonEmpty(desc))
                {
                    description = desc;
                    break;
                }
                Console.WriteLine("Ошибка! Описание не может быть пустым.");
            }

            // Ввод количества на складе
            while (true)
            {
                Console.Write("Введите количество на складе: ");
                if (uint.TryParse(Console.ReadLine(), out uint quantity))
                {
                    if (ValidateNumber(quantity, 10))
                    {
                        quantityStock = quantity;
                        break;
                    }
                    Console.WriteLine("Ошибка! Количество должно быть числом до 10 цифр.");
                }
                else
                {
                    Console.WriteLine("Ошибка! Введите целое число.");
                }
            }

            // Ввод статуса товара
            while (true)
            {
                Console.Write("Введите статус товара (1 - В наличии, 0 - Отсутствует): ");
                if (int.TryParse(Console.ReadLine(), out int statusInput))
                {
                    if (statusInput == 0 || statusInput == 1)
                    {
                        statusStock = (statusInput == 1);
                        break;
                    }
                    Console.WriteLine("Ошибка! Введите только 1 или 0.");
                }
                else
                {
                    Console.WriteLine("Ошибка! Введите 1 или 0.");
                }
            }

            // Ввод цены
            while (true)
            {
                Console.Write("Введите цену: ");
                if (int.TryParse(Console.ReadLine(), out int pr))
                {
                    if (pr >= 0 && ValidateNumber(pr, 10))
                    {
                        price = pr;
                        break;
                    }
                    Console.WriteLine("Ошибка! Цена не может быть отрицательной и должна быть числом до 10 цифр.");
                }
                else
                {
                    Console.WriteLine("Ошибка! Введите целое число.");
                }
            }
        }

        // Запись в файл
        public void WriteToFile(string filename)
        {
            try
            {//using-автоматическое закрытие файла при ошибках
                using (StreamWriter file = new StreamWriter(filename, true))
                {//форматирование строки с разделителем
                    file.WriteLine($"{article}|{name}|{brand}|{producerCountry}|{category}|{typeProduct}|{description}|{quantityStock}|{(statusStock ? 1 : 0)}|{price}");
                    Console.WriteLine($"Данные успешно записаны в файл: {filename}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка открытия файла для записи: {ex.Message}");
            }
        }

        // Чтение из файла
        public void ReadFromFile(string filename)
        {
            try
            {
                using (StreamReader file = new StreamReader(filename))
                {
                    string line = file.ReadLine();//чтение первой строки
                    if (line != null)//если она не пустая
                    {
                        string[] parts = line.Split('|');//деление по разделителю
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
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка открытия файла для чтения: {ex.Message}");
            }
        }

        // Отображение таблицы товаров
        public static void DisplayProductsTable(string filename)
        {
            try
            {
                if (!File.Exists(filename))
                {
                    Console.WriteLine($"Файл {filename} не найден!");
                    Console.WriteLine("Сначала добавьте товары через меню 'Добавить товар'");
                    return;
                }

                string[] lines = File.ReadAllLines(filename);//чтение всех строк файла
                if (lines.Length == 0)
                {
                    Console.WriteLine($"Файл {filename} пуст!");
                    Console.WriteLine("Сначала добавьте товары через меню 'Добавить товар'");
                    return;
                }

                Console.WriteLine("\n=== ИНФОРМАЦИЯ О ТОВАРАХ ===");
                int productCount = 0;

                foreach (string line in lines)
                {
                    if (string.IsNullOrEmpty(line)) continue;

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
                            uint quantity = uint.Parse(tokens[7]);
                            bool status = tokens[8] == "1";
                            int price = int.Parse(tokens[9]);

                            Console.WriteLine($"\n--- Товар {productCount + 1} ---");
                            Console.WriteLine($"Артикул: {article}");
                            Console.WriteLine($"Название: {name}");
                            Console.WriteLine($"Бренд: {brand}");
                            Console.WriteLine($"Страна: {country}");
                            Console.WriteLine($"Категория: {category}");
                            Console.WriteLine($"Тип: {type}");
                            Console.WriteLine($"Описание: {description}");
                            Console.WriteLine($"Количество: {quantity}");
                            Console.WriteLine($"Статус: {(status ? "В наличии" : "Отсутствует")}");
                            Console.WriteLine($"Цена: {price}");

                            productCount++;
                        }
                        catch (Exception)
                        {
                            Console.WriteLine($"Ошибка чтения строки: {line}");
                            continue;
                        }
                    }
                }

                Console.WriteLine("\n=================================");
                Console.WriteLine($"Всего товаров: {productCount}");
                Console.WriteLine("=================================");

                if (productCount == 0)
                {
                    Console.WriteLine("В файле нет корректных данных о товарах!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
            }
        }

        // Поиск товара по артикулу (один товар)
        public static bool FindProductByArticle(string filename, int articleToFind)
        {
            try
            {
                string[] lines = File.ReadAllLines(filename);
                foreach (string line in lines)// Ищем в каждой строке
                {
                    if (string.IsNullOrEmpty(line)) continue;// Пропускаем пустые строки

                    string[] tokens = line.Split('|');
                    if (tokens.Length >= 1)
                    {
                        if (int.TryParse(tokens[0], out int currentArticle) && currentArticle == articleToFind)//если артикул найден
                        {
                            Console.WriteLine("\n=== НАЙДЕН ТОВАР ===");
                            Console.WriteLine($"Артикул: {tokens[0]}");
                            Console.WriteLine($"Название: {tokens[1]}");
                            Console.WriteLine($"Бренд: {tokens[2]}");
                            Console.WriteLine($"Страна-производитель: {tokens[3]}");
                            Console.WriteLine($"Категория: {tokens[4]}");
                            Console.WriteLine($"Тип товара: {tokens[5]}");
                            Console.WriteLine($"Описание: {tokens[6]}");
                            Console.WriteLine($"Количество на складе: {tokens[7]}");
                            Console.WriteLine($"Статус на складе: {(tokens[8] == "1" ? "В наличии" : "Отсутствует")}");
                            Console.WriteLine($"Цена: {tokens[9]}");
                            Console.WriteLine("=========================");
                            return true;
                        }
                    }
                }
                Console.WriteLine($"Товар с артикулом {articleToFind} не найден!");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Файл {filename} не найден: {ex.Message}");
                return false;
            }
        }

        // Поиск товаров по артикулу (несколько)
        public static void FindProductsByArticle(string filename)
        {
            Console.WriteLine("\n=== ПОИСК ТОВАРОВ ПО АРТИКУЛУ ===");

            int productCount;//количество товаров для поиска
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

            int successfullyFound = 0;// Счетчик успешно найденных товаров
            for (int i = 0; i < productCount; i++)
            {
                Console.Write($"\nВведите артикул товара для поиска {i + 1}/{productCount}: ");
                if (int.TryParse(Console.ReadLine(), out int articleToFind))
                {
                    if (FindProductByArticle(filename, articleToFind))
                    {
                        successfullyFound++;// Увеличиваем счетчик если товар найден
                    }
                }
                else
                {
                    Console.WriteLine("Ошибка! Введите целое число.");
                }
            }

            Console.WriteLine("\n=== РЕЗУЛЬТАТ ПОИСКА ===");
            Console.WriteLine($"Запрошено к поиску: {productCount} товар(ов)");
            Console.WriteLine($"Успешно найдено: {successfullyFound} товар(ов)");
        }

        // Удаление товара по артикулу
        public static bool DeleteProductByArticle(string filename, int articleToDelete)
        {
            try
            {
                if (!File.Exists(filename))
                {
                    Console.WriteLine($"Файл {filename} не найден!");
                    return false;
                }
                // Читаем все строки в список
                List<string> lines = new List<string>(File.ReadAllLines(filename));
                bool found = false;// Флаг - найден ли товар для удаления
                List<string> newLines = new List<string>();// Новый список строк (без удаленных)
                // Проходим по всем строкам
                foreach (string line in lines)
                {
                    if (string.IsNullOrEmpty(line))
                    {
                        newLines.Add(line);
                        continue;
                    }

                    string[] tokens = line.Split('|');// Разделяем строку
                    if (tokens.Length >= 1)
                    {
                        if (int.TryParse(tokens[0], out int currentArticle) && currentArticle == articleToDelete)
                        {
                            found = true;
                            Console.WriteLine($"Найден товар для удаления: {line}");
                            continue; // Пропускаем эту строку (удаляем)
                        }
                    }
                    newLines.Add(line);
                }

                if (!found)// Если товар не найден
                {
                    Console.WriteLine($"Товар с артикулом {articleToDelete} не найден!");
                    return false;
                }

                File.WriteAllLines(filename, newLines);
                Console.WriteLine($"Товар с артикулом {articleToDelete} успешно удален!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка открытия файла для записи: {ex.Message}");
                return false;
            }
        }

        // Функция удаления товаров
        public static void DeleteProductsFromFile(string filename)
        {
            Console.WriteLine("\n=== УДАЛЕНИЕ ТОВАРОВ СО СКЛАДА ===");
            DisplayProductsTable(filename);

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

            int successfullyDeleted = 0; // Счетчик успешно удаленных товаров
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
                else
                {
                    Console.WriteLine("Ошибка! Введите целое число.");
                }
            }

            Console.WriteLine("\n=== РЕЗУЛЬТАТ УДАЛЕНИЯ ===");
            Console.WriteLine($"Запрошено к удалению: {productCount} товар(ов)");
            Console.WriteLine($"Успешно удалено: {successfullyDeleted} товар(ов)");

            Console.WriteLine("\n=== ОБНОВЛЕННЫЙ СПИСОК ТОВАРОВ ===");
            DisplayProductsTable(filename);
        }

        // Редактирование товара по индексу
        public static bool EditProductByIndex(string filename, int productIndex)
        {
            try
            {
                if (!File.Exists(filename))
                {
                    Console.WriteLine($"Файл {filename} не найден!");
                    return false;
                }

                List<string> lines = new List<string>(File.ReadAllLines(filename));
                int lineCount = lines.Count; // Количество строк в файле

                if (productIndex < 1 || productIndex > lineCount)
                {
                    Console.WriteLine($"Неверный номер товара! Доступные номера: 1-{lineCount}");
                    return false;
                }

                int actualIndex = productIndex - 1;
                string line = lines[actualIndex];// Получаем строку для редактирования

                Console.WriteLine("\n=== ТЕКУЩАЯ ИНФОРМАЦИЯ О ТОВАРЕ ===");
                string[] tokens = line.Split('|');
                if (tokens.Length == 10)
                {
                    Console.WriteLine($"1. Артикул: {tokens[0]}");
                    Console.WriteLine($"2. Название: {tokens[1]}");
                    Console.WriteLine($"3. Бренд: {tokens[2]}");
                    Console.WriteLine($"4. Страна-производитель: {tokens[3]}");
                    Console.WriteLine($"5. Категория: {tokens[4]}");
                    Console.WriteLine($"6. Тип товара: {tokens[5]}");
                    Console.WriteLine($"7. Описание: {tokens[6]}");
                    Console.WriteLine($"8. Количество на складе: {tokens[7]}");
                    Console.WriteLine($"9. Статус на складе: {(tokens[8] == "1" ? "В наличии" : "Отсутствует")}");
                    Console.WriteLine($"10. Цена: {tokens[9]}");
                }

                Console.WriteLine("\nКакие поля хотите отредактировать?");
                Console.Write("Введите номера полей через пробел (1-10): ");
                string fieldsInput = Console.ReadLine();
                // Создаем массив флагов для отслеживания редактируемых полей
                bool[] editFields = new bool[10];
                string[] fieldNumbers = fieldsInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (string fieldStr in fieldNumbers)
                {
                    if (int.TryParse(fieldStr, out int fieldNum) && fieldNum >= 1 && fieldNum <= 10)
                    {
                        editFields[fieldNum - 1] = true;// Помечаем поле для редактирования
                    }
                }

                // Создаем товар для редактирования
                Product editedProduct = new Product();
                try
                {
                    editedProduct.SetArticle(int.Parse(tokens[0]));
                    editedProduct.SetName(tokens[1]);
                    editedProduct.SetBrand(tokens[2]);
                    editedProduct.SetProducerCountry(tokens[3]);
                    editedProduct.SetCategory(tokens[4]);
                    editedProduct.SetTypeProduct(tokens[5]);
                    editedProduct.SetDescription(tokens[6]);
                    editedProduct.SetQuantityStock(uint.Parse(tokens[7]));
                    editedProduct.SetStatusStock(tokens[8] == "1");
                    editedProduct.SetPrice(int.Parse(tokens[9]));
                }
                catch
                {
                    Console.WriteLine("Ошибка чтения данных товара!");
                    return false;
                }

                Console.WriteLine("\n=== ВВОД НОВЫХ ДАННЫХ ===");

                if (editFields[0]) // Артикул
                {
                    while (true)
                    {
                        Console.Write("Введите новый артикул: ");
                        if (int.TryParse(Console.ReadLine(), out int newArticle))
                        {
                            if (ValidateNumber(newArticle, 10))
                            {
                                editedProduct.SetArticle(newArticle);
                                break;
                            }
                            Console.WriteLine("Ошибка! Артикул должен быть неотрицательным числом до 10 цифр.");
                        }
                        else
                        {
                            Console.WriteLine("Ошибка! Введите целое число.");
                        }
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
                            editedProduct.SetName(newName);
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
                            editedProduct.SetBrand(newBrand);
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
                            editedProduct.SetProducerCountry(newCountry);
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
                            editedProduct.SetCategory(newCategory);
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
                            editedProduct.SetTypeProduct(newType);
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
                            editedProduct.SetDescription(newDescription);
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
                        if (uint.TryParse(Console.ReadLine(), out uint newQuantity))
                        {
                            if (ValidateNumber(newQuantity, 10))
                            {
                                editedProduct.SetQuantityStock(newQuantity);
                                break;
                            }
                            Console.WriteLine("Ошибка! Количество должно быть числом до 10 цифр.");
                        }
                        else
                        {
                            Console.WriteLine("Ошибка! Введите целое число.");
                        }
                    }
                }

                if (editFields[8]) // Статус на складе
                {
                    while (true)
                    {
                        Console.Write("Введите новый статус товара (1 - В наличии, 0 - Отсутствует): ");
                        if (int.TryParse(Console.ReadLine(), out int statusInput))
                        {
                            if (statusInput == 0 || statusInput == 1)
                            {
                                editedProduct.SetStatusStock(statusInput == 1);
                                break;
                            }
                            Console.WriteLine("Ошибка! Введите только 1 или 0.");
                        }
                        else
                        {
                            Console.WriteLine("Ошибка! Введите 1 или 0.");
                        }
                    }
                }

                if (editFields[9]) // Цена
                {
                    while (true)
                    {
                        Console.Write("Введите новую цену: ");
                        if (int.TryParse(Console.ReadLine(), out int newPrice))
                        {
                            if (newPrice >= 0 && ValidateNumber(newPrice, 10))
                            {
                                editedProduct.SetPrice(newPrice);
                                break;
                            }
                            Console.WriteLine("Ошибка! Цена не может быть отрицательной и должна быть числом до 10 цифр.");
                        }
                        else
                        {
                            Console.WriteLine("Ошибка! Введите целое число.");
                        }
                    }
                }

                // Формируем новую строку
                string newLine = $"{editedProduct.GetArticle()}|{editedProduct.GetName()}|{editedProduct.GetBrand()}|" +
                               $"{editedProduct.GetProducerCountry()}|{editedProduct.GetCategory()}|" +
                               $"{editedProduct.GetTypeProduct()}|{editedProduct.GetDescription()}|" +
                               $"{editedProduct.GetQuantityStock()}|{(editedProduct.GetStatusStock() ? 1 : 0)}|" +
                               $"{editedProduct.GetPrice()}";

                lines[actualIndex] = newLine;// Заменяем старую строку на новую
                File.WriteAllLines(filename, lines);
                Console.WriteLine($"\nТовар №{productIndex} успешно отредактирован!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                return false;
            }
        }

        // Редактирование товаров (несколько)
        public static void EditProductsInFile(string filename)
        {
            Console.WriteLine("\n=== РЕДАКТИРОВАНИЕ ТОВАРОВ ===");
            DisplayProductsTable(filename);

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

            int successfullyEdited = 0;// Счетчик успешно отредактированных товаров
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
                else
                {
                    Console.WriteLine("Ошибка! Введите целое число.");
                }
            }

            Console.WriteLine("\n=== РЕЗУЛЬТАТ РЕДАКТИРОВАНИЯ ===");
            Console.WriteLine($"Запрошено к редактированию: {productCount} товар(ов)");
            Console.WriteLine($"Успешно отредактировано: {successfullyEdited} товар(ов)");

            Console.WriteLine("\n=== ОБНОВЛЕННЫЙ СПИСОК ТОВАРОВ ===");
            DisplayProductsTable(filename);
        }
    }
}
