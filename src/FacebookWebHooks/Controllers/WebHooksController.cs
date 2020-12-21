using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using FacebookWebHooks.Entities;
using System.Net.Http;
using FacebookWebHooks.Models;
using System.Net;
using System.Diagnostics;

namespace FacebookWebHooks.Controllers
{
    [Route("api/[controller]")]
    public class WebHooksController : Controller
    {
        FacebookOptions _fbOptions;
        MailOptions _mailOptions;
        ILogger<WebHooksController> _log;
        private readonly IRepository<FacebookAdsLog> _facebookAdsLogRepo;
        private readonly IRepository<FacebookAdsFormData> _facebookAdsFormDataRepo;



        public WebHooksController(IOptions<FacebookOptions> fbOptions, IOptions<MailOptions> mailOptions,
            ILogger<WebHooksController> logger, IRepository<FacebookAdsLog> facebookAdsLogRepo, IRepository<FacebookAdsFormData> facebookAdsFormDataRepo)
        {
            _fbOptions = fbOptions.Value;
            _mailOptions = mailOptions.Value;
            _log = logger;
            _facebookAdsLogRepo = facebookAdsLogRepo;
            _facebookAdsFormDataRepo = facebookAdsFormDataRepo;

        }

        // GET: api/webhooks
        [HttpGet]
        public string Get([FromQuery(Name = "hub.mode")] string hub_mode,
            [FromQuery(Name = "hub.challenge")] string hub_challenge,
            [FromQuery(Name = "hub.verify_token")] string hub_verify_token)
        {
            //string formdatatext = @"{'id':'424257145427680','locale':'tr_TR','name':'TEST FORM 2','status':'ACTIVE'}";
            //var jsonObjLead = JsonConvert.DeserializeObject<LeadFormData>(formdatatext);

            //string jsontext = @"{'created_time':'2020 - 12 - 16T11: 15:38 + 0000','id':'3511084002456338','field_data':[{'name':'FULL_NAME','values':['\u003Ctest lead: dummy data for FULL_NAME > ']},{'name':'EMAIL','values':['test\u0040fb.com']},{'name':'PHONE','values':['\u003Ctest lead: dummy data for PHONE > ']},{'name':'GENDER','values':['\u003Ctest lead: dummy data for GENDER > ']},{'name':'COMPANY_NAME','values':['\u003Ctest lead: dummy data for COMPANY_NAME > ']}]}";
            //var jsonObjFields = JsonConvert.DeserializeObject<LeadData>(jsontext);
              
            //_facebookAdsLogRepo.Add(new FacebookAdsLog { LogText = "Get Method " });

            if (_fbOptions.VerifyToken == hub_verify_token)
            {

                return hub_challenge;
            }
            else
            {
                return "error. no match";
            }
        }
        #region Post Request

        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody] JsonData data)
        {
            try
            { 
                var entry = data.Entry.FirstOrDefault();
                var change = entry?.Changes.FirstOrDefault();
                if (change == null) return new HttpResponseMessage(HttpStatusCode.BadRequest);
 


                // https://developers.facebook.com/tools/accesstoken/
                //const string token = "1328876420791875|ujQ0AWzbioUypiEY18-qBKkSXas";
                string token = _fbOptions.AppToken;

                var leadUrl = $"https://graph.facebook.com/v2.10/{change.Value.LeadGenId}?access_token={token}";
                var formUrl = $"https://graph.facebook.com/v2.10/{change.Value.FormId}?access_token={token}";
 


                using (var httpClientLead = new HttpClient())
                {
                    var response = await httpClientLead.GetStringAsync(formUrl);
                    if (!string.IsNullOrEmpty(response))
                    {
                        _facebookAdsLogRepo.Add(new FacebookAdsLog { LogText = "jsonObjLead = " + response.ToString() });
                        var jsonObjLead = JsonConvert.DeserializeObject<LeadFormData>(response);


                        using (var httpClientFields = new HttpClient())
                        {
                            var responseFields = await httpClientFields.GetStringAsync(leadUrl);
                            if (!string.IsNullOrEmpty(responseFields))
                            {
                                _facebookAdsLogRepo.Add(new FacebookAdsLog { LogText = "jsonObjFields = " + responseFields.ToString() });
                                var jsonObjFields = JsonConvert.DeserializeObject<LeadData>(responseFields);
                                try
                                {
                                    FacebookAdsFormData formData = new FacebookAdsFormData();
                                    formData.PageId = jsonObjLead.Id;
                                    formData.FormName = jsonObjLead.Name;
                                    formData.CreateTime = DateTime.Now;
                                    foreach (var item in jsonObjFields.FieldData)
                                    {
                                        if (item.Name == "FULL_NAME")
                                            formData.FullName = item.Values?[0];
                                        if (item.Name == "EMAIL")
                                            formData.Email = item.Values?[0];
                                        if (item.Name == "PHONE")
                                            formData.Phone = item.Values?[0];

                                    }
                                    _facebookAdsFormDataRepo.Add(formData);

                                }
                                catch (Exception ex)
                                {
                                   


                                }

                            }
                        }
                    }
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error-->{ex.Message}");
                Trace.WriteLine($"StackTrace-->{ex.StackTrace}");
                return new HttpResponseMessage(HttpStatusCode.BadGateway);
            }
        }

        #endregion Post Request




    }
}
