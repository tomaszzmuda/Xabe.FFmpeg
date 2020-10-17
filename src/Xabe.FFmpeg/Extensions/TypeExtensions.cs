using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public static class Extensions
    {
        public static int ToInt(this string obj)
        {
            try
            {
                if (int.TryParse(obj.ToString(), out int result))
                {
                    return result;
                }
                else return 0;
            }
            catch (Exception ex)
            {
                
                return 0;
            }
        }
    }
}
