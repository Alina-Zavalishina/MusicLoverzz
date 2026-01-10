#ifndef ORDER_HPP
#define ORDER_HPP

#include "Basket.h"
#include <string>
#include <ctime>//библиотека для работы с датой и временем

class Order {
private:
    static const int MAX_ORDER_ITEMS = 100;//максимальное количество различных товаров в заказе

    std::string numberOrder;//номер заказа
    std::string clientOrder;//идентификатор клиента
    Product* orderItems[MAX_ORDER_ITEMS];//массив указателей на товары в заказе
    int quantities[MAX_ORDER_ITEMS];//массив количеств для каждого товара
    int itemCount;//количество различных товаров в заказе
    int countOrder;//общее количество
    double costOrder;//общая стоимость
    std::string statusOrder;//статус заказа
    std::string payWay;//cпособ оплаты
    std::string shopOrder;//магазин для получения
    std::string dateOrder;//дата и время создания заказа

public:
    Order();
    Order(const std::string& clientId, Basket& basket);
    ~Order();

    void ChangeStatus(const std::string& newStatus);//изменение статуса
    bool Payment();//оплата
    void InfoOrder() const;//вывод полной информации о заказе
    static bool MakeOrder(const std::string& clientId, Basket& basket);//создание заказа
    void SaveToFile();//сохранить в файле

    // Геттеры
    std::string getNumberOrder() const { return numberOrder; }//получить номер заказа
    std::string getClientOrder() const { return clientOrder; }// получить идентификатор клиента
    std::string getStatusOrder() const { return statusOrder; }// получить текущий статус заказа
    double getCostOrder() const { return costOrder; }// получить общую стоимость заказа
    int getCountOrder() const { return countOrder; }// получить общее количество товарных единиц
    std::string getDateOrder() const { return dateOrder; }// получить дату создания заказа
    std::string getPayWay() const { return payWay; }// получить способ оплаты
    std::string getShopOrder() const { return shopOrder; }// получить магазин

    // Методы для доступа к товарам заказа
    int getItemCount() const { return itemCount; }// получить количество различных товаров в заказе

    // Получить товар по индексу с проверкой границ массива
    Product* getOrderItem(int index) const {
        if (index >= 0 && index < itemCount) {
            return orderItems[index];// возвращаем указатель на товар
        }
        return nullptr;
    }
    // Получить количество товара по индексу с проверкой границ массива
    int getQuantity(int index) const {
        if (index >= 0 && index < itemCount) {
            return quantities[index];// возвращаем количество товара
        }
        return 0;
    }

private:
    void CopyProductsFromBasket(Basket& basket);// копирование товаров из корзины в заказ
    std::string generateOrderNumber();// генерация уникального номера заказа
    std::string getCurrentDate();// получение текущей даты и времени в строковом формате
};

#endif