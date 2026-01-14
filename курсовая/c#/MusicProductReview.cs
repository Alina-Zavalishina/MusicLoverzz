using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Курсовая_работа_Музыкальный_магазин_с_
{
    public class MusicProductReview : Review<MusicProductReview>
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
            hasPhotos = false;
            monthsOfUsage = 0;
            recommendToOthers = true;
            pros = new List<string>();
            cons = new List<string>();
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
            this.pros = new List<string>();
            this.cons = new List<string>();
        }

        public override string GetReviewType() { return "Отзыв на музыкальный товар"; }
        public override string GetSubject() { return productName; }

        public override MusicProductReview Clone()
        {
            MusicProductReview copy = new MusicProductReview(
                clientId, productName, productArticle, category, brand,
                rating, comment, hasPhotos, monthsOfUsage, recommendToOthers
            );
            copy.idReview = GenerateReviewId("CPY");
            copy.dateReview = GetCurrentDate();
            copy.isVerified = isVerified;
            copy.helpfulVotes = helpfulVotes;
            copy.unhelpfulVotes = unhelpfulVotes;
            copy.pros = new List<string>(pros);
            copy.cons = new List<string>(cons);
            return copy;
        }

        public override double CalculateWeight()
        {
            double weight = base.CalculateWeight();

            if (monthsOfUsage >= 12) weight *= 1.5;
            else if (monthsOfUsage >= 6) weight *= 1.3;
            else if (monthsOfUsage >= 3) weight *= 1.2;

            if (hasPhotos) weight *= 1.4;

            if (pros.Count > 0 && cons.Count > 0) weight *= 1.3;
            else if (pros.Count > 0 || cons.Count > 0) weight *= 1.1;

            if (category == "Гитары" || category == "Клавишные" || category == "Ударные")
            {
                weight *= 1.2;
            }

            return weight;
        }

        public override string PrepareForFile()
        {
            return $"{idReview}|" +
                   $"{GetReviewType()}|" +
                   $"{clientId}|" +
                   $"{productName}|" +
                   $"{productArticle}|" +
                   $"{category}|" +
                   $"{brand}|" +
                   $"{rating}|" +
                   $"{comment}|" +
                   $"{dateReview}|" +
                   $"{(isVerified ? "1" : "0")}|" +
                   $"{helpfulVotes}|" +
                   $"{unhelpfulVotes}|" +
                   $"{(hasPhotos ? "1" : "0")}|" +
                   $"{monthsOfUsage}|" +
                   $"{(recommendToOthers ? "1" : "0")}|" +
                   $"{VectorToString(pros)}|" +
                   $"{VectorToString(cons)}";
        }

        public override string ExportToString()
        {
            return base.ExportToString() + "\n" +
                   $"Товар: {productName}\n" +
                   $"Артикул: {productArticle}\n" +
                   $"Категория: {category}\n" +
                   $"Бренд: {brand}\n" +
                   $"Фото: {(hasPhotos ? "Есть" : "Нет")}\n" +
                   $"Месяцев использования: {monthsOfUsage}\n" +
                   $"Рекомендую: {(recommendToOthers ? "Да" : "Нет")}\n" +
                   $"Достоинства: {VectorToString(pros)}\n" +
                   $"Недостатки: {VectorToString(cons)}";
        }

        public void AddPro(string pro)
        {
            if (!string.IsNullOrEmpty(pro)) pros.Add(pro);
        }

        public void AddCon(string con)
        {
            if (!string.IsNullOrEmpty(con)) cons.Add(con);
        }

        public void AddPros(List<string> newPros)
        {
            foreach (string pro in newPros)
            {
                if (!string.IsNullOrEmpty(pro))
                {
                    pros.Add(pro);
                }
            }
        }

        public void AddCons(List<string> newCons)
        {
            foreach (string con in newCons)
            {
                if (!string.IsNullOrEmpty(con))
                {
                    cons.Add(con);
                }
            }
        }

        public void RateSoundQuality(int score)
        {
            if (score >= 1 && score <= 10)
            {
                AddPro($"Качество звука: {score}/10");
            }
        }

        public void RateBuildQuality(int score)
        {
            if (score >= 1 && score <= 10)
            {
                AddPro($"Качество сборки: {score}/10");
            }
        }

        public double GetOverallQuality()
        {
            double total = rating * 2.0;
            if (monthsOfUsage > 0) total += 1.0;
            if (recommendToOthers) total += 1.0;
            if (hasPhotos) total += 0.5;
            return Math.Min(total, 10.0);
        }

        // Геттеры
        public string GetProductName() => productName;
        public string GetProductArticle() => productArticle;
        public string GetCategory() => category;
        public string GetBrand() => brand;
        public bool GetHasPhotos() => hasPhotos;
        public List<string> GetPros() => pros;
        public List<string> GetCons() => cons;
        public int GetMonthsOfUsage() => monthsOfUsage;
        public bool GetRecommendToOthers() => recommendToOthers;

        // Сеттеры
        public void SetProductName(string name) => productName = name;
        public void SetProductArticle(string article) => productArticle = article;
        public void SetCategory(string cat) => category = cat;
        public void SetBrand(string br) => brand = br;
        public void SetHasPhotos(bool photos) => hasPhotos = photos;
        public void SetMonthsOfUsage(int months) => monthsOfUsage = months;
        public void SetRecommendToOthers(bool recommend) => recommendToOthers = recommend;

        // Статические методы
        public static List<MusicProductReview> GetAllMusicReviews(List<Review<MusicProductReview>> reviews)
        {
            List<MusicProductReview> musicReviews = new List<MusicProductReview>();

            foreach (var review in reviews)
            {
                if (review is MusicProductReview musicReview)
                {
                    musicReviews.Add(musicReview);
                }
            }

            return musicReviews;
        }

        public static LinkedList<MusicProductReview> GetAllMusicReviews(LinkedList<Review<MusicProductReview>> reviewList)
        {
            LinkedList<MusicProductReview> musicReviews = new LinkedList<MusicProductReview>();

            foreach (var review in reviewList)
            {
                if (review is MusicProductReview musicReview)
                {
                    musicReviews.AddLast(musicReview);
                }
            }

            return musicReviews;
        }

        public static List<MusicProductReview> FilterByCategory(List<Review<MusicProductReview>> reviews, string category)
        {
            List<MusicProductReview> filtered = new List<MusicProductReview>();

            foreach (var review in reviews)
            {
                if (review is MusicProductReview musicReview && musicReview.GetCategory() == category)
                {
                    filtered.Add(musicReview);
                }
            }

            return filtered;
        }

        public static List<MusicProductReview> FilterByBrand(List<Review<MusicProductReview>> reviews, string brand)
        {
            List<MusicProductReview> filtered = new List<MusicProductReview>();

            foreach (var review in reviews)
            {
                if (review is MusicProductReview musicReview && musicReview.GetBrand() == brand)
                {
                    filtered.Add(musicReview);
                }
            }

            return filtered;
        }

        public static List<string> GetAllCategories(List<Review<MusicProductReview>> reviews)
        {
            List<string> categories = new List<string>();

            foreach (var review in reviews)
            {
                if (review is MusicProductReview musicReview)
                {
                    string category = musicReview.GetCategory();
                    if (!categories.Contains(category))
                    {
                        categories.Add(category);
                    }
                }
            }

            return categories;
        }

        public static List<string> GetAllBrands(List<Review<MusicProductReview>> reviews)
        {
            List<string> brands = new List<string>();

            foreach (var review in reviews)
            {
                if (review is MusicProductReview musicReview)
                {
                    string brand = musicReview.GetBrand();
                    if (!brands.Contains(brand))
                    {
                        brands.Add(brand);
                    }
                }
            }

            return brands;
        }

        public static double GetAverageRatingByCategory(List<Review<MusicProductReview>> reviews, string category)
        {
            List<int> ratings = new List<int>();

            foreach (var review in reviews)
            {
                if (review is MusicProductReview musicReview && musicReview.GetCategory() == category)
                {
                    ratings.Add(musicReview.GetRating());
                }
            }

            if (ratings.Count == 0) return 0.0;
            return ratings.Average();
        }

        public static double GetAverageRatingByBrand(List<Review<MusicProductReview>> reviews, string brand)
        {
            List<int> ratings = new List<int>();

            foreach (var review in reviews)
            {
                if (review is MusicProductReview musicReview && musicReview.GetBrand() == brand)
                {
                    ratings.Add(musicReview.GetRating());
                }
            }

            if (ratings.Count == 0) return 0.0;
            return ratings.Average();
        }

        public static List<Review<MusicProductReview>> ConvertToReviewVector(List<MusicProductReview> musicReviews)
        {
            List<Review<MusicProductReview>> reviews = new List<Review<MusicProductReview>>();

            foreach (var musicReview in musicReviews)
            {
                reviews.Add(musicReview);
            }

            return reviews;
        }

        public static LinkedList<Review<MusicProductReview>> ConvertToReviewList(List<MusicProductReview> musicReviews)
        {
            LinkedList<Review<MusicProductReview>> reviewList = new LinkedList<Review<MusicProductReview>>();

            foreach (var musicReview in musicReviews)
            {
                reviewList.AddLast(musicReview);
            }

            return reviewList;
        }

        private string VectorToString(List<string> vec)
        {
            if (vec.Count == 0) return "";
            return string.Join("; ", vec);
        }
    }
}