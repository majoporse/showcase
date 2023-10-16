cmake_minimum_required(VERSION 3.14)
project(WidgetLibrary)

set(CMAKE_CXX_STANDARD 20) # Use the C++20 standard

add_library(widlib SHARED hw1.cpp)

add_executable(sample sample.cpp)

target_link_libraries(sample widlib)

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
add_executable(tests tests.cpp)
target_link_libraries(tests widlib)
target_link_libraries(tests gtest_main)
add_test(NAME tests COMMAND tests)