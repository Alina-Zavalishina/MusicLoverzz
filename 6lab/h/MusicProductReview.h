#ifndef MUSICPRODUCTREVIEW_HPP
#define MUSICPRODUCTREVIEW_HPP

#include "Review.h"
#include <string>
#include <vector>

class MusicProductReview : public Review {
private:
    std::string productName;        // название товара
    std::string productArticle;     // артикул товара
    std::string category;           // категория (гитары, клавишные, ударные и т.д.)
    std::string brand;              // бренд
    bool hasPhotos;                 // есть ли фото
    std::vector<std::string> pros;  // достоинства
    std::vector<std::string> cons;  // недостатки
    int monthsOfUsage;              // месяцев использования
    bool recommendToOthers;         // рекомендую другим

public:
    // Конструкторы
    MusicProductReview();
    MusicProductReview(const std::string& clientId, const std::string& productName,
        const std::string& productArticle, const std::string& category,
        const std::string& brand, int rating, const std::string& comment,
        bool hasPhotos = false, int monthsOfUsage = 0, bool recommend = true);

    // Деструктор
    ~MusicProductReview() override {}

    // РЕАЛИЗАЦИЯ ВИРТУАЛЬНЫХ МЕТОДОВ 

    // Получить тип отзыва
    std::string getReviewType() const override { return "Отзыв на музыкальный товар"; }

    // Отобразить детали отзыва
    void displayDetails() const override;

    // Получить предмет отзыва
    std::string getSubject() const override { return productName; }

    // Проверить валидность отзыва
    bool validate() const override;

    // Клонирование отзыва
    Review* clone() const override;


    // Рассчитать вес отзыва для рейтинга
    double calculateWeight() const override;

    // Подготовить данные для записи в файл
    std::string prepareForFile() const override;

    // Экспортировать в строку
    std::string exportToString() const override;


    // Добавить достоинство
    void addPro(const std::string& pro);

    // Добавить недостаток
    void addCon(const std::string& con);

    // Оценить качество звука
    void rateSoundQuality(int score);  // 1-10

    // Оценить качество сборки
    void rateBuildQuality(int score);  // 1-10

    // Получить общую оценку качества
    double getOverallQuality() const;


    std::string getProductName() const { return productName; }
    std::string getProductArticle() const { return productArticle; }
    std::string getCategory() const { return category; }
    std::string getBrand() const { return brand; }
    bool getHasPhotos() const { return hasPhotos; }
    const std::vector<std::string>& getPros() const { return pros; }
    const std::vector<std::string>& getCons() const { return cons; }
    int getMonthsOfUsage() const { return monthsOfUsage; }
    bool getRecommendToOthers() const { return recommendToOthers; }


    void setProductName(const std::string& name) { productName = name; }
    void setProductArticle(const std::string& article) { productArticle = article; }
    void setCategory(const std::string& cat) { category = cat; }
    void setBrand(const std::string& br) { brand = br; }
    void setHasPhotos(bool photos) { hasPhotos = photos; }
    void setMonthsOfUsage(int months) { monthsOfUsage = months; }
    void setRecommendToOthers(bool recommend) { recommendToOthers = recommend; }

private:

    std::string vectorToString(const std::vector<std::string>& vec) const;
};

#endif
#pragma once
#pragma once
