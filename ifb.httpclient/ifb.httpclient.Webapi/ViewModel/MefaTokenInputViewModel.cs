namespace ifb.httpclient.Webapi.ViewModel
{
    public class MefaTokenInputViewModel
    {
        //  کد client_id برای استفاده در احراز هویت
        public string client_id { get; set; }

        //  کد client_secret برای استفاده در احراز هویت
        public string client_secret { get; set; }

        //نام کاربری اختصاصی دستگاه تخصصی در درگاه ملی مجوزهای کشور
        public string username { get; set; }

        //رمز عبور مخصوص دستگاه تخصصی در درگاه ملی مجوزهای کشور
        public string password { get; set; }
    }
}
