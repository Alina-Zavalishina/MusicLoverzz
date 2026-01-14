#define _CRT_SECURE_NO_WARNINGS
#include "Client.h"
#include <iostream>
#include <iomanip>
#include <algorithm>

using namespace std;


class Order {
public:
    std::string getNumber() const { return "ORD001"; }
    double getCost() const { return 1000.0; }
};

// вызов конструктора базового класса User с параметрами
Client::Client(const std::string& name,
    const std::string& email,
    const std::string& phone,
    const std::string& address)
    // ВЫЗОВ КОНСТРУКТОРА БАЗОВОГО КЛАССА User с параметрами
    : User(generateUserId("CLT"), name, email, phone),
    totalSpent(0.0), loyaltyPoints(0), address(address),
    newsletterSubscription(true) {
}


Client::Client(const std::string& id, const std::string& name,
    const std::string& email, const std::string& phone,
    const std::string& address, double totalSpent,
    int loyaltyPoints, bool newsletter)
    : User(id, name, email, phone),
    totalSpent(totalSpent), loyaltyPoints(loyaltyPoints),
    address(address), newsletterSubscription(newsletter) {

}

// Деструктор
Client::~Client() {
    // Очищаем историю заказов
    for (auto order : orderHistory) {
        delete order;
    }
}


void Client::displayInfo() const {
    showBasicInfo();


    cout << "\n--- ИНФОРМАЦИЯ О КЛИЕНТЕ ---" << endl;
    cout << "Адрес: " << (address.empty() ? "не указан" : address) << endl;
    cout << "Общая сумма покупок: " << fixed << setprecision(2)
        << totalSpent << " руб." << endl;
    cout << "Баллы лояльности: " << loyaltyPoints << endl;
    cout << "Количество заказов: " << orderHistory.size() << endl;
    cout << "Подписка на рассылку: " << (newsletterSubscription ? "Да" : "Нет") << endl;
    cout << "Текущая скидка: " << calculateDiscount() << "%" << endl;
    cout << "================================" << endl;
}

// Переопределение метода calculateDiscount
double Client::calculateDiscount() const {

    if (totalSpent >= 10000) return 10.0;
    if (totalSpent >= 5000) return 5.0;
    if (totalSpent >= 1000) return 2.0;
    return 0.0;
}

// перегрузка оператора присваивания
Client& Client::operator=(const User& user) {
    if (this != &user) {
        // Копируем данные из базового класса
        User::operator=(user); // ВЫЗЫВАЕМ ОПЕРАТОР ПРИСВАИВАНИЯ БАЗОВОГО КЛАССА

        // Устанавливаем значения по умолчанию для специфичных полей Client
        totalSpent = 0.0;
        loyaltyPoints = 0;
        address = "Адрес не указан (присвоено)";
        newsletterSubscription = true;

        // Очищаем историю заказов
        orderHistory.clear();
    }
    return *this;
}

// Методы клиента
void Client::addOrder(Order* order) {
    if (order) {
        orderHistory.push_back(order);
        updateTotalSpent(order->getCost());
        updateLoyaltyPoints(static_cast<int>(order->getCost() / 100));
        cout << "Заказ добавлен в историю клиента: " << getName() << endl;
    }
}

void Client::updateTotalSpent(double amount) {
    if (amount > 0) {
        totalSpent += amount;
        cout << "Общая сумма покупок обновлена: " << totalSpent << " руб." << endl;
    }
}

void Client::updateLoyaltyPoints(int points) {
    if (points > 0) {
        loyaltyPoints += points;
        cout << "Баллы лояльности обновлены: " << loyaltyPoints << " баллов" << endl;
    }
}

void Client::showOrderHistory() const {
    if (orderHistory.empty()) {
        cout << "\nИстория заказов пуста!" << endl;
        return;
    }

    cout << "\n=== ИСТОРИЯ ЗАКАЗОВ ===" << endl;
    for (size_t i = 0; i < orderHistory.size(); i++) {
        cout << "Заказ " << (i + 1) << ": №" << orderHistory[i]->getNumber()
            << ", Сумма: " << orderHistory[i]->getCost() << " руб." << endl;
    }
    cout << "======================" << endl;
}

void Client::initializeClientData(double totalSpent, int loyaltyPoints,
    const std::string& address, bool newsletter) {

    this->totalSpent = totalSpent;
    this->loyaltyPoints = loyaltyPoints;
    this->address = address;
    this->newsletterSubscription = newsletter;
}

// Глобальный оператор вывода для Client
std::ostream& operator<<(std::ostream& os, const Client& client) {
    const User& userRef = client;
    os << userRef;

    // Добавляем специфичную информацию клиента
    os << "\n--- ДОПОЛНИТЕЛЬНАЯ ИНФОРМАЦИЯ (Client) ---" << endl;
    os << "Адрес: " << (client.getAddress().empty() ? "не указан" : client.getAddress()) << endl;
    os << "Общая сумма покупок: " << fixed << setprecision(2)
        << client.getTotalSpent() << " руб." << endl;
    os << "Баллы лояльности: " << client.getLoyaltyPoints() << endl;
    os << "Количество заказов: " << client.getOrderCount() << endl;
    os << "Подписка на рассылку: " << (client.getNewsletterSubscription() ? "Да" : "Нет") << endl;
    os << "Скидка: " << client.calculateDiscount() << "%" << endl;
    os << "==================================" << endl;

    return os;
}

