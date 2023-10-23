#include "hw1.hpp"
#include <iostream>
#include <cassert>
#include <gtest/gtest.h>

TEST(WLabelTest, PrintFunctionTest) {
    WLabel label("Hello, World!", "label1");
    std::ostringstream output;
    label.print(output, "");

    // Replace with expected output
    std::string expected_output = "[Label:label1] Displaying: \"Hello, World!\", parent: None\n";

    ASSERT_EQ(output.str(), expected_output);
}

TEST(WButtonTest, PrintFunctionTest) {
    WButton button("Click Me!", "button1");
    std::ostringstream output;
    button.print(output, "");

    // Replace with expected output
    std::string expected_output = "[Button:button1] |Click Me!| parent: None\n";

    ASSERT_EQ(output.str(), expected_output);
}

TEST(WGridLayoutTest, PrintFunctionTest) {
    WGridLayout grid(2, 2, "grid1");
    std::ostringstream output;
    output << grid;

    // Replace with expected output
    std::string expected_output = "[GridLayout:grid1] parent: None\n";

    ASSERT_EQ(output.str(), expected_output);
}

TEST(WTabLayoutTest, PrintFunctionTest) {
    WTabLayout tabLayout("tab1");
    std::ostringstream output;
    tabLayout.print(output, "");

    // Replace with expected output
    std::string expected_output = "[Tabs:tab1] parent: None tabs: 0\n";

    ASSERT_EQ(output.str(), expected_output);
}

TEST(WWidgetTest, PrintFunctionTest) {
    WLabel *label = new WLabel("Hello, World!", "label1");
    WWidget widget("widget1", label);
    std::ostringstream output;
    widget.print(output, "");

    // Replace with expected output
    std::string expected_output = "[Widget:widget1] parent: None|label1\n   [Label:label1] Displaying: \"Hello, World!\", parent: widget1\n";

    ASSERT_EQ(output.str(), expected_output);
}
TEST(WLabelTest, NameAndSetLabelTest) {
    WLabel label("Hello, World!", "label1");

    // Test the label's name
    ASSERT_EQ(label.get_name(), "label1");

    // Test set_label method
    label.set_label("New Message");
    ASSERT_EQ(label.get_name(), "label1");
}

// Test WButton class
TEST(WButtonTest, NameTest) {
    WButton button("Click Me!", "button1");

    // Test the button's name
    ASSERT_EQ(button.get_name(), "button1");
}

// Test WGridLayout class
TEST(WGridLayoutTest, NameAndFindChildTest) {
    WGridLayout grid(2, 2, "grid1");

    // Test the grid's name
    ASSERT_EQ(grid.get_name(), "grid1");

    // Add some elements to the grid
    WButton *button1 = new WButton("Button 1", "button1");
    WButton *button2 = new WButton("Button 2", "button2");
    grid.add_elem(0, 0, button1);
    grid.add_elem(1, 1, button2);

    // Test find_child method
    ASSERT_EQ(grid.find_child("button1"), button1);
    ASSERT_EQ(grid.find_child("button2"), button2);
    ASSERT_EQ(grid.find_child("button3"), nullptr);
}

// Test WTabLayout class
TEST(WTabLayoutTest, NameAndFindChildTest) {
    WTabLayout tabLayout("tabLayout");

    // Test the tabLayout's name
    ASSERT_EQ(tabLayout.get_name(), "tabLayout");

    // Add some tabs to the tabLayout
    WButton *tab1 = new WButton("Tab 1", "tab1");
    WButton *tab2 = new WButton("Tab 2", "tab2");
    tabLayout.add_tab(tab1);
    tabLayout.add_tab(tab2);

    // Test find_child method
    ASSERT_EQ(tabLayout.find_child("tab1"), tab1);
    ASSERT_EQ(tabLayout.find_child("tab2"), tab2);
    ASSERT_EQ(tabLayout.find_child("tab3"), nullptr);
}

// Test WWidget class
TEST(WWidgetTest, SetChildAndFindChildTest) {
    WWidget widget("parentWidget");
    WButton *childButton = new WButton("Child Button", "childButton");

    // Set the child widget
    widget.set_child(childButton);

    // Test find_child method
    ASSERT_EQ(widget.find_child("childButton"), childButton);
    ASSERT_EQ(widget.find_child("nonExistentChild"), nullptr);
}

int main()
{
    ::testing::InitGoogleTest();
    return RUN_ALL_TESTS();
}
