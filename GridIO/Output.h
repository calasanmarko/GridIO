#pragma once

#include <wtypes.h>
#include <WinUser.h>
#include <string>

using namespace std;

static void MsgBox(string title, string message) {
	wstring tempTitle(title.begin(), title.end());
	wstring tempMsg(message.begin(), message.end());

	MessageBox(NULL, tempMsg.c_str(), tempTitle.c_str(), 0);
}