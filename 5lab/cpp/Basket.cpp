#include "Basket.h"
#include <iostream>
#include <iomanip>
#include <algorithm>

using namespace std;

// Конструктор по умолчанию
Basket::Basket() : productCount(0) {
    for (int i = 0; i < MAX_PRODUCTS; i++) {
        products[i] = nullptr;
        quantities[i] = 0;
    }
    cout << "Вызван конструктор по умолчанию Basket" << endl;
}

// Конструктор копирования
Basket::Basket(const Basket& other) : productCount(other.productCount) {
    cout << "Вызван конструктор копирования Basket" << endl;

    for (int i = 0; i < MAX_PRODUCTS; i++) {
        products[i] = nullptr;//обнуляет все элементы массивов
        quantities[i] = 0;
    }

    for (int i = 0; i < productCount; i++) {
        if (other.products[i] != nullptr) {
            products[i] = new Product(*(other.products[i]));//создает новый объект и разыменывает указатель
            quantities[i] = other.quantities[i];
        }
    }
}

// Оператор присваивания, копирует одну корзину из другой
Basket& Basket::operator=(const Basket& other) {
    cout << "Вызван оператор присваивания Basket" << endl;

    if (this != &other) {
        clearProducts();
        productCount = other.productCount;

        for (int i = 0; i < productCount; i++) {
            if (other.products[i] != nullptr) {
                products[i] = new Product(*(other.products[i]));
                quantities[i] = other.quantities[i];
            }
        }
    }
    return *this;
}

// Деструктор
Basket::~Basket() {
    cout << "Вызван деструктор Basket" << endl;
    clearProducts();
}

// Очистка всех продуктов
void Basket::clearProducts() {
    for (int i = 0; i < productCount; i++) {
        if (products[i] != nullptr) {
            delete products[i];
            products[i] = nullptr;
        }
        quantities[i] = 0;
    }
    productCount = 0;
}

// Глубокое копирование, безопасное копирование с динамической памятью
void Basket::deepCopy(const Basket& other) {
    clearProducts();
    productCount = other.productCount;

    for (int i = 0; i < productCount; i++) {
        if (other.products[i] != nullptr) {
            products[i] = new Product(*(other.products[i]));
            quantities[i] = other.quantities[i];
        }
    }
}



double Basket::GetBasketCost() const {
    double totalCost = 0.0;
    for (int i = 0; i < productCount; i++) {
        if (products[i] != nullptr) {
            totalCost += products[i]->getPrice() * quantities[i];
        }
    }
    return totalCost;
}

unsigned int Basket::GetBasketCount() const {
    unsigned int totalCount = 0;
    for (int i = 0; i < productCount; i++) {
        totalCount += quantities[i];
    }
    return totalCount;
}

void Basket::DisplayBasket() const {
    cout << *this;  // Используем дружественную функцию operator<<
}

void Basket::AddBasket(Product* product, int quantity) {
    addProductToBasket(*this, product, quantity);  // Используем дружественную функцию
}

void Basket::DeleteBasket(int productIndex) {
    if (productIndex < 1 || productIndex > productCount) {
        cout << "Неверный номер товара!" << endl;
        return;
    }

    int actualIndex = productIndex - 1;
    string productName = products[actualIndex]->getName();

    delete products[actualIndex];

    for (int i = actualIndex; i < productCount - 1; i++) {
        products[i] = products[i + 1];
        quantities[i] = quantities[i + 1];
    }

    products[productCount - 1] = nullptr;
    quantities[productCount - 1] = 0;
    productCount--;

    cout << "Товар '" << productName << "' удален из корзины." << endl;
}

int Basket::UpdateQuantity(int productIndex, int newQuantity) {
    if (productIndex < 1 || productIndex > productCount) {
        cout << "Неверный номер товара!" << endl;
        return -1;
    }

    if (newQuantity <= 0) {
        cout << "Количество должно быть положительным числом!" << endl;
        return -1;
    }

    int actualIndex = productIndex - 1;
    int oldQuantity = quantities[actualIndex];
    quantities[actualIndex] = newQuantity;

    cout << "Количество товара '" << products[actualIndex]->getName()
        << "' изменено с " << oldQuantity << " на " << newQuantity << " шт." << endl;

    return newQuantity;
}

bool Basket::IsEmpty() const {
    return productCount == 0;
}

void Basket::ClearBasket() {
    clearProducts();
    cout << "Корзина очищена!" << endl;
}

int Basket::FindProductIndex(int article) const {
    for (int i = 0; i < productCount; i++) {
        if (products[i] != nullptr && products[i]->getArticle() == article) {
            return i;
        }
    }
    return -1;
}

Product* Basket::getProduct(int index) const {
    if (index < 0 || index >= productCount) {
        return nullptr;
    }
    return products[index];
}

int Basket::getQuantity(int index) const {
    if (index < 0 || index >= productCount) {
        return 0;
    }
    return quantities[index];
}



// 1. Дружественная функция для вывода корзины в поток
std::ostream& operator<<(std::ostream& os, const Basket& basket) {
    os << "\n=== СОДЕРЖИМОЕ КОРЗИНЫ ===" << endl;

    if (basket.productCount == 0) {  // Прямой доступ к приватному полю
        os << "Корзина пуста!" << endl;
        os << "=========================" << endl;
        return os;
    }

    os << left << setw(3) << "№" << setw(10) << "Артикул"
        << setw(20) << "Название" << setw(8) << "Цена"
        << setw(10) << "Кол-во" << setw(12) << "Сумма" << endl;

    os << "------------------------------------------------------------" << endl;

    double totalCost = 0.0;

    // Прямой доступ к приватным массивам
    for (int i = 0; i < basket.productCount; i++) {
        if (basket.products[i] != nullptr) {
            double itemTotal = basket.products[i]->getPrice() * basket.quantities[i];
            totalCost += itemTotal;

            string productName = basket.products[i]->getName();
            if (productName.length() > 18) {
                productName = productName.substr(0, 18) + "..";
            }

            os << left << setw(3) << (i + 1)
                << setw(10) << basket.products[i]->getArticle()
                << setw(20) << productName
                << setw(8) << basket.products[i]->getPrice()
                << setw(10) << basket.quantities[i]
                << setw(12) << itemTotal << endl;
        }
    }

    os << "------------------------------------------------------------" << endl;
    os << "ИТОГО: " << totalCost << " руб." << endl;
    os << "Общее количество товаров: " << basket.GetBasketCount() << " шт." << endl;
    os << "=========================" << endl;

    return os;
}

// 2. Дружественная функция для объединения двух корзин
Basket mergeBaskets(const Basket& basket1, const Basket& basket2) {
    Basket result;

    // Объединяем товары из первой корзины
    for (int i = 0; i < basket1.productCount; i++) {
        if (basket1.products[i] != nullptr) {
            // Ищем такой же товар в результате
            bool found = false;
            for (int j = 0; j < result.productCount; j++) {
                if (result.products[j] != nullptr &&
                    result.products[j]->getArticle() == basket1.products[i]->getArticle()) {
                    result.quantities[j] += basket1.quantities[i];
                    found = true;
                    break;
                }
            }

            if (!found && result.productCount < Basket::MAX_PRODUCTS) {
                result.products[result.productCount] = new Product(*(basket1.products[i]));
                result.quantities[result.productCount] = basket1.quantities[i];
                result.productCount++;
            }
        }
    }

    // Объединяем товары из второй корзины
    for (int i = 0; i < basket2.productCount; i++) {
        if (basket2.products[i] != nullptr) {
            // Ищем такой же товар в результате
            bool found = false;
            for (int j = 0; j < result.productCount; j++) {
                if (result.products[j] != nullptr &&
                    result.products[j]->getArticle() == basket2.products[i]->getArticle()) {
                    result.quantities[j] += basket2.quantities[i];
                    found = true;
                    break;
                }
            }

            if (!found && result.productCount < Basket::MAX_PRODUCTS) {
                result.products[result.productCount] = new Product(*(basket2.products[i]));
                result.quantities[result.productCount] = basket2.quantities[i];
                result.productCount++;
            }
        }
    }

    return result;
}

// 3. Дружественная функция для расчета скидки
double calculateBasketDiscount(const Basket& basket, double discountPercent) {
    if (discountPercent < 0 || discountPercent > 100) {
        cout << "Неверный процент скидки!" << endl;
        return 0.0;
    }

    double totalCost = 0.0;

    // Прямой доступ к приватным полям для расчета
    for (int i = 0; i < basket.productCount; i++) {
        if (basket.products[i] != nullptr) {
            totalCost += basket.products[i]->getPrice() * basket.quantities[i];
        }
    }

    double discountAmount = totalCost * (discountPercent / 100.0);
    double finalPrice = totalCost - discountAmount;

    cout << "\n=== РАСЧЕТ СКИДКИ ===" << endl;
    cout << "Общая стоимость: " << totalCost << " руб." << endl;
    cout << "Процент скидки: " << discountPercent << "%" << endl;
    cout << "Сумма скидки: " << discountAmount << " руб." << endl;
    cout << "Итоговая цена: " << finalPrice << " руб." << endl;
    cout << "====================" << endl;

    return discountAmount;
}

// 4. Дружественная функция для сравнения двух корзин
bool areBasketsEqual(const Basket& basket1, const Basket& basket2) {
    // Сначала проверяем количество товаров
    if (basket1.productCount != basket2.productCount) {
        return false;
    }

    // Проверяем каждый товар
    for (int i = 0; i < basket1.productCount; i++) {
        if (basket1.products[i] == nullptr || basket2.products[i] == nullptr) {
            return false;
        }

        // Сравниваем артикулы
        if (basket1.products[i]->getArticle() != basket2.products[i]->getArticle()) {
            return false;
        }

        // Сравниваем количества
        if (basket1.quantities[i] != basket2.quantities[i]) {
            return false;
        }
    }

    return true;
}

// 5. Дружественная функция для добавления товара
void addProductToBasket(Basket& basket, Product* product, int quantity) {
    if (basket.productCount >= Basket::MAX_PRODUCTS) {
        cout << "Корзина переполнена! Максимум " << Basket::MAX_PRODUCTS << " товаров." << endl;
        return;
    }

    if (quantity <= 0) {
        cout << "Количество должно быть положительным числом!" << endl;
        return;
    }

    // Ищем товар с таким же артикулом
    int existingIndex = -1;
    for (int i = 0; i < basket.productCount; i++) {
        if (basket.products[i] != nullptr &&
            basket.products[i]->getArticle() == product->getArticle()) {
            existingIndex = i;
            break;
        }
    }

    if (existingIndex != -1) {
        // Увеличиваем количество существующего товара
        basket.quantities[existingIndex] += quantity;
        cout << "Количество товара '" << product->getName()
            << "' увеличено на " << quantity << ". Теперь: "
            << basket.quantities[existingIndex] << " шт." << endl;
    }
    else {
        // Добавляем новый товар
        basket.products[basket.productCount] = new Product(*product);
        basket.quantities[basket.productCount] = quantity;
        basket.productCount++;

        cout << "Товар '" << product->getName()
            << "' добавлен в корзину в количестве " << quantity << " шт." << endl;
    }
}

// 6. Дружественная функция для проверки, содержит ли корзина товар
bool basketContainsProduct(const Basket& basket, int article) {
    for (int i = 0; i < basket.productCount; i++) {
        if (basket.products[i] != nullptr && basket.products[i]->getArticle() == article) {
            return true;
        }
    }
    return false;
}

// 7. Дружественная функция для копирования корзины с фильтром по цене
Basket copyBasketWithPriceFilter(const Basket& basket, int maxPrice) {
    Basket filteredBasket;

    for (int i = 0; i < basket.productCount; i++) {
        if (basket.products[i] != nullptr &&
            basket.products[i]->getPrice() <= maxPrice) {

            // Добавляем товар в отфильтрованную корзину
            if (filteredBasket.productCount < Basket::MAX_PRODUCTS) {
                filteredBasket.products[filteredBasket.productCount] =
                    new Product(*(basket.products[i]));
                filteredBasket.quantities[filteredBasket.productCount] =
                    basket.quantities[i];
                filteredBasket.productCount++;
            }
        }
    }

    return filteredBasket;
}
