using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicStore
{
    public class MusicProductReview : Review, IRateable, IExportable, ICloneableReview, IValidatable
    {
        private string productName;
        private string productArticle;
        private string category;
        private string brand;
        private bool hasPhotos;
        private List<string> pros;
        private List<string> cons;
        private int monthsOfUsage;
        private bool recommendToOthers;

        public MusicProductReview() : base()
        {
            pros = new List<string>();
            cons = new List<string>();
            hasPhotos = false;
            monthsOfUsage = 0;
            recommendToOthers = true;
        }

        public MusicProductReview(string clientId, string productName, string productArticle,
                                  string category, string brand, int rating, string comment,
                                  bool hasPhotos = false, int monthsOfUsage = 0, bool recommend = true)
            : base(clientId, rating, comment)
        {
            this.productName = productName;
            this.productArticle = productArticle;
            this.category = category;
            this.brand = brand;
            this.hasPhotos = hasPhotos;
            this.monthsOfUsage = monthsOfUsage;
            this.recommendToOthers = recommend;
            pros = new List<string>();
            cons = new List<string>();
        }

        // Реализация абстрактных методов
        public override string GetReviewType() => "Отзыв на музыкальный товар";
        public override string GetSubject() => productName;

        public override void DisplayDetails()
        {
            Console.WriteLine("\n=== ОТЗЫВ НА МУЗЫКАЛЬНЫЙ ТОВАР ===");
            Console.WriteLine($"Товар: {productName}");
            Console.WriteLine($"Артикул: {productArticle}");
            Console.WriteLine($"Категория: {category}");
            Console.WriteLine($"Бренд: {brand}");
            Console.WriteLine($"Оценка: {rating}/5");
            Console.WriteLine($"Использование: {monthsOfUsage} месяцев");
            Console.WriteLine($"Рекомендую: {(recommendToOthers ? "Да" : "Нет")}");
            Console.WriteLine($"Фото: {(hasPhotos ? "Есть" : "Нет")}");
            Console.WriteLine($"Проверен: {(isVerified ? "Да" : "Нет")}");
            Console.WriteLine($"Вес отзыва: {CalculateWeight():F2}");
            Console.WriteLine($"Полезность: {GetHelpfulnessRatio() * 100:F1}%");

            if (pros.Any())
            {
                Console.WriteLine("\nДостоинства:");
                for (int i = 0; i < pros.Count; i++)
                    Console.WriteLine($" {i + 1}. {pros[i]}");
            }

            if (cons.Any())
            {
                Console.WriteLine("\nНедостатки:");
                for (int i = 0; i < cons.Count; i++)
                    Console.WriteLine($" {i + 1}. {cons[i]}");
            }

            Console.WriteLine($"\nКомментарий: {comment}");
            Console.WriteLine($"Дата: {dateReview}");
            Console.WriteLine("=================================");
        }

        public override bool Validate()
        {
            if (string.IsNullOrEmpty(productName) || string.IsNullOrEmpty(productArticle))
                return false;
            if (rating < 1 || rating > 5)
                return false;
            if (comment.Length < 10)
                return false;
            if (hasPhotos)
                return true;
            if (monthsOfUsage >= 1)
                return true;
            return comment.Length >= 20;
        }

        public override Review Clone()
        {
            var copy = new MusicProductReview(clientId, productName, productArticle, category, brand,
                                              rating, comment, hasPhotos, monthsOfUsage, recommendToOthers);
            copy.idReview = GenerateReviewId("CPY");
            copy.dateReview = GetCurrentDate();
            copy.isVerified = isVerified;
            copy.helpfulVotes = helpfulVotes;
            copy.unhelpfulVotes = unhelpfulVotes;
            copy.pros = new List<string>(pros);
            copy.cons = new List<string>(cons);
            return copy;
        }

        // Переопределение виртуальных методов
        public override double CalculateWeight()
        {
            double weight = base.CalculateWeight();
            if (monthsOfUsage >= 12) weight *= 1.5;
            else if (monthsOfUsage >= 6) weight *= 1.3;
            else if (monthsOfUsage >= 3) weight *= 1.2;
            if (hasPhotos) weight *= 1.4;
            if (pros.Any() && cons.Any()) weight *= 1.3;
            else if (pros.Any() || cons.Any()) weight *= 1.1;
            if (category == "Гитары" || category == "Клавишные" || category == "Ударные")
                weight *= 1.2;
            return weight;
        }

        public override string PrepareForFile()
        {
            return $"{idReview}|{GetReviewType()}|{clientId}|{productName}|{productArticle}|{category}|{brand}|{rating}|{comment}|{dateReview}|{(isVerified ? "1" : "0")}|{helpfulVotes}|{unhelpfulVotes}|{(hasPhotos ? "1" : "0")}|{monthsOfUsage}|{(recommendToOthers ? "1" : "0")}|{VectorToString(pros)}|{VectorToString(cons)}";
        }

        public override string ExportToString()
        {
            return base.ExportToString() +
                   $"\nТовар: {productName}\nАртикул: {productArticle}\nКатегория: {category}\nБренд: {brand}\nФото: {(hasPhotos ? "Есть" : "Нет")}\nМесяцев использования: {monthsOfUsage}\nРекомендую: {(recommendToOthers ? "Да" : "Нет")}\nДостоинства: {VectorToString(pros)}\nНедостатки: {VectorToString(cons)}";
        }

        // Методы интерфейса IRateable
        public int GetRating() => rating;
        public void SetRating(int r) => rating = r;
        public bool IsRatingValid() => rating >= 1 && rating <= 5;

        // Методы интерфейса IValidatable 

        // Собственные методы
        public void AddPro(string pro) { if (!string.IsNullOrEmpty(pro)) pros.Add(pro); }
        public void AddCon(string con) { if (!string.IsNullOrEmpty(con)) cons.Add(con); }
        public void RateSoundQuality(int score)
        {
            if (score >= 1 && score <= 10)
                AddPro($"Качество звука: {score}/10");
        }
        public void RateBuildQuality(int score)
        {
            if (score >= 1 && score <= 10)
                AddPro($"Качество сборки: {score}/10");
        }
        public double GetOverallQuality()
        {
            double total = rating * 2.0;
            if (monthsOfUsage > 0) total += 1.0;
            if (recommendToOthers) total += 1.0;
            if (hasPhotos) total += 0.5;
            return Math.Min(total, 10.0);
        }

        private string VectorToString(List<string> vec)
        {
            return vec.Any() ? string.Join("; ", vec) : "";
        }

        // Геттеры и сеттеры
        public string ProductName { get => productName; set => productName = value; }
        public string ProductArticle { get => productArticle; set => productArticle = value; }
        public string Category { get => category; set => category = value; }
        public string Brand { get => brand; set => brand = value; }
        public bool HasPhotos { get => hasPhotos; set => hasPhotos = value; }
        public List<string> Pros => pros;
        public List<string> Cons => cons;
        public int MonthsOfUsage { get => monthsOfUsage; set => monthsOfUsage = value; }
        public bool RecommendToOthers { get => recommendToOthers; set => recommendToOthers = value; }
    }
}