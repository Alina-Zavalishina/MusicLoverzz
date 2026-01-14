#include "Search.h"
#include <algorithm>
#include <cctype>
#include <iomanip>
#include <limits>

using namespace std;

Search::Search() : resultCount(0), historyCount(0) {}

// преобразование к нижнему регистру
std::string Search::toLower(const std::string& str) const {
    std::string result = str;
    std::transform(result.begin(), result.end(), result.begin(), ::tolower);
    return result;
}
//удаление пробелов
std::string Search::trim(const std::string& str) const {
    size_t start = str.find_first_not_of(" \t\n\r");
    if (start == std::string::npos) return "";

    size_t end = str.find_last_not_of(" \t\n\r");
    return str.substr(start, end - start + 1);
}
//подстрока
bool Search::contains(const std::string& str, const std::string& substr) const {
    if (substr.empty()) return true;
    return toLower(str).find(toLower(substr)) != std::string::npos;
}
// Метод для объединения массива строк в одну строку с разделителем
std::string Search::joinStrings(const std::string parts[], int count, const std::string& delimiter) const {
    if (count == 0) return "";

    std::string result = parts[0];
    for (int i = 1; i < count; ++i) {
        result += delimiter + parts[i];
    }
    return result;
}

void Search::ClearResults() {
    resultCount = 0;
}

void Search::AddToHistory(const std::string& searchInfo) {
    if (historyCount >= MAX_HISTORY) {
        for (int i = 0; i < MAX_HISTORY - 1; i++) {
            searchHistory[i] = searchHistory[i + 1];
        }
        historyCount = MAX_HISTORY - 1;
    }

    searchHistory[historyCount] = searchInfo;
    historyCount++;
}

void Search::DisplaySearchHistory() const {
    cout << "\n=== ИСТОРИЯ ПОИСКА ===" << endl;

    if (historyCount == 0) {
        cout << "История поиска пуста!" << endl;
        cout << "==========================" << endl;
        return;
    }

    cout << "Последние " << historyCount << " запросов:" << endl;
    cout << "==========================" << endl;

    for (int i = historyCount - 1; i >= 0; i--) {
        cout << "Запрос #" << (historyCount - i) << ": " << searchHistory[i] << endl;
    }
    cout << "==========================" << endl;
}

std::string Search::getSearchHistoryAsString() const {
    if (historyCount == 0) return "История поиска пуста";

    std::string result = "История поиска (" + std::to_string(historyCount) + " запросов):\n";
    for (int i = 0; i < historyCount; i++) {
        result += std::to_string(i + 1) + ". " + searchHistory[i] + "\n";
    }
    return result;
}

void Search::clearSearchHistory() {
    for (int i = 0; i < MAX_HISTORY; i++) {
        searchHistory[i] = "";
    }
    historyCount = 0;
    cout << "История поиска очищена!" << endl;
}

// Поиск по названию
void Search::SearchByName(const std::string& filename, const std::string& name) {
    ClearResults();
    request = "Поиск по названию: '" + name + "'";

    ifstream file(filename);
    if (!file.is_open()) {
        cout << "Файл " << filename << " не найден!" << endl;
        return;
    }

    string line;
    string searchName = trim(name);

    while (getline(file, line) && resultCount < MAX_RESULTS) {
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
                string productName = tokens[1];

                if (contains(productName, searchName)) {
                    searchResults[resultCount].article = stoi(tokens[0]);
                    searchResults[resultCount].name = tokens[1];
                    searchResults[resultCount].brand = tokens[2];
                    searchResults[resultCount].producerCountry = tokens[3];
                    searchResults[resultCount].category = tokens[4];
                    searchResults[resultCount].typeProduct = tokens[5];
                    searchResults[resultCount].description = tokens[6];
                    searchResults[resultCount].quantityStock = stoi(tokens[7]);
                    searchResults[resultCount].statusStock = (tokens[8] == "1");
                    searchResults[resultCount].price = stoi(tokens[9]);
                    resultCount++;
                }
            }
            catch (const exception& e) {
                continue;
            }
        }
    }

    file.close();

    string historyEntry = "По названию: '" + name + "' - найдено: " + to_string(resultCount);
    AddToHistory(historyEntry);

    DisplayResultCount();
}

// Поиск по артикулу
void Search::SearchByArticle(const std::string& filename, int article) {
    ClearResults();
    request = "Поиск по артикулу: " + to_string(article);

    ifstream file(filename);
    if (!file.is_open()) {
        cout << "Файл " << filename << " не найден!" << endl;
        return;
    }

    string line;

    while (getline(file, line) && resultCount < MAX_RESULTS) {
        if (line.empty()) continue;

        stringstream ss(line);
        string token;
        getline(ss, token, '|');

        try {
            int productArticle = stoi(token);
            if (productArticle == article) {
                string tokens[10];
                tokens[0] = token;
                int tokenCount = 1;

                while (getline(ss, token, '|') && tokenCount < 10) {
                    tokens[tokenCount++] = token;
                }

                if (tokenCount == 10) {
                    searchResults[resultCount].article = stoi(tokens[0]);
                    searchResults[resultCount].name = tokens[1];
                    searchResults[resultCount].brand = tokens[2];
                    searchResults[resultCount].producerCountry = tokens[3];
                    searchResults[resultCount].category = tokens[4];
                    searchResults[resultCount].typeProduct = tokens[5];
                    searchResults[resultCount].description = tokens[6];
                    searchResults[resultCount].quantityStock = stoi(tokens[7]);
                    searchResults[resultCount].statusStock = (tokens[8] == "1");
                    searchResults[resultCount].price = stoi(tokens[9]);
                    resultCount++;
                }
                break;
            }
        }
        catch (const exception& e) {
            continue;
        }
    }

    file.close();

    string historyEntry = "По артикулу: " + to_string(article) + " - найдено: " + to_string(resultCount);
    AddToHistory(historyEntry);

    DisplayResultCount();
}

// Поиск по бренду
void Search::SearchByBrand(const std::string& filename, const std::string& brand) {
    ClearResults();
    request = "Поиск по бренду: '" + brand + "'";

    ifstream file(filename);
    if (!file.is_open()) {
        cout << "Файл " << filename << " не найден!" << endl;
        return;
    }

    string line;
    string searchBrand = trim(brand);

    while (getline(file, line) && resultCount < MAX_RESULTS) {
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
                string productBrand = tokens[2];

                if (contains(productBrand, searchBrand)) {
                    searchResults[resultCount].article = stoi(tokens[0]);
                    searchResults[resultCount].name = tokens[1];
                    searchResults[resultCount].brand = tokens[2];
                    searchResults[resultCount].producerCountry = tokens[3];
                    searchResults[resultCount].category = tokens[4];
                    searchResults[resultCount].typeProduct = tokens[5];
                    searchResults[resultCount].description = tokens[6];
                    searchResults[resultCount].quantityStock = stoi(tokens[7]);
                    searchResults[resultCount].statusStock = (tokens[8] == "1");
                    searchResults[resultCount].price = stoi(tokens[9]);
                    resultCount++;
                }
            }
            catch (const exception& e) {
                continue;
            }
        }
    }

    file.close();

    string historyEntry = "По бренду: '" + brand + "' - найдено: " + to_string(resultCount);
    AddToHistory(historyEntry);

    DisplayResultCount();
}

// Поиск по стране-производителю
void Search::SearchByCountry(const std::string& filename, const std::string& country) {
    ClearResults();
    request = "Поиск по стране: '" + country + "'";

    ifstream file(filename);
    if (!file.is_open()) {
        cout << "Файл " << filename << " не найден!" << endl;
        return;
    }

    string line;
    string searchCountry = trim(country);

    while (getline(file, line) && resultCount < MAX_RESULTS) {
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
                string productCountry = tokens[3];

                if (contains(productCountry, searchCountry)) {
                    searchResults[resultCount].article = stoi(tokens[0]);
                    searchResults[resultCount].name = tokens[1];
                    searchResults[resultCount].brand = tokens[2];
                    searchResults[resultCount].producerCountry = tokens[3];
                    searchResults[resultCount].category = tokens[4];
                    searchResults[resultCount].typeProduct = tokens[5];
                    searchResults[resultCount].description = tokens[6];
                    searchResults[resultCount].quantityStock = stoi(tokens[7]);
                    searchResults[resultCount].statusStock = (tokens[8] == "1");
                    searchResults[resultCount].price = stoi(tokens[9]);
                    resultCount++;
                }
            }
            catch (const exception& e) {
                continue;
            }
        }
    }

    file.close();

    string historyEntry = "По стране: '" + country + "' - найдено: " + to_string(resultCount);
    AddToHistory(historyEntry);

    DisplayResultCount();
}

// Поиск по категории
void Search::SearchByCategory(const std::string& filename, const std::string& category) {
    ClearResults();
    request = "Поиск по категории: '" + category + "'";

    ifstream file(filename);
    if (!file.is_open()) {
        cout << "Файл " << filename << " не найден!" << endl;
        return;
    }

    string line;
    string searchCategory = trim(category);

    while (getline(file, line) && resultCount < MAX_RESULTS) {
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
                string productCategory = tokens[4];

                if (contains(productCategory, searchCategory)) {
                    searchResults[resultCount].article = stoi(tokens[0]);
                    searchResults[resultCount].name = tokens[1];
                    searchResults[resultCount].brand = tokens[2];
                    searchResults[resultCount].producerCountry = tokens[3];
                    searchResults[resultCount].category = tokens[4];
                    searchResults[resultCount].typeProduct = tokens[5];
                    searchResults[resultCount].description = tokens[6];
                    searchResults[resultCount].quantityStock = stoi(tokens[7]);
                    searchResults[resultCount].statusStock = (tokens[8] == "1");
                    searchResults[resultCount].price = stoi(tokens[9]);
                    resultCount++;
                }
            }
            catch (const exception& e) {
                continue;
            }
        }
    }

    file.close();

    string historyEntry = "По категории: '" + category + "' - найдено: " + to_string(resultCount);
    AddToHistory(historyEntry);

    DisplayResultCount();
}

// Поиск по типу товара
void Search::SearchByType(const std::string& filename, const std::string& type) {
    ClearResults();
    request = "Поиск по типу: '" + type + "'";

    ifstream file(filename);
    if (!file.is_open()) {
        cout << "Файл " << filename << " не найден!" << endl;
        return;
    }

    string line;
    string searchType = trim(type);

    while (getline(file, line) && resultCount < MAX_RESULTS) {
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
                string productType = tokens[5];

                if (contains(productType, searchType)) {
                    searchResults[resultCount].article = stoi(tokens[0]);
                    searchResults[resultCount].name = tokens[1];
                    searchResults[resultCount].brand = tokens[2];
                    searchResults[resultCount].producerCountry = tokens[3];
                    searchResults[resultCount].category = tokens[4];
                    searchResults[resultCount].typeProduct = tokens[5];
                    searchResults[resultCount].description = tokens[6];
                    searchResults[resultCount].quantityStock = stoi(tokens[7]);
                    searchResults[resultCount].statusStock = (tokens[8] == "1");
                    searchResults[resultCount].price = stoi(tokens[9]);
                    resultCount++;
                }
            }
            catch (const exception& e) {
                continue;
            }
        }
    }

    file.close();

    string historyEntry = "По типу: '" + type + "' - найдено: " + to_string(resultCount);
    AddToHistory(historyEntry);

    DisplayResultCount();
}

// Поиск по цене
void Search::SearchByPrice(const std::string& filename, int minPrice, int maxPrice) {
    ClearResults();
    request = "Поиск по цене: от " + to_string(minPrice) + " до " + to_string(maxPrice);

    ifstream file(filename);
    if (!file.is_open()) {
        cout << "Файл " << filename << " не найден!" << endl;
        return;
    }

    string line;

    while (getline(file, line) && resultCount < MAX_RESULTS) {
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
                int productPrice = stoi(tokens[9]);
                if (productPrice >= minPrice && productPrice <= maxPrice) {
                    searchResults[resultCount].article = stoi(tokens[0]);
                    searchResults[resultCount].name = tokens[1];
                    searchResults[resultCount].brand = tokens[2];
                    searchResults[resultCount].producerCountry = tokens[3];
                    searchResults[resultCount].category = tokens[4];
                    searchResults[resultCount].typeProduct = tokens[5];
                    searchResults[resultCount].description = tokens[6];
                    searchResults[resultCount].quantityStock = stoi(tokens[7]);
                    searchResults[resultCount].statusStock = (tokens[8] == "1");
                    searchResults[resultCount].price = productPrice;
                    resultCount++;
                }
            }
            catch (const exception& e) {
                continue;
            }
        }
    }

    file.close();

    string historyEntry = "По цене: от " + to_string(minPrice) + " до " +
        to_string(maxPrice) + " - найдено: " + to_string(resultCount);
    AddToHistory(historyEntry);

    DisplayResultCount();
}

// Универсальный поиск
void Search::UniversalSearch(const std::string& filename) {
    cout << "\n=== УНИВЕРСАЛЬНЫЙ ПОИСК ===" << endl;
    cout << "Выберите критерии поиска (введите номера через запятую):" << endl;
    cout << "1. По названию" << endl;
    cout << "2. По артикулу" << endl;
    cout << "3. По бренду" << endl;
    cout << "4. По стране-производителю" << endl;
    cout << "5. По категории" << endl;
    cout << "6. По типу товара" << endl;
    cout << "7. По цене" << endl;
    cout << "Пример: 2,3 (поиск по артикулу и бренду)" << endl;
    cout << "Ваш выбор: ";

    string criteriaInput;
    cin.ignore();
    getline(cin, criteriaInput);

    bool selectedCriteria[7] = { false };
    int criteriaCount = 0;
    stringstream ss(criteriaInput);
    string token;

    while (getline(ss, token, ',')) {
        try {
            int criterion = stoi(trim(token));
            if (criterion >= 1 && criterion <= 7) {
                selectedCriteria[criterion - 1] = true;
                criteriaCount++;
            }
        }
        catch (...) {
            // Игнорируем ошибки
        }
    }

    if (criteriaCount == 0) {
        cout << "Не выбрано ни одного критерия поиска!" << endl;
        return;
    }

    // Сбор параметров поиска
    string searchParams[7] = { "" };
    int searchArticle = 0;
    int minPrice = 0, maxPrice = 0;

    for (int i = 0; i < 7; i++) {
        if (selectedCriteria[i]) {
            switch (i + 1) {
            case 1:
                cout << "Введите название для поиска: ";
                getline(cin, searchParams[0]);
                break;
            case 2:
                while (true) {
                    cout << "Введите артикул для поиска: ";
                    string input;
                    getline(cin, input);
                    try {
                        searchArticle = stoi(trim(input));
                        break;
                    }
                    catch (...) {
                        cout << "Неверный ввод! Пожалуйста, введите целое число." << endl;
                    }
                }
                break;
            case 3:
                cout << "Введите бренд для поиска: ";
                getline(cin, searchParams[2]);
                break;
            case 4:
                cout << "Введите страну-производитель для поиска: ";
                getline(cin, searchParams[3]);
                break;
            case 5:
                cout << "Введите категорию для поиска: ";
                getline(cin, searchParams[4]);
                break;
            case 6:
                cout << "Введите тип товара для поиска: ";
                getline(cin, searchParams[5]);
                break;
            case 7:
                while (true) {
                    cout << "Введите минимальную цену: ";
                    string input;
                    getline(cin, input);
                    try {
                        minPrice = stoi(trim(input));
                        break;
                    }
                    catch (...) {
                        cout << "Неверный ввод! Пожалуйста, введите целое число." << endl;
                    }
                }
                while (true) {
                    cout << "Введите максимальную цену: ";
                    string input;
                    getline(cin, input);
                    try {
                        maxPrice = stoi(trim(input));
                        break;
                    }
                    catch (...) {
                        cout << "Неверный ввод! Пожалуйста, введите целое число." << endl;
                    }
                }
                break;
            }
        }
    }

    // Выполнение поиска
    Search finalResults;
    ifstream file(filename);

    if (!file.is_open()) {
        cout << "Файл " << filename << " не найден!" << endl;
        return;
    }

    string line;
    while (getline(file, line) && finalResults.resultCount < MAX_RESULTS) {
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
                bool matchesAll = true;

                for (int i = 0; i < 7 && matchesAll; i++) {
                    if (selectedCriteria[i]) {
                        switch (i + 1) {
                        case 1:
                            if (!contains(tokens[1], searchParams[0])) {
                                matchesAll = false;
                            }
                            break;
                        case 2:
                            if (stoi(tokens[0]) != searchArticle) {
                                matchesAll = false;
                            }
                            break;
                        case 3:
                            if (!contains(tokens[2], searchParams[2])) {
                                matchesAll = false;
                            }
                            break;
                        case 4:
                            if (!contains(tokens[3], searchParams[3])) {
                                matchesAll = false;
                            }
                            break;
                        case 5:
                            if (!contains(tokens[4], searchParams[4])) {
                                matchesAll = false;
                            }
                            break;
                        case 6:
                            if (!contains(tokens[5], searchParams[5])) {
                                matchesAll = false;
                            }
                            break;
                        case 7:
                            int price = stoi(tokens[9]);
                            if (price < minPrice || price > maxPrice) {
                                matchesAll = false;
                            }
                            break;
                        }
                    }
                }

                if (matchesAll) {
                    finalResults.searchResults[finalResults.resultCount].article = stoi(tokens[0]);
                    finalResults.searchResults[finalResults.resultCount].name = tokens[1];
                    finalResults.searchResults[finalResults.resultCount].brand = tokens[2];
                    finalResults.searchResults[finalResults.resultCount].producerCountry = tokens[3];
                    finalResults.searchResults[finalResults.resultCount].category = tokens[4];
                    finalResults.searchResults[finalResults.resultCount].typeProduct = tokens[5];
                    finalResults.searchResults[finalResults.resultCount].description = tokens[6];
                    finalResults.searchResults[finalResults.resultCount].quantityStock = stoi(tokens[7]);
                    finalResults.searchResults[finalResults.resultCount].statusStock = (tokens[8] == "1");
                    finalResults.searchResults[finalResults.resultCount].price = stoi(tokens[9]);
                    finalResults.resultCount++;
                }
            }
            catch (...) {
                continue;
            }
        }
    }

    file.close();

    cout << "\n=== РЕЗУЛЬТАТЫ УНИВЕРСАЛЬНОГО ПОИСКА ===" << endl;
    if (finalResults.resultCount == 0) {
        cout << "Товары не найдены!" << endl;
    }
    else {
        // Создаем описание поиска для истории
        const int MAX_CRITERIA_DESC = 7;
        string criteriaDesc[MAX_CRITERIA_DESC];
        int descCount = 0;

        if (selectedCriteria[0]) {
            criteriaDesc[descCount++] = "названию: '" + searchParams[0] + "'";
        }
        if (selectedCriteria[1]) {
            criteriaDesc[descCount++] = "артикулу: " + to_string(searchArticle);
        }
        if (selectedCriteria[2]) {
            criteriaDesc[descCount++] = "бренду: '" + searchParams[2] + "'";
        }
        if (selectedCriteria[3]) {
            criteriaDesc[descCount++] = "стране: '" + searchParams[3] + "'";
        }
        if (selectedCriteria[4]) {
            criteriaDesc[descCount++] = "категории: '" + searchParams[4] + "'";
        }
        if (selectedCriteria[5]) {
            criteriaDesc[descCount++] = "типу: '" + searchParams[5] + "'";
        }
        if (selectedCriteria[6]) {
            criteriaDesc[descCount++] = "цене: " + to_string(minPrice) + "-" + to_string(maxPrice);
        }

        string criteriaString = joinStrings(criteriaDesc, descCount, ", ");

        string historyEntry = "Универсальный поиск по " + criteriaString +
            " - найдено: " + to_string(finalResults.resultCount);

        finalResults.AddToHistory(historyEntry);

        cout << "Найдено товаров, удовлетворяющих ВСЕМ критериям: " << finalResults.resultCount << endl;
        finalResults.DisplayResults();
    }
}

int Search::DisplayResultCount() const {
    cout << "\n=== КОЛИЧЕСТВО НАЙДЕННЫХ ТОВАРОВ ===" << endl;
    cout << "Найдено товаров: " << resultCount << endl;

    if (resultCount > 0) {
        string resultInfo = "Результаты поиска '" + request + "': ";
        resultInfo += to_string(resultCount) + " товар";

        // Правильное склонение слова "товар"
        int lastTwoDigits = resultCount % 100;
        int lastDigit = resultCount % 10;

        if (lastTwoDigits >= 11 && lastTwoDigits <= 19) {
            resultInfo += "ов";
        }
        else if (lastDigit == 1) {
            // ничего не добавляем
        }
        else if (lastDigit >= 2 && lastDigit <= 4) {
            resultInfo += "а";
        }
        else {
            resultInfo += "ов";
        }

        cout << resultInfo << endl;
    }

    cout << "=====================================" << endl;
    return resultCount;
}

void Search::DisplayResults() const {
    if (resultCount == 0) {
        cout << "\n————————————————————————" << endl;
        cout << "По запросу \"" << request << "\" ничего не найдено." << endl;
        cout << "————————————————————————" << endl;
        return;
    }

    cout << "\n═══════════════════════════════════════════════" << endl;
    cout << "РЕЗУЛЬТАТЫ ПОИСКА (" << resultCount << " товаров)" << endl;
    cout << "Запрос: \"" << request << "\"" << endl;
    cout << "═══════════════════════════════════════════════\n" << endl;

    for (int i = 0; i < resultCount; i++) {
        const auto& product = searchResults[i];

        cout << "┌── ТОВАР №" << (i + 1) << " ───────────────────────" << endl;
        cout << "│  Название:  " << product.name << endl;
        cout << "│  Бренд:     " << product.brand << endl;
        cout << "│  Цена:      " << product.price << " руб." << endl;
        cout << "│  Наличие:   ";

        if (product.statusStock) {
            cout << product.quantityStock << " шт. на складе" << endl;
        }
        else {
            cout << "Товар отсутствует" << endl;
        }

        // Вывод описания с правильным переносом
        if (!product.description.empty()) {
            cout << "│  Описание:  ";

            string desc = product.description;
            const int maxLineLength = 50;
            int charsPrinted = 0;

            // Перенос длинного описания
            while (charsPrinted < desc.length()) {
                int remaining = desc.length() - charsPrinted;
                int printLength = min(maxLineLength, remaining);


                if (charsPrinted == 0) {
                    cout << desc.substr(charsPrinted, printLength);
                }

                else {
                    cout << "\n│             " << desc.substr(charsPrinted, printLength);
                }

                charsPrinted += printLength;

                if (remaining > maxLineLength && charsPrinted >= maxLineLength) {
                    cout << "...";
                    break;
                }
            }
            cout << endl;
        }

        cout << "└─────────────────────────────────────────" << endl;

        // Разделитель между товарами, кроме последнего
        if (i < resultCount - 1) {
            cout << endl;
        }
    }

    cout << "\n═══════════════════════════════════════════════" << endl;

    // Полезные подсказки
    if (resultCount > 0) {
        cout << "СОВЕТ: Для оформления заказа введите номер нужного товара." << endl;

        // Проверяем, есть ли отсутствующие товары
        bool hasOutOfStock = false;
        for (int i = 0; i < resultCount; i++) {
            if (!searchResults[i].statusStock) {
                hasOutOfStock = true;
                break;
            }
        }

        if (hasOutOfStock) {
            cout << "       Отсутствующие товары можно заказать у поставщика." << endl;
        }
    }
    cout << "═══════════════════════════════════════════════" << endl;
}