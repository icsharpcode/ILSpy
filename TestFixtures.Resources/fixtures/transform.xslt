<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" indent="yes" />

  <xsl:template match="/orders">
    <html>
      <body>
        <h1>Orders</h1>
        <table border="1">
          <tr>
            <th>ID</th>
            <th>Customer</th>
          </tr>
          <xsl:for-each select="order">
            <tr>
              <td><xsl:value-of select="@id" /></td>
              <td><xsl:value-of select="customer" /></td>
            </tr>
          </xsl:for-each>
        </table>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
