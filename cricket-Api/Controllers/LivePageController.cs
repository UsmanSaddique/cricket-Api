using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace cricket_Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LivePageController : ControllerBase
    {
        public LivePageController()
        {

        }

        [HttpGet("/getlivescore")]
        public async Task<List<LiveHomePageMatchOutputDto>> GetAsync()
        {
            var url = "https://www.cricbuzz.com/";
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(url);
            var node = doc.DocumentNode.SelectSingleNode("//div[contains(@id,'match_menu_container')]");
            var matchNodes = node.SelectSingleNode("//ul[contains(@class,'videos-carousal-wrapper')]").ChildNodes.Where(c => c.InnerHtml.Trim() != "").ToList();
            List<LiveHomePageMatchOutputDto> matchOutput = new List<LiveHomePageMatchOutputDto>();
            foreach (var item in matchNodes)
            {
                LiveHomePageMatchOutputDto liveHome = new LiveHomePageMatchOutputDto();
                var atag = item.ChildNodes.Where(c => c.Name == "a").FirstOrDefault();
                liveHome.Title = atag?.Attributes["title"].Value;
                liveHome.Url = atag?.Attributes["href"].Value;
                liveHome.Url = liveHome.Url?.Replace("-scores", "-scorecard");

                var nextDiv = item.SelectSingleNode(".//div[contains(@class,'cb-pos-rel')]");
                var dataDivs = nextDiv.SelectNodes("./div/div");
                liveHome.TeamOne = dataDivs[0]?.SelectNodes("./div")?.FirstOrDefault()?.InnerHtml;
                liveHome.TeamOneScore = dataDivs[0]?.SelectNodes("./div")?.LastOrDefault()?.InnerHtml;
                if (string.IsNullOrEmpty(liveHome.TeamOne))
                {
                    liveHome.TeamOne = dataDivs[0]?.InnerHtml;
                }
                liveHome.TeamTwo = dataDivs[1]?.SelectNodes("./div")?.FirstOrDefault()?.InnerHtml;
                liveHome.TeamTwoScore = dataDivs[1]?.SelectNodes("./div")?.LastOrDefault()?.InnerHtml;
                if (string.IsNullOrEmpty(liveHome.TeamTwo))
                {
                    liveHome.TeamTwo = dataDivs[1]?.InnerHtml;
                }
                liveHome.Results = dataDivs[2]?.InnerHtml;
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

        [HttpPost("/getlivescoredetail")]
        public async Task<List<LiveHomePageMatchDetailOutputDto>> GetDetailAsync([FromBody] string nextPageUrl)
        {
            var url = string.Concat("https://www.cricbuzz.com/", nextPageUrl);
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(url);
            List<LiveHomePageMatchDetailOutputDto> matchOutput = new List<LiveHomePageMatchDetailOutputDto>();
            var nodes = doc.DocumentNode.SelectSingleNode("//div[contains(@id,'page-wrapper')]").ChildNodes;
            var dataStreamNode = nodes.Where(c => c.Name == "div" && c.Attributes["class"].Value.Equals("cb-col cb-col-100 cb-bg-white")).FirstOrDefault();
            var dataStreamChildNodes = dataStreamNode.ChildNodes.Where(c => c.Name == "div").ToList();
            var headerNode = dataStreamChildNodes[0];
            var bodyNode = dataStreamChildNodes[1];
            //header Details
            var tourDetail = headerNode.ChildNodes.Where(c => c.Name == "div").LastOrDefault()?.ChildNodes.Where(c => c.Name == "a").ToList();
            var tourName = tourDetail[0].InnerText;
            var tourVenue = tourDetail[1].InnerText;
            var Title = dataStreamNode.SelectSingleNode(".//h1").InnerText;
            //header end

            //Body details
            var status = bodyNode.SelectSingleNode(".//div[contains(@class,'cb-scrcrd-status')]").InnerText;
            var inningsOne = bodyNode.SelectSingleNode(".//div[contains(@id,'innings_1')]");
            var inningsTwo = bodyNode.SelectSingleNode(".//div[contains(@id,'innings_2')]");
            var InningsOneBatterNodes = inningsOne?.ChildNodes?.Where(c => c.Name == "div")?.FirstOrDefault()?.SelectNodes(".//div[contains(@class,'cb-scrd-itms')]")?.ToList();
            var InnningsOneStatus = inningsOne?.ChildNodes?.Where(c => c.Name == "div")?.FirstOrDefault()?.ChildNodes?.Where(c => c.Name == "div")?.FirstOrDefault()?.InnerText;
            foreach (var batter in InningsOneBatterNodes)
            {
                var endBatterNodes = batter.ChildNodes.Where(c => c.Name == "div").ToList();
                if (endBatterNodes.Count() > 5)
                {
                    var BatterName = endBatterNodes[0].InnerText;
                    var BattingStatus = endBatterNodes[1].InnerText;
                    var BatterRuns = endBatterNodes[2].InnerText;
                    var BatterBalls = endBatterNodes[3].InnerText;
                    var BatterFours = endBatterNodes[4].InnerText;
                    var BatterSixs = endBatterNodes[5].InnerText;
                    var BatterStrikeRate = endBatterNodes[6].InnerText;
                }
                else
                {
                    var TipOne = endBatterNodes[0].InnerText;
                    var TipTwo = endBatterNodes[1].InnerText;
                    var TipThree = TipOne != " Did not Bat " && TipOne!= " Yet to Bat " ? endBatterNodes[2]?.InnerText : "";
                }
            }
            var InningsOneBowlerNodes = inningsOne?.ChildNodes?.Where(c => c.Name == "div")?.Take(4).LastOrDefault()?.SelectNodes(".//div[contains(@class,'cb-scrd-itms')]")?.ToList();
            foreach (var bowler in InningsOneBowlerNodes)
            {
                var endBowlerNodes = bowler.ChildNodes.Where(c => c.Name == "div").ToList();
                if (endBowlerNodes.Count() > 5)
                {
                    var BowlerName = endBowlerNodes[0].InnerText;
                    var BowlergOvers = endBowlerNodes[1].InnerText;
                    var BowlerMaiden= endBowlerNodes[2].InnerText;
                    var BowlerRuns = endBowlerNodes[3].InnerText;
                    var BowlerWicket = endBowlerNodes[4].InnerText;
                    var BowlerNoBall = endBowlerNodes[5].InnerText;
                    var BowlerWide = endBowlerNodes[6].InnerText;
                    var BowlerEco = endBowlerNodes[7].InnerText;
                }
               
            }
            //body ends

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
    public class LiveHomePageMatchDetailOutputDto
    {


    }
}
