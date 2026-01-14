#define _CRT_SECURE_NO_WARNINGS
#include "Product.h"
#include <iostream>
#include <iomanip>//для форматирования данных ввода
#include <limits>
#include <locale>
#include <fstream>
#include <sstream>
#include <memory>
#include <vector>

using namespace std;

// Конструктор по умолчанию
Product::Product() : article(0), name(""), brand(""), producerCountry(""),
category(""), typeProduct(""), description(""),
quantityStock(0), statusStock(false), price(0) {}

// Параметризованный конструктор
Product::Product(int art, const std::string& nm, const std::string& br,
    const std::string& country, const std::string& cat,
    const std::string& type, const std::string& desc,
    unsigned int quantity, int pr)
    : article(art), name(nm), brand(br), producerCountry(country),
    category(cat), typeProduct(type), description(desc),
    quantityStock(quantity), statusStock(quantity > 0), price(pr) {}

// Валидация строки по длине
bool Product::validateString(const std::string& str, int maxLength) {
    return str.length() <= maxLength && !str.empty();
}

// Валидация числа по количеству цифр
bool Product::validateNumber(long long number, int maxDigits) {
    if (number < 0) return false;
    std::string numStr = std::to_string(number);
    return numStr.length() <= maxDigits;
}

// Валидация непустой строки
bool Product::validateNonEmpty(const std::string& str) {
    return !str.empty();
}

// Добавить новый товар на склад
void Product::AddStock() {
    InputFromKeyboard();
    cout << "Товар успешно добавлен на склад!" << endl;
}

// Удалить товар со склада
bool Product::DeleteStock() {
    if (quantityStock > 0) {
        quantityStock = 0;
        statusStock = false;
        cout << "Товар удален со склада!" << endl;
        return true;
    }
    cout << "Товар уже отсутствует на складе!" << endl;
    return false;
}

// Редактировать товар на складе
void Product::EditStock() {
    cout << "=== Редактирование товара ===" << endl;
    InputFromKeyboard();
    cout << "Товар успешно отредактирован!" << endl;
}

// Найти товар на складе
bool Product::FindStock() {
    return statusStock;
}

// Поиск товара по артикулу в файле
bool Product::FindProductByArticle(const std::string& filename, int articleToFind) {
    ifstream file(filename);
    if (!file.is_open()) {
        cout << "Файл " << filename << " не найден!" << endl;
        return false;
    }

    string line;
    bool found = false;

    while (getline(file, line)) {
        if (line.empty()) continue;

        stringstream ss(line);
        string token;
        getline(ss, token, '|');

        try {
            int currentArticle = stoi(token);
            if (currentArticle == articleToFind) {
                found = true;

                string tokens[10];
                int tokenCount = 0;
                tokens[tokenCount++] = token;

                while (getline(ss, token, '|') && tokenCount < 10) {
                    tokens[tokenCount++] = token;
                }

                if (tokenCount == 10) {
                    cout << "\n=== НАЙДЕН ТОВАР ===" << endl;
                    cout << "Артикул: " << tokens[0] << endl;
                    cout << "Название: " << tokens[1] << endl;
                    cout << "Бренд: " << tokens[2] << endl;
                    cout << "Страна-производитель: " << tokens[3] << endl;
                    cout << "Категория: " << tokens[4] << endl;
                    cout << "Тип товара: " << tokens[5] << endl;
                    cout << "Описание: " << tokens[6] << endl;
                    cout << "Количество на складе: " << tokens[7] << endl;
                    cout << "Статус на складе: " << (tokens[8] == "1" ? "В наличии" : "Отсутствует") << endl;
                    cout << "Цена: " << tokens[9] << endl;
                    cout << "=========================" << endl;
                }
                break;
            }
        }
        catch (const std::exception& e) {
            continue;
        }
    }

    file.close();

    if (!found) {
        cout << "Товар с артикулом " << articleToFind << " не найден!" << endl;
        return false;
    }

    return true;
}

// Поиск товаров по артикулу с возвращением умных указателей
//возвращает вектор умных указателей на найденные товары
std::vector<std::shared_ptr<Product>> Product::FindProductsByArticleSmart(const std::string& filename, const std::vector<int>& articles) {
    // Создаем вектор для хранения найденных товаров
    std::vector<std::shared_ptr<Product>> foundProducts;

    ifstream file(filename);
    if (!file.is_open()) {
        cout << "Файл " << filename << " не найден!" << endl;
        return foundProducts;
    }

    string line;
    while (getline(file, line)) {
        if (line.empty()) continue;

        stringstream ss(line);
        string token;
        string tokens[10];
        int tokenCount = 0;

        // Читаем артикул
        if (!getline(ss, token, '|')) continue;

        try {
            int currentArticle = stoi(token);

            // Проверяем, ищем ли мы этот артикул
            bool articleFound = false;
            for (int article : articles) {
                if (currentArticle == article) {
                    articleFound = true;
                    break;
                }
            }

            if (!articleFound) continue;

            // Читаем остальные данные
            tokens[tokenCount++] = token;
            while (getline(ss, token, '|') && tokenCount < 10) {
                tokens[tokenCount++] = token;
            }

            if (tokenCount == 10) {
                // Создаем объект Product с помощью умного указателя
                auto product = std::make_shared<Product>(
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

                foundProducts.push_back(product);
            }
        }
        catch (const std::exception& e) {
            continue;
        }
    }

    file.close();
    return foundProducts;
}

// Функция поиска товара по артикулу (несколько)
void Product::FindProductsByArticle(const std::string& filename) {
    cout << "\n=== ПОИСК ТОВАРОВ ПО АРТИКУЛУ ===" << endl;

    int productCount;
    while (true) {
        cout << "Сколько товаров хотите найти? ";
        cin >> productCount;

        if (cin.fail()) {
            cin.clear();
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
            cout << "Ошибка! Введите целое число." << endl;
            continue;
        }

        if (productCount < 0) {
            cout << "Ошибка! Количество не может быть отрицательным." << endl;
            continue;
        }

        if (productCount == 0) {
            cout << "Поиск отменен." << endl;
            return;
        }

        break;
    }

    // Используем вектор для хранения артикулов
    vector<int> articles;
    for (int i = 0; i < productCount; i++) {
        int articleToFind;
        cout << "\nВведите артикул товара для поиска " << (i + 1) << "/" << productCount << ": ";
        cin >> articleToFind;
        articles.push_back(articleToFind);
    }

    // Используем новый метод с умными указателями
    auto foundProducts = FindProductsByArticleSmart(filename, articles);

    cout << "\n=== РЕЗУЛЬТАТ ПОИСКА ===" << endl;
    cout << "Запрошено к поиску: " << productCount << " товар(ов)" << endl;
    cout << "Успешно найдено: " << foundProducts.size() << " товар(ов)" << endl;

    // Отображаем найденные товары
    for (size_t i = 0; i < foundProducts.size(); i++) {
        cout << "\n--- Найденный товар " << (i + 1) << " ---" << endl;
        foundProducts[i]->DisplayInfo();
    }
}

// Загрузка всех товаров из файла с умными указателями
std::vector<std::shared_ptr<Product>> Product::LoadAllProductsSmart(const std::string& filename) {
    std::vector<std::shared_ptr<Product>> products;

    ifstream file(filename);
    if (!file.is_open()) {
        cout << "Файл " << filename << " не найден!" << endl;
        return products;
    }

    string line;
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
                // Создаем объект Product с помощью умного указателя
                auto product = std::make_shared<Product>(
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

                products.push_back(product);
            }
            catch (const std::exception& e) {
                continue;
            }
        }
    }

    file.close();
    return products;
}

// Редактирование товара в файле
bool Product::EditProductByIndex(const std::string& filename, int productIndex) {
    // Загружаем все товары с умными указателями
    auto allProducts = LoadAllProductsSmart(filename);

    if (productIndex < 1 || productIndex > static_cast<int>(allProducts.size())) {
        cout << "Неверный номер товара! Доступные номера: 1-" << allProducts.size() << endl;
        return false;
    }

    int actualIndex = productIndex - 1;
    auto& productToEdit = allProducts[actualIndex];

    cout << "\n=== ТЕКУЩАЯ ИНФОРМАЦИЯ О ТОВАРЕ ===" << endl;
    productToEdit->DisplayInfo();

    cout << "\nКакие поля хотите отредактировать?" << endl;
    cout << "Введите номера полей через пробел (1-10): ";
    cin.ignore(numeric_limits<streamsize>::max(), '\n');

    string fieldsInput;
    getline(cin, fieldsInput);

    bool editFields[10] = { false };
    stringstream fieldsStream(fieldsInput);
    int fieldNum;

    while (fieldsStream >> fieldNum) {
        if (fieldNum >= 1 && fieldNum <= 10) {
            editFields[fieldNum - 1] = true;
        }
    }

    cout << "\n=== ВВОД НОВЫХ ДАННЫХ ===" << endl;

    if (editFields[0]) { // Артикул
        while (true) {
            cout << "Введите новый артикул: ";
            int newArticle;
            cin >> newArticle;

            if (cin.fail()) {
                cin.clear();
                cin.ignore(numeric_limits<streamsize>::max(), '\n');
                cout << "Ошибка! Введите целое число." << endl;
                continue;
            }

            if (validateNumber(newArticle, 10)) {
                productToEdit->setArticle(newArticle);
                break;
            }
            cout << "Ошибка! Артикул должен быть неотрицательным числом до 10 цифр." << endl;
        }
    }

    if (editFields[1]) { // Название
        cin.ignore();
        while (true) {
            cout << "Введите новое название: ";
            string newName;
            getline(cin, newName);

            if (validateNonEmpty(newName) && validateString(newName, 30)) {
                productToEdit->setName(newName);
                break;
            }
            cout << "Ошибка! Название не может быть пустым и не должно превышать 30 символов." << endl;
        }
    }

    if (editFields[2]) { // Бренд
        while (true) {
            cout << "Введите новый бренд: ";
            string newBrand;
            getline(cin, newBrand);

            if (validateNonEmpty(newBrand) && validateString(newBrand, 30)) {
                productToEdit->setBrand(newBrand);
                break;
            }
            cout << "Ошибка! Бренд не может быть пустым и не должен превышать 30 символов." << endl;
        }
    }

    if (editFields[3]) { // Страна-производитель
        while (true) {
            cout << "Введите новую страну-производитель: ";
            string newCountry;
            getline(cin, newCountry);

            if (validateNonEmpty(newCountry) && validateString(newCountry, 30)) {
                productToEdit->setProducerCountry(newCountry);
                break;
            }
            cout << "Ошибка! Страна-производитель не может быть пустой и не должна превышать 30 символов." << endl;
        }
    }

    if (editFields[4]) { // Категория
        while (true) {
            cout << "Введите новую категорию: ";
            string newCategory;
            getline(cin, newCategory);

            if (validateNonEmpty(newCategory) && validateString(newCategory, 30)) {
                productToEdit->setCategory(newCategory);
                break;
            }
            cout << "Ошибка! Категория не может быть пустой и не должна превышать 30 символов." << endl;
        }
    }

    if (editFields[5]) { // Тип товара
        while (true) {
            cout << "Введите новый тип товара: ";
            string newType;
            getline(cin, newType);

            if (validateNonEmpty(newType) && validateString(newType, 30)) {
                productToEdit->setTypeProduct(newType);
                break;
            }
            cout << "Ошибка! Тип товара не может быть пустым и не должен превышать 30 символов." << endl;
        }
    }

    if (editFields[6]) { // Описание
        while (true) {
            cout << "Введите новое описание: ";
            string newDescription;
            getline(cin, newDescription);

            if (validateNonEmpty(newDescription)) {
                productToEdit->setDescription(newDescription);
                break;
            }
            cout << "Ошибка! Описание не может быть пустым." << endl;
        }
    }

    if (editFields[7]) { // Количество на складе
        while (true) {
            cout << "Введите новое количество на складе: ";
            unsigned int newQuantity;
            cin >> newQuantity;

            if (cin.fail()) {
                cin.clear();
                cin.ignore(numeric_limits<streamsize>::max(), '\n');
                cout << "Ошибка! Введите целое число." << endl;
                continue;
            }

            if (validateNumber(newQuantity, 10)) {
                productToEdit->setQuantityStock(newQuantity);
                break;
            }
            cout << "Ошибка! Количество должно быть числом до 10 цифр." << endl;
        }
    }

    if (editFields[8]) { // Статус на складе
        while (true) {
            int statusInput;
            cout << "Введите новый статус товара (1 - В наличии, 0 - Отсутствует): ";
            cin >> statusInput;

            if (cin.fail()) {
                cin.clear();
                cin.ignore(numeric_limits<streamsize>::max(), '\n');
                cout << "Ошибка! Введите 1 или 0." << endl;
                continue;
            }

            if (statusInput == 0 || statusInput == 1) {
                productToEdit->setStatusStock(statusInput == 1);
                break;
            }
            cout << "Ошибка! Введите только 1 или 0." << endl;
        }
    }

    if (editFields[9]) { // Цена
        while (true) {
            cout << "Введите новую цену: ";
            int newPrice;
            cin >> newPrice;

            if (cin.fail()) {
                cin.clear();
                cin.ignore(numeric_limits<streamsize>::max(), '\n');
                cout << "Ошибка! Введите целое число." << endl;
                continue;
            }

            if (newPrice >= 0 && validateNumber(newPrice, 10)) {
                productToEdit->setPrice(newPrice);
                break;
            }
            cout << "Ошибка! Цена не может быть отрицательной и должна быть числом до 10 цифр." << endl;
        }
    }

    // Перезаписываем файл с обновленными данными
    ofstream outFile(filename);
    if (!outFile.is_open()) {
        cout << "Ошибка открытия файла для записи!" << endl;
        return false;
    }

    for (const auto& product : allProducts) {//auto& - автоматическое определение типа
        outFile << product->getArticle() << "|"
            << product->getName() << "|"
            << product->getBrand() << "|"
            << product->getProducerCountry() << "|"
            << product->getCategory() << "|"
            << product->getTypeProduct() << "|"
            << product->getDescription() << "|"
            << product->getQuantityStock() << "|"
            << product->getStatusStock() << "|"
            << product->getPrice() << endl;
    }

    outFile.close();

    cout << "\nТовар №" << productIndex << " успешно отредактирован!" << endl;
    return true;
}

// Функция редактирования товаров (несколько)
void Product::EditProductsInFile(const std::string& filename) {
    cout << "\n=== РЕДАКТИРОВАНИЕ ТОВАРОВ ===" << endl;

    // Загружаем и показываем товары с помощью умных указателей
    auto allProducts = LoadAllProductsSmart(filename);
    if (allProducts.empty()) {
        cout << "Нет товаров для редактирования!" << endl;
        return;
    }

    cout << "\nСписок товаров:" << endl;
    for (size_t i = 0; i < allProducts.size(); i++) {
        cout << "\n--- Товар " << (i + 1) << " ---" << endl;
        allProducts[i]->DisplayInfo();
    }

    int productCount;
    while (true) {
        cout << "\nСколько товаров хотите отредактировать? ";
        cin >> productCount;

        if (cin.fail()) {
            cin.clear();
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
            cout << "Ошибка! Введите целое число." << endl;
            continue;
        }

        if (productCount < 0) {
            cout << "Ошибка! Количество не может быть отрицательным." << endl;
            continue;
        }

        if (productCount == 0) {
            cout << "Редактирование отменено." << endl;
            return;
        }

        break;
    }

    int successfullyEdited = 0;
    for (int i = 0; i < productCount; i++) {
        int productIndex;
        cout << "\nВведите порядковый номер товара для редактирования "
            << (i + 1) << "/" << productCount << ": ";
        cin >> productIndex;

        if (EditProductByIndex(filename, productIndex)) {
            successfullyEdited++;
        }
    }

    cout << "\n=== РЕЗУЛЬТАТ РЕДАКТИРОВАНИЯ ===" << endl;
    cout << "Запрошено к редактированию: " << productCount << " товар(ов)" << endl;
    cout << "Успешно отредактировано: " << successfullyEdited << " товар(ов)" << endl;

    // Показываем обновленный список
    auto updatedProducts = LoadAllProductsSmart(filename);
    cout << "\n=== ОБНОВЛЕННЫЙ СПИСОК ТОВАРОВ ===" << endl;
    for (size_t i = 0; i < updatedProducts.size(); i++) {
        cout << "\n--- Товар " << (i + 1) << " ---" << endl;
        updatedProducts[i]->DisplayInfo();
    }
}

// Удаление товара по артикулу
bool Product::DeleteProductByArticle(const std::string& filename, int articleToDelete) {
    // Загружаем все товары с умными указателями
    auto allProducts = LoadAllProductsSmart(filename);

    // Ищем товар для удаления
    auto it = std::find_if(allProducts.begin(), allProducts.end(),
        [articleToDelete](const std::shared_ptr<Product>& product) {
            return product->getArticle() == articleToDelete;
        });

    if (it == allProducts.end()) {
        cout << "Товар с артикулом " << articleToDelete << " не найден!" << endl;
        return false;
    }

    cout << "Найден товар для удаления: " << (*it)->getName() << endl;

    // Удаляем товар из вектора
    allProducts.erase(it);

    // Перезаписываем файл без удаленного товара
    ofstream outFile(filename);
    if (!outFile.is_open()) {
        cout << "Ошибка открытия файла для записи!" << endl;
        return false;
    }

    for (const auto& product : allProducts) {
        outFile << product->getArticle() << "|"
            << product->getName() << "|"
            << product->getBrand() << "|"
            << product->getProducerCountry() << "|"
            << product->getCategory() << "|"
            << product->getTypeProduct() << "|"
            << product->getDescription() << "|"
            << product->getQuantityStock() << "|"
            << product->getStatusStock() << "|"
            << product->getPrice() << endl;
    }

    outFile.close();

    cout << "Товар с артикулом " << articleToDelete << " успешно удален!" << endl;
    return true;
}

// Функция удаления товаров
void Product::DeleteProductsFromFile(const std::string& filename) {
    cout << "\n=== УДАЛЕНИЕ ТОВАРОВ СО СКЛАДА ===" << endl;

    // Показываем товары с помощью умных указателей
    auto allProducts = LoadAllProductsSmart(filename);
    if (allProducts.empty()) {
        cout << "Нет товаров для удаления!" << endl;
        return;
    }

    cout << "\nСписок товаров:" << endl;
    for (size_t i = 0; i < allProducts.size(); i++) {
        cout << "\n--- Товар " << (i + 1) << " ---" << endl;
        allProducts[i]->DisplayInfo();
    }

    int productCount;
    while (true) {
        cout << "\nСколько товаров хотите удалить? ";
        cin >> productCount;

        if (cin.fail()) {
            cin.clear();
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
            cout << "Ошибка! Введите целое число." << endl;
            continue;
        }

        if (productCount < 0) {
            cout << "Ошибка! Количество не может быть отрицательным." << endl;
            continue;
        }

        if (productCount == 0) {
            cout << "Удаление отменено." << endl;
            return;
        }

        break;
    }

    char confirmation;
    cout << "\nВы уверены, что хотите удалить " << productCount << " товар(ов)? (y/n): ";
    cin >> confirmation;

    if (confirmation != 'y' && confirmation != 'Y') {
        cout << "Удаление отменено." << endl;
        return;
    }

    int successfullyDeleted = 0;
    for (int i = 0; i < productCount; i++) {
        int articleToDelete;
        cout << "\nВведите артикул товара для удаления " << (i + 1) << "/" << productCount << ": ";
        cin >> articleToDelete;

        if (DeleteProductByArticle(filename, articleToDelete)) {
            successfullyDeleted++;
        }
    }

    cout << "\n=== РЕЗУЛЬТАТ УДАЛЕНИЯ ===" << endl;
    cout << "Запрошено к удалению: " << productCount << " товар(ов)" << endl;
    cout << "Успешно удалено: " << successfullyDeleted << " товар(ов)" << endl;

    // Показываем обновленный список
    auto updatedProducts = LoadAllProductsSmart(filename);
    cout << "\n=== ОБНОВЛЕННЫЙ СПИСОК ТОВАРОВ ===" << endl;
    for (size_t i = 0; i < updatedProducts.size(); i++) {
        cout << "\n--- Товар " << (i + 1) << " ---" << endl;
        updatedProducts[i]->DisplayInfo();
    }
}

// Ввод данных с клавиатуры
void Product::InputFromKeyboard() {
    cout << "\n=== Ввод данных о товаре ===" << endl;

    // Ввод артикула
    while (true) {
        cout << "Введите артикул: ";
        cin >> article;

        if (cin.fail()) {
            cin.clear();
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
            cout << "Ошибка! Введите целое число." << endl;
            continue;
        }

        if (!validateNumber(article, 10)) {
            cout << "Ошибка! Артикул должен быть неотрицательным числом до 10 цифр." << endl;
            continue;
        }

        break;
    }

    cin.ignore(numeric_limits<streamsize>::max(), '\n');

    // Ввод названия
    while (true) {
        cout << "Введите название: ";
        getline(cin, name);

        if (!validateNonEmpty(name)) {
            cout << "Ошибка! Название не может быть пустым." << endl;
            continue;
        }

        if (!validateString(name, 30)) {
            cout << "Ошибка! Название не должно превышать 30 символов." << endl;
            continue;
        }

        break;
    }

    // Ввод бренда
    while (true) {
        cout << "Введите бренд: ";
        getline(cin, brand);

        if (!validateNonEmpty(brand)) {
            cout << "Ошибка! Бренд не может быть пустым." << endl;
            continue;
        }

        if (!validateString(brand, 30)) {
            cout << "Ошибка! Бренд не должен превышать 30 символов." << endl;
            continue;
        }

        break;
    }

    // Ввод страны-производителя
    while (true) {
        cout << "Введите страну-производитель: ";
        getline(cin, producerCountry);

        if (!validateNonEmpty(producerCountry)) {
            cout << "Ошибка! Страна-производитель не может быть пустой." << endl;
            continue;
        }

        if (!validateString(producerCountry, 30)) {
            cout << "Ошибка! Страна-производитель не должна превышать 30 символов." << endl;
            continue;
        }

        break;
    }

    // Ввод категории
    while (true) {
        cout << "Введите категорию: ";
        getline(cin, category);

        if (!validateNonEmpty(category)) {
            cout << "Ошибка! Категория не может быть пустой." << endl;
            continue;
        }

        if (!validateString(category, 30)) {
            cout << "Ошибка! Категория не должна превышать 30 символов." << endl;
            continue;
        }

        break;
    }

    // Ввод типа товара
    while (true) {
        cout << "Введите тип товара: ";
        getline(cin, typeProduct);

        if (!validateNonEmpty(typeProduct)) {
            cout << "Ошибка! Тип товара не может быть пустым." << endl;
            continue;
        }

        if (!validateString(typeProduct, 30)) {
            cout << "Ошибка! Тип товара не должен превышать 30 символов." << endl;
            continue;
        }

        break;
    }

    // Ввод описания
    while (true) {
        cout << "Введите описание: ";
        getline(cin, description);

        if (!validateNonEmpty(description)) {
            cout << "Ошибка! Описание не может быть пустым." << endl;
            continue;
        }

        break;
    }

    // Ввод количества на складе
    while (true) {
        cout << "Введите количество на складе: ";
        cin >> quantityStock;

        if (cin.fail()) {
            cin.clear();
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
            cout << "Ошибка! Введите целое число." << endl;
            continue;
        }

        if (quantityStock < 0) {
            cout << "Ошибка! Количество не может быть отрицательным." << endl;
            continue;
        }

        if (!validateNumber(quantityStock, 10)) {
            cout << "Ошибка! Количество должно быть числом до 10 цифр." << endl;
            continue;
        }

        break;
    }

    // Ввод статуса товара
    while (true) {
        int statusInput;
        cout << "Введите статус товара (1 - В наличии, 0 - Отсутствует): ";
        cin >> statusInput;

        if (cin.fail()) {
            cin.clear();
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
            cout << "Ошибка! Введите 1 или 0." << endl;
            continue;
        }

        if (statusInput == 0 || statusInput == 1) {
            statusStock = (statusInput == 1);
            break;
        }

        cout << "Ошибка! Введите только 1 или 0." << endl;
    }

    // Ввод цены
    while (true) {
        cout << "Введите цену: ";
        cin >> price;

        if (cin.fail()) {
            cin.clear();
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
            cout << "Ошибка! Введите целое число." << endl;
            continue;
        }

        if (price < 0) {
            cout << "Ошибка! Цена не может быть отрицательной." << endl;
            continue;
        }

        if (!validateNumber(price, 10)) {
            cout << "Ошибка! Цена должна быть числом до 10 цифр." << endl;
            continue;
        }

        break;
    }

    cin.ignore(numeric_limits<streamsize>::max(), '\n');
}
// Запись в файл
void Product::WriteToFile(const std::string& filename) {
    ofstream file(filename, ios::app);

    if (file.is_open()) {
        file << article << "|" << name << "|" << brand << "|"
            << producerCountry << "|" << category << "|" << typeProduct << "|"
            << description << "|" << quantityStock << "|"
            << statusStock << "|" << price << endl;

        file.close();
        cout << "Данные успешно записаны в файл: " << filename << endl;
    }
    else {
        cout << "Ошибка открытия файла для записи!" << endl;
    }
}

// Чтение из файла
void Product::ReadFromFile(const std::string& filename) {
    ifstream file(filename);

    if (file.is_open()) {
        char delimiter;
        file >> article >> delimiter;
        getline(file, name, '|');
        getline(file, brand, '|');
        getline(file, producerCountry, '|');
        getline(file, category, '|');
        getline(file, typeProduct, '|');
        getline(file, description, '|');
        file >> quantityStock >> delimiter;
        file >> statusStock >> delimiter;
        file >> price;

        file.close();
        cout << "Данные успешно прочитаны из файла: " << filename << endl;
    }
    else {
        cout << "Ошибка открытия файла для чтения!" << endl;
    }
}

// Информация о товаре
void Product::DisplayInfo() {
    cout << "\n=== Информация о товаре ===" << endl;
    cout << "Артикул: " << article << endl;
    cout << "Название: " << name << endl;
    cout << "Бренд: " << brand << endl;
    cout << "Страна-производитель: " << producerCountry << endl;
    cout << "Категория: " << category << endl;
    cout << "Тип товара: " << typeProduct << endl;
    cout << "Описание: " << description << endl;
    cout << "Количество на складе: " << quantityStock << endl;
    cout << "Статус на складе: " << (statusStock ? "В наличии" : "Отсутствует") << endl;
    cout << "Цена: " << price << endl;
}

// Отображение товаров в виде таблицы (с использованием умных указателей)
void Product::DisplayProductsTable(const std::string& filename) {
    // Используем новый метод с умными указателями
    auto allProducts = LoadAllProductsSmart(filename);

    if (allProducts.empty()) {
        cout << "\nФайл " << filename << " не найден или пуст!" << endl;
        cout << "Сначала добавьте товары через меню 'Добавить товар'" << endl;
        return;
    }

    cout << "\n=== ИНФОРМАЦИЯ О ТОВАРАХ ===" << endl;

    for (size_t i = 0; i < allProducts.size(); i++) {
        cout << "\n--- Товар " << (i + 1) << " ---" << endl;
        cout << "Артикул: " << allProducts[i]->getArticle() << endl;
        cout << "Название: " << allProducts[i]->getName() << endl;
        cout << "Бренд: " << allProducts[i]->getBrand() << endl;
        cout << "Страна: " << allProducts[i]->getProducerCountry() << endl;
        cout << "Категория: " << allProducts[i]->getCategory() << endl;
        cout << "Тип: " << allProducts[i]->getTypeProduct() << endl;
        cout << "Описание: " << allProducts[i]->getDescription() << endl;
        cout << "Количество: " << allProducts[i]->getQuantityStock() << endl;
        cout << "Статус: " << (allProducts[i]->getStatusStock() ? "В наличии" : "Отсутствует") << endl;
        cout << "Цена: " << allProducts[i]->getPrice() << endl;
    }

    cout << "\n=================================" << endl;
    cout << "Всего товаров: " << allProducts.size() << endl;
    cout << "=================================" << endl;
}

// Создание товара с умным указателем из данных файла
std::shared_ptr<Product> Product::CreateProductFromFileSmart(const std::string& filename, int articleToFind) {
    ifstream file(filename);
    if (!file.is_open()) {
        cout << "Файл " << filename << " не найден!" << endl;
        return nullptr;
    }

    string line;
    while (getline(file, line)) {
        if (line.empty()) continue;

        stringstream ss(line);
        string token;
        string tokens[10];
        int tokenCount = 0;

        // Читаем артикул
        if (!getline(ss, token, '|')) continue;

        try {
            int currentArticle = stoi(token);
            if (currentArticle == articleToFind) {
                // Читаем остальные данные
                tokens[tokenCount++] = token;
                while (getline(ss, token, '|') && tokenCount < 10) {
                    tokens[tokenCount++] = token;
                }

                if (tokenCount == 10) {
                    // Создаем и возвращаем умный указатель
                    return std::make_shared<Product>(
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
                }
            }
        }
        catch (const std::exception& e) {
            continue;
        }
    }

    file.close();
    return nullptr;
}

// Поиск товара по имени с возвращением умных указателей
std::vector<std::shared_ptr<Product>> Product::FindProductsByNameSmart(const std::string& filename, const std::string& name) {
    std::vector<std::shared_ptr<Product>> foundProducts;

    ifstream file(filename);
    if (!file.is_open()) {
        cout << "Файл " << filename << " не найден!" << endl;
        return foundProducts;
    }

    string line;
    string searchNameLower = name;
    // Преобразуем к нижнему регистру для поиска без учета регистра
    std::transform(searchNameLower.begin(), searchNameLower.end(), searchNameLower.begin(), ::tolower);

    while (getline(file, line)) {
        if (line.empty()) continue;

        stringstream ss(line);
        string token;
        string tokens[10];
        int tokenCount = 0;

        // Пропускаем артикул
        if (!getline(ss, token, '|')) continue;

        // Читаем название
        if (!getline(ss, token, '|')) continue;

        string productName = token;
        string productNameLower = productName;
        std::transform(productNameLower.begin(), productNameLower.end(), productNameLower.begin(), ::tolower);

        // Проверяем, содержит ли название искомую строку
        if (productNameLower.find(searchNameLower) != string::npos) {
            // Возвращаемся к началу строки
            ss = stringstream(line);
            tokenCount = 0;

            while (getline(ss, token, '|') && tokenCount < 10) {
                tokens[tokenCount++] = token;
            }

            if (tokenCount == 10) {
                try {
                    // Создаем объект Product с помощью умного указателя
                    auto product = std::make_shared<Product>(
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

                    foundProducts.push_back(product);
                }
                catch (const std::exception& e) {
                    continue;
                }
            }
        }
    }

    file.close();
    return foundProducts;
}

// Получение товара по индексу с умным указателем
std::shared_ptr<Product> Product::GetProductByIndexSmart(const std::string& filename, int index) {
    auto allProducts = LoadAllProductsSmart(filename);

    if (index < 1 || index > static_cast<int>(allProducts.size())) {
        return nullptr;
    }

    return allProducts[index - 1];
}

//  Обновление товара в файле с помощью умного указателя
bool Product::UpdateProductInFileSmart(const std::string& filename, std::shared_ptr<Product> updatedProduct) {
    if (!updatedProduct) {
        cout << "Ошибка: передан нулевой указатель на товар!" << endl;
        return false;
    }

    // Загружаем все товары
    auto allProducts = LoadAllProductsSmart(filename);

    // Ищем товар с таким же артикулом
    bool found = false;
    for (auto& product : allProducts) {
        if (product->getArticle() == updatedProduct->getArticle()) {
            // Обновляем товар
            *product = *updatedProduct;
            found = true;
            break;
        }
    }

    if (!found) {
        // Если товар не найден, добавляем его
        allProducts.push_back(updatedProduct);
    }

    // Перезаписываем файл
    ofstream outFile(filename);
    if (!outFile.is_open()) {
        cout << "Ошибка открытия файла для записи!" << endl;
        return false;
    }

    for (const auto& product : allProducts) {
        outFile << product->getArticle() << "|"
            << product->getName() << "|"
            << product->getBrand() << "|"
            << product->getProducerCountry() << "|"
            << product->getCategory() << "|"
            << product->getTypeProduct() << "|"
            << product->getDescription() << "|"
            << product->getQuantityStock() << "|"
            << product->getStatusStock() << "|"
            << product->getPrice() << endl;
    }

    outFile.close();

    if (found) {
        cout << "Товар с артикулом " << updatedProduct->getArticle() << " успешно обновлен!" << endl;
    }
    else {
        cout << "Товар с артикулом " << updatedProduct->getArticle() << " успешно добавлен!" << endl;
    }

    return true;
}

// Создание товара из пользовательского ввода с умным указателем
std::shared_ptr<Product> Product::CreateProductFromInputSmart() {
    auto product = std::make_shared<Product>();
    product->InputFromKeyboard();
    return product;
}
