#include "Review.h"
#include <sstream>
#include <ctime>
#include <cstdlib>
#include <iomanip>

using namespace std;

Review::Review() : rating(0) {
    idReview = generateReviewId();
    dateReview = getCurrentDate();
}

Review::Review(const std::string& product, const std::string& client,
    int rating, const std::string& comment)
    : productName(product), clientId(client), rating(rating), comment(comment) {
    idReview = generateReviewId();
    dateReview = getCurrentDate();
}

void Review::SaveToFile() {
    ofstream file("reviews.txt", ios::app);
    if (file.is_open()) {
        file << idReview << "|" << productName << "|" << clientId << "|"
            << rating << "|" << comment << "|" << dateReview << endl;
        file.close();
        cout << "Отзыв сохранен в файл!" << endl;
    }
    else {
        cout << "Ошибка сохранения отзыва!" << endl;
    }
}

void Review::PublicReview() {
    cout << "\n=== ВСЕ ОТЗЫВЫ ИЗ ФАЙЛА ===" << endl;

    ifstream file("reviews.txt");
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
        string tokens[6];
        int tokenCount = 0;

        while (getline(ss, token, '|') && tokenCount < 6) {
            tokens[tokenCount++] = token;
        }

        if (tokenCount == 6) {
            reviewCount++;
            cout << "\n--- Отзыв #" << reviewCount << " ---" << endl;
            cout << "ID: " << tokens[0] << endl;
            cout << "Товар: " << tokens[1] << endl;
            cout << "Клиент: " << tokens[2] << endl;
            cout << "Оценка: " << tokens[3] << "/5" << endl;
            cout << "Комментарий: " << tokens[4] << endl;
            cout << "Дата: " << tokens[5] << endl;
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
        string tokens[6];
        int tokenCount = 0;

        while (getline(ss, token, '|') && tokenCount < 6) {
            tokens[tokenCount++] = token;
        }

        if (tokenCount == 6) {
            // Сравниваем названия товаров (без учета регистра)
            string fileProductName = tokens[1];
            if (fileProductName == productName) {
                try {
                    int productRating = stoi(tokens[3]);
                    totalRating += productRating;
                    reviewCount++;
                }
                catch (const exception& e) {
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
    cout << "Средняя оценка: " << fixed << setprecision(1) << average << " из 5" << endl;
    cout << "==========================" << endl;

    return average;
}

string Review::generateReviewId() {
    srand(static_cast<unsigned int>(time(0)));
    int randomNum = rand() % 10000;
    return "REV" + to_string(randomNum);
}

string Review::getCurrentDate() {
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