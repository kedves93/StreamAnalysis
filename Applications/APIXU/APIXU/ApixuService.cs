using APIXULib;

namespace APIXU
{
    public static class ApixuService
    {
        private const string APIXU_KEY = "937a632c5b454b2685094122181411";

        private static APIXUWeatherRepository apixu;

        static ApixuService()
        {
            apixu = new APIXUWeatherRepository();
        }

        public static WeatherModel GetWeatherDataByAutoIP()
        {
            return apixu.GetWeatherDataByAutoIP(APIXU_KEY);
        }
    }
}