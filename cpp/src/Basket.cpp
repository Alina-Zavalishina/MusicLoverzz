#include "Basket.h"
#include <iostream>
#include <iomanip>//форматирование текста и таблиц

using namespace std;

Basket::Basket() : productCount(0) {// инициализация счетчика товаров нулем
    // Инициализация массивов корзины
    for (int i = 0; i < MAX_PRODUCTS; i++) {
        products[i] = nullptr;// обнуляем все указатели на товары
        quantities[i] = 0;// обнуляем все количества
    }
}
// Деструктор класса Basket - очищает корзину при уничтожении объекта
Basket::~Basket() {
    ClearBasket(); //метод очистки корзины
}

// Метод для расчета общей стоимости всех товаров в корзине
double Basket::GetBasketCost() const {
    double totalCost = 0.0;// переменная для накопления общей стоимости
    // Проходим по всем товарам в корзине
    for (int i = 0; i < productCount; i++) {
        // Проверяем что указатель на товар не нулевой
        if (products[i] != nullptr) {
            // Суммируем: цена товара * количество
            totalCost += products[i]->getPrice() * quantities[i];
        }
    }
    return totalCost;
}

// Метод для получения общего количества всех товаров в корзине
unsigned int Basket::GetBasketCount() const {
    unsigned int totalCount = 0;// переменная для подсчета общего количества
    for (int i = 0; i < productCount; i++) {// добавляем количество каждого товара
        totalCount += quantities[i];
    }
    return totalCount;
}

void Basket::DisplayBasket() const {
    cout << "\n=== СОДЕРЖИМОЕ КОРЗИНЫ ===" << endl;
    if (productCount == 0) {
        cout << "Корзина пуста!" << endl;
        cout << "=========================" << endl;
        return;
    }

    cout << left << setw(3) << "№" << setw(10) << "Артикул"
        << setw(20) << "Название" << setw(8) << "Цена"
        << setw(10) << "Кол-во" << setw(12) << "Сумма" << endl;
    cout << "------------------------------------------------------------" << endl;

    double totalCost = 0.0;//общая стоимость

    // Вывод информации о каждом товаре в корзине
    for (int i = 0; i < productCount; i++) {
        // Проверяем что товар существует
        if (products[i] != nullptr) {
            double itemTotal = products[i]->getPrice() * quantities[i];
            totalCost += itemTotal;// добавляем к общей стоимости
            // Вывод информации о товаре с форматированием
            cout << left << setw(3) << (i + 1)
                << setw(10) << products[i]->getArticle()
                << setw(20) << (products[i]->getName().length() > 18 ?
                    products[i]->getName().substr(0, 18) + ".." :
                    products[i]->getName())
                << setw(8) << products[i]->getPrice()
                << setw(10) << quantities[i]
                << setw(12) << itemTotal << endl;
        }
    }

    cout << "------------------------------------------------------------" << endl;
    cout << "ИТОГО: " << totalCost << " руб." << endl;
    cout << "Общее количество товаров: " << GetBasketCount() << endl;
    cout << "=========================" << endl;
}

// Добавление товара в корзину
void Basket::AddBasket(Product* product, int quantity) {
    // Проверка на переполнение корзины
    if (productCount >= MAX_PRODUCTS) {
        cout << "Корзина переполнена! Максимум " << MAX_PRODUCTS << " товаров." << endl;
        return;
    }
    // Проверка корректности количества
    if (quantity <= 0) {
        cout << "Количество должно быть положительным числом!" << endl;
        return;
    }

    // Проверяем, есть ли уже такой товар в корзине
    int existingIndex = FindProductIndex(product->getArticle());
    //если товар найден, увеличиваем количество
    if (existingIndex != -1) {
        quantities[existingIndex] += quantity;
        cout << "Количество товара '" << product->getName()
            << "' увеличено на " << quantity << ". Теперь: "
            << quantities[existingIndex] << " шт." << endl;
    }
    else {
        // Добавляем новый товар
        products[productCount] = product; // сохраняем указатель на товар
        quantities[productCount] = quantity;// сохраняем количество
        productCount++;
        cout << "Товар '" << product->getName()
            << "' добавлен в корзину в количестве " << quantity << " шт." << endl;
    }
}

// Удаление товара из корзины по индексу
void Basket::DeleteBasket(int productIndex) {
    // Проверка корректности индекса (пользователь вводит с 1, а не с 0)
    if (productIndex < 1 || productIndex > productCount) {
        cout << "Неверный номер товара!" << endl;
        return;
    }

    int actualIndex = productIndex - 1;// сохраняем старое количество
    string productName = products[actualIndex]->getName();// сохраняем имя для сообщения

    // Сдвигаем элементы массива влево, начиная с удаляемого элемента
    for (int i = actualIndex; i < productCount - 1; i++) {
        products[i] = products[i + 1];// сдвиг указателей на товары
        quantities[i] = quantities[i + 1];// сдвиг количеств
    }

    products[productCount - 1] = nullptr;// Очищаем последнюю ячейку (которая теперь дублируется)
    quantities[productCount - 1] = 0;
    productCount--;// уменьшаем счетчик товаров

    cout << "Товар '" << productName << "' удален из корзины." << endl;
}

// Обновление количества товара по индексу
int Basket::UpdateQuantity(int productIndex, int newQuantity) {
    if (productIndex < 1 || productIndex > productCount) {
        // Проверка корректности индекса
        cout << "Неверный номер товара!" << endl;
        return -1;
    }

    if (newQuantity <= 0) {
        cout << "Количество должно быть положительным числом!" << endl;
        return -1;
    }

    int actualIndex = productIndex - 1;// Преобразуем пользовательский индекс в программный
    int oldQuantity = quantities[actualIndex];// сохраняем старое количество
    quantities[actualIndex] = newQuantity;// устанавливаем новое количество

    cout << "Количество товара '" << products[actualIndex]->getName()
        << "' изменено с " << oldQuantity << " на " << newQuantity << " шт." << endl;
    return newQuantity;// возвращаем новое количество
}

// Проверка, пуста ли корзина
bool Basket::IsEmpty() const {
    return productCount == 0;// true если товаров нет, false если есть
}

// Полная очистка корзины
void Basket::ClearBasket() {
    // Проходим по всем занятым ячейкам корзины
    for (int i = 0; i < productCount; i++) {
        // Не удаляем сами продукты, только обнуляем указатели
        products[i] = nullptr;
        quantities[i] = 0;
    }
    productCount = 0;// сбрасываем счетчик товаров
}

// Поиск индекса товара в корзине по артикулу
int Basket::FindProductIndex(int article) const {
    // Проходим по всем товарам в корзине
    for (int i = 0; i < productCount; i++) {
        // Проверяем что указатель не нулевой и артикул совпадает
        if (products[i] != nullptr && products[i]->getArticle() == article) {
            return i; // возвращаем индекс найденного товара
        }
    }
    return -1;
}

//нужны для безопасного доступа к данным из корзины,защищая от ошибок
// Геттер для получения товара по индексу
Product* Basket::getProduct(int index) const {
    // Проверка корректности индекса
    if (index < 0 || index >= productCount) {
        return nullptr;// возвращаем nullptr при неверном индексе
    }
    return products[index];// возвращаем указатель на товар
}

// Геттер для получения количества товара по индексу
int Basket::getQuantity(int index) const {
    // Проверка корректности индекса
    if (index < 0 || index >= productCount) {
        return 0;// возвращаем 0 при неверном индексе
    }
    return quantities[index];// возвращаем количество товара
}