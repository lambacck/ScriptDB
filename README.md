ScriptDB
========

ScriptDb is a simple console app written in C# that uses SQL Management Objects (SMO) to script all the objects in a database. It will work against any SQL Server 2000, 2005, 2008 or 2008 R2 databases. It creates a directory tree structure with a similar hierarchy to that in Object Explorer in SSMS, with a separate file for each object.

It will also (optionally) script the data using bcp.exe.

You can also (optionally) specify to script the extended properties for each object along with the DDL.

ScriptDB was originally written by Jesse Hersch of http://www.elsasoft.org.

This version was updated from the version at http://scriptdb.codeplex.com/ codeplex for SQL Server 2008/2008R2 by Chris Lambacher. Also added was Full Text index scripting. Since the owners of the codeplex repo don't seem to be responding to patches I've put this up here to keep my changes in a public place.
