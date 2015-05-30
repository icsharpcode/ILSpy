//-----------------------------------------------------------------------------
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the Microsoft Public License.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//-----------------------------------------------------------------------------
using System;

namespace Microsoft.Cci.Pdb {
  internal class PdbTokenLine {
    internal uint token;
    internal uint file_id;
    internal uint line;
    internal uint column;
    internal uint endLine;
    internal uint endColumn;
    internal PdbSource sourceFile;
    internal PdbTokenLine/*?*/ nextLine;

    internal PdbTokenLine(uint token, uint file_id, uint line, uint column, uint endLine, uint endColumn) {
      this.token = token;
      this.file_id = file_id;
      this.line = line;
      this.column = column;
      this.endLine = endLine;
      this.endColumn = endColumn;
    }
  }
}
