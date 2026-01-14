#define _CRT_SECURE_NO_WARNINGS
#include "MusicProductReview.h"
#include <sstream>
#include <algorithm>
#include <iomanip>
#include <memory>
#include <list>
#include <vector>
#include <numeric>

using namespace std;

MusicProductReview::MusicProductReview()
    : hasPhotos(false), monthsOfUsage(0), recommendToOthers(true) {}

MusicProductReview::MusicProductReview(const std::string& clientId,
    const std::string& productName, const std::string& productArticle,
    const std::string& category, const std::string& brand, int rating,
    const std::string& comment, bool hasPhotos, int monthsOfUsage, bool recommend)
    : Review<MusicProductReview>(clientId, rating, comment), productName(productName),
    productArticle(productArticle), category(category), brand(brand),
    hasPhotos(hasPhotos), monthsOfUsage(monthsOfUsage), recommendToOthers(recommend) {}

MusicProductReview* MusicProductReview::clone() const {
    MusicProductReview* copy = new MusicProductReview(
        clientId, productName, productArticle, category, brand,
        rating, comment, hasPhotos, monthsOfUsage, recommendToOthers
    );
    copy->idReview = generateReviewId("CPY");
    copy->dateReview = getCurrentDate();
    copy->isVerified = isVerified;
    copy->helpfulVotes = helpfulVotes;
    copy->unhelpfulVotes = unhelpfulVotes;
    copy->pros = pros;
    copy->cons = cons;
    return copy;
}

double MusicProductReview::calculateWeight() const {
    double weight = Review<MusicProductReview>::calculateWeight();

    if (monthsOfUsage >= 12) weight *= 1.5;
    else if (monthsOfUsage >= 6) weight *= 1.3;
    else if (monthsOfUsage >= 3) weight *= 1.2;

    if (hasPhotos) weight *= 1.4;

    if (!pros.empty() && !cons.empty()) weight *= 1.3;
    else if (!pros.empty() || !cons.empty()) weight *= 1.1;

    if (category == "Гитары" || category == "Клавишные" || category == "Ударные") {
        weight *= 1.2;
    }

    return weight;
}

std::string MusicProductReview::prepareForFile() const {
    stringstream ss;
    ss << idReview << "|"
        << getReviewType() << "|"
        << clientId << "|"
        << productName << "|"
        << productArticle << "|"
        << category << "|"
        << brand << "|"
        << rating << "|"
        << comment << "|"
        << dateReview << "|"
        << (isVerified ? "1" : "0") << "|"
        << helpfulVotes << "|"
        << unhelpfulVotes << "|"
        << (hasPhotos ? "1" : "0") << "|"
        << monthsOfUsage << "|"
        << (recommendToOthers ? "1" : "0") << "|"
        << vectorToString(pros) << "|"
        << vectorToString(cons);
    return ss.str();
}

std::string MusicProductReview::exportToString() const {
    stringstream ss;
    ss << Review<MusicProductReview>::exportToString() << "\n"
        << "Товар: " << productName << "\n"
        << "Артикул: " << productArticle << "\n"
        << "Категория: " << category << "\n"
        << "Бренд: " << brand << "\n"
        << "Фото: " << (hasPhotos ? "Есть" : "Нет") << "\n"
        << "Месяцев использования: " << monthsOfUsage << "\n"
        << "Рекомендую: " << (recommendToOthers ? "Да" : "Нет") << "\n"
        << "Достоинства: " << vectorToString(pros) << "\n"
        << "Недостатки: " << vectorToString(cons);
    return ss.str();
}

void MusicProductReview::addPro(const std::string& pro) {
    if (!pro.empty()) pros.push_back(pro);
}

void MusicProductReview::addCon(const std::string& con) {
    if (!con.empty()) cons.push_back(con);
}

void MusicProductReview::addPros(const std::vector<std::string>& newPros) {
    for (const auto& pro : newPros) {
        if (!pro.empty()) {
            pros.push_back(pro);
        }
    }
}

void MusicProductReview::addCons(const std::vector<std::string>& newCons) {
    for (const auto& con : newCons) {
        if (!con.empty()) {
            cons.push_back(con);
        }
    }
}

void MusicProductReview::rateSoundQuality(int score) {
    if (score >= 1 && score <= 10) {
        addPro("Качество звука: " + to_string(score) + "/10");
    }
}

void MusicProductReview::rateBuildQuality(int score) {
    if (score >= 1 && score <= 10) {
        addPro("Качество сборки: " + to_string(score) + "/10");
    }
}

double MusicProductReview::getOverallQuality() const {
    double total = rating * 2.0;
    if (monthsOfUsage > 0) total += 1.0;
    if (recommendToOthers) total += 1.0;
    if (hasPhotos) total += 0.5;
    return min(total, 10.0);
}

std::string MusicProductReview::vectorToString(const std::vector<std::string>& vec) const {
    if (vec.empty()) return "";
    stringstream ss;
    for (size_t i = 0; i < vec.size(); i++) {
        ss << vec[i];
        if (i < vec.size() - 1) ss << "; ";
    }
    return ss.str();
}

// ПОЛУЧЕНИЕ ВСЕХ МУЗЫКАЛЬНЫХ ОТЗЫВОВ ИЗ VECTOR
std::vector<std::shared_ptr<MusicProductReview>>
MusicProductReview::getAllMusicReviews(
    const std::vector<std::shared_ptr<Review<MusicProductReview>>>& reviews) {

    std::vector<std::shared_ptr<MusicProductReview>> musicReviews;

    for (const auto& review : reviews) {
        if (auto musicReview = tryCast(review)) {
            musicReviews.push_back(musicReview);
        }
    }

    return musicReviews;
}

std::list<std::shared_ptr<MusicProductReview>>
MusicProductReview::getAllMusicReviews(
    const std::list<std::shared_ptr<Review<MusicProductReview>>>& reviewList) {

    std::list<std::shared_ptr<MusicProductReview>> musicReviews;

    for (const auto& review : reviewList) {
        if (auto musicReview = tryCast(review)) {
            musicReviews.push_back(musicReview);
        }
    }

    return musicReviews;
}

// ФИЛЬТРАЦИЯ ПО КАТЕГОРИИ
std::vector<std::shared_ptr<MusicProductReview>>
MusicProductReview::filterByCategory(
    const std::vector<std::shared_ptr<Review<MusicProductReview>>>& reviews,
    const std::string& category) {

    std::vector<std::shared_ptr<MusicProductReview>> filtered;

    for (const auto& review : reviews) {
        if (auto musicReview = tryCast(review)) {
            if (musicReview->getCategory() == category) {
                filtered.push_back(musicReview);
            }
        }
    }

    return filtered;
}

// ФИЛЬТРАЦИЯ ПО БРЕНДУ
std::vector<std::shared_ptr<MusicProductReview>>
MusicProductReview::filterByBrand(
    const std::vector<std::shared_ptr<Review<MusicProductReview>>>& reviews,
    const std::string& brand) {

    std::vector<std::shared_ptr<MusicProductReview>> filtered;

    for (const auto& review : reviews) {
        if (auto musicReview = tryCast(review)) {
            if (musicReview->getBrand() == brand) {
                filtered.push_back(musicReview);
            }
        }
    }

    return filtered;
}

// ПОЛУЧЕНИЕ ВСЕХ КАТЕГОРИЙ ИЗ ОТЗЫВОВ
std::vector<std::string> MusicProductReview::getAllCategories(
    const std::vector<std::shared_ptr<Review<MusicProductReview>>>& reviews) {

    std::vector<std::string> categories;

    for (const auto& review : reviews) {
        if (auto musicReview = tryCast(review)) {
            std::string category = musicReview->getCategory();
            // Проверяем, нет ли уже такой категории в векторе
            if (std::find(categories.begin(), categories.end(), category) == categories.end()) {
                categories.push_back(category);
            }
        }
    }

    return categories;
}

// ПОЛУЧЕНИЕ ВСЕХ БРЕНДОВ ИЗ ОТЗЫВОВ
std::vector<std::string> MusicProductReview::getAllBrands(
    const std::vector<std::shared_ptr<Review<MusicProductReview>>>& reviews) {

    std::vector<std::string> brands;

    for (const auto& review : reviews) {
        if (auto musicReview = tryCast(review)) {
            std::string brand = musicReview->getBrand();
            // Проверяем, нет ли уже такого бренда в векторе
            if (std::find(brands.begin(), brands.end(), brand) == brands.end()) {
                brands.push_back(brand);
            }
        }
    }

    return brands;
}

// СРЕДНИЙ РЕЙТИНГ ПО КАТЕГОРИИ
double MusicProductReview::getAverageRatingByCategory(
    const std::vector<std::shared_ptr<Review<MusicProductReview>>>& reviews,
    const std::string& category) {

    std::vector<int> ratings;

    for (const auto& review : reviews) {
        if (auto musicReview = tryCast(review)) {
            if (musicReview->getCategory() == category) {
                ratings.push_back(musicReview->getRating());
            }
        }
    }

    if (ratings.empty()) return 0.0;

    double sum = std::accumulate(ratings.begin(), ratings.end(), 0.0);
    return sum / ratings.size();
}

// СРЕДНИЙ РЕЙТИНГ ПО БРЕНДУ
double MusicProductReview::getAverageRatingByBrand(
    const std::vector<std::shared_ptr<Review<MusicProductReview>>>& reviews,
    const std::string& brand) {

    std::vector<int> ratings;

    for (const auto& review : reviews) {
        if (auto musicReview = tryCast(review)) {
            if (musicReview->getBrand() == brand) {
                ratings.push_back(musicReview->getRating());
            }
        }
    }

    if (ratings.empty()) return 0.0;

    double sum = std::accumulate(ratings.begin(), ratings.end(), 0.0);
    return sum / ratings.size();
}

// КОНВЕРТАЦИЯ В VECTOR<Review>
std::vector<std::shared_ptr<Review<MusicProductReview>>>
MusicProductReview::convertToReviewVector(
    const std::vector<std::shared_ptr<MusicProductReview>>& musicReviews) {

    std::vector<std::shared_ptr<Review<MusicProductReview>>> reviews;

    // Используем std::transform для конвертации
    std::transform(musicReviews.begin(), musicReviews.end(),
        std::back_inserter(reviews),
        [](const std::shared_ptr<MusicProductReview>& musicReview) {
            return std::static_pointer_cast<Review<MusicProductReview>>(musicReview);
        });

    return reviews;
}

// КОНВЕРТАЦИЯ В LIST<Review>
std::list<std::shared_ptr<Review<MusicProductReview>>>
MusicProductReview::convertToReviewList(
    const std::vector<std::shared_ptr<MusicProductReview>>& musicReviews) {

    std::list<std::shared_ptr<Review<MusicProductReview>>> reviewList;

    // Копируем элементы из вектора в список
    for (const auto& musicReview : musicReviews) {
        reviewList.push_back(std::static_pointer_cast<Review<MusicProductReview>>(musicReview));
    }

    return reviewList;
}

// ВСПОМОГАТЕЛЬНЫЙ МЕТОД ДЛЯ ПРЕОБРАЗОВАНИЯ ТИПА
std::shared_ptr<MusicProductReview> MusicProductReview::tryCast(
    const std::shared_ptr<Review<MusicProductReview>>& review) {

    // Пытаемся привести указатель к производному типу
    return std::dynamic_pointer_cast<MusicProductReview>(review);
}