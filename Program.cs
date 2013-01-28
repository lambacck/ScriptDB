/*
 * Copyright 2006 Jesse Hersch
 *
 * Permission to use, copy, modify, and distribute this software
 * and its documentation for any purpose is hereby granted without fee,
 * provided that the above copyright notice appears in all copies and that
 * both that copyright notice and this permission notice appear in
 * supporting documentation, and that the name of Jesse Hersch or
 * Elsasoft LLC not be used in advertising or publicity
 * pertaining to distribution of the software without specific, written
 * prior permission.  Jesse Hersch and Elsasoft LLC make no
 * representations about the suitability of this software for any
 * purpose.  It is provided "as is" without express or implied warranty.
 *
 * Jesse Hersch and Elsasoft LLC disclaim all warranties with
 * regard to this software, including all implied warranties of
 * merchantability and fitness, in no event shall Jesse Hersch or
 * Elsasoft LLC be liable for any special, indirect or
 * consequential damages or any damages whatsoever resulting from loss of
 * use, data or profits, whether in an action of contract, negligence or
 * other tortious action, arising out of or in connection with the use or
 * performance of this software.
 *
 * Author:
 *  Jesse Hersch
 *  Elsasoft LLC
 * 
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using Utility;

namespace Elsasoft.ScriptDb
{
    class Program
    {
        static void Main(string[] args)
        {
            Utility.Arguments arguments = new Arguments(args);
            try
            {
                if (arguments["?"] != null)
                {
                    PrintHelp();
                    return;
                }

                string connStr = arguments["con"];
                string outputDirectory = arguments["outDir"];
                bool scriptData = arguments["d"] != null;
                bool verbose = arguments["v"] != null;
                bool scriptProperties = arguments["p"] != null;
                bool Purge = arguments["Purge"] != null;
                bool scriptAllDatabases = arguments["ScriptAllDatabases"] != null;

                if (connStr == null || outputDirectory == null)
                {
                    PrintHelp();
                    return;
                }
                string database = null;
                using (SqlConnection sc = new SqlConnection(connStr))
                {
                    database = sc.Database;
                }

                // Purge at the Server level only when we're doing all databases
                if (Purge && scriptAllDatabases && Directory.Exists(outputDirectory))
                    PurgeDirectory(outputDirectory, "*.sql");

                if (!Directory.Exists(outputDirectory)) Directory.CreateDirectory(outputDirectory);

                DatabaseScripter ds = new DatabaseScripter();

                if (arguments["table"] != null)
                    ds.TableFilter = arguments["table"].Split(',');
                if (arguments["view"] != null)
                    ds.ViewsFilter = arguments["view"].Split(',');
                if (arguments["sp"] != null)
                    ds.SprocsFilter = arguments["sp"].Split(',');
                if (arguments["TableOneFile"] != null)
                    ds.TableOneFile = true;
                if (arguments["ScriptAsCreate"] != null)
                    ds.ScriptAsCreate = true;
                if (arguments["Permissions"] != null)
                    ds.Permissions = true;
                if (arguments["NoCollation"] != null)
                    ds.NoCollation = true;
                if (arguments["IncludeDatabase"] != null)
                    ds.IncludeDatabase = true;
                if (arguments["CreateOnly"] != null)
                    ds.CreateOnly = true;
                if (arguments["filename"] != null)
                    ds.OutputFileName = arguments["filename"];
                ds.GenerateScripts(connStr, outputDirectory, scriptAllDatabases, Purge, scriptData, verbose, scriptProperties);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught in Main()");
                Console.WriteLine("---------------------------------------");
                while (e != null)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine();
                    Console.WriteLine(e.GetType().FullName);
                    Console.WriteLine();
                    Console.WriteLine(e.StackTrace);
                    Console.WriteLine("---------------------------------------");
                    e = e.InnerException;
                }
            }
        }


        public static void PurgeDirectory(string DirName, string FileSpec)
        {
            string FullPath = Path.GetFullPath(DirName);
            try
            {
                //s = string.Format(@"/c rmdir ""{0}"" /s /q", Path.GetFullPath(outputDirectory));
                //System.Diagnostics.Process.Start(@"c:\windows\system32\cmd.exe", s);

                // Remove flags from all files in the current directory
                foreach (string s in Directory.GetFiles(FullPath, FileSpec, SearchOption.AllDirectories))
                {
                    // skip files inside .svn folders (although these might be skipped regardless
                    // since they have a hidden attribute) 
                    if (!s.Contains(@"\.svn\"))
                    {
                        FileInfo file = new FileInfo(s);
                        file.Attributes = FileAttributes.Normal;
                        file.Delete();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception {0} : {1}", e.Message, FullPath);
            };
        }



        private static void PrintHelp()
        {
            Console.WriteLine(
@"ScriptDb.exe usage:

ScriptDb.exe 
    ConnectionString 
    OutputDirectory
    [-d]
    [-v]
    [-p]
    [-table:table1,table2] [-TableOneFile] 
    [-view:view1,view2] 
    [-sp:sp1,sp2] 
    [-ScriptAsCreate] 
    [-ScriptAllDatabases]
    [-Permissions] 
    [-NoCollation]
    [-CreateOnly]
    [-Purge]
    [-filename:<FileName> | -]

-con:<ConnectionString> is a connection string to the db.
-outDir:<OutputDirectory> is where the output scripts are placed.
-d script data to files for importing with bcp
-v for verbose output.
-p to script extended properties for each object.
-table - comma separated list of tables to script
-TableOneFile - script table definition into one file instad of multiple
-view - comma separated list of views to script
-sp - comma separated list of stored procedures to script
-ScriptAsCreate - script stored procedures as CREATE instead ALTER
-ScriptAllDatabases - script all databases on the current server
-IncludeDatabase - Include Database Context in scripted objects
-CreateOnly - Do not generate DROP statements
-Purge - ensures output folder is emptied of all files before generating scripts
-filename - specify output filename. If file exists, script will be appended to the end of the file
           specify '-' to output to console

Example: 

ScriptDb.exe -con:server=(local);database=pubs;trusted_connection=yes -outDir:scripts [-d] [-v] [-p] [-table:table1,table2] [-TableOneFile] [-view:view1,view2] [-sp:sp1,sp2] [-ScriptAsCreate] [-Permissions] [-NoCollation] [-IncludeDatabase] -filename:-

");
        }

    }
}
