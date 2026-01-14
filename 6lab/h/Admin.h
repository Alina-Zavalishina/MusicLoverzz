#ifndef ADMIN_HPP
#define ADMIN_HPP

#include "User.h"
#include <string>
#include <vector>

class Admin : public User {
private:
    std::string department;
    std::string position;
    std::vector<std::string> permissions;
    bool isSuperAdmin;

public:
    // Конструкторы
    Admin(const std::string& name,
        const std::string& department = "Администрация",
        const std::string& position = "Администратор");

    Admin(const std::string& id, const std::string& name,
        const std::string& email, const std::string& phone,
        const std::string& department, const std::string& position,
        bool isSuperAdmin = false);

    ~Admin();

    // 1. ПЕРЕОПРЕДЕЛЕНИЕ (override) метода базового класса
    void displayInfo() const override;

    // 2. ПЕРЕГРУЗКИ (overload) метода базового класса
    // С вызовом метода базового класса
    void displayInfo(bool showTechnicalDetails) const;

    // Без вызова метода базового класса
    void displayInfo(const std::string& reportFormat) const;

    // 3. ПЕРЕОПРЕДЕЛЕНИЕ метода calculateDiscount
    double calculateDiscount() const override;

    // 4. ПЕРЕГРУЗКА метода calculateDiscount
    double calculateDiscount(bool applyBonus) const;

    // 5. ПЕРЕГРУЗКА оператора присваивания от объекта базового класса
    Admin& operator=(const User& user);

    // 6. Перегрузка оператора присваивания от другого Admin
    Admin& operator=(const Admin& admin);

    void addPermission(const std::string& permission);
    void removePermission(const std::string& permission);
    bool hasPermission(const std::string& permission) const;
    void showPermissions() const;

    // Геттеры
    std::string getDepartment() const { return department; }
    std::string getPosition() const { return position; }
    bool getIsSuperAdmin() const { return isSuperAdmin; }
    const std::vector<std::string>& getPermissions() const { return permissions; }

    // Сеттеры
    void setDepartment(const std::string& newDept) { department = newDept; }
    void setPosition(const std::string& newPos) { position = newPos; }
    void setIsSuperAdmin(bool superAdmin) { isSuperAdmin = superAdmin; }

private:
    void initializeAdminData(const std::string& department,
        const std::string& position,
        bool isSuperAdmin);
};

// Глобальный оператор вывода для Admin
std::ostream& operator<<(std::ostream& os, const Admin& admin);

#endif // ADMIN_HPP
#pragma once
