#define _CRT_SECURE_NO_WARNINGS
#include "Admin.h"
#include <iostream>
#include <algorithm>
#include <string>

using namespace std;

// Конструктор 1: с вызовом конструктора базового класса User с параметрами
Admin::Admin(const std::string& name,
    const std::string& department,
    const std::string& position)
    : User(generateUserId("ADM"), name, "", ""),
    department(department), position(position), isSuperAdmin(false) {

    // Инициализация прав доступа
    permissions.push_back("view_products");
    permissions.push_back("view_orders");
    permissions.push_back("edit_products");
}

Admin::Admin(const std::string& id, const std::string& name,
    const std::string& email, const std::string& phone,
    const std::string& department, const std::string& position,
    bool isSuperAdmin)
    // ДЕМОНСТРАЦИЯ: вызов конструктора базового класса User с параметрами
    : User(id, name, email, phone),
    department(department), position(position), isSuperAdmin(isSuperAdmin) {

    initializeAdminData(department, position, isSuperAdmin);
}

// Деструктор
Admin::~Admin() {
}

// 1. ПЕРЕОПРЕДЕЛЕНИЕ метода displayInfo С ВЫЗОВОМ базового метода
void Admin::displayInfo() const {
    // Сначала выводим базовую информацию через вызов базового метода
    cout << "\n=== ИНФОРМАЦИЯ ОБ АДМИНИСТРАТОРЕ ===" << endl;
    User::showBasicInfo();  // ← ВЫЗОВ БАЗОВОГО МЕТОДА

    cout << "\n--- СЛУЖЕБНАЯ ИНФОРМАЦИЯ ---" << endl;
    cout << "Отдел: " << department << endl;
    cout << "Должность: " << position << endl;
    cout << "Тип: " << (isSuperAdmin ? "Супер-администратор" : "Обычный администратор") << endl;
    cout << "Количество прав доступа: " << permissions.size() << endl;

    if (!permissions.empty()) {
        cout << "Права доступа: ";
        for (size_t i = 0; i < permissions.size(); ++i) {
            cout << permissions[i];
            if (i < permissions.size() - 1) cout << ", ";
        }
        cout << endl;
    }
    cout << "Скидка: " << calculateDiscount() << "%" << endl;
    cout << "==================================" << endl;
}

// 2. ПЕРЕГРУЗКА  displayInfo с параметром bool - С ВЫЗОВОМ БАЗОВОГО МЕТОДА
void Admin::displayInfo(bool showTechnicalDetails) const {
    cout << "\n=== ТЕХНИЧЕСКАЯ ИНФОРМАЦИЯ АДМИНИСТРАТОРА ===" << endl;

    User::displayInfo();

    cout << "Отдел: " << department << endl;
    cout << "Должность: " << position << endl;

    if (showTechnicalDetails) {
        cout << "\n--- ТЕХНИЧЕСКИЕ ДАННЫЕ ---" << endl;
        cout << "Супер-админ: " << (isSuperAdmin ? "Да" : "Нет") << endl;
        cout << "Количество прав: " << permissions.size() << endl;
        cout << "Размер объекта: " << sizeof(*this) << " байт" << endl;
    }
    cout << "==================================" << endl;
}

// 3. ПЕРЕГРУЗКА БЕЗ ВЫЗОВА БАЗОВОГО МЕТОДА
void Admin::displayInfo(const std::string& reportFormat) const {

    if (reportFormat == "short") {
        cout << "[КРАТКИЙ ОТЧЕТ] Админ: " << getName()
            << " (" << department << ", " << position << ")" << endl;
        cout << "Прав доступа: " << permissions.size() << endl;
    }
    else if (reportFormat == "json") {
        cout << "{" << endl;
        cout << "  \"admin\": {" << endl;
        cout << "    \"id\": \"" << getId() << "\"," << endl;
        cout << "    \"name\": \"" << getName() << "\"," << endl;
        cout << "    \"department\": \"" << department << "\"," << endl;
        cout << "    \"position\": \"" << position << "\"," << endl;
        cout << "    \"is_super_admin\": " << (isSuperAdmin ? "true" : "false") << "," << endl;
        cout << "    \"permissions_count\": " << permissions.size() << endl;
        cout << "  }" << endl;
        cout << "}" << endl;
    }
    else {
        cout << "=== ОТЧЕТ ДЛЯ АДМИНИСТРАТОРА ===" << endl;
        cout << "Имя: " << getName() << endl;
        cout << "Отдел: " << department << endl;
        cout << "Должность: " << position << endl;
        cout << "Тип: " << (isSuperAdmin ? "Супер-администратор" : "Обычный администратор") << endl;
        cout << "Количество прав: " << permissions.size() << endl;

    }
}

// 4. ПЕРЕОПРЕДЕЛЕНИЕ метода calculateDiscount БЕЗ вызова базового метода
double Admin::calculateDiscount() const {

    return 15.0;
}

// 5. ПЕРЕГРУЗКА метода calculateDiscount с параметром bool
double Admin::calculateDiscount(bool applyBonus) const {


    if (applyBonus) {
        if (isSuperAdmin) {
            return 25.0;
        }
        else {
            return 20.0;
        }
    }
    return 15.0;
}

// 6. ПЕРЕГРУЗКА оператора присваивания от объекта базового класса User
Admin& Admin::operator=(const User& user) {
    if (this != &user) {
        // Копируем данные из базового класса
        User::operator=(user); // Вызываем оператор присваивания базового класса

        // Устанавливаем значения по умолчанию для специфичных полей Admin
        department = "Администрация (присвоено)";
        position = "Администратор (присвоено)";
        isSuperAdmin = false;

        // Очищаем права доступа и добавляем базовые
        permissions.clear();
        permissions.push_back("assigned_from_user");
    }
    return *this;
}

// Перегрузка оператора присваивания от другого Admin
Admin& Admin::operator=(const Admin& admin) {
    if (this != &admin) {
        // Копируем данные из базового класса
        User::operator=(admin);

        // Копируем специфичные поля Admin
        department = admin.department + " (копия)";
        position = admin.position + " (копия)";
        isSuperAdmin = admin.isSuperAdmin;
        permissions = admin.permissions;
    }
    return *this;
}

// Методы администратора
void Admin::addPermission(const std::string& permission) {
    if (!hasPermission(permission)) {
        permissions.push_back(permission);
        cout << "Право доступа добавлено: " << permission << endl;
    }
}

void Admin::removePermission(const std::string& permission) {
    auto it = find(permissions.begin(), permissions.end(), permission);
    if (it != permissions.end()) {
        permissions.erase(it);
        cout << "Право доступа удалено: " << permission << endl;
    }
}

bool Admin::hasPermission(const std::string& permission) const {
    return find(permissions.begin(), permissions.end(), permission) != permissions.end();
}

void Admin::showPermissions() const {
    if (permissions.empty()) {
        cout << "Нет прав доступа!" << endl;
        return;
    }

    cout << "\n=== ПРАВА ДОСТУПА ===" << endl;
    for (size_t i = 0; i < permissions.size(); i++) {
        cout << (i + 1) << ". " << permissions[i] << endl;
    }
    cout << "=====================" << endl;
}

void Admin::initializeAdminData(const std::string& department,
    const std::string& position,
    bool isSuperAdmin) {

    this->department = department;
    this->position = position;
    this->isSuperAdmin = isSuperAdmin;

    if (isSuperAdmin) {
        permissions = { "view_products", "edit_products", "delete_products",
                       "view_orders", "manage_orders", "view_users",
                       "manage_users", "system_settings" };
    }
    else {
        permissions = { "view_products", "edit_products", "view_orders", "manage_orders" };
    }
}


std::ostream& operator<<(std::ostream& os, const Admin& admin) {
    const User& userRef = admin;
    os << userRef;

    os << "\n--- ДОПОЛНИТЕЛЬНАЯ ИНФОРМАЦИЯ (Admin) ---" << endl;
    os << "Отдел: " << admin.getDepartment() << endl;
    os << "Должность: " << admin.getPosition() << endl;
    os << "Тип: " << (admin.getIsSuperAdmin() ? "Супер-администратор" : "Обычный администратор") << endl;
    os << "Количество прав: " << admin.getPermissions().size() << endl;
    os << "Скидка: " << admin.calculateDiscount() << "%" << endl;
    os << "==================================" << endl;

    return os;
}