#include "Product.h"
#include <iostream>//ввод/вывод
#include <iomanip>//форматирование ввода данных
#include <limits>//пределы типов данных
#include <locale>//локализация
#include <fstream>//работа с файлами
#include <sstream>//работа со строками

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

// поиск товара по артикулу в файле
bool Product::FindProductByArticle(const std::string& filename, int articleToFind) {
    ifstream file(filename);//объект для чтения из файла
    if (!file.is_open()) {//file.is_open-метод, проверяющий открытие класса
        cout << "Файл " << filename << " не найден!" << endl;
        return false;
    }

    const int MAX_PRODUCTS = 1000;//количество товаров для обработки
    string lines[MAX_PRODUCTS];//массив для хранения строк в файле
    int lineCount = 0;//счетчик отработанных строк
    bool found = false;//флаг - найден или нет

    // Читаем все строки в массив
    string line;
    while (getline(file, line) && lineCount < MAX_PRODUCTS) {//getline(file, line) из файла чтение в переменную
        if (line.empty()) continue;//проверка на пустую строку

        //читает строки до |
        stringstream ss(line);
        string token;
        getline(ss, token, '|');

        try {
            int currentArticle = stoi(token);
            if (currentArticle == articleToFind) {//сравнивает искомый артикул с артиклом ф файле
                found = true;

                
                string tokens[10];//хранение остальных данных о товаре
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
                break; // Нашли товар, выходим из цикла
            }
        }
        catch (const std::exception& e) {
            // Если не удалось распарсить, продолжаем поиск
        }

        lines[lineCount++] = line;
    }
    file.close();

    if (!found) {
        cout << "Товар с артикулом " << articleToFind << " не найден!" << endl;
        return false;
    }

    return true;
}

// функция поиска товара по артикулу (несколько)
void Product::FindProductsByArticle(const std::string& filename) {
    cout << "\n=== ПОИСК ТОВАРОВ ПО АРТИКУЛУ ===" << endl;

    int productCount;
    // Запрос количества товаров для поиска
    while (true) {
        cout << "Сколько товаров хотите найти? ";
        cin >> productCount;
        if (cin.fail()) {//проверка если ввели не число
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

    // Поиск товаров по артикулам
    int successfullyFound = 0;//найденные товары
    for (int i = 0; i < productCount; i++) {
        int articleToFind;
        cout << "\nВведите артикул товара для поиска " << (i + 1) << "/" << productCount << ": ";
        cin >> articleToFind;//cin-ввод данных с клавиатуры

        if (FindProductByArticle(filename, articleToFind)) {
            successfullyFound++;//если товар найден, увеличиваем счетчик
        }
    }

    cout << "\n=== РЕЗУЛЬТАТ ПОИСКА ===" << endl;
    cout << "Запрошено к поиску: " << productCount << " товар(ов)" << endl;
    cout << "Успешно найдено: " << successfullyFound << " товар(ов)" << endl;
}

// редактирование товара в файле
bool Product::EditProductByIndex(const std::string& filename, int productIndex) {
    ifstream file(filename);//объект для чтения из файла
    if (!file.is_open()) {
        cout << "Файл " << filename << " не найден!" << endl;
        return false;
    }

    const int MAX_PRODUCTS = 1000;
    string lines[MAX_PRODUCTS];
    int lineCount = 0;

    // Читаем все строки в массив
    string line;
    while (getline(file, line) && lineCount < MAX_PRODUCTS) {
        if (line.empty()) continue;
        lines[lineCount++] = line;
    }
    file.close();

    // Проверка индекса
    if (productIndex < 1 || productIndex > lineCount) {
        cout << "Неверный номер товара! Доступные номера: 1-" << lineCount << endl;
        return false;
    }

    // Показываем текущую информацию о товаре
    int actualIndex = productIndex - 1;
    cout << "\n=== ТЕКУЩАЯ ИНФОРМАЦИЯ О ТОВАРЕ ===" << endl;

    stringstream ss(lines[actualIndex]);//создает поток из этой строки для удобного парсинга
    string token; // Временная переменная для хранения одного поля
    const int MAX_TOKENS = 10; // Максимальное количество полей у товара
    string tokens[MAX_TOKENS];// Массив для хранения всех полей товара
    int tokenCount = 0;// Счетчик извлеченных полей

    while (getline(ss, token, '|') && tokenCount < MAX_TOKENS) {
        tokens[tokenCount++] = token;
    }

    if (tokenCount == 10) {
        try {
            cout << "1. Артикул: " << tokens[0] << endl;
            cout << "2. Название: " << tokens[1] << endl;
            cout << "3. Бренд: " << tokens[2] << endl;
            cout << "4. Страна-производитель: " << tokens[3] << endl;
            cout << "5. Категория: " << tokens[4] << endl;
            cout << "6. Тип товара: " << tokens[5] << endl;
            cout << "7. Описание: " << tokens[6] << endl;
            cout << "8. Количество на складе: " << tokens[7] << endl;
            cout << "9. Статус на складе: " << (tokens[8] == "1" ? "В наличии" : "Отсутствует") << endl;
            cout << "10. Цена: " << tokens[9] << endl;
        }
        catch (const std::exception& e) {
            cout << "Ошибка чтения данных товара!" << endl;
            return false;
        }
    }

    // Запрашиваем, какие поля редактировать
    cout << "\nКакие поля хотите отредактировать?" << endl;
    cout << "Введите номера полей через пробел (1-10): ";
    cin.ignore(numeric_limits<streamsize>::max(), '\n'); //чтение символов
    string fieldsInput;
    getline(cin, fieldsInput);//читает пока не нажмется enter

    // Парсим выбранные поля
    bool editFields[10] = { false };
    stringstream fieldsStream(fieldsInput);
    int fieldNum;
    while (fieldsStream >> fieldNum) {//извлекает числа из потока
        if (fieldNum >= 1 && fieldNum <= 10) {
            editFields[fieldNum - 1] = true;
        }
    }

    // Создаем новый товар для редактирования
    Product editedProduct;

    // Заполняем текущими значениями
    try {
        editedProduct.setArticle(stoi(tokens[0]));
        editedProduct.setName(tokens[1]);
        editedProduct.setBrand(tokens[2]);
        editedProduct.setProducerCountry(tokens[3]);
        editedProduct.setCategory(tokens[4]);
        editedProduct.setTypeProduct(tokens[5]);
        editedProduct.setDescription(tokens[6]);
        editedProduct.setQuantityStock(stoi(tokens[7]));
        editedProduct.setStatusStock(tokens[8] == "1");
        editedProduct.setPrice(stoi(tokens[9]));
    }
    catch (const std::exception& e) {
        cout << "Ошибка чтения данных товара!" << endl;
        return false;
    }

    // Редактируем выбранные поля
    cout << "\n=== ВВОД НОВЫХ ДАННЫХ ===" << endl;

    if (editFields[0]) { // Артикул
        while (true) {//если выбрали артикул, заходит в цикл
            cout << "Введите новый артикул: ";
            int newArticle;
            cin >> newArticle;
            if (cin.fail()) {//проверка введено ли число
                cin.clear();
                cin.ignore(numeric_limits<streamsize>::max(), '\n');
                cout << "Ошибка! Введите целое число." << endl;
                continue;
            }
            if (validateNumber(newArticle, 10)) {//если число не превышает 10 символов
                editedProduct.setArticle(newArticle);
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
            if (validateNonEmpty(newName) && validateString(newName, 30)) {//если не пустое и не больше 30 символов
                editedProduct.setName(newName);
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
                editedProduct.setBrand(newBrand);
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
                editedProduct.setProducerCountry(newCountry);
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
                editedProduct.setCategory(newCategory);
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
                editedProduct.setTypeProduct(newType);
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
                editedProduct.setDescription(newDescription);
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
                editedProduct.setQuantityStock(newQuantity);
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
                editedProduct.setStatusStock(statusInput == 1);
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
                editedProduct.setPrice(newPrice);
                break;
            }
            cout << "Ошибка! Цена не может быть отрицательной и должна быть числом до 10 цифр." << endl;
        }
    }

    // Заменяем старую строку новой
    stringstream newLine;
    newLine << editedProduct.getArticle() << "|" << editedProduct.getName() << "|"
        << editedProduct.getBrand() << "|" << editedProduct.getProducerCountry() << "|"
        << editedProduct.getCategory() << "|" << editedProduct.getTypeProduct() << "|"
        << editedProduct.getDescription() << "|" << editedProduct.getQuantityStock() << "|"
        << editedProduct.getStatusStock() << "|" << editedProduct.getPrice();

    lines[actualIndex] = newLine.str();

    // Перезаписываем файл
    ofstream outFile(filename);//поток для записи в файл, старые данные удаляются 
    if (!outFile.is_open()) {
        cout << "Ошибка открытия файла для записи!" << endl;
        return false;
    }

    for (int i = 0; i < lineCount; i++) {
        outFile << lines[i] << endl;//перезаписывается в файл, endl добавляет перевод строки после каждой записи
    }
    outFile.close();

    cout << "\nТовар №" << productIndex << " успешно отредактирован!" << endl;
    return true;
}

// функция редактирования товаров(несколько)
void Product::EditProductsInFile(const std::string& filename) {
    cout << "\n=== РЕДАКТИРОВАНИЕ ТОВАРОВ ===" << endl;

    // Сначала показываем текущие товары
    DisplayProductsTable(filename);

    int productCount;
    // Запрос количества товаров для редактирования
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

    // Редактирование товаров по порядковым номерам
    int successfullyEdited = 0;
    for (int i = 0; i < productCount; i++) {
        int productIndex;
        cout << "\nВведите порядковый номер товара для редактирования " << (i + 1) << "/" << productCount << ": ";
        cin >> productIndex;

        if (EditProductByIndex(filename, productIndex)) {
            successfullyEdited++;
        }
    }

    cout << "\n=== РЕЗУЛЬТАТ РЕДАКТИРОВАНИЯ ===" << endl;
    cout << "Запрошено к редактированию: " << productCount << " товар(ов)" << endl;
    cout << "Успешно отредактировано: " << successfullyEdited << " товар(ов)" << endl;

    // Показываем обновленную таблицу
    cout << "\n=== ОБНОВЛЕННЫЙ СПИСОК ТОВАРОВ ===" << endl;
    DisplayProductsTable(filename);
}

// удаление товара по артикулу
bool Product::DeleteProductByArticle(const std::string& filename, int articleToDelete) {
    ifstream file(filename);
    if (!file.is_open()) {
        cout << "Файл " << filename << " не найден!" << endl;
        return false;
    }

    const int MAX_PRODUCTS = 1000;
    string lines[MAX_PRODUCTS];
    int lineCount = 0;
    bool found = false;

    // Читаем все строки в массив
    string line;
    while (getline(file, line) && lineCount < MAX_PRODUCTS) {
        if (line.empty()) continue;

        // Парсим артикул из строки
        stringstream ss(line);
        string token;
        getline(ss, token, '|');

        try {
            int currentArticle = stoi(token);
            if (currentArticle == articleToDelete) {
                found = true;
                cout << "Найден товар для удаления: " << line << endl;
                continue; // Пропускаем эту строку (удаляем)
            }
        }
        catch (const std::exception& e) {
            // Если не удалось распарсить, сохраняем строку как есть
        }

        lines[lineCount++] = line;//сохранение всех строк, кроме удаляемой
    }
    file.close();

    if (!found) {
        cout << "Товар с артикулом " << articleToDelete << " не найден!" << endl;
        return false;//если не найден товар для удаления
    }

    // Перезаписываем файл без удаленного товара
    ofstream outFile(filename);
    if (!outFile.is_open()) {//открытие файла для перезаписи
        cout << "Ошибка открытия файла для записи!" << endl;
        return false;
    }

    for (int i = 0; i < lineCount; i++) {
        outFile << lines[i] << endl;
    }
    outFile.close();

    cout << "Товар с артикулом " << articleToDelete << " успешно удален!" << endl;
    return true;
}

// функция удаления одного товара
void Product::DeleteProductsFromFile(const std::string& filename) {
    cout << "\n=== УДАЛЕНИЕ ТОВАРОВ СО СКЛАДА ===" << endl;

    // Сначала показываем текущие товары
    DisplayProductsTable(filename);

    int productCount;
    // Запрос количества товаров для удаления
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

    // Подтверждение удаления
    char confirmation;
    cout << "\nВы уверены, что хотите удалить " << productCount << " товар(ов)? (y/n): ";
    cin >> confirmation;

    if (confirmation != 'y' && confirmation != 'Y') {
        cout << "Удаление отменено." << endl;
        return;
    }

    // Удаление товаров по артикулам
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

    // Показываем обновленную таблицу
    cout << "\n=== ОБНОВЛЕННЫЙ СПИСОК ТОВАРОВ ===" << endl;
    DisplayProductsTable(filename);
}

// ввод данных с клавиатуры
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
        // Явная проверка на отрицательные числа
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
        else {
            cout << "Ошибка! Введите только 1 или 0." << endl;
        }
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

// запись в файл
void Product::WriteToFile(const std::string& filename) {
    ofstream file(filename, ios::app);//добавление новых данных в конец файла
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

// чтение из файла
void Product::ReadFromFile(const std::string& filename) {
    ifstream file(filename);
    if (file.is_open()) {
        char delimiter;//чтение данных до разделителя |
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

// информация о товаре
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

// отображение товаров в виде таблицы
void Product::DisplayProductsTable(const std::string& filename) {
    ifstream file(filename);
    if (!file.is_open()) {//существование файла
        cout << "Файл " << filename << " не найден!" << endl;
        cout << "Сначала добавьте товары через меню 'Добавить товар'" << endl;
        return;
    }

    if (file.peek() == ifstream::traits_type::eof()) {//проверка не пустой ли файл
        cout << "Файл " << filename << " пуст!" << endl;
        cout << "Сначала добавьте товары через меню 'Добавить товар'" << endl;
        file.close();
        return;
    }

    cout << "\n=== ИНФОРМАЦИЯ О ТОВАРАХ ===" << endl;
    string line;
    int productCount = 0;

    const int MAX_PRODUCTS = 1000;
    string lines[MAX_PRODUCTS];
    int lineCount = 0;

    while (getline(file, line) && lineCount < MAX_PRODUCTS) {
        if (line.empty()) continue;
        lines[lineCount++] = line;
    }
    file.close();

    for (int i = 0; i < lineCount; i++) {
        string line = lines[i];
        stringstream ss(line);
        string token;

        const int MAX_TOKENS = 10;
        string tokens[MAX_TOKENS];
        int tokenCount = 0;

        while (getline(ss, token, '|') && tokenCount < MAX_TOKENS) {
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

                cout << "\n--- Товар " << (productCount + 1) << " ---" << endl;
                cout << "Артикул: " << article << endl;
                cout << "Название: " << name << endl;
                cout << "Бренд: " << brand << endl;
                cout << "Страна: " << country << endl;
                cout << "Категория: " << category << endl;
                cout << "Тип: " << type << endl;
                cout << "Описание: " << description << endl;
                cout << "Количество: " << quantity << endl;
                cout << "Статус: " << (status ? "В наличии" : "Отсутствует") << endl;
                cout << "Цена: " << price << endl;

                productCount++;
            }
            catch (const std::exception& e) {
                cout << "Ошибка чтения строки: " << line << endl;
                continue;
            }
        }
    }

    cout << "\n=================================" << endl;
    cout << "Всего товаров: " << productCount << endl;
    cout << "=================================" << endl;

    if (productCount == 0) {
        cout << "В файле нет корректных данных о товарах!" << endl;
    }
}