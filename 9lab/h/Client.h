#ifndef CLIENT_HPP
#define CLIENT_HPP

#include "User.h"
#include <string>
#include <vector>

class Order;

class Client : public User {
private:
    std::vector<Order*> orderHistory;
    double totalSpent;
    int loyaltyPoints;
    std::string address;
    bool newsletterSubscription;

public:
    // Конструкторы
    // вызов конструктора базового класса с параметрами
    Client(const std::string& name,
        const std::string& email = "",
        const std::string& phone = "",
        const std::string& address = "");

    Client(const std::string& id, const std::string& name,
        const std::string& email, const std::string& phone,
        const std::string& address, double totalSpent = 0.0,
        int loyaltyPoints = 0, bool newsletter = true);

    ~Client();

    // Переопределение метода базового класса
    void displayInfo() const override;

    // Переопределение метода базового класса
    double calculateDiscount() const override;

    // объявление перегрузки
    Client& operator=(const User& user);

   
    void addOrder(Order* order);
    void updateTotalSpent(double amount);
    void updateLoyaltyPoints(int points);
    void showOrderHistory() const;

    // Геттеры
    double getTotalSpent() const { return totalSpent; }
    int getLoyaltyPoints() const { return loyaltyPoints; }
    std::string getAddress() const { return address; }
    bool getNewsletterSubscription() const { return newsletterSubscription; }
    int getOrderCount() const { return orderHistory.size(); }

    // Сеттеры
    void setAddress(const std::string& newAddress) { address = newAddress; }
    void setNewsletterSubscription(bool subscribe) { newsletterSubscription = subscribe; }

private:
    void initializeClientData(double totalSpent, int loyaltyPoints,
        const std::string& address, bool newsletter);
};

// Глобальные операторы для Client
std::ostream& operator<<(std::ostream& os, const Client& client);

#endif