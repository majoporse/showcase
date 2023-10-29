cmake_minimum_required(VERSION 3.5)

project(HW2 VERSION 0.1 LANGUAGES CXX)

set(CMAKE_INCLUDE_CURRENT_DIR ON)

set(CMAKE_AUTOUIC ON)
set(CMAKE_AUTOMOC ON)
set(CMAKE_AUTORCC ON)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

set(PROJECT_SOURCES
        main.cpp
        mainwindow.cpp
        mainwindow.h
        mainwindow.ui
        rational.cpp
        rational.h
        parser.h
        parser.cpp
)

set(TEST_SOURCES
    tests.cpp
    mainwindow.cpp
    mainwindow.h
    mainwindow.ui
    rational.cpp
    rational.h
    parser.h
    parser.cpp
)

find_package(QT NAMES Qt6 Qt5 COMPONENTS Widgets REQUIRED)
find_package(Qt${QT_VERSION_MAJOR} COMPONENTS Widgets REQUIRED)

include(FetchContent)
FetchContent_Declare(
    googletest
    # Specify the commit you depend on and update it regularly.
    URL https://github.com/google/googletest/archive/5376968f6948923e2411081fd9372e71a59d8e77.zip
    )
# For Windows: Prevent overriding the parent project's compiler/linker settings
set(gtest_force_shared_crt ON CACHE BOOL "" FORCE)
FetchContent_MakeAvailable(googletest)

# Now simply link against gtest or gtest_main as needed. Eg



if(${QT_VERSION_MAJOR} GREATER_EQUAL 6)
    qt_add_executable(HW2
        MANUAL_FINALIZATION
        ${PROJECT_SOURCES}
    )
    qt_add_executable(tests
        MANUAL_FINALIZATION
        ${TEST_SOURCES}
    )
# Define target properties for Android with Qt 6 as:
#    set_property(TARGET HW2 APPEND PROPERTY QT_ANDROID_PACKAGE_SOURCE_DIR
#                 ${CMAKE_CURRENT_SOURCE_DIR}/android)
# For more information, see https://doc.qt.io/qt-6/qt-add-executable.html#target-creation
else()
    if(ANDROID)
        add_library(HW2 SHARED
            ${PROJECT_SOURCES}
        )
        add_library(tests SHARED
            ${TEST_SOURCES}
        )
# Define properties for Android with Qt 5 after find_package() calls as:
#    set(ANDROID_PACKAGE_SOURCE_DIR "${CMAKE_CURRENT_SOURCE_DIR}/android")
    else()
        add_executable(HW2
            ${PROJECT_SOURCES}
        )
        add_executable(tests
            ${TEST_SOURCES}
        )
    endif()
endif()

target_link_libraries(HW2 PRIVATE Qt${QT_VERSION_MAJOR}::Widgets)

set_target_properties(HW2 PROPERTIES
    MACOSX_BUNDLE_GUI_IDENTIFIER my.example.com
    MACOSX_BUNDLE_BUNDLE_VERSION ${PROJECT_VERSION}
    MACOSX_BUNDLE_SHORT_VERSION_STRING ${PROJECT_VERSION_MAJOR}.${PROJECT_VERSION_MINOR}
    MACOSX_BUNDLE TRUE
    WIN32_EXECUTABLE TRUE
)

target_link_libraries(tests PRIVATE Qt${QT_VERSION_MAJOR}::Widgets gtest_main)
#target_link_libraries(tests gtest_main)
target_link_libraries(HW2 PRIVATE Qt${QT_VERSION_MAJOR}::Widgets)

add_test(NAME tests COMMAND tests)

if(QT_VERSION_MAJOR EQUAL 6)
    qt_finalize_executable(tests)
    qt_finalize_executable(HW2)
endif()
