/**
 * @file widget_library.h
 * @brief A C++ library for creating a simple widget hierarchy.
 */

#ifndef WIDGET_LIBRARY_H
#define WIDGET_LIBRARY_H
#include <ranges>
#include <tuple>
#include <map>
#include <vector>
#include <string>
#include <sstream>
#include <utility>
#include <iostream>

/**
 * @class WObject
 * @brief Base class for widgets in the widget hierarchy.
 */
class WObject
{
    friend class WWidget;
    friend class WGridLayout;
    friend class WTabLayout;

public:
    explicit WObject(std::string name, WObject *parent) : name{name}, parent{parent} {}

    /**
     * @brief Find a child widget by name.
     * @param s_name The name of the child widget to find.
     * @return Pointer to the found child widget or nullptr if not found.
     */
    virtual WObject *find_child(const std::string &s_name) = 0;

    /**
     * @brief Get the name of the widget.
     * @return The name of the widget.
     */
    std::string get_name() const { return name; }

    /**
     * @brief Print the widget's information.
     * @param o The output stream.
     * @param prefix The prefix for formatting.
     * @return The modified output stream.
     */
    virtual std::ostream &print(std::ostream &o, std::string prefix) const = 0;

    /**
     * @brief Overloaded stream insertion operator to print the widget.
     * @param o The output stream.
     * @param w The widget to print.
     * @return The modified output stream.
     */
    friend std::ostream &operator<<(std::ostream &o, const WObject &w) { return w.print(o, ""); }

    /**
     * @brief Virtual destructor for base class.
     */
    virtual ~WObject() = default;

protected:
    WObject *parent;  /**< Pointer to the parent widget. */
    std::string name; /**< Name of the widget. */
    /**
     * @brief changes parent
     * @param new_parent The name of the next parent.
     * @return returns nothing
     */
    void change_parent(WObject *new_parent);
};

/**
 * @class WLabel
 * @brief Represents a label widget.
 */
class WLabel : public WObject
{
    std::string message; /**< The message displayed by the label. */
public:
    /**
     * @brief Constructor for WLabel.
     * @param message The initial message to display.
     * @param name The name of the label.
     * @param parent The parent widget (default is nullptr).
     */
    explicit WLabel(const std::string &message, const std::string &name, WObject *parent = nullptr) : WObject(name, parent), message{message} {}

    /**
     * @brief Set a new label message.
     * @param new_message The new message to display.
     */
    void set_label(std::string new_message);

    /**
     * @brief Find a child widget by name.
     * @param s_name The name of the child widget to find.
     * @return Pointer to the found child widget or nullptr if not found.
     */
    WObject *find_child(const std::string &s_name) override;

    /**
     * @brief Print the label's information.
     * @param o The output stream.
     * @param prefix The prefix for formatting.
     * @return The modified output stream.
     */
    std::ostream &print(std::ostream &o, std::string prefix) const override;
};

/**
 * @class WButton
 * @brief Represents a button widget.
 */
class WButton : public WObject
{
    std::string message; /**< The message displayed on the button. */

public:
    /**
     * @brief Constructor for WButton.
     * @param message The initial message to display on the button.
     * @param name The name of the button.
     * @param parent The parent widget (default is nullptr).
     */
    explicit WButton(const std::string &message, const std::string &name, WObject *parent = nullptr) : WObject(name, parent), message{message} {}

    /**
     * @brief Find a child widget by name.
     * @param s_name The name of the child widget to find.
     * @return Pointer to the found child widget or nullptr if not found.
     */
    WObject *find_child(const std::string &s_name) override;

    /**
     * @brief Print the button's information.
     * @param o The output stream.
     * @param prefix The prefix for formatting.
     * @return The modified output stream.
     */
    std::ostream &print(std::ostream &o, std::string prefix) const override;
};

/**
 * @class WGridLayout
 * @brief Represents a grid layout widget.
 */
class WGridLayout : public WObject
{
    int rows;                                              /**< Number of rows in the grid layout. */
    int cols;                                              /**< Number of columns in the grid layout. */
    std::map<std::tuple<int, int>, WObject *> elems; /**< Map of grid elements. */
public:
    /**
     * @brief Constructor for WGridLayout.
     * @param rows The number of rows in the grid layout.
     * @param cols The number of columns in the grid layout.
     * @param name The name of the grid layout.
     * @param parent The parent widget (default is nullptr).
     */
    WGridLayout(int rows, int cols, std::string name, WObject *parent = nullptr) : WObject(name, parent), rows{rows}, cols{cols} {}

    /**
     * @brief Find a child widget by name.
     * @param s_name The name of the child widget to find.
     * @return Pointer to the found child widget or nullptr if not found.
     */
    WObject *find_child(const std::string &s_name) override;

    /**
     * @brief Add a new widget to the grid layout.
     * @param x The X coordinate of the grid cell.
     * @param y The Y coordinate of the grid cell.
     * @param new_object The widget to add to the grid cell.
     */
    void add_elem(int x, int y, WObject *new_object);

    /**
     * @brief Print the grid layout's information.
     * @param o The output stream.
     * @param prefix The prefix for formatting.
     * @return The modified output stream.
     */
    std::ostream &print(std::ostream &o, std::string prefix) const override;

    /**
     * @brief Destructor for WGridLayout.
     */
    ~WGridLayout();
};

/**
 * @class WTabLayout
 * @brief Represents a tab layout widget.
 */
class WTabLayout : public WObject
{
    std::vector<WObject *> tabs; /**< Vector of tab widgets. */

public:
    /**
     * @brief Print the grid layout's information.
     * @param o The output stream.
     * @param prefix The prefix for formatting.
     * @return The modified output stream.
     */
    std::ostream &print(std::ostream &o, std::string prefix) const;
    /**
     * @brief Constructor for WTabLayout.
     * @param name The name of the tab layout.
     * @param parent The parent widget (default is nullptr).
     */
    explicit WTabLayout(std::string name, WObject *parent = nullptr) : WObject(name, parent) {}

    /**
     * @brief Find a child widget by name.
     * @param s_name The name of the child widget to find.
     * @return Pointer to the found child widget or nullptr if not found.
     */
    WObject *find_child(const std::string &s_name) override;

    /**
     * @brief Add a new tab to the tab layout.
     * @param new_object The tab widget to add.
     */
    void add_tab(WObject *new_object);

    ~WTabLayout();
};

/**
 * @struct WWidget
 * @brief Represents a widget that can contain another child widget.
 * @details WWidget is a class that inherits from the WObject class, making it a part of a hierarchical widget structure. It can contain a single child widget, which can be any type of widget.
 */

class WWidget : public WObject
{
public:
    /**
     * @brief Constructor for WWidget.
     * @param name The name of the widget.
     * @param child A pointer to the child widget (default is nullptr).
     * @param parent A pointer to the parent widget (default is nullptr).
     */
    explicit WWidget(std::string name, WObject *child = nullptr, WObject *parent1 = nullptr) : WObject(name, parent1), child{child}
    {
        parent = parent1;
        if (child)
        {
            child->change_parent(this);
        }
    }
    /**
     * @brief Set or replace the child widget.
     * @param new_child A pointer to the new child widget.
     * @details This method allows you to set or replace the child widget contained within the WWidget. It takes care of managing the parent-child relationship.
     */
    void set_child(WObject *new_child);

    /**
     * @brief Find a child widget by name.
     * @param s_name The name of the child widget to find.
     * @return Pointer to the found child widget or nullptr if not found.
     * @details This method searches for a child widget with a given name within the WWidget's hierarchy, including its child and descendants.
     */
    WObject *find_child(const std::string &s_name) override;

    /**
     * @brief Print information about the widget and its child.
     * @param o The output stream for printing.
     * @param prefix The prefix for formatting.
     * @return The modified output stream.
     * @details This method prints information about the WWidget and its child widget, including the parent-child relationship.
     */
    std::ostream &print(std::ostream &o, std::string prefix) const override;

    /**
     * @brief Destructor for WWidget.
     * @details This destructor cleans up resources, including deleting the child widget, if one exists.
     */
    ~WWidget();

private:
    WObject *child;  /**< Pointer to the child widget. */
    WObject *parent; /**< Pointer to the parent widget. */
};
#endif
