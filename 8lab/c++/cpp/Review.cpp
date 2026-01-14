#ifndef REVIEW_CPP
#define REVIEW_CPP

#include "Review.h"
#include <fstream>
#include <iostream>
#include <sstream>
#include <iomanip>
#include <algorithm>
#include <functional>
#include <numeric>
#include <ctime>
#include <list>
#include <vector>

using namespace std;

// Реализация конструктора шаблонного класса
template<typename Derived>
Review<Derived>::Review(const std::string& clientId, int rating,
    const std::string& comment, bool verified,
    const std::string& id, const std::string& date)
    : clientId(clientId.empty() ? "UNKNOWN" : clientId),
    rating(rating), comment(comment), isVerified(verified) {

    idReview = id.empty() ? generateReviewId() : id;
    dateReview = date.empty() ? getCurrentDate() : date;
    helpfulVotes = 0;
    unhelpfulVotes = 0;

    if (rating < 1 || rating > 5) {
        this->rating = 3;
    }
}

// НЕШАБЛОННЫЕ МЕТОДЫ 

template<typename Derived>
double Review<Derived>::calculateWeight() const {
    double weight = 1.0;

    if (isVerified) weight *= 1.5;
    if (rating == 5) weight *= 1.3;
    else if (rating == 4) weight *= 1.1;
    else if (rating == 1 || rating == 2) weight *= 0.7;

    if (helpfulVotes > 0) {
        double helpfulRatio = getHelpfulnessRatio();
        if (helpfulRatio > 0.7) weight *= 1.4;
        else if (helpfulRatio > 0.4) weight *= 1.2;
        else if (helpfulRatio > 0.1) weight *= 1.1;
    }

    if (comment.length() > 100) weight *= 1.3;
    else if (comment.length() > 50) weight *= 1.1;

    return weight;
}

template<typename Derived>
std::string Review<Derived>::prepareForFile() const {
    stringstream ss;
    ss << idReview << "|"
        << getReviewType() << "|"
        << clientId << "|"
        << rating << "|"
        << comment << "|"
        << dateReview << "|"
        << (isVerified ? "1" : "0") << "|"
        << helpfulVotes << "|"
        << unhelpfulVotes;
    return ss.str();
}

template<typename Derived>
std::string Review<Derived>::exportToString() const {
    stringstream ss;
    ss << "ID: " << idReview << "\n"
        << "Тип: " << getReviewType() << "\n"
        << "Клиент: " << clientId << "\n"
        << "Оценка: " << rating << "/5\n"
        << "Комментарий: " << comment << "\n"
        << "Дата: " << dateReview << "\n"
        << "Проверен: " << (isVerified ? "Да" : "Нет") << "\n"
        << "Полезных: " << helpfulVotes << "\n"
        << "Бесполезных: " << unhelpfulVotes << "\n"
        << "Вес: " << fixed << setprecision(2) << calculateWeight();
    return ss.str();
}

template<typename Derived>
void Review<Derived>::SaveToFile(const std::string& filename) const {
    ofstream file(filename, ios::app);
    if (file.is_open()) {
        file << prepareForFile() << endl;
        file.close();
    }
}

template<typename Derived>
void Review<Derived>::DisplayAllReviews(const std::string& filename) {
    ifstream file(filename);
    if (!file.is_open()) {
        cout << "Файл " << filename << " не найден!" << endl;
        return;
    }

    string line;
    vector<string> fileContents;
    while (getline(file, line)) {
        if (!line.empty()) {
            fileContents.push_back(line);
        }
    }
    file.close();

    int count = 0;
    for (const auto& line : fileContents) {
        cout << "\n--- Отзыв " << ++count << " ---" << endl;
        stringstream ss(line);
        string token;
        vector<string> tokens;

        while (getline(ss, token, '|')) {
            tokens.push_back(token);
        }

        if (tokens.size() >= 9) {
            cout << "ID: " << tokens[0] << endl;
            cout << "Тип: " << tokens[1] << endl;
            cout << "Клиент: " << tokens[2] << endl;
            cout << "Оценка: " << tokens[3] << "/5" << endl;
            cout << "Комментарий: " << tokens[4] << endl;
            cout << "Дата: " << tokens[5] << endl;
            cout << "Проверен: " << (tokens[6] == "1" ? "Да" : "Нет") << endl;
            cout << "Полезные: " << tokens[7] << endl;
            cout << "Бесполезные: " << tokens[8] << endl;
        }
    }
}

template<typename Derived>
double Review<Derived>::AverageRating(const std::string& productName) {
    ifstream file("reviews.txt");
    if (!file.is_open()) return 0.0;

    string line;
    vector<double> ratings;

    while (getline(file, line)) {
        if (line.empty()) continue;
        stringstream ss(line);
        string token;
        vector<string> tokens;

        while (getline(ss, token, '|')) {
            tokens.push_back(token);
        }

        if (tokens.size() >= 5) {
            if (tokens[1].find(productName) != string::npos || productName.empty()) {
                try {
                    ratings.push_back(stod(tokens[3]));
                }
                catch (...) {
                    continue;
                }
            }
        }
    }
    file.close();

    if (ratings.empty()) return 0.0;
    double sum = accumulate(ratings.begin(), ratings.end(), 0.0);
    return sum / ratings.size();
}

template<typename Derived>
void Review<Derived>::markAsHelpful() { helpfulVotes++; }

template<typename Derived>
void Review<Derived>::markAsUnhelpful() { unhelpfulVotes++; }

template<typename Derived>
double Review<Derived>::getHelpfulnessRatio() const {
    int totalVotes = helpfulVotes + unhelpfulVotes;
    if (totalVotes == 0) return 0.0;
    return static_cast<double>(helpfulVotes) / totalVotes;
}

template<typename Derived>
std::string Review<Derived>::generateReviewId(const std::string& prefix) {
    static int counter = 0;
    stringstream ss;
    ss << prefix << setw(6) << setfill('0') << ++counter;
    return ss.str();
}

template<typename Derived>
std::string Review<Derived>::getCurrentDate() {
    time_t now = time(0);
    tm* localTime = localtime(&now);
    char buffer[80];
    strftime(buffer, 80, "%d.%m.%Y %H:%M:%S", localTime);
    return buffer;
}

//  ШАБЛОННЫЕ МЕТОДЫ 

template<typename Derived>
bool Review<Derived>::compareByDate(const Derived* a, const Derived* b, bool newestFirst) {
    return newestFirst ? a->dateReview > b->dateReview : a->dateReview < b->dateReview;
}

template<typename Derived>
bool Review<Derived>::compareByRating(const Derived* a, const Derived* b, bool highestFirst) {
    return highestFirst ? a->rating > b->rating : a->rating < b->rating;
}

template<typename Derived>
bool Review<Derived>::compareByHelpfulness(const Derived* a, const Derived* b) {
    double ratioA = a->getHelpfulnessRatio();
    double ratioB = b->getHelpfulnessRatio();

    if (abs(ratioA - ratioB) < 0.001) {
        int totalA = a->helpfulVotes + a->unhelpfulVotes;
        int totalB = b->helpfulVotes + b->unhelpfulVotes;
        return totalA > totalB;
    }
    return ratioA > ratioB;
}

template<typename Derived>
bool Review<Derived>::compareByVerification(const Derived* a, const Derived* b) {
    if (a->isVerified && !b->isVerified) return true;
    if (!a->isVerified && b->isVerified) return false;
    return compareByDate(a, b, true);
}

template<typename Derived>
std::vector<std::shared_ptr<Derived>> Review<Derived>::sortReviews(
    std::vector<std::shared_ptr<Derived>>& reviews, SortCriteria criteria) {

    std::vector<std::shared_ptr<Derived>> sortedReviews = reviews;

    switch (criteria) {
    case SortCriteria::DATE_NEWEST:
        sort(sortedReviews.begin(), sortedReviews.end(),
            [](const auto& a, const auto& b) { return compareByDate(a.get(), b.get(), true); });
        break;
    case SortCriteria::DATE_OLDEST:
        sort(sortedReviews.begin(), sortedReviews.end(),
            [](const auto& a, const auto& b) { return compareByDate(a.get(), b.get(), false); });
        break;
    case SortCriteria::RATING_HIGHEST:
        sort(sortedReviews.begin(), sortedReviews.end(),
            [](const auto& a, const auto& b) { return compareByRating(a.get(), b.get(), true); });
        break;
    case SortCriteria::RATING_LOWEST:
        sort(sortedReviews.begin(), sortedReviews.end(),
            [](const auto& a, const auto& b) { return compareByRating(a.get(), b.get(), false); });
        break;
    case SortCriteria::HELPFULNESS:
        sort(sortedReviews.begin(), sortedReviews.end(),
            [](const auto& a, const auto& b) { return compareByHelpfulness(a.get(), b.get()); });
        break;
    case SortCriteria::VERIFIED_FIRST:
        sort(sortedReviews.begin(), sortedReviews.end(),
            [](const auto& a, const auto& b) { return compareByVerification(a.get(), b.get()); });
        break;
    }

    return sortedReviews;
}

template<typename Derived>
std::vector<std::shared_ptr<Derived>> Review<Derived>::sortReviewsCustom(
    std::vector<std::shared_ptr<Derived>>& reviews,
    std::function<bool(const Derived*, const Derived*)> comparator) {

    std::vector<std::shared_ptr<Derived>> sortedReviews = reviews;
    sort(sortedReviews.begin(), sortedReviews.end(),
        [&comparator](const auto& a, const auto& b) { return comparator(a.get(), b.get()); });
    return sortedReviews;
}


template<typename Derived>
std::list<std::shared_ptr<Derived>> Review<Derived>::vectorToList(
    const std::vector<std::shared_ptr<Derived>>& reviews) {

    std::list<std::shared_ptr<Derived>> reviewList;
    copy(reviews.begin(), reviews.end(), back_inserter(reviewList));
    return reviewList;
}

template<typename Derived>
std::vector<std::shared_ptr<Derived>> Review<Derived>::listToVector(
    const std::list<std::shared_ptr<Derived>>& reviewList) {

    std::vector<std::shared_ptr<Derived>> reviews;
    copy(reviewList.begin(), reviewList.end(), back_inserter(reviews));
    return reviews;
}

// СОРТИРОВКА СПИСКА
template<typename Derived>
void Review<Derived>::sortListByDate(std::list<std::shared_ptr<Derived>>& reviewList,
    bool newestFirst) {

    reviewList.sort([newestFirst](const auto& a, const auto& b) {
        return compareByDate(a.get(), b.get(), newestFirst);
        });
}

template<typename Derived>
void Review<Derived>::sortListByRating(std::list<std::shared_ptr<Derived>>& reviewList,
    bool highestFirst) {

    reviewList.sort([highestFirst](const auto& a, const auto& b) {
        return compareByRating(a.get(), b.get(), highestFirst);
        });
}

// ФИЛЬТРАЦИЯ ВЕКТОРА
template<typename Derived>
std::vector<std::shared_ptr<Derived>> Review<Derived>::filterVector(
    const std::vector<std::shared_ptr<Derived>>& reviews,
    std::function<bool(const Derived*)> predicate) {

    std::vector<std::shared_ptr<Derived>> filtered;
    copy_if(reviews.begin(), reviews.end(), back_inserter(filtered),
        [&predicate](const auto& review) { return predicate(review.get()); });
    return filtered;
}

// ФИЛЬТРАЦИЯ СПИСКА
template<typename Derived>
std::list<std::shared_ptr<Derived>> Review<Derived>::filterList(
    const std::list<std::shared_ptr<Derived>>& reviewList,
    std::function<bool(const Derived*)> predicate) {

    std::list<std::shared_ptr<Derived>> filtered;
    copy_if(reviewList.begin(), reviewList.end(), back_inserter(filtered),
        [&predicate](const auto& review) { return predicate(review.get()); });
    return filtered;
}

// ПОИСК В ВЕКТОРЕ
template<typename Derived>
std::shared_ptr<Derived> Review<Derived>::findInVectorById(
    const std::vector<std::shared_ptr<Derived>>& reviews,
    const std::string& id) {

    auto it = find_if(reviews.begin(), reviews.end(),
        [&id](const auto& review) { return review->getIdReview() == id; });

    return (it != reviews.end()) ? *it : nullptr;
}

template<typename Derived>
std::shared_ptr<Derived> Review<Derived>::findInVectorByClient(
    const std::vector<std::shared_ptr<Derived>>& reviews,
    const std::string& clientId) {

    auto it = find_if(reviews.begin(), reviews.end(),
        [&clientId](const auto& review) { return review->getClientId() == clientId; });

    return (it != reviews.end()) ? *it : nullptr;
}

// ПОИСК В СПИСКЕ
template<typename Derived>
std::shared_ptr<Derived> Review<Derived>::findInListById(
    const std::list<std::shared_ptr<Derived>>& reviewList,
    const std::string& id) {

    auto it = find_if(reviewList.begin(), reviewList.end(),
        [&id](const auto& review) { return review->getIdReview() == id; });

    return (it != reviewList.end()) ? *it : nullptr;
}

// ГРУППИРОВКА ПО РЕЙТИНГУ
template<typename Derived>
std::vector<std::vector<std::shared_ptr<Derived>>> Review<Derived>::groupByRating(
    const std::vector<std::shared_ptr<Derived>>& reviews) {

    vector<vector<shared_ptr<Derived>>> groups(6); // индексы 0-5 для рейтингов 0-5

    for (const auto& review : reviews) {
        int rating = review->getRating();
        if (rating >= 0 && rating <= 5) {
            groups[rating].push_back(review);
        }
    }

    return groups;
}

// СТАТИСТИКА ВЕКТОРА
template<typename Derived>
typename Review<Derived>::ReviewStats Review<Derived>::getVectorStats(
    const std::vector<std::shared_ptr<Derived>>& reviews) {

    ReviewStats stats = { 0, 0.0, 0, 5, 0 };

    if (reviews.empty()) return stats;

    stats.totalReviews = reviews.size();

    vector<int> ratings;
    transform(reviews.begin(), reviews.end(), back_inserter(ratings),
        [](const auto& review) { return review->getRating(); });

    stats.averageRating = accumulate(ratings.begin(), ratings.end(), 0.0) / ratings.size();
    stats.maxRating = *max_element(ratings.begin(), ratings.end());
    stats.minRating = *min_element(ratings.begin(), ratings.end());

    stats.verifiedCount = count_if(reviews.begin(), reviews.end(),
        [](const auto& review) { return review->getIsVerified(); });

    return stats;
}

// СТАТИСТИКА СПИСКА
template<typename Derived>
typename Review<Derived>::ReviewStats Review<Derived>::getListStats(
    const std::list<std::shared_ptr<Derived>>& reviewList) {

    ReviewStats stats = { 0, 0.0, 0, 5, 0 };

    if (reviewList.empty()) return stats;

    stats.totalReviews = reviewList.size();

    vector<int> ratings;
    transform(reviewList.begin(), reviewList.end(), back_inserter(ratings),
        [](const auto& review) { return review->getRating(); });

    stats.averageRating = accumulate(ratings.begin(), ratings.end(), 0.0) / ratings.size();
    stats.maxRating = *max_element(ratings.begin(), ratings.end());
    stats.minRating = *min_element(ratings.begin(), ratings.end());

    stats.verifiedCount = count_if(reviewList.begin(), reviewList.end(),
        [](const auto& review) { return review->getIsVerified(); });

    return stats;
}

// ДОБАВЛЕНИЕ ОТЗЫВОВ
template<typename Derived>
void Review<Derived>::addReviewsToVector(std::vector<std::shared_ptr<Derived>>& reviews,
    const std::vector<std::shared_ptr<Derived>>& newReviews) {

    reviews.insert(reviews.end(), newReviews.begin(), newReviews.end());
}

template<typename Derived>
void Review<Derived>::addReviewsToList(std::list<std::shared_ptr<Derived>>& reviewList,
    const std::vector<std::shared_ptr<Derived>>& newReviews) {

    copy(newReviews.begin(), newReviews.end(), back_inserter(reviewList));
}

// УДАЛЕНИЕ НИЗКОРЕЙТИНГОВЫХ
template<typename Derived>
void Review<Derived>::removeLowRatingFromVector(std::vector<std::shared_ptr<Derived>>& reviews,
    int minRating) {

    auto newEnd = remove_if(reviews.begin(), reviews.end(),
        [minRating](const auto& review) { return review->getRating() < minRating; });
    reviews.erase(newEnd, reviews.end());
}

template<typename Derived>
void Review<Derived>::removeLowRatingFromList(std::list<std::shared_ptr<Derived>>& reviewList,
    int minRating) {

    reviewList.remove_if([minRating](const auto& review) {
        return review->getRating() < minRating;
        });
}

// ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ
template<typename Derived>
bool Review<Derived>::isValidReview(const Derived* review) {
    return review != nullptr &&
        review->getRating() >= 1 &&
        review->getRating() <= 5 &&
        !review->getComment().empty();
}

template<typename Derived>
bool Review<Derived>::isHighRatingReview(const Derived* review) {
    return review != nullptr && review->getRating() >= 4;
}

template<typename Derived>
bool Review<Derived>::isVerifiedReview(const Derived* review) {
    return review != nullptr && review->getIsVerified();
}

