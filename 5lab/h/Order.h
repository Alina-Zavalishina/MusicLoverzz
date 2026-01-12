#ifndef ORDER_HPP
#define ORDER_HPP

#include "Basket.h"
#include <string>
#include <ctime>

class Order {
private:
    static const int MAX_ORDER_ITEMS = 100;
    std::string numberOrder;
    std::string clientOrder;
    Product* orderItems[MAX_ORDER_ITEMS];
    int quantities[MAX_ORDER_ITEMS];
    int itemCount;
    int countOrder;
    double costOrder;
    std::string statusOrder;
    std::string payWay;
    std::string shopOrder;
    std::string dateOrder;

public:
    // Конструкторы
    Order();
    Order(const std::string& clientId, Basket& basket);

    // Конструктор копирования
    Order(const Order& other);

    // Деструктор
    ~Order();

    // Оператор присваивания 
    Order& operator=(const Order& other);

    // Методы
    void ChangeStatus(const std::string& newStatus);
    bool Payment();
    void InfoOrder() const;
    static bool MakeOrder(const std::string& clientId, Basket& basket);
    void SaveToFile();

    // Геттеры
    std::string getNumberOrder() const { return numberOrder; }
    std::string getClientOrder() const { return clientOrder; }
    std::string getStatusOrder() const { return statusOrder; }
    double getCostOrder() const { return costOrder; }
    int getCountOrder() const { return countOrder; }
    std::string getDateOrder() const { return dateOrder; }
    std::string getPayWay() const { return payWay; }
    std::string getShopOrder() const { return shopOrder; }
    int getItemCount() const { return itemCount; }

    // Методы для доступа к товарам заказа
    Product* getOrderItem(int index) const {
        if (index >= 0 && index < itemCount) {
            return orderItems[index];
        }
        return nullptr;
    }

    int getQuantity(int index) const {
        if (index >= 0 && index < itemCount) {
            return quantities[index];
        }
        return 0;
    }

private:
    void CopyProductsFromBasket(Basket& basket);
    std::string generateOrderNumber();
    std::string getCurrentDate();

    // Вспомогательный метод для очистки памяти
    void clearOrderItems();

    // Вспомогательный метод для глубокого копирования
    void deepCopy(const Order& other);
};
#endif
