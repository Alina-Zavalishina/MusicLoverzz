#ifndef MUSICPRODUCTREVIEW_H
#define MUSICPRODUCTREVIEW_H

#include "Review.h"
#include <string>
#include <vector>
#include <list>
#include <memory>


class MusicProductReview : public Review<MusicProductReview> {
private:
    std::string productName;
    std::string productArticle;
    std::string category;
    std::string brand;
    bool hasPhotos;
    std::vector<std::string> pros;
    std::vector<std::string> cons;
    int monthsOfUsage;
    bool recommendToOthers;

public:
    MusicProductReview();
    MusicProductReview(const std::string& clientId, const std::string& productName,
        const std::string& productArticle, const std::string& category,
        const std::string& brand, int rating, const std::string& comment,
        bool hasPhotos = false, int monthsOfUsage = 0, bool recommend = true);

    ~MusicProductReview() override {}

    std::string getReviewType() const override { return "Отзыв на музыкальный товар"; }
    std::string getSubject() const override { return productName; }
    MusicProductReview* clone() const override;
    double calculateWeight() const override;
    std::string prepareForFile() const override;
    std::string exportToString() const override;


    void addPro(const std::string& pro);
    void addCon(const std::string& con);
    void addPros(const std::vector<std::string>& newPros);
    void addCons(const std::vector<std::string>& newCons);

    void rateSoundQuality(int score);
    void rateBuildQuality(int score);
    double getOverallQuality() const;


    static std::vector<std::shared_ptr<MusicProductReview>> getAllMusicReviews(
        const std::vector<std::shared_ptr<Review<MusicProductReview>>>& reviews);

    static std::list<std::shared_ptr<MusicProductReview>> getAllMusicReviews(
        const std::list<std::shared_ptr<Review<MusicProductReview>>>& reviewList);

    static std::vector<std::shared_ptr<MusicProductReview>> filterByCategory(
        const std::vector<std::shared_ptr<Review<MusicProductReview>>>& reviews,
        const std::string& category);

    static std::vector<std::shared_ptr<MusicProductReview>> filterByBrand(
        const std::vector<std::shared_ptr<Review<MusicProductReview>>>& reviews,
        const std::string& brand);

    static std::vector<std::string> getAllCategories(
        const std::vector<std::shared_ptr<Review<MusicProductReview>>>& reviews);

    static std::vector<std::string> getAllBrands(
        const std::vector<std::shared_ptr<Review<MusicProductReview>>>& reviews);

    static double getAverageRatingByCategory(
        const std::vector<std::shared_ptr<Review<MusicProductReview>>>& reviews,
        const std::string& category);

    static double getAverageRatingByBrand(
        const std::vector<std::shared_ptr<Review<MusicProductReview>>>& reviews,
        const std::string& brand);

    // Геттеры
    std::string getProductName() const { return productName; }
    std::string getProductArticle() const { return productArticle; }
    std::string getCategory() const { return category; }
    std::string getBrand() const { return brand; }
    bool getHasPhotos() const { return hasPhotos; }
    const std::vector<std::string>& getPros() const { return pros; }
    const std::vector<std::string>& getCons() const { return cons; }
    int getMonthsOfUsage() const { return monthsOfUsage; }
    bool getRecommendToOthers() const { return recommendToOthers; }

    // Сеттеры
    void setProductName(const std::string& name) { productName = name; }
    void setProductArticle(const std::string& article) { productArticle = article; }
    void setCategory(const std::string& cat) { category = cat; }
    void setBrand(const std::string& br) { brand = br; }
    void setHasPhotos(bool photos) { hasPhotos = photos; }
    void setMonthsOfUsage(int months) { monthsOfUsage = months; }
    void setRecommendToOthers(bool recommend) { recommendToOthers = recommend; }


    static std::vector<std::shared_ptr<Review<MusicProductReview>>> convertToReviewVector(
        const std::vector<std::shared_ptr<MusicProductReview>>& musicReviews);

    static std::list<std::shared_ptr<Review<MusicProductReview>>> convertToReviewList(
        const std::vector<std::shared_ptr<MusicProductReview>>& musicReviews);

private:
    std::string vectorToString(const std::vector<std::string>& vec) const;
    static std::shared_ptr<MusicProductReview> tryCast(
        const std::shared_ptr<Review<MusicProductReview>>& review);
};

#endif
