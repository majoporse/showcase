#include "rational.h" // Include the header file with the rational struct
#include <gtest/gtest.h>
#include "mainwindow.h"
#include "parser.h"

TEST(RationalTest, Value) {
    rational r(1, 2);
    EXPECT_FLOAT_EQ(r.value(), 0.5);
}

TEST(RationalTest, Addition) {
    rational r1(1, 2);
    rational r2(1, 3);
    rational result = r1 + r2;
    EXPECT_EQ(result.nom, 5);
    EXPECT_EQ(result.denom, 6);
}

TEST(RationalTest, Subtraction) {
    rational r1(3, 4);
    rational r2(1, 4);
    rational result = r1 - r2;
    EXPECT_EQ(result.nom, 1);
    EXPECT_EQ(result.denom, 2);
}

TEST(RationalTest, Multiplication) {
    rational r1(2, 3);
    rational r2(3, 4);
    rational result = r1 * r2;
    EXPECT_EQ(result.nom, 1);
    EXPECT_EQ(result.denom, 2);
}

TEST(RationalTest, Division) {
    rational r1(1, 2);
    rational r2(3, 4);
    rational result = r1 / r2;
    EXPECT_EQ(result.nom, 2);
    EXPECT_EQ(result.denom, 3);
}

TEST(RationalTest, Equality) {
    rational r1(1, 2);
    rational r2(2, 4);
    EXPECT_EQ(r1, r2);
}

TEST(RationalTest, Inequality) {
    rational r1(2, 3);
    rational r2(3, 4);
    EXPECT_NE(r1, r2);
}

TEST(RationalTest, Comparison) {
    rational r1(1, 2);
    rational r2(3, 4);
    EXPECT_LT(r1, r2);
    EXPECT_LE(r1, r2);
    EXPECT_GT(r2, r1);
    EXPECT_GE(r2, r1);
}


TEST(ParserTest, ParsePrio1) {
    std::vector<rational> memory;
    std::stringstream ss("2+3-4+12/4-13/4+1/4");
    rational result = parsePrio1(ss, memory, false);
    EXPECT_EQ(result.nom, 1);
    EXPECT_EQ(result.denom, 1);
}

TEST(ParserTest, ParsePrio2) {
    std::vector<rational> memory;
    std::stringstream ss("2*3/4");
    rational result = parsePrio2(ss, memory);
    EXPECT_EQ(result.nom, 3);
    EXPECT_EQ(result.denom, 2);
}

TEST(ParserTest, ParsePrio3) {
    std::vector<rational> memory;
    std::stringstream ss("2^3");
    rational result = parsePrio3(ss, memory);
    EXPECT_EQ(result.nom, 8);
    EXPECT_EQ(result.denom, 1);
}

int main(int argc, char **argv) {
    ::testing::InitGoogleTest(&argc, argv);
    return RUN_ALL_TESTS();
}
