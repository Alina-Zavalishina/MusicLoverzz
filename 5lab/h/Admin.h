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
        bool isSuperAdmin, const std::string& regDate);

    ~Admin();

    // Переопределение виртуальной функции
    void displayInfo() const override;

    //  ПЕРЕГРУЗКА ОПЕРАТОРОВ
    Admin& operator+=(const std::string& permission);
    Admin& operator-=(const std::string& permission);
    Admin& operator^=(const std::string& permission);
    std::string operator[](size_t index) const;
    void operator()(const std::string& command) const;
    bool operator!() const;
    bool operator<(const Admin& other) const;
    bool operator>(const Admin& other) const;

    //  ДРУГИЕ МЕТОДЫ
    void addPermission(const std::string& permission);
    void removePermission(const std::string& permission);
    bool hasPermission(const std::string& permission) const;

    void showPermissions() const;
    void resetPassword(const std::string& userId);
    void manageProductCatalog() const;

    // Геттеры
    std::string getDepartment() const { return department; }
    std::string getPosition() const { return position; }
    bool getIsSuperAdmin() const { return isSuperAdmin; }
    const std::vector<std::string>& getPermissions() const { return permissions; }
    size_t getPermissionCount() const { return permissions.size(); }

    // Сеттеры
    void setDepartment(const std::string& newDept) { department = newDept; }
    void setPosition(const std::string& newPos) { position = newPos; }
    void setIsSuperAdmin(bool superAdmin) { isSuperAdmin = superAdmin; }

private:
    void initializeAdminSpecificData(const std::string& department,
        const std::string& position,
        bool isSuperAdmin);
};

// Глобальные операторы
std::ostream& operator<<(std::ostream& os, const Admin& admin);
bool operator==(const Admin& a1, const Admin& a2);

#endif
