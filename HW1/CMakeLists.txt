cmake_minimum_required(VERSION 3.10)
project(WidgetLibrary)

set(CMAKE_CXX_STANDARD 20) # Use the C++20 standard

add_library(widlib SHARED hw1.cpp)


add_executable(testy tests.cpp)
target_link_libraries(testy widlib)