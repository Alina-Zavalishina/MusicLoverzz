#include "Client.h"
#include "Review.h"
#include "Order.h"
#include <sstream>
#include <limits>

using namespace std;

Client::Client() : orderCount(0) {
    clientId = "USER001";
    clientName = "Пользователь";
    initializeArrays();
}

Client::Client(const std::string& id) : clientId(id), orderCount(0) {
    clientName = "Пользователь";
    initializeArrays();
}
// Освобождаем память от всех заказов в истории
Client::~Client() {
    for (int i = 0; i < orderCount; i++) {
        if (orderHistory[i] != nullptr) {
            delete orderHistory[i];// Удаляем каждый заказ
        }
    }
}

void Client::GetReview() {
    cout << "\n=== ОСТАВИТЬ ОТЗЫВ ===" << endl;

    // Простой ввод данных отзыва
    string productName;
    string comment;
    int rating;

    cout << "Введите название товара: ";
    cin.ignore(numeric_limits<streamsize>::max(), '\n'); // Очищаем буфер ввода
    getline(cin, productName);

    cout << "Введите ваш отзыв: ";
    getline(cin, comment);

    // Ввод оценки с валидацией
    while (true) {
        cout << "Введите оценку (1-5, где 5 - отлично): ";
        cin >> rating;
        // Проверка на ошибку ввода
        if (cin.fail()) {
            cin.clear();
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
            cout << "Ошибка! Введите число от 1 до 5." << endl;
            continue;
        }

        if (rating >= 1 && rating <= 5) {
            break;// Выходим из цикла при корректной оценке
        }
        cout << "Ошибка! Оценка должна быть от 1 до 5." << endl;
    }

    // Создание и сохранение отзыва
    Review newReview(productName, clientId, rating, comment);
    newReview.SaveToFile();
    // Вывод подтверждения
    cout << "\n==================================" << endl;
    cout << "ОТЗЫВ УСПЕШНО СОХРАНЕН!" << endl;
    cout << "Товар: " << productName << endl;
    cout << "Ваша оценка: " << rating << "/5" << endl;
    cout << "Спасибо за ваш отзыв!" << endl;
    cout << "==================================" << endl;
}

void Client::GetOrderHistory(Order* order) {
    if (orderCount >= MAX_ORDER_HISTORY) { // Проверка на переполнение истории заказов
        cout << "История заказов переполнена!" << endl;
        return;
    }
    // Добавляем заказ в историю если он не пустой
    if (order != nullptr) {
        orderHistory[orderCount] = order;// Сохраняем указатель на заказ
        orderCount++;// Увеличиваем счетчик заказов
        cout << "Заказ №" << order->getNumberOrder() << " добавлен в историю клиента!" << endl;
    }
}

string Client::getProductNameByArticle(int article) {
    // Простой метод для получения названия товара по артикулу
    ifstream file("products.txt");
    if (!file.is_open()) {
        return "Неизвестный товар";
    }

    string line;
    while (getline(file, line)) {
        if (line.empty()) continue;

        stringstream ss(line);
        string token;
        getline(ss, token, '|');

        try {
            int currentArticle = stoi(token);
            if (currentArticle == article) {
                getline(ss, token, '|'); // Получаем название
                file.close();
                return token;
            }
        }
        catch (const exception& e) {
            continue;
        }
    }

    file.close();
    return "Неизвестный товар";
}

void Client::initializeArrays() {
    for (int i = 0; i < MAX_ORDER_HISTORY; i++) {
        orderHistory[i] = nullptr;
    }
}