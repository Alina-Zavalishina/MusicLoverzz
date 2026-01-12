using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace MusicShopSystem
{
    public class Review
    {
        // Поля класса
        private string idReview;          // уникальный идентификатор отзыва
        private string productName;       // название товара
        private string clientId;          // идентификатор клиента
        private int rating;              // оценка товара (1-5)
        private string comment;          // текстовый комментарий
        private string dateReview;       // дата создания отзыва

        // Конструктор по умолчанию
        public Review()
        {
            rating = 0;
            idReview = GenerateReviewId();
            dateReview = GetCurrentDate();
        }

        // Конструктор с параметрами
        public Review(string product, string client, int rating, string comment)
        {
            productName = product;
            clientId = client;
            this.rating = rating;
            this.comment = comment;
            idReview = GenerateReviewId();
            dateReview = GetCurrentDate();
        }

        // Метод сохранения отзыва в файл
        public void SaveToFile()
        {
            try
            {
                using (StreamWriter file = new StreamWriter("reviews.txt", true, Encoding.UTF8))
                { // Формируем строку с разделителем
                    file.WriteLine($"{idReview}|{productName}|{clientId}|{rating}|{comment}|{dateReview}");
                    Console.WriteLine("Отзыв сохранен в файл!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения отзыва: {ex.Message}");
            }
        }

        // Статический метод для показа всех отзывов из файла
        public static void PublicReview()
        {
            Console.WriteLine("\n=== ВСЕ ОТЗЫВЫ ИЗ ФАЙЛА ===");

            if (!File.Exists("reviews.txt"))
            {
                Console.WriteLine("Файл отзывов не найден или пуст!");
                return;
            }

            try
            {// Чтение всех строк из файла
                string[] lines = File.ReadAllLines("reviews.txt", Encoding.UTF8);
                int reviewCount = 0;// Счетчик обработанных отзывов

                foreach (string line in lines)// Обработка каждой строки файла
                {
                    if (string.IsNullOrEmpty(line)) continue;

                    string[] tokens = line.Split('|');
                    if (tokens.Length >= 6)
                    {
                        reviewCount++;
                        Console.WriteLine($"\n--- Отзыв #{reviewCount} ---");
                        Console.WriteLine($"ID: {tokens[0]}");
                        Console.WriteLine($"Товар: {tokens[1]}");
                        Console.WriteLine($"Клиент: {tokens[2]}");
                        Console.WriteLine($"Оценка: {tokens[3]}/5");
                        Console.WriteLine($"Комментарий: {tokens[4]}");
                        Console.WriteLine($"Дата: {tokens[5]}");
                    }
                }

                if (reviewCount == 0)
                {
                    Console.WriteLine("Отзывов не найдено!");
                }
                else
                {
                    Console.WriteLine($"\nВсего отзывов: {reviewCount}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка чтения файла отзывов: {ex.Message}");
            }
        }

        // Статический метод для расчета средней оценки товара
        public static double AverageRating(string productName)
        {
            if (!File.Exists("reviews.txt"))
            {
                Console.WriteLine("Файл отзывов не найден!");
                return 0.0;
            }

            double totalRating = 0.0;// Сумма всех оценок
            int reviewCount = 0;// Количество отзывов для данного товара

            try
            {// Чтение всех строк из файла
                string[] lines = File.ReadAllLines("reviews.txt", Encoding.UTF8);

                foreach (string line in lines) // Обработка каждой строки
                {
                    if (string.IsNullOrEmpty(line)) continue;// Пропуск пустых строк

                    string[] tokens = line.Split('|');
                    if (tokens.Length >= 6)
                    {
                        string fileProductName = tokens[1];

                        // Сравниваем названия товаров (регистронезависимо)
                        if (fileProductName.Equals(productName, StringComparison.OrdinalIgnoreCase))
                        {
                            if (int.TryParse(tokens[3], out int productRating))//оценка преобразовывается в чилос
                            {
                                totalRating += productRating;// Добавление к общей сумме
                                reviewCount++;// Увеличение счетчика отзывов
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка чтения файла отзывов: {ex.Message}");
                return 0.0;
            }

            if (reviewCount == 0)
            {
                Console.WriteLine($"\nОтзывов для товара '{productName}' не найдено!");
                return 0.0;
            }

            double average = totalRating / reviewCount;// Расчет средней оценки

            Console.WriteLine("\n=== РЕЗУЛЬТАТЫ РАСЧЕТА ===");
            Console.WriteLine($"Товар: {productName}");
            Console.WriteLine($"Количество отзывов: {reviewCount}");
            Console.WriteLine($"Средняя оценка: {average:F1} из 5");
            Console.WriteLine("==========================");

            return average;
        }

        // Генерация уникального ID отзыва
        private string GenerateReviewId()
        {
            Random random = new Random();// Создание объекта для генерации случайных чисел
            int randomNum = random.Next(10000); // Генерация числа от 0 до 9999
            return $"REV{randomNum:D4}";
        }

        // Получение текущей даты и времени
        private string GetCurrentDate()
        {
            return DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");// Форматирование даты: день.месяц.год часы:минуты:секунды
        }

        // Свойства для доступа к полям
        public string IdReview
        {
            get { return idReview; }// Только чтение
        }

        public string ProductName
        {
            get { return productName; }// Можно изменять
            set { productName = value; }
        }

        public string ClientId
        {
            get { return clientId; }
            set { clientId = value; }
        }

        public int Rating
        {
            get { return rating; }
            set { rating = value; }
        }

        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }

        public string DateReview
        {
            get { return dateReview; }
        }


        public string GetIdReview() { return idReview; }
        public string GetProductName() { return productName; }
        public string GetClientId() { return clientId; }
        public int GetRating() { return rating; }
        public string GetComment() { return comment; }
        public string GetDateReview() { return dateReview; }
    }
}
