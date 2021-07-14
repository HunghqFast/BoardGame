<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:KPI="KPI" version="1.0">

	<msxsl:script language="JavaScript" implements-prefix="KPI">
		var Map;
		
		// Initialize the variables
		function Initialize() {
			var name;  Map = new Object;

			InitMapEntry( "Standard Arrow",				"Arrow_XP",		5 );
			InitMapEntry( "XP Arrow",					"Arrow_XP",				5 );
			InitMapEntry( "Status Arrow - Ascending",	"Arrow_Status_Asc",		5 );
			InitMapEntry( "Status Arrow - Descending",	"Arrow_Status_Desc",	5 );
			InitMapEntry( "Shapes",						"Stoplight_Single",		3 );
			InitMapEntry( "Traffic Light",				"Stoplight_Multiple",	3 );
			InitMapEntry( "Road Signs",					"Road",					3 );
			InitMapEntry( "Gauge - Ascending",			"Gauge_Asc",			5 );
			InitMapEntry( "Gauge - Descending",			"Gauge_Desc",			5 );
			InitMapEntry( "Thermometer",				"Therm",				3 );
			InitMapEntry( "Cylinder",					"Cylinder",				3 );
			InitMapEntry( "Smiley Face",				"Smiley",				3 );
			InitMapEntry( "Variance Arrow",				"Arrow_Beveled",		3 );

			
			return "";  //to be able to run from IE
		}

		// Map the graphics Type to the File Name
		function InitMapEntry( gfxType, fileName, range ){
			Map[gfxType] = new Object;
			Map[gfxType].FileName = fileName;
			Map[gfxType].Range = range;
		}

		// Match the graphics Type to the appropriate icon (general)
		function GetMatchedFileName( gfxType, statusValue){
		
			try
			{
				var range = Map[gfxType].Range;			
				var index = 0;
				
				if (range == 3)
				{
					if ((statusValue &gt;= -1) &amp;&amp; (statusValue &lt;= -0.5))				
						index = 0;				
					else if ((statusValue &gt; -0.5) &amp;&amp; (statusValue &lt; 0.5))
						index = 1;
					else
						index = 2; 					
				}
				if (range == 5)
				{
					if ((statusValue &gt;= -1) &amp;&amp; (statusValue &lt;= -0.75))				
						index = 0;				
					else if ((statusValue &gt; -0.75) &amp;&amp; (statusValue &lt;= -0.25))
						index = 1;
					else if ((statusValue &gt; -0.25) &amp;&amp; (statusValue &lt; 0.25))
						index = 2;
					else if ((statusValue &gt;= 0.25) &amp;&amp; (statusValue &lt; 0.75))
				index = 3;
				else
				index = 4;
				}
			}
			catch (Exception)
			{
				return ".\\images\\error.gif";
			}
			return ".\\images\\" + Map[gfxType].FileName + index + ".gif";
		}

		// Match the graphics Type to the appropriate icon (for Trend/Status sets)
		function GetGfxFileName( gfxType, gfxValue ){
			if(gfxValue &lt; -1) gfxValue = -1;
			if(gfxValue &gt;  1) gfxValue =  1;			
			var status = gfxValue; 			
			return GetMatchedFileName( gfxType, status);
		}

	</msxsl:script>

	<xsl:template match="/KpiResultDocument">
		<xsl:value-of select="KPI:Initialize()"/>
		<TABLE BORDER="1" CELLPADDING="1" CELLSPACING="0" WIDTH="100%">
			<xsl:apply-templates select="./Columns"/>
			<xsl:call-template name="ProcessKpiLines"/>
		</TABLE>
	</xsl:template>

	<xsl:template match="/KpiResultDocument/Columns">
		<TR>
		<xsl:for-each select="./Column">
			<xsl:if test="ID[.!='StatusGraphicID'] and ID[.!='TrendGraphicID']">
				<TD HEIGHT="24">
					<xsl:if test="ID[.='StructureID']">
						<xsl:attribute name="WIDTH">40%</xsl:attribute>
						<xsl:attribute name="ALIGN">Left</xsl:attribute>
					</xsl:if>
					
					<xsl:if test="ID[.!='StructureID']">
						<xsl:if test="ID[.!='DescriptionID']">
							<xsl:attribute name="WIDTH">12%</xsl:attribute>
							<xsl:attribute name="ALIGN">Center</xsl:attribute>
						</xsl:if>
					</xsl:if>
					
					<xsl:attribute name="BGCOLOR">#D4D0C8</xsl:attribute>
					<xsl:if test="ID[.='DescriptionID']">
                        <!-- Make the column have border-->
						<xsl:element name="FONT">
                            <xsl:attribute name="FACE"><xsl:value-of select="//FontName"/></xsl:attribute>
                            <xsl:attribute name="STYLE"><xsl:value-of select="//FontSize"/></xsl:attribute>						    
						    <xsl:attribute name="COLOR">#D4D0C8</xsl:attribute>                            
	    					<B>.</B>
						</xsl:element>
					</xsl:if>
					<xsl:if test="ID[.!='DescriptionID']">						
						<xsl:element name="FONT">
                            <xsl:attribute name="FACE"><xsl:value-of select="//FontName"/></xsl:attribute>
                            <xsl:attribute name="STYLE"><xsl:value-of select="//FontSize"/></xsl:attribute>						    
							<B><xsl:value-of select="Caption"/> </B>
						</xsl:element>
					</xsl:if>
				</TD>
			</xsl:if>
		</xsl:for-each>
		</TR>
	</xsl:template>

	<xsl:template name="ProcessKpiLines">
		<xsl:param name="pIndent">0</xsl:param>
		<xsl:for-each select="./KpiLines/KpiLine">
				<xsl:choose>
					<xsl:when test="count(./KpiLines/KpiLine) > 0">
						<xsl:choose>
						<xsl:when test="./IsFolder='true'"> 						
						<TR>
							<TD style="border:0" COLSPAN="7" HEIGHT="24">								
								<xsl:choose>
									<xsl:when test="$pIndent='0'">
										<xsl:attribute name="BGCOLOR">#A9B2CA</xsl:attribute>
									</xsl:when>
									<xsl:when test="$pIndent='1'">
										<xsl:attribute name="BGCOLOR">#C0C7D9</xsl:attribute>
									</xsl:when>
									<xsl:when test="$pIndent &gt; '1'">
										<xsl:attribute name="BGCOLOR">#D5DAE6</xsl:attribute>
									</xsl:when>
								</xsl:choose>
								
								<xsl:element name="IMG">
									<xsl:attribute name="SRC">.\images\empty.gif</xsl:attribute>
									<xsl:attribute name="ALIGN">absMiddle</xsl:attribute>
									<xsl:attribute name="WIDTH"><xsl:value-of select="$pIndent*20"/></xsl:attribute>
									<xsl:attribute name="HEIGHT">1</xsl:attribute>
								</xsl:element>
								
								<xsl:element name="IMG">
									<xsl:attribute name="SRC">.\images\folder.gif</xsl:attribute>									
									<xsl:attribute name="ALIGN">absMiddle</xsl:attribute>
									<xsl:attribute name="HSPACE">5</xsl:attribute>
								</xsl:element>
								<xsl:element name="FONT">
                                    <xsl:attribute name="FACE"><xsl:value-of select="//FontName"/></xsl:attribute>
                                    <xsl:attribute name="STYLE"><xsl:value-of select="//FontSize"/></xsl:attribute>	
									<xsl:value-of select="Caption"/>
								</xsl:element>										
							</TD>
						</TR>
						</xsl:when>
						<xsl:otherwise>
							<xsl:element name="IMG">
								<xsl:attribute name="SRC">.\images\empty.gif</xsl:attribute>
								<xsl:attribute name="ALIGN">absMiddle</xsl:attribute>
								<xsl:attribute name="WIDTH"><xsl:value-of select="$pIndent*20"/></xsl:attribute>
								<xsl:attribute name="HEIGHT">1</xsl:attribute>
							</xsl:element>
							<TR>
								<xsl:call-template name="ProcessKpiColumns">
									<xsl:with-param name="pIndent"><xsl:value-of select="$pIndent"/></xsl:with-param>
								</xsl:call-template>
							</TR>
						</xsl:otherwise>
						</xsl:choose>
												
						<xsl:call-template name="ProcessKpiLines">
							<xsl:with-param name="pIndent"><xsl:value-of select="$pIndent+1"/></xsl:with-param>
						</xsl:call-template>
												
					</xsl:when>
					<xsl:otherwise>
					<TR>
						<xsl:call-template name="ProcessKpiColumns">
							<xsl:with-param name="pIndent"><xsl:value-of select="$pIndent"/></xsl:with-param>
						</xsl:call-template>
					</TR>
					</xsl:otherwise>
				</xsl:choose>
		</xsl:for-each>
	</xsl:template>

	<xsl:template name="ProcessKpiColumns">
		<xsl:param name="pIndent">0</xsl:param>
		<xsl:for-each select="./Columns/Column">
			<xsl:if test="ID[.!='StatusGraphicID'] and ID[.!='TrendGraphicID']">
				<TD style="border:0" HEIGHT="24">

				<xsl:if test="ID[.!='StructureID']">
					<xsl:choose>
					    <xsl:when test="ID[.='ValueID'] or ID[.='GoalID']">
					        <xsl:attribute name="ALIGN">Left</xsl:attribute>
						</xsl:when>
						<xsl:otherwise>
                            <xsl:attribute name="ALIGN">Center</xsl:attribute>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:if>

				<xsl:if test="ID[.='StructureID']">
					<xsl:attribute name="ALIGN">Left</xsl:attribute>
					
					<xsl:element name="IMG">
						<xsl:attribute name="SRC">.\images\empty.gif</xsl:attribute>
						<xsl:attribute name="ALIGN">absMiddle</xsl:attribute>
						<xsl:attribute name="WIDTH"><xsl:value-of select="$pIndent*20"/></xsl:attribute>
						<xsl:attribute name="HEIGHT">0</xsl:attribute>
					</xsl:element>

					<xsl:element name="IMG">				
						<xsl:attribute name="SRC">.\images\kpi_icon.gif</xsl:attribute>						
						<xsl:attribute name="ALIGN">absMiddle</xsl:attribute>
						<xsl:attribute name="HSPACE">5</xsl:attribute>
					</xsl:element>
				</xsl:if>
				
				<xsl:if test="ID[.='DescriptionID']">
                    <xsl:if test="Value[.!='']">
					    <xsl:choose>
						    <xsl:when test="../../ShowError='true'">
							    <!-- Show Error Message when there is an exception -->
							    <xsl:element name="IMG">
								    <xsl:attribute name="SRC">.\images\error.gif</xsl:attribute>
								    <xsl:attribute name="ALT"><xsl:value-of select="../Column[ID='DescriptionID']/Value"/></xsl:attribute>
								    <xsl:attribute name="ALIGN">absMiddle</xsl:attribute>
							    </xsl:element>							
						    </xsl:when>
						    <xsl:otherwise>
							    <!-- Show description when available -->							
							    <xsl:element name="IMG">
								    <xsl:attribute name="SRC">.\images\info.gif</xsl:attribute>
								    <xsl:attribute name="ALT"><xsl:value-of select="../Column[ID='DescriptionID']/Value"/></xsl:attribute>
								    <xsl:attribute name="ALIGN">absMiddle</xsl:attribute>
							    </xsl:element>
						    </xsl:otherwise>
					    </xsl:choose>
                    </xsl:if>
				</xsl:if>

				<!-- Show the indicators only when the KPI is valid. Always show the KPI name -->
				<xsl:if test="(../Column[ID='ValueID']/Value[.!='']) or ID[.='StructureID']">
					<xsl:if test="ID[.='StatusID'] and ../Column[ID='StatusGraphicID']/Value[.!=''] and Value[.!='']">
						<xsl:element name="IMG">
							<xsl:attribute name="SRC">
								<xsl:value-of select="KPI:GetGfxFileName( string(../Column[ID='StatusGraphicID']/Value), number(Value) )" />
							</xsl:attribute>	
						    <xsl:attribute name="ALT"><xsl:value-of select="../Column[ID='StatusID']/Value"/></xsl:attribute>					
						</xsl:element>
					</xsl:if>

					<xsl:if test="ID[.='TrendID'] and ../Column[ID='TrendGraphicID']/Value[.!=''] and Value[.!='']">
						<xsl:element name="IMG">				
							<xsl:attribute name="SRC">
								<xsl:value-of select="KPI:GetGfxFileName( string(../Column[ID='TrendGraphicID']/Value), number(Value) )" />
							</xsl:attribute>
							<xsl:attribute name="ALT"><xsl:value-of select="../Column[ID='TrendID']/ColumnValueToolTip"/></xsl:attribute>							
						</xsl:element>
					</xsl:if>

					<xsl:element name="FONT">
						<xsl:attribute name="STYLE"><xsl:value-of select="//FontSize"/></xsl:attribute>
						<xsl:attribute name="FACE"><xsl:value-of select="//FontName"/></xsl:attribute>
						<xsl:attribute name="COLOR">#000000</xsl:attribute>
						<xsl:if test="ID[.!='DescriptionID'] and ID[.!='TrendID'] and ID[.!='StatusID']">
							<xsl:value-of select="Value"/>
						</xsl:if>
					</xsl:element>
				</xsl:if>
				</TD>
			</xsl:if>
		</xsl:for-each>
	</xsl:template>
</xsl:stylesheet>
