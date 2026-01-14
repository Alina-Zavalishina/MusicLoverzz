#define _CRT_SECURE_NO_WARNINGS

#include "Basket.h"
#include <iostream>
#include <iomanip>
#include <algorithm>

using namespace std;

// Определение конфигурации корзины
const BasketConfig basketConfig = BasketConfig(150, 0.18, 70.0);

// Простые constexpr вычисления 
constexpr double calculateSimpleCost(double price, int quantity) {
    return price * quantity;
}

// Простые constexpr переменные
constexpr double SAMPLE_PRICE = 100.0;
constexpr int SAMPLE_QUANTITY = 3;
constexpr double SAMPLE_TOTAL = calculateSimpleCost(SAMPLE_PRICE, SAMPLE_QUANTITY);
constexpr double SAMPLE_DISCOUNT = 10.0;
constexpr double SAMPLE_DISCOUNTED_TOTAL = SAMPLE_TOTAL * (100.0 - SAMPLE_DISCOUNT) / 100.0;
constexpr double SAMPLE_TAX_RATE = 20.0;
constexpr double SAMPLE_TOTAL_WITH_TAX = SAMPLE_TOTAL * (1.0 + SAMPLE_TAX_RATE / 100.0);


constexpr SimpleBasket createSimpleBasketMSVC() {
    SimpleBasket basket;
    basket.addProduct(SimpleProduct(1, 100.0, 2));
    basket.addProduct(SimpleProduct(2, 50.0, 3));
    return basket;
}


static const SimpleBasket sampleBasket = []() {
    SimpleBasket b;
    b.addProduct(SimpleProduct(1, 100.0, 2));
    b.addProduct(SimpleProduct(2, 50.0, 3));
    return b;
    }();

// Конструктор
Basket::Basket() : productCount(0) {
    // Инициализация массива
    for (int i = 0; i < MAX_PRODUCTS; ++i) {
        products[i] = nullptr;
        quantities[i] = 0;
    }
    cout << "Вызван конструктор Basket" << endl;
}

// Конструктор копирования
Basket::Basket(const Basket& other) : productCount(other.productCount) {
    cout << "Вызван конструктор копирования Basket" << endl;

    for (int i = 0; i < MAX_PRODUCTS; ++i) {
        products[i] = nullptr;
        quantities[i] = 0;
    }

    for (int i = 0; i < productCount; ++i) {
        if (other.products[i] != nullptr) {
            products[i] = new Product(*(other.products[i]));
            quantities[i] = other.quantities[i];
        }
    }
}

// Оператор присваивания
Basket& Basket::operator=(const Basket& other) {
    cout << "Вызван оператор присваивания Basket" << endl;

    if (this != &other) {
        clearProducts();
        productCount = other.productCount;

        for (int i = 0; i < productCount; ++i) {
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
    for (int i = 0; i < productCount; ++i) {
        if (products[i] != nullptr) {
            delete products[i];
            products[i] = nullptr;
        }
        quantities[i] = 0;
    }
    productCount = 0;
}

// Глубокое копирование
void Basket::deepCopy(const Basket& other) {
    clearProducts();
    productCount = other.productCount;

    for (int i = 0; i < productCount; ++i) {
        if (other.products[i] != nullptr) {
            products[i] = new Product(*(other.products[i]));
            quantities[i] = other.quantities[i];
        }
    }
}

// Вычисление стоимости корзины - ВЫЗЫВАЕТ ШАБЛОННУЮ ФУНКЦИЮ
double Basket::GetBasketCost() const {
    return calculateBasketTotal<double>(
        [](const Product& product, int quantity) -> double {
            return product.getPrice() * quantity;
        }
    );
}

// Получение общего количества товаров
unsigned int Basket::GetBasketCount() const {
    unsigned int totalCount = 0;
    for (int i = 0; i < productCount; ++i) {
        totalCount += quantities[i];
    }
    return totalCount;
}

// Расчет стоимости с налогом - ВЫЗЫВАЕТ ШАБЛОННУЮ ФУНКЦИЮ
double Basket::calculateCostWithTax(double taxRate) const {
    if (taxRate < 0) {
        cerr << "Ошибка: налог не может быть отрицательным!" << endl;
        return GetBasketCost();
    }

    return calculateBasketTotal<double>(
        [taxRate](const Product& product, int quantity) -> double {
            double price = product.getPrice();
            double itemTotal = price * quantity;
            return itemTotal * (1.0 + taxRate / 100.0);
        }
    );
}

// Расчет стоимости со скидкой - ВЫЗЫВАЕТ ШАБЛОННУЮ ФУНКЦИЮ
double Basket::calculateCostWithDiscount(double discountPercent) const {
    if (discountPercent < 0 || discountPercent > 100) {
        cerr << "Ошибка: скидка должна быть от 0 до 100%!" << endl;
        return GetBasketCost();
    }

    return calculateBasketTotal<double>(
        [discountPercent](const Product& product, int quantity) -> double {
            double price = product.getPrice();
            double itemTotal = price * quantity;
            return itemTotal * (100.0 - discountPercent) / 100.0;
        }
    );
}

// Метод, демонстрирующий возврат int - ВЫЗЫВАЕТ ШАБЛОННУЮ ФУНКЦИЮ
int Basket::calculateTotalQuantity() const {
    return calculateBasketTotal<int>(
        [](const Product& product, int quantity) -> int {
            return quantity;
        }
    );
}

// Отображение корзины
void Basket::DisplayBasket() const {
    cout << *this;
}

// Добавление товара в корзину
void Basket::AddBasket(Product* product, int quantity) {
    addProductToBasket(*this, product, quantity);
}

// Удаление товара из корзины
void Basket::DeleteBasket(int productIndex) {
    if (productIndex < 1 || productIndex > productCount) {
        cout << "Неверный номер товара!" << endl;
        return;
    }

    int actualIndex = productIndex - 1;
    string productName = products[actualIndex]->getName();
    delete products[actualIndex];

    for (int i = actualIndex; i < productCount - 1; ++i) {
        products[i] = products[i + 1];
        quantities[i] = quantities[i + 1];
    }

    products[productCount - 1] = nullptr;
    quantities[productCount - 1] = 0;
    productCount--;

    cout << "Товар '" << productName << "' удален из корзины." << endl;
}

// Изменение количества товара
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

// Проверка пустоты корзины
bool Basket::IsEmpty() const {
    return productCount == 0;
}

// Очистка корзины
void Basket::ClearBasket() {
    clearProducts();
    cout << "Корзина очищена!" << endl;
}

// Поиск индекса товара по артикулу
int Basket::FindProductIndex(int article) const {
    for (int i = 0; i < productCount; ++i) {
        if (products[i] != nullptr && products[i]->getArticle() == article) {
            return i;
        }
    }
    return -1;
}

// Получение товара по индексу
Product* Basket::getProduct(int index) const {
    if (index < 0 || index >= productCount) {
        return nullptr;
    }
    return products[index];
}

// Получение количества товара по индексу
int Basket::getQuantity(int index) const {
    if (index < 0 || index >= productCount) {
        return 0;
    }
    return quantities[index];
}

// Дружественная функция для вывода корзины в поток
std::ostream& operator<<(std::ostream& os, const Basket& basket) {
    os << "\n=== СОДЕРЖИМОЕ КОРЗИНЫ ===" << endl;

    if (basket.productCount == 0) {
        os << "Корзина пуста!" << endl;
        os << "=========================" << endl;
        return os;
    }

    os << left << setw(3) << "№" << setw(10) << "Артикул"
        << setw(20) << "Название" << setw(8) << "Цена"
        << setw(10) << "Кол-во" << setw(12) << "Сумма" << endl;

    os << "-------------------------------------------------------------" << endl;

    double totalCost = 0.0;

    for (int i = 0; i < basket.productCount; ++i) {
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
                << setw(8) << fixed << setprecision(2) << basket.products[i]->getPrice()
                << setw(10) << basket.quantities[i]
                << setw(12) << fixed << setprecision(2) << itemTotal << endl;
        }
    }

    os << "-------------------------------------------------------------" << endl;
    os << "ИТОГО: " << fixed << setprecision(2) << totalCost << " руб." << endl;
    os << "Общее количество товаров: " << basket.GetBasketCount() << " шт." << endl;
    os << "=========================" << endl;

    return os;
}

// Дружественная функция для объединения двух корзин
Basket mergeBaskets(const Basket& basket1, const Basket& basket2) {
    Basket result;

    // Объединяем товары из первой корзины
    for (int i = 0; i < basket1.productCount; ++i) {
        if (basket1.products[i] != nullptr) {
            bool found = false;

            for (int j = 0; j < result.productCount; ++j) {
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
    for (int i = 0; i < basket2.productCount; ++i) {
        if (basket2.products[i] != nullptr) {
            bool found = false;

            for (int j = 0; j < result.productCount; ++j) {
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

// Дружественная функция для расчета скидки
double calculateBasketDiscount(const Basket& basket, double discountPercent) {
    double discountedCost = basket.calculateCostWithDiscount(discountPercent);
    double originalCost = basket.GetBasketCost();
    double discountAmount = originalCost - discountedCost;

    cout << "\n=== РАСЧЕТ СКИДКИ ===" << endl;
    cout << "Общая стоимость: " << fixed << setprecision(2) << originalCost << " руб." << endl;
    cout << "Процент скидки: " << discountPercent << "%" << endl;
    cout << "Сумма скидки: " << fixed << setprecision(2) << discountAmount << " руб." << endl;
    cout << "Итоговая цена: " << fixed << setprecision(2) << discountedCost << " руб." << endl;
    cout << "====================" << endl;

    return discountAmount;
}

// Дружественная функция для сравнения двух корзин
bool areBasketsEqual(const Basket& basket1, const Basket& basket2) {
    if (basket1.productCount != basket2.productCount) {
        return false;
    }

    for (int i = 0; i < basket1.productCount; ++i) {
        if (basket1.products[i] == nullptr || basket2.products[i] == nullptr) {
            return false;
        }

        if (basket1.products[i]->getArticle() != basket2.products[i]->getArticle()) {
            return false;
        }

        if (basket1.quantities[i] != basket2.quantities[i]) {
            return false;
        }
    }

    return true;
}

// Дружественная функция для добавления товара
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
    for (int i = 0; i < basket.productCount; ++i) {
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

// Дружественная функция для проверки наличия товара
bool basketContainsProduct(const Basket& basket, int article) {
    for (int i = 0; i < basket.productCount; ++i) {
        if (basket.products[i] != nullptr && basket.products[i]->getArticle() == article) {
            return true;
        }
    }
    return false;
}

// Дружественная функция для копирования корзины с фильтром по цене
Basket copyBasketWithPriceFilter(const Basket& basket, int maxPrice) {
    Basket filteredBasket;

    for (int i = 0; i < basket.productCount; ++i) {
        if (basket.products[i] != nullptr &&
            basket.products[i]->getPrice() <= maxPrice) {
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
