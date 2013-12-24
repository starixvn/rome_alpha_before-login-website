/*
 * Created 20091224@Lee - Db types definitions
 * Revision:
 * 20091224@Lee - Initializing
 * 
 * !!!CAUTION: THIS IS GLOBAL DEFINITIONS, ANY REVISION MUST BE APPROVED BEFORE PROCEEDING!!!
 * Please refer to <document url> for more details
 * 
 * 
 */

public class DbTypes
{
    private static string main = "main";
    private static string archive = "archive";
    private static string onUser = "on_user";
    private static string wareHouse = "warehouse";
    private static string message = "MessageDB";
    private static string maintenance = "maintenance";
    private static string dbLog = "dblog";
    private static string db03 = "db03";

    public static string MESSAGE
    {
        get
        {
            return message;
        }
    }

    public static string MAIN
    {
        get
        {
            return main;
        }
    }

    public static string ARCHIVE
    {
        get
        {
            return archive;
        }
    }

    public static string ON_USER
    {
        get
        {
            return onUser;
        }
    }

    public static string WAREHOUSE
    {
        get
        {
            return wareHouse;
        }
    }

    public static string DBLOG
    {
        get
        {
            return dbLog;
        }
    }

    public static string DB03
    {
        get
        {
            return db03;
        }
    }

    public static string Maintenance
    {
        get
        {
            return maintenance;
        }
    }
}
