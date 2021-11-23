using System;
using System.Collections.Generic;
using System.Text;

namespace Active8_CancelAllMemberships.Utility
{
    public class SafeConvert
    {

        public static int SafeInt(object value)
        {
            try
            {
                if (value == null)
                {
                    return 0;
                }

                string n = value.ToString();

                if (string.IsNullOrEmpty(n))
                {
                    return 0;
                }

                //Check for negative
                if (n.StartsWith("-"))
                {
                    if (!ValidateNumber(n.Substring(1)))
                    {
                        return 0;
                    }
                }
                else
                {
                    if (!ValidateNumber(value))
                    {
                        return 0;
                    }
                }

                return Convert.ToInt32(value);
            }
            catch
            {
                return 0;
            }
        }

        private static bool ValidateNumber(object value, string exceptionChar = "")
        {
            string validChar = "0123456789" + exceptionChar;

            foreach (char c in value.ToString())
            {
                if (!validChar.Contains(c.ToString()))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
