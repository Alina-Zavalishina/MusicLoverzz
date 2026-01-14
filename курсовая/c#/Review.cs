using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Курсовая_работа_Музыкальный_магазин_с_
{
    // Шаблонный класс Review
    public abstract class Review<T> where T : Review<T>
    {
        protected string idReview;
        protected string clientId;
        protected int rating;
        protected string comment;
        protected string dateReview;
        protected bool isVerified;
        protected int helpfulVotes;
        protected int unhelpfulVotes;

        public Review(string clientId = "", int rating = 0,
            string comment = "", bool verified = false,
            string id = "", string date = "")
        {
            this.clientId = string.IsNullOrEmpty(clientId) ? "UNKNOWN" : clientId;
            this.rating = rating;
            this.comment = comment;
            this.isVerified = verified;

            idReview = string.IsNullOrEmpty(id) ? GenerateReviewId() : id;
            dateReview = string.IsNullOrEmpty(date) ? GetCurrentDate() : date;
            helpfulVotes = 0;
            unhelpfulVotes = 0;

            if (rating < 1 || rating > 5)
            {
                this.rating = 3;
            }
        }

        // Абстрактные методы
        public abstract string GetReviewType();
        public abstract string GetSubject();
        public abstract T Clone();

        // Виртуальные методы с реализацией по умолчанию
        public virtual double CalculateWeight()
        {
            double weight = 1.0;

            if (isVerified) weight *= 1.5;
            if (rating == 5) weight *= 1.3;
            else if (rating == 4) weight *= 1.1;
            else if (rating == 1 || rating == 2) weight *= 0.7;

            if (helpfulVotes > 0)
            {
                double helpfulRatio = GetHelpfulnessRatio();
                if (helpfulRatio > 0.7) weight *= 1.4;
                else if (helpfulRatio > 0.4) weight *= 1.2;
                else if (helpfulRatio > 0.1) weight *= 1.1;
            }

            if (comment.Length > 100) weight *= 1.3;
            else if (comment.Length > 50) weight *= 1.1;

            return weight;
        }

        public virtual string PrepareForFile()
        {
            return $"{idReview}|{GetReviewType()}|{clientId}|{rating}|{comment}|{dateReview}|{(isVerified ? "1" : "0")}|{helpfulVotes}|{unhelpfulVotes}";
        }

        public virtual string ExportToString()
        {
            return $"ID: {idReview}\n" +
                   $"Тип: {GetReviewType()}\n" +
                   $"Клиент: {clientId}\n" +
                   $"Оценка: {rating}/5\n" +
                   $"Комментарий: {comment}\n" +
                   $"Дата: {dateReview}\n" +
                   $"Проверен: {(isVerified ? "Да" : "Нет")}\n" +
                   $"Полезных: {helpfulVotes}\n" +
                   $"Бесполезных: {unhelpfulVotes}\n" +
                   $"Вес: {CalculateWeight():F2}";
        }

        // Невиртуальные методы
        public void SaveToFile(string filename = "reviews.txt")
        {
            try
            {
                using (StreamWriter file = new StreamWriter(filename, true))
                {
                    file.WriteLine(PrepareForFile());
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"Ошибка при сохранении в файл {filename}");
            }
        }

        public static void DisplayAllReviews(string filename = "reviews.txt")
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine($"Файл {filename} не найден!");
                return;
            }

            string[] lines = File.ReadAllLines(filename);
            int count = 0;

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;

                Console.WriteLine($"\n--- Отзыв {++count} ---");
                string[] tokens = line.Split('|');

                if (tokens.Length >= 9)
                {
                    Console.WriteLine($"ID: {tokens[0]}");
                    Console.WriteLine($"Тип: {tokens[1]}");
                    Console.WriteLine($"Клиент: {tokens[2]}");
                    Console.WriteLine($"Оценка: {tokens[3]}/5");
                    Console.WriteLine($"Комментарий: {tokens[4]}");
                    Console.WriteLine($"Дата: {tokens[5]}");
                    Console.WriteLine($"Проверен: {(tokens[6] == "1" ? "Да" : "Нет")}");
                    Console.WriteLine($"Полезные: {tokens[7]}");
                    Console.WriteLine($"Бесполезные: {tokens[8]}");
                }
            }
        }

        public static double AverageRating(string productName)
        {
            if (!File.Exists("reviews.txt"))
                return 0.0;

            string[] lines = File.ReadAllLines("reviews.txt");
            List<double> ratings = new List<double>();

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;

                string[] tokens = line.Split('|');
                if (tokens.Length >= 5)
                {
                    if (tokens[1].Contains(productName) || string.IsNullOrEmpty(productName))
                    {
                        if (double.TryParse(tokens[3], out double rating))
                        {
                            ratings.Add(rating);
                        }
                    }
                }
            }

            if (ratings.Count == 0) return 0.0;
            return ratings.Average();
        }

        public void MarkAsHelpful() { helpfulVotes++; }
        public void MarkAsUnhelpful() { unhelpfulVotes++; }

        public double GetHelpfulnessRatio()
        {
            int totalVotes = helpfulVotes + unhelpfulVotes;
            if (totalVotes == 0) return 0.0;
            return (double)helpfulVotes / totalVotes;
        }

        // Критерии сортировки
        public enum SortCriteria
        {
            DATE_NEWEST,
            DATE_OLDEST,
            RATING_HIGHEST,
            RATING_LOWEST,
            HELPFULNESS,
            VERIFIED_FIRST
        }

        // ШАБЛОННЫЕ МЕТОДЫ

        public static List<T> SortReviews(List<T> reviews, SortCriteria criteria = SortCriteria.DATE_NEWEST)
        {
            List<T> sortedReviews = new List<T>(reviews);

            switch (criteria)
            {
                case SortCriteria.DATE_NEWEST:
                    sortedReviews.Sort((a, b) => CompareByDate(a, b, true));
                    break;
                case SortCriteria.DATE_OLDEST:
                    sortedReviews.Sort((a, b) => CompareByDate(a, b, false));
                    break;
                case SortCriteria.RATING_HIGHEST:
                    sortedReviews.Sort((a, b) => CompareByRating(a, b, true));
                    break;
                case SortCriteria.RATING_LOWEST:
                    sortedReviews.Sort((a, b) => CompareByRating(a, b, false));
                    break;
                case SortCriteria.HELPFULNESS:
                    sortedReviews.Sort((a, b) => CompareByHelpfulness(a, b));
                    break;
                case SortCriteria.VERIFIED_FIRST:
                    sortedReviews.Sort((a, b) => CompareByVerification(a, b));
                    break;
            }

            return sortedReviews;
        }

        public static List<T> SortReviewsCustom(List<T> reviews,
            Func<T, T, bool> comparator)
        {
            List<T> sortedReviews = new List<T>(reviews);
            sortedReviews.Sort((a, b) => comparator(a, b) ? -1 : 1);
            return sortedReviews;
        }

        public static LinkedList<T> VectorToList(List<T> reviews)
        {
            return new LinkedList<T>(reviews);
        }

        public static List<T> ListToVector(LinkedList<T> reviewList)
        {
            return new List<T>(reviewList);
        }

        public static void SortListByDate(LinkedList<T> reviewList, bool newestFirst = true)
        {
            List<T> tempList = new List<T>(reviewList);
            tempList.Sort((a, b) => CompareByDate(a, b, newestFirst));

            reviewList.Clear();
            foreach (var item in tempList)
            {
                reviewList.AddLast(item);
            }
        }

        public static void SortListByRating(LinkedList<T> reviewList, bool highestFirst = true)
        {
            List<T> tempList = new List<T>(reviewList);
            tempList.Sort((a, b) => CompareByRating(a, b, highestFirst));

            reviewList.Clear();
            foreach (var item in tempList)
            {
                reviewList.AddLast(item);
            }
        }

        public static List<T> FilterVector(List<T> reviews,
            Func<T, bool> predicate)
        {
            return reviews.Where(predicate).ToList();
        }

        public static LinkedList<T> FilterList(LinkedList<T> reviewList,
            Func<T, bool> predicate)
        {
            return new LinkedList<T>(reviewList.Where(predicate));
        }

        public static T FindInVectorById(List<T> reviews, string id)
        {
            return reviews.FirstOrDefault(r => r.GetIdReview() == id);
        }

        public static T FindInVectorByClient(List<T> reviews, string clientId)
        {
            return reviews.FirstOrDefault(r => r.GetClientId() == clientId);
        }

        public static T FindInListById(LinkedList<T> reviewList, string id)
        {
            return reviewList.FirstOrDefault(r => r.GetIdReview() == id);
        }

        public static List<List<T>> GroupByRating(List<T> reviews)
        {
            List<List<T>> groups = new List<List<T>>();
            for (int i = 0; i <= 5; i++) // индексы 0-5 для рейтингов 0-5
            {
                groups.Add(new List<T>());
            }

            foreach (var review in reviews)
            {
                int rating = review.GetRating();
                if (rating >= 0 && rating <= 5)
                {
                    groups[rating].Add(review);
                }
            }

            return groups;
        }

        // Статистика
        public struct ReviewStats
        {
            public int totalReviews;
            public double averageRating;
            public int maxRating;
            public int minRating;
            public int verifiedCount;
        }

        public static ReviewStats GetVectorStats(List<T> reviews)
        {
            ReviewStats stats = new ReviewStats
            {
                totalReviews = 0,
                averageRating = 0.0,
                maxRating = 0,
                minRating = 5,
                verifiedCount = 0
            };

            if (reviews.Count == 0) return stats;

            stats.totalReviews = reviews.Count;
            stats.averageRating = reviews.Average(r => r.GetRating());
            stats.maxRating = reviews.Max(r => r.GetRating());
            stats.minRating = reviews.Min(r => r.GetRating());
            stats.verifiedCount = reviews.Count(r => r.GetIsVerified());

            return stats;
        }

        public static ReviewStats GetListStats(LinkedList<T> reviewList)
        {
            ReviewStats stats = new ReviewStats
            {
                totalReviews = 0,
                averageRating = 0.0,
                maxRating = 0,
                minRating = 5,
                verifiedCount = 0
            };

            if (reviewList.Count == 0) return stats;

            stats.totalReviews = reviewList.Count;
            stats.averageRating = reviewList.Average(r => r.GetRating());
            stats.maxRating = reviewList.Max(r => r.GetRating());
            stats.minRating = reviewList.Min(r => r.GetRating());
            stats.verifiedCount = reviewList.Count(r => r.GetIsVerified());

            return stats;
        }

        public static void AddReviewsToVector(List<T> reviews, List<T> newReviews)
        {
            reviews.AddRange(newReviews);
        }

        public static void AddReviewsToList(LinkedList<T> reviewList, List<T> newReviews)
        {
            foreach (var review in newReviews)
            {
                reviewList.AddLast(review);
            }
        }

        public static void RemoveLowRatingFromVector(List<T> reviews, int minRating)
        {
            reviews.RemoveAll(r => r.GetRating() < minRating);
        }

        public static void RemoveLowRatingFromList(LinkedList<T> reviewList, int minRating)
        {
            var node = reviewList.First;
            while (node != null)
            {
                var next = node.Next;
                if (node.Value.GetRating() < minRating)
                {
                    reviewList.Remove(node);
                }
                node = next;
            }
        }

        // НЕШАБЛОННЫЕ МЕТОДЫ

        // Геттеры
        public string GetIdReview() => idReview;
        public string GetClientId() => clientId;
        public int GetRating() => rating;
        public string GetComment() => comment;
        public string GetDateReview() => dateReview;
        public bool GetIsVerified() => isVerified;
        public int GetHelpfulVotes() => helpfulVotes;
        public int GetUnhelpfulVotes() => unhelpfulVotes;

        // Сеттеры
        public void SetIdReview(string id) => idReview = id;
        public void SetDateReview(string date) => dateReview = date;
        public void SetRating(int r)
        {
            if (r >= 1 && r <= 5) rating = r;
        }
        public void SetComment(string com) => comment = com;
        public void VerifyReview() => isVerified = true;
        public void UnverifyReview() => isVerified = false;
        public void SetClientId(string id) => clientId = id;
        public void SetHelpfulVotes(int votes) => helpfulVotes = votes;
        public void SetUnhelpfulVotes(int votes) => unhelpfulVotes = votes;

        // Статические вспомогательные методы
        private static int counter = 0;

        public static string GenerateReviewId(string prefix = "REV")
        {
            counter++;
            return $"{prefix}{counter:D6}";
        }

        public static string GetCurrentDate()
        {
            return DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        }

        public static int CompareByDate(T a, T b, bool newestFirst = true)
        {
            return newestFirst
                ? string.Compare(b.GetDateReview(), a.GetDateReview(), StringComparison.Ordinal)
                : string.Compare(a.GetDateReview(), b.GetDateReview(), StringComparison.Ordinal);
        }

        public static int CompareByRating(T a, T b, bool highestFirst = true)
        {
            return highestFirst
                ? b.GetRating().CompareTo(a.GetRating())
                : a.GetRating().CompareTo(b.GetRating());
        }

        public static int CompareByHelpfulness(T a, T b)
        {
            double ratioA = a.GetHelpfulnessRatio();
            double ratioB = b.GetHelpfulnessRatio();

            if (Math.Abs(ratioA - ratioB) < 0.001)
            {
                int totalA = a.GetHelpfulVotes() + a.GetUnhelpfulVotes();
                int totalB = b.GetHelpfulVotes() + b.GetUnhelpfulVotes();
                return totalB.CompareTo(totalA);
            }
            return ratioB.CompareTo(ratioA);
        }

        public static int CompareByVerification(T a, T b)
        {
            if (a.GetIsVerified() && !b.GetIsVerified()) return -1;
            if (!a.GetIsVerified() && b.GetIsVerified()) return 1;
            return CompareByDate(a, b, true);
        }

        public static bool IsValidReview(T review)
        {
            return review != null &&
                   review.GetRating() >= 1 &&
                   review.GetRating() <= 5 &&
                   !string.IsNullOrEmpty(review.GetComment());
        }

        public static bool IsHighRatingReview(T review)
        {
            return review != null && review.GetRating() >= 4;
        }

        public static bool IsVerifiedReview(T review)
        {
            return review != null && review.GetIsVerified();
        }
    }
}