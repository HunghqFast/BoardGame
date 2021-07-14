<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:fo="http://www.w3.org/1999/XSL/Format">
	<xsl:output  method="html" encoding="UTF-8"/>

	<xsl:template match="/DSVRefreshDocument">
		<TABLE BORDER="1" WIDTH="100%" CELLSPACING="0">
			<TR>
			<TD>
				<xsl:element name="FONT">
				<xsl:attribute name="FACE"><xsl:value-of select="//FontName"/></xsl:attribute>
				<xsl:attribute name="STYLE"><xsl:value-of select="//FontStyle"/></xsl:attribute>
				<B><xsl:value-of select="ColumnTitle1"/></B>
				</xsl:element>  
			</TD>
			<TD>
				<xsl:element name="FONT">
				<xsl:attribute name="FACE"><xsl:value-of select="//FontName"/></xsl:attribute>
				<xsl:attribute name="STYLE"><xsl:value-of select="//FontStyle"/></xsl:attribute>
				<B><xsl:value-of select="ColumnTitle2"/></B>
				</xsl:element>  
			</TD>
			</TR>
			<xsl:apply-templates select="./Tables"/>
		</TABLE>
	</xsl:template>

	<xsl:template match="/DSVRefreshDocument/Tables">		
		<xsl:for-each select="./Table">
			<xsl:sort select="Name"/>
			<TR>
			<TD>
				<xsl:attribute name="ALIGN">Left</xsl:attribute>
				<xsl:attribute name="WIDTH">70%</xsl:attribute>												
				<xsl:if test="Type[.='Table']">				
					<IMG SRC=".\images\table.gif" ALIGN="absMiddle"/>
				</xsl:if>
				<xsl:if test="Type[.='View']">
					<IMG SRC=".\images\view.gif" ALIGN="absMiddle"/>
				</xsl:if>

				<xsl:element name="FONT">
				<xsl:attribute name="FACE"><xsl:value-of select="//FontName"/></xsl:attribute>
				<xsl:attribute name="STYLE"><xsl:value-of select="//FontStyle"/></xsl:attribute>
				<xsl:value-of select="Name"/>
				</xsl:element>  
			</TD>
			
			<TD>
				<xsl:if test="Action[.!='']">
				<xsl:attribute name="ALIGN">Center</xsl:attribute>
				
				<xsl:element name="FONT">
				<xsl:attribute name="FACE"><xsl:value-of select="//FontName"/></xsl:attribute>
				<xsl:attribute name="STYLE"><xsl:value-of select="//FontStyle"/></xsl:attribute>
				<B> <xsl:value-of select="Action"/> </B>
				</xsl:element>  

				</xsl:if>
				
				<xsl:if test="Action[.='']">
				<xsl:attribute name="ALIGN">Center</xsl:attribute>				
				<xsl:element name="FONT">
				<xsl:attribute name="FACE"><xsl:value-of select="//FontName"/></xsl:attribute>
				<xsl:attribute name="STYLE"><xsl:value-of select="//FontStyle"/></xsl:attribute>
					<BR/>				
				</xsl:element>  
				</xsl:if>
			</TD>
			
			<xsl:apply-templates select="./Columns"/>
			<xsl:apply-templates select="./Relations"/>
			<xsl:apply-templates select="./UniqueConstraints"/>
			</TR>
		</xsl:for-each>		
	</xsl:template>

	<xsl:template match="Table/Columns">		
		<xsl:for-each select="./Column">
			<xsl:sort select="Name"/>
			<TR>
			<TD>
				<xsl:attribute name="ALIGN">Left</xsl:attribute>
				<xsl:attribute name="WIDTH">70%</xsl:attribute>								
				<IMG SRC=".\images\empty.gif" WIDTH="20" HEIGHT="1"/>
				<IMG SRC=".\images\column.gif" ALIGN="absMiddle"/>
				<xsl:element name="FONT">
				<xsl:attribute name="FACE"><xsl:value-of select="//FontName"/></xsl:attribute>
				<xsl:attribute name="STYLE"><xsl:value-of select="//FontStyle"/></xsl:attribute>
					<B> <xsl:value-of select="Name"/> </B>					
				</xsl:element>  
			</TD>
			<TD>
				<xsl:attribute name="ALIGN">Center</xsl:attribute>
				<xsl:element name="FONT">
				<xsl:attribute name="FACE"><xsl:value-of select="//FontName"/></xsl:attribute>
				<xsl:attribute name="STYLE"><xsl:value-of select="//FontStyle"/></xsl:attribute>
					<B> <xsl:value-of select="Action"/> </B>					
				</xsl:element>  
			</TD>			
			</TR>
		</xsl:for-each>		
	</xsl:template>

	<xsl:template match="Table/Relations">		
		<xsl:for-each select="./Relation">
			<xsl:sort select="Name"/>
			<TR>
			<TD>
				<xsl:attribute name="ALIGN">Left</xsl:attribute>
				<xsl:attribute name="WIDTH">70%</xsl:attribute>								
				<IMG SRC=".\images\empty.gif" WIDTH="20" HEIGHT="1"/>
				<IMG SRC=".\images\relationship.gif" ALIGN="absMiddle"/>
				<xsl:element name="FONT">
				<xsl:attribute name="FACE"><xsl:value-of select="//FontName"/></xsl:attribute>
				<xsl:attribute name="STYLE"><xsl:value-of select="//FontStyle"/></xsl:attribute>
					<B> <xsl:value-of select="Name"/> </B>					
				</xsl:element>  
			</TD>
			<TD>
				<xsl:attribute name="ALIGN">Center</xsl:attribute>											
				<xsl:element name="FONT">
				<xsl:attribute name="FACE"><xsl:value-of select="//FontName"/></xsl:attribute>
				<xsl:attribute name="STYLE"><xsl:value-of select="//FontStyle"/></xsl:attribute>
					<B> <xsl:value-of select="Action"/> </B>					
				</xsl:element>  
			</TD>			
			</TR>
		</xsl:for-each>		
	</xsl:template>

	<xsl:template match="Table/UniqueConstraints">		
		<xsl:for-each select="./Constraint">
			<xsl:sort select="Name"/>
			<TR>
			<TD>
				<xsl:attribute name="ALIGN">Left</xsl:attribute>
				<xsl:attribute name="WIDTH">70%</xsl:attribute>								
				<IMG SRC=".\images\empty.gif" WIDTH="20" HEIGHT="1"/>
				<IMG SRC=".\images\uniqueconstraint.gif" ALIGN="absMiddle" />
				<xsl:element name="FONT">
				<xsl:attribute name="FACE"><xsl:value-of select="//FontName"/></xsl:attribute>
				<xsl:attribute name="STYLE"><xsl:value-of select="//FontStyle"/></xsl:attribute>
					<B> <xsl:value-of select="Name"/> </B>					
				</xsl:element>  
			</TD>
			<TD>
				<xsl:attribute name="ALIGN">Center</xsl:attribute>											
				<xsl:element name="FONT">
				<xsl:attribute name="FACE"><xsl:value-of select="//FontName"/></xsl:attribute>
				<xsl:attribute name="STYLE"><xsl:value-of select="//FontStyle"/></xsl:attribute>
					<B> <xsl:value-of select="Action"/> </B>					
				</xsl:element>  
			</TD>			
			</TR>
		</xsl:for-each>		
	</xsl:template>	
	
</xsl:stylesheet>
