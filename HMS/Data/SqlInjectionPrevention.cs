namespace HMS.Data
{
    public class SqlInjectionPrevention
    {
        /// <summary>
        /// Returns true if the string is suspicious.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool CheckString(string value) 
        {
            List<string> keywords = new List<string>()
            {
                "alter",
                "begin",
                "break",
                "checkpoint",
                "commit",
                "create",
                "cursor",
                "dbcc",
                "deny",
                "drop",
                "exec",
                "execute",
                "insert",
                "go",
                "grant",
                "opendatasource",
                "openquery",
                "openrowset",
                "shutdown",
                "sp_",
                "tran",
                "transaction",
                "update",
                "while",
                ";",
                "_",
                "xp_",
                "insert",
                "declare",
                "select",
                "*",
                "where",
                "values",
                "count"
            };

            if (keywords.Contains(value.ToLower())) return true;
            else return false;
        } 
    }
}
