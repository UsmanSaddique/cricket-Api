using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace cricket_Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LivePageController  : ControllerBase
    {
        public LivePageController()
        {

        }

        [HttpGet("/getlivescore")]
        public async Task<List<LiveHomePageMatchOutputDto>> GetAsync()
        {
            var url = "https://www.cricbuzz.com/";
            var web = new HtmlWeb();
            var doc =await web.LoadFromWebAsync(url);
            var node = doc.DocumentNode.SelectSingleNode("//div[contains(@id,'match_menu_container')]");
            var matchNodes = node.SelectSingleNode("//ul[contains(@class,'videos-carousal-wrapper')]").ChildNodes.Where(c=>c.InnerHtml.Trim()!="").ToList();
            List< LiveHomePageMatchOutputDto > matchOutput = new List< LiveHomePageMatchOutputDto >();
            foreach (var item in matchNodes)
            {
                LiveHomePageMatchOutputDto liveHome = new LiveHomePageMatchOutputDto();
                var atag = item.ChildNodes.Where(c=>c.Name=="a").FirstOrDefault();
                liveHome.Title = atag.Attributes["title"].Value;
                liveHome.Url = atag.Attributes["href"].Value;
                var nextDiv = item.SelectSingleNode(".//div[contains(@class,'cb-pos-rel')]");
                var dataDivs = nextDiv.SelectNodes("./div/div");
                liveHome.TeamOne=dataDivs[0]?.SelectNodes("./div")?.FirstOrDefault()?.InnerHtml;
                liveHome.TeamOneScore  =dataDivs[0]?.SelectNodes("./div")?.LastOrDefault()?.InnerHtml;
                if (string.IsNullOrEmpty(liveHome.TeamOne))
                {
                    liveHome.TeamOne = dataDivs[0]?.InnerHtml;
                }
                liveHome.TeamTwo  = dataDivs[1]?.SelectNodes("./div")?.FirstOrDefault()?.InnerHtml;
                liveHome.TeamTwoScore  = dataDivs[1]?.SelectNodes("./div")?.LastOrDefault()?.InnerHtml;
                if (string.IsNullOrEmpty(liveHome.TeamTwo))
                {
                    liveHome.TeamTwo = dataDivs[1]?.InnerHtml;
                }
                liveHome.Results  = dataDivs[2]?.InnerHtml;
                if (string.IsNullOrEmpty(liveHome.Results))
                {
                    liveHome.DateTime = dataDivs[3]?.Attributes["ng-bind"]?.Value;
                    if (!string.IsNullOrEmpty(liveHome.DateTime))
                    {
                        liveHome.DateTime = Regex.Match(liveHome.DateTime, @"\d+").Value;

                    }
                }

                matchOutput.Add(liveHome);

            }
            return matchOutput;
        }
     

    }
    public class LiveHomePageMatchOutputDto
    {
        public string? Title { get; set; } = String.Empty;
        public string? Url { get; set; } = String.Empty;
        public string? TeamOne { get; set; } = String.Empty;
        public string? TeamOneScore { get; set; } = String.Empty;
        public string? TeamTwo { get; set; } = String.Empty;
        public string? TeamTwoScore { get; set; } = String.Empty;
        public string? Results { get; set; } = String.Empty;
        public string? DateTime { get; set; } = String.Empty;

    }
}
