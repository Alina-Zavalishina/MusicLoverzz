#include "Admin.h"
#include <iostream>
#include <algorithm>
#include <string>

using namespace std;

Admin::Admin(const std::string& name, const std::string& department,
    const std::string& position)
    : User(generateUserId("ADM"), name, "", ""),
    department(department), position(position), isSuperAdmin(false) {
    permissions.push_back("view_products");
    permissions.push_back("view_orders");
}


Admin::Admin(const std::string& id, const std::string& name,
    const std::string& email, const std::string& phone,
    const std::string& department, const std::string& position,
    bool isSuperAdmin, const std::string& regDate)
    : User(id, name, email, phone, regDate),
    department(department), position(position), isSuperAdmin(isSuperAdmin) {
    if (isSuperAdmin) {
        permissions = { "view_products", "edit_products", "delete_products",
                       "view_orders", "manage_orders", "view_users",
                       "manage_users", "system_settings" };
    }
    else {
        permissions = { "view_products", "edit_products", "view_orders", "manage_orders" };
    }
}

// Деструктор
Admin::~Admin() {}

// РЕАЛИЗАЦИЯ ОПЕРАТОРОВ 

void Admin::displayInfo() const {
    cout << *this;
}

Admin& Admin::operator+=(const std::string& permission) {
    if (!hasPermission(permission)) {
        permissions.push_back(permission);
    }
    return *this;
}

Admin& Admin::operator-=(const std::string& permission) {
    auto it = find(permissions.begin(), permissions.end(), permission);
    if (it != permissions.end()) {
        permissions.erase(it);
    }
    return *this;
}

Admin& Admin::operator^=(const std::string& permission) {
    if (hasPermission(permission)) {
        *this -= permission;
    }
    else {
        *this += permission;
    }
    return *this;
}

std::string Admin::operator[](size_t index) const {
    if (index < permissions.size()) {
        return permissions[index];
    }
    return "";
}

void Admin::operator()(const std::string& command) const {
    cout << "\nАдминистратор " << getName() << " выполняет команду: " << command << endl;
    if (command == "show_info") {
        displayInfo();
    }
    else if (command == "show_permissions") {
        showPermissions();
    }
    else if (command == "catalog") {
        manageProductCatalog();
    }
    else {
        cout << "Неизвестная команда!" << endl;
    }
}

bool Admin::operator!() const {
    return !isSuperAdmin && permissions.empty();
}

bool Admin::operator<(const Admin& other) const {
    if (isSuperAdmin != other.isSuperAdmin) {
        return !isSuperAdmin;
    }
    return permissions.size() < other.permissions.size();
}

bool Admin::operator>(const Admin& other) const {
    return other < *this;
}

// ДРУГИЕ МЕТОДЫ

void Admin::addPermission(const std::string& permission) {
    *this += permission;
}

void Admin::removePermission(const std::string& permission) {
    *this -= permission;
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

void Admin::resetPassword(const std::string& userId) {
    if (hasPermission("manage_users") || isSuperAdmin) {
        cout << "Сброс пароля для пользователя с ID: " << userId << endl;
        cout << "Новый пароль сгенерирован: Admin" << userId.substr(0, 3) << "!" << endl;
    }
    else {
        cout << "Ошибка: недостаточно прав для сброса пароля!" << endl;
    }
}

void Admin::manageProductCatalog() const {
    if (hasPermission("edit_products") || isSuperAdmin) {
        cout << "Управление каталогом товаров..." << endl;
        cout << "1. Добавить товар" << endl;
        cout << "2. Редактировать товар" << endl;
        cout << "3. Удалить товар" << endl;
        cout << "4. Обновить склад" << endl;
    }
    else {
        cout << "Ошибка: недостаточно прав для управления каталогом!" << endl;
    }
}

void Admin::initializeAdminSpecificData(const std::string& department,
    const std::string& position,
    bool isSuperAdmin) {
    this->department = department;
    this->position = position;
    this->isSuperAdmin = isSuperAdmin;
}



std::ostream& operator<<(std::ostream& os, const Admin& admin) {
    os << static_cast<const User&>(admin);
    os << "\n--- СЛУЖЕБНАЯ ИНФОРМАЦИЯ ---" << endl;
    os << "Отдел: " << admin.getDepartment() << endl;
    os << "Должность: " << admin.getPosition() << endl;
    os << "Тип: " << (admin.getIsSuperAdmin() ? "Супер-администратор" : "Обычный администратор") << endl;
    os << "Количество прав доступа: " << admin.getPermissionCount() << endl;
    os << "==================================" << endl;
    return os;
}

bool operator==(const Admin& a1, const Admin& a2) {
    return a1.getId() == a2.getId() &&
        a1.getIsSuperAdmin() == a2.getIsSuperAdmin() &&
        a1.getPermissionCount() == a2.getPermissionCount();
}
