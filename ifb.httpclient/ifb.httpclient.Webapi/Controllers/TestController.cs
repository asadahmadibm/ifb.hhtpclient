using ifb.httpclient.Webapi.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;

namespace ifb.httpclient.Webapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        protected IHttpService HttpService { get; }
        private readonly IConfiguration _config;
        public TestController(IConfiguration config)
        {
            _config = config;
            HttpService = new BaseService(new System.Net.Http.HttpClient { BaseAddress = new Uri(_config.GetValue<string>("AppSetting:MefaBaseAddress"))});
            //HttpService.Headers.Add("content-type", "application/json");
        }
        //public async Task<MefaTokenOutputViewModel> GetTokenBySpecifiedInfo()
        //{
        //    var token = await GetToken(new MefaTokenInputViewModel
        //    {
        //        username = "ifb_user_ir",
        //        password = "Ifb@Iservice#!09!",
        //        client_id = "FCCBUEXJSDSSPXFGLFZJYKCHJDRFRKCN",
        //        client_secret = "24819997461a73c3928c5c1086625696"
        //    });

        //    return token;
        //}

        /// <summary>
        /// دریافت توکن از درگاه ملی
        /// </summary>
        [HttpPost(Name = "GetToken")]
        public async Task<MefaTokenOutputViewModel> GetToken(MefaTokenInputViewModel mefaTokenViewModel)
        {
            HttpService.RequestUri = "token";

            var result =
                await HttpService.PostAsync<MefaTokenInputViewModel, MefaTokenOutputViewModel>(mefaTokenViewModel);

            return result;
        }

    
    }
}
