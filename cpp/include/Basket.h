#ifndef BASKET_HPP
#define BASKET_HPP

#include "Product.h"
#include <string>

class Basket {
private:
    // Приватные поля класса (инкапсуляция)
    static const int MAX_PRODUCTS = 100;// максимальное количество товаров в корзине
    Product* products[MAX_PRODUCTS];// массив указателей на товары
    int quantities[MAX_PRODUCTS];// массив количеств для каждого товара
    int productCount;// текущее количество товаров в корзине

public:
    Basket();
    ~Basket();

    double GetBasketCost() const;// Получить общую стоимость всех товаров в корзине
    unsigned int GetBasketCount() const; // Получить общее количество товаров (сумму quantities)
    void DisplayBasket() const;// Отобразить содержимое корзины
    // Добавить товар в корзину
   // product - указатель на добавляемый товар
   // quantity - количество (по умолчанию 1)
    void AddBasket(Product* product, int quantity = 1);
    void DeleteBasket(int productIndex);// Удалить товар из корзины по индексу
    // Обновить количество товара по индексу
    // Возвращает 0 при успехе, -1 при ошибке
    int UpdateQuantity(int productIndex, int newQuantity);

    bool IsEmpty() const;// Проверить, пуста ли корзина
    void ClearBasket();// Очистить корзину (удалить все товары)
    // Найти индекс товара в корзине по артикулу
    int FindProductIndex(int article) const;

    int getProductCount() const { return productCount; }// Получить текущее количество различных товаров в корзине
    Product* getProduct(int index) const;// Получить товар по индексу
    int getQuantity(int index) const;// Получить количество товара по индексу
};

#endif