cmake_minimum_required(VERSION 3.14)

project(httpServer LANGUAGES CXX)

set(CMAKE_AUTOUIC ON)
set(CMAKE_AUTOMOC ON)
set(CMAKE_AUTORCC ON)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

find_package(QT NAMES Qt6 Qt5 REQUIRED COMPONENTS Core Network HttpServer Concurrent Widgets Xml WebEngineWidgets)
find_package(Qt${QT_VERSION_MAJOR} REQUIRED COMPONENTS Core Network HttpServer Concurrent Widgets Xml WebEngineWidgets)

add_executable(httpServer
  main.cpp
  myserver.h myserver.cpp
  database.cpp database.h
  databaseupdater.cpp databaseupdater.h
  config.cpp config.h
)

add_executable(client
    mainclient.cpp
    client.cpp client.h
)

target_link_libraries(httpServer Qt${QT_VERSION_MAJOR}::Core Qt${QT_VERSION_MAJOR}::Network Qt${QT_VERSION_MAJOR}::HttpServer Qt6::Concurrent Qt6::Xml)
target_link_libraries(client Qt${QT_VERSION_MAJOR}::Core Qt${QT_VERSION_MAJOR}::Network Qt${QT_VERSION_MAJOR}::HttpServer Qt6::Widgets Qt6::Xml Qt6::WebEngineWidgets)

include(GNUInstallDirs)
install(TARGETS httpServer
    LIBRARY DESTINATION ${CMAKE_INSTALL_LIBDIR}
    RUNTIME DESTINATION ${CMAKE_INSTALL_BINDIR}
)
