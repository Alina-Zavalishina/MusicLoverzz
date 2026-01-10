#include <iostream>
#include <limits>
#include <locale>
#include "Product.h"
#include "Search.h"
#include "Basket.h"
#include "Order.h"
#include "ProviderOrder.h"
#include "SalesReport.h"
#include "Shop.h"
#include "Review.h"
#include "Client.h"

using namespace std;

Basket userBasket;
Shop musicShop("Музыкальный магазин 'MusicLoverzz'", "г. Барнаул, ул. Ленина, 1");
Client currentClient("USER001");

void addProducts(); // Добавление новых товаров в систему
void adminMode(); // Режим администратора
void userMode();// Режим пользователя
bool checkAdminPassword();// Проверка пароля администратора
void userSearchMode(); // Поиск товаров в пользовательском режиме
void userBasketMode(); // Управление корзиной покупок
void userOrderMode(); // Оформление заказа
void providerOrderMode();// Заказ товаров у поставщиков
void addProductToBasketByArticle(Basket& basket);// Добавление товара в корзину по артикулу
void shopManagementMode();// Управление магазином

int main() {
    setlocale(LC_ALL, "ru_RU.UTF-8");

    int choice;
    do {
        cout << "\n=== ДОБРО ПОЖАЛОВАТЬ В 'MUSICLOVERZZ'! == = " << endl;
        cout << "Выберите режим работы:" << endl;
        cout << "1. Режим администратора" << endl;
        cout << "2. Режим пользователя" << endl;
        cout << "3. Управление магазином" << endl;
        cout << "0. Выход" << endl;
        cout << "Выберите действие: ";
        cin >> choice;

        switch (choice) {
        case 1:
            adminMode();
            break;
        case 2:
            userMode();
            break;
        case 3:
            shopManagementMode();
            break;
        case 0:
            cout << "Выход из программы..." << endl;
            break;
        default:
            cout << "Неверный выбор!" << endl;
        }
    } while (choice != 0);

    return 0;
}

bool checkAdminPassword() {
    string password;
    cout << "Введите пароль администратора: ";
    cin >> password;

    if (password == "26102006") {
        cout << "Доступ разрешен!" << endl;
        return true;
    }
    else {
        cout << "Неверный пароль!" << endl;
        return false;
    }
}
// Заказ товаров у поставщиков
void providerOrderMode() {
    int choice;
    ProviderOrder providerOrder;

    do {
        cout << "\n=== ЗАКАЗ У ПОСТАВЩИКА ===" << endl;
        cout << "1. Добавить товар в заказ (из заказов клиентов)" << endl;
        cout << "2. Отправить заказ поставщику" << endl;
        cout << "3. Обновить информацию о товарах" << endl;
        cout << "0. Назад" << endl;
        cout << "Выберите действие: ";
        cin >> choice;

        switch (choice) {
        case 1:
            providerOrder.AddProvider();
            break;
        case 2:
            providerOrder.sending();
            break;
        case 3:
            providerOrder.NewInfoStock();
            break;
        case 0:
            cout << "Выход из режима заказа поставщика..." << endl;
            break;
        default:
            cout << "Неверный выбор!" << endl;
            cin.clear();
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
        }
    } while (choice != 0);
}

void adminMode() {
    if (!checkAdminPassword()) {
        return;
    }

    int choice;
    do {
        cout << "\n=== РЕЖИМ АДМИНИСТРАТОРА ===" << endl;
        cout << "1. Добавить товар" << endl;
        cout << "2. Показать информацию о товарах" << endl;
        cout << "3. Удалить товары со склада" << endl;
        cout << "4. Редактировать товары" << endl;
        cout << "5. Найти товары по артикулу" << endl;
        cout << "6. Заказ у поставщика" << endl;
        cout << "0. Выход в главное меню" << endl;
        cout << "Выберите действие: ";
        cin >> choice;

        switch (choice) {
        case 1:
            addProducts();
            break;
        case 2:
            Product::DisplayProductsTable("products.txt");
            break;
        case 3:
            Product::DeleteProductsFromFile("products.txt");
            break;
        case 4:
            Product::EditProductsInFile("products.txt");
            break;
        case 5:
            Product::FindProductsByArticle("products.txt");
            break;
        case 6:
            providerOrderMode();
            break;
        case 0:
            cout << "Выход из режима администратора..." << endl;
            break;
        default:
            cout << "Неверный выбор!" << endl;
        }
    } while (choice != 0);
}

void userSearchMode() {
    int choice;
    Search search;

    do {
        cout << "\n=== РЕЖИМ ПОИСКА ===" << endl;
        cout << "1. Поиск по названию" << endl;
        cout << "2. Поиск по артикулу" << endl;
        cout << "3. Поиск по бренду" << endl;
        cout << "4. Поиск по стране-производителю" << endl;
        cout << "5. Поиск по категории" << endl;
        cout << "6. Поиск по типу товара" << endl;
        cout << "7. Поиск по цене" << endl;
        cout << "8. Универсальный поиск" << endl;
        cout << "9. Показать историю поиска" << endl;
        cout << "0. Назад" << endl;
        cout << "Выберите действие: ";
        cin >> choice;

        // Проверка ввода для основного меню
        if (cin.fail()) {
            cin.clear();
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
            cout << "Неверный ввод! Пожалуйста, введите число от 0 до 9." << endl;
            continue;
        }

        cin.ignore(numeric_limits<streamsize>::max(), '\n');

        string searchString;
        int searchArticle, minPrice, maxPrice;

        switch (choice) {
        case 1:
            cout << "Введите название для поиска: ";
            getline(cin, searchString);
            search.SearchByName("products.txt", searchString);
            search.DisplayResults();
            break;
        case 2:
            // Защищенный ввод артикула
            while (true) {
                cout << "Введите артикул для поиска: ";
                cin >> searchArticle;
                if (cin.fail()) {
                    cin.clear();
                    cin.ignore(numeric_limits<streamsize>::max(), '\n');
                    cout << "Неверный ввод! Пожалуйста, введите целое число." << endl;
                    continue;
                }
                break;
            }
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
            search.SearchByArticle("products.txt", searchArticle);
            search.DisplayResults();
            break;
        case 3:
            cout << "Введите бренд для поиска: ";
            getline(cin, searchString);
            search.SearchByBrand("products.txt", searchString);
            search.DisplayResults();
            break;
        case 4:
            cout << "Введите страну-производитель для поиска: ";
            getline(cin, searchString);
            search.SearchByCountry("products.txt", searchString);
            search.DisplayResults();
            break;
        case 5:
            cout << "Введите категорию для поиска: ";
            getline(cin, searchString);
            search.SearchByCategory("products.txt", searchString);
            search.DisplayResults();
            break;
        case 6:
            cout << "Введите тип товара для поиска: ";
            getline(cin, searchString);
            search.SearchByType("products.txt", searchString);
            search.DisplayResults();
            break;
        case 7:
            // Защищенный ввод цен
            while (true) {
                cout << "Введите минимальную цену: ";
                cin >> minPrice;
                if (cin.fail()) {
                    cin.clear();
                    cin.ignore(numeric_limits<streamsize>::max(), '\n');
                    cout << "Неверный ввод! Пожалуйста, введите целое число." << endl;
                    continue;
                }
                break;
            }
            while (true) {
                cout << "Введите максимальную цену: ";
                cin >> maxPrice;
                if (cin.fail()) {
                    cin.clear();
                    cin.ignore(numeric_limits<streamsize>::max(), '\n');
                    cout << "Неверный ввод! Пожалуйста, введите целое число." << endl;
                    continue;
                }
                break;
            }
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
            search.SearchByPrice("products.txt", minPrice, maxPrice);
            search.DisplayResults();
            break;
        case 8:
            Search::UniversalSearch("products.txt");
            break;
        case 9:
            search.DisplaySearchHistory();
            break;
        case 0:
            cout << "Выход из режима поиска..." << endl;
            break;
        default:
            cout << "Неверный выбор!" << endl;
        }
    } while (choice != 0);
}

void userBasketMode() {
    int choice;

    do {
        cout << "\n=== КОРЗИНА ТОВАРОВ ===" << endl;
        cout << "1. Посмотреть цену товаров в корзине" << endl;
        cout << "2. Количество товаров в корзине" << endl;
        cout << "3. Показать содержимое корзины" << endl;
        cout << "4. Добавить товар в корзину по артикулу" << endl;
        cout << "5. Удалить товар из корзины" << endl;
        cout << "6. Изменить количество товара в корзине" << endl;
        cout << "7. Очистить корзину" << endl;
        cout << "0. Назад" << endl;
        cout << "Выберите действие: ";
        cin >> choice;

        switch (choice) {
        case 1: {
            double totalCost = userBasket.GetBasketCost();
            cout << "\n=== ОБЩАЯ СТОИМОСТЬ КОРЗИНЫ ===" << endl;
            cout << "Общая стоимость: " << totalCost << " руб." << endl;
            cout << "===============================" << endl;
            break;
        }
        case 2: {
            unsigned int totalCount = userBasket.GetBasketCount();
            cout << "\n=== КОЛИЧЕСТВО ТОВАРОВ В КОРЗИНЕ ===" << endl;
            cout << "Общее количество товаров: " << totalCount << " шт." << endl;
            cout << "====================================" << endl;
            break;
        }
        case 3:
            userBasket.DisplayBasket();
            break;
        case 4:
            addProductToBasketByArticle(userBasket);
            break;
        case 5: {
            if (userBasket.IsEmpty()) {
                cout << "Корзина пуста!" << endl;
                break;
            }
            userBasket.DisplayBasket();
            int productIndex;
            cout << "Введите номер товара для удаления: ";
            cin >> productIndex;
            userBasket.DeleteBasket(productIndex);
            break;
        }
        case 6: {
            if (userBasket.IsEmpty()) {
                cout << "Корзина пуста!" << endl;
                break;
            }
            userBasket.DisplayBasket();
            int productIndex, newQuantity;
            cout << "Введите номер товара для изменения: ";
            cin >> productIndex;
            cout << "Введите новое количество: ";
            cin >> newQuantity;
            userBasket.UpdateQuantity(productIndex, newQuantity);
            break;
        }
        case 7:
            userBasket.ClearBasket();
            cout << "Корзина очищена!" << endl;
            break;
        case 0:
            cout << "Выход из режима корзины..." << endl;
            break;
        default:
            cout << "Неверный выбор!" << endl;
        }
    } while (choice != 0);
}

void userOrderMode() {
    cout << "\n=== ОФОРМЛЕНИЕ ЗАКАЗА ===" << endl;

    // Проверка пустой корзины
    if (userBasket.IsEmpty()) {
        cout << "Корзина пуста! Добавьте товары перед оформлением заказа." << endl;
        cout << "Перейдите в раздел 'Корзина товаров' для добавления товаров." << endl;
        return;
    }

    // Показываем содержимое корзины
    cout << "\nСодержимое вашей корзины:" << endl;
    userBasket.DisplayBasket();

    // Подтверждение оформления заказа
    char confirmation;
    cout << "\nВы уверены, что хотите оформить заказ? (y/n): ";
    cin >> confirmation;

    if (confirmation != 'y' && confirmation != 'Y') {
        cout << "Оформление заказа отменено." << endl;
        return;
    }

    string clientId = "USER001";

    // СОЗДАЕМ КОПИЮ КОРЗИНЫ: потому что оригинальная корзина очистится при оформлении
    Basket tempBasket;
    for (int i = 0; i < userBasket.getProductCount(); i++) {
        Product* product = userBasket.getProduct(i);
        int quantity = userBasket.getQuantity(i);
        if (product != nullptr) {
            tempBasket.AddBasket(product, quantity); // Копируем товары во временную корзину
        }
    }

    // СОЗДАЕМ ОБЪЕКТ ЗАКАЗА: из временной корзины (для истории клиента)
    Order* clientOrder = new Order(clientId, tempBasket);

    // ОФОРМЛЯЕМ ЗАКАЗ В МАГАЗИНЕ: это очистит оригинальную корзину userBasket
    musicShop.CreateOrder(clientId, userBasket);

    // СОХРАНЯЕМ ЗАКАЗ В ИСТОРИИ КЛИЕНТА
    currentClient.GetOrderHistory(clientOrder);

    cout << "\n==================================" << endl;
    cout << "ЗАКАЗ УСПЕШНО ОФОРМЛЕН!" << endl;
    cout << "Номер заказа: " << clientOrder->getNumberOrder() << endl;
    cout << "Дата: " << clientOrder->getDateOrder() << endl;
    cout << "Сумма: " << clientOrder->getCostOrder() << " руб." << endl;
    cout << "Количество товаров: " << clientOrder->getCountOrder() << " шт." << endl;
    cout << "==================================" << endl;

    // Показываем состав заказа
    cout << "\nСостав заказа:" << endl;
    cout << "--------------" << endl;
    for (int i = 0; i < clientOrder->getItemCount(); i++) {
        Product* product = clientOrder->getOrderItem(i);
        int quantity = clientOrder->getQuantity(i);
        if (product != nullptr) {
            cout << "- " << product->getName()
                << " (арт. " << product->getArticle() << ")"
                << " - " << quantity << " шт."
                << " x " << product->getPrice() << " руб." << endl;
        }
    }

    cout << "\nТеперь вы можете оставить отзыв о товарах" << endl;
    cout << "в разделе 'Оставить отзыв'." << endl;
}

void userMode() {
    int choice;
    do {
        cout << "\n=== РЕЖИМ ПОЛЬЗОВАТЕЛЯ ===" << endl;
        cout << "1. Поиск товаров" << endl;
        cout << "2. Корзина товаров" << endl;
        cout << "3. Оформить заказ" << endl;
        cout << "4. Оставить отзыв" << endl;
        cout << "0. Выход в главное меню" << endl;
        cout << "Выберите действие: ";
        cin >> choice;

        switch (choice) {
        case 1:
            userSearchMode();
            break;
        case 2:
            userBasketMode();
            break;
        case 3:
            userOrderMode();
            break;
        case 4:
            currentClient.GetReview();
            break;
        case 0:
            cout << "Выход из режима пользователя..." << endl;
            break;
        default:
            cout << "Неверный выбор!" << endl;
        }
    } while (choice != 0);
}

void shopManagementMode() {
    if (!checkAdminPassword()) {
        return;
    }

    int choice;
    do {
        cout << "\n=== УПРАВЛЕНИЕ МАГАЗИНОМ ===" << endl;
        cout << "1. Информация о магазине" << endl;
        cout << "2. Показать каталог товаров" << endl;
        cout << "3. Добавить товар в каталог" << endl;
        cout << "4. Удалить товар из каталога" << endl;
        cout << "5. Показать активные заказы" << endl;
        cout << "6. Показать заказы поставщикам" << endl;
        cout << "7. Создать отчет по продажам" << endl;
        cout << "8. Изменить отчет по продажам" << endl;
        cout << "9. Показать отчеты по продажам" << endl;
        cout << "10. Показать все отзывы" << endl;
        cout << "11. Расчет средней оценки товара" << endl;
        cout << "0. Назад" << endl;
        cout << "Выберите действие: ";
        cin >> choice;

        switch (choice) {
        case 1:
            musicShop.ShowShopInfo();
            break;
        case 2:
            musicShop.DisplayCatalog();
            break;
        case 3:
            musicShop.AddProduct();
            break;
        case 4:
            musicShop.DeleteProduct();
            break;
        case 5:
            musicShop.DisplayOrders();
            break;
        case 6:
            musicShop.DisplayProviderOrders();
            break;
        case 7:
            musicShop.CreateSalesReport();
            break;
        case 8:
            musicShop.RemakeSalesReport();
            break;
        case 9:
            musicShop.DisplaySalesReports();
            break;
        case 10:
            Review::PublicReview();
            break;
        case 11: {
            cout << "\n=== РАСЧЕТ СРЕДНЕЙ ОЦЕНКИ ТОВАРА ===" << endl;
            string productName;
            cout << "Введите название товара: ";
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
            getline(cin, productName);
            Review::AverageRating(productName);
            break;
        }
        case 0:
            cout << "Выход из управления магазином..." << endl;
            break;
        default:
            cout << "Неверный выбор!" << endl;
        }
    } while (choice != 0);
}

void addProducts() {
    cout << "\n--- ДОБАВЛЕНИЕ ТОВАРОВ ---" << endl;

    int productCount;
    while (true) {
        cout << "Введите количество товаров для добавления: ";
        cin >> productCount;

        if (cin.fail()) {
            cin.clear();
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
            cout << "Ошибка! Введите целое число." << endl;
            continue;
        }

        if (productCount < 0) {
            cout << "Ошибка! Количество не может быть отрицательным. Попробуйте снова." << endl;
            continue;
        }

        if (productCount == 0) {
            cout << "Добавление товаров отменено." << endl;
            return;
        }

        if (!Product::validateNumber(productCount, 10)) {
            cout << "Ошибка! Количество товаров не должно превышать 10 цифр." << endl;
            continue;
        }

        break;
    }

    cin.ignore(numeric_limits<streamsize>::max(), '\n');

    for (int i = 0; i < productCount; i++) {
        cout << "\n=== Товар " << (i + 1) << " из " << productCount << " ===" << endl;
        Product product;
        product.InputFromKeyboard();

        cout << "\nТовар успешно добавлен!" << endl;
        product.DisplayInfo();
        product.WriteToFile("products.txt");

        cout << "------------------------------------------" << endl;
    }

    cout << "\nУспешно добавлено " << productCount << " товаров!" << endl;
}

void addProductToBasketByArticle(Basket& basket) {
    int article, quantity;
    cout << "Введите артикул товара: ";
    cin >> article;
    cout << "Введите количество: ";
    cin >> quantity;

    if (quantity <= 0) {
        cout << "Количество должно быть положительным числом!" << endl;
        return;
    }

    ifstream file("products.txt");
    if (!file.is_open()) {
        cout << "Файл с товарами не найден!" << endl;
        return;
    }

    string line;
    bool found = false;

    while (getline(file, line)) {
        if (line.empty()) continue;

        stringstream ss(line);
        string token;
        string tokens[10];
        int tokenCount = 0;

        while (getline(ss, token, '|') && tokenCount < 10) {
            tokens[tokenCount++] = token;
        }

        if (tokenCount == 10) {
            try {
                int currentArticle = stoi(tokens[0]);
                if (currentArticle == article) {
                    found = true;
                    Product* product = new Product(
                        stoi(tokens[0]),
                        tokens[1],
                        tokens[2],
                        tokens[3],
                        tokens[4],
                        tokens[5],
                        tokens[6],
                        stoi(tokens[7]),
                        stoi(tokens[9])
                    );

                    basket.AddBasket(product, quantity);
                    cout << "Товар '" << tokens[1] << "' добавлен в корзину!" << endl;
                    break;
                }
            }
            catch (const exception& e) {
                continue;
            }
        }
    }

    file.close();

    if (!found) {
        cout << "Товар с артикулом " << article << " не найден!" << endl;
    }
}