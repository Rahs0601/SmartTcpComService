// Ignore Spelling: Vapour

using System;
using System.Data.SqlClient;

namespace SmartTcpComService
{
    internal static class DataBaseAcess
    {

        public static void InsertIntoDB(string Date, string MixingRatio, string Temperature, string RelativeHumidity, string DewPoint, string VapourPressure, string AbsoluteHumidity, string wetBulbTemperature, string RequestTime)
        {

            SqlConnection conn = null;
            try
            {
                conn = ConnectionManager.GetConnection();
                //"230606123535" --> "`2023-06-06 12:35:35"
                Date = getDateTime(Date).ToString("yyyy-MM-dd HH:mm:ss");
                RequestTime = DateTime.Parse(RequestTime).ToString("yyyy-MM-dd HH:mm:ss");
                //813-- 81.3-- Temperature
                Temperature = (Convert.ToDouble(Temperature) / 10).ToString();
                //127-- 12.7-- Mixing Ratio
                MixingRatio = (Convert.ToDouble(MixingRatio) / 10).ToString();
                //580-- 58.0-- Relative Humidity
                RelativeHumidity = (Convert.ToDouble(RelativeHumidity) / 10).ToString();
                //153 / 133-- 15.3 / 13.3-- Absolute Humidity
                AbsoluteHumidity = (Convert.ToDouble(AbsoluteHumidity) / 10).ToString();
                //651-- 65.1-- Dew point
                DewPoint = (Convert.ToDouble(DewPoint) / 10).ToString();
                //702-- 70.2-- wet bulb Temperature
                wetBulbTemperature = (Convert.ToDouble(wetBulbTemperature) / 10).ToString();
                //2117-- 21.17-- vapourpressure
                VapourPressure = (Convert.ToDouble(VapourPressure) / 100).ToString();

                string insertQry = "Insert into TCPCOMPTABLE(Date,MixingRatio, Temperature, RelativeHumidity, DewPoint, VapourPressure, AbsoluteHumidity, wetBulbTemperature , RequestTime) values(@Date,@MixingRatio, @Temperature, @RelativeHumidity, @DewPoint, @VapourPressure, @AbsoluteHumidity, @wetBulbTemperature, @RequestTime)";
                using (SqlCommand cmd = new SqlCommand(insertQry, conn))
                {
                    _ = cmd.Parameters.AddWithValue("@Date", Date);
                    _ = cmd.Parameters.AddWithValue("@MixingRatio", MixingRatio);
                    _ = cmd.Parameters.AddWithValue("@Temperature", Temperature);
                    _ = cmd.Parameters.AddWithValue("@RelativeHumidity", RelativeHumidity);
                    _ = cmd.Parameters.AddWithValue("@DewPoint", DewPoint);
                    _ = cmd.Parameters.AddWithValue("@VapourPressure", VapourPressure);
                    _ = cmd.Parameters.AddWithValue("@AbsoluteHumidity", AbsoluteHumidity);
                    _ = cmd.Parameters.AddWithValue("@wetBulbTemperature", wetBulbTemperature);
                    _ = cmd.Parameters.AddWithValue("@RequestTime", RequestTime);
                    _ = cmd.ExecuteNonQuery();
                    Logger.WriteExtraLog("Data Inserted into DB");
                }

            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error in Inserting Data into DB" + ex.Message);
            }
            finally
            {
                conn?.Close();
            }

        }

        private static DateTime getDateTime(string input)
        {
            int year = int.Parse("20" + input.Substring(0, 2));
            int month = int.Parse(input.Substring(2, 2));
            int day = int.Parse(input.Substring(4, 2));
            int hour = int.Parse(input.Substring(6, 2));
            int minute = int.Parse(input.Substring(8, 2));
            int second = int.Parse(input.Substring(10, 2));

            DateTime dateTime = new DateTime(year, month, day, hour, minute, second);
            return dateTime;

        }
    }
}
