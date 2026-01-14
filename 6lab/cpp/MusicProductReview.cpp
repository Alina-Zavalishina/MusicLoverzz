#include "MusicProductReview.h"
#include <iostream>
#include <sstream>
#include <algorithm>
#include <iomanip>

using namespace std;

// Конструктор по умолчанию
MusicProductReview::MusicProductReview()
    : hasPhotos(false), monthsOfUsage(0), recommendToOthers(true) {}

// Конструктор с параметрами
MusicProductReview::MusicProductReview(const std::string& clientId,
    const std::string& productName,
    const std::string& productArticle,
    const std::string& category,
    const std::string& brand, int rating,
    const std::string& comment,
    bool hasPhotos, int monthsOfUsage,
    bool recommend)
    : Review(clientId, rating, comment),
    productName(productName), productArticle(productArticle),
    category(category), brand(brand), hasPhotos(hasPhotos),
    monthsOfUsage(monthsOfUsage), recommendToOthers(recommend) {}

// Отобразить детали отзыва
void MusicProductReview::displayDetails() const {
    cout << "\n=== ОТЗЫВ НА МУЗЫКАЛЬНЫЙ ТОВАР ===" << endl;
    cout << "Товар: " << productName << endl;
    cout << "Артикул: " << productArticle << endl;
    cout << "Категория: " << category << endl;
    cout << "Бренд: " << brand << endl;
    cout << "Оценка: " << rating << "/5" << endl;
    cout << "Использование: " << monthsOfUsage << " месяцев" << endl;
    cout << "Рекомендую: " << (recommendToOthers ? "Да" : "Нет") << endl;
    cout << "Фото: " << (hasPhotos ? "Есть" : "Нет") << endl;
    cout << "Проверен: " << (isVerified ? "Да" : "Нет") << endl;
    cout << "Вес отзыва: " << fixed << setprecision(2) << calculateWeight() << endl;
    cout << "Полезность: " << getHelpfulnessRatio() * 100 << "%" << endl;

    if (!pros.empty()) {
        cout << "\nДостоинства:" << endl;
        for (size_t i = 0; i < pros.size(); i++) {
            cout << "  " << (i + 1) << ". " << pros[i] << endl;
        }
    }

    if (!cons.empty()) {
        cout << "\nНедостатки:" << endl;
        for (size_t i = 0; i < cons.size(); i++) {
            cout << "  " << (i + 1) << ". " << cons[i] << endl;
        }
    }

    cout << "\nКомментарий: " << comment << endl;
    cout << "Дата: " << dateReview << endl;
    cout << "=================================" << endl;
}

// Проверить валидность отзыва
bool MusicProductReview::validate() const {
    // Проверяем основные критерии валидности
    if (productName.empty() || productArticle.empty()) {
        return false;
    }

    if (rating < 1 || rating > 5) {
        return false;
    }

    if (comment.length() < 10) {
        return false;  // Слишком короткий комментарий
    }

    if (hasPhotos) {
        return true;
    }

    // Если использовался более 1 месяца, отзыв валидный
    if (monthsOfUsage >= 1) {
        return true;
    }

    // Минимальная длина комментария для валидности
    return comment.length() >= 20;
}

// Реализация: Клонирование отзыва
Review* MusicProductReview::clone() const {
    // Создаем глубокую копию
    MusicProductReview* copy = new MusicProductReview(
        clientId, productName, productArticle, category, brand,
        rating, comment, hasPhotos, monthsOfUsage, recommendToOthers
    );

    // Копируем дополнительные поля
    copy->idReview = this->generateReviewId("CPY");
    copy->dateReview = this->getCurrentDate();      // Новая дата
    copy->isVerified = isVerified;
    copy->helpfulVotes = helpfulVotes;
    copy->unhelpfulVotes = unhelpfulVotes;

    // Копируем векторы
    copy->pros = pros;
    copy->cons = cons;

    return copy;
}

// Переопределение: Рассчитать вес отзыва для рейтинга
double MusicProductReview::calculateWeight() const {
    double weight = Review::calculateWeight();  // Базовый вес

    // Дополнительные факторы для музыкальных товаров

    // Длительное использование увеличивает вес
    if (monthsOfUsage >= 12) weight *= 1.5;
    else if (monthsOfUsage >= 6) weight *= 1.3;
    else if (monthsOfUsage >= 3) weight *= 1.2;

    // Отзывы с фото имеют больший вес
    if (hasPhotos) weight *= 1.4;

    // Детальные отзывы с плюсами/минусами имеют больший вес
    if (!pros.empty() && !cons.empty()) {
        weight *= 1.3;
    }
    else if (!pros.empty() || !cons.empty()) {
        weight *= 1.1;
    }

    // Отзывы на определенные категории могут иметь разный вес
    if (category == "Гитары" || category == "Клавишные" || category == "Ударные") {
        weight *= 1.2;  // Основные категории
    }

    return weight;
}

// Переопределение: Подготовить данные для записи в файл
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

// Переопределение: Экспортировать в строку
std::string MusicProductReview::exportToString() const {
    stringstream ss;
    ss << Review::exportToString() << "\n"
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

// Добавить достоинство
void MusicProductReview::addPro(const std::string& pro) {
    if (!pro.empty()) {
        pros.push_back(pro);
    }
}

// Добавить недостаток
void MusicProductReview::addCon(const std::string& con) {
    if (!con.empty()) {
        cons.push_back(con);
    }
}

// Оценить качество звука
void MusicProductReview::rateSoundQuality(int score) {
    if (score >= 1 && score <= 10) {
        addPro("Качество звука: " + to_string(score) + "/10");
    }
}

// Оценить качество сборки
void MusicProductReview::rateBuildQuality(int score) {
    if (score >= 1 && score <= 10) {
        addPro("Качество сборки: " + to_string(score) + "/10");
    }
}

// Получить общую оценку качества
double MusicProductReview::getOverallQuality() const {
    double total = rating * 2.0;  // Конвертируем 5-балльную в 10-балльную

    // Дополнительные бонусы за длительное использование
    if (monthsOfUsage > 0) {
        total += 1.0;
    }

    if (recommendToOthers) {
        total += 1.0;
    }

    if (hasPhotos) {
        total += 0.5;
    }

    // Ограничиваем максимум 10 баллами
    return min(total, 10.0);
}

// Вспомогательный метод: Преобразовать вектор в строку
std::string MusicProductReview::vectorToString(const std::vector<std::string>& vec) const {
    if (vec.empty()) return "";

    stringstream ss;
    for (size_t i = 0; i < vec.size(); i++) {
        ss << vec[i];
        if (i < vec.size() - 1) {
            ss << "; ";
        }
    }
    return ss.str();
}
