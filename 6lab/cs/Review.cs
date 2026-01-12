using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MusicStore
{
    public abstract class Review
    {
        protected string idReview;
        protected string clientId;
        protected int rating;
        protected string comment;
        protected string dateReview;
        protected bool isVerified;
        protected int helpfulVotes;
        protected int unhelpfulVotes;

        public Review(string clientId = "", int rating = 0, string comment = "",
                      bool verified = false, string id = "", string date = "")
        {
            this.clientId = clientId;
            this.comment = comment;
            this.isVerified = verified;
            this.helpfulVotes = 0;
            this.unhelpfulVotes = 0;
            SetRating(rating);

            if (string.IsNullOrEmpty(id))
                idReview = GenerateReviewId();
            else
                idReview = id;

            if (string.IsNullOrEmpty(date))
                dateReview = GetCurrentDate();
            else
                dateReview = date;
        }

        

        // Абстрактные методы
        public abstract string GetReviewType();
        public abstract void DisplayDetails();
        public abstract string GetSubject();
        public abstract bool Validate();
        public abstract Review Clone();

        // Виртуальные методы с реализацией
        public virtual double CalculateWeight()
        {
            double weight = 1.0;
            if (isVerified) weight *= 1.5;
            if (comment.Length > 100) weight *= 1.2;
            else if (comment.Length > 50) weight *= 1.1;

            double helpfulRatio = GetHelpfulnessRatio();
            if (helpfulRatio > 0.8) weight *= 1.3;
            else if (helpfulRatio > 0.5) weight *= 1.1;

            return weight;
        }

        public virtual string PrepareForFile()
        {
            return $"{idReview}|{GetReviewType()}|{clientId}|{rating}|{comment}|{dateReview}|{(isVerified ? "1" : "0")}|{helpfulVotes}|{unhelpfulVotes}";
        }

        public virtual string ExportToString()
        {
            return $"Тип: {GetReviewType()}\nID: {idReview}\nКлиент: {clientId}\nОценка: {rating}/5\nДата: {dateReview}\nПроверен: {(isVerified ? "Да" : "Нет")}\nПолезно: {helpfulVotes}\nБесполезно: {unhelpfulVotes}\nКомментарий: {comment}";
        }

        public void SaveToFile(string filename = "reviews.txt")
        {
            try
            {
                File.AppendAllText(filename, PrepareForFile() + Environment.NewLine);
                Console.WriteLine("Отзыв сохранен в файл!");
            }
            catch
            {
                Console.WriteLine("Ошибка сохранения отзыва!");
            }
        }

        public static void DisplayAllReviews(string filename = "reviews.txt")
        {
            Console.WriteLine("\n=== ВСЕ ОТЗЫВЫ ИЗ ФАЙЛА ===");
            if (!File.Exists(filename))
            {
                Console.WriteLine("Файл отзывов не найден или пуст!");
                return;
            }

            var lines = File.ReadAllLines(filename);
            int reviewCount = 0;
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                var tokens = line.Split('|');
                if (tokens.Length >= 6)
                {
                    reviewCount++;
                    Console.WriteLine($"\n--- Отзыв #{reviewCount} ---");
                    Console.WriteLine($"Тип: {tokens[1]}");
                    Console.WriteLine($"ID: {tokens[0]}");
                    Console.WriteLine($"Клиент: {tokens[2]}");
                    Console.WriteLine($"Оценка: {tokens[3]}/5");
                    Console.WriteLine($"Комментарий: {tokens[4]}");
                    Console.WriteLine($"Дата: {tokens[5]}");
                    if (tokens.Length > 6)
                        Console.WriteLine($"Проверен: {(tokens[6] == "1" ? "Да" : "Нет")}");
                }
            }
            if (reviewCount == 0)
                Console.WriteLine("Отзывов не найдено!");
            else
                Console.WriteLine($"\nВсего отзывов: {reviewCount}");
        }

        public static double AverageRating(string productName)
        {
            if (!File.Exists("reviews.txt"))
            {
                Console.WriteLine("Файл отзывов не найден!");
                return 0.0;
            }

            double totalRating = 0.0;
            int reviewCount = 0;
            var lines = File.ReadAllLines("reviews.txt");
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                var tokens = line.Split('|');
                if (tokens.Length >= 5)
                {
                    string comment = tokens[4].ToLower();
                    if (comment.Contains(productName.ToLower()))
                    {
                        if (int.TryParse(tokens[3], out int productRating))
                        {
                            totalRating += productRating;
                            reviewCount++;
                        }
                    }
                }
            }

            if (reviewCount == 0)
            {
                Console.WriteLine($"Отзывов для товара '{productName}' не найдено!");
                return 0.0;
            }

            double average = totalRating / reviewCount;
            Console.WriteLine("\n=== РЕЗУЛЬТАТЫ РАСЧЕТА ===");
            Console.WriteLine($"Товар: {productName}");
            Console.WriteLine($"Количество отзывов: {reviewCount}");
            Console.WriteLine($"Средняя оценка: {average:F1} из 5");
            Console.WriteLine("==========================");
            return average;
        }

        public void MarkAsHelpful() => helpfulVotes++;
        public void MarkAsUnhelpful() => unhelpfulVotes++;
        public double GetHelpfulnessRatio()
        {
            int totalVotes = helpfulVotes + unhelpfulVotes;
            return totalVotes == 0 ? 0.0 : (double)helpfulVotes / totalVotes;
        }

        public static string GenerateReviewId(string prefix = "REV")
        {
            Random rand = new Random();
            return $"{prefix}{rand.Next(1000, 9999)}";
        }

        public static string GetCurrentDate()
        {
            return DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        }

        // Геттеры и сеттеры
        public string IdReview => idReview;
        public string ClientId => clientId;
        public int Rating => rating;
        public string Comment => comment;
        public string DateReview => dateReview;
        public bool IsVerified => isVerified;
        public int HelpfulVotes => helpfulVotes;
        public int UnhelpfulVotes => unhelpfulVotes;

        public void SetRating(int r)
        {
            if (r >= 1 && r <= 5) rating = r;
        }

        public void VerifyReview() => isVerified = true;
        public void UnverifyReview() => isVerified = false;
        public void SetClientId(string id) => clientId = id;
        public void SetHelpfulVotes(int votes) => helpfulVotes = votes;
        public void SetUnhelpfulVotes(int votes) => unhelpfulVotes = votes;
    }
}