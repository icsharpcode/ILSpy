#pragma once

#ifdef DLLMAIN_EXPORTS
#define DLLMAIN_API __declspec(dllexport)
#else
#define DLLMAIN_API __declspec(dllimport)
#endif

extern "C" DLLMAIN_API int RegisterDLL();
