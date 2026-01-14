#ifndef REVIEW_H
#define REVIEW_H

#include <string>
#include <memory>
#include <vector>
#include <list>
#include <functional>
#include <algorithm>
#include <iostream>

// Шаблонный класс Review
template<typename Derived>
class Review {
protected:
    std::string idReview;
    std::string clientId;
    int rating;
    std::string comment;
    std::string dateReview;
    bool isVerified;
    int helpfulVotes;
    int unhelpfulVotes;

public:
    Review(const std::string& clientId = "", int rating = 0,
        const std::string& comment = "", bool verified = false,
        const std::string& id = "", const std::string& date = "");

    virtual ~Review() {}

    //  виртуальные методы 
    virtual std::string getReviewType() const = 0;
    virtual std::string getSubject() const = 0;
    virtual Derived* clone() const = 0;

    // Виртуальные методы с реализацией по умолчанию
    virtual double calculateWeight() const;
    virtual std::string prepareForFile() const;
    virtual std::string exportToString() const;

    // Невиртуальные методы
    void SaveToFile(const std::string& filename = "reviews.txt") const;
    static void DisplayAllReviews(const std::string& filename = "reviews.txt");
    static double AverageRating(const std::string& productName);

    void markAsHelpful();
    void markAsUnhelpful();
    double getHelpfulnessRatio() const;

    // Критерии сортировки
    enum class SortCriteria {
        DATE_NEWEST,
        DATE_OLDEST,
        RATING_HIGHEST,
        RATING_LOWEST,
        HELPFULNESS,
        VERIFIED_FIRST
    };

    // ШАБЛОННЫЕ МЕТОДЫ

    // Сортировка вектора
    static std::vector<std::shared_ptr<Derived>> sortReviews(
        std::vector<std::shared_ptr<Derived>>& reviews,
        SortCriteria criteria = SortCriteria::DATE_NEWEST);

    // Пользовательская сортировка вектора
    static std::vector<std::shared_ptr<Derived>> sortReviewsCustom(
        std::vector<std::shared_ptr<Derived>>& reviews,
        std::function<bool(const Derived*, const Derived*)> comparator);

    // Конвертация между vector и list
    static std::list<std::shared_ptr<Derived>> vectorToList(
        const std::vector<std::shared_ptr<Derived>>& reviews);

    static std::vector<std::shared_ptr<Derived>> listToVector(
        const std::list<std::shared_ptr<Derived>>& reviewList);

    // Сортировка списка
    static void sortListByDate(std::list<std::shared_ptr<Derived>>& reviewList,
        bool newestFirst = true);

    static void sortListByRating(std::list<std::shared_ptr<Derived>>& reviewList,
        bool highestFirst = true);

    // Фильтрация вектора
    static std::vector<std::shared_ptr<Derived>> filterVector(
        const std::vector<std::shared_ptr<Derived>>& reviews,
        std::function<bool(const Derived*)> predicate);

    // Фильтрация списка
    static std::list<std::shared_ptr<Derived>> filterList(
        const std::list<std::shared_ptr<Derived>>& reviewList,
        std::function<bool(const Derived*)> predicate);

    // Поиск в векторе
    static std::shared_ptr<Derived> findInVectorById(
        const std::vector<std::shared_ptr<Derived>>& reviews,
        const std::string& id);

    static std::shared_ptr<Derived> findInVectorByClient(
        const std::vector<std::shared_ptr<Derived>>& reviews,
        const std::string& clientId);

    // Поиск в списке
    static std::shared_ptr<Derived> findInListById(
        const std::list<std::shared_ptr<Derived>>& reviewList,
        const std::string& id);

    // Группировка по рейтингу
    static std::vector<std::vector<std::shared_ptr<Derived>>> groupByRating(
        const std::vector<std::shared_ptr<Derived>>& reviews);

    // Статистика
    struct ReviewStats {
        int totalReviews;
        double averageRating;
        int maxRating;
        int minRating;
        int verifiedCount;
    };

    static ReviewStats getVectorStats(const std::vector<std::shared_ptr<Derived>>& reviews);
    static ReviewStats getListStats(const std::list<std::shared_ptr<Derived>>& reviewList);

    // Работа с контейнерами
    static void addReviewsToVector(std::vector<std::shared_ptr<Derived>>& reviews,
        const std::vector<std::shared_ptr<Derived>>& newReviews);

    static void addReviewsToList(std::list<std::shared_ptr<Derived>>& reviewList,
        const std::vector<std::shared_ptr<Derived>>& newReviews);

    static void removeLowRatingFromVector(std::vector<std::shared_ptr<Derived>>& reviews,
        int minRating);

    static void removeLowRatingFromList(std::list<std::shared_ptr<Derived>>& reviewList,
        int minRating);

    // НЕШАБЛОННЫЕ МЕТОДЫ 

    // Геттеры
    std::string getIdReview() const { return idReview; }
    std::string getClientId() const { return clientId; }
    int getRating() const { return rating; }
    std::string getComment() const { return comment; }
    std::string getDateReview() const { return dateReview; }
    bool getIsVerified() const { return isVerified; }
    int getHelpfulVotes() const { return helpfulVotes; }
    int getUnhelpfulVotes() const { return unhelpfulVotes; }

    // Сеттеры
    void setIdReview(const std::string& id) { idReview = id; }
    void setDateReview(const std::string& date) { dateReview = date; }
    void setRating(int r) { if (r >= 1 && r <= 5) rating = r; }
    void setComment(const std::string& com) { comment = com; }
    void verifyReview() { isVerified = true; }
    void unverifyReview() { isVerified = false; }
    void setClientId(const std::string& id) { clientId = id; }
    void setHelpfulVotes(int votes) { helpfulVotes = votes; }
    void setUnhelpfulVotes(int votes) { unhelpfulVotes = votes; }

    // Статические вспомогательные методы
    static std::string generateReviewId(const std::string& prefix = "REV");
    static std::string getCurrentDate();

    static bool compareByDate(const Derived* a, const Derived* b, bool newestFirst = true);
    static bool compareByRating(const Derived* a, const Derived* b, bool highestFirst = true);
    static bool compareByHelpfulness(const Derived* a, const Derived* b);
    static bool compareByVerification(const Derived* a, const Derived* b);

    static bool isValidReview(const Derived* review);
    static bool isHighRatingReview(const Derived* review);
    static bool isVerifiedReview(const Derived* review);
};


#include "Review.cpp"

#endif 