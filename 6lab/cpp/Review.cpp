#define _CRT_SECURE_NO_WARNINGS
#include "Review.h"
#include <sstream>
#include <ctime>
#include <cstdlib>
#include <iomanip>
#include <fstream>
#include <algorithm>
#include <random>

using namespace std;

// Конструктор с дополнительными параметрами для ID и даты
Review::Review(const std::string& clientId, int rating,
    const std::string& comment, bool verified,
    const std::string& id, const std::string& date)
    : clientId(clientId), comment(comment), isVerified(verified),
    helpfulVotes(0), unhelpfulVotes(0) {

    setRating(rating);


    if (id.empty()) {
        idReview = generateReviewId();
    }
    else {
        idReview = id;
    }


    if (date.empty()) {
        dateReview = getCurrentDate();
    }
    else {
        dateReview = date;
    }
}

// Виртуальный метод: Рассчитать вес отзыва для рейтинга
double Review::calculateWeight() const {
    double weight = 1.0;

    // Проверенные отзывы имеют больший вес
    if (isVerified) weight *= 1.5;

    // Отзывы с большим количеством текста имеют больший вес
    if (comment.length() > 100) weight *= 1.2;
    else if (comment.length() > 50) weight *= 1.1;

    // Отзывы с высоким рейтингом полезности имеют больший вес
    double helpfulRatio = getHelpfulnessRatio();
    if (helpfulRatio > 0.8) weight *= 1.3;
    else if (helpfulRatio > 0.5) weight *= 1.1;

    return weight;
}

// Виртуальный метод: Подготовить данные для записи в файл
std::string Review::prepareForFile() const {
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

// Виртуальный метод: Экспортировать в строку
std::string Review::exportToString() const {
    stringstream ss;
    ss << "Тип: " << getReviewType() << "\n"
        << "ID: " << idReview << "\n"
        << "Клиент: " << clientId << "\n"
        << "Оценка: " << rating << "/5\n"
        << "Дата: " << dateReview << "\n"
        << "Проверен: " << (isVerified ? "Да" : "Нет") << "\n"
        << "Полезно: " << helpfulVotes << "\n"
        << "Бесполезно: " << unhelpfulVotes << "\n"
        << "Комментарий: " << comment;
    return ss.str();
}

// Сохранить в файл
void Review::SaveToFile(const std::string& filename) const {
    ofstream file(filename, ios::app);
    if (file.is_open()) {
        file << prepareForFile() << endl;
        file.close();
        cout << "Отзыв сохранен в файл!" << endl;
    }
    else {
        cout << "Ошибка сохранения отзыва!" << endl;
    }
}

// Статический метод для отображения всех отзывов из файла
void Review::DisplayAllReviews(const std::string& filename) {
    cout << "\n=== ВСЕ ОТЗЫВЫ ИЗ ФАЙЛА ===" << endl;

    ifstream file(filename);
    if (!file.is_open()) {
        cout << "Файл отзывов не найден или пуст!" << endl;
        return;
    }

    string line;
    int reviewCount = 0;

    while (getline(file, line)) {
        if (line.empty()) continue;

        stringstream ss(line);
        string token;
        string tokens[9];
        int tokenCount = 0;

        while (getline(ss, token, '|') && tokenCount < 9) {
            tokens[tokenCount++] = token;
        }

        if (tokenCount >= 6) {
            reviewCount++;
            cout << "\n--- Отзыв #" << reviewCount << " ---" << endl;
            cout << "Тип: " << tokens[1] << endl;
            cout << "ID: " << tokens[0] << endl;
            cout << "Клиент: " << tokens[2] << endl;
            cout << "Оценка: " << tokens[3] << "/5" << endl;
            cout << "Комментарий: " << tokens[4] << endl;
            cout << "Дата: " << tokens[5] << endl;

            if (tokenCount > 6) {
                cout << "Проверен: " << (tokens[6] == "1" ? "Да" : "Нет") << endl;
            }
        }
    }

    file.close();

    if (reviewCount == 0) {
        cout << "Отзывов не найдено!" << endl;
    }
    else {
        cout << "\nВсего отзывов: " << reviewCount << endl;
    }
}

// Статический метод для расчета средней оценки
double Review::AverageRating(const std::string& productName) {
    ifstream file("reviews.txt");
    if (!file.is_open()) {
        cout << "Файл отзывов не найден!" << endl;
        return 0.0;
    }

    double totalRating = 0.0;
    int reviewCount = 0;
    string line;

    while (getline(file, line)) {
        if (line.empty()) continue;

        stringstream ss(line);
        string token;
        string tokens[9];
        int tokenCount = 0;

        while (getline(ss, token, '|') && tokenCount < 9) {
            tokens[tokenCount++] = token;
        }

        if (tokenCount >= 4) {
            // Проверяем, содержит ли комментарий название товара
            string comment = tokens[4];
            transform(comment.begin(), comment.end(), comment.begin(), ::tolower);

            string searchName = productName;
            transform(searchName.begin(), searchName.end(), searchName.begin(), ::tolower);

            if (comment.find(searchName) != string::npos) {
                try {
                    int productRating = stoi(tokens[3]);
                    totalRating += productRating;
                    reviewCount++;
                }
                catch (...) {
                    continue;
                }
            }
        }
    }

    file.close();

    if (reviewCount == 0) {
        cout << "Отзывов для товара '" << productName << "' не найдено!" << endl;
        return 0.0;
    }

    double average = totalRating / reviewCount;

    cout << "\n=== РЕЗУЛЬТАТЫ РАСЧЕТА ===" << endl;
    cout << "Товар: " << productName << endl;
    cout << "Количество отзывов: " << reviewCount << endl;
    cout << "Средняя оценка: " << fixed << setprecision(1)
        << average << " из 5" << endl;
    cout << "==========================" << endl;

    return average;
}

// Методы оценки полезности
void Review::markAsHelpful() {
    helpfulVotes++;
}

void Review::markAsUnhelpful() {
    unhelpfulVotes++;
}

double Review::getHelpfulnessRatio() const {
    int totalVotes = helpfulVotes + unhelpfulVotes;
    if (totalVotes == 0) return 0.0;
    return static_cast<double>(helpfulVotes) / totalVotes;
}

// Статический метод: Генерация ID отзыва
std::string Review::generateReviewId(const std::string& prefix) {
    static random_device rd;
    static mt19937 gen(rd());
    static uniform_int_distribution<> dis(1000, 9999);

    int randomNum = dis(gen);
    return prefix + to_string(randomNum);
}

// Статический метод: Получение текущей даты
std::string Review::getCurrentDate() {
    time_t now = time(0);

#ifdef _WIN32
    struct tm timeinfo;
    localtime_s(&timeinfo, &now);
    char buffer[80];
    strftime(buffer, 80, "%d.%m.%Y %H:%M:%S", &timeinfo);
#else
    tm* localTime = localtime(&now);
    char buffer[80];
    strftime(buffer, 80, "%d.%m.%Y %H:%M:%S", localTime);
#endif

    return string(buffer);
}

// Вспомогательный метод: Генерация случайного числа
int Review::generateRandomNumber() const {
    return rand() % 10000;
}
