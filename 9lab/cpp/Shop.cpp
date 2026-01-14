#define _CRT_SECURE_NO_WARNINGS

#include "Shop.h"
#include <iostream>
#include <iomanip>
#include <limits>
#include <fstream>
#include <sstream>
#include <ctime>
#include <direct.h>

using namespace std;

//конструктор по умолчанию
Shop::Shop() : shopName("Музыкальный магазин 'MusicLoverzz'"),
addressShop("г. Барнаул, ул. Ленина, 1"),
productCount(0), orderCount(0), providerOrderCount(0), salesReportCount(0) {
    initializeArrays();
    LoadProductsFromFile("products.txt");
    LoadSalesReports();
}
// Конструктор с параметрами класса Shop
Shop::Shop(const std::string& name, const std::string& address) :
    shopName(name), addressShop(address),
    productCount(0), orderCount(0), providerOrderCount(0), salesReportCount(0) {
    initializeArrays();
    LoadProductsFromFile("products.txt");
    LoadSalesReports();
}
// Деструктор класса Shop - освобождает всю динамически выделенную память
Shop::~Shop() {
    // Освобождаем память всех товаров в каталоге
    for (int i = 0; i < productCount; i++) {
        delete catalogProducts[i];// удаляем каждый объект Product
    }
    // Освобождаем память всех активных заказов клиентов
    for (int i = 0; i < orderCount; i++) {
        delete activeOrders[i];// удаляем каждый объект Order
    }
    // Освобождаем память всех заказов поставщикам
    for (int i = 0; i < providerOrderCount; i++) {
        delete providerOrders[i];// удаляем каждый объект ProviderOrder
    }
    // Освобождаем память всех отчетов о продажах
    for (int i = 0; i < salesReportCount; i++) {
        delete salesReports[i];// удаляем каждый объект SalesReport
    }
}
// Метод инициализации массивов
void Shop::initializeArrays() {
    // Инициализируем массив товаров
    for (int i = 0; i < MAX_PRODUCTS; i++) {
        catalogProducts[i] = nullptr;// обнуляем указатели на товары
    }
    // Инициализируем массив заказов клиентов
    for (int i = 0; i < MAX_ORDERS; i++) {
        activeOrders[i] = nullptr;// обнуляем указатели на заказы
    }
    // Инициализируем массив заказов поставщикам
    for (int i = 0; i < MAX_PROVIDER_ORDERS; i++) {
        providerOrders[i] = nullptr;// обнуляем указатели на заказы поставщикам
    }
    // Инициализируем массив отчетов о продажах
    for (int i = 0; i < MAX_SALES_REPORTS; i++) {
        salesReports[i] = nullptr;// обнуляем указатели на отчеты
    }
}

void Shop::LoadProductsFromFile(const std::string& filename) {
    ifstream file(filename);
    if (!file.is_open()) {
        cout << "Файл товаров не найден! Создайте сначала товары через режим администратора." << endl;
        return;
    }

    string line;
    while (getline(file, line) && productCount < MAX_PRODUCTS) {
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
                int article = stoi(tokens[0]);
                string name = tokens[1];
                string brand = tokens[2];
                string country = tokens[3];
                string category = tokens[4];
                string type = tokens[5];
                string description = tokens[6];
                unsigned int quantity = stoi(tokens[7]);
                bool status = (tokens[8] == "1");
                int price = stoi(tokens[9]);
                //проверяет находится ли товар в файле
                bool exists = false;
                for (int i = 0; i < productCount; i++) {
                    if (catalogProducts[i] && catalogProducts[i]->getArticle() == article) {
                        exists = true;
                        break;
                    }
                }

                if (!exists) {
                    catalogProducts[productCount] = new Product(
                        article, name, brand, country, category,
                        type, description, quantity, price
                    );
                    productCount++;
                }
            }
            catch (const exception& e) {
                continue;
            }
        }
    }
    file.close();
}
//отчет по продажам за последние 7 дней
void Shop::LoadSalesReports() {

    for (int daysAgo = 0; daysAgo < 7 && salesReportCount < MAX_SALES_REPORTS; daysAgo++) {
        time_t now = time(0) - (daysAgo * 24 * 60 * 60);// time(0) - текущее время в секундах
        // daysAgo * 24 * 60 * 60 - отступ в секундах (дни × часы × минуты × секунды)

#ifdef _WIN32
        struct tm timeinfo;
        localtime_s(&timeinfo, &now);
        char buffer[80];
        strftime(buffer, 80, "%d.%m.%Y", &timeinfo);
#else
        tm* localTime = localtime(&now);
        char buffer[80];
        strftime(buffer, 80, "%d.%m.%Y", localTime);
#endif

        string filename = "sales_report_" + string(buffer) + ".txt";
        ifstream file(filename);
        if (file.is_open()) {
            SalesReport* report = new SalesReport(string(buffer));// создаем отчет с датой
            if (report->LoadFromFile(filename)) {
                salesReports[salesReportCount] = report;// сохраняем в массив
                salesReportCount++;
            }
            else {
                delete report;// если загрузка не удалась - удаляем объект
            }
            file.close();
        }
    }
}

void Shop::AddProduct() {
    if (productCount >= MAX_PRODUCTS) {
        cout << "Каталог товаров переполнен! Максимум " << MAX_PRODUCTS << " товаров." << endl;
        return;
    }

    cout << "\n=== ДОБАВЛЕНИЕ НОВОГО ТОВАРА ===" << endl;
    Product* newProduct = new Product();
    newProduct->InputFromKeyboard();

    for (int i = 0; i < productCount; i++) {
        if (catalogProducts[i]->getArticle() == newProduct->getArticle()) {
            cout << "Ошибка: товар с артикулом " << newProduct->getArticle() << " уже существует!" << endl;
            delete newProduct;
            return;
        }
    }

    catalogProducts[productCount] = newProduct;
    productCount++;
    newProduct->WriteToFile("products.txt");

    cout << "Товар успешно добавлен в каталог и файл!" << endl;
}

void Shop::DeleteProduct() {
    if (productCount == 0) {
        cout << "Каталог товаров пуст!" << endl;
        return;
    }

    DisplayCatalog();

    int articleToDelete;
    cout << "Введите артикул товара для удаления: ";
    cin >> articleToDelete;

    if (cin.fail()) {
        cin.clear();
        cin.ignore(numeric_limits<streamsize>::max(), '\n');
        cout << "Ошибка ввода!" << endl;
        return;
    }

    int index = findProductIndex(articleToDelete);
    if (index == -1) {
        cout << "Товар с артикулом " << articleToDelete << " не найден!" << endl;
        return;
    }

    Product::DeleteProductByArticle("products.txt", articleToDelete);

    string productName = catalogProducts[index]->getName();
    delete catalogProducts[index];

    for (int i = index; i < productCount - 1; i++) {
        catalogProducts[i] = catalogProducts[i + 1];
    }
    catalogProducts[productCount - 1] = nullptr;
    productCount--;

    cout << "Товар '" << productName << "' успешно удален из каталога и файла!" << endl;
}

void Shop::CreateOrder(const std::string& clientId, Basket& basket) {
    if (orderCount >= MAX_ORDERS) {
        cout << "Достигнуто максимальное количество активных заказов!" << endl;
        return;
    }

    if (basket.IsEmpty()) {
        cout << "Корзина пуста!" << endl;
        return;
    }

    for (int i = 0; i < basket.getProductCount(); i++) {
        Product* basketProduct = basket.getProduct(i);
        int basketQuantity = basket.getQuantity(i);

        int productIndex = findProductIndex(basketProduct->getArticle());
        if (productIndex == -1) {
            cout << "Товар '" << basketProduct->getName() << "' не найден в каталоге!" << endl;
            return;
        }

        if (catalogProducts[productIndex]->getQuantityStock() < basketQuantity) {
            cout << "Недостаточно товара '" << basketProduct->getName()
                << "' на складе. Доступно: " << catalogProducts[productIndex]->getQuantityStock()
                << ", требуется: " << basketQuantity << endl;
            return;
        }
    }

    Order* newOrder = new Order(clientId, basket);

    if (newOrder->Payment()) {
        newOrder->ChangeStatus("Оплачен");
        activeOrders[orderCount] = newOrder;// добавляем в массив активных заказов
        orderCount++;
        // Уменьшаем количество товаров на складе
        for (int i = 0; i < basket.getProductCount(); i++) {
            Product* basketProduct = basket.getProduct(i);
            int basketQuantity = basket.getQuantity(i);
            updateProductStock(basketProduct->getArticle(), -basketQuantity);
        }

        newOrder->SaveToFile();// Записывает заказ в файл orders.txt для истории
        cout << "\n=== ЗАКАЗ УСПЕШНО ОФОРМЛЕН И ДОБАВЛЕН В СИСТЕМУ ===" << endl;
        newOrder->InfoOrder();
        basket.ClearBasket();
    }
    else {
        newOrder->ChangeStatus("Ошибка оплаты");
        cout << "Ошибка при оформлении заказа!" << endl;
        delete newOrder;
    }
}

void Shop::ShowShopInfo() const {
    cout << "\n=== ИНФОРМАЦИЯ О МАГАЗИНЕ ===" << endl;
    cout << "Название: " << shopName << endl;
    cout << "Адрес: " << addressShop << endl;
    cout << "Количество товаров в каталоге: " << productCount << endl;
    cout << "Активных заказов: " << orderCount << endl;
    cout << "Заказов у поставщиков: " << providerOrderCount << endl;
    cout << "Отчетов по продажам: " << salesReportCount << endl;
    cout << "=================================" << endl;
}

void Shop::DisplayCatalog() const {
    cout << "\n=== КАТАЛОГ ТОВАРОВ МАГАЗИНА ===" << endl;
    if (productCount == 0) {
        cout << "Каталог пуст!" << endl;
        return;
    }

    cout << "Всего товаров: " << productCount << endl;
    for (int i = 0; i < productCount; i++) {
        cout << "\n--- Товар " << (i + 1) << " ---" << endl;
        catalogProducts[i]->DisplayInfo();
    }
}

void Shop::DisplayOrders() const {
    cout << "\n=== АКТИВНЫЕ ЗАКАЗЫ МАГАЗИНА ===" << endl;
    if (orderCount == 0) {
        cout << "Нет активных заказов!" << endl;
        cout << "Сначала оформите заказы через режим пользователя." << endl;
        return;
    }

    cout << "Всего активных заказов: " << orderCount << endl;
    for (int i = 0; i < orderCount; i++) {
        cout << "\n--- Заказ " << (i + 1) << " ---" << endl;
        activeOrders[i]->InfoOrder();
    }
}

void Shop::DisplayProviderOrders() const {
    cout << "\n=== ЗАКАЗЫ У ПОСТАВЩИКОВ ===" << endl;
    if (providerOrderCount == 0) {
        cout << "Нет заказов у поставщиков!" << endl;
        cout << "Чтобы создать заказ поставщику:" << endl;
        cout << "1. Оформите заказы через режим пользователя" << endl;
        cout << "2. Перейдите в Режим администратора -> Заказ у поставщика" << endl;
        cout << "3. Добавьте заказы клиентов в заказ поставщику" << endl;
        return;
    }

    for (int i = 0; i < providerOrderCount; i++) {
        cout << "\n--- Заказ поставщику " << (i + 1) << " ---" << endl;
        cout << "Номер заказа: " << providerOrders[i]->getNumberProviderOrder() << endl;
    }
}

void Shop::DisplaySalesReports() const {
    cout << "\n=== ОТЧЕТЫ ПО ПРОДАЖАМ ===" << endl;
    if (salesReportCount == 0) {
        cout << "Нет созданных отчетов!" << endl;
        return;
    }

    for (int i = 0; i < salesReportCount; i++) {
        cout << "\n--- Отчет " << (i + 1) << " ---" << endl;
        salesReports[i]->DisplayReport();
    }
}

void Shop::CreateSalesReport() {
    if (salesReportCount >= MAX_SALES_REPORTS) {
        cout << "Достигнуто максимальное количество отчетов!" << endl;
        return;
    }

    SalesReport* newReport = new SalesReport();
    newReport->GetReport();
    salesReports[salesReportCount] = newReport;
    salesReportCount++;
}

void Shop::RemakeSalesReport() {
    if (salesReportCount == 0) {
        cout << "Нет созданных отчетов для изменения!" << endl;
        return;
    }

    DisplaySalesReports();

    int reportIndex;
    cout << "Введите номер отчета для изменения: ";
    cin >> reportIndex;

    if (reportIndex < 1 || reportIndex > salesReportCount) {
        cout << "Неверный номер отчета!" << endl;
        return;
    }

    salesReports[reportIndex - 1]->RemakeReport();
}

void Shop::AddProviderOrder(ProviderOrder* providerOrder) {
    if (providerOrderCount >= MAX_PROVIDER_ORDERS) {
        cout << "Достигнуто максимальное количество заказов поставщикам!" << endl;
        return;
    }

    providerOrders[providerOrderCount] = providerOrder;
    providerOrderCount++;

    cout << "Заказ поставщику добавлен в систему магазина!" << endl;
}

int Shop::findProductIndex(int article) const {
    for (int i = 0; i < productCount; i++) {
        if (catalogProducts[i]->getArticle() == article) {
            return i;
        }
    }
    return -1;
}

void Shop::updateProductStock(int article, int quantityChange) {
    int index = findProductIndex(article);
    if (index != -1) {
        int newQuantity = catalogProducts[index]->getQuantityStock() + quantityChange;
        catalogProducts[index]->setQuantityStock(newQuantity);

        ofstream file("products.txt");
        if (file.is_open()) {
            for (int i = 0; i < productCount; i++) {
                file << catalogProducts[i]->getArticle() << "|"
                    << catalogProducts[i]->getName() << "|"
                    << catalogProducts[i]->getBrand() << "|"
                    << catalogProducts[i]->getProducerCountry() << "|"
                    << catalogProducts[i]->getCategory() << "|"
                    << catalogProducts[i]->getTypeProduct() << "|"
                    << catalogProducts[i]->getDescription() << "|"
                    << catalogProducts[i]->getQuantityStock() << "|"
                    << catalogProducts[i]->getStatusStock() << "|"
                    << catalogProducts[i]->getPrice() << endl;
            }
            file.close();
        }
    }
}