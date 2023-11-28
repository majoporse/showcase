#include <gtest/gtest.h>
#include "database.h"

TEST(databaseTest, insert_company) {
    database d;
    EXPECT_TRUE(d.add_company("c1", "c1", 1));
    EXPECT_FALSE(d.add_company("c1", "c1", 2));
    EXPECT_FALSE(d.add_company("c2", "c1", 1));
}

TEST(databaseTest, insert_invoice) {
    database d;
    EXPECT_FALSE(d.add_invoice(11,1,2,1000,210));
    EXPECT_TRUE(d.add_company("c1", "c1", 1));
    EXPECT_FALSE(d.add_invoice(1,1,22,1000,210));
    EXPECT_TRUE(d.add_company("c2", "c2", 2));
    EXPECT_TRUE(d.add_invoice(1,1,2,1000,210));
    EXPECT_FALSE(d.add_invoice(1,1,2,1000,210));
}

TEST(databaseTest, insert_vat_payment) {
    database d;
    EXPECT_FALSE(d.add_vat_payment(1,1,123));
    EXPECT_FALSE(d.add_vat_payment(1,2,123));
    EXPECT_TRUE(d.add_company("c1", "c1", 1));
    EXPECT_TRUE(d.add_vat_payment(1,1,123));
    EXPECT_TRUE(d.add_vat_payment(1,1,123));
}

TEST(databaseTest, added_vat) {
    database d;
    EXPECT_TRUE(d.add_company("c1", "c1", 1));
    EXPECT_TRUE(d.add_company("c2", "c2", 2));
    EXPECT_TRUE(d.add_invoice(1,1,2,1000,210));
    EXPECT_TRUE(d.add_invoice(2,2,1,500, 0));
    EXPECT_EQ(d.get_added_vat(1), 210);
    EXPECT_EQ(d.get_added_vat(2), -105);
}

TEST(databaseTest, profit_vat) {
    database d;
    EXPECT_TRUE(d.add_company("c1", "c1", 1));
    EXPECT_TRUE(d.add_company("c2", "c2", 2));
    EXPECT_TRUE(d.add_invoice(1,1,2,1000,210));
    EXPECT_TRUE(d.add_invoice(2,2,1,500, 0));
    EXPECT_EQ(d.get_profit_vat(1), 95);
    EXPECT_EQ(d.get_profit_vat(2), 0);
}

TEST(databaseTest, unpaid_vat) {
    database d;
    EXPECT_TRUE(d.add_company("c1", "c1", 1));
    EXPECT_TRUE(d.add_company("c2", "c2", 2));
    EXPECT_TRUE(d.add_invoice(1,1,2,1000,210));
    EXPECT_TRUE(d.add_invoice(2,2,1,500, 0));
    EXPECT_TRUE(d.add_vat_payment(1,1,210 + 95));
    EXPECT_EQ(d.get_unpaid_vat(1), 0);
}

int main(int argc, char **argv) {
    ::testing::InitGoogleTest(&argc, argv);
    return RUN_ALL_TESTS();
}
