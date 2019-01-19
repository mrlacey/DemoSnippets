// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

// DEMOSNIPPETS-TAB C++
// DEMOSNIPPETS-LABEL includes
#include "pch.h"
#include "common.h"
#include "ColorChangedEventArgs.h"

// DEMOSNIPPETS-LABEL OldColor
winrt::Color ColorChangedEventArgs::OldColor()
{
	return m_oldColor;
}
//DEMOSNIPPETS-ENDSNIPPET
winrt::Color ColorChangedEventArgs::NewColor()
{
	return m_newColor;
}

void ColorChangedEventArgs::OldColor(winrt::Color const& value)
{
	m_oldColor = value;
}
/* DEMOSNIPPETS-LABEL NewColor */
void ColorChangedEventArgs::NewColor(winrt::Color const& value)
{
	m_newColor = value;
}