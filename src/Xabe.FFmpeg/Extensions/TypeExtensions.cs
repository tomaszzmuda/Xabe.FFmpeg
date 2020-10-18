using System;
using System.Collections.Generic;
using System.Text;
using Xabe.FFmpeg;

namespace System
{
    public static class Extensions
    {
        public static string ToVSync(this VsyncParams vsync)
        {
            string result = "-1";
            switch (vsync)
            {
                case VsyncParams.passthrough:
                    {
                        result = "0";
                        break;
                    }           
                case VsyncParams.cfr:
                    {
                        result = "1";
                        break;
                    }
                case VsyncParams.vfr:
                    {
                        result = "2";
                        break;
                    }
   
                case VsyncParams.drop:
                    {
                        result = "drop";
                        break;
                    }
                case VsyncParams.auto:
                    {
                        result = "-1";
                        break;
                    }
            }
            return result;
        }

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
