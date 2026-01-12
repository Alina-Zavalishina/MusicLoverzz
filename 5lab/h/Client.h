#ifndef CLIENT_HPP
#define CLIENT_HPP

#include "User.h"
#include "Order.h"
#include <string>
#include <vector>

class Client : public User {
private:
    std::vector<Order*> orderHistory;
    double totalSpent;
    int loyaltyPoints;
    std::string address;
    bool newsletterSubscription;

public:
    // Конструкторы
    Client(const std::string& name, const std::string& email = "",
        const std::string& phone = "", const std::string& address = "");

    Client(const std::string& id, const std::string& name,
        const std::string& email, const std::string& phone,
        const std::string& address, double totalSpent, int loyaltyPoints,
        bool newsletter, const std::string& regDate);

    ~Client();
    void GetReview();

    void displayInfo() const override;


    Client& operator+=(double amount);        // Добавить сумму покупки
    Client& operator+=(const Order& order);   // Добавить заказ
    Client& operator-=(double amount);        // Вернуть деньги
    Client& operator-=(const Order& order);   // Отменить заказ

    // Операторы сравнения по потраченной сумме
    bool operator<(const Client& other) const;
    bool operator>(const Client& other) const;

    // Оператор индекса для доступа к заказам
    Order* operator[](size_t index) const;

    // Унарный оператор для проверки активности
    bool operator!() const;

    void addOrder(Order* order);
    void updateLoyaltyPoints(int points);
    void updateTotalSpent(double amount);

    // Геттеры
    double getTotalSpent() const { return totalSpent; }
    int getLoyaltyPoints() const { return loyaltyPoints; }
    std::string getAddress() const { return address; }
    bool getNewsletterSubscription() const { return newsletterSubscription; }
    int getOrderCount() const { return orderHistory.size(); }

    // Сеттеры
    void setAddress(const std::string& newAddress) { address = newAddress; }
    void setNewsletterSubscription(bool subscribe) { newsletterSubscription = subscribe; }

    // Специальные методы
    void showOrderHistory() const;
    double calculateDiscount() const;

private:
    void initializeClientSpecificData(double totalSpent, int loyaltyPoints,
        const std::string& address, bool newsletter);
};

// Глобальные операторы для Client
std::ostream& operator<<(std::ostream& os, const Client& client);
Client operator+(const Client& c1, const Client& c2);  // Объединение клиентов

#endif
