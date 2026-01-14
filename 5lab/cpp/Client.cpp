#include "Client.h"
#include "Order.h"
#include "Review.h"
#include <iostream>
#include <iomanip>
#include <algorithm>

using namespace std;
Client::Client(const std::string& name, const std::string& email,
    const std::string& phone, const std::string& address)
    : User(generateUserId("CLT"), name, email, phone),
    totalSpent(0.0), loyaltyPoints(0), address(address), newsletterSubscription(true) {}


Client::Client(const std::string& id, const std::string& name,
    const std::string& email, const std::string& phone,
    const std::string& address, double totalSpent, int loyaltyPoints,
    bool newsletter, const std::string& regDate)
    : User(id, name, email, phone, regDate),
    totalSpent(totalSpent), loyaltyPoints(loyaltyPoints),
    address(address), newsletterSubscription(newsletter) {}

// Деструктор
Client::~Client() {
    for (auto order : orderHistory) {
        delete order;
    }
}


// Оператор += для добавления суммы покупки
Client& Client::operator+=(double amount) {
    totalSpent += amount;
    loyaltyPoints += static_cast<int>(amount / 100);
    return *this;
}

// Оператор += для добавления заказа
Client& Client::operator+=(const Order& order) {
    Order* newOrder = new Order(order);
    orderHistory.push_back(newOrder);
    *this += order.getCostOrder();  // Используем другой operator+=
    return *this;
}

// Оператор -= для возврата денег
Client& Client::operator-=(double amount) {
    if (totalSpent >= amount) {
        totalSpent -= amount;
        loyaltyPoints = max(0, loyaltyPoints - static_cast<int>(amount / 100));
    }
    return *this;
}

// Оператор -= для отмены заказа
Client& Client::operator-=(const Order& order) {
    auto it = find_if(orderHistory.begin(), orderHistory.end(),
        [&order](Order* o) { return o->getNumberOrder() == order.getNumberOrder(); });

    if (it != orderHistory.end()) {
        *this -= (*it)->getCostOrder();  // Используем другой operator-=
        delete* it;
        orderHistory.erase(it);
    }
    return *this;
}

// Оператор < (сравнение по потраченной сумме)
bool Client::operator<(const Client& other) const {
    return totalSpent < other.totalSpent;
}

// Оператор > (сравнение по потраченной сумме)
bool Client::operator>(const Client& other) const {
    return totalSpent > other.totalSpent;
}

// Оператор индекса для доступа к заказам
Order* Client::operator[](size_t index) const {
    if (index < orderHistory.size()) {
        return orderHistory[index];
    }
    return nullptr;
}

// Унарный оператор для проверки активности
bool Client::operator!() const {
    return totalSpent == 0.0 && orderHistory.empty();
}


void Client::displayInfo() const {
    cout << *this;  // Используем глобальный operator<<
}

void Client::addOrder(Order* order) {
    orderHistory.push_back(order);
    updateTotalSpent(order->getCostOrder());
    updateLoyaltyPoints(static_cast<int>(order->getCostOrder() / 100));
}

void Client::updateTotalSpent(double amount) {
    totalSpent += amount;
}

void Client::updateLoyaltyPoints(int points) {
    loyaltyPoints += points;
}

void Client::showOrderHistory() const {
    if (orderHistory.empty()) {
        cout << "\nИстория заказов пуста!" << endl;
        return;
    }

    cout << "\n=== ИСТОРИЯ ЗАКАЗОВ ===" << endl;
    for (size_t i = 0; i < orderHistory.size(); i++) {
        cout << "\n--- Заказ " << (i + 1) << " ---" << endl;
        orderHistory[i]->InfoOrder();
    }
}

double Client::calculateDiscount() const {
    if (totalSpent >= 10000) return 10.0;
    if (totalSpent >= 5000) return 5.0;
    if (totalSpent >= 1000) return 2.0;
    return 0.0;
}

void Client::initializeClientSpecificData(double totalSpent, int loyaltyPoints,
    const std::string& address, bool newsletter) {
    this->totalSpent = totalSpent;
    this->loyaltyPoints = loyaltyPoints;
    this->address = address;
    this->newsletterSubscription = newsletter;
}


// Оператор вывода
std::ostream& operator<<(std::ostream& os, const Client& client) {
    // Выводим базовую информацию через operator<< User
    os << static_cast<const User&>(client);

    // Добавляем специфичную информацию
    os << "\n--- ДОПОЛНИТЕЛЬНАЯ ИНФОРМАЦИЯ ---" << endl;
    os << "Адрес: " << (client.getAddress().empty() ? "не указан" : client.getAddress()) << endl;
    os << "Общая сумма покупок: " << fixed << setprecision(2)
        << client.getTotalSpent() << " руб." << endl;
    os << "Баллы лояльности: " << client.getLoyaltyPoints() << endl;
    os << "Количество заказов: " << client.getOrderCount() << endl;
    os << "Подписка на рассылку: " << (client.getNewsletterSubscription() ? "Да" : "Нет") << endl;
    os << "Текущая скидка: " << client.calculateDiscount() << "%" << endl;
    os << "==================================" << endl;

    return os;
}

// Оператор + для объединения клиентов
Client operator+(const Client& c1, const Client& c2) {
    string newId = c1.getId() + "_" + c2.getId();
    string newName = c1.getName() + " & " + c2.getName();
    string newEmail = c1.getEmail() + "; " + c2.getEmail();
    string newAddress = c1.getAddress() + " / " + c2.getAddress();

    double newTotalSpent = c1.getTotalSpent() + c2.getTotalSpent();
    int newLoyaltyPoints = c1.getLoyaltyPoints() + c2.getLoyaltyPoints();
    bool newNewsletter = c1.getNewsletterSubscription() || c2.getNewsletterSubscription();

    return Client(newId, newName, newEmail, c1.getPhone(), newAddress,
        newTotalSpent, newLoyaltyPoints, newNewsletter, "");
}
void Client::GetReview() {
    cout << "\n=== ОСТАВИТЬ ОТЗЫВ ===" << endl;

    string productName;
    string comment;
    int rating;

    cout << "Введите название товара: ";
    cin.ignore(numeric_limits<streamsize>::max(), '\n');
    getline(cin, productName);

    cout << "Введите ваш отзыв: ";
    getline(cin, comment);

    while (true) {
        cout << "Введите оценку (1-5, где 5 - отлично): ";
        cin >> rating;

        if (cin.fail()) {
            cin.clear();
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
            cout << "Ошибка! Введите число от 1 до 5." << endl;
            continue;
        }

        if (rating >= 1 && rating <= 5) {
            break;
        }

        cout << "Ошибка! Оценка должна быть от 1 до 5." << endl;
    }

    // Создаем Review через конструктор по умолчанию и сеттеры
    Review newReview;
    newReview.setProductName(productName);
    newReview.setClientId(getId());
    newReview.setRating(rating);
    newReview.setComment(comment);
    newReview.SaveToFile();

    cout << "\n==================================" << endl;
    cout << "ОТЗЫВ УСПЕШНО СОХРАНЕН!" << endl;
    cout << "Товар: " << productName << endl;
    cout << "Ваша оценка: " << rating << "/5" << endl;
    cout << "Спасибо за ваш отзыв!" << endl;
    cout << "==================================" << endl;
}
