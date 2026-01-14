#ifndef SHOP_HPP
#define SHOP_HPP

#include "Product.h"
#include "Order.h"
#include "ProviderOrder.h"
#include "SalesReport.h"
#include <string>

class Shop {
private:
    static const int MAX_PRODUCTS = 1000;// максимальное количество товаров в каталоге
    static const int MAX_ORDERS = 1000;// максимальное количество активных заказов
    static const int MAX_PROVIDER_ORDERS = 100;// максимальное количество заказов поставщикам
    static const int MAX_SALES_REPORTS = 50;// максимальное количество отчетов о продажах

    std::string shopName;// название магазина
    std::string addressShop;// адрес магазина
    Product* catalogProducts[MAX_PRODUCTS];// массив указателей на товары
    int productCount;// текущее количество товаров в каталоге
    Order* activeOrders[MAX_ORDERS];// массив указателей на активные заказы клиентов
    int orderCount; // текущее количество активных заказов
    ProviderOrder* providerOrders[MAX_PROVIDER_ORDERS];// массив указателей на заказы поставщикам
    int providerOrderCount;// текущее количество заказов поставщикам
    SalesReport* salesReports[MAX_SALES_REPORTS]; // массив указателей на отчеты о продажах
    int salesReportCount;// текущее количество отчетов

public:
    Shop();
    Shop(const std::string& name, const std::string& address);
    ~Shop();

    void AddProduct(); // добавление нового товара в каталог
    void DeleteProduct(); // удаление товара из каталога
    void CreateOrder(const std::string& clientId, Basket& basket); // создание нового заказа
    void ShowShopInfo() const;// вывод основной информации о магазине
    void DisplayCatalog() const;// отображение каталога товаров
    void DisplayOrders() const; // отображение списка заказов клиентов
    void DisplayProviderOrders() const;// отображение списка заказов поставщикам
    void DisplaySalesReports() const;// отображение отчетов о продажах
    void CreateSalesReport();// создание нового отчета о продажах
    void RemakeSalesReport();// пересчет существующего отчета

    void LoadProductsFromFile(const std::string& filename);// загрузка товаров из файла
    void AddProviderOrder(ProviderOrder* providerOrder); // Метод добавления заказа поставщику в систему магазина

    std::string getShopName() const { return shopName; }// получить название магазина
    std::string getAddress() const { return addressShop; }  // получить адрес магазина
    int getProductCount() const { return productCount; }// получить количество товаров
    int getOrderCount() const { return orderCount; }// получить количество заказов

private:
    void initializeArrays();// инициализация массивов
    int findProductIndex(int article) const;// поиск индекса товара по артикулу
    void updateProductStock(int article, int quantityChange);// обновление количества товара на складе
    void LoadSalesReports(); // загрузка отчетов о продажах
};

#endif
