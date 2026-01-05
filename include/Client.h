#ifndef CLIENT_HPP
#define CLIENT_HPP

#include <string>
#include <fstream>
#include <iostream>

class Order;

class Client {
private:
    static const int MAX_ORDER_HISTORY = 100;// Константа для ограничения размера истории заказов

    std::string clientId;  // Уникальный идентификатор клиента
    std::string clientName;// Имя клиента
    Order* orderHistory[MAX_ORDER_HISTORY]; // История заказов (массив указателей на объекты Order)
    int orderCount; // Текущее количество заказов в истории

public:
    Client();
    Client(const std::string& id);
    ~Client();

    void GetReview();  // Оставить отзыв
    void GetOrderHistory(Order* order);  // Добавить заказ в историю

    std::string getClientId() const { return clientId; }
    std::string getClientName() const { return clientName; }
    int getOrderCount() const { return orderCount; }

private:
    std::string getProductNameByArticle(int article); // Получить название товара по артикулу
    void initializeArrays();// Инициализация массивов
};

#endif