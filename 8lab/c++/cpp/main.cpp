#define _CRT_SECURE_NO_WARNINGS
#include <iostream>
#include <iomanip>
#include <numeric>
#include <limits>
#include <locale>
#include <memory>
#include <vector>
#include <algorithm>
#include <fstream>
#include <sstream>
#include <functional>
#include "Product.h"
#include "Search.h"
#include "Basket.h"
#include "Order.h"
#include "ProviderOrder.h"
#include "SalesReport.h"
#include "Shop.h"
#include "Review.h"
#include "MusicProductReview.h"
#include "User.h"
#include "Admin.h"
#include "Client.h"

using namespace std;

// Глобальные переменные
Basket userBasket;
Shop musicShop("Музыкальный магазин 'MusicLoverzz'", "г. Барнаул, ул. Ленина, 1");
Client* currentClient = nullptr;
Admin* currentAdmin = nullptr;

// Прототипы функций
void addProducts();
void adminMode();
void userMode();
bool checkAdminPassword();
void userSearchMode();
void userBasketMode();
void userOrderMode();
void providerOrderMode();
void addProductToBasketByArticle(Basket& basket);
void shopManagementMode();
void loginMenu();
void registerNewClient();
void demonstrateReviewSorting();
void adminReviewManagement();

// Функция проверки пароля администратора
bool checkAdminPassword() {
    string password;
    cout << "Введите пароль администратора: ";
    cin >> password;
    if (password == "26102006") {
        cout << "Доступ разрешен!" << endl;
        return true;
    }
    else {
        cout << "Неверный пароль!" << endl;
        return false;
    }
}

// Меню входа/регистрации
void loginMenu() {
    int choice;
    do {
        cout << "\n=== СИСТЕМА АВТОРИЗАЦИИ ===" << endl;
        cout << "1. Войти как клиент" << endl;
        cout << "2. Войти как администратор" << endl;
        cout << "3. Зарегистрировать нового клиента" << endl;
        cout << "4. Продолжить без авторизации" << endl;
        cout << "0. Выход" << endl;
        cout << "Выберите действие: ";
        cin >> choice;

        switch (choice) {
        case 1: {
            string clientId;
            cout << "Введите ID клиента (или нажмите Enter для нового): ";
            cin.ignore();
            getline(cin, clientId);

            if (clientId.empty()) {
                cout << "\nКлиент не найден. Зарегистрируйтесь." << endl;
                registerNewClient();
            }
            else {
                string name;
                cout << "Введите ваше имя: ";
                getline(cin, name);

                if (currentClient) delete currentClient;
                currentClient = new Client(clientId, name, "", "", "");
                cout << "\nВход выполнен как клиент: " << name << " (ID: " << clientId << ")" << endl;
            }
            return;
        }
        case 2:
            if (checkAdminPassword()) {
                if (currentAdmin) delete currentAdmin;
                currentAdmin = new Admin("Главный Админ");
                cout << "\nВход выполнен как администратор" << endl;
            }
            return;
        case 3:
            registerNewClient();
            return;
        case 4:
            cout << "\nПродолжение без авторизации." << endl;
            cout << "Некоторые функции могут быть ограничены." << endl;
            return;
        case 0:
            cout << "Выход из программы..." << endl;
            exit(0);
        default:
            cout << "Неверный выбор!" << endl;
        }
    } while (choice != 0);
}

// Регистрация нового клиента
void registerNewClient() {
    cout << "\n=== РЕГИСТРАЦИЯ НОВОГО КЛИЕНТА ===" << endl;

    string name, email, phone, address;

    cin.ignore(numeric_limits<streamsize>::max(), '\n');

    cout << "Введите имя: ";
    getline(cin, name);

    cout << "Введите email: ";
    getline(cin, email);

    cout << "Введите телефон: ";
    getline(cin, phone);

    cout << "Введите адрес: ";
    getline(cin, address);

    if (currentClient) delete currentClient;
    currentClient = new Client(name, email, phone, address);

    cout << "\n=== РЕГИСТРАЦИЯ УСПЕШНА! ===" << endl;
    cout << "Ваш ID: " << currentClient->getId() << endl;
    cout << "Добро пожаловать, " << currentClient->getName() << "!" << endl;
}

void adminReviewManagement() {
    if (!currentAdmin) {
        cout << "\nСначала выполните вход как администратор!" << endl;
        return;
    }

    cout << "\n=== УПРАВЛЕНИЕ ОТЗЫВАМИ (АДМИНИСТРАТОР) ===" << endl;

    // Загружаем отзывы из файла как MusicProductReview
    vector<shared_ptr<MusicProductReview>> reviews;

    ifstream file("reviews.txt");
    if (!file.is_open()) {
        cout << "Файл с отзывами не найден!" << endl;
        return;
    }

    string line;
    while (getline(file, line)) {
        if (line.empty()) continue;

        stringstream ss(line);
        string token;
        vector<string> tokens;

        while (getline(ss, token, '|')) {
            tokens.push_back(token);
        }

        if (tokens.size() >= 9) {
            try {
                auto review = make_shared<MusicProductReview>(
                    tokens[2], tokens[3], tokens[4], tokens[5], tokens[6],
                    stoi(tokens[7]), tokens[8]
                );

                review->setIdReview(tokens[0]);
                review->setDateReview(tokens[9]);

                if (tokens.size() > 10 && tokens[10] == "1") {
                    review->verifyReview();
                }

                reviews.push_back(review);
            }
            catch (const exception& e) {
                cout << "Ошибка при чтении отзыва: " << e.what() << endl;
            }
        }
    }
    file.close();

    if (reviews.empty()) {
        cout << "Нет отзывов для управления." << endl;
        return;
    }

    int choice;
    do {
        cout << "\n=== МЕНЮ УПРАВЛЕНИЯ ОТЗЫВАМИ ===" << endl;
        cout << "Всего отзывов: " << reviews.size() << endl;
        cout << "1. Показать все отзывы" << endl;
        cout << "2. Сортировать отзывы" << endl;
        cout << "3. Найти отзыв с максимальным рейтингом" << endl;
        cout << "4. Найти отзыв с минимальным рейтингом" << endl;
        cout << "5. Фильтровать отзывы по рейтингу" << endl;
        cout << "6. Проверить, есть ли проверенные отзывы" << endl;
        cout << "7. Общая статистика отзывов" << endl;
        cout << "8. Показать отзывы в виде таблицы" << endl;
        cout << "0. Назад" << endl;
        cout << "Выберите действие: ";
        cin >> choice;

        switch (choice) {
        case 1: {
            cout << "\n=== ВСЕ ОТЗЫВЫ ===" << endl;
            for (size_t i = 0; i < reviews.size(); i++) {
                cout << "\n--- Отзыв " << (i + 1) << " ---" << endl;
                cout << reviews[i]->exportToString() << endl;
            }
            break;
        }
        case 2: {
            cout << "\n=== СОРТИРОВКА ОТЗЫВОВ ===" << endl;
            cout << "Выберите критерий сортировки:" << endl;
            cout << "1. По дате (новые сначала)" << endl;
            cout << "2. По дате (старые сначала)" << endl;
            cout << "3. По рейтингу (высокий сначала)" << endl;
            cout << "4. По рейтингу (низкий сначала)" << endl;
            cout << "5. По полезности" << endl;
            cout << "6. Проверенные сначала" << endl;
            cout << "Ваш выбор: ";

            int sortChoice;
            cin >> sortChoice;

            // Используем std::sort напрямую с лямбда-функциями
            vector<shared_ptr<MusicProductReview>> sortedReviews = reviews;

            switch (sortChoice) {
            case 1: // По дате (новые сначала)
                sort(sortedReviews.begin(), sortedReviews.end(),
                    [](const shared_ptr<MusicProductReview>& a, const shared_ptr<MusicProductReview>& b) {
                        return a->getDateReview() > b->getDateReview();
                    });
                break;
            case 2: // По дате (старые сначала)
                sort(sortedReviews.begin(), sortedReviews.end(),
                    [](const shared_ptr<MusicProductReview>& a, const shared_ptr<MusicProductReview>& b) {
                        return a->getDateReview() < b->getDateReview();
                    });
                break;
            case 3: // По рейтингу (высокий сначала)
                sort(sortedReviews.begin(), sortedReviews.end(),
                    [](const shared_ptr<MusicProductReview>& a, const shared_ptr<MusicProductReview>& b) {
                        return a->getRating() > b->getRating();
                    });
                break;
            case 4: // По рейтингу (низкий сначала)
                sort(sortedReviews.begin(), sortedReviews.end(),
                    [](const shared_ptr<MusicProductReview>& a, const shared_ptr<MusicProductReview>& b) {
                        return a->getRating() < b->getRating();
                    });
                break;
            case 5: // По полезности
                sort(sortedReviews.begin(), sortedReviews.end(),
                    [](const shared_ptr<MusicProductReview>& a, const shared_ptr<MusicProductReview>& b) {
                        return a->calculateWeight() > b->calculateWeight();
                    });
                break;
            case 6: // Проверенные сначала
                sort(sortedReviews.begin(), sortedReviews.end(),
                    [](const shared_ptr<MusicProductReview>& a, const shared_ptr<MusicProductReview>& b) {
                        if (a->getIsVerified() && !b->getIsVerified()) return true;
                        if (!a->getIsVerified() && b->getIsVerified()) return false;
                        return a->getDateReview() > b->getDateReview(); // при равенстве - по дате
                    });
                break;
            default:
                cout << "Неверный выбор!" << endl;
                continue;
            }

            cout << "\n=== ОТСОРТИРОВАННЫЕ ОТЗЫВЫ (первые 5) ===" << endl;
            for (size_t i = 0; i < min(sortedReviews.size(), size_t(5)); i++) {
                cout << "\n--- Отзыв " << (i + 1) << " ---" << endl;
                cout << "Рейтинг: " << sortedReviews[i]->getRating() << "/5" << endl;
                cout << "Дата: " << sortedReviews[i]->getDateReview() << endl;
                cout << "Товар: " << sortedReviews[i]->getSubject() << endl;
                cout << "Проверен: " << (sortedReviews[i]->getIsVerified() ? "Да" : "Нет") << endl;
            }
            break;
        }
        case 3: {
            if (reviews.empty()) {
                cout << "Нет отзывов!" << endl;
                break;
            }

            auto maxRatingReview = *max_element(reviews.begin(), reviews.end(),
                [](const shared_ptr<MusicProductReview>& a, const shared_ptr<MusicProductReview>& b) {
                    return a->getRating() < b->getRating();
                });

            cout << "\n=== ОТЗЫВ С МАКСИМАЛЬНЫМ РЕЙТИНГОМ ===" << endl;
            cout << "Рейтинг: " << maxRatingReview->getRating() << "/5" << endl;
            cout << "Товар: " << maxRatingReview->getSubject() << endl;
            cout << "Дата: " << maxRatingReview->getDateReview() << endl;
            cout << "Клиент: " << maxRatingReview->getClientId() << endl;
            break;
        }
        case 4: {
            if (reviews.empty()) {
                cout << "Нет отзывов!" << endl;
                break;
            }

            auto minRatingReview = *min_element(reviews.begin(), reviews.end(),
                [](const shared_ptr<MusicProductReview>& a, const shared_ptr<MusicProductReview>& b) {
                    return a->getRating() < b->getRating();
                });

            cout << "\n=== ОТЗЫВ С МИНИМАЛЬНЫМ РЕЙТИНГОМ ===" << endl;
            cout << "Рейтинг: " << minRatingReview->getRating() << "/5" << endl;
            cout << "Товар: " << minRatingReview->getSubject() << endl;
            cout << "Дата: " << minRatingReview->getDateReview() << endl;
            cout << "Клиент: " << minRatingReview->getClientId() << endl;
            break;
        }
        case 5: {
            cout << "\n=== ФИЛЬТРАЦИЯ ОТЗЫВОВ ПО РЕЙТИНГУ ===" << endl;
            cout << "Введите минимальный рейтинг (1-5): ";
            int minRating;
            cin >> minRating;

            if (minRating < 1 || minRating > 5) {
                cout << "Некорректный рейтинг!" << endl;
                break;
            }

            // Используем STL алгоритмы напрямую
            vector<shared_ptr<MusicProductReview>> filteredReviews;
            copy_if(reviews.begin(), reviews.end(), back_inserter(filteredReviews),
                [minRating](const shared_ptr<MusicProductReview>& r) {
                    return r->getRating() >= minRating;
                });

            cout << "\nНайдено отзывов с рейтингом >= " << minRating << ": "
                << filteredReviews.size() << endl;

            if (!filteredReviews.empty()) {
                for (size_t i = 0; i < min(filteredReviews.size(), size_t(5)); i++) {
                    cout << "\n--- Отзыв " << (i + 1) << " ---" << endl;
                    cout << "Рейтинг: " << filteredReviews[i]->getRating() << "/5" << endl;
                    cout << "Товар: " << filteredReviews[i]->getSubject() << endl;
                    cout << "Дата: " << filteredReviews[i]->getDateReview() << endl;
                }
            }
            break;
        }
        case 6: {
            bool hasVerified = any_of(reviews.begin(), reviews.end(),
                [](const shared_ptr<MusicProductReview>& r) { return r->getIsVerified(); });

            cout << "\n=== ПРОВЕРЕННЫЕ ОТЗЫВЫ ===" << endl;
            cout << "Есть проверенные отзывы: " << (hasVerified ? "Да" : "Нет") << endl;

            if (hasVerified) {
                int verifiedCount = count_if(reviews.begin(), reviews.end(),
                    [](const shared_ptr<MusicProductReview>& r) { return r->getIsVerified(); });
                cout << "Количество проверенных отзывов: " << verifiedCount << endl;
            }
            break;
        }
        case 7: {
            cout << "\n=== СТАТИСТИКА ОТЗЫВОВ ===" << endl;

            if (reviews.empty()) {
                cout << "Нет отзывов для статистики!" << endl;
                break;
            }

            cout << "Общее количество отзывов: " << reviews.size() << endl;

            // Используем STL алгоритмы напрямую
            vector<int> ratings;
            transform(reviews.begin(), reviews.end(), back_inserter(ratings),
                [](const shared_ptr<MusicProductReview>& r) { return r->getRating(); });

            double avgRating = accumulate(ratings.begin(), ratings.end(), 0.0) / ratings.size();
            int maxRating = *max_element(ratings.begin(), ratings.end());
            int minRating = *min_element(ratings.begin(), ratings.end());
            int verifiedCount = count_if(reviews.begin(), reviews.end(),
                [](const shared_ptr<MusicProductReview>& r) { return r->getIsVerified(); });

            cout << "Средний рейтинг: " << fixed << setprecision(2) << avgRating << "/5" << endl;
            cout << "Минимальный рейтинг: " << minRating << "/5" << endl;
            cout << "Максимальный рейтинг: " << maxRating << "/5" << endl;
            cout << "Количество проверенных отзывов: " << verifiedCount << endl;

            // Распределение по рейтингам
            vector<int> ratingCount(6, 0);
            for (const auto& review : reviews) {
                int rating = review->getRating();
                if (rating >= 0 && rating <= 5) {
                    ratingCount[rating]++;
                }
            }

            cout << "\nРаспределение по рейтингам:" << endl;
            for (int i = 5; i >= 1; i--) {
                cout << i << " звезд: " << ratingCount[i] << " ("
                    << (ratingCount[i] * 100.0 / reviews.size()) << "%)" << endl;
            }
            break;
        }
        case 8: {
            cout << "\n=== ТАБЛИЦА ОТЗЫВОВ ===" << endl;
            cout << left << setw(5) << "№"
                << setw(20) << "Товар"
                << setw(8) << "Рейтинг"
                << setw(15) << "Дата"
                << setw(10) << "Проверен"
                << setw(10) << "Вес" << endl;
            cout << string(68, '-') << endl;

            for (size_t i = 0; i < reviews.size(); i++) {
                string productName = reviews[i]->getSubject();
                if (productName.length() > 18) {
                    productName = productName.substr(0, 15) + "...";
                }

                cout << left << setw(5) << (i + 1)
                    << setw(20) << productName
                    << setw(8) << (to_string(reviews[i]->getRating()) + "/5")
                    << setw(15) << reviews[i]->getDateReview()
                    << setw(10) << (reviews[i]->getIsVerified() ? "Да" : "Нет")
                    << setw(10) << fixed << setprecision(2) << reviews[i]->calculateWeight()
                    << endl;
            }
            break;
        }
        case 0:
            cout << "Выход из управления отзывами..." << endl;
            break;
        default:
            cout << "Неверный выбор!" << endl;
        }
    } while (choice != 0);
}

// Демонстрация сортировки отзывов
void demonstrateReviewSorting() {
    cout << "\n=== ДЕМОНСТРАЦИЯ АЛГОРИТМОВ СОРТИРОВКИ ОТЗЫВОВ ===" << endl;

    // Загружаем отзывы из файла
    vector<shared_ptr<MusicProductReview>> reviews;

    ifstream file("reviews.txt");
    if (!file.is_open()) {
        cout << "Файл 'reviews.txt' не найден!" << endl;
        cout << "Сначала создайте отзывы через меню пользователя." << endl;
        return;
    }

    string line;
    while (getline(file, line)) {
        if (line.empty()) continue;

        stringstream ss(line);
        string token;
        vector<string> tokens;

        while (getline(ss, token, '|')) {
            tokens.push_back(token);
        }

        if (tokens.size() >= 9) {
            try {
                auto review = make_shared<MusicProductReview>(
                    tokens[2], tokens[3], tokens[4], tokens[5], tokens[6],
                    stoi(tokens[7]), tokens[8]
                );
                review->setIdReview(tokens[0]);
                review->setDateReview(tokens[9]);
                if (tokens.size() > 10 && tokens[10] == "1") {
                    review->verifyReview();
                }
                reviews.push_back(review);
            }
            catch (...) {
                continue;
            }
        }
    }
    file.close();

    if (reviews.empty()) {
        cout << "Нет отзывов для демонстрации!" << endl;
        return;
    }

    cout << "Всего отзывов загружено: " << reviews.size() << endl;

    // 1. Демонстрация сортировки по рейтингу с помощью STL
    cout << "\n1. СОРТИРОВКА ПО РЕЙТИНГУ (высокий сначала) с помощью std::sort:" << endl;
    vector<shared_ptr<MusicProductReview>> sortedByRating = reviews;
    sort(sortedByRating.begin(), sortedByRating.end(),
        [](const shared_ptr<MusicProductReview>& a, const shared_ptr<MusicProductReview>& b) {
            return a->getRating() > b->getRating();
        });

    for (size_t i = 0; i < min(sortedByRating.size(), size_t(5)); i++) {
        cout << "  " << (i + 1) << ". " << sortedByRating[i]->getSubject()
            << " - " << sortedByRating[i]->getRating() << "/5" << endl;
    }

    // 2. Демонстрация сортировки по дате с помощью STL
    cout << "\n2. СОРТИРОВКА ПО ДАТЕ (новые сначала) с помощью std::sort:" << endl;
    vector<shared_ptr<MusicProductReview>> sortedByDate = reviews;
    sort(sortedByDate.begin(), sortedByDate.end(),
        [](const shared_ptr<MusicProductReview>& a, const shared_ptr<MusicProductReview>& b) {
            return a->getDateReview() > b->getDateReview();
        });

    for (size_t i = 0; i < min(sortedByDate.size(), size_t(5)); i++) {
        cout << "  " << (i + 1) << ". " << sortedByDate[i]->getDateReview()
            << " - " << sortedByDate[i]->getSubject() << endl;
    }

    // 3. Демонстрация пользовательской сортировки по весу отзыва
    cout << "\n3. ПОЛЬЗОВАТЕЛЬСКАЯ СОРТИРОВКА (по весу отзыва):" << endl;
    vector<shared_ptr<MusicProductReview>> sortedByWeight = reviews;
    sort(sortedByWeight.begin(), sortedByWeight.end(),
        [](const shared_ptr<MusicProductReview>& a, const shared_ptr<MusicProductReview>& b) {
            return a->calculateWeight() > b->calculateWeight();
        });

    for (size_t i = 0; i < min(sortedByWeight.size(), size_t(5)); i++) {
        cout << "  " << (i + 1) << ". Вес: " << fixed << setprecision(2)
            << sortedByWeight[i]->calculateWeight()
            << " - " << sortedByWeight[i]->getSubject() << endl;
    }

    // 4. Демонстрация фильтрации с помощью STL
    cout << "\n4. ФИЛЬТРАЦИЯ ОТЗЫВОВ (рейтинг >= 4) с помощью std::copy_if:" << endl;
    vector<shared_ptr<MusicProductReview>> highRatedReviews;
    copy_if(reviews.begin(), reviews.end(), back_inserter(highRatedReviews),
        [](const shared_ptr<MusicProductReview>& r) { return r->getRating() >= 4; });

    cout << "   Найдено высокооцененных отзывов: " << highRatedReviews.size() << endl;

    // 5. Демонстрация поиска с помощью STL
    cout << "\n5. ПОИСК ОТЗЫВОВ (рейтинг = 5) с помощью std::find_if:" << endl;
    auto it = find_if(reviews.begin(), reviews.end(),
        [](const shared_ptr<MusicProductReview>& r) { return r->getRating() == 5; });

    if (it != reviews.end()) {
        cout << "   Найден отзыв с рейтингом 5: " << (*it)->getSubject() << endl;
    }
    else {
        cout << "   Отзывов с рейтингом 5 не найдено" << endl;
    }

    // 6. Демонстрация статистики
    cout << "\n6. СТАТИСТИКА ОТЗЫВОВ с помощью STL алгоритмов:" << endl;
    cout << "   Всего отзывов: " << reviews.size() << endl;

    if (!reviews.empty()) {
        vector<int> ratings;
        transform(reviews.begin(), reviews.end(), back_inserter(ratings),
            [](const shared_ptr<MusicProductReview>& r) { return r->getRating(); });

        double avgRating = accumulate(ratings.begin(), ratings.end(), 0.0) / ratings.size();
        int maxRating = *max_element(ratings.begin(), ratings.end());
        int minRating = *min_element(ratings.begin(), ratings.end());
        int verifiedCount = count_if(reviews.begin(), reviews.end(),
            [](const shared_ptr<MusicProductReview>& r) { return r->getIsVerified(); });

        cout << "   Средний рейтинг: " << fixed << setprecision(2) << avgRating << "/5" << endl;
        cout << "   Проверенных отзывов: " << verifiedCount << endl;
    }

    // 7. Демонстрация группировки по рейтингу
    cout << "\n7. ГРУППИРОВКА ОТЗЫВОВ ПО РЕЙТИНГУ:" << endl;
    vector<vector<shared_ptr<MusicProductReview>>> groups(6);
    for (const auto& review : reviews) {
        int rating = review->getRating();
        if (rating >= 0 && rating <= 5) {
            groups[rating].push_back(review);
        }
    }

    for (int i = 5; i >= 1; i--) {
        if (!groups[i].empty()) {
            cout << "   Рейтинг " << i << ": " << groups[i].size() << " отзывов" << endl;
        }
    }

    cout << "\n=== ДЕМОНСТРАЦИЯ ЗАВЕРШЕНА ===" << endl;
}

// Режим администратора
void adminMode() {
    if (!currentAdmin) {
        cout << "\nСначала выполните вход как администратор!" << endl;
        return;
    }

    if (!checkAdminPassword()) {
        return;
    }

    int choice;
    do {
        cout << "\n=== РЕЖИМ АДМИНИСТРАТОРА ===" << endl;
        cout << "1. Добавить товар" << endl;
        cout << "2. Показать информацию о товарах" << endl;
        cout << "3. Удалить товары со склада" << endl;
        cout << "4. Редактировать товары" << endl;
        cout << "5. Найти товары по артикулу" << endl;
        cout << "6. Заказ у поставщика" << endl;
        cout << "7. Управление отзывами" << endl;
        cout << "8. Демонстрация сортировки отзывов" << endl;
        cout << "9. Показать информацию администратора" << endl;
        cout << "10. Управление правами доступа" << endl;
        cout << "0. Выход в главное меню" << endl;
        cout << "Выберите действие: ";
        cin >> choice;

        switch (choice) {
        case 1:
            addProducts();
            break;
        case 2:
            Product::DisplayProductsTable("products.txt");
            break;
        case 3:
            Product::DeleteProductsFromFile("products.txt");
            break;
        case 4:
            Product::EditProductsInFile("products.txt");
            break;
        case 5:
            Product::FindProductsByArticle("products.txt");
            break;
        case 6:
            providerOrderMode();
            break;
        case 7:
            adminReviewManagement();
            break;
        case 8:
            demonstrateReviewSorting();
            break;
        case 9:
            if (currentAdmin) {
                currentAdmin->displayInfo();
                currentAdmin->showPermissions();
            }
            break;
        case 10:
            if (currentAdmin) {
                int permChoice;
                cout << "\n=== УПРАВЛЕНИЕ ПРАВАМИ ДОСТУПА ===" << endl;
                cout << "1. Добавить право доступа" << endl;
                cout << "2. Удалить право доступа" << endl;
                cout << "3. Показать все права" << endl;
                cout << "Ваш выбор: ";
                cin >> permChoice;
                cin.ignore();

                if (permChoice == 1) {
                    string perm;
                    cout << "Введите право доступа: ";
                    getline(cin, perm);
                    currentAdmin->addPermission(perm);
                }
                else if (permChoice == 2) {
                    string perm;
                    cout << "Введите право доступа для удаления: ";
                    getline(cin, perm);
                    currentAdmin->removePermission(perm);
                }
                else if (permChoice == 3) {
                    currentAdmin->showPermissions();
                }
            }
            break;
        case 0:
            cout << "Выход из режима администратора..." << endl;
            break;
        default:
            cout << "Неверный выбор!" << endl;
        }
    } while (choice != 0);
}

// Режим пользователя
void userMode() {
    if (!currentClient) {
        cout << "\nСначала выполните вход как клиент!" << endl;
        return;
    }

    int choice;
    do {
        cout << "\n=== РЕЖИМ ПОЛЬЗОВАТЕЛЯ ===" << endl;
        cout << "1. Поиск товаров" << endl;
        cout << "2. Корзина товаров" << endl;
        cout << "3. Оформить заказ" << endl;
        cout << "4. Оставить отзыв на музыкальный товар" << endl;
        cout << "5. Показать информацию клиента" << endl;
        cout << "6. Рассчитать скидку" << endl;
        cout << "0. Выход в главное меню" << endl;
        cout << "Выберите действие: ";
        cin >> choice;

        switch (choice) {
        case 1:
            userSearchMode();
            break;
        case 2:
            userBasketMode();
            break;
        case 3:
            userOrderMode();
            break;
        case 4:
            if (currentClient) {
                cout << "\n=== ОСТАВИТЬ ОТЗЫВ НА МУЗЫКАЛЬНЫЙ ТОВАР ===" << endl;

                string productName, productArticle, category, brand, comment;
                int rating, monthsOfUsage;
                char hasPhotos, recommend;

                cin.ignore();
                cout << "Введите название товара: ";
                getline(cin, productName);

                cout << "Введите артикул товара: ";
                getline(cin, productArticle);

                cout << "Введите категорию (Гитары, Клавишные, Ударные и т.д.): ";
                getline(cin, category);

                cout << "Введите бренд: ";
                getline(cin, brand);

                cout << "Введите оценку (1-5): ";
                cin >> rating;
                cin.ignore();

                cout << "Введите комментарий: ";
                getline(cin, comment);

                cout << "Есть ли фото? (y/n): ";
                cin >> hasPhotos;
                cin.ignore();

                cout << "Сколько месяцев используете товар? ";
                cin >> monthsOfUsage;
                cin.ignore();

                cout << "Рекомендуете товар? (y/n): ";
                cin >> recommend;
                cin.ignore();

                auto musicReview = make_shared<MusicProductReview>(
                    currentClient->getId(),
                    productName,
                    productArticle,
                    category,
                    brand,
                    rating,
                    comment,
                    (hasPhotos == 'y' || hasPhotos == 'Y'),
                    monthsOfUsage,
                    (recommend == 'y' || recommend == 'Y')
                );

                cout << "\n=== ДОБАВЛЕНИЕ ДОСТОИНСТВ ===" << endl;
                cout << "Введите достоинства товара (по одному, пустая строка - завершить):" << endl;
                string pro;
                while (true) {
                    cout << "Достоинство: ";
                    getline(cin, pro);
                    if (pro.empty()) break;
                    musicReview->addPro(pro);
                }

                cout << "\n=== ДОБАВЛЕНИЕ НЕДОСТАТКОВ ===" << endl;
                cout << "Введите недостатки товара (по одному, пустая строка - завершить):" << endl;
                string con;
                while (true) {
                    cout << "Недостаток: ";
                    getline(cin, con);
                    if (con.empty()) break;
                    musicReview->addCon(con);
                }

                // Оценка звука и сборки
                char rateChoice;
                cout << "\nХотите оценить качество звука? (y/n): ";
                cin >> rateChoice;
                if (rateChoice == 'y' || rateChoice == 'Y') {
                    int soundScore;
                    cout << "Оценка качества звука (1-10): ";
                    cin >> soundScore;
                    musicReview->rateSoundQuality(soundScore);
                }

                cout << "Хотите оценить качество сборки? (y/n): ";
                cin >> rateChoice;
                if (rateChoice == 'y' || rateChoice == 'Y') {
                    int buildScore;
                    cout << "Оценка качества сборки (1-10): ";
                    cin >> buildScore;
                    musicReview->rateBuildQuality(buildScore);
                }

                musicReview->SaveToFile();

                cout << "\n=== ИНФОРМАЦИЯ О СОЗДАННОМ ОТЗЫВЕ ===" << endl;
                cout << musicReview->exportToString() << endl;

                cout << "\nОтзыв успешно сохранен!" << endl;
            }
            break;
        case 5:
            if (currentClient) {
                currentClient->displayInfo();
                cout << "Ваша скидка: " << currentClient->calculateDiscount() << "%" << endl;
            }
            break;
        case 6:
            if (currentClient) {
                cout << "Ваша скидка составляет: " << currentClient->calculateDiscount() << "%" << endl;
            }
            break;
        case 0:
            cout << "Выход из режима пользователя..." << endl;
            break;
        default:
            cout << "Неверный выбор!" << endl;
        }
    } while (choice != 0);
}

// Управление магазином
void shopManagementMode() {
    if (!checkAdminPassword()) {
        return;
    }

    int choice;
    do {
        cout << "\n=== УПРАВЛЕНИЕ МАГАЗИНОМ ===" << endl;
        cout << "1. Информация о магазине" << endl;
        cout << "2. Показать каталог товаров" << endl;
        cout << "3. Добавить товар в каталог" << endl;
        cout << "4. Удалить товар из каталога" << endl;
        cout << "5. Показать активные заказы" << endl;
        cout << "6. Показать заказы поставщикам" << endl;
        cout << "7. Создать отчет по продажам" << endl;
        cout << "8. Изменить отчет по продажам" << endl;
        cout << "9. Показать отчеты по продажам" << endl;
        cout << "10. Показать все отзывы" << endl;
        cout << "11. Расчет средней оценки товара" << endl;
        cout << "12. Анализ отзывов" << endl;
        cout << "0. Назад" << endl;
        cout << "Выберите действие: ";
        cin >> choice;

        switch (choice) {
        case 1:
            musicShop.ShowShopInfo();
            break;
        case 2:
            musicShop.DisplayCatalog();
            break;
        case 3:
            musicShop.AddProduct();
            break;
        case 4:
            musicShop.DeleteProduct();
            break;
        case 5:
            musicShop.DisplayOrders();
            break;
        case 6:
            musicShop.DisplayProviderOrders();
            break;
        case 7:
            musicShop.CreateSalesReport();
            break;
        case 8:
            musicShop.RemakeSalesReport();
            break;
        case 9:
            musicShop.DisplaySalesReports();
            break;
        case 10:
            Review<MusicProductReview>::DisplayAllReviews();
            break;
        case 11: {
            string productName;
            cout << "\n=== РАСЧЕТ СРЕДНЕЙ ОЦЕНКИ ТОВАРА ===" << endl;
            cout << "Введите название товара: ";
            cin.ignore();
            getline(cin, productName);
            double avgRating = Review<MusicProductReview>::AverageRating(productName);
            cout << "Средняя оценка товара '" << productName << "': "
                << fixed << setprecision(2) << avgRating << "/5" << endl;
            break;
        }
        case 12: {
            // Анализ отзывов с использованием STL алгоритмов
            cout << "\n=== АНАЛИЗ ОТЗЫВОВ ===" << endl;

            vector<shared_ptr<MusicProductReview>> allReviews;

            // Чтение отзывов из файла
            ifstream file("reviews.txt");
            if (!file.is_open()) {
                cout << "Файл с отзывами не найден!" << endl;
                break;
            }

            string line;
            while (getline(file, line)) {
                if (line.empty()) continue;

                stringstream ss(line);
                string token;
                vector<string> tokens;

                while (getline(ss, token, '|')) {
                    tokens.push_back(token);
                }

                if (tokens.size() >= 9) {
                    try {
                        auto review = make_shared<MusicProductReview>(
                            tokens[2], tokens[3], tokens[4], tokens[5], tokens[6],
                            stoi(tokens[7]), tokens[8]);
                        review->setIdReview(tokens[0]);
                        review->setDateReview(tokens[9]);
                        allReviews.push_back(review);
                    }
                    catch (...) {
                        continue;
                    }
                }
            }
            file.close();

            if (allReviews.empty()) {
                cout << "Нет отзывов для анализа." << endl;
                break;
            }

            cout << "Всего отзывов: " << allReviews.size() << endl;

            // Используем std::accumulate для подсчета общего рейтинга
            int totalRating = accumulate(allReviews.begin(), allReviews.end(), 0,
                [](int sum, const shared_ptr<MusicProductReview>& r) {
                    return sum + r->getRating();
                });

            cout << "Средний рейтинг: " << (double)totalRating / allReviews.size() << "/5" << endl;

            // Используем std::count_if для подсчета по категориям
            int fiveStar = count_if(allReviews.begin(), allReviews.end(),
                [](const shared_ptr<MusicProductReview>& r) { return r->getRating() == 5; });
            int oneStar = count_if(allReviews.begin(), allReviews.end(),
                [](const shared_ptr<MusicProductReview>& r) { return r->getRating() == 1; });

            cout << "5-звездочных отзывов: " << fiveStar << endl;
            cout << "1-звездочных отзывов: " << oneStar << endl;

            // Используем std::partition для разделения отзывов на положительные и отрицательные
            auto middle = partition(allReviews.begin(), allReviews.end(),
                [](const shared_ptr<MusicProductReview>& r) { return r->getRating() >= 3; });

            int positiveCount = distance(allReviews.begin(), middle);
            int negativeCount = distance(middle, allReviews.end());

            cout << "Положительных отзывов (>=3): " << positiveCount << endl;
            cout << "Отрицательных отзывов (<3): " << negativeCount << endl;

            // Используем std::for_each для вывода топ-3 товаров
            cout << "\n=== ТОП-3 ТОВАРА ПО РЕЙТИНГУ ===" << endl;

            // Сортируем по рейтингу с помощью std::sort
            vector<shared_ptr<MusicProductReview>> sortedReviews = allReviews;
            sort(sortedReviews.begin(), sortedReviews.end(),
                [](const shared_ptr<MusicProductReview>& a, const shared_ptr<MusicProductReview>& b) {
                    return a->getRating() > b->getRating();
                });

            // Берем первые 3
            int limit = min(3, (int)sortedReviews.size());
            for_each(sortedReviews.begin(), sortedReviews.begin() + limit,
                [](const shared_ptr<MusicProductReview>& r) {
                    cout << "- " << r->getSubject() << ": " << r->getRating() << "/5" << endl;
                });

            // Анализ по категориям
            cout << "\n=== АНАЛИЗ ПО КАТЕГОРИЯМ ===" << endl;

            // Собираем уникальные категории вручную
            vector<string> categories;
            for (const auto& review : allReviews) {
                string category = review->getCategory();
                if (find(categories.begin(), categories.end(), category) == categories.end()) {
                    categories.push_back(category);
                }
            }

            cout << "Всего категорий: " << categories.size() << endl;

            for (const auto& category : categories) {
                // Фильтруем отзывы по категории
                vector<shared_ptr<MusicProductReview>> categoryReviews;
                copy_if(allReviews.begin(), allReviews.end(), back_inserter(categoryReviews),
                    [&category](const shared_ptr<MusicProductReview>& r) {
                        return r->getCategory() == category;
                    });

                if (!categoryReviews.empty()) {
                    // Вычисляем средний рейтинг
                    double avg = 0.0;
                    for (const auto& review : categoryReviews) {
                        avg += review->getRating();
                    }
                    avg /= categoryReviews.size();

                    cout << "Категория '" << category << "': " << categoryReviews.size()
                        << " отзывов, средний рейтинг: " << fixed << setprecision(2) << avg << "/5" << endl;
                }
            }

            break;
        }
        case 0:
            cout << "Выход из управления магазином..." << endl;
            break;
        default:
            cout << "Неверный выбор!" << endl;
        }
    } while (choice != 0);
}

void addProducts() {
    cout << "\n--- ДОБАВЛЕНИЕ ТОВАРОВ ---" << endl;
    int productCount;
    while (true) {
        cout << "Введите количество товаров для добавления: ";
        cin >> productCount;
        if (cin.fail()) {
            cin.clear();
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
            cout << "Ошибка! Введите целое число." << endl;
            continue;
        }
        if (productCount < 0) {
            cout << "Ошибка! Количество не может быть отрицательным. Попробуйте снова." << endl;
            continue;
        }
        if (productCount == 0) {
            cout << "Добавление товаров отменено." << endl;
            return;
        }
        if (!Product::validateNumber(productCount, 10)) {
            cout << "Ошибка! Количество товаров не должно превышать 10 цифр." << endl;
            continue;
        }
        break;
    }

    cin.ignore(numeric_limits<streamsize>::max(), '\n');

    for (int i = 0; i < productCount; i++) {
        cout << "\n=== Товар " << (i + 1) << " из " << productCount << " ===" << endl;
        Product product;
        product.InputFromKeyboard();
        cout << "\nТовар успешно добавлен!" << endl;
        product.DisplayInfo();
        product.WriteToFile("products.txt");
        cout << "---------------------------------------------" << endl;
    }
    cout << "\nУспешно добавлено " << productCount << " товаров!" << endl;
}

void providerOrderMode() {
    int choice;
    ProviderOrder providerOrder;
    do {
        cout << "\n=== ЗАКАЗ У ПОСТАВЩИКА ===" << endl;
        cout << "1. Добавить товар в заказ (из заказов клиентов)" << endl;
        cout << "2. Отправить заказ поставщику" << endl;
        cout << "3. Обновить информацию о товарах" << endl;
        cout << "0. Назад" << endl;
        cout << "Выберите действие: ";
        cin >> choice;

        switch (choice) {
        case 1:
            providerOrder.AddProvider();
            break;
        case 2:
            providerOrder.sending();
            break;
        case 3:
            providerOrder.NewInfoStock();
            break;
        case 0:
            cout << "Выход из режима заказа поставщика..." << endl;
            break;
        default:
            cout << "Неверный выбор!" << endl;
            cin.clear();
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
        }
    } while (choice != 0);
}

void userSearchMode() {
    int choice;
    Search search;
    do {
        cout << "\n=== РЕЖИМ ПОИСКА ===" << endl;
        cout << "1. Поиск по названию" << endl;
        cout << "2. Поиск по артикулу" << endl;
        cout << "3. Поиск по бренду" << endl;
        cout << "4. Поиск по стране-производителю" << endl;
        cout << "5. Поиск по категории" << endl;
        cout << "6. Поиск по типу товара" << endl;
        cout << "7. Поиск по цене" << endl;
        cout << "8. Универсальный поиск" << endl;
        cout << "9. Показать историю поиска" << endl;
        cout << "10. Очистить историю поиска" << endl;
        cout << "0. Назад" << endl;
        cout << "Выберите действие: ";
        cin >> choice;

        if (cin.fail()) {
            cin.clear();
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
            cout << "Неверный ввод! Пожалуйста, введите число от 0 до 10." << endl;
            continue;
        }

        cin.ignore(numeric_limits<streamsize>::max(), '\n');

        string searchString;
        int searchArticle, minPrice, maxPrice;

        switch (choice) {
        case 1:
            cout << "Введите название для поиска: ";
            getline(cin, searchString);
            search.SearchByName("products.txt", searchString);
            search.DisplayResults();
            break;
        case 2:
            while (true) {
                cout << "Введите артикул для поиска: ";
                cin >> searchArticle;
                if (cin.fail()) {
                    cin.clear();
                    cin.ignore(numeric_limits<streamsize>::max(), '\n');
                    cout << "Неверный ввод! Пожалуйста, введите целое число." << endl;
                    continue;
                }
                break;
            }
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
            search.SearchByArticle("products.txt", searchArticle);
            search.DisplayResults();
            break;
        case 3:
            cout << "Введите бренд для поиска: ";
            getline(cin, searchString);
            search.SearchByBrand("products.txt", searchString);
            search.DisplayResults();
            break;
        case 4:
            cout << "Введите страну-производитель для поиска: ";
            getline(cin, searchString);
            search.SearchByCountry("products.txt", searchString);
            search.DisplayResults();
            break;
        case 5:
            cout << "Введите категорию для поиска: ";
            getline(cin, searchString);
            search.SearchByCategory("products.txt", searchString);
            search.DisplayResults();
            break;
        case 6:
            cout << "Введите тип товара для поиска: ";
            getline(cin, searchString);
            search.SearchByType("products.txt", searchString);
            search.DisplayResults();
            break;
        case 7:
            while (true) {
                cout << "Введите минимальную цену: ";
                cin >> minPrice;
                if (cin.fail()) {
                    cin.clear();
                    cin.ignore(numeric_limits<streamsize>::max(), '\n');
                    cout << "Неверный ввод! Пожалуйста, введите целое число." << endl;
                    continue;
                }
                break;
            }
            while (true) {
                cout << "Введите максимальную цену: ";
                cin >> maxPrice;
                if (cin.fail()) {
                    cin.clear();
                    cin.ignore(numeric_limits<streamsize>::max(), '\n');
                    cout << "Неверный ввод! Пожалуйста, введите целое число." << endl;
                    continue;
                }
                break;
            }
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
            search.SearchByPrice("products.txt", minPrice, maxPrice);
            search.DisplayResults();
            break;
        case 8:
            search.UniversalSearch("products.txt");
            break;
        case 9:
            search.DisplaySearchHistory();
            break;
        case 10:
            //search.clearSearchHistory();
            break;
        case 0:
            cout << "Выход из режима поиска..." << endl;
            break;
        default:
            cout << "Неверный выбор!" << endl;
        }
    } while (choice != 0);
}

void userBasketMode() {
    int choice;
    do {
        cout << "\n=== КОРЗИНА ТОВАРОВ ===" << endl;
        cout << "1. Посмотреть цену товаров в корзине" << endl;
        cout << "2. Количество товаров в корзине" << endl;
        cout << "3. Показать содержимое корзины" << endl;
        cout << "4. Добавить товар в корзину по артикулу" << endl;
        cout << "5. Удалить товар из корзины" << endl;
        cout << "6. Изменить количество товара в корзине" << endl;
        cout << "7. Очистить корзину" << endl;
        cout << "8. Применить алгоритмы сортировки к корзине" << endl;
        cout << "0. Назад" << endl;
        cout << "Выберите действие: ";
        cin >> choice;

        switch (choice) {
        case 1: {
            double totalCost = userBasket.GetBasketCost();
            cout << "\n=== ОБЩАЯ СТОИМОСТЬ КОРЗИНЫ ===" << endl;
            cout << "Общая стоимость: " << totalCost << " руб." << endl;

            // Дополнительно показываем статистику с использованием STL
            vector<double> prices;
            for (int i = 0; i < userBasket.getProductCount(); i++) {
                Product* product = userBasket.getProduct(i);
                if (product) {
                    prices.push_back(product->getPrice() * userBasket.getQuantity(i));
                }
            }

            if (!prices.empty()) {
                auto minmax = minmax_element(prices.begin(), prices.end());
                cout << "Самый дорогой товар: " << *minmax.second << " руб." << endl;
                cout << "Самый дешевый товар: " << *minmax.first << " руб." << endl;
                double avg = accumulate(prices.begin(), prices.end(), 0.0) / prices.size();
                cout << "Средняя стоимость товара: " << avg << " руб." << endl;
            }

            cout << "===============================" << endl;
            break;
        }
        case 2: {
            unsigned int totalCount = userBasket.GetBasketCount();
            cout << "\n=== КОЛИЧЕСТВО ТОВАРОВ В КОРЗИНЕ ===" << endl;
            cout << "Общее количество товаров: " << totalCount << " шт." << endl;
            cout << "====================================" << endl;
            break;
        }
        case 3:
            userBasket.DisplayBasket();
            break;
        case 4:
            addProductToBasketByArticle(userBasket);
            break;
        case 5: {
            if (userBasket.IsEmpty()) {
                cout << "Корзина пуста!" << endl;
                break;
            }
            userBasket.DisplayBasket();
            int productIndex;
            cout << "Введите номер товара для удаления: ";
            cin >> productIndex;
            userBasket.DeleteBasket(productIndex);
            break;
        }
        case 6: {
            if (userBasket.IsEmpty()) {
                cout << "Корзина пуста!" << endl;
                break;
            }
            userBasket.DisplayBasket();
            int productIndex, newQuantity;
            cout << "Введите номер товара для изменения: ";
            cin >> productIndex;
            cout << "Введите новое количество: ";
            cin >> newQuantity;
            userBasket.UpdateQuantity(productIndex, newQuantity);
            break;
        }
        case 7:
            userBasket.ClearBasket();
            cout << "Корзина очищена!" << endl;
            break;
        case 8: {
            // Демонстрация STL алгоритмов с корзиной
            cout << "\n=== АЛГОРИТМЫ STL ДЛЯ КОРЗИНЫ ===" << endl;

            // Создаем вектор товаров из корзины
            vector<pair<Product*, int>> basketItems;
            for (int i = 0; i < userBasket.getProductCount(); i++) {
                Product* product = userBasket.getProduct(i);
                int quantity = userBasket.getQuantity(i);
                if (product) {
                    basketItems.push_back({ product, quantity });
                }
            }

            if (basketItems.empty()) {
                cout << "Корзина пуста!" << endl;
                break;
            }

            cout << "1. Сортировка товаров по цене (по возрастанию)" << endl;
            sort(basketItems.begin(), basketItems.end(),
                [](const pair<Product*, int>& a, const pair<Product*, int>& b) {
                    return a.first->getPrice() < b.first->getPrice();
                });

            cout << "Отсортированные товары:" << endl;
            for (const auto& item : basketItems) {
                cout << "- " << item.first->getName() << ": "
                    << item.first->getPrice() << " руб. (x" << item.second << ")" << endl;
            }

            cout << "\n2. Поиск самого дорогого товара" << endl;
            auto maxItem = max_element(basketItems.begin(), basketItems.end(),
                [](const pair<Product*, int>& a, const pair<Product*, int>& b) {
                    return a.first->getPrice() < b.first->getPrice();
                });

            if (maxItem != basketItems.end()) {
                cout << "Самый дорогой товар: " << maxItem->first->getName()
                    << " - " << maxItem->first->getPrice() << " руб." << endl;
            }

            cout << "\n3. Подсчет товаров дороже 1000 руб." << endl;
            int expensiveCount = count_if(basketItems.begin(), basketItems.end(),
                [](const pair<Product*, int>& item) {
                    return item.first->getPrice() > 1000;
                });
            cout << "Товаров дороже 1000 руб.: " << expensiveCount << endl;

            cout << "\n4. Общая стоимость: " << endl;
            double total = accumulate(basketItems.begin(), basketItems.end(), 0.0,
                [](double sum, const pair<Product*, int>& item) {
                    return sum + (item.first->getPrice() * item.second);
                });
            cout << "Общая стоимость: " << total << " руб." << endl;

            break;
        }
        case 0:
            cout << "Выход из режима корзины..." << endl;
            break;
        default:
            cout << "Неверный выбор!" << endl;
        }
    } while (choice != 0);
}

void userOrderMode() {
    cout << "\n=== ОФОРМЛЕНИЕ ЗАКАЗА ===" << endl;
    if (userBasket.IsEmpty()) {
        cout << "Корзина пуста! Добавьте товары перед оформлением заказа." << endl;
        cout << "Перейдите в раздел 'Корзина товаров' для добавления товаров." << endl;
        return;
    }

    if (!currentClient) {
        cout << "Сначала выполните вход как клиент!" << endl;
        return;
    }

    cout << "\nСодержимое вашей корзины:" << endl;
    userBasket.DisplayBasket();

    char confirmation;
    cout << "\nВы уверены, что хотите оформить заказ? (y/n): ";
    cin >> confirmation;

    if (confirmation != 'y' && confirmation != 'Y') {
        cout << "Оформление заказа отменено." << endl;
        return;
    }

    string clientId = currentClient->getId();

    // Создаем временную корзину для заказа
    Basket tempBasket;
    for (int i = 0; i < userBasket.getProductCount(); i++) {
        Product* product = userBasket.getProduct(i);
        int quantity = userBasket.getQuantity(i);
        if (product != nullptr) {
            tempBasket.AddBasket(product, quantity);
        }
    }

    // Создаем заказ
    Order* clientOrder = new Order(clientId, tempBasket);

    // Передаем заказ в магазин
    musicShop.CreateOrder(clientId, userBasket);

    cout << "\n==================================" << endl;
    cout << "ЗАКАЗ УСПЕШНО ОФОРМЛЕН!" << endl;
    cout << "Номер заказа: " << clientOrder->getNumberOrder() << endl;
    cout << "Дата: " << clientOrder->getDateOrder() << endl;
    cout << "Сумма: " << clientOrder->getCostOrder() << " руб." << endl;
    cout << "Количество товаров: " << clientOrder->getCountOrder() << " шт." << endl;
    cout << "==================================" << endl;

    // Обновляем информацию о клиенте
    if (currentClient) {
        currentClient->updateTotalSpent(clientOrder->getCostOrder());
        int loyaltyPoints = clientOrder->getCostOrder() / 100; // 1 балл за каждые 100 руб.
        currentClient->updateLoyaltyPoints(loyaltyPoints);
        cout << "Начислено бонусных баллов: " << loyaltyPoints << endl;
        cout << "Ваша новая скидка: " << currentClient->calculateDiscount() << "%" << endl;
    }

    // Предлагаем оставить отзыв о купленных товарах
    cout << "\nХотите оставить отзыв о купленных товарах? (y/n): ";
    cin >> confirmation;
    if (confirmation == 'y' || confirmation == 'Y') {
        cout << "Перейдите в раздел 'Оставить отзыв на музыкальный товар' в меню пользователя." << endl;
    }

    // Очищаем корзину после оформления заказа
    userBasket.ClearBasket();
}

void addProductToBasketByArticle(Basket& basket) {
    int article, quantity;
    cout << "Введите артикул товара: ";
    cin >> article;
    cout << "Введите количество: ";
    cin >> quantity;

    if (quantity <= 0) {
        cout << "Количество должно быть положительным числом!" << endl;
        return;
    }

    ifstream file("products.txt");
    if (!file.is_open()) {
        cout << "Файл с товарами не найден!" << endl;
        return;
    }

    string line;
    bool found = false;
    while (getline(file, line)) {
        if (line.empty()) continue;

        stringstream ss(line);
        string token;
        string tokens[10];
        int tokenCount = 0;

        while (getline(ss, token, '|') && tokenCount < 10) {
            tokens[tokenCount++] = token;
        }

        if (tokenCount == 10) {
            try {
                int currentArticle = stoi(tokens[0]);
                if (currentArticle == article) {
                    found = true;
                    Product* product = new Product(
                        stoi(tokens[0]),
                        tokens[1],
                        tokens[2],
                        tokens[3],
                        tokens[4],
                        tokens[5],
                        tokens[6],
                        stoi(tokens[7]),
                        stoi(tokens[9])
                    );
                    basket.AddBasket(product, quantity);
                    cout << "Товар '" << tokens[1] << "' добавлен в корзину!" << endl;
                    break;
                }
            }
            catch (const exception& e) {
                continue;
            }
        }
    }
    file.close();

    if (!found) {
        cout << "Товар с артикулом " << article << " не найден!" << endl;
    }
}

// Главная функция
int main() {
    setlocale(LC_ALL, "ru_RU.UTF-8");

    cout << "===============================================" << endl;
    cout << "   МУЗЫКАЛЬНЫЙ МАГАЗИН 'MUSICLOVERZZ'   " << endl;
    cout << "===============================================" << endl;

    loginMenu();

    int choice;
    do {
        cout << "\n=== ГЛАВНОЕ МЕНЮ ===" << endl;
        cout << "1. Режим администратора" << endl;
        cout << "2. Режим пользователя" << endl;
        cout << "3. Управление магазином" << endl;
        cout << "4. Сменить пользователя" << endl;
        cout << "5. Информация о системе" << endl;
        cout << "0. Выход" << endl;
        cout << "Выберите действие: ";
        cin >> choice;

        switch (choice) {
        case 1:
            adminMode();
            break;
        case 2:
            userMode();
            break;
        case 3:
            shopManagementMode();
            break;
        case 4:
            loginMenu();
            break;
        case 5: {
            cout << "\n=== ИНФОРМАЦИЯ О СИСТЕМЕ ===" << endl;
            cout << "Система управления музыкальным магазином" << endl;
            cout << "Используемые технологии:" << endl;
            cout << "- Шаблонные классы (Review<Derived>)" << endl;
            cout << "- Наследование и полиморфизм (Review, MusicProductReview, User, Product)" << endl;
            cout << "- STL алгоритмы (sort, find, transform, accumulate, copy_if и др.)" << endl;
            cout << "- Умные указатели (shared_ptr)" << endl;
            cout << "- Работа с файлами" << endl;
            cout << "- Валидация данных" << endl;
            cout << "\nДоступные алгоритмы сортировки отзывов:" << endl;
            cout << "- По дате (новые/старые сначала)" << endl;
            cout << "- По рейтингу (высокий/низкий сначала)" << endl;
            cout << "- По полезности" << endl;
            cout << "- Проверенные сначала" << endl;
            cout << "- Пользовательские критерии сортировки" << endl;
            cout << "\nSTL контейнеры в системе:" << endl;
            cout << "- vector<shared_ptr<MusicProductReview>>" << endl;
            cout << "- list<shared_ptr<MusicProductReview>>" << endl;
            cout << "- vector<string> для хранения преимуществ/недостатков" << endl;
            cout << "===============================" << endl;
            break;
        }
        case 0:
            cout << "Выход из программы..." << endl;
            if (currentClient) {
                delete currentClient;
                currentClient = nullptr;
            }
            if (currentAdmin) {
                delete currentAdmin;
                currentAdmin = nullptr;
            }
            break;
        default:
            cout << "Неверный выбор!" << endl;
        }
    } while (choice != 0);

    return 0;
}