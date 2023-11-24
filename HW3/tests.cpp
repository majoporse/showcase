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
    EXPECT_FALSE(d.add_invoice(1,1,2,1000,210));
    EXPECT_TRUE(d.add_company("c1", "c1", 1));
    EXPECT_TRUE(d.add_invoice(1,1,2,1000,210));
    EXPECT_FALSE(d.add_invoice(1,1,2,1000,210));
}

TEST(databaseTest, insert_invoice) {
    database d;
    EXPECT_FALSE(d.add_vat_payment(1,1,123));
    EXPECT_FALSE(d.add_vat_payment(1,2,123));
    EXPECT_TRUE(d.add_company("c1", "c1", 1));
    EXPECT_TRUE(d.add_vat_payment(1,1,123));
    EXPECT_TRUE(d.add_vat_payment(1,1,123));
}

int main(int argc, char **argv) {
    ::testing::InitGoogleTest(&argc, argv);
    return RUN_ALL_TESTS();
}
