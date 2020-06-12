cmake_minimum_required(VERSION 3.10)
PROJECT(lua)

add_subdirectory(${LUA_LIB_PATH})

include_directories(${PROJECT_SOURCE_DIR}/${LUA_LIB_PATH})

set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} ${CFLAGS}")    
set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} ${CFLAGS}")

if(${PLATFORM} STREQUAL "MacOSX")
	add_library(lua MODULE lua_extension.c)
	set_target_properties(lua PROPERTIES BUNDLE TRUE)
else()
	add_library(lua lua_extension.c)
endif()

target_link_libraries(lua lualib)